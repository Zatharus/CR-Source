// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.FileFormat
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  [Serializable]
  public class FileFormat : IComparable<FileFormat>
  {
    private readonly string[] extensionArray;
    private readonly string extensions;
    private int iconId = 1;

    public FileFormat(string name, int id, string extensions)
    {
      this.Name = name;
      this.Id = id;
      this.extensions = extensions;
      this.extensionArray = extensions.Replace(" ", string.Empty).Split(';');
    }

    public string Name { get; set; }

    public int Id { get; set; }

    public string ExtensionList => this.extensions;

    public IEnumerable<string> Extensions => (IEnumerable<string>) this.extensionArray;

    public string ExtensionFilter
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string extension in this.extensionArray)
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append("; ");
          stringBuilder.Append("*");
          stringBuilder.Append(extension);
        }
        return stringBuilder.ToString();
      }
    }

    public int IconId
    {
      get => this.iconId;
      set => this.iconId = value;
    }

    public bool SupportsUpdate { get; set; }

    public bool Dynamic { get; set; }

    public bool HasExtension(string extension)
    {
      return ((IEnumerable<string>) this.extensionArray).Any<string>((Func<string, bool>) (ext => string.Equals(extension, ext, StringComparison.OrdinalIgnoreCase)));
    }

    public string MainExtension => this.extensionArray[0];

    public bool IsShellRegistered(string typeId)
    {
      try
      {
        return ((IEnumerable<string>) this.extensionArray).All<string>((Func<string, bool>) (ext => ShellRegister.IsFileOpenRegistered(typeId, ext) || ShellRegister.IsFileOpenWithRegistered(ext)));
      }
      catch
      {
        return false;
      }
    }

    public void RegisterShell(string typeId, string docName, bool overwrite)
    {
      try
      {
        foreach (string extension in this.extensionArray)
        {
          if (!ShellRegister.IsFileOpenInUse(typeId, extension) | overwrite)
            ShellRegister.RegisterFileOpen(typeId, extension, docName, this.iconId);
          else
            ShellRegister.RegisterFileOpenWith(extension);
        }
      }
      catch
      {
      }
    }

    public void UnregisterShell(string typeId)
    {
      try
      {
        foreach (string extension in this.extensionArray)
        {
          ShellRegister.UnregisterFileOpen(typeId, extension);
          ShellRegister.UnregisterFileOpenWith(extension);
        }
      }
      catch
      {
      }
    }

    public bool Supports(string source) => this.HasExtension(Path.GetExtension(source));

    public override string ToString() => this.Name + " (" + this.ExtensionFilter + ")";

    public static bool CanRegisterShell => ShellRegister.CanRegisterShell;

    public int CompareTo(FileFormat other)
    {
      return string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
  }
}
