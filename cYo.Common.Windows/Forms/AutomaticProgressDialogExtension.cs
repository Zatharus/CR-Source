// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutomaticProgressDialogExtension
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class AutomaticProgressDialogExtension
  {
    public static bool ForEachProgress<T>(
      this IEnumerable<T> items,
      Action<T> action,
      IWin32Window parent = null,
      string caption = null,
      string description = null,
      bool enableCancel = true,
      int timeToWait = 1000)
    {
      return AutomaticProgressDialog.Process(parent, caption, description, timeToWait, (Action) (() =>
      {
        T[] array = items.ToArray<T>();
        int length = array.Length;
        for (int index = 0; index < length && !AutomaticProgressDialog.ShouldAbort; ++index)
        {
          AutomaticProgressDialog.Value = index;
          action(array[index]);
        }
      }), enableCancel ? AutomaticProgressDialogOptions.EnableCancel : AutomaticProgressDialogOptions.None);
    }
  }
}
