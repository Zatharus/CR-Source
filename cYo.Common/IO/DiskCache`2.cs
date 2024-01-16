// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.DiskCache`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

#nullable disable
namespace cYo.Common.IO
{
  public abstract class DiskCache<K, T> : DisposableObject, IDiskCache<K, T>, IDisposable
  {
    private const string indexFile = "cache.idx";
    private readonly string cacheIndex;
    private readonly Dictionary<K, LinkedListNode<DiskCache<K, T>.CacheItem>> fileDict = new Dictionary<K, LinkedListNode<DiskCache<K, T>.CacheItem>>();
    private readonly LinkedList<DiskCache<K, T>.CacheItem> fileList = new LinkedList<DiskCache<K, T>.CacheItem>();
    private readonly LockFile lockFile;
    private Timer indexSaver;
    private readonly string cacheFolder;
    private volatile int cacheSizeMB = 50;
    private long size;
    private volatile bool enabled = true;
    private readonly Dictionary<K, ManualResetEvent> creationLocks = new Dictionary<K, ManualResetEvent>();

    protected DiskCache(string cacheFolder, int cacheSizeMB, int saveIndex = 10)
    {
      this.cacheFolder = cacheFolder;
      this.cacheSizeMB = cacheSizeMB;
      this.cacheIndex = Path.Combine(cacheFolder, "cache.idx");
      try
      {
        Directory.CreateDirectory(cacheFolder);
        this.lockFile = new LockFile(Path.Combine(cacheFolder, "cache.lock"));
        List<DiskCache<K, T>.CacheItem> cacheItemList = DiskCache<K, T>.LoadCacheIndex(this.cacheIndex);
        foreach (DiskCache<K, T>.CacheItem cacheItem in cacheItemList)
        {
          LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode = this.fileList.AddLast(cacheItem);
          try
          {
            this.fileDict.Add(cacheItem.Key, linkedListNode);
            this.size += cacheItem.Length;
          }
          catch (Exception ex)
          {
            FileUtility.SafeDelete(Path.Combine(cacheFolder, cacheItem.File));
            this.fileList.RemoveLast();
          }
        }
        if (cacheItemList.Count == 0)
          this.Clear();
        if (this.lockFile.WasLocked)
          ThreadUtility.CreateWorkerThread("Cache cleanup", new ThreadStart(this.BackgroundCleanUp), ThreadPriority.Lowest);
      }
      catch (Exception ex)
      {
        this.enabled = false;
      }
      if (saveIndex == 0)
        return;
      this.indexSaver = new Timer((TimerCallback) (obj =>
      {
        if (!this.CacheIndexDirty)
          return;
        this.SaveCacheIndex(this.cacheIndex);
      }), (object) null, 1000 * saveIndex, 1000 * saveIndex);
    }

    public string CacheFolder => this.cacheFolder;

    public int CacheSizeMB
    {
      get => this.cacheSizeMB;
      set
      {
        this.cacheSizeMB = value;
        this.CleanUp();
      }
    }

    public long Size => Interlocked.Read(ref this.size);

    public int Count
    {
      get
      {
        using (ItemMonitor.Lock((object) this.fileList))
          return this.fileList.Count;
      }
    }

    public bool Enabled
    {
      get => this.enabled;
      set => this.enabled = value;
    }

    public bool CacheIndexDirty { get; set; }

    protected virtual string CreateCacheFileName()
    {
      byte[] byteArray = Guid.NewGuid().ToByteArray();
      return byteArray[0].ToString() + Path.DirectorySeparatorChar.ToString() + Base32.ToBase32String(byteArray) + ".cache";
    }

    protected abstract T LoadItem(string file);

    protected abstract void StoreItem(string file, T item);

    protected override void Dispose(bool disposing)
    {
      this.indexSaver.SafeDispose();
      this.SaveCacheIndex(this.cacheIndex);
      if (this.lockFile != null)
        this.lockFile.Dispose();
      base.Dispose(disposing);
    }

    public event EventHandler SizeChanged;

    protected virtual void OnSizeChanged()
    {
      if (this.SizeChanged == null)
        return;
      this.SizeChanged((object) this, EventArgs.Empty);
    }

    private void IncSize(long add)
    {
      Interlocked.Add(ref this.size, add);
      this.OnSizeChanged();
    }

    private static List<DiskCache<K, T>.CacheItem> LoadCacheIndex(string cacheIndexFile)
    {
      try
      {
        using (Stream serializationStream = (Stream) File.OpenRead(cacheIndexFile))
          return (List<DiskCache<K, T>.CacheItem>) new BinaryFormatter()
          {
            Binder = ((SerializationBinder) new VersionNeutralBinder())
          }.Deserialize(serializationStream);
      }
      catch (Exception ex)
      {
        return new List<DiskCache<K, T>.CacheItem>();
      }
    }

