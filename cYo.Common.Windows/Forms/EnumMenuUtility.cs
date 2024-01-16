// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.EnumMenuUtility
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class EnumMenuUtility
  {
    private readonly bool flagsMode;
    private readonly bool isFlags;
    private readonly Type enumType;
    private readonly ToolStripItem[] items;
    private int enumValue;

    protected EnumMenuUtility(
      Type enumType,
      bool flagsMode,
      IDictionary<int, Image> images,
      Keys startKey)
    {
      this.enumType = enumType;
      this.isFlags = Attribute.IsDefined((MemberInfo) enumType, typeof (FlagsAttribute));
      this.flagsMode = flagsMode && this.isFlags;
      this.items = this.MakeEnumMenu(images, startKey);
    }

    public EnumMenuUtility(
      ToolStripDropDownItem item,
      Type enumType,
      bool flagsMode,
      IDictionary<int, Image> images,
      Keys startKey)
      : this(enumType, flagsMode, images, startKey)
    {
      item.DropDownItems.AddRange(this.items);
      item.DropDownOpening += new EventHandler(this.DropDownOpening);
    }

    public EnumMenuUtility(
      ToolStripDropDown cms,
      Type enumType,
      bool flagsMode,
      IDictionary<int, Image> images,
      Keys startKey)
      : this(enumType, flagsMode, images, startKey)
    {
      cms.Items.AddRange(this.items);
      cms.Opening += new CancelEventHandler(this.ContextMenuOpening);
    }

    public bool FlagsMode => this.flagsMode;

    public bool IsFlags => this.isFlags;

    public Type EnumType => this.enumType;

    public IEnumerable<ToolStripItem> Items => (IEnumerable<ToolStripItem>) this.items;

    public string Text
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (object obj in Enum.GetValues(this.enumType))
        {
          int int32 = Convert.ToInt32(obj);
          if ((this.isFlags && BitUtility.GetBitCount(int32) == 1 || !this.isFlags) && (this.Value & int32) == int32)
          {
            if (stringBuilder.Length != 0)
              stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
            stringBuilder.Append(LocalizeUtility.LocalizeEnum(this.enumType, int32));
          }
        }
        return stringBuilder.ToString();
      }
    }

    public int Value
    {
      get => this.enumValue;
      set
      {
        if (this.enumValue == value)
          return;
        this.enumValue = value;
        this.OnValueChanged();
      }
    }

    public bool Enabled
    {
      get
      {
        return ((IEnumerable<ToolStripItem>) this.items).All<ToolStripItem>((Func<ToolStripItem, bool>) (ti => ti.Enabled));
      }
      set
      {
        ((IEnumerable<ToolStripItem>) this.items).ForEach<ToolStripItem>((Action<ToolStripItem>) (ti => ti.Enabled = value));
      }
    }

    public event EventHandler ValueChanged;

    protected virtual void OnValueChanged()
    {
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, EventArgs.Empty);
    }

    private ToolStripItem[] MakeEnumMenu(IDictionary<int, Image> images, Keys startKey)
    {
      List<ToolStripItem> toolStripItemList = new List<ToolStripItem>();
      foreach (object obj in Enum.GetValues(this.enumType))
      {
        int int32 = Convert.ToInt32(obj);
        BrowsableAttribute browsableAttribute = this.enumType.GetField(Enum.GetName(this.enumType, obj)).GetCustomAttributes(typeof (BrowsableAttribute), true).OfType<BrowsableAttribute>().FirstOrDefault<BrowsableAttribute>();
        if ((browsableAttribute == null || browsableAttribute.Browsable) && (this.isFlags && BitUtility.GetBitCount(int32) == 1 || !this.isFlags))
        {
          ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
          toolStripMenuItem1.Text = LocalizeUtility.LocalizeEnum(this.enumType, int32);
          toolStripMenuItem1.Tag = (object) int32;
          ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
          toolStripMenuItem2.Click += new EventHandler(this.EnumClicked);
          if (images != null)
            toolStripMenuItem2.Image = images[int32];
          if (startKey != Keys.None)
            toolStripMenuItem2.ShortcutKeys = startKey + toolStripItemList.Count;
          toolStripItemList.Add((ToolStripItem) toolStripMenuItem2);
        }
      }
      if (this.flagsMode)
      {
        toolStripItemList.Add((ToolStripItem) new ToolStripSeparator());
        toolStripItemList.Add((ToolStripItem) new ToolStripMenuItem(TR.Default["SetAll", "Set all"], (Image) null, new EventHandler(this.EnumAllClicked)));
        toolStripItemList.Add((ToolStripItem) new ToolStripMenuItem(TR.Default["ClearAll", "Clear all"], (Image) null, new EventHandler(this.EnumNoneClicked)));
        toolStripItemList.Add((ToolStripItem) new ToolStripMenuItem(TR.Default["Invert", "Invert"], (Image) null, new EventHandler(this.EnumInvertClicked)));
      }
      return toolStripItemList.ToArray();
    }

    private int GetFlagValue()
    {
      int flagValue = 0;
      foreach (object obj in Enum.GetValues(this.enumType))
      {
        if (!obj.Equals((object) uint.MaxValue))
          flagValue |= Convert.ToInt32(obj);
      }
      return flagValue;
    }

    private void UpdateItems()
    {
      foreach (ToolStripMenuItem toolStripMenuItem in this.items.OfType<ToolStripMenuItem>())
      {
        if (toolStripMenuItem.Tag != null)
        {
          int tag = (int) toolStripMenuItem.Tag;
          toolStripMenuItem.Checked = !this.flagsMode ? this.Value.Equals(toolStripMenuItem.Tag) : (this.Value & tag) == tag;
        }
      }
    }

    private void EnumClicked(object sender, EventArgs e)
    {
      ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem) sender;
      if (this.flagsMode)
        this.Value = this.enumValue ^ Convert.ToInt32(toolStripMenuItem.Tag);
      else
        this.Value = (int) toolStripMenuItem.Tag;
    }

    private void EnumAllClicked(object sender, EventArgs e) => this.Value = this.GetFlagValue();

    private void EnumNoneClicked(object sender, EventArgs e) => this.Value = 0;

    private void EnumInvertClicked(object sender, EventArgs e) => this.Value = ~this.Value;

    private void DropDownOpening(object sender, EventArgs e) => this.UpdateItems();

    private void ContextMenuOpening(object sender, CancelEventArgs e) => this.UpdateItems();

    public override string ToString() => this.Text;
  }
}
