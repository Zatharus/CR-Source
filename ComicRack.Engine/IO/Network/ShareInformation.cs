// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ShareInformation
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  public class ShareInformation
  {
    public string Id { get; set; }

    public string Uri { get; set; }

    public string Name { get; set; }

    public string Comment { get; set; }

    public ServerOptions Options { get; set; }

    public bool IsLocal { get; set; }

    public bool IsProtected => (this.Options & ServerOptions.ShareNeedsPassword) != 0;

    public bool IsEditable => (this.Options & ServerOptions.ShareIsEditable) != 0;

    public bool IsExportable => (this.Options & ServerOptions.ShareIsExportable) != 0;

    public static implicit operator ServerInfo(ShareInformation info)
    {
      return new ServerInfo()
      {
        Name = info.Name,
        Comment = info.Comment,
        Uri = info.Uri,
        Options = (int) info.Options
      };
    }

    public static implicit operator ShareInformation(ServerInfo info)
    {
      return new ShareInformation()
      {
        Name = info.Name,
        Comment = info.Comment,
        Uri = info.Uri,
        Options = (ServerOptions) info.Options
      };
    }
  }
}
