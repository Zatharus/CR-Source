// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.ControlExtensions
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows
{
  public static class ControlExtensions
  {
    private static bool IsValid(Control c) => c != null && !c.IsDisposed;

    public static void Invoke(this Control control, Action action)
    {
      if (!ControlExtensions.IsValid(control))
        return;
      control.Invoke((Delegate) (() =>
      {
        if (!ControlExtensions.IsValid(control))
          return;
        action();
      }));
    }

    public static void BeginInvoke(this Control control, Action action, bool catchErrors = true)
    {
      if (!ControlExtensions.IsValid(control))
        return;
      control.BeginInvoke((Delegate) (() =>
      {
        try
        {
          if (!ControlExtensions.IsValid(control))
            return;
          action();
        }
        catch (Exception ex)
        {
          if (catchErrors)
            return;
          throw;
        }
      }));
    }

    public static bool InvokeIfRequired(this Control control, Action action)
    {
      if (!ControlExtensions.IsValid(control) || !control.InvokeRequired)
        return false;
      ControlExtensions.Invoke(control, action);
      return true;
    }

    public static bool BeginInvokeIfRequired(this Control control, Action action, bool catchErrors = true)
    {
      if (!ControlExtensions.IsValid(control) || !control.InvokeRequired)
        return false;
      ControlExtensions.BeginInvoke(control, action, catchErrors);
      return true;
    }

    public static Control TopParent(this Control c)
    {
      Control control = c;
      while ((c = c.Parent) != null)
        control = c;
      return control;
    }

    public static Control Clone(this Control controlToClone)
    {
      Type type = controlToClone.GetType();
      Control instance = (Control) Activator.CreateInstance(type);
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (property.CanWrite && property.Name != "WindowTarget")
          property.SetValue((object) instance, property.GetValue((object) controlToClone, (object[]) null), (object[]) null);
      }
      return instance;
    }
  }
}
