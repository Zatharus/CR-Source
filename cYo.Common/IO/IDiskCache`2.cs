// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.IDiskCache`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.IO
{
  public interface IDiskCache<K, T> : IDisposable
  {
    T GetItem(K key);

    bool AddItem(K key, T item);

    void RemoveItem(K key);

    bool IsAvailable(K key);

    void UpdateKeys(Func<K, bool> select, Action<K> update);

    void RemoveKeys(Func<K, bool> select);

    K[] GetKeys();

    void CleanUp();

    void Clear();

    bool Enabled { get; set; }

    int CacheSizeMB { get; set; }

    long Size { get; }

    int Count { get; }

    event EventHandler SizeChanged;
  }
}
