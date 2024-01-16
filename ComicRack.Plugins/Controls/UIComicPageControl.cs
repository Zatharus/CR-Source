// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.Controls.UIComicPageControl
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins.Controls
{
  public class UIComicPageControl : ComicPageControl
  {
    public UIComicPageControl(Control c) => this.Plugin = c;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Plugin.Dispose();
      base.Dispose(disposing);
    }

    public Control Plugin
    {
      get => this.Controls.Count != 0 ? this.Controls[0] : (Control) null;
      set
      {
        if (this.Plugin == value)
          return;
        if (this.Plugin != null)
        {
          Control plugin = this.Plugin;
          this.Controls.Remove(plugin);
          plugin.Dispose();
        }
        value.Dock = DockStyle.Fill;
        value.Visible = true;
        this.Controls.Add(value);
      }
    }

    public Func<Control> CreatePlugin { get; set; }

    protected override void OnShowInfo(IEnumerable<ComicBook> books)
    {
      base.OnShowInfo(books);
      object plugin = (object) this.Plugin;
      try
      {
        // ISSUE: reference to a compiler-generated field
        if (UIComicPageControl.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UIComicPageControl.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, ComicBook[]>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "ShowInfo", (IEnumerable<Type>) null, typeof (UIComicPageControl), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        UIComicPageControl.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) UIComicPageControl.\u003C\u003Eo__9.\u003C\u003Ep__0, plugin, books.ToArray<ComicBook>());
      }
      catch (Exception ex)
      {
        Trace.WriteLine("Failed to execute view plugin: " + ex.Message);
      }
    }

    protected override bool ProcessKeyPreview(ref Message m)
    {
      if (this.CreatePlugin != null && m.Msg == 256)
      {
        KeyEventArgs keyEventArgs = new KeyEventArgs((Keys) (long) m.WParam | Control.ModifierKeys);
        Trace.WriteLine((object) keyEventArgs.KeyCode);
        if (keyEventArgs.KeyCode == Keys.R && Control.ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt))
        {
          Control control = (Control) null;
          try
          {
            control = this.CreatePlugin();
          }
          catch (Exception ex)
          {
          }
          if (control != null)
            this.Plugin = control;
        }
      }
      return base.ProcessKeyPreview(ref m);
    }
  }
}
