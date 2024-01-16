// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.WrappingCheckBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class WrappingCheckBox : CheckBox
  {
    private System.Drawing.Size cachedSizeOfOneLineOfText = System.Drawing.Size.Empty;
    private readonly Dictionary<System.Drawing.Size, System.Drawing.Size> preferredSizeHash = new Dictionary<System.Drawing.Size, System.Drawing.Size>(3);

    protected override void OnAutoSizeChanged(EventArgs e)
    {
      base.OnAutoSizeChanged(e);
      this.CacheTextSize();
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.CacheTextSize();
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      this.CacheTextSize();
    }

    private void CacheTextSize()
    {
      this.preferredSizeHash.Clear();
      this.cachedSizeOfOneLineOfText = string.IsNullOrEmpty(this.Text) ? System.Drawing.Size.Empty : TextRenderer.MeasureText(this.Text, this.Font, new System.Drawing.Size(int.MaxValue, int.MaxValue), TextFormatFlags.WordBreak);
    }

    public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
    {
      System.Drawing.Size preferredSize = base.GetPreferredSize(proposedSize);
      if (preferredSize.Width > proposedSize.Width)
      {
        int num;
        if (!string.IsNullOrEmpty(this.Text))
        {
          num = proposedSize.Width;
          if (!num.Equals(int.MaxValue))
            goto label_4;
        }
        num = proposedSize.Height;
        if (num.Equals(int.MaxValue))
          goto label_7;
label_4:
        System.Drawing.Size size1 = preferredSize - this.cachedSizeOfOneLineOfText;
        System.Drawing.Size size2 = proposedSize - size1 - new System.Drawing.Size(3, 0);
        if (!this.preferredSizeHash.ContainsKey(size2))
        {
          preferredSize = size1 + TextRenderer.MeasureText(this.Text, this.Font, size2, TextFormatFlags.WordBreak);
          this.preferredSizeHash[size2] = preferredSize;
        }
        else
          preferredSize = this.preferredSizeHash[size2];
      }
label_7:
      return preferredSize;
    }
  }
}
