// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.DeviceInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class DeviceInfo
  {
    public DeviceInfo(IDictionary<string, string> values)
    {
      this.Name = this.GetProperty(values, nameof (Name));
      this.Model = this.GetProperty(values, nameof (Model));
      this.Manufacturer = this.GetProperty(values, nameof (Manufacturer));
      this.SerialNumber = this.GetProperty(values, "Serial");
      this.Key = this.GetProperty(values, "ID");
      this.DeviceHash = this.GetProperty(values, "Hash");
      this.Version = int.Parse(values[nameof (Version)]);
      string property = this.GetProperty(values, nameof (Edition));
      string hexString = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(this.Model + this.Manufacturer + this.SerialNumber + property + (object) this.Version)).ToHexString(true);
      switch (property)
      {
        case "Android Free":
          this.Edition = SyncAppEdition.AndroidFree;
          break;
        case "Android Full":
          this.Edition = SyncAppEdition.AndroidFull;
          break;
        case "iOS":
          this.Edition = SyncAppEdition.iOS;
          break;
        default:
          this.Edition = SyncAppEdition.Unknown;
          break;
      }
      string[] strArray = this.GetProperty(values, "Screen").Split(',');
      this.ScreenPixelSize = new Size(int.Parse(strArray[0]), int.Parse(strArray[1]));
      this.ScreenDpi = new PointF(float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture));
      if ((this.GetProperty(values, "Capabilities") ?? string.Empty).Contains("WEBP"))
        this.Capabilites |= DeviceCapabilites.WebP;
      if (!string.IsNullOrEmpty(this.Model) && !string.IsNullOrEmpty(this.SerialNumber) && !string.IsNullOrEmpty(this.Key))
      {
        Size screenPixelSize = this.ScreenPixelSize;
        if (screenPixelSize.Width != 0)
        {
          screenPixelSize = this.ScreenPixelSize;
          if (screenPixelSize.Height != 0)
          {
            PointF screenDpi = this.ScreenDpi;
            if ((double) screenDpi.X > 0.10000000149011612)
            {
              screenDpi = this.ScreenDpi;
              if ((double) screenDpi.Y > 0.10000000149011612 && !(hexString != this.DeviceHash))
                return;
            }
          }
        }
      }
      throw new InvalidDataException();
    }

    public string Name { get; private set; }

    public string Model { get; private set; }

    public string SerialNumber { get; private set; }

    public string Key { get; private set; }

    public string Manufacturer { get; private set; }

    public SyncAppEdition Edition { get; private set; }

    public int Version { get; private set; }

    public string DeviceHash { get; private set; }

    public Size ScreenPixelSize { get; private set; }

    public PointF ScreenDpi { get; private set; }

    public DeviceCapabilites Capabilites { get; private set; }

    public int BookSyncLimit { get; set; }

    public SizeF ScreenPhysicalSize
    {
      get
      {
        Size screenPixelSize = this.ScreenPixelSize;
        double width = (double) screenPixelSize.Width / (double) this.ScreenDpi.X;
        screenPixelSize = this.ScreenPixelSize;
        double height = (double) screenPixelSize.Height / (double) this.ScreenDpi.Y;
        return new SizeF((float) width, (float) height);
      }
    }

    private string GetProperty(IDictionary<string, string> bag, string key)
    {
      string str;
      return !bag.TryGetValue(key, out str) ? (string) null : str;
    }
  }
}
