// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.CommandLineParser
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Common.Runtime
{
  public static class CommandLineParser
  {
    public static IEnumerable<string> Parse(
      object switches,
      IEnumerable<string> args,
      CommandLineParserOptions options = CommandLineParserOptions.UseIni)
    {
      try
      {
        if ((options & CommandLineParserOptions.UseIni) != CommandLineParserOptions.None)
          IniFile.UpdateProperties(switches, true);
      }
      catch (Exception ex)
      {
      }
      List<string> unnamed = new List<string>();
      IEnumerator<string> enumerator = args.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        if (current.StartsWith("-"))
        {
          try
          {
            CommandLineParser.ParseSwitch(switches, current.Substring(1), enumerator);
          }
          catch
          {
            if ((options & CommandLineParserOptions.FailOnError) != CommandLineParserOptions.None)
              throw;
          }
        }
        else
          unnamed.Add(current);
      }
      if (switches != null)
        CommandLineParser.SetFiles(switches, (IEnumerable<string>) unnamed);
      return (IEnumerable<string>) unnamed;
    }

    public static IEnumerable<string> Parse(object switches, CommandLineParserOptions options)
    {
      return CommandLineParser.Parse(switches, ((IEnumerable<string>) Environment.GetCommandLineArgs()).Skip<string>(1), options);
    }

    public static IEnumerable<string> Parse(object switches)
    {
      return CommandLineParser.Parse(switches, ((IEnumerable<string>) Environment.GetCommandLineArgs()).Skip<string>(1));
    }

    public static T Parse<T>(CommandLineParserOptions options)
    {
      T instance = Activator.CreateInstance<T>();
      try
      {
        CommandLineParser.Parse((object) instance, options);
      }
      catch (Exception ex)
      {
      }
      return instance;
    }

    public static T Parse<T>() => CommandLineParser.Parse<T>(CommandLineParserOptions.UseIni);

    private static void ParseSwitch(object switches, string name, IEnumerator<string> args)
    {
      if (switches == null)
        return;
      PropertyInfo property = (((IEnumerable<PropertyInfo>) switches.GetType().GetProperties()).Select(p => new
      {
        Attr = p.GetAttribute<CommandLineSwitchAttribute>(),
        Property = p
      }).Where(pinfo => pinfo.Attr != null).FirstOrDefault(pinfo => string.Equals(name, pinfo.Attr.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(name, pinfo.Attr.ShortName, StringComparison.OrdinalIgnoreCase)) ?? throw new ArgumentException("is not a valid command line switch", name)).Property;
      try
      {
        if (property.PropertyType == typeof (Action))
        {
          ((Action) property.GetValue(switches, (object[]) null))();
        }
        else
        {
          if (!property.CanWrite)
            throw new InvalidOperationException("Not valid on a read only property");
          if (property.PropertyType == typeof (bool))
          {
            property.SetValue(switches, (object) !(bool) property.GetValue(switches, (object[]) null), (object[]) null);
          }
          else
          {
            if (!args.MoveNext())
              throw new ArgumentException("switch needs a parameter", name);
            if (((IEnumerable<Type>) new Type[5]
            {
              typeof (string),
              typeof (int),
              typeof (float),
              typeof (double),
              typeof (bool)
            }).Contains<Type>(property.PropertyType))
            {
              property.SetValue(switches, Convert.ChangeType((object) args.Current, property.PropertyType), (object[]) null);
            }
            else
            {
              if (!property.PropertyType.IsEnum)
                return;
              property.SetValue(switches, Enum.Parse(property.PropertyType, args.Current), (object[]) null);
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new ArgumentException("failed to parse command switch value", name, ex);
      }
    }

    private static void SetFiles(object switches, IEnumerable<string> unnamed)
    {
      foreach (PropertyInfo property in switches.GetType().GetProperties())
      {
        if (Attribute.IsDefined((MemberInfo) property, typeof (CommandLineFilesAttribute)))
        {
          if (property.GetValue(switches, (object[]) null) is ICollection<string> list)
            list.AddRange<string>(unnamed);
          if (property.CanRead && property.CanWrite)
          {
            if (property.PropertyType == typeof (string[]))
              property.SetValue(switches, (object) unnamed.ToArray<string>(), (object[]) null);
            else if (property.PropertyType == typeof (IEnumerable<string>))
              property.SetValue(switches, (object) unnamed, (object[]) null);
          }
        }
      }
    }
  }
}
