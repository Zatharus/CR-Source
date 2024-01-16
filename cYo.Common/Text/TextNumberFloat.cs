// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.TextNumberFloat
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Linq;

#nullable disable
namespace cYo.Common.Text
{
  public class TextNumberFloat : IComparable<TextNumberFloat>
  {
    private float number;
    private string text;

    public TextNumberFloat()
    {
    }

    public TextNumberFloat(string text)
      : this()
    {
      this.Text = text;
    }

    public float Number
    {
      get => this.number;
      protected set
      {
        this.IsNumber = true;
        this.number = value;
      }
    }

    public bool IsNumber { get; private set; }

    public string Text
    {
      get => this.text;
      private set
      {
        if (this.text == value)
          return;
        this.text = value.Trim();
        this.IsNumber = false;
        this.OnParseText(this.text);
      }
    }

    public int CompareTo(TextNumberFloat other)
    {
      int num = 0;
      if (this.IsNumber && other.IsNumber)
        num = this.Number.CompareTo(other.Number);
      if (num == 0)
        num = string.Compare(this.Text, other.Text, StringComparison.Ordinal);
      return num;
    }

    protected virtual void OnParseText(string s)
    {
      float f;
      if (!s.TryParse(out f, true))
        return;
      this.Number = f;
    }

    public static bool Parse(string text, int start, out float accu, out int n)
    {
      bool flag = false;
      int num1 = 1;
      int num2 = 10;
      int num3 = 1;
      accu = 0.0f;
      n = start;
      while (n < text.Length)
      {
        char ch = text[n];
        switch (ch)
        {
          case ' ':
            if (flag)
            {
              accu *= (float) num1;
              return true;
            }
            break;
          case '+':
            if (flag)
            {
              accu *= (float) num1;
              return true;
            }
            num1 = 1;
            break;
          case ',':
          case '.':
            flag = true;
            if (num3 == 1)
            {
              num2 = 1;
              num3 = 10;
              break;
            }
            break;
          case '-':
            if (flag)
            {
              accu *= (float) num1;
              return true;
            }
            num1 *= -1;
            break;
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            flag = true;
            accu = (float) ((double) accu * (double) num2 + (double) ((int) ch - 48) / (double) num3);
            if (num3 != 1)
            {
              num3 *= 10;
              break;
            }
            break;
          case '\u00BC':
            flag = true;
            accu += 0.25f;
            break;
          case '\u00BD':
            flag = true;
            accu += 0.5f;
            break;
          case '\u00BE':
            flag = true;
            accu += 0.75f;
            break;
          default:
            accu *= (float) num1;
            return flag;
        }
        ++n;
      }
      accu *= (float) num1;
      return flag;
    }

    public static bool ParseExpression(string text, int start, out float accu, out int n)
    {
      char ch = '+';
      bool expression = false;
      accu = 0.0f;
      n = start;
      float accu1;
      int n1;
      while (n < text.Length && TextNumberFloat.Parse(text, n, out accu1, out n1))
      {
        n = n1;
        expression = true;
        switch (ch)
        {
          case '*':
            accu *= accu1;
            break;
          case '+':
            accu += accu1;
            break;
          case '-':
            accu -= accu1;
            break;
          case '/':
            accu /= accu1;
            break;
        }
        if (n < text.Length)
        {
          ch = text[n];
          if (!"*/+-".Contains<char>(ch))
            return true;
        }
        ++n;
      }
      return expression;
    }

    public static bool TryParseExpression(string text, out float accu)
    {
      return TextNumberFloat.ParseExpression(text, 0, out accu, out int _);
    }

    public static float Parse(string text)
    {
      float accu;
      if (!TextNumberFloat.TryParseExpression(text, out accu))
        throw new FormatException();
      return accu;
    }
  }
}
