// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.CompoundSingleGrouper`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class CompoundSingleGrouper<T> : IGrouper<T>
  {
    public CompoundSingleGrouper(IGrouper<T>[] groupers)
    {
      this.Groupers = groupers;
      if (((IEnumerable<IGrouper<T>>) this.Groupers).Any<IGrouper<T>>((Func<IGrouper<T>, bool>) (g => g.IsMultiGroup)))
        throw new ArgumentException("Only single groupers are supported");
    }

    public IGrouper<T>[] Groupers { get; private set; }

    public bool IsMultiGroup => false;

    public IGroupInfo GetGroup(T item)
    {
      return (IGroupInfo) new CompoundSingleGrouper<T>.CompoundGroupInfo(((IEnumerable<IGrouper<T>>) this.Groupers).Select<IGrouper<T>, IGroupInfo>((Func<IGrouper<T>, IGroupInfo>) (g => g.GetGroup(item))).ToArray<IGroupInfo>());
    }

    public IEnumerable<IGroupInfo> GetGroups(T item) => throw new NotImplementedException();

    private class CompoundGroupInfo : ICompoundGroupInfo, IGroupInfo, IComparable<IGroupInfo>
    {
      public CompoundGroupInfo(IGroupInfo[] infos)
      {
        this.Infos = infos;
        foreach (IGroupInfo info in infos)
        {
          if (this.Key == null)
          {
            this.Key = info.Key;
            this.Caption = info.Caption;
            this.Index = info.Index;
          }
          else
          {
            this.Key = (object) (this.Key.ToString() + "/" + info.Key);
            this.Caption = this.Caption + " - " + info.Caption;
          }
        }
      }

      public IGroupInfo[] Infos { get; private set; }

      public object Key { get; private set; }

      public string Caption { get; private set; }

      public int Index { get; private set; }

      public int CompareTo(IGroupInfo other)
      {
        if (!(other is CompoundSingleGrouper<T>.CompoundGroupInfo compoundGroupInfo))
          return GroupInfo.Compare((IGroupInfo) this, other);
        int length1 = this.Infos.Length;
        int length2 = compoundGroupInfo.Infos.Length;
        for (int index = 0; index < Math.Min(length1, length2); ++index)
        {
          int num = this.Infos[index].CompareTo(compoundGroupInfo.Infos[index]);
          if (num != 0)
            return num;
        }
        return Math.Sign(length1 - length2);
      }
    }
  }
}
