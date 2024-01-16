// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.RichTextBoxExtensions
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class RichTextBoxExtensions
  {
    public static void RegisterColorize(
      this RichTextBox rtb,
      IEnumerable<ValuePair<Color, Regex>> colors)
    {
      Action action = (Action) (() =>
      {
        using (rtb.SuspendPainting())
        {
          rtb.SelectAll();
          rtb.SelectionColor = SystemColors.WindowText;
          foreach (ValuePair<Color, Regex> color in colors)
          {
            foreach (Match match in color.Value.Matches(rtb.Text))
            {
              rtb.Select(match.Index, match.Length);
              rtb.SelectionColor = color.Key;
            }
          }
        }
      });
      rtb.TextChanged += (EventHandler) ((s, e) => action());
      action();
    }

    public static void RegisterColorize(
      this RichTextBox rtb,
      IEnumerable<ValuePair<Color, string>> colors)
    {
      rtb.RegisterColorize(colors.Select<ValuePair<Color, string>, ValuePair<Color, Regex>>((Func<ValuePair<Color, string>, ValuePair<Color, Regex>>) (vp => new ValuePair<Color, Regex>(vp.Key, new Regex(vp.Value, RegexOptions.IgnoreCase)))));
    }

    public static void RegisterColorize(this RichTextBox rtb, Color color, string expression)
    {
      rtb.RegisterColorize((IEnumerable<ValuePair<Color, string>>) new ValuePair<Color, string>[1]
      {
        new ValuePair<Color, string>(color, expression)
      });
    }

    public static void RegisterColorize(this RichTextBox rtb, Color color, Regex expression)
    {
      rtb.RegisterColorize((IEnumerable<ValuePair<Color, Regex>>) new ValuePair<Color, Regex>[1]
      {
        new ValuePair<Color, Regex>(color, expression)
      });
    }

    public static IDisposable SuspendPainting(this RichTextBox rtb)
    {
      return (IDisposable) new RichTextBoxExtensions.PaintSupend(rtb);
    }

    private class PaintSupend : DisposableObject
    {
      private const int WM_USER = 1024;
      private const int WM_SETREDRAW = 11;
      private const int EM_GETEVENTMASK = 1083;
      private const int EM_SETEVENTMASK = 1093;
      private const int EM_GETSCROLLPOS = 1245;
      private const int EM_SETSCROLLPOS = 1246;
      private System.Drawing.Point scrollPoint;
      private readonly IntPtr eventMask;
      private readonly int suspendIndex;
      private readonly int suspendLength;
      private RichTextBox rtb;

      [DllImport("user32.dll")]
      private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int wMsg,
        int wParam,
        ref System.Drawing.Point lParam);

      [DllImport("user32.dll")]
      private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);

      public PaintSupend(RichTextBox rtb)
      {
        this.rtb = rtb;
        this.suspendIndex = rtb.SelectionStart;
        this.suspendLength = rtb.SelectionLength;
        RichTextBoxExtensions.PaintSupend.SendMessage(rtb.Handle, 1245, 0, ref this.scrollPoint);
        RichTextBoxExtensions.PaintSupend.SendMessage(rtb.Handle, 11, 0, IntPtr.Zero);
        this.eventMask = RichTextBoxExtensions.PaintSupend.SendMessage(rtb.Handle, 1083, 0, IntPtr.Zero);
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          this.rtb.Select(this.suspendIndex, this.suspendLength);
          RichTextBoxExtensions.PaintSupend.SendMessage(this.rtb.Handle, 1246, 0, ref this.scrollPoint);
          RichTextBoxExtensions.PaintSupend.SendMessage(this.rtb.Handle, 1093, 0, this.eventMask);
          RichTextBoxExtensions.PaintSupend.SendMessage(this.rtb.Handle, 11, 1, IntPtr.Zero);
          this.rtb.Invalidate();
        }
        base.Dispose(disposing);
      }
    }
  }
}
