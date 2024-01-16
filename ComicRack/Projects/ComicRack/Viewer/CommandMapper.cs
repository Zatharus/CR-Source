// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.CommandMapper
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Win32;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public class CommandMapper : Component
  {
    private readonly Dictionary<object, CommandMapper.HandleItem> ht = new Dictionary<object, CommandMapper.HandleItem>();
    private bool enable = true;
    private bool handleShield;

    public CommandMapper(bool enable)
    {
      this.enable = enable;
      IdleProcess.Idle += new EventHandler(this.ApplicationIdle);
    }

    public CommandMapper()
      : this(true)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        IdleProcess.Idle -= new EventHandler(this.ApplicationIdle);
      base.Dispose(disposing);
    }

    public void Add(
      CommandHandler clickHandler,
      UpdateHandler enabledHandler,
      UpdateHandler checkedHandler,
      params object[] senders)
    {
      foreach (object sender in senders)
      {
        CommandMapper.HandleItem hi = new CommandMapper.HandleItem(sender, clickHandler, enabledHandler, checkedHandler);
        this.ht[sender] = hi;
        switch (sender)
        {
          case ToolStripItem toolStripItem:
            if (toolStripItem is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.ShortcutKeys != Keys.None)
              hi.ForcedUpdate = true;
            else if (!toolStripItem.IsOnOverflow)
            {
              if (toolStripItem.GetCurrentParent() is ContextMenuStrip currentParent2)
              {
                currentParent2.Opening += (CancelEventHandler) ((s, e) => this.IdleUpdate(hi, true));
                hi.IdleUpdate = false;
              }
              else if (toolStripItem.GetCurrentParent() is ToolStripDropDown currentParent1)
              {
                currentParent1.Opening += (CancelEventHandler) ((s, e) => this.IdleUpdate(hi, true));
                hi.IdleUpdate = false;
              }
            }
            if (toolStripItem is ToolStripSplitButton stripSplitButton)
            {
              stripSplitButton.ButtonClick += new EventHandler(this.CommandMapperClick);
              break;
            }
            toolStripItem.Click += new EventHandler(this.CommandMapperClick);
            break;
          case ButtonBase buttonBase:
            buttonBase.Click += new EventHandler(this.CommandMapperClick);
            break;
        }
      }
    }

    public void Add(
      CommandHandler clickHandler,
      bool isCheckedHandler,
      UpdateHandler updateHandler,
      params object[] senders)
    {
      if (isCheckedHandler)
        this.Add(clickHandler, (UpdateHandler) null, updateHandler, senders);
      else
        this.Add(clickHandler, updateHandler, (UpdateHandler) null, senders);
    }

    public void Add(
      CommandHandler clickHandler,
      UpdateHandler enableHandler,
      params object[] senders)
    {
      this.Add(clickHandler, enableHandler, (UpdateHandler) null, senders);
    }

    public void Add(CommandHandler ch, params object[] senders)
    {
      this.Add(ch, (UpdateHandler) null, senders);
    }

    public bool Handle(object sender)
    {
      if (!this.ht.ContainsKey(sender))
        return false;
      this.HandleCommandItem(this.ht[sender]);
      return true;
    }

    public bool InvokeKey(Keys shortcutKeys)
    {
      foreach (CommandMapper.HandleItem handleItem in this.ht.Values)
      {
        if (handleItem.ShortcutKeys == shortcutKeys)
        {
          this.HandleCommandItem(handleItem);
          return true;
        }
      }
      return false;
    }

    public void AddService<T>(
      Control c,
      ServiceCommandHandler<T> clickHandler,
      ServiceUpdateHandler<T> enabledHandler,
      ServiceUpdateHandler<T> checkedHandler,
      params object[] senders)
      where T : class
    {
      CommandHandler clickHandler1 = (CommandHandler) null;
      UpdateHandler enabledHandler1 = (UpdateHandler) null;
      UpdateHandler checkedHandler1 = (UpdateHandler) null;
      if (clickHandler != null)
        clickHandler1 = (CommandHandler) (() =>
        {
          T activeService = c.FindActiveService<T>();
          if ((object) activeService == null)
            return;
          clickHandler(activeService);
        });
      if (enabledHandler != null)
        enabledHandler1 = (UpdateHandler) (() =>
        {
          T activeService = c.FindActiveService<T>();
          return (object) activeService != null && enabledHandler(activeService);
        });
      if (checkedHandler != null)
        checkedHandler1 = (UpdateHandler) (() =>
        {
          T activeService = c.FindActiveService<T>();
          return (object) activeService != null && checkedHandler(activeService);
        });
      this.Add(clickHandler1, enabledHandler1, checkedHandler1, senders);
    }

    public void AddService<T>(
      Control c,
      ServiceCommandHandler<T> clickHandler,
      bool isCheckedHandler,
      ServiceUpdateHandler<T> updateHandler,
      params object[] senders)
      where T : class
    {
      if (isCheckedHandler)
        this.AddService<T>(c, clickHandler, (ServiceUpdateHandler<T>) null, updateHandler, senders);
      else
        this.AddService<T>(c, clickHandler, updateHandler, (ServiceUpdateHandler<T>) null, senders);
    }

    public void AddService<T>(
      Control c,
      ServiceCommandHandler<T> clickHandler,
      ServiceUpdateHandler<T> enableHandler,
      params object[] senders)
      where T : class
    {
      this.AddService<T>(c, clickHandler, enableHandler, (ServiceUpdateHandler<T>) null, senders);
    }

    public void AddService<T>(Control c, ServiceCommandHandler<T> ch, params object[] senders) where T : class
    {
      this.AddService<T>(c, ch, (ServiceUpdateHandler<T>) null, senders);
    }

    public bool Enable
    {
      get => this.enable;
      set => this.enable = value;
    }

    private void HandleCommandItem(CommandMapper.HandleItem item)
    {
      if (item.Update != null && !item.Update())
        return;
      CommandHandler command = item.Command;
      if (command == null)
        return;
      command();
    }

    private void CommandMapperClick(object sender, EventArgs e)
    {
      if (this.handleShield)
        return;
      this.handleShield = true;
      try
      {
        this.Handle(sender);
      }
      finally
      {
        this.handleShield = false;
      }
    }

    private void IdleUpdate(CommandMapper.HandleItem hi, bool forced = false)
    {
      forced |= hi.ForcedUpdate;
      if (!forced && !hi.IsVisible)
        return;
      if (hi.Update != null)
      {
        bool flag = hi.Update();
        if (hi.Sender is ToolStripItem sender2)
          sender2.Enabled = flag;
        else if (hi.Sender is ButtonBase sender1)
          sender1.Enabled = flag;
      }
      if (hi.Check == null)
        return;
      bool flag1 = hi.Check();
      if (hi.Sender is ToolStripButton sender3)
        sender3.Checked = flag1;
      if (!(hi.Sender is ToolStripMenuItem sender4))
        return;
      sender4.Checked = flag1;
    }

    private void ApplicationIdle(object sender, EventArgs e)
    {
      if (!this.enable)
        return;
      foreach (CommandMapper.HandleItem hi in this.ht.Values)
      {
        if (hi.IdleUpdate)
          this.IdleUpdate(hi);
      }
    }

    private class HandleItem
    {
      public readonly object Sender;
      public readonly CommandHandler Command;
      public readonly UpdateHandler Update;
      public readonly UpdateHandler Check;
      private readonly Func<bool> checkVisibility;
      public bool IdleUpdate;
      public bool ForcedUpdate;

      public HandleItem(
        object sender,
        CommandHandler command,
        UpdateHandler update,
        UpdateHandler check)
      {
        this.Sender = sender;
        this.Command = command;
        this.Update = update;
        this.Check = check;
        this.IdleUpdate = true;
        if (this.Sender is ToolStripItem)
        {
          ToolStripItem tsi = (ToolStripItem) this.Sender;
          this.checkVisibility = (Func<bool>) (() => tsi.Visible);
        }
        else if (this.Sender is ButtonBase)
        {
          ButtonBase bb = (ButtonBase) this.Sender;
          this.checkVisibility = (Func<bool>) (() => bb.Visible);
        }
        else
          this.checkVisibility = (Func<bool>) (() => false);
      }

      public bool IsVisible => this.checkVisibility();

      public Keys ShortcutKeys
      {
        get => this.Sender is ToolStripMenuItem sender ? sender.ShortcutKeys : Keys.None;
      }
    }
  }
}
