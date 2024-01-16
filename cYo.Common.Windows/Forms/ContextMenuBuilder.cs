// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ContextMenuBuilder
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ContextMenuBuilder
  {
    private readonly List<ContextMenuBuilder.MenuEntry> entries = new List<ContextMenuBuilder.MenuEntry>();

    public int Count => this.entries.Count;

    public void Add(
      string text,
      bool topLevel,
      bool chk,
      EventHandler handler,
      object tag,
      DateTime lastTimeUsed)
    {
      this.entries.Add(new ContextMenuBuilder.MenuEntry()
      {
        Text = text,
        Checked = chk,
        TopLevel = topLevel,
        Click = handler,
        Tag = tag,
        LastTimeUsed = lastTimeUsed
      });
    }

    public ToolStripItem[] Create(int maxLength)
    {
      bool oneLevel = this.entries.Count < maxLength;
      List<ToolStripItem> list = this.entries.Where<ContextMenuBuilder.MenuEntry>((Func<ContextMenuBuilder.MenuEntry, bool>) (tsi => tsi.TopLevel | oneLevel)).Select<ContextMenuBuilder.MenuEntry, ToolStripMenuItem>((Func<ContextMenuBuilder.MenuEntry, ToolStripMenuItem>) (tsi => tsi.Create())).Cast<ToolStripItem>().ToList<ToolStripItem>();
      if (oneLevel)
        return list.ToArray();
      if (list.Count > 0)
        list.Add((ToolStripItem) new ToolStripSeparator());
      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(TR.Default["Recent", "Recent"]);
      list.Add((ToolStripItem) toolStripMenuItem1);
      List<ContextMenuBuilder.MenuEntry> lastUsed = new List<ContextMenuBuilder.MenuEntry>();
      this.entries.ForEach((Action<ContextMenuBuilder.MenuEntry>) (me =>
      {
        if (me.TopLevel)
          return;
        lastUsed.Add(me);
      }));
      lastUsed.Sort((Comparison<ContextMenuBuilder.MenuEntry>) ((a, b) =>
      {
        int num = -DateTime.Compare(a.LastTimeUsed, b.LastTimeUsed);
        return num != 0 ? num : string.Compare(a.Text, b.Text);
      }));
      for (int index = 0; index < Math.Min(maxLength, lastUsed.Count); ++index)
      {
        ContextMenuBuilder.MenuEntry menuEntry = lastUsed[index];
        if (!(menuEntry.LastTimeUsed == DateTime.MinValue))
          toolStripMenuItem1.DropDownItems.Add((ToolStripItem) menuEntry.Create());
        else
          break;
      }
      toolStripMenuItem1.Visible = toolStripMenuItem1.DropDownItems.Count > 0;
      ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(TR.Default["All"]);
      list.Add((ToolStripItem) toolStripMenuItem2);
      foreach (ContextMenuBuilder.MenuEntry entry in this.entries)
        toolStripMenuItem2.DropDownItems.Add((ToolStripItem) entry.Create());
      Dictionary<char, List<ContextMenuBuilder.MenuEntry>> dictionary = new Dictionary<char, List<ContextMenuBuilder.MenuEntry>>();
      foreach (ContextMenuBuilder.MenuEntry entry in this.entries)
      {
        char key = entry.Text[0];
        List<ContextMenuBuilder.MenuEntry> menuEntryList;
        if (!dictionary.TryGetValue(key, out menuEntryList))
          menuEntryList = dictionary[key] = new List<ContextMenuBuilder.MenuEntry>();
        menuEntryList.Add(entry);
      }
      List<char> charList = new List<char>((IEnumerable<char>) dictionary.Keys);
      charList.Sort((Comparison<char>) ((a, b) => string.Compare(a.ToString(), b.ToString())));
      int index1 = -1;
      int num1 = 0;
      for (int index2 = 0; index2 < charList.Count; ++index2)
      {
        char key = charList[index2];
        if (index1 == -1)
          index1 = index2;
        num1 += dictionary[key].Count;
        int count = index2 == charList.Count - 1 ? 0 : dictionary[charList[index2 + 1]].Count;
        if (num1 + count >= maxLength || index2 == charList.Count - 1)
        {
          ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
          toolStripMenuItem3.Text = index2 == index1 ? key.ToString() : string.Format("{0}-{1}", (object) charList[index1], (object) key);
          ToolStripMenuItem toolStripMenuItem4 = toolStripMenuItem3;
          for (int index3 = index1; index3 <= index2; ++index3)
          {
            foreach (ContextMenuBuilder.MenuEntry menuEntry in (IEnumerable<ContextMenuBuilder.MenuEntry>) dictionary[charList[index3]].OrderBy<ContextMenuBuilder.MenuEntry, string>((Func<ContextMenuBuilder.MenuEntry, string>) (t => t.Text)))
              toolStripMenuItem4.DropDownItems.Add((ToolStripItem) menuEntry.Create());
          }
          num1 = 0;
          index1 = -1;
          list.Add((ToolStripItem) toolStripMenuItem4);
        }
      }
      return list.ToArray();
    }

    private class MenuEntry
    {
      public string Text;
      public EventHandler Click;
      public bool Checked;
      public bool TopLevel;
      public object Tag;
      public DateTime LastTimeUsed;

      public ToolStripMenuItem Create()
      {
        ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
        toolStripMenuItem1.Text = this.Text;
        toolStripMenuItem1.Checked = this.Checked;
        toolStripMenuItem1.Tag = this.Tag;
        ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
        toolStripMenuItem2.Click += this.Click;
        return toolStripMenuItem2;
      }
    }
  }
}
