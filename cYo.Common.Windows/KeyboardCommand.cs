// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.KeyboardCommand
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Localize;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Common.Windows
{
  [Serializable]
  public class KeyboardCommand
  {
    public const int NumberOfKeys = 4;
    private readonly CommandKey[] keyboard = new CommandKey[4];
    [NonSerialized]
    private Action method;
    [NonSerialized]
    private Action<CommandKey> methodWithKey;

    protected KeyboardCommand(
      Image image,
      string id,
      string group,
      string text,
      params CommandKey[] keys)
    {
      this.Image = image;
      this.Id = id;
      this.Group = TR.Load(nameof (Keyboard))[group, group];
      this.Text = TR.Load(nameof (Keyboard))[id, text];
      for (int index = 0; index < Math.Min(this.Keyboard.Length, keys.Length); ++index)
        this.keyboard[index] = keys[index];
    }

    public KeyboardCommand(
      Image image,
      string id,
      string group,
      string text,
      Action<CommandKey> method,
      params CommandKey[] keys)
      : this(image, id, group, text, keys)
    {
      this.methodWithKey = method;
    }

    public KeyboardCommand(
      Image image,
      string id,
      string group,
      string text,
      Action method,
      params CommandKey[] keys)
      : this(image, id, group, text, keys)
    {
      this.method = method;
    }

    public KeyboardCommand(
      string id,
      string group,
      string text,
      Action method,
      params CommandKey[] keys)
      : this((Image) null, id, group, text, method, keys)
    {
    }

    public KeyboardCommand(KeyboardCommand copy)
    {
      this.Image = copy.Image;
      this.Id = copy.Id;
      this.Group = copy.Group;
      this.Text = copy.Text;
      this.method = copy.method;
      this.methodWithKey = copy.methodWithKey;
      for (int index = 0; index < copy.Keyboard.Length; ++index)
        this.Keyboard[index] = copy.Keyboard[index];
    }

    public string Group { get; private set; }

    public Image Image { get; private set; }

    public string Id { get; private set; }

    public string Text { get; private set; }

    public CommandKey[] Keyboard => this.keyboard;

    public string KeyList
    {
      get => this.Keyboard.ToListString("|");
      set
      {
        if (value == null)
          return;
        int num = 0;
        foreach (string str in value.Split("|".ToCharArray(), 4))
        {
          CommandKey commandKey = CommandKey.None;
          try
          {
            commandKey = (CommandKey) Enum.Parse(typeof (CommandKey), str);
          }
          catch
          {
          }
          this.keyboard[num++] = commandKey;
        }
      }
    }

    public string KeyNames
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (CommandKey key in this.Keyboard)
        {
          if (key != CommandKey.None)
          {
            if (stringBuilder.Length != 0)
            {
              stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
              stringBuilder.Append(" ");
            }
            stringBuilder.Append(KeyboardCommand.GetKeyName(key));
          }
        }
        return stringBuilder.ToString();
      }
    }

    public void Invoke(CommandKey key)
    {
      if (this.method != null)
        this.method();
      if (this.methodWithKey == null)
        return;
      this.methodWithKey(key);
    }

    public bool Handles(CommandKey key)
    {
      return ((IEnumerable<CommandKey>) this.keyboard).Any<CommandKey>((Func<CommandKey, bool>) (k => k == key));
    }

    private static string GetEnglishName(CommandKey key)
    {
      switch (key)
      {
        case CommandKey.D0:
          return "0";
        case CommandKey.D1:
          return "1";
        case CommandKey.D2:
          return "2";
        case CommandKey.D3:
          return "3";
        case CommandKey.D4:
          return "4";
        case CommandKey.D5:
          return "5";
        case CommandKey.D6:
          return "6";
        case CommandKey.D7:
          return "7";
        case CommandKey.D8:
          return "8";
        case CommandKey.D9:
          return "9";
        default:
          return key.ToString().PascalToSpaced();
      }
    }

    public static string GetKeyName(CommandKey key)
    {
      CommandKey key1 = key & ~CommandKey.Modifiers;
      TR tr = TR.Load("CommandKeys");
      StringBuilder stringBuilder = new StringBuilder();
      if (key1 != CommandKey.None)
      {
        if ((key & CommandKey.Ctrl) != CommandKey.None)
        {
          stringBuilder.Append(tr["Ctrl", "Ctrl"]);
          stringBuilder.Append("+");
        }
        if ((key & CommandKey.Shift) != CommandKey.None)
        {
          stringBuilder.Append(tr["Shift", "Shift"]);
          stringBuilder.Append("+");
        }
        if ((key & CommandKey.Alt) != CommandKey.None)
        {
          stringBuilder.Append(tr["Alt", "Alt"]);
          stringBuilder.Append("+");
        }
      }
      stringBuilder.Append(tr[key1.ToString(), KeyboardCommand.GetEnglishName(key1)]);
      return stringBuilder.ToString();
    }

    public static bool IsKeyValue(CommandKey key)
    {
      return (key & CommandKey.Modifiers) == CommandKey.None;
    }
  }
}
