// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.VersionNeutralBinder
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Runtime
{
  public sealed class VersionNeutralBinder : SerializationBinder
  {
    private static readonly Regex rxVersion = new Regex(", Version=.*?PublicKeyToken=[a-f0-9]*", RegexOptions.Compiled);

    public override Type BindToType(string assemblyName, string typeName)
    {
      typeName = VersionNeutralBinder.rxVersion.Replace(typeName, string.Empty);
      return Type.GetType(typeName);
    }
  }
}
