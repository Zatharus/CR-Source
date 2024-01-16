// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.SizeValue
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public struct SizeValue
  {
    public int Value;
    public bool IsPercent;

    public SizeValue(int value, bool isPercent)
    {
      this.Value = value;
      this.IsPercent = isPercent;
    }

    public SizeValue(string value)
    {
      string lower = value.ToLower();
      this.IsPercent = false;
      if (lower == "auto")
        this.Value = 0;
      else if (lower.EndsWith("%"))
      {
        this.IsPercent = true;
        this.Value = int.Parse(lower.Substring(0, lower.Length - 1));
      }
      else
        this.Value = int.Parse(lower);
    }

    public int GetSize(int size)
    {
      return this.Value > 0 ? Math.Min(this.IsPercent ? size * this.Value / 100 : this.Value, size) : size;
    }

    public bool IsFixed => this.Value > 0;

    public bool IsAuto => this.Value == 0;

    public override bool Equals(object obj)
    {
      try
      {
        SizeValue sizeValue = (SizeValue) obj;
        return sizeValue.Value == this.Value && sizeValue.IsPercent == this.IsPercent;
      }
      catch
      {
        return false;
      }
    }

    public override int GetHashCode() => this.Value.GetHashCode() ^ this.IsPercent.GetHashCode();

    public static bool operator ==(SizeValue sv1, SizeValue sv2)
    {
      return object.Equals((object) sv1, (object) sv2);
    }

    public static bool operator !=(SizeValue sv1, SizeValue sv2) => !(sv1 == sv2);
  }
}
