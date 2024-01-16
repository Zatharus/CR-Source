// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.Base32
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Text;

#nullable disable
namespace cYo.Common.Text
{
  public static class Base32
  {
    private static readonly char[] Base32Chars = new char[32]
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7'
    };

    public static string ToBase32String(byte[] inArray)
    {
      if (inArray == null)
        return (string) null;
      int length = inArray.Length;
      int num1 = length / 5;
      int num2 = length - 5 * num1;
      StringBuilder stringBuilder = new StringBuilder();
      int num3 = 0;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        byte[] numArray1 = inArray;
        int index2 = num3;
        int num4 = index2 + 1;
        byte num5 = numArray1[index2];
        byte[] numArray2 = inArray;
        int index3 = num4;
        int num6 = index3 + 1;
        byte num7 = numArray2[index3];
        byte[] numArray3 = inArray;
        int index4 = num6;
        int num8 = index4 + 1;
        byte num9 = numArray3[index4];
        byte[] numArray4 = inArray;
        int index5 = num8;
        int num10 = index5 + 1;
        byte num11 = numArray4[index5];
        byte[] numArray5 = inArray;
        int index6 = num10;
        num3 = index6 + 1;
        byte num12 = numArray5[index6];
        stringBuilder.Append(Base32.Base32Chars[(int) num5 >> 3]);
        stringBuilder.Append(Base32.Base32Chars[(int) num5 << 2 & 31 | (int) num7 >> 6]);
        stringBuilder.Append(Base32.Base32Chars[(int) num7 >> 1 & 31]);
        stringBuilder.Append(Base32.Base32Chars[(int) num7 << 4 & 31 | (int) num9 >> 4]);
        stringBuilder.Append(Base32.Base32Chars[(int) num9 << 1 & 31 | (int) num11 >> 7]);
        stringBuilder.Append(Base32.Base32Chars[(int) num11 >> 2 & 31]);
        stringBuilder.Append(Base32.Base32Chars[(int) num11 << 3 & 31 | (int) num12 >> 5]);
        stringBuilder.Append(Base32.Base32Chars[(int) num12 & 31]);
      }
      if (num2 > 0)
      {
        byte[] numArray6 = inArray;
        int index7 = num3;
        int index8 = index7 + 1;
        byte num13 = numArray6[index7];
        stringBuilder.Append(Base32.Base32Chars[(int) num13 >> 3]);
        switch (num2)
        {
          case 1:
            stringBuilder.Append(Base32.Base32Chars[(int) num13 << 2 & 31]);
            break;
          case 2:
            byte num14 = inArray[index8];
            stringBuilder.Append(Base32.Base32Chars[(int) num13 << 2 & 31 | (int) num14 >> 6]);
            stringBuilder.Append(Base32.Base32Chars[(int) num14 >> 1 & 31]);
            stringBuilder.Append(Base32.Base32Chars[(int) num14 << 4 & 31]);
            break;
          case 3:
            byte[] numArray7 = inArray;
            int index9 = index8;
            int index10 = index9 + 1;
            byte num15 = numArray7[index9];
            byte num16 = inArray[index10];
            stringBuilder.Append(Base32.Base32Chars[(int) num13 << 2 & 31 | (int) num15 >> 6]);
            stringBuilder.Append(Base32.Base32Chars[(int) num15 >> 1 & 31]);
            stringBuilder.Append(Base32.Base32Chars[(int) num15 << 4 & 31 | (int) num16 >> 4]);
            stringBuilder.Append(Base32.Base32Chars[(int) num16 << 1 & 31]);
            break;
          case 4:
            byte[] numArray8 = inArray;
            int index11 = index8;
            int num17 = index11 + 1;
            byte num18 = numArray8[index11];
            byte[] numArray9 = inArray;
            int index12 = num17;
            int index13 = index12 + 1;
            byte num19 = numArray9[index12];
            byte num20 = inArray[index13];
            stringBuilder.Append(Base32.Base32Chars[(int) num13 << 2 & 31 | (int) num18 >> 6]);
            stringBuilder.Append(Base32.Base32Chars[(int) num18 >> 1 & 31]);
            stringBuilder.Append(Base32.Base32Chars[(int) num18 << 4 & 31 | (int) num19 >> 4]);
            stringBuilder.Append(Base32.Base32Chars[(int) num19 << 1 & 31 | (int) num20 >> 7]);
            stringBuilder.Append(Base32.Base32Chars[(int) num20 >> 2 & 31]);
            stringBuilder.Append(Base32.Base32Chars[(int) num20 << 3 & 31]);
            break;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
