// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.DeviceSyncFactory
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using cYo.Common.Win32.PortableDevices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public static class DeviceSyncFactory
  {
    private static List<IPAddress> extraWifiDeviceAddresses = new List<IPAddress>();

    public static IEnumerable<ISyncProvider> Discover()
    {
      Dictionary<string, ISyncProvider> dictionary = new Dictionary<string, ISyncProvider>();
      try
      {
        foreach (Device device in DeviceFactory.GetDevices())
        {
          try
          {
            PortableDeviceSyncProvider deviceSyncProvider = new PortableDeviceSyncProvider(device.Key);
            dictionary[deviceSyncProvider.Device.Key] = (ISyncProvider) deviceSyncProvider;
          }
          catch (Exception ex)
          {
          }
        }
      }
      catch (Exception ex)
      {
      }
      foreach (string removeableDrive in DeviceSyncFactory.GetRemoveableDrives())
      {
        try
        {
          DiskDriveSyncProvider driveSyncProvider = new DiskDriveSyncProvider(removeableDrive);
          dictionary[driveSyncProvider.Device.Key] = (ISyncProvider) driveSyncProvider;
        }
        catch (Exception ex)
        {
        }
      }
      foreach (ISyncProvider syncProvider in DeviceSyncFactory.Discover(WirelessSyncProvider.GetWirelessDevices().Concat<IPAddress>(DeviceSyncFactory.ExtraWifiDeviceAddresses)))
        dictionary[syncProvider.Device.Key] = syncProvider;
      return (IEnumerable<ISyncProvider>) dictionary.Values;
    }

    public static IEnumerable<ISyncProvider> Discover(IEnumerable<IPAddress> adresses)
    {
      Dictionary<string, ISyncProvider> dictionary = new Dictionary<string, ISyncProvider>();
      foreach (IPAddress address in adresses.Distinct<IPAddress>())
      {
        try
        {
          WirelessSyncProvider wirelessSyncProvider = new WirelessSyncProvider(address);
          dictionary[wirelessSyncProvider.Device.Key] = (ISyncProvider) wirelessSyncProvider;
        }
        catch (Exception ex)
        {
        }
      }
      return (IEnumerable<ISyncProvider>) dictionary.Values;
    }

    public static ISyncProvider Create(string deviceKey)
    {
      try
      {
        foreach (Device device in DeviceFactory.GetDevices())
        {
          try
          {
            return (ISyncProvider) new PortableDeviceSyncProvider(device.Key, deviceKey);
          }
          catch (Exception ex)
          {
          }
        }
      }
      catch (Exception ex)
      {
      }
      foreach (string removeableDrive in DeviceSyncFactory.GetRemoveableDrives())
      {
        try
        {
          return (ISyncProvider) new DiskDriveSyncProvider(removeableDrive, deviceKey);
        }
        catch (Exception ex)
        {
        }
      }
      foreach (IPAddress address in WirelessSyncProvider.GetWirelessDevices().Concat<IPAddress>(DeviceSyncFactory.ExtraWifiDeviceAddresses))
      {
        try
        {
          return (ISyncProvider) new WirelessSyncProvider(address, deviceKey);
        }
        catch (Exception ex)
        {
        }
      }
      return (ISyncProvider) null;
    }

    private static IEnumerable<string> GetRemoveableDrives()
    {
      return DiskDriveSyncProvider.GetRemoveableDrives().Select<DriveInfo, string>((Func<DriveInfo, string>) (d => d.RootDirectory.FullName));
    }

    public static IEnumerable<IPAddress> ExtraWifiDeviceAddresses
    {
      get => (IEnumerable<IPAddress>) DeviceSyncFactory.extraWifiDeviceAddresses;
    }

    public static void SetExtraWifiDeviceAddresses(string addresses)
    {
      DeviceSyncFactory.extraWifiDeviceAddresses = new List<IPAddress>(DeviceSyncFactory.ParseWifiAddressList(addresses));
    }

    public static IEnumerable<IPAddress> ParseWifiAddressList(string addresses)
    {
      string str = addresses;
      char[] chArray = new char[2]{ ',', ';' };
      foreach (string ipString in ((IEnumerable<string>) str.Split(chArray)).TrimStrings().RemoveEmpty().Distinct<string>())
      {
        IPAddress address;
        if (IPAddress.TryParse(ipString, out address))
          yield return address;
      }
    }
  }
}
