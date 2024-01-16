// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicReadingListContainer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [XmlRoot("ReadingList")]
  public class ComicReadingListContainer
  {
    private readonly ComicReadingList items = new ComicReadingList();
    private ComicBookMatcherCollection matchers = new ComicBookMatcherCollection();
    private static Type[] extraTypes;

    public ComicReadingListContainer() => this.MatcherMode = MatcherMode.And;

    public ComicReadingListContainer(ComicListItem list, bool withFilenames, bool alwaysList = false)
      : this()
    {
      ComicReadingListContainer readingListContainer = this;
      this.Name = list.Name;
      ComicSmartListItem comicSmartListItem = list as ComicSmartListItem;
      if (alwaysList || comicSmartListItem == null)
      {
        list.GetBooks().ForEach<ComicBook>((Action<ComicBook>) (b => readingListContainer.items.Add(new ComicReadingListItem(b, withFilenames))));
      }
      else
      {
        this.MatcherMode = comicSmartListItem.MatcherMode;
        comicSmartListItem.Matchers.ForEach((Action<ComicBookMatcher>) (m => this.Matchers.Add(m.Clone() as ComicBookMatcher)));
      }
    }

    public string Name { get; set; }

    [XmlArray("Books")]
    [XmlArrayItem("Book")]
    public ComicReadingList Items => this.items;

    [XmlAttribute]
    [DefaultValue(MatcherMode.And)]
    public MatcherMode MatcherMode { get; set; }

    public ComicBookMatcherCollection Matchers => this.matchers;

    public void Serialize(Stream outStream)
    {
      XmlUtility.GetSerializer<ComicReadingListContainer>().Serialize(outStream, (object) this);
    }

    public void Serialize(string file)
    {
      using (FileStream outStream = File.Create(file))
        this.Serialize((Stream) outStream);
    }

    public static ComicReadingListContainer Deserialize(Stream inStream)
    {
      return XmlUtility.GetSerializer<ComicReadingListContainer>().Deserialize(inStream) as ComicReadingListContainer;
    }

    public static ComicReadingListContainer Deserialize(string file)
    {
      using (FileStream inStream = File.OpenRead(file))
        return ComicReadingListContainer.Deserialize((Stream) inStream);
    }

    public static Type[] GetExtraXmlSerializationTypes()
    {
      if (ComicReadingListContainer.extraTypes == null)
      {
        List<Type> typeList = new List<Type>();
        typeList.AddRange(ComicBookValueMatcher.GetAvailableMatcherTypes());
        typeList.Add(typeof (ComicBookGroupMatcher));
        ComicReadingListContainer.extraTypes = typeList.ToArray();
      }
      return ComicReadingListContainer.extraTypes;
    }
  }
}
