// Decompiled with JetBrains decompiler
// Type: cYo.Common.Localize.TREntryList
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.Localize
{
  public class TREntryList : SmartList<TREntry>
  {
    private readonly Dictionary<string, TREntry> rs = new Dictionary<string, TREntry>();
    private readonly TR owner;

    public TREntryList(TR owner) => this.owner = owner;

    protected override void OnInsertCompleted(int index, TREntry item)
    {
      this.rs[item.Key] = item;
      item.Resource = this.owner;
      base.OnInsertCompleted(index, item);
    }

    protected override void OnRemoveCompleted(int index, TREntry item)
    {
      this.rs.Remove(item.Key);
      item.Resource = (TR) null;
      base.OnRemoveCompleted(index, item);
    }

    public string GetText(string key, string value = null)
    {
      if (string.IsNullOrEmpty(key))
        return value;
      using (ItemMonitor.Lock(this.SyncRoot))
      {
        TREntry trEntry;
        return this.rs.TryGetValue(key, out trEntry) && !string.IsNullOrEmpty(trEntry.Text) ? trEntry.Text : value;
      }
    }

    public void Merge(IEnumerable<TREntry> list)
    {
      using (ItemMonitor.Lock(this.SyncRoot))
      {
        foreach (TREntry trEntry1 in list)
        {
          TREntry trEntry2 = this[trEntry1.Key];
          if (trEntry2 != null)
            this.Remove(trEntry2);
          this.Add(trEntry1);
        }
      }
    }

    public void Update(TREntryList list)
    {
      foreach (string key in this.Keys.ToArray<string>())
      {
        if (list[key] == null)
          this.Remove(this[key]);
      }
      foreach (TREntry trEntry1 in (SmartList<TREntry>) list)
      {
        TREntry trEntry2 = this[trEntry1.Key];
        if (trEntry2 == null)
          this.Add(new TREntry(trEntry1.Key, trEntry1.Comment, trEntry1.Comment));
        else if (trEntry2.Comment != trEntry1.Comment)
        {
          this.Remove(trEntry2);
          this.Add(new TREntry(trEntry1.Key, trEntry2.Text, trEntry1.Comment));
        }
      }
    }

    public IEnumerable<string> Keys => this.rs.Keys.Lock<string>();

    public TREntry this[string key]
    {
      get
      {
        TREntry trEntry;
        return !this.rs.TryGetValue(key, out trEntry) ? (TREntry) null : trEntry;
      }
    }
  }
}
