// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SearchContextMenuBuilder
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Localize;
using cYo.Common.Net.Search;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SearchContextMenuBuilder
  {
    private const int Limit = 10;
    private const int MultiLimit = 20;

    public ToolStripMenuItem CreateMenuItem(INetSearch search, string hint, string text)
    {
      ToolStripMenuItem mi = new ToolStripMenuItem(TR.Load("SearchMenu")[search.Name, search.Name], search.Image);
      mi.DropDownOpening += (EventHandler) ((s, e) =>
      {
        mi.DropDownItems.Clear();
        mi.DropDownItems.AddRange(this.CreateItems(search, hint, text, false).ToArray<ToolStripItem>());
      });
      mi.DropDownItems.Add((ToolStripItem) new ToolStripMenuItem("Dummy"));
      return mi;
    }

    public IEnumerable<ToolStripMenuItem> CreateMenuItems(
      IEnumerable<INetSearch> searches,
      string hint,
      string text)
    {
      foreach (INetSearch search in searches)
        yield return this.CreateMenuItem(search, hint, text);
    }

    public IEnumerable<ToolStripItem> CreateItems(
      INetSearch search,
      string hint,
      string text,
      bool withImages)
    {
      using (new WaitCursor())
      {
        List<ToolStripItem> cm = new List<ToolStripItem>();
        Image image = withImages ? search.Image : (Image) null;
        try
        {
          text = (text ?? string.Empty).Trim();
          if (!string.IsNullOrEmpty(text))
          {
            SearchResult[] array = search.Search(hint, text, 10).ToArray<SearchResult>();
            if (array.Length != 0)
            {
              this.AddEntries((IList<ToolStripItem>) cm, (IEnumerable<SearchResult>) array, image);
            }
            else
            {
              foreach (string text1 in ((IEnumerable<string>) text.Split(',')).TrimStrings().RemoveEmpty().Distinct<string>())
              {
                if (this.AddEntries((IList<ToolStripItem>) cm, search.Search(hint, text1, 2), image) > 0)
                  cm.Add((ToolStripItem) new ToolStripSeparator());
                if (cm.Count > 20)
                  break;
              }
            }
          }
        }
        catch
        {
        }
        finally
        {
          if (cm.Count == 0)
          {
            List<ToolStripItem> toolStripItemList = cm;
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(TR.Load("SearchMenu")["NoResult", "No Results"]);
            toolStripMenuItem.Enabled = false;
            toolStripItemList.Add((ToolStripItem) toolStripMenuItem);
          }
          string gl = search.GenericSearchLink(hint, text);
          if (!string.IsNullOrEmpty(gl))
          {
            if (cm.Count > 0 && !(cm[cm.Count - 1] is ToolStripSeparator))
              cm.Add((ToolStripItem) new ToolStripSeparator());
            cm.Add((ToolStripItem) new ToolStripMenuItem(TR.Load("SearchMenu")["Search", "Search..."], image, (EventHandler) ((o, ex) => this.SafeStart(gl))));
          }
        }
        return (IEnumerable<ToolStripItem>) cm;
      }
    }

    public ContextMenuStrip CreateContextMenu(
      IEnumerable<INetSearch> searches,
      string hint,
      string text,
      Action<ContextMenuStrip> customItems)
    {
      ContextMenuStrip cm = new ContextMenuStrip();
      if (searches.Count<INetSearch>() > 1)
      {
        cm.Items.AddRange((ToolStripItem[]) this.CreateMenuItems(searches, hint, text).ToArray<ToolStripMenuItem>());
        if (customItems != null)
          customItems(cm);
      }
      else
      {
        INetSearch search = searches.FirstOrDefault<INetSearch>();
        if (search != null)
        {
          cm.Opening += (CancelEventHandler) ((s, e) =>
          {
            cm.Items.Clear();
            cm.Items.AddRange(this.CreateItems(search, hint, text, true).ToArray<ToolStripItem>());
            if (customItems == null)
              return;
            customItems(cm);
          });
          cm.Items.Add((ToolStripItem) new ToolStripMenuItem("Dummy"));
        }
      }
      return cm;
    }

    private int AddEntries(IList<ToolStripItem> cm, IEnumerable<SearchResult> results, Image image)
    {
      int num = 0;
      foreach (SearchResult result in results)
      {
        SearchResult sr = result;
        cm.Add((ToolStripItem) new ToolStripMenuItem(sr.Name, image, (EventHandler) ((o, ex) => this.SafeStart(sr.Result))));
        ++num;
      }
      return num;
    }

    private void SafeStart(string address)
    {
      try
      {
        Process.Start(address);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
