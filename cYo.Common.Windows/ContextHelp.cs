// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.ContextHelp
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.IO;
using cYo.Common.Runtime;
using cYo.Common.Text;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows
{
  public class ContextHelp
  {
    private ContextHelp.HelpMessageFilter filter;
    private static Regex rxLink = new Regex("^[a-z]+:[/\\\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public ContextHelp(string helpPath)
    {
      this.Variables = new Dictionary<string, string>();
      this.HelpPath = helpPath;
    }

    public void Initialize()
    {
      if (this.filter != null)
        return;
      this.filter = new ContextHelp.HelpMessageFilter(this);
      Application.AddMessageFilter((IMessageFilter) this.filter);
    }

    public string HelpName { get; private set; }

    public string HelpPath { get; set; }

    public Dictionary<string, string> Lookup { get; set; }

    public Dictionary<string, string> Variables { get; private set; }

    public IEnumerable<string> HelpSystems
    {
      get
      {
        return FileUtility.GetFiles(this.HelpPath, SearchOption.AllDirectories, ".ini").Select<string, string>((Func<string, string>) (p => Path.GetFileNameWithoutExtension(p)));
      }
    }

    public bool ShowKey { get; set; }

    private string SubstituteVariabels(string text)
    {
      if (text != null)
      {
        foreach (string key in this.Variables.Keys)
          text = text.Replace("$" + key, this.Variables[key]);
      }
      return text;
    }

    private string GetApplication(string key)
    {
      int num = 0;
      string application = (string) null;
      while (this.Lookup.TryGetValue(++num == 1 ? key : key + num.ToString(), out application))
      {
        application = this.SubstituteVariabels(application);
        if (application != null)
        {
          if (application.StartsWith("HKEY"))
          {
            try
            {
              application = Registry.GetValue(application, string.Empty, (object) string.Empty).ToString();
            }
            catch
            {
            }
          }
        }
        if (File.Exists(application))
          return application;
      }
      return (string) null;
    }

    private void StartLink(string link)
    {
      string format = (string) null;
      if (this.Lookup != null)
        this.Lookup.TryGetValue("HelpLink", out format);
      link = this.SubstituteVariabels(link).Trim();
      if (ContextHelp.rxLink.IsMatch(link))
      {
        ContextHelp.StartDocument(link);
      }
      else
      {
        string application = this.GetApplication("HelpApp");
        if (!string.IsNullOrEmpty(format))
          link = this.SubstituteVariabels(string.Format(format, (object) link));
        if (string.IsNullOrEmpty(application))
          ContextHelp.StartDocument(link);
        else
          ContextHelp.StartProgram(application, link);
      }
    }

    private bool HelpRequest(Control c)
    {
      try
      {
        if (c == null)
          c = Form.ActiveForm.ActiveControl;
        Control control = c.TopParent();
        while (this.Lookup != null)
        {
          if (c != null)
          {
            string str = c.Name;
            if (control != null && control.Name != str)
              str = control.Name + ":" + str;
            if (this.ShowKey)
            {
              int num = (int) MessageBox.Show(str);
            }
            string link;
            if (this.Lookup.TryGetValue(str, out link))
            {
              this.StartLink(link);
              return true;
            }
            c = c.Parent;
          }
          else
            break;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool Load(string help)
    {
      this.Lookup = IniFile.ReadFile(Path.Combine(this.HelpPath, help) + ".ini");
      this.HelpName = help;
      return this.Lookup.Count != 0;
    }

    public IEnumerable<ToolStripItem> GetCustomHelpMenu()
    {
      if (this.Lookup != null)
      {
        foreach (var data in this.Lookup.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (n => n.Key.StartsWith("HelpMenu"))).Select(n =>
        {
          string[] array = ((IEnumerable<string>) n.Value.Split(';')).TrimStrings().RemoveEmpty().ToArray<string>();
          return new
          {
            Text = array[0],
            Document = array.Length < 2 ? string.Empty : this.SubstituteVariabels(array[1])
          };
        }))
        {
          var n = data;
          if (n.Text == "-")
            yield return (ToolStripItem) new ToolStripSeparator();
          else if (!string.IsNullOrEmpty(n.Document))
            yield return (ToolStripItem) new ToolStripMenuItem(n.Text, (Image) null, (EventHandler) ((s, e) => ContextHelp.StartDocument(n.Document)));
        }
      }
    }

    public bool Execute(string lookup)
    {
      string text;
      if (this.Lookup == null || !this.Lookup.TryGetValue(lookup, out text))
        return false;
      this.StartLink(this.SubstituteVariabels(text));
      return true;
    }

    public static void StartDocument(string document)
    {
      try
      {
        Process.Start(document);
      }
      catch (Exception ex)
      {
      }
    }

    public static void StartProgram(string exe, string commandLine)
    {
      try
      {
        Process.Start(exe, commandLine);
      }
      catch (Exception ex)
      {
      }
    }

    private class HelpMessageFilter : IMessageFilter
    {
      private ContextHelp contextHelp;

      public HelpMessageFilter(ContextHelp contextHelp) => this.contextHelp = contextHelp;

      bool IMessageFilter.PreFilterMessage(ref Message m)
      {
        return m.Msg == 256 && (int) m.WParam == 112 && Control.ModifierKeys == Keys.None && this.contextHelp.HelpRequest(Control.FromHandle(m.HWnd));
      }
    }
  }
}
