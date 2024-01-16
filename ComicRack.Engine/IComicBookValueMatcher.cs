// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IComicBookValueMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public interface IComicBookValueMatcher : IComicBookMatcher, IMatcher<ComicBook>, ICloneable
  {
    string Description { get; }

    string DescriptionNeutral { get; }

    string MatchValue { get; set; }

    string MatchValue2 { get; set; }

    int MatchOperator { get; set; }

    string[] OperatorsListNeutral { get; }

    string[] OperatorsList { get; }

    int ArgumentCount { get; }

    string UnitDescription { get; }

    bool SwapOperatorArgument { get; }
  }
}
