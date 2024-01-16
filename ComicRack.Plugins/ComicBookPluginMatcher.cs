// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.ComicBookPluginMatcher
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Projects.ComicRack.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  [System.ComponentModel.Description("User Scripts")]
  [ComicBookMatcherHint(true)]
  [Serializable]
  public class ComicBookPluginMatcher : ComicBookValueMatcher
  {
    public static PluginEngine PluginEngine { get; set; }

    public static IEnumerable<Command> Commands
    {
      get
      {
        try
        {
          return ComicBookPluginMatcher.PluginEngine.GetCommands("CreateBookList");
        }
        catch
        {
          return Enumerable.Empty<Command>();
        }
      }
    }

    [XmlAttribute]
    [DefaultValue(null)]
    public string PluginKey { get; set; }

    public override int ArgumentCount
    {
      get
      {
        try
        {
          return ComicBookPluginMatcher.Commands.FirstOrDefault<Command>((Func<Command, bool>) (cmd => cmd.Key == this.PluginKey)).PCount.Clamp(0, 2);
        }
        catch
        {
          return 0;
        }
      }
    }

    public override string[] OperatorsList
    {
      get
      {
        List<string> stringList = new List<string>();
        stringList.Add(TR.Default["None", "None"]);
        stringList.AddRange(ComicBookPluginMatcher.Commands.Select<Command, string>((Func<Command, string>) (cmd => cmd.GetLocalizedName())));
        return stringList.ToArray();
      }
    }

    public override string[] OperatorsListNeutral => this.OperatorsList;

    public override object Clone()
    {
      ComicBookPluginMatcher bookPluginMatcher = (ComicBookPluginMatcher) base.Clone();
      bookPluginMatcher.PluginKey = this.PluginKey;
      return (object) bookPluginMatcher;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    public override int MatchOperator
    {
      get
      {
        int matchOperator = 1;
        foreach (Command command in ComicBookPluginMatcher.Commands)
        {
          if (command.Key == this.PluginKey)
            return matchOperator;
          ++matchOperator;
        }
        return 0;
      }
      set => base.MatchOperator = value;
    }

    public override bool Set(ComicBookValueMatcher matcher)
    {
      bool flag = base.Set(matcher);
      ComicBookPluginMatcher bookPluginMatcher = matcher as ComicBookPluginMatcher;
      if (flag && bookPluginMatcher != null)
        this.PluginKey = bookPluginMatcher.PluginKey;
      return flag;
    }

    protected override void OnMatchOperatorChanged()
    {
      base.OnMatchOperatorChanged();
      try
      {
        if (base.MatchOperator == 0)
          this.PluginKey = (string) null;
        else
          this.PluginKey = ComicBookPluginMatcher.Commands.ElementAt<Command>(base.MatchOperator - 1).Key;
      }
      catch
      {
        this.PluginKey = (string) null;
      }
    }

    public override IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
    {
      try
      {
        Command command = ComicBookPluginMatcher.Commands.FirstOrDefault<Command>((Func<Command, bool>) (c => c.Key == this.PluginKey));
        if (command == null)
          return items;
        return (IEnumerable<ComicBook>) command.Invoke(new object[3]
        {
          (object) items.ToArray<ComicBook>(),
          (object) this.MatchValue,
          (object) this.MatchValue2
        });
      }
      catch
      {
        return Enumerable.Empty<ComicBook>();
      }
    }
  }
}