    public void SaveCacheIndex(string cacheIndexFile)
    {
      lock (this)
      {
        try
        {
          this.CacheIndexDirty = false;
          BinaryFormatter binaryFormatter = new BinaryFormatter()
          {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded
          };
          List<DiskCache<K, T>.CacheItem> list;
          using (ItemMonitor.Lock((object) this.fileList))
            list = this.fileList.ToList<DiskCache<K, T>.CacheItem>();
          using (Stream serializationStream = (Stream) File.Create(cacheIndexFile))
            binaryFormatter.Serialize(serializationStream, (object) list);
        }
        catch (Exception ex)
        {
        }
      }
    }

    private LinkedListNode<DiskCache<K, T>.CacheItem> GetCacheItem(K key)
    {
      if (!this.Enabled)
        return (LinkedListNode<DiskCache<K, T>.CacheItem>) null;
      using (ItemMonitor.Lock((object) this.fileDict))
      {
        LinkedListNode<DiskCache<K, T>.CacheItem> node;
        if (this.fileDict.TryGetValue(key, out node) && !File.Exists(this.GetFullPath(node.Value)))
        {
          using (ItemMonitor.Lock((object) this.fileList))
          {
            try
            {
              this.fileList.Remove(node);
            }
            catch (InvalidOperationException ex)
            {
            }
          }
          this.fileDict.Remove(key);
          node = (LinkedListNode<DiskCache<K, T>.CacheItem>) null;
        }
        return node;
      }
    }

    private string GetFullPath(DiskCache<K, T>.CacheItem cacheItem)
    {
      return Path.Combine(this.CacheFolder, cacheItem.File);
    }

    private void BackgroundCleanUp()
    {
      ILookup<string, K> lookup;
      using (ItemMonitor.Lock((object) this.fileList))
        lookup = this.fileList.ToLookup<DiskCache<K, T>.CacheItem, string, K>(new Func<DiskCache<K, T>.CacheItem, string>(this.GetFullPath), (Func<DiskCache<K, T>.CacheItem, K>) (f => f.Key));
      foreach (string file in FileUtility.GetFiles(this.CacheFolder, SearchOption.AllDirectories, ".cache"))
      {
        if (lookup[file].DefaultIfEmpty<K>() == null)
          FileUtility.SafeDelete(file);
      }
    }

    public bool IsAvailable(K key) => this.Enabled && this.GetCacheItem(key) != null;

    public T GetItem(K key)
    {
      LinkedListNode<DiskCache<K, T>.CacheItem> cacheItem = this.GetCacheItem(key);
      T obj = default (T);
      if (cacheItem != null)
      {
        using (ItemMonitor.Lock((object) cacheItem))
        {
          try
          {
            obj = this.LoadItem(this.GetFullPath(cacheItem.Value));
            using (ItemMonitor.Lock((object) this.fileList))
            {
              this.fileList.Remove(cacheItem);
              this.fileList.AddFirst(cacheItem);
              this.CacheIndexDirty = true;
            }
          }
          catch (Exception ex)
          {
            this.RemoveItem(key);
          }
        }
      }
      return obj;
    }

