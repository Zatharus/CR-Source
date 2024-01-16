// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ProviderFactory`1
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public class ProviderFactory<T> where T : class
  {
    private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
    private readonly List<ProviderInfo> providerDict = new List<ProviderInfo>();

    public void RegisterProvider(Type pt, IEnumerable<FileFormat> formats, bool withLocking = true)
    {
      using (withLocking ? this.rwLock.UpgradeableReadLock() : (IDisposable) null)
      {
        if (this.providerDict.Any<ProviderInfo>((Func<ProviderInfo, bool>) (pi => pi.ProviderType == pt)))
          return;
        using (withLocking ? this.rwLock.WriteLock() : (IDisposable) null)
          this.providerDict.Add(new ProviderInfo(pt, formats));
      }
    }

    public void RegisterProvider(Type pt, bool withLocking = true)
    {
      if (Activator.CreateInstance(pt) is IValidateProvider instance && !instance.IsValid)
        return;
      this.RegisterProvider(pt, ReflectionExtension.GetAttributes<FileFormatAttribute>(pt, true).Select<FileFormatAttribute, FileFormat>((Func<FileFormatAttribute, FileFormat>) (ffa => ffa.Format)), withLocking);
    }

    public void RegisterProviders(Assembly assembly, Type baseType)
    {
      using (this.rwLock.WriteLock())
        ((IEnumerable<Type>) assembly.GetTypes()).Where<Type>((Func<Type, bool>) (t => !t.IsAbstract && t.IsSubclassOf(baseType) && t.GetConstructor(new Type[0]) != (ConstructorInfo) null)).ForEach<Type>((Action<Type>) (t => this.RegisterProvider(t, false)));
    }

    public void RegisterProviders(Assembly assembly)
    {
      this.RegisterProviders(assembly, typeof (T));
    }

    public void RegisterProviders() => this.RegisterProviders(Assembly.GetExecutingAssembly());

    public IEnumerable<ProviderInfo> GetProviderInfos()
    {
      return this.providerDict.ReadLock<ProviderInfo>(this.rwLock);
    }

    public IEnumerable<Type> GetProviderTypes()
    {
      return this.GetProviderInfos().Select<ProviderInfo, Type>((Func<ProviderInfo, Type>) (pi => pi.ProviderType));
    }

    public IEnumerable<ProviderInfo> GetSourceProviderInfos(string source)
    {
      return this.GetProviderInfos().Where<ProviderInfo>((Func<ProviderInfo, bool>) (pi => pi.Formats.Any<FileFormat>((Func<FileFormat, bool>) (f => f.Supports(source)))));
    }

    public IEnumerable<Type> GetSourceProviderTypes(string source)
    {
      return this.GetSourceProviderInfos(source).Select<ProviderInfo, Type>((Func<ProviderInfo, Type>) (f => f.ProviderType));
    }

    public ProviderInfo GetSourceProviderInfo(string source)
    {
      return this.GetSourceProviderInfos(source).FirstOrDefault<ProviderInfo>();
    }

    public Type GetSourceProviderType(string source)
    {
      return this.GetSourceProviderTypes(source).FirstOrDefault<Type>();
    }

    public IEnumerable<FileFormat> GetSourceFormats()
    {
      return this.GetProviderInfos().SelectMany<ProviderInfo, FileFormat>((Func<ProviderInfo, IEnumerable<FileFormat>>) (pi => pi.Formats));
    }

    public IEnumerable<FileFormat> GetSourceFormats(string source)
    {
      return this.GetSourceProviderInfos(source).SelectMany<ProviderInfo, FileFormat>((Func<ProviderInfo, IEnumerable<FileFormat>>) (pi => pi.Formats));
    }

    public FileFormat GetSourceFormat(string source)
    {
      return this.GetSourceFormats(source).FirstOrDefault<FileFormat>((Func<FileFormat, bool>) (ff => ff.Supports(source)));
    }

    public string GetSourceFormatName(string source)
    {
      FileFormat sourceFormat = this.GetSourceFormat(source);
      return sourceFormat != null && !string.IsNullOrEmpty(sourceFormat.Name) ? sourceFormat.Name : TR.Default["Unknown", "Unknown"];
    }

    public Type GetFormatProviderType(string formatName)
    {
      return this.GetProviderInfos().Where<ProviderInfo>((Func<ProviderInfo, bool>) (pi => pi.Formats.Any<FileFormat>((Func<FileFormat, bool>) (f => f.Name == formatName)))).Select<ProviderInfo, Type>((Func<ProviderInfo, Type>) (pi => pi.ProviderType)).FirstOrDefault<Type>();
    }

    public Type GetFormatProviderType(int formatId)
    {
      return this.GetProviderInfos().Where<ProviderInfo>((Func<ProviderInfo, bool>) (pi => pi.Formats.Any<FileFormat>((Func<FileFormat, bool>) (f => f.Id == formatId)))).Select<ProviderInfo, Type>((Func<ProviderInfo, Type>) (pi => pi.ProviderType)).FirstOrDefault<Type>();
    }

    public IEnumerable<string> GetFileExtensions()
    {
      return this.GetSourceFormats().SelectMany<FileFormat, string>((Func<FileFormat, IEnumerable<string>>) (f => f.Extensions)).Distinct<string>();
    }

    public T CreateFormatProvider(string formatName)
    {
      try
      {
        return Activator.CreateInstance(this.GetFormatProviderType(formatName)) as T;
      }
      catch (Exception ex)
      {
        return default (T);
      }
    }

    public T CreateFormatProvider(int formatId)
    {
      Type formatProviderType = this.GetFormatProviderType(formatId);
      return !(formatProviderType != (Type) null) ? default (T) : Activator.CreateInstance(formatProviderType) as T;
    }

    public IEnumerable<T> CreateProviders()
    {
      return this.GetProviderTypes().Select<Type, T>((Func<Type, T>) (t => Activator.CreateInstance(t) as T));
    }

    public virtual T CreateSourceProvider(string source)
    {
      try
      {
        return Activator.CreateInstance(this.GetSourceProviderType(source)) as T;
      }
      catch
      {
        return default (T);
      }
    }

    public string GetDialogFilter(bool withAllFilter, bool sort)
    {
      IEnumerable<FileFormat> fileFormats = this.GetSourceFormats();
      if (sort)
        fileFormats = (IEnumerable<FileFormat>) fileFormats.OrderBy<FileFormat, FileFormat>((Func<FileFormat, FileFormat>) (f => f));
      return fileFormats.GetDialogFilter(withAllFilter);
    }
  }
}
