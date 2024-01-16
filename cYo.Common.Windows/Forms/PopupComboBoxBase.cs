// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.PopupComboBoxBase
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [ToolboxBitmap(typeof (ComboBox))]
  [ToolboxItem(true)]
  [ToolboxItemFilter("System.Windows.Forms")]
  [Description("Displays an editable text box with a drop-down list of permitted values.")]
  public class PopupComboBoxBase : ComboBox
  {
    private static Type modalMenuFilter;
    private static MethodInfo suspendMenuModeMethodInfo;
    private static MethodInfo resumeMenuModeMethodInfo;

    private static Type ModalMenuFilter
    {
      get
      {
        if (PopupComboBoxBase.modalMenuFilter == (Type) null)
          PopupComboBoxBase.modalMenuFilter = Type.GetType("System.Windows.Forms.ToolStripManager+ModalMenuFilter");
        if (PopupComboBoxBase.modalMenuFilter == (Type) null)
          PopupComboBoxBase.modalMenuFilter = new List<Type>((IEnumerable<Type>) typeof (ToolStripManager).Assembly.GetTypes()).Find((Predicate<Type>) (type => type.FullName == "System.Windows.Forms.ToolStripManager+ModalMenuFilter"));
        return PopupComboBoxBase.modalMenuFilter;
      }
    }

    private static MethodInfo SuspendMenuModeMethodInfo
    {
      get
      {
        if (PopupComboBoxBase.suspendMenuModeMethodInfo == (MethodInfo) null)
        {
          Type modalMenuFilter = PopupComboBoxBase.ModalMenuFilter;
          if (modalMenuFilter != (Type) null)
            PopupComboBoxBase.suspendMenuModeMethodInfo = modalMenuFilter.GetMethod("SuspendMenuMode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return PopupComboBoxBase.suspendMenuModeMethodInfo;
      }
    }

    private static void SuspendMenuMode()
    {
      MethodInfo menuModeMethodInfo = PopupComboBoxBase.SuspendMenuModeMethodInfo;
      if (!(menuModeMethodInfo != (MethodInfo) null))
        return;
      menuModeMethodInfo.Invoke((object) null, (object[]) null);
    }

    private static MethodInfo ResumeMenuModeMethodInfo
    {
      get
      {
        if (PopupComboBoxBase.resumeMenuModeMethodInfo == (MethodInfo) null)
        {
          Type modalMenuFilter = PopupComboBoxBase.ModalMenuFilter;
          if (modalMenuFilter != (Type) null)
            PopupComboBoxBase.resumeMenuModeMethodInfo = modalMenuFilter.GetMethod("ResumeMenuMode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return PopupComboBoxBase.resumeMenuModeMethodInfo;
      }
    }

    private static void ResumeMenuMode()
    {
      MethodInfo menuModeMethodInfo = PopupComboBoxBase.ResumeMenuModeMethodInfo;
      if (!(menuModeMethodInfo != (MethodInfo) null))
        return;
      menuModeMethodInfo.Invoke((object) null, (object[]) null);
    }

    protected override void OnDropDown(EventArgs e)
    {
      base.OnDropDown(e);
      PopupComboBoxBase.SuspendMenuMode();
    }

    protected override void OnDropDownClosed(EventArgs e)
    {
      PopupComboBoxBase.ResumeMenuMode();
      base.OnDropDownClosed(e);
    }
  }
}
