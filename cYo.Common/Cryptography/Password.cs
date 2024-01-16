// Decompiled with JetBrains decompiler
// Type: cYo.Common.Cryptography.Password
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace cYo.Common.Cryptography
{
  public static class Password
  {
    private static readonly HashAlgorithm algorithm = (HashAlgorithm) new SHA1Managed();

    public static string CreateHash(string text)
    {
      return Convert.ToBase64String(Password.CreateByteHash(text));
    }

    public static byte[] CreateByteHash(string text)
    {
      return Password.CreateByteHash(Encoding.UTF8.GetBytes(text));
    }

    public static byte[] CreateByteHash(byte[] text)
    {
      return text.Length == 0 ? new byte[0] : Password.algorithm.ComputeHash(text);
    }

    public static bool Verify(string text, string hashValue)
    {
      return !string.IsNullOrEmpty(hashValue) && hashValue.Equals(Password.CreateHash(text));
    }

    public static string Create(int len)
    {
      StringBuilder stringBuilder = new StringBuilder();
      Random random = new Random();
      for (int index = 0; index < len; ++index)
      {
        int num1 = random.Next(60);
        if (num1 < 10)
        {
          stringBuilder.Append(48 + num1);
        }
        else
        {
          int num2 = num1 - 10;
          if (num2 < 25)
          {
            stringBuilder.Append(97 + num2);
          }
          else
          {
            int num3 = num2 - 25;
            stringBuilder.Append(65 + num3);
          }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
