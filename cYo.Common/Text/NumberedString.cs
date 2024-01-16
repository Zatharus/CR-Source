// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.NumberedString
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Text
{
  public static class NumberedString
  {
    private static readonly Regex rxBrackets = new Regex("\\s*\\((?<number>\\d+)\\)", RegexOptions.Compiled);
    private static Regex rxRange = new Regex("(?<from>\\d*)\\s*(?<range>-?)\\s*(?<to>\\d*)", RegexOptions.Compiled);

    public static int GetNumber(string s)
    {
      Match match = NumberedString.rxBrackets.Match(s ?? string.Empty);
      int result = 0;
      if (match.Success)
        int.TryParse(match.Groups["number"].Value, out result);
      return result;
    }

    public static string StripNumber(string s)
    {
      return NumberedString.rxBrackets.Replace(s ?? string.Empty, string.Empty);
    }

    public static int MaxNumber(IEnumerable<string> texts)
    {
      try
      {
        return texts.Max<string>((Func<string, int>) (t => NumberedString.GetNumber(t) + 1));
      }
      catch (Exception ex)
      {
        return 0;
      }
    }

    public static string Format(string s, int number)
    {
      return number >= 2 ? string.Format("{0} ({1})", (object) s, (object) number) : s;
    }

    public static bool TestRangeString(this string s, int n)
    {
      if (s == null || s.Trim() == string.Empty)
        return true;
      MatchCollection matchCollection = NumberedString.rxRange.Matches(s);
      if (matchCollection.Count == 0)
        return true;
      foreach (Match match in matchCollection)
      {
        if (match.Length != 0)
        {
          int result = 0;
          bool flag = match.Groups["range"].Value == "-";
          int num1 = int.TryParse(match.Groups["from"].Value, out result) ? result : int.MinValue;
          int num2 = int.TryParse(match.Groups["to"].Value, out result) ? result : int.MaxValue;
          if (flag)
          {
            if (n >= num1 && n <= num2)
              return true;
          }
          else if (n == num1 || n == num2)
            return true;
        }
      }
      return false;
    }
  }
}
