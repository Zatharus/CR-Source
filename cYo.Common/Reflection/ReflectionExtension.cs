// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.ReflectionExtension
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Common.Reflection
{
  public static class ReflectionExtension
  {
    public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit = true)
    {
      return member.GetCustomAttributes(typeof (T), inherit).OfType<T>();
    }

    public static T GetAttribute<T>(this MemberInfo member, bool inherit = true)
    {
      return member.GetAttributes<T>(inherit).FirstOrDefault<T>();
    }

    public static bool HasAttribute<T>(this MemberInfo member, bool inherit = true)
    {
      return (object) member.GetAttribute<T>(inherit) != null;
    }

    public static IEnumerable<T> GetAttributes<T>(this Type member, bool inherit = true)
    {
      return member.GetCustomAttributes(typeof (T), inherit).OfType<T>();
    }

    public static T GetAttribute<T>(this Type member, bool inherit = true)
    {
      return ReflectionExtension.GetAttributes<T>(member, inherit).FirstOrDefault<T>();
    }

    public static bool Browsable(this MemberInfo member, bool forced = false)
    {
      BrowsableAttribute attribute = member.GetAttribute<BrowsableAttribute>();
      return attribute != null ? attribute.Browsable : !forced;
    }

    public static string Category(this MemberInfo member)
    {
      CategoryAttribute attribute = member.GetAttribute<CategoryAttribute>();
      return attribute != null && !string.IsNullOrEmpty(attribute.Category) ? attribute.Category : (string) null;
    }

    public static string Description(this MemberInfo member)
    {
      DescriptionAttribute attribute = member.GetAttribute<DescriptionAttribute>();
      return attribute != null && !string.IsNullOrEmpty(attribute.Description) ? attribute.Description : (string) null;
    }

    public static T DefaultValue<T>(this MemberInfo member, T v = null)
    {
      DefaultValueAttribute attribute = member.GetAttribute<DefaultValueAttribute>();
      return attribute != null ? (T) Convert.ChangeType(attribute.Value, typeof (T)) : v;
    }

    public static object DefaultValue(this MemberInfo member)
    {
      return member.GetAttribute<DefaultValueAttribute>()?.Value;
    }

    public static bool HasDefaultValue(this MemberInfo member)
    {
      return member.GetAttribute<DefaultValueAttribute>() != null;
    }

    public static string Name(this Enum e) => Enum.GetName(e.GetType(), (object) e);

    public static string Description(this Enum e)
    {
      Type type = e.GetType();
      string name = Enum.GetName(type, (object) e);
      return type.GetField(name).Description() ?? name;
    }
  }
}
