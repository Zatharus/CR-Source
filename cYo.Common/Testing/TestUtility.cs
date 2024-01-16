// Decompiled with JetBrains decompiler
// Type: cYo.Common.Testing.TestUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.Diagnostics;

#nullable disable
namespace cYo.Common.Testing
{
  public static class TestUtility
  {
    public static IDisposable Time(string message = "Time needed: {0}")
    {
      Stopwatch sw = Stopwatch.StartNew();
      return (IDisposable) new LeanDisposer((Action) (() =>
      {
        sw.Stop();
        Console.WriteLine(message, (object) sw.Elapsed);
      }));
    }
  }
}
