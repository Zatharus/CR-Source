// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.PropVariant
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [StructLayout(LayoutKind.Explicit, Size = 64)]
  public struct PropVariant
  {
    [FieldOffset(0)]
    public short vt;
    [FieldOffset(8)]
    public IntPtr pointerValue;
    [FieldOffset(8)]
    public byte byteValue;
    [FieldOffset(8)]
    public short shortValue;
    [FieldOffset(8)]
    public int intValue;
    [FieldOffset(8)]
    public long longValue;
    [FieldOffset(8)]
    public float floatValue;
    [FieldOffset(8)]
    public double doubleValue;
    [FieldOffset(8)]
    public System.Runtime.InteropServices.ComTypes.FILETIME filetime;

    [DllImport("ole32.dll")]
    private static extern int PropVariantClear(ref PropVariant pvar);

    public VarEnum VarType => (VarEnum) this.vt;

    public void Clear()
    {
      switch (this.VarType)
      {
        case VarEnum.VT_EMPTY:
          break;
        case VarEnum.VT_NULL:
        case VarEnum.VT_I2:
        case VarEnum.VT_I4:
        case VarEnum.VT_R4:
        case VarEnum.VT_R8:
        case VarEnum.VT_CY:
        case VarEnum.VT_DATE:
        case VarEnum.VT_ERROR:
        case VarEnum.VT_BOOL:
        case VarEnum.VT_I1:
        case VarEnum.VT_UI1:
        case VarEnum.VT_UI2:
        case VarEnum.VT_UI4:
        case VarEnum.VT_I8:
        case VarEnum.VT_UI8:
        case VarEnum.VT_INT:
        case VarEnum.VT_UINT:
        case VarEnum.VT_HRESULT:
        case VarEnum.VT_FILETIME:
          this.vt = (short) 0;
          break;
        case VarEnum.VT_BSTR:
          Marshal.FreeBSTR(this.pointerValue);
          this.vt = (short) 0;
          break;
        default:
          PropVariant.PropVariantClear(ref this);
          break;
      }
    }

    public object GetObject()
    {
      switch (this.VarType)
      {
        case VarEnum.VT_EMPTY:
          return (object) null;
        case VarEnum.VT_FILETIME:
          return (object) DateTime.FromFileTime(this.longValue);
        default:
          GCHandle gcHandle = GCHandle.Alloc((object) this, GCHandleType.Pinned);
          try
          {
            return Marshal.GetObjectForNativeVariant(gcHandle.AddrOfPinnedObject());
          }
          finally
          {
            gcHandle.Free();
          }
      }
    }

    public void SetObject(object value)
    {
      if (value == null)
      {
        this.vt = (short) 0;
      }
      else
      {
        switch (Type.GetTypeCode(value.GetType()))
        {
          case TypeCode.DBNull:
            this.vt = (short) 1;
            break;
          case TypeCode.Boolean:
            this.shortValue = Convert.ToInt16(value);
            this.vt = (short) 11;
            break;
          case TypeCode.SByte:
            this.byteValue = (byte) value;
            this.vt = (short) 16;
            break;
          case TypeCode.Byte:
            this.byteValue = (byte) value;
            this.vt = (short) 17;
            break;
          case TypeCode.Int16:
            this.shortValue = (short) value;
            this.vt = (short) 2;
            break;
          case TypeCode.UInt16:
            this.shortValue = (short) value;
            this.vt = (short) 18;
            break;
          case TypeCode.Int32:
            this.intValue = (int) value;
            this.vt = (short) 3;
            break;
          case TypeCode.UInt32:
            this.intValue = (int) value;
            this.vt = (short) 19;
            break;
          case TypeCode.Int64:
            this.longValue = (long) value;
            this.vt = (short) 20;
            break;
          case TypeCode.UInt64:
            this.longValue = (long) value;
            this.vt = (short) 21;
            break;
          case TypeCode.Single:
            this.floatValue = (float) value;
            this.vt = (short) 4;
            break;
          case TypeCode.Double:
            this.doubleValue = (double) value;
            this.vt = (short) 5;
            break;
          case TypeCode.String:
            this.pointerValue = Marshal.StringToBSTR((string) value);
            this.vt = (short) 8;
            break;
        }
      }
    }
  }
}
