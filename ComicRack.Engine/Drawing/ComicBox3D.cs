// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ComicBox3D
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Drawing3D;
using cYo.Common.Mathematics;
using cYo.Projects.ComicRack.Engine.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public class ComicBox3D
  {
    private const float DefaultRotateX = 54f;
    private const float DefaultRotateY = -28f;
    private const float DefaultRotateZ = 12f;
    private const float DefaultDistance = 1.5f;
    private static readonly Bitmap shadowMap = Resources.ShadowMap;

    public static Bitmap CreateDefaultBook(
      Bitmap cover,
      Bitmap coverBack,
      Size size,
      int pages,
      ComicBox3DOptions options = ComicBox3DOptions.Default)
    {
      return ComicBox3D.Create(cover, coverBack, size, (float) ((double) pages / 25.0 * 0.02500000037252903), 1.5f, 54f, -28f, 12f, options);
    }

    public static Bitmap Create(
      Bitmap cover,
      Bitmap coverBack,
      Size size,
      float thickness,
      float distance,
      float rx,
      float ry,
      float rz,
      ComicBox3DOptions options = ComicBox3DOptions.Default)
    {
      CyoGl cyoGl = new CyoGl();
      cyoGl.Lights.Add(new Light()
      {
        Position = new Vector3(0.5f, 3f, -5f),
        LightType = LightType.Point,
        DistanceFallOff = true,
        DiffusePower = 80f
      });
      Size size1 = size;
      float x = 0.6666667f;
      float y = 1f;
      float z = thickness.Clamp(0.02f, 0.15f);
      if ((double) x > 1.0)
      {
        x = 1f;
        y = (float) cover.Height / (float) cover.Width;
      }
      cyoGl.Projection = Matrix4.PerspectiveFOVInfinity(Numeric.DegToRad(35f), 1f, -distance);
      Matrix4 matrix4 = Matrix4.Identity * Matrix4.Translation((float) (-(double) x / 2.0), (float) (-(double) y / 2.0), (float) (-(double) z / 2.0)) * Matrix4.RotationX(Numeric.DegToRad(rx)) * Matrix4.RotationY(Numeric.DegToRad(ry)) * Matrix4.RotationZ(Numeric.DegToRad(rz));
      cyoGl.ModelView *= matrix4;
      Rectangle rectangle1 = cover.Size.ToRectangle();
      Rectangle clip = coverBack != null ? coverBack.Size.ToRectangle() : rectangle1;
      if (options.HasFlag((Enum) ComicBox3DOptions.SplitDoublePages))
      {
        if (rectangle1.Width > rectangle1.Height)
        {
          int width = rectangle1.Width;
          rectangle1.Width = Math.Min(width, rectangle1.Height * cover.Height / cover.Width);
          rectangle1.X = width - rectangle1.Width;
        }
        if (clip.Width > clip.Height)
        {
          int width = clip.Width;
          clip.Width = Math.Min(width, clip.Height * cover.Height / cover.Width);
          clip.X = width - clip.Width;
        }
      }
      Size size2 = ComicBox3D.shadowMap.Size;
      Vertex[] vertexArray1 = new Vertex[4]
      {
        new Vertex(0.0f, y, 0.0f, 0.0f, 0.0f),
        new Vertex(x, y, 0.0f, (float) (rectangle1.Width - 1), 0.0f),
        new Vertex(x, 0.0f, 0.0f, (float) (rectangle1.Width - 1), (float) (rectangle1.Height - 1)),
        new Vertex(0.0f, 0.0f, 0.0f, 0.0f, (float) (rectangle1.Height - 1))
      };
      Vertex[] vertexArray2 = new Vertex[4]
      {
        new Vertex(0.0f, 0.0f, z, 0.0f, (float) (clip.Height - 1)),
        new Vertex(x, 0.0f, z, (float) (clip.Width - 1), (float) (clip.Height - 1)),
        new Vertex(x, y, z, (float) (clip.Width - 1), 0.0f),
        new Vertex(0.0f, y, z, 0.0f, 0.0f)
      };
      RectangleF rectangleF = new RectangleF((float) rectangle1.Width * 0.1f, (float) rectangle1.Height * 0.05f, (float) rectangle1.Width * 0.8f, (float) rectangle1.Height * z);
      Vertex[] vertexArray3 = new Vertex[4]
      {
        new Vertex(0.0f, y, 0.0f, rectangleF.Left, rectangleF.Top),
        new Vertex(0.0f, 0.0f, 0.0f, rectangleF.Right, rectangleF.Top),
        new Vertex(0.0f, 0.0f, z, rectangleF.Right, rectangleF.Bottom),
        new Vertex(0.0f, y, z, rectangleF.Left, rectangleF.Bottom)
      };
      Vertex[] vertexArray4 = new Vertex[4]
      {
        new Vertex(0.0f, 0.0f, 0.0f),
        new Vertex(x, 0.0f, 0.0f),
        new Vertex(x, 0.0f, z),
        new Vertex(0.0f, 0.0f, z)
      };
      Vertex[] vertexArray5 = new Vertex[4]
      {
        new Vertex(0.0f, y, 0.0f),
        new Vertex(0.0f, y, z),
        new Vertex(x, y, z),
        new Vertex(x, y, 0.0f)
      };
      Vertex[] vertexArray6 = new Vertex[4]
      {
        new Vertex(x, y, 0.0f),
        new Vertex(x, y, z),
        new Vertex(x, 0.0f, z),
        new Vertex(x, 0.0f, 0.0f)
      };
      IEnumerable<Vertex> vertices = ((IEnumerable<Vertex>) vertexArray1).Concat<Vertex>((IEnumerable<Vertex>) vertexArray2).Concat<Vertex>((IEnumerable<Vertex>) vertexArray3).Concat<Vertex>((IEnumerable<Vertex>) vertexArray4).Concat<Vertex>((IEnumerable<Vertex>) vertexArray5).Concat<Vertex>((IEnumerable<Vertex>) vertexArray6);
      Vertex[] vertexArray7 = (Vertex[]) null;
      if (options.HasFlag((Enum) ComicBox3DOptions.SimpleShadow))
      {
        float num = (float) (0.014999999664723873 + (double) z / 4.0);
        vertexArray7 = new Vertex[4]
        {
          new Vertex(0.0f - num, y + num, z, 0.0f, 0.0f),
          new Vertex(x + num, y + num, z, (float) (size2.Width - 1), 0.0f),
          new Vertex(x + num, 0.0f - num, z, (float) (size2.Width - 1), (float) (size2.Height - 1)),
          new Vertex(0.0f - num, 0.0f - num, z, 0.0f, (float) (size2.Height - 1))
        };
        vertices = vertices.Concat<Vertex>((IEnumerable<Vertex>) vertexArray7);
      }
      if (options.HasFlag((Enum) ComicBox3DOptions.Wireless))
        vertices.ForEach<Vertex>((Action<Vertex>) (p => p.SetColor((ColorF) Color.Black)));
      RectangleF viewportExtent = cyoGl.GetViewportExtent(vertices, false);
      float num1 = Math.Max(viewportExtent.Width, viewportExtent.Height);
      viewportExtent.X += (float) (((double) viewportExtent.Width - (double) num1) / 2.0);
      viewportExtent.Y += (float) (((double) viewportExtent.Height - (double) num1) / 2.0);
      viewportExtent.Width = viewportExtent.Height = num1;
      cyoGl.Viewport = viewportExtent;
      if (options.HasFlag((Enum) ComicBox3DOptions.Filter))
        size = size.Scale(1.5f);
      Bitmap bitmap1 = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
      try
      {
        using (BitmapFrameBuffer bitmapFrameBuffer1 = new BitmapFrameBuffer(bitmap1))
        {
          using (BitmapFrameBuffer bitmapFrameBuffer2 = new BitmapFrameBuffer(ComicBox3D.shadowMap))
          {
            using (BitmapFrameBuffer bitmapFrameBuffer3 = new BitmapFrameBuffer(cover, rectangle1))
            {
              using (BitmapFrameBuffer bitmapFrameBuffer4 = coverBack != null ? new BitmapFrameBuffer(coverBack, clip) : bitmapFrameBuffer3)
              {
                cyoGl.FrameBuffer = (IFrameBuffer) bitmapFrameBuffer1;
                cyoGl.Wireless = options.HasFlag((Enum) ComicBox3DOptions.Wireless);
                cyoGl.BacksideCulling = true;
                cyoGl.ShadingModel = ShadingModel.Gourard;
                if (vertexArray7 != null)
                {
                  cyoGl.Texture = (ITexture) bitmapFrameBuffer2;
                  cyoGl.DrawQuad(vertexArray7);
                }
                cyoGl.Texture = (ITexture) bitmapFrameBuffer3;
                cyoGl.DrawQuad(vertexArray1);
                cyoGl.DrawQuad(vertexArray3);
                cyoGl.Texture = (ITexture) bitmapFrameBuffer4;
                cyoGl.DrawQuad(vertexArray2);
                cyoGl.Texture = (ITexture) null;
                cyoGl.DrawQuad(vertexArray4);
                cyoGl.DrawQuad(vertexArray5);
                cyoGl.DrawQuad(vertexArray6);
              }
            }
          }
        }
        Rectangle rectangle2 = Rectangle.Truncate(cyoGl.GetViewportExtent(vertices));
        if (options.HasFlag((Enum) ComicBox3DOptions.Filter))
        {
          rectangle2 = rectangle2.Scale(0.6666667f);
          using (Bitmap bmp = bitmap1)
            bitmap1 = bmp.Scale(size1, BitmapResampling.GdiPlusHQ);
        }
        if (!options.HasFlag((Enum) ComicBox3DOptions.Trim))
        {
          Bitmap bitmap2 = bitmap1;
          bitmap1 = (Bitmap) null;
          return bitmap2;
        }
        return rectangle2.Width == 0 || rectangle2.Height == 0 ? (Bitmap) null : bitmap1.CreateCopy(rectangle2);
      }
      finally
      {
        bitmap1?.Dispose();
      }
    }
  }
}
