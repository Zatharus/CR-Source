// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.IRemoteComicLibrary
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.ServiceModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  [ServiceContract]
  public interface IRemoteComicLibrary
  {
    bool IsValid { [OperationContract] get; }

    [OperationContract]
    byte[] GetLibraryData();

    [OperationContract]
    int GetImageCount(Guid comicGuid);

    [OperationContract]
    byte[] GetImage(Guid comicGuid, int index);

    [OperationContract]
    byte[] GetThumbnailImage(Guid comicGuid, int index);

    [OperationContract]
    [ServiceKnownType(typeof (BitmapAdjustment))]
    void UpdateComic(Guid comicGuid, string propertyName, object value);
  }
}
