// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.SevenZipFactory
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public class SevenZipFactory : DisposableObject
  {
    private static readonly Dictionary<KnownSevenZipFormat, Guid> knownFormats = new Dictionary<KnownSevenZipFormat, Guid>()
    {
      {
        KnownSevenZipFormat.SevenZip,
        new Guid("23170f69-40c1-278a-1000-000110070000")
      },
      {
        KnownSevenZipFormat.Arj,
        new Guid("23170f69-40c1-278a-1000-000110040000")
      },
      {
        KnownSevenZipFormat.BZip2,
        new Guid("23170f69-40c1-278a-1000-000110020000")
      },
      {
        KnownSevenZipFormat.Cab,
        new Guid("23170f69-40c1-278a-1000-000110080000")
      },
      {
        KnownSevenZipFormat.Chm,
        new Guid("23170f69-40c1-278a-1000-000110e90000")
      },
      {
        KnownSevenZipFormat.Compound,
        new Guid("23170f69-40c1-278a-1000-000110e50000")
      },
      {
        KnownSevenZipFormat.Cpio,
        new Guid("23170f69-40c1-278a-1000-000110ed0000")
      },
      {
        KnownSevenZipFormat.Deb,
        new Guid("23170f69-40c1-278a-1000-000110ec0000")
      },
      {
        KnownSevenZipFormat.GZip,
        new Guid("23170f69-40c1-278a-1000-000110ef0000")
      },
      {
        KnownSevenZipFormat.Iso,
        new Guid("23170f69-40c1-278a-1000-000110e70000")
      },
      {
        KnownSevenZipFormat.Lzh,
        new Guid("23170f69-40c1-278a-1000-000110060000")
      },
      {
        KnownSevenZipFormat.Lzma,
        new Guid("23170f69-40c1-278a-1000-0001100a0000")
      },
      {
        KnownSevenZipFormat.Nsis,
        new Guid("23170f69-40c1-278a-1000-000110090000")
      },
      {
        KnownSevenZipFormat.Rar,
        new Guid("23170f69-40c1-278a-1000-000110030000")
      },
      {
        KnownSevenZipFormat.Rpm,
        new Guid("23170f69-40c1-278a-1000-000110eb0000")
      },
      {
        KnownSevenZipFormat.Split,
        new Guid("23170f69-40c1-278a-1000-000110ea0000")
      },
      {
        KnownSevenZipFormat.Tar,
        new Guid("23170f69-40c1-278a-1000-000110ee0000")
      },
      {
        KnownSevenZipFormat.Wim,
        new Guid("23170f69-40c1-278a-1000-000110e60000")
      },
      {
        KnownSevenZipFormat.Z,
        new Guid("23170f69-40c1-278a-1000-000110050000")
      },
      {
        KnownSevenZipFormat.Zip,
        new Guid("23170f69-40c1-278a-1000-000110010000")
      }
    };
    private readonly SevenZipFactory.NativeMethods.SafeLibraryHandle libHandle;
    private readonly CreateObjectDelegate createObject;

    public SevenZipFactory(string sevenZipLibPath)
    {
      this.libHandle = SevenZipFactory.NativeMethods.LoadLibrary(sevenZipLibPath);
      this.createObject = !this.libHandle.IsInvalid ? (CreateObjectDelegate) Marshal.GetDelegateForFunctionPointer(SevenZipFactory.NativeMethods.GetProcAddress(this.libHandle, "CreateObject"), typeof (CreateObjectDelegate)) : throw new Win32Exception();
      if (object.Equals((object) this.createObject, (object) null))
      {
        this.libHandle.Close();
        throw new ArgumentException();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (this.libHandle == null || this.libHandle.IsClosed)
        return;
      this.libHandle.Close();
    }

    private T CreateInterface<T>(Guid classId) where T : class
    {
      Guid guid = typeof (T).GUID;
      object outObject;
      int num = this.createObject(ref classId, ref guid, out outObject);
      return outObject as T;
    }

    public IInArchive CreateInArchive(KnownSevenZipFormat format)
    {
      return this.CreateInterface<IInArchive>(SevenZipFactory.knownFormats[format]);
    }

    public IOutArchive CreateOutArchive(KnownSevenZipFormat format)
    {
      return this.CreateInterface<IOutArchive>(SevenZipFactory.knownFormats[format]);
    }

    private static class NativeMethods
    {
      private const string Kernel32Dll = "kernel32.dll";

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern SevenZipFactory.NativeMethods.SafeLibraryHandle LoadLibrary(
        [MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

      [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
      public static extern IntPtr GetProcAddress(
        SevenZipFactory.NativeMethods.SafeLibraryHandle hModule,
        [MarshalAs(UnmanagedType.LPStr)] string procName);

      public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
      {
        public SafeLibraryHandle()
          : base(true)
        {
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
          return SevenZipFactory.NativeMethods.SafeLibraryHandle.FreeLibrary(this.handle);
        }
      }
    }
  }
}
