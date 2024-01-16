// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.ResetValueAttribute
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Common.Reflection
{
  [AttributeUsage(AttributeTargets.Property, Inherited = true)]
  public class ResetValueAttribute : Attribute
  {
    public ResetValueAttribute(int level = 0) => this.Level = level;

    public int Level { get; private set; }

    public static void ResetProperties(object data, int level = 0)
    {
      if (data == null)
        return;
      ((IEnumerable<PropertyInfo>) data.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.CanWrite && pi.HasAttribute<DefaultValueAttribute>() && pi.HasAttribute<ResetValueAttribute>())).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.GetAttribute<ResetValueAttribute>().Level <= level)).ForEach<PropertyInfo>((Action<PropertyInfo>) (pi => pi.SetValue(data, pi.DefaultValue(), (object[]) null)));
    }
  }
}
