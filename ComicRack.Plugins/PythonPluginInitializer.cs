// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.PythonPluginInitializer
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.IO;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public class PythonPluginInitializer : PluginInitializer
  {
    private static readonly Regex rxComment = new Regex("#\\s*@(?<name>[A-Za-z][\\w_]*)\\s+(?<value>.*)", RegexOptions.Compiled);
    private static readonly Regex rxFunction = new Regex("def\\s+(?<function>[A-Za-z][\\w_]+)", RegexOptions.Compiled);

    public override IEnumerable<Command> GetCommands(string file)
    {
      List<Command> commands = new List<Command>();
      try
      {
        if (!".py".Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase))
          return (IEnumerable<Command>) commands.ToArray();
        PythonCommand pythonCommand = (PythonCommand) null;
        foreach (string input in FileUtility.ReadLines(file).TrimStrings().RemoveEmpty())
        {
          Match match1 = PythonPluginInitializer.rxComment.Match(input);
          Match match2 = PythonPluginInitializer.rxFunction.Match(input);
          if (match1.Success)
          {
            if (pythonCommand == null)
              pythonCommand = new PythonCommand()
              {
                ScriptFile = Path.GetFileName(file)
              };
            try
            {
              string name = match1.Groups[1].Value;
              PropertyInfo property = pythonCommand.GetType().GetProperty(name);
              object obj = Convert.ChangeType((object) match1.Groups[2].Value, property.PropertyType);
              property.SetValue((object) pythonCommand, obj, (object[]) null);
            }
            catch
            {
            }
          }
          if (match2.Success && pythonCommand != null)
          {
            pythonCommand.Method = match2.Groups[1].Value;
            commands.Add((Command) pythonCommand);
            pythonCommand = (PythonCommand) null;
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (IEnumerable<Command>) commands;
    }
  }
}
