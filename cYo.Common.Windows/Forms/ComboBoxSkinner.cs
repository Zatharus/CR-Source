// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ComboBoxSkinner
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ComboBoxSkinner
  {
    private ComboBox comboBox;
    private int verticalItemPadding = 4;
    private int separatorHeight = 3;
    private bool autoSizeDropDown = true;

    public ComboBoxSkinner() => this.IconSize = new System.Drawing.Size(24, 24);

    public ComboBoxSkinner(ComboBox comboBox, IImagePackage icons = null)
      : this()
    {
      this.ComboBox = comboBox;
      this.IconPackage = icons;
    }

    public ComboBox ComboBox
    {
      get => this.comboBox;
      set
      {
        if (this.comboBox != null)
        {
          this.comboBox.DrawMode = DrawMode.Normal;
          this.comboBox.MeasureItem -= new MeasureItemEventHandler(this.comboBox_MeasureItem);
          this.comboBox.DrawItem -= new DrawItemEventHandler(this.comboBox_DrawItem);
          this.comboBox.DropDown -= new EventHandler(this.comboBox_Enter);
        }
        this.comboBox = value;
        if (this.comboBox == null)
          return;
        this.comboBox.IntegralHeight = false;
        this.comboBox.DrawMode = DrawMode.OwnerDrawVariable;
        this.comboBox.MeasureItem += new MeasureItemEventHandler(this.comboBox_MeasureItem);
        this.comboBox.DrawItem += new DrawItemEventHandler(this.comboBox_DrawItem);
        this.comboBox.DropDown += new EventHandler(this.comboBox_Enter);
      }
    }

    [DefaultValue(4)]
    public int VerticalItemPadding
    {
      get => this.verticalItemPadding;
      set
      {
        if (this.verticalItemPadding == value)
          return;
        this.verticalItemPadding = value;
        this.comboBox.Invalidate();
      }
    }

    public int SeparatorHeight
    {
      get => this.separatorHeight;
      set
      {
        if (this.separatorHeight == value)
          return;
        this.separatorHeight = value;
        this.comboBox.Invalidate();
      }
    }

    [DefaultValue(true)]
    public bool AutoSizeDropDown
    {
      get => this.autoSizeDropDown;
      set => this.autoSizeDropDown = value;
    }

    [DefaultValue(null)]
    public IImagePackage IconPackage { get; set; }

    [DefaultValue(typeof (System.Drawing.Size), "24, 24")]
    public System.Drawing.Size IconSize { get; set; }

    private void comboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      ComboBox comboBox = (ComboBox) sender;
      object obj = comboBox.Items[e.Index];
      if (obj is ComboBoxSkinner.IComboBoxItem comboBoxItem && comboBoxItem.IsOwnerDrawn)
      {
        System.Drawing.Size size = comboBoxItem.Measure(e.Graphics, comboBox.Font);
        e.ItemHeight = size.Height;
        e.ItemWidth = size.Width;
      }
      else
      {
        System.Drawing.Size size = e.Graphics.MeasureString(comboBox.GetItemText(obj), comboBox.Font).ToSize();
        int val2 = e.Graphics.MeasureString("M", comboBox.Font).ToSize().Height;
        int num = 0;
        if (this.IconPackage != null)
        {
          System.Drawing.Size iconSize;
          if (this.IconPackage.ImageExists(comboBox.GetItemText(obj)))
          {
            int val1 = val2;
            iconSize = this.IconSize;
            int height = iconSize.Height;
            val2 = Math.Max(val1, height);
          }
          iconSize = this.IconSize;
          num = iconSize.Width;
        }
        e.ItemHeight = Math.Max(size.Height, val2);
        e.ItemWidth = size.Width + num;
      }
      e.ItemHeight += this.verticalItemPadding;
      if (comboBoxItem == null || !comboBoxItem.IsSeparator)
        return;
      e.ItemHeight += this.separatorHeight;
    }

    private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      ComboBox comboBox = (ComboBox) sender;
      object obj = comboBox.Items[e.Index];
      ComboBoxSkinner.IComboBoxItem comboBoxItem = obj as ComboBoxSkinner.IComboBoxItem;
      bool flag1 = (e.State & DrawItemState.ComboBoxEdit) != 0;
      bool flag2 = comboBoxItem != null && comboBoxItem.IsSeparator && !flag1 && e.Index > 0;
      e.DrawBackground();
      e.DrawFocusRectangle();
      using (Brush brush1 = (Brush) new SolidBrush(e.ForeColor))
      {
        Rectangle rectangle = e.Bounds;
        if (flag2)
          rectangle = rectangle.Pad(0, this.separatorHeight);
        if (this.IconPackage != null)
        {
          Image image = this.IconPackage.GetImage(comboBox.GetItemText(obj));
          if (image != null)
          {
            using (e.Graphics.HighQuality(true))
              e.Graphics.DrawImage(image, new Rectangle(rectangle.Left, rectangle.Top, this.IconSize.Width, this.IconSize.Height));
          }
          rectangle = rectangle.Pad(this.IconSize.Width, 0);
        }
        if (comboBoxItem != null && comboBoxItem.IsOwnerDrawn)
        {
          comboBoxItem.Draw(e.Graphics, rectangle, e.ForeColor, comboBox.Font);
        }
        else
        {
          using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Near
          })
            e.Graphics.DrawString(comboBox.GetItemText(obj), comboBox.Font, brush1, (RectangleF) rectangle, format);
        }
        if (!flag2)
          return;
        Rectangle rect;
        ref Rectangle local = ref rect;
        Rectangle bounds = e.Bounds;
        int left = bounds.Left;
        bounds = e.Bounds;
        int top = bounds.Top;
        bounds = e.Bounds;
        int width = bounds.Width;
        int separatorHeight = this.separatorHeight;
        local = new Rectangle(left, top, width, separatorHeight);
        using (Brush brush2 = (Brush) new SolidBrush(comboBox.BackColor))
          e.Graphics.FillRectangle(brush2, rect);
        e.Graphics.DrawLine(SystemPens.ControlLight, rect.Left + 2, rect.Top + 1, rect.Right - 2, rect.Top + 1);
      }
    }

    private void comboBox_Enter(object sender, EventArgs e)
    {
      if (!this.autoSizeDropDown)
        return;
      this.SizeDropDown();
    }

    public static IList AutoSeparatorList(IEnumerable st)
    {
      ArrayList arrayList = new ArrayList();
      bool flag = false;
      foreach (object obj in st)
      {
        if (obj.ToString() == "-")
        {
          flag = true;
        }
        else
        {
          if (flag)
            arrayList.Add((object) new ComboBoxSkinner.ComboBoxSeparator(obj));
          else
            arrayList.Add(obj);
          flag = false;
        }
      }
      return (IList) arrayList;
    }

    public void SizeDropDown()
    {
      if (this.ComboBox == null)
        return;
      int num = 0;
      using (Graphics graphics = this.ComboBox.CreateGraphics())
      {
        foreach (object obj in this.ComboBox.Items)
          num = Math.Max(num, graphics.MeasureString(this.ComboBox.GetItemText(obj), this.ComboBox.Font).ToSize().Width);
      }
      ComboBoxSkinner.Native.SendMessage(this.ComboBox.Handle, 352U, num, 0);
      this.ComboBox.DropDownHeight = 150;
    }

    public interface IComboBoxItem
    {
      bool IsSeparator { get; set; }

      bool IsOwnerDrawn { get; set; }

      System.Drawing.Size Measure(Graphics gr, Font font);

      void Draw(Graphics gr, Rectangle bounds, Color foreColor, Font font);
    }

    public class ComboBoxItem<T> : ComboBoxSkinner.IComboBoxItem
    {
      public ComboBoxItem(T item) => this.Item = item;

      public T Item { get; set; }

      public override string ToString()
      {
        try
        {
          return this.Item.ToString();
        }
        catch (Exception ex)
        {
          return string.Empty;
        }
      }

      public override int GetHashCode()
      {
        try
        {
          return this.Item.GetHashCode();
        }
        catch (Exception ex)
        {
          return 0;
        }
      }

      public static implicit operator T(ComboBoxSkinner.ComboBoxItem<T> obj) => obj.Item;

      public bool IsSeparator { get; set; }

      public bool IsOwnerDrawn { get; set; }

      public virtual System.Drawing.Size Measure(Graphics gr, Font font)
      {
        return new System.Drawing.Size(0, gr.MeasureString("M", font).ToSize().Height);
      }

      public virtual void Draw(Graphics gr, Rectangle bounds, Color foreColor, Font font)
      {
      }
    }

    public class ComboBoxItem : ComboBoxSkinner.ComboBoxItem<object>
    {
      public ComboBoxItem(object item)
        : base(item)
      {
      }
    }

    public class ComboBoxSeparator<T> : ComboBoxSkinner.ComboBoxItem<T>
    {
      public ComboBoxSeparator(T item)
        : base(item)
      {
        this.IsSeparator = true;
      }
    }

    public class ComboBoxSeparator : ComboBoxSkinner.ComboBoxSeparator<object>
    {
      public ComboBoxSeparator(object item)
        : base(item)
      {
      }
    }

    private static class Native
    {
      public const int CB_SETDROPPEDWIDTH = 352;

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
    }
  }
}
