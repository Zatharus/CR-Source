// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.TextRun
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class TextRun : Span, IRender
  {
    private int lastMeasureWidth;
    private const TextFormatFlags formatFlags = TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding;
    public static bool UseTextRenderer = true;
    public static TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAlias;
    private static readonly StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
    private string text;

    static TextRun() => TextRun.stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

    public TextRun()
    {
    }

    public TextRun(string text, bool wordBreak)
    {
      if (wordBreak)
        this.Inlines.AddRange((IEnumerable<Inline>) TextRun.GetWords(text));
      else
        this.text = text;
    }

    public TextRun(string text)
      : this(text, true)
    {
    }

    public string Text
    {
      get => this.text;
      set => this.text = value;
    }

    public void Measure(Graphics gr, int maxWidth)
    {
      LayoutType pendingLayout = this.PendingLayout;
      this.PendingLayout = LayoutType.None;
      if (pendingLayout != LayoutType.Full && !this.Size.IsEmpty || pendingLayout == LayoutType.Position && this.lastMeasureWidth == maxWidth)
        return;
      this.lastMeasureWidth = maxWidth;
      this.Size = Size.Empty;
      if (string.IsNullOrEmpty(this.text))
        return;
      if (TextRun.UseTextRenderer)
      {
        this.Size = TextRenderer.MeasureText((IDeviceContext) gr, this.text, this.Font, new Size(maxWidth, 1000), TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding);
      }
      else
      {
        TextRenderingHint textRenderingHint = gr.TextRenderingHint;
        gr.TextRenderingHint = TextRun.TextRenderingHint;
        try
        {
          this.Size = gr.MeasureString(this.text, this.Font, maxWidth, TextRun.stringFormat).ToSize();
        }
        finally
        {
          gr.TextRenderingHint = textRenderingHint;
        }
      }
      this.BaseLine = this.Size.Height - this.DescentHeight;
    }

    public void Draw(Graphics gr, Point location)
    {
      if (string.IsNullOrEmpty(this.text))
        return;
      Point location1 = this.Location;
      location1.Offset(location);
      if (TextRun.UseTextRenderer)
      {
        TextRenderer.DrawText((IDeviceContext) gr, this.text, this.Font, location1, this.ForeColor, TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.NoPadding);
      }
      else
      {
        TextRenderingHint textRenderingHint = gr.TextRenderingHint;
        gr.TextRenderingHint = TextRun.TextRenderingHint;
        try
        {
          using (Brush brush = (Brush) new SolidBrush(this.ForeColor))
            gr.DrawString(this.text, this.Font, brush, (PointF) location1, TextRun.stringFormat);
        }
        finally
        {
          gr.TextRenderingHint = textRenderingHint;
        }
      }
    }

    public bool IsWhiteSpace
    {
      get
      {
        return this.text == null || this.text.All<char>((Func<char, bool>) (c => char.IsWhiteSpace(c)));
      }
    }

    public override string ToString() => this.text;

    public static IList<Inline> GetWords(string text)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<Inline> words = new List<Inline>();
      foreach (char c in text)
      {
        if (char.IsLetterOrDigit(c))
        {
          stringBuilder.Append(c);
        }
        else
        {
          if (stringBuilder.Length > 0)
          {
            words.Add((Inline) new TextRun(stringBuilder.ToString(), false));
            stringBuilder.Length = 0;
          }
          words.Add((Inline) new TextRun(c.ToString(), false));
        }
      }
      if (stringBuilder.Length > 0)
        words.Add((Inline) new TextRun(stringBuilder.ToString(), false));
      return (IList<Inline>) words;
    }

    private class TextRendererDC : DisposableObject, IDeviceContext, IDisposable
    {
      private readonly Graphics graphics;
      private IntPtr dc;

      private TextRendererDC()
      {
      }

      public TextRendererDC(Graphics g) => this.graphics = g;

      public IntPtr GetHdc()
      {
        TextRun.TextRendererDC.NativeMethods.XFORM xform;
        using (Matrix transform = this.graphics.Transform)
          xform = new TextRun.TextRendererDC.NativeMethods.XFORM(transform.Elements);
        IntPtr hrgn;
        using (Region clip = this.graphics.Clip)
          hrgn = clip.GetHrgn(this.graphics);
        this.dc = this.graphics.GetHdc();
        HandleRef hdc = new HandleRef((object) this, this.dc);
        HandleRef hRegion = new HandleRef((object) null, hrgn);
        TextRun.TextRendererDC.SetTransform(hdc, xform);
        TextRun.TextRendererDC.SetClip(hdc, hRegion);
        return this.dc;
      }

      public void ReleaseHdc()
      {
        if (!(this.dc != IntPtr.Zero))
          return;
        this.graphics.ReleaseHdc();
        this.dc = IntPtr.Zero;
      }

      protected override void Dispose(bool disposing) => this.ReleaseHdc();

      private static void SetTransform(
        HandleRef hdc,
        TextRun.TextRendererDC.NativeMethods.XFORM xform)
      {
        TextRun.TextRendererDC.NativeMethods.SetGraphicsMode(hdc, 2);
        TextRun.TextRendererDC.NativeMethods.SetWorldTransform(hdc, xform);
      }

      private static void SetClip(HandleRef hdc, HandleRef hRegion)
      {
        TextRun.TextRendererDC.NativeMethods.SelectClipRgn(hdc, hRegion);
      }

      private class NativeMethods
      {
        public const int GM_ADVANCED = 2;

        [DllImport("Gdi32")]
        public static extern int SetGraphicsMode(HandleRef hdc, int mode);

        [DllImport("Gdi32")]
        public static extern bool SetWorldTransform(
          HandleRef hDC,
          TextRun.TextRendererDC.NativeMethods.XFORM xform);

        [DllImport("Gdi32")]
        public static extern int SelectClipRgn(HandleRef hDC, HandleRef hRgn);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class XFORM
        {
          public float eM11;
          public float eM12;
          public float eM21;
          public float eM22;
          public float eDx;
          public float eDy;

          private XFORM()
          {
          }

          public XFORM(float[] elements)
          {
            this.eM11 = elements[0];
            this.eM12 = elements[1];
            this.eM21 = elements[2];
            this.eM22 = elements[3];
            this.eDx = elements[4];
            this.eDy = elements[5];
          }
        }
      }
    }
  }
}
