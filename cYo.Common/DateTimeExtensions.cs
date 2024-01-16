// Decompiled with JetBrains decompiler
// Type: cYo.Common.DateTimeExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Localize;
using System;

#nullable disable
namespace cYo.Common
{
  public static class DateTimeExtensions
  {
    private static Lazy<string[]> relativeFormat = new Lazy<string[]>((Func<string[]>) (() => TR.Load("Common").GetStrings("RelativeDateTimeFormat", "minute|minutes|hour|hours|day|days|week|weeks|month|months|year|years|{0} ago|in {0}", '|')));

    public static bool IsDateOnly(this DateTime dt)
    {
      return dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0 && dt.Millisecond == 0;
    }

    public static DateTime DateOnly(this DateTime dt) => new DateTime(dt.Year, dt.Month, dt.Day);

    public static int CompareTo(this DateTime dt, DateTime value, bool ignoreTime)
    {
      if (!ignoreTime)
        return dt.CompareTo(value);
      int num1 = Math.Sign(dt.Year - value.Year);
      if (num1 != 0)
        return num1;
      int num2 = Math.Sign(dt.Month - value.Month);
      return num2 != 0 ? num2 : Math.Sign(dt.Day - value.Day);
    }

    public static DateTime SafeToLocalTime(this DateTime dt)
    {
      return !(dt == DateTime.MinValue) ? dt.ToLocalTime() : dt;
    }

    public static string ToRelativeDateString(this DateTime dt, DateTime toDate)
    {
      string[] strArray = DateTimeExtensions.relativeFormat.Value;
      string format;
      if (dt > toDate)
      {
        CloneUtility.Swap<DateTime>(ref dt, ref toDate);
        format = strArray[strArray.Length - 1];
      }
      else
        format = strArray[strArray.Length - 2];
      TimeSpan timeSpan = toDate - dt;
      int totalMinutes = (int) timeSpan.TotalMinutes;
      int totalHours = (int) timeSpan.TotalHours;
      int totalDays = (int) timeSpan.TotalDays;
      int num1 = totalDays / 7;
      int num2 = num1 / 4;
      int num3 = num1 / 52;
      string str;
      if (totalMinutes <= 1)
      {
        str = "1 " + strArray[0];
      }
      else
      {
        switch (totalHours)
        {
          case 0:
            str = totalMinutes.ToString() + " " + strArray[1];
            break;
          case 1:
            str = "1 " + strArray[2];
            break;
          default:
            switch (totalDays)
            {
              case 0:
                str = totalHours.ToString() + " " + strArray[3];
                break;
              case 1:
                str = "1 " + strArray[4];
                break;
              default:
                switch (num1)
                {
                  case 0:
                    str = totalDays.ToString() + " " + strArray[5];
                    break;
                  case 1:
                    str = "1 " + strArray[6];
                    break;
                  default:
                    switch (num2)
                    {
                      case 0:
                        str = num1.ToString() + " " + strArray[7];
                        break;
                      case 1:
                        str = "1 " + strArray[8];
                        break;
                      default:
                        switch (num3)
                        {
                          case 0:
                            str = num2.ToString() + " " + strArray[9];
                            break;
                          case 1:
                            str = "1 " + strArray[10];
                            break;
                          default:
                            str = num3.ToString() + " " + strArray[11];
                            break;
                        }
                        break;
                    }
                    break;
                }
                break;
            }
            break;
        }
      }
      return string.Format(format, (object) str);
    }
  }
}
