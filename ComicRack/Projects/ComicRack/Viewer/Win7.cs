// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Win7
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public static class Win7
  {
    private static TaskbarManager windowsTaskbar;
    private static Dictionary<string, TabbedThumbnail> thumbnails = new Dictionary<string, TabbedThumbnail>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static bool Initialize()
    {
      if (!TaskbarManager.IsPlatformSupported)
        return false;
      try
      {
        Win7.windowsTaskbar = TaskbarManager.Instance;
        Win7.windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress);
        JumpList jumpList1 = JumpList.CreateJumpList();
        jumpList1.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
        jumpList1.Refresh();
        JumpList jumpList2 = JumpList.CreateJumpList();
        jumpList2.KnownCategoryToDisplay = JumpListKnownCategoryType.Frequent;
        jumpList2.Refresh();
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public static void UpdateRecent(string file)
    {
      if (!TaskbarManager.IsPlatformSupported || !File.Exists(file))
        return;
      JumpList.AddToRecent(file);
    }

    public static void BuildCategory(string title, IEnumerable<string> files)
    {
    }

    public static void SetOverlayIcon(Bitmap bitmap, string text)
    {
      if (Win7.windowsTaskbar == null)
        return;
      if (bitmap == null)
        Win7.windowsTaskbar.SetOverlayIcon((Icon) null, (string) null);
      else
        Win7.windowsTaskbar.SetOverlayIcon(Icon.FromHandle(bitmap.GetHicon()), text);
    }

    public static bool TabbedThumbnailsEnabled => false;

    public static void AddTabbedThumbnail(
      IWin32Window parent,
      string url,
      Action activated,
      Action closed,
      Func<Bitmap> requestThumbnail)
    {
      if (!Win7.TabbedThumbnailsEnabled || Win7.thumbnails.ContainsKey(url))
        return;
      TabbedThumbnail tt = new TabbedThumbnail(parent.Handle, new Control());
      tt.TabbedThumbnailActivated += (EventHandler<TabbedThumbnailEventArgs>) ((s, e) => activated());
      tt.TabbedThumbnailBitmapRequested += (EventHandler<TabbedThumbnailBitmapRequestedEventArgs>) ((s, e) => tt.SetImage(requestThumbnail()));
      tt.TabbedThumbnailClosed += (EventHandler<TabbedThumbnailClosedEventArgs>) ((s, e) => closed());
      Win7.thumbnails.Add(url, tt);
      TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(tt);
    }

    public static void RemoveThumbnail(string url)
    {
      TabbedThumbnail preview;
      if (!Win7.TabbedThumbnailsEnabled || !Win7.thumbnails.TryGetValue(url, out preview))
        return;
      TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(preview);
      preview.Dispose();
      Win7.thumbnails.Remove(url);
    }

    public static void InvalidateThumbnail(string url)
    {
      TabbedThumbnail tabbedThumbnail;
      if (!Win7.TabbedThumbnailsEnabled || !Win7.thumbnails.TryGetValue(url, out tabbedThumbnail))
        return;
      tabbedThumbnail.InvalidatePreview();
    }

    public static void SetActiveThumbnail(string url)
    {
      TabbedThumbnail preview;
      if (!Win7.TabbedThumbnailsEnabled || !Win7.thumbnails.TryGetValue(url, out preview))
        return;
      TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(preview);
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public static bool ShowShield(Button button)
    {
      if (button == null)
        return false;
      uint msg = 5644;
      button.FlatStyle = FlatStyle.System;
      return Win7.SendMessage(new HandleRef((object) button, button.Handle), msg, new IntPtr(0), new IntPtr(1)) == 0;
    }
  }
}
