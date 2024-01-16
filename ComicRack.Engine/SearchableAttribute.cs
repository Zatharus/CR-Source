// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.SearchableAttribute
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class SearchableAttribute : Attribute
  {
    public SearchableAttribute(bool searchable) => this.Searchable = searchable;

    public SearchableAttribute()
      : this(true)
    {
    }

    public bool Searchable { get; private set; }

    public static bool IsSearchable(PropertyInfo pi)
    {
      return pi.GetCustomAttributes(true).OfType<SearchableAttribute>().FirstOrDefault<SearchableAttribute>((Func<SearchableAttribute, bool>) (a => a.Searchable)) != null;
    }
  }
}
