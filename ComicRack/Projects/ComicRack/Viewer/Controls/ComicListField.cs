// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.ComicListField
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class ComicListField
  {
    public ComicListField()
    {
    }

    public ComicListField(
      string displayProperty,
      string description,
      string editProperty = null,
      StringTrimming trimming = StringTrimming.EllipsisCharacter,
      Type valueType = null,
      string defaultText = null)
      : this()
    {
      this.DisplayProperty = displayProperty;
      this.Description = description;
      this.EditProperty = editProperty;
      this.Trimming = trimming;
      this.ValueType = valueType;
      this.DefaultText = defaultText;
    }

    [DefaultValue(null)]
    public string DisplayProperty { get; set; }

    [DefaultValue(null)]
    public string EditProperty { get; set; }

    [DefaultValue(null)]
    public string Description { get; set; }

    public StringTrimming Trimming { get; set; }

    public Type ValueType { get; set; }

    public string DefaultText { get; set; }

    public static void TranslateColumns(IEnumerable<IColumn> itemViewColumnCollection)
    {
      TR tr = TR.Load("Columns");
      foreach (ItemViewColumn itemViewColumn in itemViewColumnCollection)
      {
        ComicListField tag = (ComicListField) itemViewColumn.Tag;
        string displayProperty = tag.DisplayProperty;
        itemViewColumn.Text = tr[displayProperty, itemViewColumn.Text];
        tag.Description = tr[displayProperty + ".Description", tag.Description];
        for (int index = 0; index < itemViewColumn.FormatTexts.Length; ++index)
          itemViewColumn.FormatTexts[index] = tr[displayProperty + ".Format" + (object) (index + 1), itemViewColumn.FormatTexts[index]];
      }
    }
  }
}
