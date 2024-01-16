// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TextBoxExtensions
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class TextBoxExtensions
  {
    public const string OnlyNumberKeys = "0123456789.,";

    public static void SetCueText(this TextBox tb, string text)
    {
      TextBoxExtensions.Native.SendMessage(tb.Handle, 5377, IntPtr.Zero, text);
    }

    public static void EnableKeys(this TextBoxBase tb, IEnumerable<char> enabledKeys)
    {
      tb.KeyPress += (KeyPressEventHandler) ((s, e) => e.Handled = !enabledKeys.Contains<char>(e.KeyChar) && !"\b".Contains<char>(e.KeyChar));
    }

    public static void EnableOnlyNumberKeys(this TextBoxBase tb)
    {
      tb.EnableKeys((IEnumerable<char>) "0123456789.,");
    }

    private static class Native
    {
      public const int ECM_FIRST = 5376;
      public const int EM_SETCUEBANNER = 5377;
      public const int EM_GETCUEBANNER = 5378;

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
    }
  }
}
