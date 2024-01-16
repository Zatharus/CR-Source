// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.Diagnostic
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Text;
using cYo.Common.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Runtime
{
  public static class Diagnostic
  {
    [Conditional("DEBUG")]
    public static void WaitAndConsume(int ms)
    {
      long num = Machine.Ticks + (long) ms;
      do
        ;
      while (DateTime.Now.Ticks < num);
    }

    public static void StartWatchDog(BarkEventHandler bark, int lockTestTimeSeconds = 0)
    {
      ThreadUtility.AddActiveThread(Thread.CurrentThread);
      CrashWatchDog crashWatchDog = new CrashWatchDog()
      {
        LockTestTime = TimeSpan.FromSeconds((double) lockTestTimeSeconds)
      };
      crashWatchDog.Bark += bark;
      crashWatchDog.Register();
    }

    public static void WriteProgramInfo(TextWriter sw)
    {
      sw.WriteLine("Application: {0}", (object) Application.ProductName);
      sw.WriteLine("Version    : {0}", (object) Application.ProductVersion);
      sw.WriteLine("Assembly   : {0}", (object) Assembly.GetEntryAssembly().GetName().Version);
      sw.WriteLine("OS         : {0} {1}", (object) Environment.OSVersion, Environment.Is64BitProcess ? (object) "64" : (object) "32");
      sw.WriteLine(".NET       : {0}", (object) Environment.Version);
      sw.WriteLine("Processors : {0}", (object) Environment.ProcessorCount);
      sw.WriteLine("Workingset : {0}", (object) new FileLengthFormat().Format((string) null, (object) Environment.WorkingSet, (IFormatProvider) null));
    }
  }
}
