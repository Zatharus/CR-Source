// Decompiled with JetBrains decompiler
// Type: cYo.Common.BitUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common
{
  public static class BitUtility
  {
    public static int SetMask(this int n, int mask, bool set = true) => !set ? n & ~mask : n | mask;

    public static int Clear(this int n, int mask) => n.SetMask(mask, false);

    public static int Flip(this int n, int mask) => n.SetMask(mask, !n.IsSet(mask));

    public static T SetMask<T>(this Enum n, T mask, bool set = true)
    {
      int int32_1 = Convert.ToInt32((object) n);
      int int32_2 = Convert.ToInt32((object) mask);
      return (T) (ValueType) (set ? int32_1 | int32_2 : int32_1 & ~int32_2);
    }

    public static T Clear<T>(this Enum n, T mask) => n.SetMask<T>(mask, false);

    public static T Flip<T>(this Enum n, T mask) => n.SetMask<T>(mask, !n.IsSet<T>(mask));

    public static bool IsSet(this int n, int mask, bool all)
    {
      return all ? (n & mask) == mask : (n & mask) != 0;
    }

    public static bool IsSet(this int n, int mask) => n.IsSet(mask, true);

    public static bool IsNotSet(this int n, int mask) => !n.IsSet(mask);

    public static bool IsSet<T>(this Enum n, T mask, bool all = true)
    {
      int int32_1 = Convert.ToInt32((object) n);
      int int32_2 = Convert.ToInt32((object) mask);
      return all ? (int32_1 & int32_2) == int32_2 : (int32_1 & int32_2) != 0;
    }

    public static bool IsNotSet<T>(this Enum n, T mask) => !n.IsSet<T>(mask);

    public static int GetBitCount(int n)
    {
      int bitCount = 0;
      for (int index = 0; index < 32 && n != 0; ++index)
      {
        if ((n & 1) != 0)
          ++bitCount;
        n >>= 1;
      }
      return bitCount;
    }

    public static IEnumerable<int> GetBitValues(int n, bool set = true)
    {
      for (int i = 0; i < 32; ++i)
      {
        int bitValue = 1 << i;
        if ((n & bitValue) != 0 == set)
          yield return bitValue;
      }
    }

    public static int EndianSwap(this int x)
    {
      return x >> 24 & (int) byte.MaxValue | x << 8 & 16711680 | x >> 8 & 65280 | x << 24;
    }

    public static long EndianSwap(this long x)
    {
      return x >> 56 & (long) byte.MaxValue | x << 40 & 71776119061217280L | x << 24 & 280375465082880L | x << 8 & 1095216660480L | x >> 8 & 4278190080L | x >> 24 & 16711680L | x >> 40 & 65280L | x << 56;
    }

    public static int HiInt(this long x) => (int) (x >> 32);

    public static int LoInt(this long x) => (int) (x & (long) uint.MaxValue);

    public static int CreateMask(params bool[] flags)
    {
      int mask = 0;
      foreach (bool flag in flags)
      {
        mask <<= 1;
        if (flag)
          mask |= 1;
      }
      return mask;
    }
  }
}
