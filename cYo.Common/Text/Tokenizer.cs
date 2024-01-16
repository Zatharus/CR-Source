// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.Tokenizer
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
  public class Tokenizer
  {
    private readonly string source;
    private readonly MatchCollection matches;
    private int position;
    private bool trim;

    public Tokenizer(Regex tokenizer, string text, bool trim = true)
    {
      this.source = text;
      this.matches = tokenizer.Matches(text);
      this.position = 0;
      this.trim = trim;
    }

    public IEnumerable<Tokenizer.Token> GetAll()
    {
      Tokenizer.Token t;
      while ((t = this.Take()) != null)
        yield return t;
    }

    public Tokenizer.Token Get(int n)
    {
      Match match = this.matches[n];
      string str = match.Value;
      if (this.trim)
        str = str.Trim();
      return new Tokenizer.Token()
      {
        Source = this.source,
        Text = str,
        Index = match.Index,
        Length = match.Length
      };
    }

    public Tokenizer.Token Take(string startsWith = null, string endsWith = null, bool trimStartEnd = true)
    {
      Tokenizer.Token current = this.Current;
      ++this.position;
      string str = current == null ? string.Empty : current.Text;
      if (!string.IsNullOrEmpty(startsWith))
      {
        if (current == null)
          this.ThrowEndException(current);
        if (!str.StartsWith(startsWith))
          current.ThrowParserException(string.Format("'{0}' must start with '{1}'", (object) current.Text, (object) startsWith));
        if (trimStartEnd)
          str = str.Substring(startsWith.Length);
      }
      if (!string.IsNullOrEmpty(endsWith))
      {
        if (current == null)
          this.ThrowEndException(current);
        if (!str.EndsWith(endsWith))
          current.ThrowParserException(string.Format("'{0}' must end with '{1}'", (object) current.Text, (object) endsWith));
        if (trimStartEnd)
          str = str.Substring(0, str.Length - endsWith.Length);
      }
      if (current != null)
        current.Text = str;
      return current;
    }

    public Tokenizer.Token TakeString()
    {
      Tokenizer.Token token = this.Take("\"", "\"");
      token.Text = token.Text.Unescape();
      return token;
    }

    public Tokenizer.Token Expect(params string[] expect)
    {
      if (this.Current == null || !this.Current.Is(expect))
        this.ThrowExpectedException(this.Current, expect);
      return this.Take();
    }

    public bool Is(params string[] p)
    {
      if (this.Current == null)
        this.ThrowExpectedException(this.Current, p);
      return this.IsOptional(p);
    }

    public bool IsOptional(params string[] p)
    {
      return ((IEnumerable<string>) p).Any<string>((Func<string, bool>) (s => string.Equals(this.Text, s, StringComparison.OrdinalIgnoreCase)));
    }

    public void Skip(int count = 1) => this.position += count;

    public int Count => this.matches.Count;

    public string Text => this.Current == null ? (string) null : this.Current.Text;

    public Tokenizer.Token Current
    {
      get => this.position >= this.matches.Count ? (Tokenizer.Token) null : this.Get(this.position);
    }

    public Tokenizer.Token Last
    {
      get
      {
        return this.position - 1 >= this.matches.Count ? (Tokenizer.Token) null : this.Get(this.position - 1);
      }
    }

    public Tokenizer.Token Next
    {
      get
      {
        return this.position + 1 >= this.matches.Count ? (Tokenizer.Token) null : this.Get(this.position + 1);
      }
    }

    public bool EndReached => this.Current == null;

    private void ThrowEndException(Tokenizer.Token token)
    {
      if (token == null)
        throw new Tokenizer.ParseException("Unexpected end reached.", (Tokenizer.Token) null);
    }

    private void ThrowExpectedException(Tokenizer.Token token, params string[] expected)
    {
      string str = expected.Length == 1 ? expected[0] : string.Format("one of ({0})", (object) expected.ToListString(", "));
      if (token == null)
        throw new Exception("Unexpected end reached");
      token.ThrowParserException(string.Format("Expected '{0}', but found '{1}'", (object) str, (object) token.Text));
    }

    public class TextPosition
    {
      public int Line { get; set; }

      public int Column { get; set; }
    }

    public class ParseException : Exception
    {
      public ParseException(string message, Tokenizer.Token token)
        : base(message)
      {
        this.Token = token;
      }

      public Tokenizer.Token Token { get; private set; }
    }

    public class Token
    {
      public string Text { get; set; }

      public string Source { get; set; }

      public int Index { get; set; }

      public int Length { get; set; }

      public Tokenizer.TextPosition Position
      {
        get
        {
          int line;
          int column;
          StringUtility.ConvertIndexToLineAndColumn(this.Source, this.Index, out line, out column);
          return new Tokenizer.TextPosition()
          {
            Line = line,
            Column = column
          };
        }
      }

      public bool Is(params string[] p)
      {
        return ((IEnumerable<string>) p).Any<string>((Func<string, bool>) (s => string.Equals(this.Text, s, StringComparison.OrdinalIgnoreCase)));
      }

      public void ThrowEndException()
      {
      }

      public void ThrowParserException(string format)
      {
        throw new Tokenizer.ParseException(string.Format(format, (object) this.Text), this);
      }

      public override string ToString() => this.Text;
    }
  }
}