    public bool AddItem(K key, T item)
    {
      if (!this.Enabled)
        return false;
      ManualResetEvent manualResetEvent1 = (ManualResetEvent) null;
      ManualResetEvent manualResetEvent2;
      using (ItemMonitor.Lock((object) this.creationLocks))
      {
        if (!this.creationLocks.TryGetValue(key, out manualResetEvent2))
          this.creationLocks.Add(key, manualResetEvent1 = new ManualResetEvent(false));
      }
      if (manualResetEvent2 != null)
      {
        try
        {
          manualResetEvent2.WaitOne();
        }
        catch
        {
        }
        return true;
      }
      try
      {
        if (this.GetCacheItem(key) != null)
          return false;
        DiskCache<K, T>.CacheItem cacheItem = new DiskCache<K, T>.CacheItem(key, this.CreateCacheFileName(), 0L);
        string fullPath = this.GetFullPath(cacheItem);
        string directoryName = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        this.StoreItem(fullPath, item);
        cacheItem.Length = new FileInfo(fullPath).Length;
        using (ItemMonitor.Lock((object) this.fileList))
        {
          using (ItemMonitor.Lock((object) this.fileDict))
            this.fileDict[key] = this.fileList.AddFirst(cacheItem);
        }
        this.IncSize(cacheItem.Length);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
      finally
      {
        using (ItemMonitor.Lock((object) this.creationLocks))
          this.creationLocks.Remove(key);
        if (manualResetEvent1 != null)
        {
          manualResetEvent1.Set();
          manualResetEvent1.Close();
        }
        this.CleanUp();
        this.CacheIndexDirty = true;
      }
    }

    public void RemoveItem(K key)
    {
      LinkedListNode<DiskCache<K, T>.CacheItem> cacheItem = this.GetCacheItem(key);
      if (cacheItem == null)
        return;
      using (ItemMonitor.Lock((object) cacheItem))
      {
        FileUtility.SafeDelete(this.GetFullPath(cacheItem.Value));
        using (ItemMonitor.Lock((object) this.fileList))
        {
          this.fileDict.Remove(key);
          try
          {
            this.fileList.Remove(cacheItem);
          }
          catch (Exception ex)
          {
          }
        }
      }
      this.IncSize(-cacheItem.Value.Length);
      this.CacheIndexDirty = true;
    }

    public void UpdateKeys(Func<K, bool> select, Action<K> update)
    {
      List<LinkedListNode<DiskCache<K, T>.CacheItem>> linkedListNodeList = new List<LinkedListNode<DiskCache<K, T>.CacheItem>>();
      using (ItemMonitor.Lock((object) this.fileList))
      {
        for (LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode = this.fileList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (select(linkedListNode.Value.Key))
            linkedListNodeList.Add(linkedListNode);
        }
      }
      using (ItemMonitor.Lock((object) this.fileDict))
      {
        foreach (LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode in linkedListNodeList)
        {
          K key = linkedListNode.Value.Key;
          this.fileDict.Remove(key);
          update(key);
          this.fileDict[key] = linkedListNode;
        }
      }
      this.CacheIndexDirty = true;
    }

    public void RemoveKeys(Func<K, bool> select)
    {
      List<K> kList = new List<K>();
      using (ItemMonitor.Lock((object) this.fileList))
      {
        for (LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode = this.fileList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (select(linkedListNode.Value.Key))
            kList.Add(linkedListNode.Value.Key);
        }
      }
      kList.ForEach(new Action<K>(this.RemoveItem));
    }

    public K[] GetKeys()
    {
      using (ItemMonitor.Lock((object) this.fileList))
        return this.fileList.Select<DiskCache<K, T>.CacheItem, K>((Func<DiskCache<K, T>.CacheItem, K>) (lln => lln.Key)).ToArray<K>();
    }

    public void CleanUp(long checkSize)
    {
      using (ItemMonitor.Lock((object) this.fileList))
      {
        LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode1 = this.fileList.Last;
        long size = this.Size;
        while (linkedListNode1 != null)
        {
          if (size > checkSize)
          {
            LinkedListNode<DiskCache<K, T>.CacheItem> linkedListNode2 = linkedListNode1;
            linkedListNode1 = linkedListNode2.Previous;
            try
            {
              FileUtility.SafeDelete(this.GetFullPath(linkedListNode2.Value));
              this.IncSize(-linkedListNode2.Value.Length);
              size -= linkedListNode2.Value.Length;
              this.fileList.RemoveLast();
              this.fileDict.Remove(linkedListNode2.Value.Key);
            }
            catch (Exception ex)
            {
            }
          }
          else
            break;
        }
      }
      this.CacheIndexDirty = true;
    }

    public void CleanUp() => this.CleanUp((long) this.cacheSizeMB * 1024L * 1024L);

    public void Clear()
    {
      using (ItemMonitor.Lock((object) this.fileList))
      {
        using (ItemMonitor.Lock((object) this.fileDict))
        {
          foreach (string directory in Directory.GetDirectories(this.CacheFolder))
          {
            try
            {
              Directory.Delete(directory, true);
            }
            catch (Exception ex)
            {
            }
          }
          this.fileList.Clear();
          this.fileDict.Clear();
          if (Interlocked.Exchange(ref this.size, 0L) != 0L)
            this.OnSizeChanged();
        }
      }
      this.CacheIndexDirty = true;
    }

    [Serializable]
    private class CacheItem
    {
      private long length;

      public CacheItem(K key, string file, long length)
      {
        if (Path.IsPathRooted(file))
          throw new ArgumentException("No rooted paths");
        this.Key = key;
        this.File = file;
        this.length = length;
      }

      public string File { get; set; }

      public K Key { get; set; }

      public long Length
      {
        get => Interlocked.Read(ref this.length);
        set => Interlocked.Exchange(ref this.length, value);
      }

      public string FileName => Path.GetFileName(this.File);
    }
  }
}
