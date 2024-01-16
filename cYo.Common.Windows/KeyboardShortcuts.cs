// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.KeyboardShortcuts
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows
{
  [Serializable]
  public class KeyboardShortcuts : ICloneable
  {
    private readonly List<KeyboardCommand> commands = new List<KeyboardCommand>();

    public KeyboardShortcuts()
    {
    }

    public KeyboardShortcuts(KeyboardShortcuts copy)
    {
      this.Commands.AddRange(copy.Commands.Select<KeyboardCommand, KeyboardCommand>((Func<KeyboardCommand, KeyboardCommand>) (kc => new KeyboardCommand(kc))));
    }

    public bool HandleKey(CommandKey key)
    {
      foreach (KeyboardCommand command in this.Commands)
      {
        if (command.Handles(key))
        {
          command.Invoke(key);
          return true;
        }
      }
      return false;
    }

    public bool HandleKey(CommandKey key, CommandKey modifiers) => this.HandleKey(key | modifiers);

    public bool HandleKey(CommandKey key, Keys modifiers)
    {
      return this.HandleKey(key, (CommandKey) modifiers);
    }

    public bool HandleKey(MouseButtons button, bool doubleClick, bool isTouch)
    {
      if (isTouch && this.HandleKey(doubleClick ? CommandKey.TouchDoubleTap : CommandKey.TouchTap, Control.ModifierKeys))
        return true;
      if ((button & MouseButtons.Left) != MouseButtons.None)
        return this.HandleKey(doubleClick ? CommandKey.MouseDoubleLeft : CommandKey.MouseLeft, Control.ModifierKeys);
      if ((button & MouseButtons.Right) != MouseButtons.None)
        return this.HandleKey(doubleClick ? CommandKey.MouseDoubleRight : CommandKey.MouseRight, Control.ModifierKeys);
      if ((button & MouseButtons.Middle) != MouseButtons.None)
        return this.HandleKey(doubleClick ? CommandKey.MouseDoubleMiddle : CommandKey.MouseMiddle, Control.ModifierKeys);
      if ((button & MouseButtons.XButton1) != MouseButtons.None)
        return this.HandleKey(doubleClick ? CommandKey.MouseDoubleButton4 : CommandKey.MouseButton4, Control.ModifierKeys);
      return (button & MouseButtons.XButton2) != MouseButtons.None && this.HandleKey(doubleClick ? CommandKey.MouseDoubleButton5 : CommandKey.MouseButton5, Control.ModifierKeys);
    }

    public bool HandleKey(Keys k)
    {
      return Enum.IsDefined(typeof (CommandKey), (object) (int) (k & Keys.KeyCode)) && this.HandleKey((CommandKey) k);
    }

    public KeyboardCommand FindCommandByKey(string key)
    {
      return this.commands.FirstOrDefault<KeyboardCommand>((Func<KeyboardCommand, bool>) (kc => kc.Id == key));
    }

    public void SetKeyMapping(IEnumerable<StringPair> list)
    {
      foreach (StringPair stringPair in list)
      {
        KeyboardCommand commandByKey = this.FindCommandByKey(stringPair.Key);
        if (commandByKey != null)
          commandByKey.KeyList = stringPair.Value;
      }
    }

    public IEnumerable<StringPair> GetKeyMapping()
    {
      return this.Commands.Select<KeyboardCommand, StringPair>((Func<KeyboardCommand, StringPair>) (kc => new StringPair(kc.Id, kc.KeyList)));
    }

    public List<KeyboardCommand> Commands => this.commands;

    public object Clone() => (object) new KeyboardShortcuts(this);
  }
}
