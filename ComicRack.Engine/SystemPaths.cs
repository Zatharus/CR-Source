// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.SystemPaths
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Reflection;
using System;
using System.IO;
using System.Reflection;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class SystemPaths
  {
    public static readonly string ProductName = AssemblyInfo.GetProductName(Assembly.GetEntryAssembly());
    public static readonly string CompanyName = AssemblyInfo.GetCompanyName(Assembly.GetEntryAssembly());
    public readonly string ApplicationPath;
    public readonly string ApplicationDataPath;
    public readonly string LocalApplicationDataPath;
    public readonly string DatabasePath;
    public readonly string ThumbnailCachePath;
    public readonly string ImageCachePath;
    public readonly string FileCachePath;
    public readonly string CustomThumbnailPath;
    public readonly string ScriptPath;
    public readonly string ScriptPathSecondary;
    public readonly string PendingScriptsPath;

    public SystemPaths(
      bool useLocal,
      string alternateConfig,
      string databasePath,
      string cachePath)
    {
      Assembly assembly = Assembly.GetEntryAssembly();
      if ((object) assembly == null)
        assembly = Assembly.GetExecutingAssembly();
      this.ApplicationPath = Path.GetDirectoryName(assembly.Location);
      if (useLocal)
      {
        this.ApplicationDataPath = Path.Combine(this.ApplicationPath, "Data");
        this.LocalApplicationDataPath = Path.Combine(this.ApplicationPath, "Data");
      }
      else
      {
        this.ApplicationDataPath = SystemPaths.MakeApplicationPath(alternateConfig, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        this.LocalApplicationDataPath = SystemPaths.MakeApplicationPath(alternateConfig, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
      }
      this.DatabasePath = Path.Combine(string.IsNullOrEmpty(databasePath) ? this.ApplicationDataPath : databasePath, "ComicDb");
      this.ThumbnailCachePath = Path.Combine(string.IsNullOrEmpty(cachePath) ? Path.Combine(this.LocalApplicationDataPath, "Cache") : cachePath, "Thumbnails");
      this.ImageCachePath = Path.Combine(string.IsNullOrEmpty(cachePath) ? Path.Combine(this.LocalApplicationDataPath, "Cache") : cachePath, "Images");
      this.FileCachePath = Path.Combine(string.IsNullOrEmpty(cachePath) ? Path.Combine(this.LocalApplicationDataPath, "Cache") : cachePath, "Files");
      this.CustomThumbnailPath = Path.Combine(string.IsNullOrEmpty(cachePath) ? Path.Combine(this.LocalApplicationDataPath, "Cache") : cachePath, "CustomThumbnails");
      this.ScriptPath = Path.Combine(this.ApplicationPath, "Scripts");
      this.ScriptPathSecondary = Path.Combine(this.ApplicationDataPath, "Scripts");
      this.PendingScriptsPath = Path.Combine(this.ScriptPathSecondary, ".Pending");
    }

    public static string MakeApplicationPath(string alternateConfig, string folder)
    {
      if (!string.IsNullOrEmpty(SystemPaths.CompanyName))
        folder = Path.Combine(folder, SystemPaths.CompanyName);
      if (!string.IsNullOrEmpty(SystemPaths.ProductName))
        folder = Path.Combine(folder, SystemPaths.ProductName);
      if (!string.IsNullOrEmpty(alternateConfig))
        folder = Path.Combine(folder, Path.Combine("Configurations", alternateConfig));
      Directory.CreateDirectory(folder);
      return folder;
    }
  }
}
