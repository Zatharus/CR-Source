// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.CyoGl
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class CyoGl
  {
    private Stack<Matrix4> modelMatrixStack = new Stack<Matrix4>();
    private List<cYo.Common.Drawing3D.Light> lights = new List<cYo.Common.Drawing3D.Light>();

    public CyoGl()
    {
      this.Viewport = (RectangleF) new Rectangle(-1, -1, 2, 2);
      this.Projection = Matrix4.Identity;
      this.ModelView = Matrix4.Identity;
      this.AmbientLight = new ColorF(0.1f);
      this.ShadingModel = ShadingModel.Flat;
    }

    public RectangleF Viewport { get; set; }

    public IFrameBuffer FrameBuffer { get; set; }

    public Matrix4 Projection { get; set; }

    public Matrix4 ModelView { get; set; }

    public ITexture Texture { get; set; }

    public bool BacksideCulling { get; set; }

    public ColorF AmbientLight { get; set; }

    public List<cYo.Common.Drawing3D.Light> Lights => this.lights;

    public ShadingModel ShadingModel { get; set; }

    public bool Wireless { get; set; }

    public void PushMatrix() => this.modelMatrixStack.Push(this.ModelView);

    public void PopMatrix() => this.ModelView = this.modelMatrixStack.Pop();

    public Vector4 GetPlaneVector(Vector3 v1, Vector3 v2, Vector3 v3)
    {
      Vector4 vector4 = this.GetSurfaceNormal(v1, v2, v3, this.ModelView).Vector4;
      vector4[3] = (float) ((double) vector4[0] * (double) v1[0] + (double) vector4[1] * (double) v1[1] + (double) vector4[2] * (double) v1[2]);
      return vector4;
    }

    public Matrix4 GetShadowMatrix(Vector4 plane, Vector4 lightPosition)
    {
      Matrix4 shadowMatrix = new Matrix4();
      float num = Vector4.Dot(plane, lightPosition);
      shadowMatrix[0, 0] = num - lightPosition[0] * plane[0];
      shadowMatrix[0, 1] = -lightPosition[0] * plane[1];
      shadowMatrix[0, 2] = -lightPosition[0] * plane[2];
      shadowMatrix[0, 3] = -lightPosition[0] * plane[3];
      shadowMatrix[1, 0] = -lightPosition[1] * plane[0];
      shadowMatrix[1, 1] = num - lightPosition[1] * plane[1];
      shadowMatrix[1, 2] = -lightPosition[1] * plane[2];
      shadowMatrix[1, 3] = -lightPosition[1] * plane[3];
      shadowMatrix[2, 0] = -lightPosition[2] * plane[0];
      shadowMatrix[2, 1] = -lightPosition[2] * plane[1];
      shadowMatrix[2, 2] = num - lightPosition[2] * plane[2];
      shadowMatrix[2, 3] = -lightPosition[2] * plane[3];
      shadowMatrix[3, 0] = -lightPosition[3] * plane[0];
      shadowMatrix[3, 1] = -lightPosition[3] * plane[1];
      shadowMatrix[3, 2] = -lightPosition[3] * plane[2];
      shadowMatrix[3, 3] = num - lightPosition[3] * plane[3];
      return shadowMatrix;
    }

    public void DrawTriangle(Vertex p1, Vertex p2, Vertex p3)
    {
      if (this.BacksideCulling && this.IsBackside(p1, p2, p3))
        return;
      Vertex vertex1 = new Vertex(p1);
      Vertex vertex2 = new Vertex(p2);
      Vertex vertex3 = new Vertex(p3);
      if (this.ShadingModel == ShadingModel.Gourard)
      {
        Vector3 surfaceNormal = this.GetSurfaceNormal((Vector3) vertex1, (Vector3) vertex2, (Vector3) vertex3, this.ModelView);
        vertex1.SetColor(this.Light(vertex1, surfaceNormal));
        vertex2.SetColor(this.Light(vertex2, surfaceNormal));
        vertex3.SetColor(this.Light(vertex3, surfaceNormal));
      }
      Vertex vertex4 = this.Project(vertex1);
      Vertex vertex5 = this.Project(vertex2);
      Vertex vertex6 = this.Project(vertex3);
      if (!this.Wireless)
      {
        Rasterizer.RasterizeTriangle(this.FrameBuffer, vertex4, vertex5, vertex6, this.Texture);
      }
      else
      {
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex4, vertex5);
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex5, vertex6);
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex6, vertex4);
      }
    }

    public void DrawQuad(Vertex p1, Vertex p2, Vertex p3, Vertex p4)
    {
      if (this.BacksideCulling && this.IsBackside(p1, p2, p4))
        return;
      Vertex vertex1 = new Vertex(p1);
      Vertex vertex2 = new Vertex(p2);
      Vertex vertex3 = new Vertex(p3);
      Vertex vertex4 = new Vertex(p4);
      if (this.ShadingModel == ShadingModel.Gourard)
      {
        Vector3 surfaceNormal = this.GetSurfaceNormal((Vector3) vertex1, (Vector3) vertex2, (Vector3) vertex4, this.ModelView);
        vertex1.SetColor(this.Light(vertex1, surfaceNormal));
        vertex2.SetColor(this.Light(vertex2, surfaceNormal));
        vertex3.SetColor(this.Light(vertex3, surfaceNormal));
        vertex4.SetColor(this.Light(vertex4, surfaceNormal));
      }
      Vertex vertex5 = this.Project(vertex1);
      Vertex vertex6 = this.Project(vertex2);
      Vertex vertex7 = this.Project(vertex3);
      Vertex vertex8 = this.Project(vertex4);
      if (!this.Wireless)
      {
        Rasterizer.RasterizeQuad(this.FrameBuffer, vertex5, vertex6, vertex7, vertex8, this.Texture);
      }
      else
      {
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex5, vertex6);
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex6, vertex7);
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex7, vertex8);
        Rasterizer.RasterizeLine(this.FrameBuffer, vertex8, vertex5);
      }
    }

    public void DrawQuad(Vertex[] v)
    {
      if (v.Length != 4)
        throw new ArgumentException();
      this.DrawQuad(v[0], v[1], v[2], v[3]);
    }

    private ColorF Light(Vertex p, Vector3 surfaceNormal)
    {
      ColorF ambientLight = this.AmbientLight;
      Vector3 viewDirection = new Vector3(0.0f, 0.0f, 1f);
      foreach (cYo.Common.Drawing3D.Light light in this.lights)
      {
        if (light.Enabled)
          ambientLight += light.Calculate((Vector3) p * this.ModelView, viewDirection, surfaceNormal);
      }
      ambientLight.Clamp();
      return ambientLight;
    }

    public Vertex Project(Vertex p, bool toFramebuffer)
    {
      Vector3 vector3 = (Vector3) p * this.ModelView * this.Projection;
      Vertex vertex = new Vertex(p);
      vertex.X = vector3.X / vector3.Z;
      vertex.Y = -vector3.Y / vector3.Z;
      vertex.Z = vector3.Z;
      if (toFramebuffer)
      {
        Size size = this.FrameBuffer.Size;
        float num1 = (float) size.Width / this.Viewport.Width;
        float num2 = (float) size.Height / this.Viewport.Height;
        vertex.X = num1 * (vertex.X - this.Viewport.Left);
        vertex.Y = num2 * (vertex.Y - this.Viewport.Top);
      }
      return vertex;
    }

    public Vertex Project(Vertex v) => this.Project(v, true);

    public Vector3 GetSurfaceNormal(Vector3 p1, Vector3 p2, Vector3 p3, Matrix4 matrix)
    {
      Vector3 vector3_1 = p1 * matrix;
      Vector3 vector3_2 = p2 * matrix;
      Vector3 vector3_3 = p3 * matrix;
      return Vector3.Cross(vector3_3 - vector3_1, vector3_3 - vector3_2);
    }

    public bool IsBackside(Vertex p1, Vertex p2, Vertex p3)
    {
      Vector3 surfaceNormal = this.GetSurfaceNormal((Vector3) p1, (Vector3) p2, (Vector3) p3, this.ModelView * this.Projection);
      return (double) Vector3.Dot((Vector3) p1 * this.ModelView * this.Projection, surfaceNormal) >= 0.0;
    }

    public RectangleF GetViewportExtent(IEnumerable<Vertex> vertices, bool toFrameBuffer)
    {
      float num1 = float.MaxValue;
      float num2 = float.MaxValue;
      float val1_1 = float.MinValue;
      float val1_2 = float.MinValue;
      foreach (Vertex vertex1 in vertices)
      {
        Vertex vertex2 = this.Project(vertex1, toFrameBuffer);
        num1 = Math.Min(num1, vertex2.X);
        num2 = Math.Min(num2, vertex2.Y);
        val1_1 = Math.Max(val1_1, vertex2.X);
        val1_2 = Math.Max(val1_2, vertex2.Y);
      }
      return new RectangleF(num1, num2, val1_1 - num1, val1_2 - num2);
    }

    public RectangleF GetViewportExtent(IEnumerable<Vertex> vertices)
    {
      return this.GetViewportExtent(vertices, true);
    }

    public static Bitmap RotateBitmap(
      Bitmap bitmap,
      Size size,
      float distance,
      float rx,
      float ry,
      float rz,
      bool trim,
      bool filter)
    {
      CyoGl cyoGl = new CyoGl();
      Size size1 = size;
      float x = (float) bitmap.Width / (float) bitmap.Height;
      float y = 1f;
      if ((double) x > 1.0)
      {
        x = 1f;
        y = (float) bitmap.Height / (float) bitmap.Width;
      }
      cyoGl.Projection = Matrix4.PerspectiveFOVInfinity(Numeric.DegToRad(45f), 1f, -distance);
      cyoGl.ModelView = Matrix4.Translation((float) (-(double) x / 2.0), (float) (-(double) y / 2.0), 0.0f);
      cyoGl.ModelView *= Matrix4.RotationX(Numeric.DegToRad(rx));
      cyoGl.ModelView *= Matrix4.RotationY(Numeric.DegToRad(ry));
      cyoGl.ModelView *= Matrix4.RotationZ(Numeric.DegToRad(rz));
      Vertex[] v = new Vertex[4]
      {
        new Vertex(0.0f, y, 0.0f, 0.0f, 0.0f),
        new Vertex(x, y, 0.0f, (float) (bitmap.Width - 1), 0.0f),
        new Vertex(x, 0.0f, 0.0f, (float) (bitmap.Width - 1), (float) (bitmap.Height - 1)),
        new Vertex(0.0f, 0.0f, 0.0f, 0.0f, (float) (bitmap.Height - 1))
      };
      RectangleF viewportExtent = cyoGl.GetViewportExtent((IEnumerable<Vertex>) v, false);
      float num = Math.Max(viewportExtent.Width, viewportExtent.Height);
      viewportExtent.X += (float) (((double) viewportExtent.Width - (double) num) / 2.0);
      viewportExtent.Y += (float) (((double) viewportExtent.Height - (double) num) / 2.0);
      viewportExtent.Width = viewportExtent.Height = num;
      cyoGl.Viewport = viewportExtent;
      if (filter)
        size = size.Scale(1.5f);
      Bitmap bitmap1 = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
      try
      {
        using (BitmapFrameBuffer bitmapFrameBuffer1 = new BitmapFrameBuffer(bitmap1))
        {
          using (BitmapFrameBuffer bitmapFrameBuffer2 = new BitmapFrameBuffer(bitmap))
          {
            cyoGl.FrameBuffer = (IFrameBuffer) bitmapFrameBuffer1;
            cyoGl.Texture = (ITexture) bitmapFrameBuffer2;
            cyoGl.DrawQuad(v);
          }
        }
        Rectangle rectangle = Rectangle.Truncate(cyoGl.GetViewportExtent((IEnumerable<Vertex>) v));
        if (filter)
        {
          rectangle = rectangle.Scale(0.6666667f);
          using (Bitmap bmp = bitmap1)
            bitmap1 = bmp.Scale(size1, BitmapResampling.GdiPlusHQ);
        }
        if (!trim)
        {
          Bitmap bitmap2 = bitmap1;
          bitmap1 = (Bitmap) null;
          return bitmap2;
        }
        return rectangle.Width == 0 || rectangle.Height == 0 ? (Bitmap) null : bitmap1.CreateCopy(rectangle);
      }
      finally
      {
        bitmap1?.Dispose();
      }
    }

    public static Bitmap RotateBitmap(
      Bitmap bitmap,
      Size size,
      float distance,
      Vector3 rot,
      bool trim,
      bool filter)
    {
      return CyoGl.RotateBitmap(bitmap, size, distance, rot.X, rot.Y, rot.Z, trim, filter);
    }

    public static Bitmap RotateBitmap(
      Bitmap bitmap,
      Size size,
      float distance,
      Vector3 rot,
      bool trim)
    {
      return CyoGl.RotateBitmap(bitmap, size, distance, rot, trim, false);
    }
  }
}
