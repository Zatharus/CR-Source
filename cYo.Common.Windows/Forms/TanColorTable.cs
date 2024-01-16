// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TanColorTable
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TanColorTable : ProfessionalColorTable
  {
    private const string blueColorScheme = "NormalColor";
    private const string oliveColorScheme = "HomeStead";
    private const string silverColorScheme = "Metallic";
    private Dictionary<TanColorTable.KnownColors, System.Drawing.Color> tanRGB;

    private System.Drawing.Color FromKnownColor(TanColorTable.KnownColors color)
    {
      return this.ColorTable[color];
    }

    private static void InitTanLunaColors(
      ref Dictionary<TanColorTable.KnownColors, System.Drawing.Color> rgbTable)
    {
      rgbTable[TanColorTable.KnownColors.GripDark] = System.Drawing.Color.FromArgb(193, 190, 179);
      rgbTable[TanColorTable.KnownColors.SeparatorDark] = System.Drawing.Color.FromArgb(197, 194, 184);
      rgbTable[TanColorTable.KnownColors.MenuItemSelected] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[TanColorTable.KnownColors.ButtonPressedBorder] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[TanColorTable.KnownColors.CheckBackground] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[TanColorTable.KnownColors.MenuItemBorder] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[TanColorTable.KnownColors.CheckBackgroundMouseOver] = System.Drawing.Color.FromArgb(49, 106, 197);
      rgbTable[TanColorTable.KnownColors.MenuItemBorderMouseOver] = System.Drawing.Color.FromArgb(75, 75, 111);
      rgbTable[TanColorTable.KnownColors.ToolStripDropDownBackground] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[TanColorTable.KnownColors.MenuBorder] = System.Drawing.Color.FromArgb(138, 134, 122);
      rgbTable[TanColorTable.KnownColors.SeparatorLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      rgbTable[TanColorTable.KnownColors.ToolStripBorder] = System.Drawing.Color.FromArgb(163, 163, 124);
      rgbTable[TanColorTable.KnownColors.MenuStripGradientBegin] = System.Drawing.Color.FromArgb(229, 229, 215);
      rgbTable[TanColorTable.KnownColors.MenuStripGradientEnd] = System.Drawing.Color.FromArgb(244, 242, 232);
      rgbTable[TanColorTable.KnownColors.ImageMarginGradientBegin] = System.Drawing.Color.FromArgb(254, 254, 251);
      rgbTable[TanColorTable.KnownColors.ImageMarginGradientMiddle] = System.Drawing.Color.FromArgb(236, 231, 224);
      rgbTable[TanColorTable.KnownColors.ImageMarginGradientEnd] = System.Drawing.Color.FromArgb(189, 189, 163);
      rgbTable[TanColorTable.KnownColors.OverflowButtonGradientBegin] = System.Drawing.Color.FromArgb(243, 242, 240);
      rgbTable[TanColorTable.KnownColors.OverflowButtonGradientMiddle] = System.Drawing.Color.FromArgb(226, 225, 219);
      rgbTable[TanColorTable.KnownColors.OverflowButtonGradientEnd] = System.Drawing.Color.FromArgb(146, 146, 118);
      rgbTable[TanColorTable.KnownColors.MenuItemPressedGradientBegin] = System.Drawing.Color.FromArgb(252, 252, 249);
      rgbTable[TanColorTable.KnownColors.MenuItemPressedGradientEnd] = System.Drawing.Color.FromArgb(246, 244, 236);
      rgbTable[TanColorTable.KnownColors.ImageMarginRevealedGradientBegin] = System.Drawing.Color.FromArgb(247, 246, 239);
      rgbTable[TanColorTable.KnownColors.ImageMarginRevealedGradientMiddle] = System.Drawing.Color.FromArgb(242, 240, 228);
      rgbTable[TanColorTable.KnownColors.ImageMarginRevealedGradientEnd] = System.Drawing.Color.FromArgb(230, 227, 210);
      rgbTable[TanColorTable.KnownColors.ButtonCheckedGradientBegin] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[TanColorTable.KnownColors.ButtonCheckedGradientMiddle] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[TanColorTable.KnownColors.ButtonCheckedGradientEnd] = System.Drawing.Color.FromArgb(225, 230, 232);
      rgbTable[TanColorTable.KnownColors.ButtonSelectedGradientBegin] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[TanColorTable.KnownColors.ButtonSelectedGradientMiddle] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[TanColorTable.KnownColors.ButtonSelectedGradientEnd] = System.Drawing.Color.FromArgb(193, 210, 238);
      rgbTable[TanColorTable.KnownColors.ButtonPressedGradientBegin] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[TanColorTable.KnownColors.ButtonPressedGradientMiddle] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[TanColorTable.KnownColors.ButtonPressedGradientEnd] = System.Drawing.Color.FromArgb(152, 181, 226);
      rgbTable[TanColorTable.KnownColors.GripLight] = System.Drawing.Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
    }

    public override System.Drawing.Color ButtonCheckedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonCheckedGradientBegin) : base.ButtonCheckedGradientBegin;
      }
    }

    public override System.Drawing.Color ButtonCheckedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonCheckedGradientEnd) : base.ButtonCheckedGradientEnd;
      }
    }

    public override System.Drawing.Color ButtonCheckedGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonCheckedGradientMiddle) : base.ButtonCheckedGradientMiddle;
      }
    }

    public override System.Drawing.Color ButtonPressedBorder
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonPressedBorder) : base.ButtonPressedBorder;
      }
    }

    public override System.Drawing.Color ButtonPressedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonPressedGradientBegin) : base.ButtonPressedGradientBegin;
      }
    }

    public override System.Drawing.Color ButtonPressedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonPressedGradientEnd) : base.ButtonPressedGradientEnd;
      }
    }

    public override System.Drawing.Color ButtonPressedGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonPressedGradientMiddle) : base.ButtonPressedGradientMiddle;
      }
    }

    public override System.Drawing.Color ButtonSelectedBorder
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonPressedBorder) : base.ButtonSelectedBorder;
      }
    }

    public override System.Drawing.Color ButtonSelectedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonSelectedGradientBegin) : base.ButtonSelectedGradientBegin;
      }
    }

    public override System.Drawing.Color ButtonSelectedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonSelectedGradientEnd) : base.ButtonSelectedGradientEnd;
      }
    }

    public override System.Drawing.Color ButtonSelectedGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonSelectedGradientMiddle) : base.ButtonSelectedGradientMiddle;
      }
    }

    public override System.Drawing.Color CheckBackground
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.CheckBackground) : base.CheckBackground;
      }
    }

    public override System.Drawing.Color CheckPressedBackground
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.CheckBackgroundMouseOver) : base.CheckPressedBackground;
      }
    }

    public override System.Drawing.Color CheckSelectedBackground
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.CheckBackgroundMouseOver) : base.CheckSelectedBackground;
      }
    }

    internal static string ColorScheme => TanColorTable.DisplayInformation.ColorScheme;

    private Dictionary<TanColorTable.KnownColors, System.Drawing.Color> ColorTable
    {
      get
      {
        if (this.tanRGB == null)
        {
          this.tanRGB = new Dictionary<TanColorTable.KnownColors, System.Drawing.Color>(34);
          TanColorTable.InitTanLunaColors(ref this.tanRGB);
        }
        return this.tanRGB;
      }
    }

    public override System.Drawing.Color GripDark
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.GripDark) : base.GripDark;
      }
    }

    public override System.Drawing.Color GripLight
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.GripLight) : base.GripLight;
      }
    }

    public override System.Drawing.Color ImageMarginGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientBegin) : base.ImageMarginGradientBegin;
      }
    }

    public override System.Drawing.Color ImageMarginGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientEnd) : base.ImageMarginGradientEnd;
      }
    }

    public override System.Drawing.Color ImageMarginGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientMiddle) : base.ImageMarginGradientMiddle;
      }
    }

    public override System.Drawing.Color ImageMarginRevealedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginRevealedGradientBegin) : base.ImageMarginRevealedGradientBegin;
      }
    }

    public override System.Drawing.Color ImageMarginRevealedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginRevealedGradientEnd) : base.ImageMarginRevealedGradientEnd;
      }
    }

    public override System.Drawing.Color ImageMarginRevealedGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginRevealedGradientMiddle) : base.ImageMarginRevealedGradientMiddle;
      }
    }

    public override System.Drawing.Color MenuBorder
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuBorder) : base.MenuItemBorder;
      }
    }

    public override System.Drawing.Color MenuItemBorder
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuItemBorder) : base.MenuItemBorder;
      }
    }

    public override System.Drawing.Color MenuItemPressedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuItemPressedGradientBegin) : base.MenuItemPressedGradientBegin;
      }
    }

    public override System.Drawing.Color MenuItemPressedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuItemPressedGradientEnd) : base.MenuItemPressedGradientEnd;
      }
    }

    public override System.Drawing.Color MenuItemPressedGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginRevealedGradientMiddle) : base.MenuItemPressedGradientMiddle;
      }
    }

    public override System.Drawing.Color MenuItemSelected
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuItemSelected) : base.MenuItemSelected;
      }
    }

    public override System.Drawing.Color MenuItemSelectedGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonSelectedGradientBegin) : base.MenuItemSelectedGradientBegin;
      }
    }

    public override System.Drawing.Color MenuItemSelectedGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ButtonSelectedGradientEnd) : base.MenuItemSelectedGradientEnd;
      }
    }

    public override System.Drawing.Color MenuStripGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuStripGradientBegin) : base.MenuStripGradientBegin;
      }
    }

    public override System.Drawing.Color MenuStripGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuStripGradientEnd) : base.MenuStripGradientEnd;
      }
    }

    public override System.Drawing.Color OverflowButtonGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.OverflowButtonGradientBegin) : base.OverflowButtonGradientBegin;
      }
    }

    public override System.Drawing.Color OverflowButtonGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.OverflowButtonGradientEnd) : base.OverflowButtonGradientEnd;
      }
    }

    public override System.Drawing.Color OverflowButtonGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.OverflowButtonGradientMiddle) : base.OverflowButtonGradientMiddle;
      }
    }

    public override System.Drawing.Color RaftingContainerGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuStripGradientBegin) : base.RaftingContainerGradientBegin;
      }
    }

    public override System.Drawing.Color RaftingContainerGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.MenuStripGradientEnd) : base.RaftingContainerGradientEnd;
      }
    }

    public override System.Drawing.Color SeparatorDark
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.SeparatorDark) : base.SeparatorDark;
      }
    }

    public override System.Drawing.Color SeparatorLight
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.SeparatorLight) : base.SeparatorLight;
      }
    }

    public override System.Drawing.Color ToolStripBorder
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ToolStripBorder) : base.ToolStripBorder;
      }
    }

    public override System.Drawing.Color ToolStripDropDownBackground
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ToolStripDropDownBackground) : base.ToolStripDropDownBackground;
      }
    }

    public override System.Drawing.Color ToolStripGradientBegin
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientBegin) : base.ToolStripGradientBegin;
      }
    }

    public override System.Drawing.Color ToolStripGradientEnd
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientEnd) : base.ToolStripGradientEnd;
      }
    }

    public override System.Drawing.Color ToolStripGradientMiddle
    {
      get
      {
        return !this.UseBaseColorTable ? this.FromKnownColor(TanColorTable.KnownColors.ImageMarginGradientMiddle) : base.ToolStripGradientMiddle;
      }
    }

    private bool UseBaseColorTable
    {
      get
      {
        bool useBaseColorTable = !TanColorTable.DisplayInformation.IsLunaTheme || TanColorTable.ColorScheme != "HomeStead" && TanColorTable.ColorScheme != "NormalColor";
        if (useBaseColorTable && this.tanRGB != null)
        {
          this.tanRGB.Clear();
          this.tanRGB = (Dictionary<TanColorTable.KnownColors, System.Drawing.Color>) null;
        }
        return useBaseColorTable;
      }
    }

    private static class DisplayInformation
    {
      [ThreadStatic]
      private static string colorScheme;
      [ThreadStatic]
      private static bool isLunaTheme;
      private const string lunaFileName = "luna.msstyles";

      static DisplayInformation()
      {
        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(TanColorTable.DisplayInformation.OnUserPreferenceChanged);
        TanColorTable.DisplayInformation.SetScheme();
      }

      private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
      {
        TanColorTable.DisplayInformation.SetScheme();
      }

      private static void SetScheme()
      {
        TanColorTable.DisplayInformation.isLunaTheme = false;
        if (VisualStyleRenderer.IsSupported)
        {
          TanColorTable.DisplayInformation.colorScheme = VisualStyleInformation.ColorScheme;
          if (!VisualStyleInformation.IsEnabledByUser)
            return;
          StringBuilder pszThemeFileName = new StringBuilder(512);
          TanColorTable.DisplayInformation.GetCurrentThemeName(pszThemeFileName, pszThemeFileName.Capacity, (StringBuilder) null, 0, (StringBuilder) null, 0);
          TanColorTable.DisplayInformation.isLunaTheme = string.Equals("luna.msstyles", Path.GetFileName(pszThemeFileName.ToString()), StringComparison.InvariantCultureIgnoreCase);
        }
        else
          TanColorTable.DisplayInformation.colorScheme = (string) null;
      }

      public static string ColorScheme => TanColorTable.DisplayInformation.colorScheme;

      public static bool IsLunaTheme => TanColorTable.DisplayInformation.isLunaTheme;

      [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
      public static extern int GetCurrentThemeName(
        StringBuilder pszThemeFileName,
        int dwMaxNameChars,
        StringBuilder pszColorBuff,
        int dwMaxColorChars,
        StringBuilder pszSizeBuff,
        int cchMaxSizeChars);
    }

    private new enum KnownColors
    {
      ButtonPressedBorder = 0,
      MenuItemBorder = 1,
      MenuItemBorderMouseOver = 2,
      MenuItemSelected = 3,
      CheckBackground = 4,
      CheckBackgroundMouseOver = 5,
      GripDark = 6,
      GripLight = 7,
      MenuStripGradientBegin = 8,
      MenuStripGradientEnd = 9,
      ImageMarginRevealedGradientBegin = 10, // 0x0000000A
      ImageMarginRevealedGradientEnd = 11, // 0x0000000B
      ImageMarginRevealedGradientMiddle = 12, // 0x0000000C
      MenuItemPressedGradientBegin = 13, // 0x0000000D
      MenuItemPressedGradientEnd = 14, // 0x0000000E
      ButtonPressedGradientBegin = 15, // 0x0000000F
      ButtonPressedGradientEnd = 16, // 0x00000010
      ButtonPressedGradientMiddle = 17, // 0x00000011
      ButtonSelectedGradientBegin = 18, // 0x00000012
      ButtonSelectedGradientEnd = 19, // 0x00000013
      ButtonSelectedGradientMiddle = 20, // 0x00000014
      OverflowButtonGradientBegin = 21, // 0x00000015
      OverflowButtonGradientEnd = 22, // 0x00000016
      OverflowButtonGradientMiddle = 23, // 0x00000017
      ButtonCheckedGradientBegin = 24, // 0x00000018
      ButtonCheckedGradientEnd = 25, // 0x00000019
      ButtonCheckedGradientMiddle = 26, // 0x0000001A
      ImageMarginGradientBegin = 27, // 0x0000001B
      ImageMarginGradientEnd = 28, // 0x0000001C
      ImageMarginGradientMiddle = 29, // 0x0000001D
      MenuBorder = 30, // 0x0000001E
      ToolStripDropDownBackground = 31, // 0x0000001F
      ToolStripBorder = 32, // 0x00000020
      SeparatorDark = 33, // 0x00000021
      LastKnownColor = 34, // 0x00000022
      SeparatorLight = 34, // 0x00000022
    }
  }
}
