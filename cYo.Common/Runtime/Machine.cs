// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.Machine
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Threading;

#nullable disable
namespace cYo.Common.Runtime
{
  public static class Machine
  {
    public static long Ticks => DateTime.Now.Ticks / 10000L;

    public static void Sleep(int ms) => Thread.Sleep(ms);

    public static bool Is64Bit => Environment.Is64BitProcess;
  }
}
