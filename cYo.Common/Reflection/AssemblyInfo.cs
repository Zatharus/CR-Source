// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.AssemblyInfo
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Reflection;

#nullable disable
namespace cYo.Common.Reflection
{
  public static class AssemblyInfo
  {
    public static string GetCompanyName(Assembly ass)
    {
      if (ass == (Assembly) null)
        return (string) null;
      object[] customAttributes = ass.GetCustomAttributes(typeof (AssemblyCompanyAttribute), true);
      return customAttributes.Length == 0 ? (string) null : ((AssemblyCompanyAttribute) customAttributes[0]).Company;
    }

    public static string GetProductName(Assembly ass)
    {
      if (ass == (Assembly) null)
        return (string) null;
      object[] customAttributes = ass.GetCustomAttributes(typeof (AssemblyProductAttribute), true);
      return customAttributes.Length == 0 ? (string) null : ((AssemblyProductAttribute) customAttributes[0]).Product;
    }
  }
}
