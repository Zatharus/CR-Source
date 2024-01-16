// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.EditControlUtility
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public static class EditControlUtility
  {
    public static string GetText(TextBox control, string compare = null, bool trim = true)
    {
      string text = control.Text;
      if (trim)
        text = text.Trim();
      if (text != compare && control.AutoCompleteCustomSource != null)
        control.AutoCompleteCustomSource.Add(text);
      return text;
    }

    public static int GetNumber(Control control)
    {
      int result;
      return int.TryParse(control.Text, out result) && result >= 0 ? result : -1;
    }

    public static float GetRealNumber(Control control)
    {
      float result;
      return float.TryParse(control.Text, out result) && (double) result >= 0.0 ? result : -1f;
    }

    public static void SetLabel(Label label, string text, bool enabled)
    {
      label.Text = string.IsNullOrEmpty(text) ? "Unknown" : text;
      label.Enabled = !string.IsNullOrEmpty(text) & enabled;
    }

    public static void SetLabel(Label label, string text)
    {
      EditControlUtility.SetLabel(label, text, true);
    }

    public static void SetText(Control control, string text)
    {
      if (text == null)
        return;
      try
      {
        control.Text = text;
      }
      catch (Exception ex)
      {
      }
    }

    public static void SetText(
      TextBox textBox,
      string text,
      Func<AutoCompleteStringCollection> autoCompletePredicate)
    {
      EditControlUtility.SetText((Control) textBox, text);
      if (textBox is IDelayedAutoCompleteList autoCompleteList)
      {
        if (autoCompletePredicate != null)
          autoCompleteList.SetLazyAutoComplete(autoCompletePredicate);
        else
          autoCompleteList.ResetLazyAutoComplete();
      }
      else
      {
        textBox.AutoCompleteCustomSource = autoCompletePredicate();
        textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        textBox.AutoCompleteMode = AutoCompleteMode.Append;
      }
    }

    public static void SetText(
      ComboBox c,
      string text,
      Func<AutoCompleteStringCollection> autoCompletePredicate,
      bool onlyDirectList = false)
    {
      if (autoCompletePredicate != null)
      {
        Func<IEnumerable<string>> allItems = (Func<IEnumerable<string>>) (() =>
        {
          HashSet<string> list = new HashSet<string>()
          {
            string.Empty
          };
          list.AddRange<string>(autoCompletePredicate().Cast<string>());
          return (IEnumerable<string>) list;
        });
        if (c.DataSource == null)
        {
          if (!onlyDirectList)
            c.Enter += (EventHandler) ((s, e) =>
            {
              string text1 = c.Text;
              c.DataSource = (object) ComboBoxSkinner.AutoSeparatorList((IEnumerable) allItems());
              c.Text = text1;
            });
        }
        else
          c.DataSource = (object) ComboBoxSkinner.AutoSeparatorList((IEnumerable) allItems());
      }
      EditControlUtility.SetText((Control) c, text);
    }

    public static void SetText(Control control, int value)
    {
      control.Text = value == -1 ? string.Empty : value.ToString();
    }

    public static void SetText(Control control, float value)
    {
      control.Text = (double) value == -1.0 ? string.Empty : value.ToString();
    }

    public static void SetText(Control c, bool value)
    {
      c.Text = value ? ComicInfo.YesText : ComicInfo.NoText;
    }

    public static void SetText(Control c, YesNo yn) => c.Text = ComicInfo.GetYesNoAsText(yn);

    public static void SetText(Control c, MangaYesNo yn) => c.Text = ComicInfo.GetYesNoAsText(yn);

    public static YesNo GetYesNo(string text)
    {
      if (string.Equals(text, ComicInfo.YesText, StringComparison.OrdinalIgnoreCase))
        return YesNo.Yes;
      return !string.Equals(text, ComicInfo.NoText, StringComparison.OrdinalIgnoreCase) ? YesNo.Unknown : YesNo.No;
    }

    public static MangaYesNo GetMangaYesNo(string text)
    {
      if (string.Equals(text, ComicInfo.YesText, StringComparison.OrdinalIgnoreCase))
        return MangaYesNo.Yes;
      if (string.Equals(text, ComicInfo.YesRightToLeftText, StringComparison.OrdinalIgnoreCase))
        return MangaYesNo.YesAndRightToLeft;
      return string.Equals(text, ComicInfo.NoText, StringComparison.OrdinalIgnoreCase) ? MangaYesNo.No : MangaYesNo.Unknown;
    }

    public static void InitializeYesNo(ComboBox cb, bool withEmpty = true)
    {
      if (withEmpty)
        cb.Items.Add((object) string.Empty);
      cb.Items.Add((object) ComicInfo.YesText);
      cb.Items.Add((object) ComicInfo.NoText);
    }

    public static void InitializeMangaYesNo(ComboBox cb, bool withEmpty = true)
    {
      if (withEmpty)
        cb.Items.Add((object) string.Empty);
      cb.Items.Add((object) ComicInfo.YesText);
      cb.Items.Add((object) ComicInfo.YesRightToLeftText);
      cb.Items.Add((object) ComicInfo.NoText);
    }
  }
}
