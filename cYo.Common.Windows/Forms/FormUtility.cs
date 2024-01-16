// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.FormUtility
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class FormUtility
  {
    public static readonly Dictionary<object, object> ServiceTranslation = new Dictionary<object, object>();
    private static Dictionary<string, Rectangle> formPositions;
    private static Dictionary<string, IEnumerable<FormUtility.PanelState>> panelStates;
    private const int LOGPIXELSX = 88;
    private const int LOGPIXELSY = 90;
    private static PointF dpiScale = PointF.Empty;

    public static object FindActiveService(this Control root, Type service)
    {
      if (service != (Type) null && root != null)
      {
        if (root is ContainerControl containerControl)
        {
          Control activeControl = containerControl.ActiveControl;
          if (activeControl != null && activeControl.Visible)
          {
            object activeService = activeControl.FindActiveService(service);
            if (activeService != null)
              return activeService;
          }
        }
        if (service.IsInstanceOfType(FormUtility.GetServiceObject((object) root)))
          return FormUtility.GetServiceObject((object) root);
        foreach (Control control in (ArrangedElementCollection) root.Controls)
        {
          if (control != null && control.Visible)
          {
            object activeService = control.FindActiveService(service);
            if (activeService != null)
              return activeService;
          }
        }
      }
      return (object) null;
    }

    public static T FindActiveService<T>(this Control root) where T : class
    {
      return (T) root.FindActiveService(typeof (T));
    }

    public static K InvokeActiveService<T, K>(
      this Control root,
      Func<T, K> predicate,
      K defaultReturn = null)
      where T : class
    {
      T activeService = root.FindActiveService<T>();
      return (object) activeService != null ? predicate(activeService) : defaultReturn;
    }

    public static void InvokeActiveService<T>(this Control root, Action<T> action) where T : class
    {
      T activeService = root.FindActiveService<T>();
      if ((object) activeService == null)
        return;
      action(activeService);
    }

    public static IEnumerable<T> FindServices<T>(this Control root) where T : class
    {
      if (FormUtility.GetServiceObject((object) root) is T serviceObject)
        yield return serviceObject;
      foreach (Control control in (ArrangedElementCollection) root.Controls)
      {
        foreach (T service in control.FindServices<T>())
          yield return service;
      }
    }

    public static T FindFirstService<T>(this Control root) where T : class
    {
      return root.FindServices<T>().FirstOrDefault<T>();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public static T FindActiveService<T>() where T : class
    {
      return ((Control) Form.ActiveForm ?? Control.FromHandle(FormUtility.GetActiveWindow())).FindActiveService<T>();
    }

    public static T FindParentService<T>(this Control c) where T : class
    {
      while ((c = c.Parent) != null)
      {
        if (c is T parentService)
          return parentService;
      }
      return default (T);
    }

    private static T GetAttribute<T>(this MemberInfo pi) where T : class
    {
      return Attribute.GetCustomAttribute(pi, typeof (T)) as T;
    }

    public static void FillPanelWithOptions(Control panel, object data, TR tr, bool rebuild = false)
    {
      if (panel.Tag is TabControl)
        panel = (Control) panel.Tag;
      if (rebuild)
        panel.Clear(true);
      PropertyInfo[] properties = data.GetType().GetProperties();
      foreach (string str in ((IEnumerable<PropertyInfo>) properties).Select<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Category())).Distinct<string>())
      {
        string cat = str;
        string caption = tr[cat ?? "Other"];
        CollapsibleGroupBox child = panel.Controls.OfType<CollapsibleGroupBox>().FirstOrDefault<CollapsibleGroupBox>((Func<CollapsibleGroupBox, bool>) (cp => cp.Text == caption));
        foreach (var data1 in ((IEnumerable<PropertyInfo>) properties).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType == typeof (bool) && p.Browsable() && p.Category() == cat)).Select(p => new
        {
          Property = p,
          Description = tr[p.Name, p.Description()]
        }).Where(c => !string.IsNullOrEmpty(c.Description)).OrderBy(c => c.Description).ToArray())
        {
          var pi = data1;
          CheckBox checkBox1 = panel.GetControls<CheckBox>().FirstOrDefault<CheckBox>((Func<CheckBox, bool>) (cb => pi.Property.Name.Equals(cb.Tag)));
          if (checkBox1 == null)
          {
            if (child == null)
            {
              CollapsibleGroupBox collapsibleGroupBox = new CollapsibleGroupBox();
              collapsibleGroupBox.Dock = DockStyle.Top;
              collapsibleGroupBox.Text = caption;
              collapsibleGroupBox.Tag = (object) cat;
              child = collapsibleGroupBox;
              panel.Controls.Add((Control) child);
              panel.Controls.SetChildIndex((Control) child, 0);
            }
            CheckBox checkBox2 = new CheckBox();
            checkBox2.AutoSize = true;
            checkBox2.Text = pi.Description;
            checkBox2.Tag = (object) pi.Property.Name;
            checkBox1 = checkBox2;
            child.Controls.Add((Control) checkBox1);
            child.Height = FormUtility.ScaleDpiY(30) + child.Controls.Count * (checkBox1.Height + FormUtility.ScaleDpiY(2)) + FormUtility.ScaleDpiY(10);
            checkBox1.Left = FormUtility.ScaleDpiX(20);
            checkBox1.Top = child.Height - FormUtility.ScaleDpiY(10) - checkBox1.Height - FormUtility.ScaleDpiY(1);
          }
          checkBox1.Checked = (bool) pi.Property.GetValue(data, (object[]) null);
        }
      }
    }

    public static void RetrieveOptionsFromPanel(Control panel, object data)
    {
      if (panel.Tag is TabControl)
        panel = (Control) panel.Tag;
      foreach (CheckBox control in panel.GetControls<CheckBox>())
      {
        try
        {
          data.GetType().GetProperty((string) control.Tag).SetValue(data, (object) control.Checked, (object[]) null);
        }
        catch
        {
        }
      }
    }

    public static void FillListWithOptions(ListView lv, object data, TR tr)
    {
      lv.BeginUpdate();
      try
      {
        lv.Items.Clear();
        lv.CheckBoxes = true;
        lv.ShowGroups = true;
        lv.View = View.Details;
        foreach (PropertyInfo property in data.GetType().GetProperties())
        {
          if (!(property.PropertyType != typeof (bool)) && (property.GetAttribute<BrowsableAttribute>() == null || property.GetAttribute<BrowsableAttribute>().Browsable))
          {
            DescriptionAttribute attribute1 = property.GetAttribute<DescriptionAttribute>();
            CategoryAttribute attribute2 = property.GetAttribute<CategoryAttribute>();
            string key = attribute2 == null || string.IsNullOrEmpty(attribute2.Category) ? "Other" : attribute2.Category;
            if (attribute1 != null && !string.IsNullOrEmpty(attribute1.Description))
            {
              ListViewItem listViewItem = lv.Items.Add(tr[property.Name, attribute1.Description]);
              listViewItem.Checked = (bool) property.GetValue(data, (object[]) null);
              listViewItem.Group = lv.Groups[key] ?? lv.Groups.Add(key, tr[key]);
              listViewItem.Tag = (object) property.Name;
            }
          }
        }
      }
      finally
      {
        lv.EndUpdate();
      }
    }

    public static void FillListWithOptions(ListView lv, object data)
    {
      FormUtility.FillListWithOptions(lv, data, new TR());
    }

    public static void RetrieveOptionsFromList(ListView lv, object data)
    {
      foreach (ListViewItem listViewItem in lv.Items)
      {
        try
        {
          data.GetType().GetProperty((string) listViewItem.Tag).SetValue(data, (object) listViewItem.Checked, (object[]) null);
        }
        catch
        {
        }
      }
    }

    public static IEnumerable<ListViewItem> Enumerate(this ListView listView)
    {
      for (int i = 0; i < listView.Items.Count; ++i)
        yield return listView.Items[i];
    }

    public static void SortGroups(this ListView listView, IComparer comparer = null)
    {
      if (comparer == null)
        comparer = (IComparer) new FormUtility.GroupHeaderComparer();
      ArrayList arrayList = new ArrayList((ICollection) listView.Groups);
      arrayList.Sort(comparer);
      listView.Groups.Clear();
      foreach (ListViewGroup group in arrayList)
        listView.Groups.Add(group);
    }

    public static IEnumerable<TreeNode> All(this TreeNodeCollection nodes)
    {
      foreach (TreeNode tn in nodes)
      {
        yield return tn;
        foreach (TreeNode treeNode in tn.Nodes.All())
          yield return treeNode;
      }
    }

    public static IEnumerable<TreeNode> AllNodes(this TreeView tree) => tree.Nodes.All();

    public static TreeNode Find(this TreeNodeCollection tnc, Predicate<TreeNode> pred, bool all = true)
    {
      foreach (TreeNode treeNode1 in tnc)
      {
        if (pred(treeNode1))
          return treeNode1;
        if (all)
        {
          TreeNode treeNode2 = treeNode1.Nodes.Find(pred);
          if (treeNode2 != null)
            return treeNode2;
        }
      }
      return (TreeNode) null;
    }

    public static void AddRange<T>(this ComboBox.ObjectCollection collection, IEnumerable<T> list)
    {
      foreach (T obj in list)
        collection.Add((object) obj);
    }

    public static IEnumerable<T> GetControls<T>(this Control container, bool all = true) where T : Control
    {
      return !all ? container.Controls.OfType<T>() : container.Controls.Recurse<T>((Func<object, IEnumerable>) (o => (IEnumerable) ((Control) o).Controls));
    }

    public static void ForEachControl<T>(this Control container, Action<T> action, bool all = true) where T : Control
    {
      container.GetControls<T>(all).ForEach<T>(action);
    }

    public static void Clear(this Control control, bool withDispose)
    {
      for (int index = control.Controls.Count - 1; index >= 0; --index)
      {
        Control control1 = control.Controls[index];
        control1.Clear(withDispose);
        control.Controls.RemoveAt(index);
        if (withDispose)
          control1.Dispose();
      }
    }

    public static void AutoTabIndex(this Control control)
    {
      control.Controls.OfType<Control>().ForEach<Control>((Action<Control>) (c => c.TabIndex = control.Controls.IndexOf(c)));
    }

    public static string FixAmpersand(string text) => text?.Replace("&", "&&");

    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    public static bool MessageHasExtraInfo(this Control control)
    {
      return FormUtility.GetMessageExtraInfo() != IntPtr.Zero;
    }

    public static bool IsTouchMessage(this Control control)
    {
      return (FormUtility.GetMessageExtraInfo().ToInt64() & 4294967040L) == 4283520768L;
    }

    public static IEnumerable<ToolStripItem> GetTools(ToolStripItemCollection tic)
    {
      foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) tic)
      {
        ToolStripItem t = toolStripItem;
        yield return t;
        if (t is ToolStripDropDownItem stripDropDownItem)
        {
          foreach (ToolStripItem tool in FormUtility.GetTools(stripDropDownItem.DropDownItems))
            yield return tool;
          t = (ToolStripItem) null;
        }
      }
    }

    public static void PrefixToolStrip(ToolStripItemCollection tsic)
    {
      List<ToolStripMenuItem> toolStripMenuItemList = new List<ToolStripMenuItem>();
      List<string> texts = new List<string>();
      foreach (ToolStripMenuItem toolStripMenuItem in tsic.OfType<ToolStripMenuItem>())
      {
        toolStripMenuItemList.Add(toolStripMenuItem);
        texts.Add(toolStripMenuItem.Text);
      }
      StringUtility.Prefix((IList<string>) texts, '&');
      for (int index = 0; index < texts.Count; ++index)
        toolStripMenuItemList[index].Text = texts[index];
    }

    public static void PrefixToolStrip(ToolStrip ts)
    {
      FormUtility.PrefixToolStrip(ts.Items);
      foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) ts.Items)
      {
        if (toolStripItem is ToolStripDropDownItem stripDropDownItem)
          FormUtility.PrefixToolStrip(stripDropDownItem.DropDownItems);
      }
    }

    public static void PrefixToolStrips(Form f, ComponentCollection components)
    {
      foreach (ToolStrip ts in f.Controls.OfType<ToolStrip>())
        FormUtility.PrefixToolStrip(ts);
      if (components == null)
        return;
      foreach (ToolStrip ts in components.OfType<ToolStrip>())
        FormUtility.PrefixToolStrip(ts);
    }

    public static void SafeToolStripClear(ToolStripItemCollection tsic, int startAt = 0)
    {
      foreach (ToolStripItem toolStripItem in ((IEnumerable<ToolStripItem>) tsic.OfType<ToolStripItem>().ToArray<ToolStripItem>()).Skip<ToolStripItem>(startAt))
      {
        tsic.Remove(toolStripItem);
        toolStripItem.Dispose();
      }
    }

    public static void EnableRightClickSplitButtons(ToolStripItemCollection tsic)
    {
      foreach (ToolStripSplitButton stripSplitButton in FormUtility.GetTools(tsic).OfType<ToolStripSplitButton>())
      {
        ToolStripSplitButton t = stripSplitButton;
        t.MouseUp += (MouseEventHandler) ((s, e) =>
        {
          if (e.Button != MouseButtons.Right)
            return;
          t.ShowDropDown();
        });
      }
    }

    public static int Clamp(this NumericUpDown num, int n)
    {
      return n.Clamp((int) num.Minimum, (int) num.Maximum);
    }

    public static Dictionary<string, Rectangle> FormPositions
    {
      get
      {
        if (FormUtility.formPositions == null)
          FormUtility.formPositions = new Dictionary<string, Rectangle>();
        return FormUtility.formPositions;
      }
    }

    public static Rectangle GetSafeBounds(this Form form)
    {
      return form.WindowState != FormWindowState.Normal ? form.RestoreBounds : form.Bounds;
    }

    public static void RestorePosition(this Form form, string key = null)
    {
      if (string.IsNullOrEmpty(key))
        key = form.GetType().Name;
      Rectangle bounds;
      if (FormUtility.FormPositions.TryGetValue(key, out bounds))
      {
        if (form.IsHandleCreated)
          form.Bounds = bounds;
        else
          form.Load += (EventHandler) ((s, e) => form.Bounds = bounds);
      }
      form.Closing += (CancelEventHandler) ((s, e) => FormUtility.FormPositions[key] = ((Form) s).GetSafeBounds());
    }

    private static FormUtility.PanelState GetPanelState(ScrollableControl panel)
    {
      return new FormUtility.PanelState()
      {
        Name = panel.Name,
        AutoScrollPosition = panel.AutoScrollPosition.Multiply(-1, -1),
        Collapsed = panel.Controls.OfType<CollapsibleGroupBox>().Select<CollapsibleGroupBox, bool>((Func<CollapsibleGroupBox, bool>) (c => c.Collapsed)).ToArray<bool>()
      };
    }

    private static void SetPanelState(ScrollableControl panel, FormUtility.PanelState ps)
    {
      for (int index = 0; index < ps.Collapsed.Length; ++index)
        ((CollapsibleGroupBox) panel.Controls[index]).Collapsed = ps.Collapsed[index];
      panel.PerformLayout();
      panel.AutoScrollPosition = ps.AutoScrollPosition;
    }

    private static IEnumerable<FormUtility.PanelState> GetPanelStates(Control container)
    {
      return (IEnumerable<FormUtility.PanelState>) container.GetControls<Panel>().Select<Panel, FormUtility.PanelState>(new Func<Panel, FormUtility.PanelState>(FormUtility.GetPanelState)).ToArray<FormUtility.PanelState>();
    }

    private static void SetPanelStates(Control container, IEnumerable<FormUtility.PanelState> psl)
    {
      if (psl == null)
        return;
      foreach (var data in container.GetControls<Panel>().Join(psl, (Func<Panel, string>) (panel => panel.Name), (Func<FormUtility.PanelState, string>) (ps => ps.Name), (panel, ps) => new
      {
        Panel = panel,
        State = ps
      }))
        FormUtility.SetPanelState((ScrollableControl) data.Panel, data.State);
    }

    public static void RestorePanelStates(this Form control)
    {
      if (control == null)
        return;
      if (FormUtility.panelStates == null)
        FormUtility.panelStates = new Dictionary<string, IEnumerable<FormUtility.PanelState>>();
      IEnumerable<FormUtility.PanelState> psl;
      if (FormUtility.panelStates.TryGetValue(control.Name, out psl))
        control.Load += (EventHandler) ((s, e) => FormUtility.SetPanelStates((Control) control, psl));
      control.FormClosing += (FormClosingEventHandler) ((s, e) => FormUtility.panelStates[control.Name] = FormUtility.GetPanelStates((Control) control));
    }

    public static bool PanelToTab(Control control, bool on, Action<bool> setState = null)
    {
      Control parent = control.Parent;
      if (control.Tag is TabControl)
      {
        if (on)
          return true;
        TabControl tag1 = control.Tag as TabControl;
        bool flag = tag1.IsVisibleSet();
        foreach (TabPage tabPage in tag1.TabPages)
        {
          Control tag2 = tabPage.Tag as Control;
          foreach (Control control1 in tabPage.Controls.OfType<Control>().ToArray<Control>())
          {
            tabPage.Controls.Remove(control1);
            if (tag2 is CollapsibleGroupBox)
              control1.Top += FormUtility.ScaleDpiY((tag2 as CollapsibleGroupBox).HeaderHeight);
            tag2.Controls.Add(control1);
          }
        }
        control.Visible = flag;
        control.Tag = (object) null;
        tag1.Dispose();
        parent.Controls.Remove((Control) tag1);
      }
      else
      {
        if (!on)
          return false;
        TabControl tabControl = new TabControl();
        bool flag = control.IsVisibleSet();
        control.Visible = false;
        control.Tag = (object) tabControl;
        parent.Controls.Add((Control) tabControl);
        tabControl.Bounds = control.Bounds;
        tabControl.Visible = flag;
        foreach (Control c in control.Controls.OfType<Control>().Reverse<Control>())
        {
          if (c.IsVisibleSet())
          {
            TabPage tabPage = new TabPage(c.Text);
            tabControl.TabPages.Add(tabPage);
            tabPage.Tag = (object) c;
            tabPage.BackColor = Color.Transparent;
            tabPage.UseVisualStyleBackColor = true;
            tabPage.DoubleClick += (EventHandler) ((s, e) => FormUtility.TogglePanelTab(control, setState));
            foreach (Control control2 in c.Controls.OfType<Control>().ToArray<Control>())
            {
              c.Controls.Remove(control2);
              if (c is CollapsibleGroupBox)
                control2.Top -= FormUtility.ScaleDpiY((c as CollapsibleGroupBox).HeaderHeight);
              tabPage.Controls.Add(control2);
            }
          }
        }
        tabControl.SelectedIndex = 0;
      }
      return on;
    }

    public static bool TogglePanelTab(Control control, Action<bool> setState = null)
    {
      bool tab = FormUtility.PanelToTab(control, !(control.Tag is TabControl), setState);
      if (setState != null)
        setState(tab);
      return tab;
    }

    public static void RegisterPanelToTabToggle(
      Control control,
      Func<bool> getState = null,
      Action<bool> setState = null)
    {
      foreach (Control control1 in (ArrangedElementCollection) control.Controls)
      {
        control1.DoubleClick += (EventHandler) ((s, e) => FormUtility.TogglePanelTab(control, setState));
        control1.RegisterTouchScrolling();
      }
      if (getState == null || !getState())
        return;
      FormUtility.TogglePanelTab(control, setState);
    }

    public static void RegisterPanelToTabToggle(Control control, IValueStore<bool> setting)
    {
      FormUtility.RegisterPanelToTabToggle(control, new Func<bool>(setting.GetValue), new Action<bool>(setting.SetValue));
    }

    public static bool RegisterTouchScrolling(this Control control) => false;

    [DllImport("user32.dll")]
    private static extern bool IsProcessDPIAware();

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    public static PointF DpiScale
    {
      get
      {
        if (!FormUtility.dpiScale.IsEmpty)
          return FormUtility.dpiScale;
        if (!FormUtility.IsProcessDPIAware())
        {
          FormUtility.dpiScale = new PointF(1f, 1f);
        }
        else
        {
          IntPtr dc = FormUtility.GetDC(IntPtr.Zero);
          System.Drawing.Size size = new System.Drawing.Size(FormUtility.GetDeviceCaps(dc, 88), FormUtility.GetDeviceCaps(dc, 90));
          FormUtility.dpiScale = new PointF((float) size.Width / 96f, (float) size.Height / 96f);
        }
        return FormUtility.dpiScale;
      }
    }

    public static System.Drawing.Size ScaleDpi(this System.Drawing.Size size)
    {
      return new System.Drawing.Size((int) ((double) size.Width * (double) FormUtility.DpiScale.X), (int) ((double) size.Height * (double) FormUtility.DpiScale.Y));
    }

    public static System.Drawing.Point ScaleDpi(this System.Drawing.Point pt)
    {
      return new System.Drawing.Point((int) ((double) pt.X * (double) FormUtility.DpiScale.X), (int) ((double) pt.Y * (double) FormUtility.DpiScale.Y));
    }

    public static Padding ScaleDpi(this Padding pd)
    {
      pd.Left = FormUtility.ScaleDpiX(pd.Left);
      pd.Right = FormUtility.ScaleDpiX(pd.Right);
      pd.Top = FormUtility.ScaleDpiY(pd.Top);
      pd.Bottom = FormUtility.ScaleDpiY(pd.Bottom);
      return pd;
    }

    public static RectangleF ScaleDpi(this RectangleF rect)
    {
      rect.X = FormUtility.ScaleDpiX(rect.X);
      rect.Width = FormUtility.ScaleDpiX(rect.Width);
      rect.Y = FormUtility.ScaleDpiX(rect.Y);
      rect.Height = FormUtility.ScaleDpiY(rect.Height);
      return rect;
    }

    public static float ScaleDpiX(float v) => v * FormUtility.DpiScale.X;

    public static float ScaleDpiY(float v) => v * FormUtility.DpiScale.Y;

    public static int ScaleDpiX(int v) => (int) ((double) v * (double) FormUtility.DpiScale.X);

    public static int ScaleDpiY(int v) => (int) ((double) v * (double) FormUtility.DpiScale.Y);

    public static Bitmap ScaleDpi(this Bitmap bitmap) => bitmap.Scale(FormUtility.DpiScale.Y);

    public static Rectangle ScaleDpi(this Rectangle rect)
    {
      return new Rectangle(FormUtility.ScaleDpiX(rect.X), FormUtility.ScaleDpiY(rect.Y), FormUtility.ScaleDpiX(rect.Width), FormUtility.ScaleDpiY(rect.Height));
    }

    public static void ScaleDpi(this ListView.ColumnHeaderCollection chc)
    {
      foreach (ColumnHeader columnHeader in chc)
        columnHeader.Width = FormUtility.ScaleDpiX(columnHeader.Width);
    }

    private static object GetServiceObject(object obj)
    {
      object obj1;
      return !FormUtility.ServiceTranslation.TryGetValue(obj, out obj1) ? obj : obj1;
    }

    private class GroupHeaderComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        return string.Compare((x as ListViewGroup).Header, (y as ListViewGroup).Header);
      }
    }

    private class PanelState
    {
      public string Name { get; set; }

      public System.Drawing.Point AutoScrollPosition { get; set; }

      public bool[] Collapsed { get; set; }
    }
  }
}
