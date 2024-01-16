// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.IProgressState
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Threading
{
  public interface IProgressState
  {
    bool ProgressAvailable { get; set; }

    int ProgressPercentage { get; set; }

    string ProgressMessage { get; set; }

    bool Abort { get; set; }

    ProgressState State { get; }
  }
}
