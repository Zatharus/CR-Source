// Decompiled with JetBrains decompiler
// Type: cYo.Common.Properties.Resources
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace cYo.Common.Properties
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
        if (cYo.Common.Properties.Resources.resourceMan == null)
          cYo.Common.Properties.Resources.resourceMan = new ResourceManager("cYo.Common.Properties.Resources", typeof (cYo.Common.Properties.Resources).Assembly);
        return cYo.Common.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => cYo.Common.Properties.Resources.resourceCulture;
      set => cYo.Common.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap Folder
    {
      get
      {
        return (Bitmap) cYo.Common.Properties.Resources.ResourceManager.GetObject(nameof (Folder), cYo.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap FolderHome
    {
      get
      {
        return (Bitmap) cYo.Common.Properties.Resources.ResourceManager.GetObject(nameof (FolderHome), cYo.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap PlusOverlay
    {
      get
      {
        return (Bitmap) cYo.Common.Properties.Resources.ResourceManager.GetObject(nameof (PlusOverlay), cYo.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap Wikipedia
    {
      get
      {
        return (Bitmap) cYo.Common.Properties.Resources.ResourceManager.GetObject(nameof (Wikipedia), cYo.Common.Properties.Resources.resourceCulture);
      }
    }
  }
}
