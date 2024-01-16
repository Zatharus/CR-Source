// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.ImageItem
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class ImageItem : Block
  {
    private Image image;
    private string source;
    private Size padding;
    private Size inflate;
    private VerticalAlignment vAlign;
    private HorizontalAlignment align;

    public ImageItem()
    {
    }

    public ImageItem(string file) => this.image = Image.FromFile(file);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.image != null)
        this.image.Dispose();
      base.Dispose(disposing);
    }

    protected override void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl)
    {
      int size1 = this.BlockWidth.IsAuto ? 0 : this.BlockWidth.GetSize(maxWidth);
      int blockHeight = this.BlockHeight;
      Size size2 = Size.Empty;
      if (this.Image != null)
      {
        if (size1 <= 0 && blockHeight <= 0)
          size2 = this.Image.Size;
        else if (size1 > 0)
          size2 = new Size(size1, this.Image.Height * size1 / this.Image.Width);
        else if (blockHeight > 0)
          size2 = new Size(this.Image.Width * blockHeight / this.Image.Height, blockHeight);
      }
      if (size2.Width < size1)
        size2.Width = size1;
      if (blockHeight != 0)
        size2.Height = blockHeight;
      Size size3 = size2 + (this.inflate + this.inflate);
      this.MinimumWidth = size1 + this.inflate.Width * 2;
      this.Size = size3;
      switch (this.VAlign)
      {
        case VerticalAlignment.Top:
          this.BaseLine = this.Font.Height;
          break;
        case VerticalAlignment.Middle:
          this.BaseLine = (this.Font.Height + size3.Height) / 2 - this.DescentHeight;
          break;
        default:
          this.BaseLine = size3.Height;
          break;
      }
    }

    public override void Draw(Graphics gr, Point location)
    {
      Image image = this.Image;
      if (image == null)
        return;
      Rectangle bounds = this.Bounds;
      bounds.Inflate(-this.padding.Width, -this.padding.Height);
      bounds.Offset(location);
      gr.DrawImage(image, bounds);
    }

    public Image Image
    {
      get
      {
        if (this.image == null && this.source != null)
          this.image = this.GetImage(this.source);
        return this.image;
      }
      set
      {
        if (this.image == value)
          return;
        this.image = value;
        this.OnImageChanged();
      }
    }

    public string Source
    {
      get => this.source;
      set
      {
        if (this.source == value)
          return;
        this.source = value;
        this.image = (Image) null;
        this.OnImageChanged();
      }
    }

    public Size Padding
    {
      get => this.padding;
      set
      {
        if (this.padding == value)
          return;
        this.padding = value;
        this.inflate = new Size(this.padding.Width * 2, this.padding.Height * 2);
        this.OnPaddingChanged();
      }
    }

    public override VerticalAlignment VAlign
    {
      get => this.vAlign;
      set
      {
        if (this.vAlign == value)
          return;
        this.vAlign = value;
        this.OnVAlignChanged();
      }
    }

    public override HorizontalAlignment Align
    {
      get => this.align;
      set
      {
        if (this.align == value)
          return;
        this.align = value;
        this.OnAlignChanged();
      }
    }

    protected virtual void OnImageChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnPaddingChanged() => this.InvokeLayout(LayoutType.Full);
  }
}
