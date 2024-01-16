// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.StacksConfig
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Windows.Forms;
using System;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class StacksConfig
  {
    private const int MaxConfigCount = 1024;
    private readonly StacksConfig.StackConfigItemCollection configs = new StacksConfig.StackConfigItemCollection();

    public StacksConfig.StackConfigItem FindItem(string stack)
    {
      return this.configs.Find((Predicate<StacksConfig.StackConfigItem>) (ti => ti.Stack == stack));
    }

    public bool IsTop(string stack, ComicBook cb)
    {
      StacksConfig.StackConfigItem stackConfigItem = this.FindItem(stack);
      return stackConfigItem != null && stackConfigItem.TopId == cb.Id;
    }

    public void SetStackTop(string stack, ComicBook cb)
    {
      StacksConfig.StackConfigItem stackConfigItem = this.FindItem(stack);
      if (stackConfigItem == null)
      {
        this.AddConfig(new StacksConfig.StackConfigItem(stack, cb.Id, (ItemViewConfig) null));
      }
      else
      {
        stackConfigItem.ThumbnailKey = (string) null;
        stackConfigItem.TopId = cb.Id;
      }
    }

    public void SetStackThumbnailKey(string stack, string key)
    {
      StacksConfig.StackConfigItem stackConfigItem = this.FindItem(stack);
      if (stackConfigItem == null)
        this.AddConfig(new StacksConfig.StackConfigItem(stack, key, (ItemViewConfig) null));
      else
        stackConfigItem.ThumbnailKey = key;
    }

    public Guid GetStackTopId(string stack)
    {
      StacksConfig.StackConfigItem stackConfigItem = this.FindItem(stack);
      return stackConfigItem != null ? stackConfigItem.TopId : Guid.Empty;
    }

    public string GetStackCustomThumbnail(string stack) => this.FindItem(stack)?.ThumbnailKey;

    public ItemViewConfig GetStackViewConfig(string stack) => this.FindItem(stack)?.Config;

    public void SetStackViewConfig(string stack, ItemViewConfig config)
    {
      StacksConfig.StackConfigItem stackConfigItem = this.FindItem(stack);
      if (stackConfigItem == null)
        this.AddConfig(new StacksConfig.StackConfigItem(stack, Guid.Empty, config));
      else
        stackConfigItem.Config = config;
    }

    private void AddConfig(StacksConfig.StackConfigItem sci)
    {
      this.configs.Insert(0, sci);
      this.configs.Trim(1024);
    }

    public StacksConfig.StackConfigItemCollection Configs => this.configs;

    [Serializable]
    public class StackConfigItem
    {
      private Guid topId = Guid.Empty;

      public StackConfigItem()
      {
      }

      public StackConfigItem(string stack, Guid id, ItemViewConfig config)
      {
        this.topId = id;
        this.Stack = stack;
        this.Config = config;
      }

      public StackConfigItem(string stack, string thumbnailKey, ItemViewConfig config)
      {
        this.ThumbnailKey = thumbnailKey;
        this.Stack = stack;
        this.Config = config;
      }

      public string Stack { get; set; }

      public Guid TopId
      {
        get => this.topId;
        set => this.topId = value;
      }

      public string ThumbnailKey { get; set; }

      public ItemViewConfig Config { get; set; }

      [XmlIgnore]
      public bool TopIdSpecified => this.topId != Guid.Empty;
    }

    [Serializable]
    public class StackConfigItemCollection : SmartList<StacksConfig.StackConfigItem>
    {
    }
  }
}
