// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.LanguageComboBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class LanguageComboBox : ComboBox
  {
    private CultureTypes cultureTypes = CultureTypes.NeutralCultures;
    private IEnumerable<string> topTwoLetterISOLanguages;

    public LanguageComboBox()
    {
      this.DropDownStyle = ComboBoxStyle.DropDownList;
      this.SelectedTwoLetterISOLanguage = string.Empty;
      ComboBoxSkinner comboBoxSkinner = new ComboBoxSkinner((ComboBox) this);
      this.FillList();
    }

    public override string Text
    {
      get => this.SelectedTwoLetterISOLanguage;
      set => this.SelectedTwoLetterISOLanguage = value;
    }

    private void OnCultureTypesChanged()
    {
      if (this.DesignMode)
        return;
      this.FillList();
    }

    public CultureTypes CultureTypes
    {
      get => this.cultureTypes;
      set
      {
        if (value == this.cultureTypes)
          return;
        this.cultureTypes = value;
        this.OnCultureTypesChanged();
      }
    }

    public IEnumerable<string> TopTwoLetterISOLanguages
    {
      get => this.topTwoLetterISOLanguages;
      set
      {
        this.topTwoLetterISOLanguages = value;
        string letterIsoLanguage = this.SelectedTwoLetterISOLanguage;
        this.FillList();
        this.SelectedTwoLetterISOLanguage = letterIsoLanguage;
      }
    }

    public string SelectedTwoLetterISOLanguage
    {
      get
      {
        LanguageComboBox.LanguageItem selectedItem = (LanguageComboBox.LanguageItem) this.SelectedItem;
        return selectedItem != null && !string.IsNullOrEmpty(selectedItem.Item.Name) ? selectedItem.Item.TwoLetterISOLanguageName : string.Empty;
      }
      set
      {
        foreach (LanguageComboBox.LanguageItem languageItem in this.Items)
        {
          if (languageItem.Item.TwoLetterISOLanguageName == value || string.IsNullOrEmpty(value) && string.IsNullOrEmpty(languageItem.Item.Name))
          {
            this.SelectedItem = (object) languageItem;
            break;
          }
        }
      }
    }

    public string SelectedCulture
    {
      get
      {
        return !(this.SelectedItem is LanguageComboBox.LanguageItem selectedItem) || string.IsNullOrEmpty(selectedItem.Item.Name) ? string.Empty : selectedItem.Item.Name;
      }
      set
      {
        foreach (LanguageComboBox.LanguageItem languageItem in this.Items)
        {
          if (languageItem.Item.Name == value)
          {
            this.SelectedItem = (object) languageItem;
            break;
          }
        }
      }
    }

    private bool IsValidCulture(string iso)
    {
      if (string.IsNullOrEmpty(iso))
        return false;
      try
      {
        CultureInfo cultureInfo = new CultureInfo(iso);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private void FillList()
    {
      this.Items.Clear();
      this.Items.Add((object) new LanguageComboBox.LanguageItem(new CultureInfo(string.Empty)));
      this.Sorted = false;
      LanguageComboBox.LanguageItem[] items1 = this.topTwoLetterISOLanguages == null ? (LanguageComboBox.LanguageItem[]) null : this.topTwoLetterISOLanguages.Where<string>(new Func<string, bool>(this.IsValidCulture)).Select<string, LanguageComboBox.LanguageItem>((Func<string, LanguageComboBox.LanguageItem>) (iso => new LanguageComboBox.LanguageItem(new CultureInfo(iso)))).ToArray<LanguageComboBox.LanguageItem>().Sort<LanguageComboBox.LanguageItem>();
      bool hasTop = items1 != null && items1.Length != 0;
      if (hasTop)
        this.Items.AddRange((object[]) items1);
      LanguageComboBox.LanguageItem[] items2 = ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(this.cultureTypes)).Select<CultureInfo, LanguageComboBox.LanguageItem>((Func<CultureInfo, LanguageComboBox.LanguageItem>) (ci => new LanguageComboBox.LanguageItem(ci))).Where<LanguageComboBox.LanguageItem>((Func<LanguageComboBox.LanguageItem, bool>) (li => !hasTop || !string.IsNullOrEmpty(li.ToString()))).ToArray<LanguageComboBox.LanguageItem>().Sort<LanguageComboBox.LanguageItem>();
      if (hasTop)
        items2[0].IsSeparator = true;
      this.Items.AddRange((object[]) items2);
    }

    private class LanguageItem : 
      ComboBoxSkinner.ComboBoxItem<CultureInfo>,
      IComparable<LanguageComboBox.LanguageItem>
    {
      public LanguageItem(CultureInfo ci)
        : base(ci)
      {
      }

      public override string ToString()
      {
        return string.IsNullOrEmpty(this.Item.Name) ? string.Empty : this.Item.DisplayName;
      }

      public int CompareTo(LanguageComboBox.LanguageItem other)
      {
        return string.Compare(this.ToString(), other.ToString());
      }
    }
  }
}
