// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Properties.Resources
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace cYo.Common.Presentation.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (cYo.Common.Presentation.Properties.Resources.resourceMan == null)
          cYo.Common.Presentation.Properties.Resources.resourceMan = new ResourceManager("cYo.Common.Presentation.Properties.Resources", typeof (cYo.Common.Presentation.Properties.Resources).Assembly);
        return cYo.Common.Presentation.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => cYo.Common.Presentation.Properties.Resources.resourceCulture;
      set => cYo.Common.Presentation.Properties.Resources.resourceCulture = value;
    }

    internal static byte[] Flags
    {
      get
      {
        return (byte[]) cYo.Common.Presentation.Properties.Resources.ResourceManager.GetObject(nameof (Flags), cYo.Common.Presentation.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap Plug
    {
      get => (Bitmap) cYo.Common.Presentation.Properties.Resources.ResourceManager.GetObject(nameof (Plug), cYo.Common.Presentation.Properties.Resources.resourceCulture);
    }
  }
}
