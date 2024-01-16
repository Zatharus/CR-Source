// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.ThumbnailViewItem
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Runtime;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public abstract class ThumbnailViewItem : ItemViewItem, IDisposable
  {
    public const int RowColumnBorder = 2;
    public const int FadeInTime = 300;
    public const int FadeInSteps = 25;
    public const float FontHeightPercent = 0.07f;
    public const float MinFontSize = 0.8f;
    public const float MaxFontSize = 1f;
    public static readonly Bitmap DeletedStateImage = Resources.Deleted;
    private volatile float opacity = 1f;
    private System.Drawing.Size border = new System.Drawing.Size(4, 4);
    private volatile bool disposed;
    private static Timer animationTimer;
    private static readonly LinkedList<ThumbnailViewItem> animatedItems = new LinkedList<ThumbnailViewItem>();
    private static long animationTime;
    private bool validImage = true;
    private List<ThumbnailViewItem.ClickRegion> clickRegions;

    ~ThumbnailViewItem() => this.Dispose(false);

    public float Opacity
    {
      get => this.opacity;
      set
      {
        value = value.Clamp(0.0f, 1f);
        if (this.opacity.CompareTo(value, 0.01f))
          return;
        this.opacity = value;
        this.Update();
      }
    }

    public System.Drawing.Size Border
    {
      get => this.border;
      set
      {
        if (this.border == value)
          return;
        this.border = value;
        this.Update(true);
      }
    }

    public abstract ThumbnailKey ThumbnailKey { get; }

    public bool IsDisposed => this.disposed;

    public void RefreshImage()
    {
      this.OnRefreshImage();
      this.Update(true);
    }

    protected IItemLock<ThumbnailImage> GetThumbnail(ThumbnailKey key, bool memoryOnly)
    {
      IItemLock<ThumbnailImage> image = Program.ImagePool.Thumbs.GetImage((ImageKey) key, memoryOnly);
      if (image == null)
        Program.ImagePool.AddThumbToQueue(key, (object) this.View, (AsyncCallback) (ar => this.MakePageThumbnail(key)));
      return image;
    }

    public IItemLock<ThumbnailImage> GetThumbnail(bool memoryOnly)
    {
      return this.GetThumbnail(this.ThumbnailKey, memoryOnly);
    }

    public IItemLock<ThumbnailImage> GetThumbnail(ItemDrawInformation drawInfo)
    {
      if (drawInfo != null && drawInfo.DisplayType == ItemViewMode.Detail)
      {
        if (drawInfo.Header == null)
          return (IItemLock<ThumbnailImage>) null;
        if (drawInfo.Header.Tag is ComicListField tag && tag.DisplayProperty != "Cover" && tag.DisplayProperty != "Thumbnail")
          return (IItemLock<ThumbnailImage>) null;
      }
      return this.GetThumbnail(true);
    }

    private static void AddAnimation(ThumbnailViewItem item)
    {
      if (ThumbnailViewItem.animationTimer == null)
      {
        ThumbnailViewItem.animationTimer = new Timer(12.0)
        {
          AutoReset = true
        };
        ThumbnailViewItem.animationTimer.Elapsed += new ElapsedEventHandler(ThumbnailViewItem.animationTimer_Elapsed);
      }
      using (ItemMonitor.Lock((object) ThumbnailViewItem.animatedItems))
      {
        if ((double) item.Opacity >= 0.949999988079071 || ThumbnailViewItem.animatedItems.Contains(item))
          return;
        ThumbnailViewItem.animatedItems.AddLast(item);
        if (ThumbnailViewItem.animationTimer.Enabled)
          return;
        ThumbnailViewItem.animationTime = 0L;
        ThumbnailViewItem.animationTimer.Start();
      }
    }

    private static void animationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      using (ItemMonitor.Lock((object) ThumbnailViewItem.animatedItems))
      {
        long ticks = Machine.Ticks;
        long num = ThumbnailViewItem.animationTime == 0L ? 12L : ticks - ThumbnailViewItem.animationTime;
        ThumbnailViewItem.animationTime = ticks;
        LinkedListNode<ThumbnailViewItem> node = ThumbnailViewItem.animatedItems.First;
        bool flag = false;
        while (node != null)
        {
          LinkedListNode<ThumbnailViewItem> next = node.Next;
          try
          {
            ThumbnailViewItem thumbnailViewItem = node.Value;
            if (thumbnailViewItem.View == null || thumbnailViewItem.disposed)
            {
              ThumbnailViewItem.animatedItems.Remove(node);
            }
            else
            {
              thumbnailViewItem.Opacity += (float) num / 300f;
              if ((double) thumbnailViewItem.Opacity >= 1.0)
                ThumbnailViewItem.animatedItems.Remove(node);
              else
                flag = true;
            }
          }
          catch
          {
            ThumbnailViewItem.animatedItems.Remove(node);
          }
          finally
          {
            node = next;
          }
        }
        if (flag)
          return;
        ThumbnailViewItem.animationTimer.Stop();
      }
    }

    public void Animate(Image image)
    {
      if (!Program.Settings.FadeInThumbnails)
        return;
      if (image == null)
      {
        this.validImage = false;
      }
      else
      {
        if (!this.validImage)
        {
          this.opacity = 0.0f;
          ThumbnailViewItem.AddAnimation(this);
        }
        this.validImage = true;
      }
    }

    protected virtual System.Drawing.Size GetDefaultMaximumSize(System.Drawing.Size defaultSize)
    {
      int height = defaultSize.Height;
      return new System.Drawing.Size(height * 2, height);
    }

    protected virtual System.Drawing.Size GetEstimatedSize(System.Drawing.Size canvasSize)
    {
      return System.Drawing.Size.Empty;
    }

    protected System.Drawing.Size GetThumbnailSizeSafe(System.Drawing.Size defaultSize)
    {
      using (IItemLock<ThumbnailImage> image = Program.ImagePool.Thumbs.GetImage((ImageKey) this.ThumbnailKey, true))
      {
        System.Drawing.Size imageSize = image != null ? image.Item.GetThumbnailSize(defaultSize.Height) : System.Drawing.Size.Empty;
        System.Drawing.Size defaultMaximumSize = this.GetDefaultMaximumSize(defaultSize);
        if (imageSize.IsEmpty)
        {
          System.Drawing.Size estimatedSize = this.GetEstimatedSize(defaultMaximumSize);
          if (!estimatedSize.IsEmpty)
            return estimatedSize;
        }
        return ThumbRenderer.GetSafeScaledImageSize(imageSize, defaultMaximumSize);
      }
    }

    protected System.Drawing.Size AddBorder(System.Drawing.Size size)
    {
      return size + this.border + this.border;
    }

    public override void OnDraw(ItemDrawInformation drawInfo)
    {
      base.OnDraw(drawInfo);
      if (drawInfo.SubItem < 0)
      {
        this.clickRegions = (List<ThumbnailViewItem.ClickRegion>) null;
      }
      else
      {
        Rectangle bounds = drawInfo.Bounds;
        if (!drawInfo.DrawBorder)
          return;
        using (Pen pen = new Pen(Color.FromArgb(48, SystemColors.ControlDark), 1f))
          drawInfo.Graphics.DrawLine(pen, bounds.Location, new System.Drawing.Point(bounds.Left, bounds.Bottom));
      }
    }

    protected void MakePageThumbnail(ThumbnailKey key) => this.MakePageThumbnail(key, true);

    protected void MakePageThumbnail(ThumbnailKey key, bool updateSize)
    {
      if (this.disposed)
        return;
      updateSize &= this.View.ItemViewMode == ItemViewMode.Thumbnail;
      if (!Program.ImagePool.Thumbs.IsAvailable((ImageKey) key))
        this.CreateThumbnail(key);
      else
        updateSize = false;
      if (this.View == null)
        return;
      this.Update(updateSize);
    }

    protected abstract void CreateThumbnail(ThumbnailKey key);

    protected virtual void OnRefreshImage()
    {
      Program.ImagePool.Pages.RefreshImage((ImageKey) new PageKey((ImageKey) this.ThumbnailKey));
      Program.ImagePool.Thumbs.RefreshImage((ImageKey) this.ThumbnailKey);
    }

    protected void AddClickRegion(Rectangle bounds, Action<Rectangle, System.Drawing.Point> click)
    {
      this.clickRegions = this.clickRegions.SafeAdd<ThumbnailViewItem.ClickRegion>(new ThumbnailViewItem.ClickRegion()
      {
        Bounds = bounds,
        Click = click
      });
    }

    public override bool OnClick(System.Drawing.Point pt)
    {
      List<ThumbnailViewItem.ClickRegion> clickRegions = this.clickRegions;
      ThumbnailViewItem.ClickRegion clickRegion = clickRegions != null ? clickRegions.FirstOrDefault<ThumbnailViewItem.ClickRegion>((Func<ThumbnailViewItem.ClickRegion, bool>) (c => c.Bounds.Contains(pt))) : (ThumbnailViewItem.ClickRegion) null;
      if (clickRegion == null)
        return false;
      clickRegion.Click(clickRegion.Bounds, pt.Subtract(clickRegion.Bounds.Location));
      return true;
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      try
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
      finally
      {
        this.disposed = true;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private class ClickRegion
    {
      public Rectangle Bounds;
      public Action<Rectangle, System.Drawing.Point> Click;
    }
  }
}
