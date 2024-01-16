// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookMatcherHintAttribute
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ComicBookMatcherHintAttribute : Attribute
  {
    private ComicBookMatcherHintAttribute(IEnumerable<string> names)
    {
      this.Properties = (ISet<string>) new HashSet<string>(names.SelectMany<string, string>(new Func<string, IEnumerable<string>>(ComicBookMatcherHintAttribute.SplitName)));
    }

    public ComicBookMatcherHintAttribute(bool disable) => this.DisableOptimizedUpdate = disable;

    public ComicBookMatcherHintAttribute(string name)
      : this(ListExtensions.AsEnumerable<string>(name))
    {
    }

    public ComicBookMatcherHintAttribute(string name1, string name2)
      : this(ListExtensions.AsEnumerable<string>(name1, name2))
    {
    }

    public ComicBookMatcherHintAttribute(string name1, string name2, string name3)
      : this(ListExtensions.AsEnumerable<string>(name1, name2, name3))
    {
    }

    public ComicBookMatcherHintAttribute(string name1, string name2, string name3, string name4)
      : this(ListExtensions.AsEnumerable<string>(name1, name2, name3, name4))
    {
    }

    public ComicBookMatcherHintAttribute(
      string name1,
      string name2,
      string name3,
      string name4,
      string name5)
      : this(ListExtensions.AsEnumerable<string>(name1, name2, name3, name4, name5))
    {
    }

    public ComicBookMatcherHintAttribute(
      string name1,
      string name2,
      string name3,
      string name4,
      string name5,
      string name6)
      : this(ListExtensions.AsEnumerable<string>(name1, name2, name3, name4, name5, name6))
    {
    }

    public ComicBookMatcherHintAttribute(
      string name1,
      string name2,
      string name3,
      string name4,
      string name5,
      string name6,
      string name7)
      : this(ListExtensions.AsEnumerable<string>(name1, name2, name3, name4, name5, name6, name7))
    {
    }

    public ISet<string> Properties { get; private set; }

    public bool DisableOptimizedUpdate { get; set; }

    private static IEnumerable<string> SplitName(string name)
    {
      return ((IEnumerable<string>) name.Split(',')).TrimStrings().RemoveEmpty();
    }
  }
}
