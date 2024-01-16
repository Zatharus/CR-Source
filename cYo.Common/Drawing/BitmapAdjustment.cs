// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.BitmapAdjustment
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Mathematics;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Drawing
{
  [TypeConverter(typeof (BitmapAdjustmentConverter))]
  [Serializable]
  public struct BitmapAdjustment
  {
    public static readonly BitmapAdjustment Empty = new BitmapAdjustment(0.0f);

    public BitmapAdjustment(
      float saturation,
      float brightness,
      float contrast,
      float gamma,
      Color whitePoint,
      BitmapAdjustmentOptions options = BitmapAdjustmentOptions.None,
      int sharpen = 0)
      : this()
    {
      this.Brightness = brightness;
      this.Contrast = contrast;
      this.Saturation = saturation;
      this.Gamma = gamma;
      this.WhitePointColor = whitePoint;
      this.Options = options;
      this.Sharpen = sharpen;
    }

    public BitmapAdjustment(
      float saturation,
      float brightness,
      float contrast,
      float gamma,
      BitmapAdjustmentOptions options,
      int sharpen)
      : this(saturation, brightness, contrast, gamma, Color.White, options, sharpen)
    {
    }

    public BitmapAdjustment(Color whitePoint)
      : this(0.0f, 0.0f, 0.0f, 0.0f, whitePoint)
    {
    }

    public BitmapAdjustment(float saturation, float brightness = 0.0f, float contrast = 0.0f, float gamma = 0.0f)
      : this(saturation, brightness, contrast, gamma, Color.White)
    {
    }

    [DefaultValue(0.0f)]
    public float Saturation { get; set; }

    [DefaultValue(0.0f)]
    public float Contrast { get; set; }

    [DefaultValue(0.0f)]
    public float Brightness { get; set; }

    [DefaultValue(0.0f)]
    public float Gamma { get; set; }

    [XmlIgnore]
    public Color WhitePointColor { get; set; }

    [DefaultValue(0)]
    public int WhitePointArgb
    {
      get
      {
        int argb = this.WhitePointColor.ToArgb();
        switch (argb)
        {
          case -1:
          case 0:
            return 0;
          default:
            return argb;
        }
      }
      set => this.WhitePointColor = Color.FromArgb(value);
    }

    [DefaultValue(BitmapAdjustmentOptions.None)]
    public BitmapAdjustmentOptions Options { get; set; }

    [DefaultValue(0)]
    public int Sharpen { get; set; }

    public bool HasColorTransformations
    {
      get
      {
        return !BitmapAdjustment.EpsTest(this.Contrast, 0.0f) || !BitmapAdjustment.EpsTest(this.Saturation, 0.0f) || !BitmapAdjustment.EpsTest(this.Brightness, 0.0f) || !this.WhitePointColor.IsBlackOrWhite();
      }
    }

    public bool HasAutoContrast => (this.Options & BitmapAdjustmentOptions.AutoContrast) != 0;

    public bool HasSharpening => this.Sharpen != 0;

    public bool HasGamma => !BitmapAdjustment.EpsTest(this.Gamma, 0.0f);

    public bool IsEmpty
    {
      get
      {
        return !this.HasColorTransformations && this.Options == BitmapAdjustmentOptions.None && this.Sharpen == 0;
      }
    }

    public BitmapAdjustment ChangeSaturation(float saturation)
    {
      this.Saturation = saturation;
      return this;
    }

    public BitmapAdjustment ChangeBrightness(float brightness)
    {
      this.Brightness = brightness;
      return this;
    }

    public BitmapAdjustment ChangeContrast(float contrast)
    {
      this.Contrast = contrast;
      return this;
    }

    public BitmapAdjustment ChangeGamma(float gamma)
    {
      this.Gamma = gamma;
      return this;
    }

    public BitmapAdjustment ChangeWhitepoint(Color whitePoint)
    {
      this.WhitePointColor = whitePoint;
      return this;
    }

    public BitmapAdjustment ChangeOption(BitmapAdjustmentOptions options)
    {
      this.Options = options;
      return this;
    }

    public BitmapAdjustment ChangeSharpness(int sharpness)
    {
      this.Sharpen = sharpness;
      return this;
    }

    public override bool Equals(object compare)
    {
      return compare is BitmapAdjustment bitmapAdjustment && (double) this.Saturation == (double) bitmapAdjustment.Saturation && (double) this.Brightness == (double) bitmapAdjustment.Brightness && (double) this.Contrast == (double) bitmapAdjustment.Contrast && (double) this.Gamma == (double) bitmapAdjustment.Gamma && this.WhitePointArgb == bitmapAdjustment.WhitePointArgb && this.Options == bitmapAdjustment.Options && this.Sharpen == bitmapAdjustment.Sharpen;
    }

    public override int GetHashCode()
    {
      float num1 = this.Saturation;
      int hashCode1 = num1.GetHashCode();
      num1 = this.Contrast * 10f;
      int hashCode2 = num1.GetHashCode();
      int num2 = hashCode1 ^ hashCode2;
      num1 = this.Brightness * 100f;
      int hashCode3 = num1.GetHashCode();
      int num3 = num2 ^ hashCode3;
      num1 = this.Gamma * 10000f;
      int hashCode4 = num1.GetHashCode();
      return num3 ^ hashCode4 ^ this.WhitePointArgb.GetHashCode() ^ this.Options.GetHashCode() ^ this.Sharpen.GetHashCode() << 4;
    }

    public override string ToString()
    {
      return TypeDescriptor.GetConverter((object) this).ConvertToString((object) this);
    }

    public static bool operator ==(BitmapAdjustment a, BitmapAdjustment b)
    {
      return object.Equals((object) a, (object) b);
    }

    public static bool operator !=(BitmapAdjustment a, BitmapAdjustment b) => !(a == b);

    private static bool EpsTest(float f, float t) => f.CompareTo(t, 0.01f);

    public static BitmapAdjustment Add(BitmapAdjustment c1, BitmapAdjustment c2)
    {
      Color whitePoint = c1.WhitePointColor.IsBlackOrWhite() ? c2.WhitePointColor : c1.WhitePointColor;
      return new BitmapAdjustment(c1.Saturation + c2.Saturation, c1.Brightness + c2.Brightness, c1.Contrast + c2.Contrast, c1.Gamma + c2.Gamma, whitePoint, c1.Options | c2.Options, Math.Max(c1.Sharpen, c2.Sharpen));
    }

    public static BitmapAdjustment Parse(string text)
    {
      return (BitmapAdjustment) TypeDescriptor.GetConverter(typeof (BitmapAdjustment)).ConvertFromString(text);
    }
  }
}
