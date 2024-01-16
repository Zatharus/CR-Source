// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.TextureManager
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tao.OpenGl;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  public class TextureManager : DisposableObject
  {
    private readonly LinkedList<TextureManager.TextureElement> textures = new LinkedList<TextureManager.TextureElement>();

    protected override void Dispose(bool disposing)
    {
      this.textures.Dispose();
      base.Dispose(disposing);
    }

    private void CleanUp()
    {
      LinkedListNode<TextureManager.TextureElement> linkedListNode = this.textures.Last;
      int totalMemory = this.GetTotalMemory();
      int num = this.Settings.MaxTextureMemoryMB * 1024 * 1024;
      int count = this.textures.Count;
      while (linkedListNode != null)
      {
        LinkedListNode<TextureManager.TextureElement> node = linkedListNode;
        TextureManager.TextureElement textureElement = node.Value;
        linkedListNode = linkedListNode.Previous;
        if (textureElement.Bitmap == null || !textureElement.Bitmap.IsValid || totalMemory > num || count > this.Settings.MaxTextureCount)
        {
          int memory = textureElement.Memory;
          textureElement.Dispose();
          this.textures.Remove(node);
          totalMemory -= memory;
          --count;
        }
      }
    }

    private int GetTotalMemory()
    {
      return this.textures.Sum<TextureManager.TextureElement>((Func<TextureManager.TextureElement, int>) (te => te.Memory));
    }

    private TextureManager.TextureElement GetTextureElement(RendererImage bmp, int part)
    {
      TextureManager.TextureElement textureElement = new TextureManager.TextureElement(this, bmp, part);
      this.CleanUp();
      LinkedListNode<TextureManager.TextureElement> node = this.textures.Find(textureElement);
      if (node == null)
      {
        node = this.textures.AddFirst(textureElement);
      }
      else
      {
        textureElement.Dispose();
        this.textures.Remove(node);
        this.textures.AddFirst(node);
      }
      return node.Value;
    }

    private TextureManager.TextureElement BindTexturePart(RendererImage bmp, int n)
    {
      TextureManager.TextureElement textureElement = this.GetTextureElement(bmp, n);
      while (this.textures.Count > 0)
      {
        if (textureElement.Bind())
          return textureElement;
        this.textures.Last.Value.Destroy();
        this.textures.RemoveLast();
      }
      return (TextureManager.TextureElement) null;
    }

    public void DrawImage(RendererImage image, RectangleF dest, RectangleF src, float opacity)
    {
      if ((double) opacity < 0.05000000074505806)
        return;
      Gl.glPushMatrix();
      Gl.glPushAttrib(286721);
      try
      {
        Gl.glEnable(3553);
        Gl.glTexEnvf(8960, 8704, 8448f);
        Gl.glColor4f(1f, 1f, 1f, opacity);
        Gl.glTranslatef(dest.X, dest.Y, 0.0f);
        Gl.glScalef(dest.Width / src.Width, dest.Height / src.Height, 0.0f);
        Size textureGridSize = TextureManager.TextureElement.GetTextureGridSize(image, this.Settings.MaxTextureTileSize);
        for (int index1 = 0; index1 < textureGridSize.Height; ++index1)
        {
          for (int index2 = 0; index2 < textureGridSize.Width; ++index2)
          {
            TextureManager.TextureElement textureElement = this.BindTexturePart(image, index1 * textureGridSize.Width + index2);
            if (textureElement != null)
            {
              RectangleF textureTileBounds = (RectangleF) textureElement.TextureTileBounds;
              RectangleF rectangleF1 = RectangleF.Intersect(textureTileBounds, src);
              if (!rectangleF1.IsEmpty)
              {
                RectangleF textureTileCoord = textureElement.TextureTileCoord;
                RectangleF rectangleF2 = new RectangleF(textureTileCoord.X + textureTileCoord.Width * (rectangleF1.X - textureTileBounds.X) / textureTileBounds.Width, textureTileCoord.Y + textureTileCoord.Height * (rectangleF1.Y - textureTileBounds.Y) / textureTileBounds.Height, textureTileCoord.Width * rectangleF1.Width / textureTileBounds.Width, textureTileCoord.Height * rectangleF1.Height / textureTileBounds.Height);
                rectangleF1.X -= src.X;
                rectangleF1.Y -= src.Y;
                Gl.glBegin(7);
                Gl.glTexCoord2f(rectangleF2.Left, rectangleF2.Top);
                Gl.glVertex2f(rectangleF1.Left, rectangleF1.Top);
                Gl.glTexCoord2f(rectangleF2.Right, rectangleF2.Top);
                Gl.glVertex2f(rectangleF1.Right, rectangleF1.Top);
                Gl.glTexCoord2f(rectangleF2.Right, rectangleF2.Bottom);
                Gl.glVertex2f(rectangleF1.Right, rectangleF1.Bottom);
                Gl.glTexCoord2f(rectangleF2.Left, rectangleF2.Bottom);
                Gl.glVertex2f(rectangleF1.Left, rectangleF1.Bottom);
                Gl.glEnd();
              }
            }
          }
        }
      }
      finally
      {
        Gl.glPopAttrib();
        Gl.glPopMatrix();
      }
    }

    public TextureManagerSettings Settings { get; set; }

    public bool IsOptimizedTexture { get; set; }

    public bool EnableFilter { get; set; }

    private class TextureElement : DisposableObject
    {
      private readonly int part;
      private readonly TextureManager manager;
      private readonly int[] name = new int[1];
      private readonly RendererImage bitmap;
      private int memory;
      private Size textureTileGlSize;

      public TextureElement(TextureManager manager, RendererImage bitmap, int part)
      {
        this.manager = manager;
        this.bitmap = bitmap;
        this.part = part;
      }

      protected override void Dispose(bool disposing)
      {
        this.Destroy();
        base.Dispose(disposing);
      }

      public int Name => this.name[0];

      public RendererImage Bitmap => this.bitmap;

      public int Memory => this.memory;

      public Rectangle TextureTileBounds => this.GetTexturePartBounds(this.Bitmap, this.part, true);

      public Size TextureTileGlSize => this.textureTileGlSize;

      public RectangleF TextureTileCoord
      {
        get
        {
          RectangleF textureTileBounds = (RectangleF) this.TextureTileBounds;
          Size textureTileGlSize = this.TextureTileGlSize;
          return new RectangleF(0.0f, 0.0f, textureTileBounds.Width / (float) textureTileGlSize.Width, textureTileBounds.Height / (float) textureTileGlSize.Height);
        }
      }

      public bool Bind()
      {
        using (ItemMonitor.Lock((object) this.Bitmap))
        {
          if (!TextureManager.TextureElement.IsValid(this.Bitmap))
            return false;
          if (this.Name != 0)
          {
            Gl.glBindTexture(3553, this.Name);
          }
          else
          {
            System.Drawing.Bitmap bitmap = this.Bitmap.Bitmap;
            using (ItemMonitor.Lock((object) bitmap))
            {
              try
              {
                Gl.glGenTextures(1, this.name);
                Gl.glBindTexture(3553, this.Name);
                Gl.glTexParameteri(3553, 10242, 33071);
                Gl.glTexParameteri(3553, 10243, 33071);
                Rectangle texturePartBounds = this.GetTexturePartBounds((RendererImage) bitmap, this.part, !this.manager.Settings.IsSquareTextures);
                this.textureTileGlSize = texturePartBounds.Size;
                using (FastBitmapLock fastBitmapLock = new FastBitmapLock(bitmap, texturePartBounds))
                {
                  int num1 = bitmap.Width * bitmap.Height;
                  int num2 = texturePartBounds.Width * texturePartBounds.Height;
                  int internalformat = 6408;
                  this.memory = num2 * 4;
                  if (this.manager.Settings.IsTextureCompression)
                  {
                    internalformat = 34030;
                    this.memory = num2 * 2;
                  }
                  if (this.manager.Settings.IsBigTexturesAs16Bit && num1 > 800000)
                  {
                    internalformat = 32848;
                    this.memory = num2 * 2;
                  }
                  if (this.manager.Settings.IsBigTexturesAs24Bit && num1 > 800000)
                  {
                    internalformat = 32849;
                    this.memory = num2 * 3;
                  }
                  if (this.manager.IsOptimizedTexture)
                  {
                    Gl.glTexParameteri(3553, 10241, 9728);
                    Gl.glTexParameteri(3553, 10240, 9728);
                    if (Gl.glGetError() != 0)
                      Gl.glTexParameteri(3553, 10240, 9729);
                  }
                  else
                  {
                    Gl.glGetTexParameteriv(3553, 33169, out int _);
                    if (Gl.glGetError() != 0 || !this.manager.Settings.IsMipMapFilter || !this.manager.EnableFilter)
                    {
                      Gl.glTexParameteri(3553, 10241, 9729);
                    }
                    else
                    {
                      Gl.glTexParameteri(3553, 33169, 1);
                      Gl.glTexParameteri(3553, 10241, 9987);
                      if (this.manager.Settings.IsAnisotropicFilter)
                      {
                        float[] @params = new float[1];
                        Gl.glGetFloatv(34047, @params);
                        Gl.glTexParameterf(3553, 34046, @params[0]);
                      }
                    }
                    Gl.glTexParameteri(3553, 10240, 9729);
                  }
                  Gl.glTexImage2D(3553, 0, internalformat, fastBitmapLock.Width, fastBitmapLock.Height, 0, 32993, 5121, fastBitmapLock.Data);
                  if (Gl.glGetError() == 1285)
                    throw new InvalidOperationException();
                }
              }
              catch (Exception ex)
              {
                this.Destroy();
                return false;
              }
            }
          }
        }
        return true;
      }

      public void Destroy()
      {
        if (this.Name == 0)
          return;
        try
        {
          Gl.glDeleteTextures(1, this.name);
        }
        catch (Exception ex)
        {
        }
        this.name[0] = this.memory = 0;
      }

      private Rectangle GetTexturePartBounds(RendererImage bmp, int part, bool clamp)
      {
        return !TextureManager.TextureElement.IsValid(bmp) ? Rectangle.Empty : TextureManager.TextureElement.GetPartBounds(bmp.Size, part, this.manager.Settings.MinTextureTileSize, this.manager.Settings.MaxTextureTileSize, clamp);
      }

      private static Rectangle GetPartBounds(
        Size fullSize,
        int part,
        int minPartSize,
        int maxPartSize,
        bool clamp)
      {
        Size gridSize = TextureManager.TextureElement.GetGridSize(fullSize, maxPartSize);
        if (gridSize.IsEmpty)
          return Rectangle.Empty;
        int num1 = part % gridSize.Width;
        int num2 = part / gridSize.Width;
        Rectangle partBounds = new Rectangle(num1 * maxPartSize, num2 * maxPartSize, maxPartSize, maxPartSize);
        int val1_1 = partBounds.Right > fullSize.Width ? fullSize.Width - partBounds.X : partBounds.Width;
        int val1_2 = partBounds.Height > fullSize.Height ? fullSize.Height - partBounds.Y : partBounds.Height;
        if (clamp)
        {
          partBounds.Width = val1_1;
          partBounds.Height = val1_2;
        }
        else
        {
          partBounds.Width = TextureManager.TextureElement.NearestSquare(maxPartSize, Math.Max(val1_1, minPartSize));
          partBounds.Height = TextureManager.TextureElement.NearestSquare(maxPartSize, Math.Max(val1_2, minPartSize));
        }
        return partBounds;
      }

      public override bool Equals(object obj)
      {
        return obj is TextureManager.TextureElement textureElement && this.part == textureElement.part && object.Equals((object) this.Bitmap, (object) textureElement.Bitmap);
      }

      public override int GetHashCode() => this.part;

      public static bool IsValid(RendererImage bmp)
      {
        using (ItemMonitor.Lock((object) bmp))
        {
          try
          {
            return bmp != null && bmp.IsValid && bmp.Width != 0 && bmp.Height != 0;
          }
          catch
          {
            return false;
          }
        }
      }

      public static int NearestSquare(int value, int target)
      {
        int num = value;
        for (; value > target; value >>= 1)
          num = value;
        return num;
      }

      public static Size GetGridSize(Size fullSize, int partSize)
      {
        return new Size((fullSize.Width - 1) / partSize + 1, (fullSize.Height - 1) / partSize + 1);
      }

      public static Size GetTextureGridSize(RendererImage bmp, int partSize)
      {
        return !TextureManager.TextureElement.IsValid(bmp) ? Size.Empty : TextureManager.TextureElement.GetGridSize(bmp.Size, partSize);
      }
    }
  }
}
