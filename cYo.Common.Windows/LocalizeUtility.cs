// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.LocalizeUtility
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Common.Windows
{
  public static class LocalizeUtility
  {
    public static void Localize(TR tr, ToolStripItemCollection tsic)
    {
      foreach (ToolStripItem tsi in (ArrangedElementCollection) tsic)
        LocalizeUtility.Localize(tr, tsi);
      FormUtility.PrefixToolStrip(tsic);
    }

    public static void Localize(TR tr, ToolStripItem tsi)
    {
      switch (tsi)
      {
        case ToolStripTextBox _:
          break;
        case ToolStripComboBox _:
          break;
        default:
          bool flag = tsi.ToolTipText != tsi.Text;
          tsi.Text = tr[tsi.Name, tsi.Text];
          tsi.ToolTipText = flag ? tr[tsi.Name + ".Tooltip", tsi.ToolTipText] : tsi.Text;
          switch (tsi)
          {
            case ToolStripDropDownItem stripDropDownItem when stripDropDownItem.HasDropDownItems:
              LocalizeUtility.Localize(tr, stripDropDownItem.DropDownItems);
              return;
            case ToolStripDropDownButton stripDropDownButton when stripDropDownButton.HasDropDownItems:
              LocalizeUtility.Localize(tr, stripDropDownButton.DropDownItems);
              return;
            default:
              return;
          }
      }
    }

    public static void Localize(TR tr, ToolStrip ts) => LocalizeUtility.Localize(tr, ts.Items);

    public static void Localize(TR tr, Control c)
    {
      switch (c)
      {
        case Label _:
        case GroupBox _:
        case CollapsibleGroupBox _:
        case ButtonBase _:
        case Form _:
          c.Text = tr[c.Name, c.Text];
          break;
        case ToolStrip _:
          LocalizeUtility.Localize(tr, (ToolStrip) c);
          break;
        case TabControl _:
          IEnumerator enumerator1 = (c as TabControl).TabPages.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              TabPage current = (TabPage) enumerator1.Current;
              current.Text = tr[current.Name, current.Text];
            }
            break;
          }
          finally
          {
            if (enumerator1 is IDisposable disposable)
              disposable.Dispose();
          }
        case TabBar _:
          using (IEnumerator<TabBar.TabBarItem> enumerator2 = (c as TabBar).Items.GetEnumerator())
          {
            while (enumerator2.MoveNext())
            {
              TabBar.TabBarItem current = enumerator2.Current;
              current.Text = tr[current.Name, current.Text];
              current.ToolTipText = tr[current.Name + ".Tooltip", current.ToolTipText];
            }
            break;
          }
        case ListView _:
          ListView listView = (ListView) c;
          foreach (ColumnHeader column in listView.Columns)
            column.Text = tr["col" + column.Text, column.Text];
          IEnumerator enumerator3 = listView.Groups.GetEnumerator();
          try
          {
            while (enumerator3.MoveNext())
            {
              ListViewGroup current = (ListViewGroup) enumerator3.Current;
              current.Header = tr[current.Name, current.Header];
            }
            break;
          }
          finally
          {
            if (enumerator3 is IDisposable disposable)
              disposable.Dispose();
          }
        case DataGridView _:
          IEnumerator enumerator4 = ((DataGridView) c).Columns.GetEnumerator();
          try
          {
            while (enumerator4.MoveNext())
            {
              DataGridViewColumn current = (DataGridViewColumn) enumerator4.Current;
              current.HeaderText = tr[current.Name, current.HeaderText];
            }
            break;
          }
          finally
          {
            if (enumerator4 is IDisposable disposable)
              disposable.Dispose();
          }
      }
      foreach (Control control in (ArrangedElementCollection) c.Controls)
      {
        if (!(control is UserControl))
          LocalizeUtility.Localize(tr, control);
      }
    }

    public static void Localize(TR tr, ComboBox cb)
    {
      for (int index = 0; index < cb.Items.Count; ++index)
        cb.Items[index] = (object) tr[cb.Name + ".Item" + (object) index, cb.Items[index] as string];
    }

    public static void Localize(Control control, string controlName, IContainer components)
    {
      TR tr = TR.Load(controlName);
      LocalizeUtility.Localize(tr, control);
      if (components == null || components.Components == null)
        return;
      foreach (ToolStrip ts in components.Components.OfType<ToolStrip>())
        LocalizeUtility.Localize(tr, ts);
    }

    public static void Localize(Control control, IContainer components)
    {
      LocalizeUtility.Localize(control, control.Name, components);
    }

    public static string GetText(Control control, string key, string value)
    {
      return TR.Load(control.GetType().Name)[key, value];
    }

    public static void UpdateRightToLeft(Form f)
    {
      if (!TR.Info.RightToLeft)
        return;
      f.RightToLeft = RightToLeft.Yes;
    }

    public static string LocalizeEnum(Type enumType, int value)
    {
      string name = Enum.GetName(enumType, (object) value);
      return TR.Load(enumType.Name)[name, LocalizeUtility.GetEnumDescription(enumType, name).PascalToSpaced()];
    }

    private static string GetEnumDescription(Type enumType, string name)
    {
      DescriptionAttribute descriptionAttribute = enumType.GetField(name).GetCustomAttributes(typeof (DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault<DescriptionAttribute>();
      return descriptionAttribute != null ? descriptionAttribute.Description : name;
    }
  }
}
