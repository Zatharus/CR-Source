// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TextBoxContextMenu
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Net.Search;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TextBoxContextMenu : ContextMenuStrip
  {
    public const int MaxTextLength = 128;
    private IContainer components;
    private ToolStripMenuItem miCut;
    private ToolStripMenuItem miCopy;
    private ToolStripMenuItem miPaste;
    private ToolStripMenuItem miDelete;
    private ToolStripMenuItem miSelectAll;
    private ToolStripSeparator miDeleteSep;
    private ToolStripSeparator miUndoSep;
    private ToolStripMenuItem miUndo;

    public TextBoxContextMenu()
      : this((IContainer) null)
    {
    }

    public TextBoxContextMenu(IContainer container)
    {
      container?.Add((IComponent) this);
      this.InitializeComponent();
      LocalizeUtility.Localize(TR.Load(nameof (TextBoxContextMenu)), (ToolStrip) this);
    }

    public static void Register(
      TextBoxBase tb,
      Action<ContextMenuStrip, TextBoxContextMenu.ContextMenuSource> opening = null)
    {
      if (tb == null)
        return;
      TextBoxContextMenu cm = new TextBoxContextMenu(tb.Container);
      cm.Opening += (CancelEventHandler) ((s, e) =>
      {
        cm.miUndo.Enabled = tb.CanUndo;
        cm.miCopy.Enabled = tb.SelectionLength != 0;
        cm.miPaste.Enabled = Clipboard.ContainsText();
      });
      if (opening != null)
        cm.Opening += (CancelEventHandler) ((s, e) => opening((ContextMenuStrip) cm, new TextBoxContextMenu.ContextMenuSource((Control) tb, TextBoxContextMenu.GetText(tb))));
      cm.miUndo.Click += (EventHandler) ((s, e) => tb.Undo());
      cm.miPaste.Click += (EventHandler) ((s, e) => tb.SelectedText = TextBoxContextMenu.SafeGetClipboardText());
      cm.miCopy.Click += (EventHandler) ((s, e) => TextBoxContextMenu.SafeSetClipboardText(tb.SelectedText));
      cm.miDelete.Click += (EventHandler) ((s, e) => tb.SelectedText = string.Empty);
      cm.miCut.Click += (EventHandler) ((s, e) =>
      {
        TextBoxContextMenu.SafeSetClipboardText(tb.SelectedText);
        tb.SelectedText = string.Empty;
      });
      cm.miSelectAll.Click += (EventHandler) ((s, e) => tb.SelectAll());
      tb.ContextMenuStrip = (ContextMenuStrip) cm;
    }

    public static void Register(
      ComboBox cb,
      Action<ContextMenuStrip, TextBoxContextMenu.ContextMenuSource> opening = null)
    {
      if (cb == null || cb.DropDownStyle != ComboBoxStyle.DropDown)
        return;
      TextBoxContextMenu cm = new TextBoxContextMenu(cb.Container);
      cm.Items.Remove((ToolStripItem) cm.miUndo);
      cm.Items.Remove((ToolStripItem) cm.miUndoSep);
      cm.Opening += (CancelEventHandler) ((s, e) =>
      {
        cm.miCopy.Enabled = cm.miCut.Enabled = cb.SelectionLength != 0;
        cm.miPaste.Enabled = Clipboard.ContainsText();
      });
      if (opening != null)
        cm.Opening += (CancelEventHandler) ((s, e) => opening((ContextMenuStrip) cm, new TextBoxContextMenu.ContextMenuSource((Control) cb, string.IsNullOrEmpty(cb.SelectedText) ? cb.Text : cb.SelectedText)));
      cm.miPaste.Click += (EventHandler) ((s, e) => cb.SelectedText = TextBoxContextMenu.SafeGetClipboardText());
      cm.miCopy.Click += (EventHandler) ((s, e) => TextBoxContextMenu.SafeSetClipboardText(cb.SelectedText));
      cm.miSelectAll.Click += (EventHandler) ((s, e) => cb.SelectAll());
      cm.miDelete.Click += (EventHandler) ((s, e) => cb.SelectedText = string.Empty);
      cb.ContextMenuStrip = (ContextMenuStrip) cm;
    }

    public static void Register(
      Form c,
      Action<ContextMenuStrip, TextBoxContextMenu.ContextMenuSource> opening = null)
    {
      c.GetControls<TextBoxBase>().ForEach<TextBoxBase>((Action<TextBoxBase>) (tb => TextBoxContextMenu.Register(tb, opening)));
      c.GetControls<ComboBox>().ForEach<ComboBox>((Action<ComboBox>) (tb => TextBoxContextMenu.Register(tb, opening)));
    }

    public static Action<ContextMenuStrip, TextBoxContextMenu.ContextMenuSource> AddSearchLinks(
      IEnumerable<INetSearch> searches,
      bool top = false)
    {
      return (Action<ContextMenuStrip, TextBoxContextMenu.ContextMenuSource>) ((cm, cms) =>
      {
        foreach (ToolStripItem toolStripItem in cm.Items.OfType<ToolStripItem>().ToArray<ToolStripItem>())
        {
          if (toolStripItem.Tag != null)
            cm.Items.Remove(toolStripItem);
        }
        string str = cms.Text ?? string.Empty;
        SearchContextMenuBuilder contextMenuBuilder = new SearchContextMenuBuilder();
        IEnumerable<INetSearch> searches1 = searches;
        if (!(cms.Control.Tag is string hint2))
          hint2 = string.Empty;
        string text = str;
        ToolStripMenuItem[] array = contextMenuBuilder.CreateMenuItems(searches1, hint2, text).ToArray<ToolStripMenuItem>();
        if (array.Length != 0)
        {
          ToolStripItemCollection items = cm.Items;
          int count = top ? 0 : cm.Items.Count;
          items.Insert(count, (ToolStripItem) new ToolStripSeparator()
          {
            Tag = (object) str
          });
        }
        foreach (ToolStripMenuItem toolStripMenuItem in ((IEnumerable<ToolStripMenuItem>) array).Reverse<ToolStripMenuItem>())
        {
          toolStripMenuItem.Tag = (object) (cms.Text ?? str);
          cm.Items.Insert(top ? 0 : cm.Items.Count, (ToolStripItem) toolStripMenuItem);
        }
      });
    }

    private static string SafeGetClipboardText()
    {
      try
      {
        return Clipboard.GetText();
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    private static void SafeSetClipboardText(string text)
    {
      try
      {
        Clipboard.SetText(text);
      }
      catch (Exception ex)
      {
      }
    }

    private static string GetText(TextBoxBase tb)
    {
      string s = string.IsNullOrEmpty(tb.SelectedText) ? tb.Text : tb.SelectedText;
      if (string.IsNullOrEmpty(s) && tb is IPromptText)
        s = ((IPromptText) tb).PromptText;
      return s.Left(128);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.miCut = new ToolStripMenuItem();
      this.miCopy = new ToolStripMenuItem();
      this.miPaste = new ToolStripMenuItem();
      this.miDelete = new ToolStripMenuItem();
      this.miSelectAll = new ToolStripMenuItem();
      this.miDeleteSep = new ToolStripSeparator();
      this.miUndoSep = new ToolStripSeparator();
      this.miUndo = new ToolStripMenuItem();
      this.SuspendLayout();
      this.miCut.Name = "miCut";
      this.miCut.ShortcutKeys = Keys.X | Keys.Control;
      this.miCut.Size = new System.Drawing.Size(164, 22);
      this.miCut.Text = "Cut";
      this.miCopy.Name = "miCopy";
      this.miCopy.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopy.Size = new System.Drawing.Size(164, 22);
      this.miCopy.Text = "Copy";
      this.miPaste.Name = "miPaste";
      this.miPaste.ShortcutKeys = Keys.V | Keys.Control;
      this.miPaste.Size = new System.Drawing.Size(164, 22);
      this.miPaste.Text = "Paste";
      this.miDelete.Name = "miDelete";
      this.miDelete.Size = new System.Drawing.Size(164, 22);
      this.miDelete.Text = "Delete";
      this.miSelectAll.Name = "miSelectAll";
      this.miSelectAll.ShortcutKeys = Keys.A | Keys.Control;
      this.miSelectAll.Size = new System.Drawing.Size(164, 22);
      this.miSelectAll.Text = "Select All";
      this.miDeleteSep.Name = "miDeleteSep";
      this.miDeleteSep.Size = new System.Drawing.Size(161, 6);
      this.miUndoSep.Name = "miUndoSep";
      this.miUndoSep.Size = new System.Drawing.Size(161, 6);
      this.miUndo.Name = "miUndo";
      this.miUndo.ShortcutKeys = Keys.Z | Keys.Control;
      this.miUndo.Size = new System.Drawing.Size(164, 22);
      this.miUndo.Text = "Undo";
      this.Items.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.miUndo,
        (ToolStripItem) this.miUndoSep,
        (ToolStripItem) this.miCut,
        (ToolStripItem) this.miCopy,
        (ToolStripItem) this.miPaste,
        (ToolStripItem) this.miDelete,
        (ToolStripItem) this.miDeleteSep,
        (ToolStripItem) this.miSelectAll
      });
      this.Size = new System.Drawing.Size(165, 148);
      this.ResumeLayout(false);
    }

    public class ContextMenuSource
    {
      public ContextMenuSource()
      {
      }

      public ContextMenuSource(Control control, string text)
      {
        this.Control = control;
        this.Text = text;
      }

      public Control Control { get; set; }

      public string Text { get; set; }
    }
  }
}
