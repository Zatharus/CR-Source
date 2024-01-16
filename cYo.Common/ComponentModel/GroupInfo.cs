// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.GroupInfo
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Localize;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class GroupInfo : IGroupInfo, IComparable<IGroupInfo>
  {
    public static readonly TR TRGroup = TR.Load("Groups");
    public static readonly string Unspecified = TR.Default[nameof (Unspecified), nameof (Unspecified)];
    private static readonly string[] DateGroups = GroupInfo.TRGroup.GetStrings(nameof (DateGroups), "Never|Today|Yesterday|Two Days Ago|Three Days Ago|This Week|Last Week|Two Weeks Ago|Three Weeks Ago|This Month|Last Month|Two Months ago|Three Months ago|Four Months ago|Five Months ago|Six Months ago|This Year|Last Year|Two Years ago|Three Years ago|Older than Three Years|Next Year|In the Future", '|');
    private static readonly string[] alphabetCaptions = GroupInfo.TRGroup.GetStrings("AlphabetGroups", "0-9|A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|Other", '|');
    private static readonly string[] sizeGroups = GroupInfo.TRGroup.GetStrings("SizeGroups", "Empty|Very Small|Small|Medium|Big|Huge", '|');

    public GroupInfo(object key, string caption, int index = -1)
    {
      this.Key = key;
      this.Caption = caption;
      this.Index = index;
    }

    public GroupInfo(string caption, int index)
      : this((object) caption, caption, index)
    {
    }

    public GroupInfo(string caption)
      : this((object) caption, caption)
    {
    }

    public object Key { get; set; }

    public int Index { get; set; }

    public string Caption { get; set; }

    public static IGroupInfo GetDateGroup(DateTime date, string unknown = null)
    {
      try
      {
        DateTime now = DateTime.Now;
        int num1 = now.Year * 12 + now.Month;
        int num2 = now.Year * 366 + now.DayOfYear;
        int num3 = date.Year * 12 + date.Month;
        int num4 = date.Year * 366 + date.DayOfYear;
        if (date.CompareTo(DateTime.MinValue, true) == 0)
          return (IGroupInfo) new GroupInfo(unknown ?? GroupInfo.DateGroups[0], 1000);
        if (num2 == num4)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[1], 0);
        if (num2 - 1 == num4)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[2], 1);
        if (num2 - 2 == num4)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[3], 2);
        if (num2 - 3 == num4)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[4], 3);
        if (num2 / 7 == num4 / 7)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[5], 10);
        if (num2 / 7 - 1 == num4 / 7)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[6], 11);
        if (num2 / 7 - 2 == num4 / 7)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[7], 12);
        if (num2 / 7 - 3 == num4 / 7)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[8], 13);
        if (num1 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[9], 20);
        if (num1 - 1 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[10], 21);
        if (num1 - 2 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[11], 22);
        if (num1 - 3 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[12], 23);
        if (num1 - 4 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[13], 24);
        if (num1 - 5 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[14], 25);
        if (num1 - 6 == num3)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[15], 26);
        if (now.Year == date.Year)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[16], 30);
        if (now.Year - 1 == date.Year)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[17], 31);
        if (now.Year - 2 == date.Year)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[18], 32);
        if (now.Year - 3 == date.Year)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[19], 33);
        if (now.Year + 1 == date.Year)
          return (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[21], 34);
        return now.Year < date.Year ? (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[22], 35) : (IGroupInfo) new GroupInfo(GroupInfo.DateGroups[20], 34);
      }
      catch
      {
        return (IGroupInfo) new GroupInfo("Missing Group Value", 100);
      }
    }

    public static string CompressedName(string text)
    {
      if (string.IsNullOrEmpty(text))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string text1 in text.Split(StringUtility.CommonSeparators))
      {
        if (!string.IsNullOrEmpty(text1) && !text1.IsArticle())
          stringBuilder.Append(text1);
      }
      return stringBuilder.ToString();
    }

    public static IGroupInfo GetCompressedNameGroup(string text)
    {
      return (IGroupInfo) new GroupInfo((object) GroupInfo.CompressedName(text).ToUpper(), string.IsNullOrEmpty(text) ? GroupInfo.Unspecified : text);
    }

    public static IEnumerable<IGroupInfo> GetCompressedNameGroups(string text)
    {
      if (text.Contains(","))
        return ((IEnumerable<string>) text.Split(',', StringSplitOptions.RemoveEmptyEntries)).Select<string, IGroupInfo>(new Func<string, IGroupInfo>(GroupInfo.GetCompressedNameGroup));
      return (IEnumerable<IGroupInfo>) new IGroupInfo[1]
      {
        GroupInfo.GetCompressedNameGroup(text)
      };
    }

    public static IGroupInfo GetAlphabetGroup(string text, bool articleAware)
    {
      int index1;
      if (string.IsNullOrEmpty(text))
      {
        index1 = GroupInfo.alphabetCaptions.Length - 1;
      }
      else
      {
        int index2 = articleAware ? text.IndexAfterArticle() : 0;
        char c = char.ToUpper(text[index2]).Normalize();
        if (char.IsDigit(c))
        {
          index1 = 0;
        }
        else
        {
          index1 = (int) c - 65 + 1;
          if (index1 < 1 || index1 > GroupInfo.alphabetCaptions.Length - 2)
            index1 = GroupInfo.alphabetCaptions.Length - 1;
        }
      }
      return (IGroupInfo) new GroupInfo(GroupInfo.alphabetCaptions[index1], index1);
    }

    public static IGroupInfo GetFileSizeGroup(long size)
    {
      if (size <= 0L)
        return (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[0], 0);
      if (size < 1048576L)
        return (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[1], 1);
      if (size < 10485760L)
        return (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[2], 2);
      if (size < 52428800L)
        return (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[3], 3);
      return size < 104857600L ? (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[4], 4) : (IGroupInfo) new GroupInfo(GroupInfo.sizeGroups[5], 5);
    }

    public static int Compare(IGroupInfo x, IGroupInfo y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      return x.Index == y.Index ? ExtendedStringComparer.Compare(x.Caption, y.Caption, ExtendedStringComparison.IgnoreArticles) : x.Index.CompareTo(y.Index);
    }

    public int CompareTo(IGroupInfo other) => GroupInfo.Compare((IGroupInfo) this, other);
  }
}
