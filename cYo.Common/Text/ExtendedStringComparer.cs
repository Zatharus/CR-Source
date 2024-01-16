// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.ExtendedStringComparer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Text
{
  public sealed class ExtendedStringComparer : IComparer<string>
  {
    private readonly ExtendedStringComparison compareMode;
    private static readonly IComparer<string> defaultComparer = (IComparer<string>) new ExtendedStringComparer();
    private static readonly IComparer<string> defaultZeroesFirst = (IComparer<string>) new ExtendedStringComparer(ExtendedStringComparison.ZeroesFirst);
    private static readonly IComparer<string> defaultNoArticles = (IComparer<string>) new ExtendedStringComparer(ExtendedStringComparison.IgnoreArticles);

    public ExtendedStringComparer(ExtendedStringComparison compareMode)
    {
      this.compareMode = compareMode;
    }

    public ExtendedStringComparer()
      : this(ExtendedStringComparison.Default)
    {
    }

    int IComparer<string>.Compare(string x, string y)
    {
      return ExtendedStringComparer.Compare(x, y, this.compareMode);
    }

    public int Compare(object x, object y)
    {
      string s1 = x as string;
      string s2 = y as string;
      return s1 != null && s2 != null ? ExtendedStringComparer.Compare(s1, s2, this.compareMode) : Comparer.Default.Compare(x, y);
    }

    public static IComparer<string> Default => ExtendedStringComparer.defaultComparer;

    public static IComparer<string> DefaultZeroesFirst => ExtendedStringComparer.defaultZeroesFirst;

    public static IComparer<string> DefaultNoArticles => ExtendedStringComparer.defaultNoArticles;

    public static int Compare(string s1, string s2)
    {
      return ExtendedStringComparer.Compare(s1, s2, ExtendedStringComparison.Default);
    }

    public static int Compare(string s1, string s2, ExtendedStringComparison compareMode)
    {
      if (string.IsNullOrEmpty(s1))
        return !string.IsNullOrEmpty(s2) ? -1 : 0;
      if (string.IsNullOrEmpty(s2))
        return 1;
      if (s1 == s2)
        return 0;
      int i1 = 0;
      int i2 = 0;
      bool zeroesFirst = (compareMode & ExtendedStringComparison.ZeroesFirst) != 0;
      bool ignoreCase = (compareMode & ExtendedStringComparison.IgnoreCase) != 0;
      bool flag1 = (compareMode & ExtendedStringComparison.Ordinal) != 0;
      bool flag2 = (compareMode & ExtendedStringComparison.IgnoreArticles) != 0;
      int num1 = 0;
      if (flag2)
      {
        i1 = s1.IndexAfterArticle();
        i2 = s2.IndexAfterArticle();
        num1 = Math.Sign(i2 - i1);
      }
      int length1 = s1.Length;
      int length2 = s2.Length;
      bool flag3 = char.IsLetterOrDigit(s1[i1]);
      bool flag4 = char.IsLetterOrDigit(s2[i2]);
      if (flag3 && !flag4)
        return 1;
      if (!flag3 & flag4)
        return -1;
      do
      {
        char c1 = s1[i1];
        char c2 = s2[i2];
        bool flag5 = char.IsDigit(c1);
        bool flag6 = char.IsDigit(c2);
        if (!flag5 && !flag6)
        {
          if ((int) c1 != (int) c2)
          {
            bool flag7 = char.IsLetter(c1);
            bool flag8 = char.IsLetter(c2);
            if (flag7 & flag8)
            {
              int num2 = flag1 ? (int) char.ToUpper(c1) - (int) char.ToUpper(c2) : string.Compare(s1, i1, s2, i2, 1, ignoreCase);
              if (num2 != 0)
                return num2;
            }
            else if (!flag7 && !flag8)
            {
              int num3 = flag1 ? (int) c1 - (int) c2 : string.Compare(s1, i1, s2, i2, 1);
              if (num3 != 0)
                return num3;
            }
            else
              return flag7 ? 1 : -1;
          }
        }
        else if (flag5 & flag6)
        {
          int num4 = ExtendedStringComparer.CompareNumbers(s1, length1, ref i1, s2, length2, ref i2, zeroesFirst);
          if (num4 != 0)
            return num4;
        }
        else
          return flag5 ? -1 : 1;
        ++i1;
        ++i2;
        if (i1 >= length1)
          return i2 >= length2 ? num1 : -1;
      }
      while (i2 < length2);
      return 1;
    }

    private static int CompareNumbers(
      string s1,
      int s1Length,
      ref int i1,
      string s2,
      int s2Length,
      ref int i2,
      bool zeroesFirst)
    {
      int nzStart1 = i1;
      int nzStart2 = i2;
      int end1 = i1;
      int end2 = i2;
      ExtendedStringComparer.ScanNumber(s1, s1Length, i1, ref nzStart1, ref end1);
      ExtendedStringComparer.ScanNumber(s2, s2Length, i2, ref nzStart2, ref end2);
      int num1 = i1;
      i1 = end1 - 1;
      int num2 = i2;
      i2 = end2 - 1;
      if (zeroesFirst)
      {
        int num3 = nzStart1 - num1;
        int num4 = nzStart2 - num2;
        if (num3 > num4)
          return -1;
        if (num3 < num4)
          return 1;
      }
      int num5 = end2 - nzStart2;
      int num6 = end1 - nzStart1;
      if (num5 == num6)
      {
        int index1 = nzStart1;
        int index2 = nzStart2;
        while (index1 <= i1)
        {
          int num7 = (int) s1[index1] - (int) s2[index2];
          if (num7 != 0)
            return num7;
          ++index1;
          ++index2;
        }
        num5 = end1 - num1;
        num6 = end2 - num2;
        if (num5 == num6)
          return 0;
      }
      return num5 > num6 ? -1 : 1;
    }

    private static void ScanNumber(string s, int length, int start, ref int nzStart, ref int end)
    {
      nzStart = start;
      end = start;
      bool flag = true;
      char c = s[end];
      do
      {
        if (flag)
        {
          if ('0' == c)
            ++nzStart;
          else
            flag = false;
        }
        ++end;
        if (end < length)
          c = s[end];
        else
          goto label_7;
      }
      while (char.IsDigit(c));
      goto label_8;
label_7:
      return;
label_8:;
    }
  }
}
