// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.ApproximateMatching
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Text
{
  public static class ApproximateMatching
  {
    public static int LevenshteinDistance(string s, string t)
    {
      return ApproximateMatching.LevenshteinDistance(s, t, false);
    }

    public static int LevenshteinDistance(string s, string t, bool ignoreCase)
    {
      int length1 = s.Length;
      int length2 = t.Length;
      int[] b = new int[length2 + 1];
      int[] a = new int[length2 + 1];
      if (length1 == 0)
        return length2;
      if (length2 == 0)
        return length1;
      if (ignoreCase)
      {
        s = s.ToLower();
        t = t.ToLower();
      }
      int index1 = 0;
      while (index1 <= length2)
        b[index1] = index1++;
      for (int index2 = 1; index2 <= length1; ++index2)
      {
        a[0] = index2;
        char ch = s[index2 - 1];
        for (int index3 = 1; index3 <= length2; ++index3)
        {
          int num = (int) t[index3 - 1] == (int) ch ? 0 : 1;
          a[index3] = Math.Min(Math.Min(b[index3] + 1, a[index3 - 1] + 1), b[index3 - 1] + num);
        }
        CloneUtility.Swap<int[]>(ref a, ref b);
      }
      return b[length2];
    }
  }
}
