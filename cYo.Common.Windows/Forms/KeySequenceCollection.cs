// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeySequenceCollection
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Serializable]
  public class KeySequenceCollection : List<KeySequence>
  {
    public KeySequence Add(string name, params Keys[] keys)
    {
      KeySequence keySequence = new KeySequence(name, keys);
      this.Add(keySequence);
      return keySequence;
    }
  }
}
