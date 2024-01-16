// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeySequence
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [TypeConverter(typeof (KeySequence.KeySequenceConverter))]
  [Serializable]
  public class KeySequence
  {
    private readonly List<Keys> sequence = new List<Keys>();

    public KeySequence(string name, IEnumerable<Keys> sequence)
    {
      this.Name = name;
      this.sequence = new List<Keys>(sequence);
    }

    public KeySequence(string name, params Keys[] keys)
      : this(name, (IEnumerable<Keys>) keys)
    {
    }

    public KeySequence()
      : this((string) null, (Keys[]) null)
    {
    }

    public string Name { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<Keys> Sequence => this.sequence;

    public override string ToString() => this.Name;

    internal class KeySequenceConverter : TypeConverter
    {
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        return destinationType == typeof (InstanceDescriptor) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertTo(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value,
        Type destinationType)
      {
        if (value is KeySequence keySequence && destinationType == typeof (InstanceDescriptor))
        {
          ConstructorInfo constructor = typeof (KeySequence).GetConstructor(new Type[2]
          {
            typeof (string),
            typeof (IEnumerable<Keys>)
          });
          if (constructor != (ConstructorInfo) null)
            return (object) new InstanceDescriptor((MemberInfo) constructor, (ICollection) new object[2]
            {
              (object) keySequence.Name,
              (object) keySequence.Sequence
            });
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
  }
}
