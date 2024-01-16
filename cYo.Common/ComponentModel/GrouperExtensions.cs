// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.GrouperExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public static class GrouperExtensions
  {
    public static IGrouper<T> Append<T>(
      this IGrouper<T> itemGrouper,
      IGrouper<T> grouper,
      int max,
      bool removeIfContained = false)
    {
      if (itemGrouper == null)
        return grouper;
      List<IGrouper<T>> source = new List<IGrouper<T>>();
      if (!(itemGrouper is CompoundSingleGrouper<T> compoundSingleGrouper))
        source.Add(itemGrouper);
      else
        source.AddRange((IEnumerable<IGrouper<T>>) compoundSingleGrouper.Groupers);
      if (source.Contains(grouper))
        source.Remove(grouper);
      else
        source.Add(grouper);
      return source.Count != 0 ? (IGrouper<T>) new CompoundSingleGrouper<T>(source.Take<IGrouper<T>>(max).ToArray<IGrouper<T>>()) : (IGrouper<T>) null;
    }

    public static bool Contains<T>(this IGrouper<T> itemGrouper, IGrouper<T> grouper)
    {
      return !(itemGrouper is CompoundSingleGrouper<T> compoundSingleGrouper) ? itemGrouper == grouper : ((IEnumerable<IGrouper<T>>) compoundSingleGrouper.Groupers).Contains<IGrouper<T>>(grouper);
    }

    public static IEnumerable<IGrouper<T>> GetGroupers<T>(this IGrouper<T> itemGrouper)
    {
      if (itemGrouper == null)
        return Enumerable.Empty<IGrouper<T>>();
      if (itemGrouper is CompoundSingleGrouper<T> compoundSingleGrouper)
        return (IEnumerable<IGrouper<T>>) compoundSingleGrouper.Groupers;
      return ListExtensions.AsEnumerable<IGrouper<T>>(itemGrouper);
    }

    public static IGrouper<T> First<T>(this IGrouper<T> itemGrouper)
    {
      return itemGrouper.GetGroupers<T>().FirstOrDefault<IGrouper<T>>();
    }
  }
}
