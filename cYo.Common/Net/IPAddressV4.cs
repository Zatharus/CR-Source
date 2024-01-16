// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.IPAddressV4
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace cYo.Common.Net
{
  public struct IPAddressV4 : IComparable<IPAddressV4>
  {
    public static readonly IPAddressV4 Empty = new IPAddressV4(0, 0, 0, 0);
    public static readonly IPAddressV4 Loopback = new IPAddressV4((int) sbyte.MaxValue, 0, 0, 1);
    public static readonly IPAddressV4 PrivateStartA = new IPAddressV4(10, 0, 0, 0);
    public static readonly IPAddressV4 PrivateEndA = new IPAddressV4(10, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
    public static readonly IPAddressV4 PrivateStartB = new IPAddressV4(172, 16, 0, 0);
    public static readonly IPAddressV4 PrivateEndB = new IPAddressV4(172, 31, (int) byte.MaxValue, (int) byte.MaxValue);
    public static readonly IPAddressV4 PrivateStartC = new IPAddressV4(192, 168, 0, 0);
    public static readonly IPAddressV4 PrivateEndC = new IPAddressV4(192, 168, (int) byte.MaxValue, (int) byte.MaxValue);
    public static readonly IPAddressV4 PrivateStartD = new IPAddressV4(169, 254, 0, 0);
    public static readonly IPAddressV4 PrivateEndD = new IPAddressV4(169, 254, (int) byte.MaxValue, (int) byte.MaxValue);

    public IPAddressV4(int a, int b, int c, int d)
      : this()
    {
      this.A = (byte) a;
      this.B = (byte) b;
      this.C = (byte) c;
      this.D = (byte) d;
    }

    public IPAddressV4(byte[] bytes)
      : this((int) bytes[0], (int) bytes[1], (int) bytes[2], (int) bytes[3])
    {
    }

    public IPAddressV4(string address)
      : this()
    {
      IPAddressV4 address1;
      if (!IPAddressV4.TryParse(address, out address1))
        throw new ArgumentException("no valid ip v4 address");
      this.A = address1.A;
      this.B = address1.B;
      this.C = address1.C;
      this.D = address1.D;
    }

    public byte A { get; private set; }

    public byte B { get; private set; }

    public byte C { get; private set; }

    public byte D { get; private set; }

    public byte[] GetAddressBytes()
    {
      return new byte[4]{ this.A, this.B, this.C, this.D };
    }

    public long GetLongAddress()
    {
      return ((long) this.A << 24) + ((long) this.B << 16) + ((long) this.C << 8) + (long) this.D;
    }

    public int GetAddress() => (int) this.GetLongAddress();

    public bool InRange(IPAddressV4 a, IPAddressV4 b) => this >= a && this <= b;

    public bool IsPrivate()
    {
      return this.InRange(IPAddressV4.PrivateStartA, IPAddressV4.PrivateEndA) || this.InRange(IPAddressV4.PrivateStartB, IPAddressV4.PrivateEndB) || this.InRange(IPAddressV4.PrivateStartC, IPAddressV4.PrivateEndC) || this.InRange(IPAddressV4.PrivateStartD, IPAddressV4.PrivateEndD) || this == IPAddressV4.Loopback;
    }

    public int CompareTo(IPAddressV4 other)
    {
      return Math.Sign(this.GetLongAddress() - other.GetLongAddress());
    }

    public override bool Equals(object obj)
    {
      try
      {
        return this.GetAddress() == ((IPAddressV4) obj).GetAddress();
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}.{3}", (object) this.A, (object) this.B, (object) this.C, (object) this.D);
    }

    public override int GetHashCode() => this.GetAddress();

    public static bool TryParse(string text, out IPAddressV4 address)
    {
      address = IPAddressV4.Empty;
      IPAddress address1;
      if (!IPAddress.TryParse(text, out address1))
        return false;
      try
      {
        address = (IPAddressV4) address1;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static explicit operator IPAddressV4(IPAddress address)
    {
      return address.AddressFamily == AddressFamily.InterNetwork ? new IPAddressV4(address.GetAddressBytes()) : throw new InvalidCastException("Can only cast ip v4 addresses");
    }

    public static implicit operator IPAddress(IPAddressV4 address)
    {
      return new IPAddress(address.GetAddressBytes());
    }

    public static bool operator ==(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) == 0;

    public static bool operator !=(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) != 0;

    public static bool operator >(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) > 0;

    public static bool operator <(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) < 0;

    public static bool operator >=(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) >= 0;

    public static bool operator <=(IPAddressV4 a, IPAddressV4 b) => a.CompareTo(b) <= 0;
  }
}
