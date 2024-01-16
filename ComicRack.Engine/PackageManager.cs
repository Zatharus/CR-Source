// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.PackageManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Runtime;
using cYo.Common.Text;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class PackageManager
  {
    private string packagePath;
    private string pendingPackagePath;

    public PackageManager(string path, string tempPath, bool commit)
    {
      this.PackagePath = path;
      this.PendingPackagePath = tempPath;
      if (!commit)
        return;
      this.Commit();
    }

    public string PackagePath
    {
      get => this.packagePath;
      set
      {
        this.packagePath = value;
        try
        {
          Directory.CreateDirectory(this.packagePath);
        }
        catch
        {
        }
      }
    }

    public string PendingPackagePath
    {
      get => this.pendingPackagePath;
      set
      {
        this.pendingPackagePath = value;
        try
        {
          Directory.CreateDirectory(this.pendingPackagePath);
        }
        catch
        {
        }
      }
    }

    public bool IsValid
    {
      get
      {
        try
        {
          return Directory.Exists(this.PackagePath) && Directory.Exists(this.PendingPackagePath);
        }
        catch
        {
          return false;
        }
      }
    }

    public IList<PackageManager.Package> GetPackages()
    {
      List<PackageManager.Package> packages = new List<PackageManager.Package>();
      try
      {
        foreach (string directory in Directory.GetDirectories(this.PackagePath))
        {
          if (!string.Equals(directory, this.PendingPackagePath, StringComparison.OrdinalIgnoreCase))
            packages.Add(PackageManager.Package.CreateFromPath(directory, false));
        }
        foreach (string directory in Directory.GetDirectories(this.PendingPackagePath))
          packages.Add(PackageManager.Package.CreateFromPath(directory, true));
      }
      catch
      {
      }
      return (IList<PackageManager.Package>) packages;
    }

    public IEnumerable<string> GetPackageNames()
    {
      return this.GetPackages().Select<PackageManager.Package, string>((Func<PackageManager.Package, string>) (p => p.Name));
    }

    public bool PackageExists(string name)
    {
      return this.GetPackageNames().Contains<string>(name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool PackageFileExists(string file)
    {
      try
      {
        return this.PackageExists(PackageManager.Package.CreateFromFile(file).Name);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool Install(string packageFile)
    {
      PackageManager.Package fromFile = PackageManager.Package.CreateFromFile(packageFile);
      if (fromFile == null)
        return false;
      string packagePath = this.GetPackagePath(fromFile, true);
      try
      {
        FileUtility.SafeDirectoryDelete(packagePath);
        PackageManager.Package.UnzipFile(packageFile, packagePath);
        return true;
      }
      catch (Exception ex)
      {
        FileUtility.SafeDirectoryDelete(packagePath);
        return false;
      }
    }

    public bool Uninstall(PackageManager.Package package)
    {
      try
      {
        switch (package.PackageType)
        {
          case PackageManager.PackageType.Installed:
            FileUtility.CreateEmpty(Path.Combine(package.PackagePath, ".remove"));
            break;
          case PackageManager.PackageType.PendingInstall:
            FileUtility.SafeDirectoryDelete(package.PackagePath);
            break;
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    public void Commit()
    {
      this.GetPackages().Where<PackageManager.Package>((Func<PackageManager.Package, bool>) (p => p.PackageType == PackageManager.PackageType.PendingRemove)).ForEach<PackageManager.Package>((Action<PackageManager.Package>) (p => PackageManager.CommitUninstallPackage(p)));
      this.GetPackages().Where<PackageManager.Package>((Func<PackageManager.Package, bool>) (p => p.PackageType == PackageManager.PackageType.PendingInstall)).ForEach<PackageManager.Package>((Action<PackageManager.Package>) (p => this.CommitInstallPackage(p)));
    }

    public void RemovePending()
    {
      this.GetPackages().ForEach<PackageManager.Package>((Action<PackageManager.Package>) (p => FileUtility.SafeDelete(Path.Combine(p.PackagePath, ".remove"))));
      FileUtility.SafeDirectoryDelete(this.PendingPackagePath);
    }

    private bool CommitInstallPackage(PackageManager.Package package)
    {
      if (package.PackageType != PackageManager.PackageType.PendingInstall)
        return false;
      string packagePath = this.GetPackagePath(package, false);
      try
      {
        Directory.CreateDirectory(packagePath);
        if (!((IEnumerable<string>) package.KeepFiles).IsEmpty<string>())
        {
          string[] keep = package.KeepFiles;
          foreach (string path in ((IEnumerable<string>) Directory.GetFiles(packagePath)).Where<string>((Func<string, bool>) (f => !((IEnumerable<string>) keep).Contains<string>(Path.GetFileName(f), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
            FileUtility.SafeDelete(path);
          foreach (string path in ((IEnumerable<string>) Directory.GetDirectories(packagePath)).Where<string>((Func<string, bool>) (f => !((IEnumerable<string>) keep).Contains<string>(Path.GetFileName(f), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
            FileUtility.SafeDirectoryDelete(path);
        }
        foreach (string file in Directory.GetFiles(package.PackagePath))
          File.Copy(file, Path.Combine(packagePath, Path.GetFileName(file)), true);
        return true;
      }
      catch
      {
        FileUtility.SafeDirectoryDelete(packagePath);
        return false;
      }
      finally
      {
        FileUtility.SafeDirectoryDelete(package.PackagePath);
      }
    }

    private static bool CommitUninstallPackage(PackageManager.Package package)
    {
      return FileUtility.SafeDirectoryDelete(package.PackagePath);
    }

    private string GetPackagePath(PackageManager.Package package, bool pending)
    {
      return Path.Combine(pending ? this.PendingPackagePath : this.PackagePath, package.Name);
    }

    public enum PackageType
    {
      None,
      Installed,
      PendingInstall,
      PendingRemove,
    }

    public class Package
    {
      private Package(string name) => this.Name = name;

      public string Name { get; private set; }

      public string PackagePath { get; private set; }

      public PackageManager.PackageType PackageType { get; private set; }

      public string Description { get; private set; }

      public string Author { get; private set; }

      public string Version { get; private set; }

      public string HelpLink { get; private set; }

      public Image Image { get; private set; }

      public bool Installed
      {
        get
        {
          return this.PackageType == PackageManager.PackageType.Installed || this.PackageType == PackageManager.PackageType.PendingInstall;
        }
      }

      public string[] KeepFiles { get; private set; }

      private T GetValue<T>(string value, T def)
      {
        return IniFile.GetValue<T>(Path.Combine(this.PackagePath, "package.ini"), value, def);
      }

      private void InitValues()
      {
        IniFile iniFile = new IniFile(Path.Combine(this.PackagePath, "package.ini"));
        this.Name = iniFile.GetValue<string>("Name", PackageManager.Package.FileToName(this.Name));
        this.Description = iniFile.GetValue<string>("Description", string.Empty);
        this.Author = iniFile.GetValue<string>("Author", string.Empty);
        this.Version = iniFile.GetValue<string>("Version", string.Empty);
        this.HelpLink = iniFile.GetValue<string>("HelpLink", string.Empty);
        this.KeepFiles = ((IEnumerable<string>) iniFile.GetValue<string>("KeepFiles", string.Empty).Split(',')).TrimStrings().RemoveEmpty().ToArray<string>();
        try
        {
          this.Image = (Image) BitmapExtensions.BitmapFromFile(Path.Combine(this.PackagePath, iniFile.GetValue<string>("Image", string.Empty))).Scale(32, 32);
        }
        catch
        {
        }
      }

      private static string FileToName(string file)
      {
        return Path.GetFileNameWithoutExtension(file).RemoveDigits().Replace(".", " ").Trim().StartToUpper().PascalToSpaced();
      }

      public static void UnzipFile(string packagePath, string targetPath)
      {
        Directory.CreateDirectory(targetPath);
        using (ZipFile source = new ZipFile(packagePath))
        {
          foreach (ZipEntry entry in source.Cast<ZipEntry>().Where<ZipEntry>((Func<ZipEntry, bool>) (ze => ze.IsFile)))
          {
            using (FileStream destination = File.Create(Path.Combine(targetPath, Path.GetFileName(entry.Name))))
            {
              using (Stream inputStream = source.GetInputStream(entry))
                inputStream.CopyTo((Stream) destination);
            }
          }
        }
      }

      public static string GetName(string file)
      {
        try
        {
          return PackageManager.Package.CreateFromFile(file).Name;
        }
        catch (Exception ex)
        {
          return (string) null;
        }
      }

      public static PackageManager.Package CreateFromPath(string name, string path, bool pending)
      {
        PackageManager.Package fromPath = new PackageManager.Package(name)
        {
          PackagePath = path
        };
        fromPath.InitValues();
        fromPath.PackageType = !pending ? (!File.Exists(Path.Combine(path, ".remove")) ? PackageManager.PackageType.Installed : PackageManager.PackageType.PendingRemove) : PackageManager.PackageType.PendingInstall;
        return fromPath;
      }

      public static PackageManager.Package CreateFromPath(string path, bool pending)
      {
        return PackageManager.Package.CreateFromPath(PackageManager.Package.FileToName(path), path, pending);
      }

      public static PackageManager.Package CreateFromFile(string file)
      {
        string str = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
          PackageManager.Package.UnzipFile(file, str);
          PackageManager.Package fromPath = PackageManager.Package.CreateFromPath(PackageManager.Package.FileToName(file), str, false);
          fromPath.PackagePath = file;
          fromPath.PackageType = PackageManager.PackageType.None;
          return fromPath;
        }
        catch (Exception ex)
        {
          return (PackageManager.Package) null;
        }
        finally
        {
          FileUtility.SafeDirectoryDelete(str);
        }
      }
    }
  }
}
