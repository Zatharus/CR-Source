// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeySearch
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Runtime;
using cYo.Common.Text;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class KeySearch : Component
  {
    private readonly Func<string, bool> select;
    private long lastTime;
    private string currentText = string.Empty;
    private int searchDelay = 2500;

    public KeySearch(Func<string, bool> select) => this.select = select;

    public bool Select(char c)
    {
      long ticks = Machine.Ticks;
      if (ticks > this.lastTime + (long) this.SearchDelay)
        this.currentText = string.Empty;
      string str = c != '\b' ? this.CurrentText + c.ToString() : (string.IsNullOrEmpty(this.CurrentText) ? string.Empty : this.CurrentText.Substring(0, this.CurrentText.Length - 1));
      bool flag = this.select(str);
      if (flag)
        this.currentText = str;
      this.lastTime = ticks;
      return flag;
    }

    public void Reset()
    {
      this.lastTime = 0L;
      this.currentText = string.Empty;
    }

    public string CurrentText => this.currentText;

    public int SearchDelay
    {
      get => this.searchDelay;
      set => this.searchDelay = value;
    }

    public static void Create(ListView listView) => KeySearch.Create(listView, true);

    public static void Create(ListView listView, bool ignoreAricles)
    {
      KeySearch ks = new KeySearch((Func<string, bool>) (s =>
      {
        ListViewItem li = listView.Enumerate().FirstOrDefault<ListViewItem>((Func<ListViewItem, bool>) (item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase, ignoreAricles)));
        if (li == null)
          return false;
        if (li == listView.FocusedItem)
          return true;
        try
        {
          listView.BeginUpdate();
          li.EnsureVisible();
          li.Focused = true;
          listView.Enumerate().ForEach<ListViewItem>((Action<ListViewItem>) (item => item.Selected = item == li));
        }
        finally
        {
          listView.EndUpdate();
        }
        return true;
      }));
      KeySearch.RegisterKeys((Control) listView, ks);
    }

    public static void Create(ItemView itemView) => KeySearch.Create(itemView, true);

    public static void Create(ItemView itemView, bool ignoreAricles)
    {
      KeySearch ks = new KeySearch((Func<string, bool>) (s =>
      {
        IViewableItem viewableItem = itemView.DisplayedItems.FirstOrDefault<IViewableItem>((Func<IViewableItem, bool>) (item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase, ignoreAricles)));
        if (viewableItem == null)
          return false;
        if (viewableItem == itemView.FocusedItem)
          return true;
        itemView.BeginUpdate();
        try
        {
          itemView.EnsureItemVisible(viewableItem);
          itemView.SelectAll(false);
          itemView.Select(viewableItem);
          itemView.FocusedItem = viewableItem;
        }
        finally
        {
          itemView.EndUpdate();
        }
        return true;
      }));
      KeySearch.RegisterKeys((Control) itemView, ks);
    }

    private static void RegisterKeys(Control control, KeySearch ks)
    {
      ToolTip toolTip = new ToolTip();
      control.KeyPress += (KeyPressEventHandler) ((s, e) =>
      {
        if (e.Handled)
          return;
        e.Handled = ks.Select(e.KeyChar);
        if (!e.Handled)
          return;
        toolTip.Show(TR.Default["Search", "Search"] + ": " + ks.CurrentText, (IWin32Window) control, 0, 0, 3000);
      });
      control.Disposed += (EventHandler) ((s, e) => toolTip.Dispose());
    }
  }
}
