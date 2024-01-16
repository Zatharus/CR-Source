// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.ListConfiguration
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Serializable]
  public class ListConfiguration : IComparable<ListConfiguration>, INamed, IDescription
  {
    public ListConfiguration()
      : this(string.Empty)
    {
    }

    public ListConfiguration(string name) => this.Name = name;

    [DefaultValue("")]
    public string Name { get; set; }

    [DefaultValue(null)]
    public DisplayListConfig Config { get; set; }

    public string Description
    {
      get
      {
        return LocalizeUtility.LocalizeEnum(typeof (ItemViewMode), (int) this.Config.View.ItemViewMode);
      }
    }

    public int CompareTo(ListConfiguration other) => string.Compare(this.Name, other.Name);

    public override string ToString() => this.Name;
  }
}
