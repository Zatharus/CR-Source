// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.StringUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Text
{
  public static class StringUtility
  {
    public static readonly char[] CommonSeparators = new char[15]
    {
      ' ',
      '\t',
      '\n',
      '\r',
      '-',
      '~',
      ',',
      '.',
      ';',
      ':',
      '/',
      '\\',
      '\'',
      '´',
      '`'
    };
    private static string[] articles;
    private static string[] articlesWithSpaces;
    private static readonly Regex rxFloat = new Regex("[-\\+]?\\d*\\.?\\d+", RegexOptions.Compiled);
    private static readonly Regex rxInt = new Regex("[-\\+]?\\d+", RegexOptions.Compiled);
    private static readonly Regex rxUpperWordStart = new Regex("\\b(?<letter>)[A-Z0-9]", RegexOptions.Compiled);
    private static readonly Regex rxLowerWordStart = new Regex("\\b(?<letter>[a-z])", RegexOptions.Compiled);
    private static readonly Regex rxmiddleLetters = new Regex("\\B(?<letter>[a-z0-9])", RegexOptions.Compiled);
    private static readonly Regex rxRemoveFillWords = new Regex("(?<!\\A)\\b(for|the|of|to|in|at|by|and|or)\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex rxStartWord = new Regex("\\b\\w", RegexOptions.Compiled);
    private static readonly Regex rxNonWordLetters = new Regex("[^\\s\\w]", RegexOptions.Compiled);
    private static readonly Regex rxSpace = new Regex("\\s", RegexOptions.Compiled);
    private static readonly Regex rxMultiSpace = new Regex("\\s{2,}", RegexOptions.Compiled);

    static StringUtility() => StringUtility.Articles = "the, der, die, das";

    public static string Articles
    {
      get => StringUtility.articles.ToListString(", ");
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        List<string> stringList1 = new List<string>();
        List<string> stringList2 = new List<string>();
        string str1 = value;
        char[] chArray = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray))
        {
          string str3 = str2.Trim();
          if (!string.IsNullOrEmpty(str3))
          {
            stringList1.Add(str3);
            stringList2.Add(str3 + " ");
          }
        }
        StringUtility.articles = stringList1.ToArray();
        StringUtility.articlesWithSpaces = stringList2.ToArray();
      }
    }

    public static void ConvertIndexToLineAndColumn(
      string text,
      int index,
      out int line,
      out int column)
    {
      line = 1;
      column = 1;
      foreach (char ch in text)
      {
        if (index-- == 0)
          break;
        switch (ch)
        {
          case '\n':
            ++line;
            column = 1;
            continue;
          case '\r':
            continue;
          default:
            ++column;
            continue;
        }
      }
    }

    public static string Escape(this string text, IEnumerable<char> characters, char escape)
    {
      foreach (char ch in ListExtensions.AsEnumerable<char>(escape).Concat<char>(characters))
      {
        string oldValue = ch.ToString();
        text = text.Replace(oldValue, escape.ToString() + oldValue);
      }
      return text;
    }

    public static string Escape(this string text) => text.Escape((IEnumerable<char>) "\"", '\\');

    public static string Unescape(this string text, IEnumerable<char> characters, char escape)
    {
      foreach (char ch in characters.Concat<char>(ListExtensions.AsEnumerable<char>(escape)))
      {
        string newValue = ch.ToString();
        text = text.Replace(escape.ToString() + newValue, newValue);
      }
      return text;
    }

    public static string Unescape(this string text)
    {
      return text.Unescape((IEnumerable<char>) "\"", '\\');
    }

    public static string[] Split(this string s, char c, StringSplitOptions options)
    {
      return s.Split(new char[1]{ c }, options);
    }

    public static string[] Split(this string s, string c, StringSplitOptions options)
    {
      return s.Split(new string[1]{ c }, options);
    }

    public static string[] Split(this string s, int lengthFirstPart, int between)
    {
      return new string[2]
      {
        s.Left(lengthFirstPart),
        s.Substring(lengthFirstPart + between)
      };
    }

    public static string[] Split(this string s, int lengthFirstPart) => s.Split(lengthFirstPart, 0);

    public static string ToXmlString(this string s)
    {
      return string.IsNullOrEmpty(s) ? string.Empty : SecurityElement.Escape(s);
    }

    public static bool IsArticle(this string text)
    {
      return ((IEnumerable<string>) StringUtility.articles).Any<string>((Func<string, bool>) (a => string.Compare(text, a, true) == 0));
    }

    public static bool Contains(this string s, string search, StringComparison comparison)
    {
      return s != null && s.IndexOf(search, comparison) != -1;
    }

    public static int IndexAfterArticle(this string s)
    {
      for (int index = 0; index < StringUtility.articlesWithSpaces.Length; ++index)
      {
        string articlesWithSpace = StringUtility.articlesWithSpaces[index];
        if (s.StartsWith(articlesWithSpace, StringComparison.OrdinalIgnoreCase))
          return articlesWithSpace.Length;
      }
      return 0;
    }

    public static string Remove(this string s, string text) => s.Replace(text, string.Empty);

    public static string RemoveArticle(this string s)
    {
      int startIndex = s.IndexAfterArticle();
      return startIndex != -1 ? s.Substring(startIndex) : s;
    }

    public static int ExtendedCompareTo(this string a, string b, ExtendedStringComparison mode)
    {
      return ExtendedStringComparer.Compare(a, b, mode);
    }

    public static int ExtendedCompareTo(this string a, string b, bool ignoreCase = false)
    {
      return a.ExtendedCompareTo(b, ignoreCase ? ExtendedStringComparison.IgnoreCase : ExtendedStringComparison.Default);
    }

    public static bool StartsWith(
      this string a,
      string b,
      StringComparison comparisonType,
      bool ignoreArticles)
    {
      if (!ignoreArticles)
        return a.StartsWith(b, comparisonType);
      int startIndex = a.IndexAfterArticle();
      return a.IndexOf(b, startIndex, Math.Min(b.Length, a.Length - startIndex), comparisonType) == startIndex;
    }

    public static string PascalToSpaced(this string pascalFormattedString)
    {
      if (pascalFormattedString.Contains(" "))
        return pascalFormattedString;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = true;
      bool flag2 = false;
      foreach (char c in pascalFormattedString)
      {
        bool flag3 = char.IsUpper(c);
        bool flag4 = char.IsDigit(c);
        if (flag3 && !flag2 && stringBuilder.Length != 0)
          stringBuilder.Append(' ');
        if (flag4 && !flag1)
          stringBuilder.Append(' ');
        flag1 = flag4;
        flag2 = flag3;
        stringBuilder.Append(c);
      }
      return stringBuilder.ToString();
    }

    public static string StartToUpper(this string s)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char c in s)
        stringBuilder.Append(stringBuilder.Length == 0 ? char.ToUpper(c) : c);
      return stringBuilder.ToString();
    }

    public static int CompareNumberString(
      this string a,
      string b,
      StringComparison ct,
      bool invariantNumber)
    {
      float f1;
      float f2;
      return a.TryParse(out f1, invariantNumber) && b.TryParse(out f2, invariantNumber) ? f1.CompareTo(f2) : string.Compare(a, b, ct);
    }

    public static bool TryParse(this string number, out float f, bool invariant)
    {
      Match match = StringUtility.rxFloat.Match(number ?? string.Empty);
      CultureInfo provider = invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
      f = 0.0f;
      return match.Success && float.TryParse(match.Value, NumberStyles.Float, (IFormatProvider) provider, out f);
    }

    public static bool TryParse(this string number, out int n, bool invariant)
    {
      Match match = StringUtility.rxInt.Match(number ?? string.Empty);
      CultureInfo provider = invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
      n = 0;
      return match.Success && int.TryParse(match.Value, NumberStyles.Integer, (IFormatProvider) provider, out n);
    }

    public static string ToListString(this IEnumerable list, string separator)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (object obj in list)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(separator);
        stringBuilder.Append(obj.ToString());
      }
      return stringBuilder.ToString();
    }

    public static IEnumerable<string> FromListString(this string listString, char separator)
    {
      if (string.IsNullOrEmpty(listString))
        return Enumerable.Empty<string>();
      return !listString.Contains(separator.ToString()) ? ListExtensions.AsEnumerable<string>(listString.Trim()) : ((IEnumerable<string>) listString.Split(separator)).Select<string, string>((Func<string, string>) (s => s.Trim())).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s)));
    }

    public static HashSet<string> ListStringToSet(this string listString, char separator)
    {
      return new HashSet<string>(listString.FromListString(separator));
    }

    public static string Format(string format, params object[] args)
    {
      try
      {
        return string.Format(format, args);
      }
      catch
      {
        return format;
      }
    }

    private static string Prefix(char prefix, string s, Regex search, HashSet<char> used)
    {
      foreach (Match match in search.Matches(s))
      {
        Group group = match.Groups[0];
        char upper = char.ToUpper(group.Value[0]);
        if (!used.Contains(upper))
        {
          used.Add(upper);
          return s.Insert(group.Index, prefix.ToString());
        }
      }
      return s;
    }

    public static void Prefix(
      IList<string> texts,
      char prefix,
      StringUtility.PrefixOptions options = StringUtility.PrefixOptions.Default)
    {
      HashSet<char> used = new HashSet<char>();
      Regex regex = new Regex(string.Format("\\{0}(?=[a-zA-Z0-9])", (object) prefix));
      for (int index = 0; index < texts.Count; ++index)
      {
        string str1 = texts[index];
        if ((options & StringUtility.PrefixOptions.RemovePrefixes) != StringUtility.PrefixOptions.None)
          str1 = regex.Replace(str1, string.Empty);
        string str2 = str1;
        if ((options & StringUtility.PrefixOptions.UseCaptialWordStarts) != StringUtility.PrefixOptions.None)
          str2 = StringUtility.Prefix(prefix, str1, StringUtility.rxUpperWordStart, used);
        if (str2 == str1 && (options & StringUtility.PrefixOptions.UseSmallWordStarts) != StringUtility.PrefixOptions.None)
          str2 = StringUtility.Prefix(prefix, str1, StringUtility.rxLowerWordStart, used);
        if (str2 == str1 && (options & StringUtility.PrefixOptions.UseNonStartingLetters) != StringUtility.PrefixOptions.None)
          str2 = StringUtility.Prefix(prefix, str1, StringUtility.rxmiddleLetters, used);
        texts[index] = str2;
      }
    }

    public static bool HasLetters(this IEnumerable<char> text)
    {
      return text.Any<char>((Func<char, bool>) (c => char.IsLetter(c)));
    }

    public static string OnlyDigits(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      string empty = string.Empty;
      foreach (char c in text)
      {
        if (char.IsDigit(c) || c == '.' || c == ',')
          empty += c.ToString();
      }
      return empty;
    }

    public static string RemoveDigits(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      string empty = string.Empty;
      foreach (char c in text)
      {
        if (!char.IsDigit(c))
          empty += c.ToString();
      }
      return empty;
    }

    public static string AppendWithSeparator(
      this string text,
      string separator,
      params string[] texts)
    {
      text = text ?? string.Empty;
      foreach (string text1 in texts)
      {
        if (!string.IsNullOrEmpty(text1))
        {
          if (!string.IsNullOrEmpty(text))
            text += separator;
          text += text1;
        }
      }
      return text;
    }

    public static string MakeEditBoxMultiline(string text)
    {
      return text?.Replace("\r\n", "\n").Replace("\n", "\r\n");
    }

    public static IEnumerable<string> TrimStrings(this IEnumerable<string> list)
    {
      return list.Select<string, string>((Func<string, string>) (x => x.Trim()));
    }

    public static IEnumerable<string> TrimEndStrings(this IEnumerable<string> list)
    {
      return list.Select<string, string>((Func<string, string>) (x => x.TrimEnd()));
    }

    public static IEnumerable<string> RemoveEmpty(this IEnumerable<string> list)
    {
      return list.Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
    }

    public static string CutOff(this string text, params char[] delimiters)
    {
      int length = text.IndexOfAny(delimiters);
      return length == -1 ? text : text.Substring(0, length);
    }

    public static bool IsNumber(this string text)
    {
      foreach (char c in text)
      {
        if (!char.IsNumber(c))
          return false;
      }
      return true;
    }

    public static char Normalize(this char c)
    {
      int index = "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿÖöÜüÄä".IndexOf(c);
      return index == -1 ? c : "AAAAAAACEEEEIIIIDNOOOOOOUUUUYPSaaaaaaaceeeeiiiionoooooouuuuybyOoUuAa"[index];
    }

    public static string ReplaceAny(this string s, string search, char newChar)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(search))
        return s;
      search.ForEach<char>((Action<char>) (c => s = s.Replace(c, newChar)));
      return s;
    }

    public static string ReplaceAny(this string s, string search, string newChar)
    {
      if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(search))
        return s;
      search.ForEach<char>((Action<char>) (c => s = s.Replace(c.ToString(), newChar)));
      return s;
    }

    public static bool ContainsAny(this string s, string characters)
    {
      return !string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(characters) && characters.Any<char>((Func<char, bool>) (c => s.Contains<char>(c)));
    }

    public static string Left(this string s, int len)
    {
      try
      {
        return s.Substring(0, len);
      }
      catch (Exception ex)
      {
        return s;
      }
    }

    public static string Ellipsis(this string s, int len, string append, int minLength = 5)
    {
      if (s == null)
        return s;
      len -= append.Length;
      if (len < minLength)
        len = minLength;
      return s.Length < len ? s : s.Left(len) + append;
    }

    public static string LineBreak(this string s, int lineLength)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      if (string.IsNullOrEmpty(s))
        return s;
      string str1 = s.Replace(Environment.NewLine, "\n");
      char[] chArray1 = new char[1]{ '\n' };
      foreach (string str2 in str1.Split(chArray1))
      {
        if (stringBuilder1.Length > 0)
          stringBuilder1.Append(Environment.NewLine);
        bool flag = true;
        string str3 = str2;
        char[] chArray2 = new char[1]{ ' ' };
        foreach (string str4 in str3.Split(chArray2))
        {
          if (stringBuilder2.Length > 0)
            stringBuilder2.Append(" ");
          stringBuilder2.Append(str4);
          if (stringBuilder2.Length > lineLength)
          {
            if (!flag)
              stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append((object) stringBuilder2);
            stringBuilder2.Clear();
            flag = false;
          }
        }
        if (stringBuilder2.Length > 0)
        {
          if (!flag)
            stringBuilder1.Append(Environment.NewLine);
          stringBuilder1.Append((object) stringBuilder2);
          stringBuilder2.Clear();
        }
      }
      return stringBuilder1.ToString();
    }

    public static string Intent(this string s, int intention)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = new string(' ', intention);
      string[] strArray = s.Replace(Environment.NewLine, "\n").Split('\n');
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str2 = strArray[index];
        stringBuilder.Append(str1);
        stringBuilder.Append(str2);
        if (index != strArray.Length - 1)
          stringBuilder.Append(Environment.NewLine);
      }
      return stringBuilder.ToString();
    }

    public static string ToHexString(this byte[] data, bool trimZeros = false)
    {
      StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
      string format = trimZeros ? "{0:x}" : "{0:x2}";
      foreach (byte num in data)
        stringBuilder.AppendFormat(format, (object) num);
      return stringBuilder.ToString();
    }

    public static string SafeFormat(this string format, params object[] objects)
    {
      try
      {
        return string.Format(format, objects);
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    public static string SafeTrim(this string s) => s?.Trim();

    public static string ShortenText(
      this string text,
      int maxLength = -1,
      StringUtility.ShortenTextOptions options = StringUtility.ShortenTextOptions.Default)
    {
      if (options.HasFlag((Enum) StringUtility.ShortenTextOptions.RemoveFillwWords))
        text = StringUtility.rxRemoveFillWords.Replace(text, string.Empty);
      if (options.HasFlag((Enum) StringUtility.ShortenTextOptions.RemoveUppercaseWords))
      {
        char[] charArray = text.ToCharArray();
        for (Match match = StringUtility.rxStartWord.Match(text); match.Success; match = match.NextMatch())
          charArray[match.Index] = char.ToUpper(charArray[match.Index]);
        text = new string(charArray);
      }
      if (options.HasFlag((Enum) StringUtility.ShortenTextOptions.RemoveNonWordLetters))
        text = StringUtility.rxNonWordLetters.Replace(text, string.Empty);
      text = !options.HasFlag((Enum) StringUtility.ShortenTextOptions.RemoveSpaces) ? StringUtility.rxMultiSpace.Replace(text, " ") : StringUtility.rxSpace.Replace(text, string.Empty);
      return text.Trim().Left(maxLength);
    }

    [Flags]
    public enum PrefixOptions
    {
      None = 0,
      RemovePrefixes = 1,
      UseCaptialWordStarts = 2,
      UseSmallWordStarts = 4,
      UseNonStartingLetters = 8,
      Default = UseNonStartingLetters | UseSmallWordStarts | UseCaptialWordStarts | RemovePrefixes, // 0x0000000F
    }

    public enum ShortenTextOptions
    {
      None = 0,
      RemoveSpaces = 1,
      RemoveNonWordLetters = 2,
      RemoveUppercaseWords = 4,
      RemoveFillwWords = 8,
      Default = 15, // 0x0000000F
    }
  }
}
