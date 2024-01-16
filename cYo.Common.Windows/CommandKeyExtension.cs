// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.CommandKeyExtension
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

#nullable disable
namespace cYo.Common.Windows
{
  public static class CommandKeyExtension
  {
    public static bool IsMouseButton(this CommandKey key)
    {
      key &= ~CommandKey.Modifiers;
      return key == CommandKey.MouseLeft || key == CommandKey.MouseDoubleLeft || key == CommandKey.MouseMiddle || key == CommandKey.MouseDoubleMiddle || key == CommandKey.MouseRight || key == CommandKey.MouseDoubleRight || key == CommandKey.MouseButton4 || key == CommandKey.MouseDoubleButton4 || key == CommandKey.MouseButton5 || key == CommandKey.MouseDoubleButton5 || key == CommandKey.MouseWheelUp || key == CommandKey.MouseWheelDown || key == CommandKey.MouseTiltRight || key == CommandKey.MouseTiltLeft || key == CommandKey.TouchTap || key == CommandKey.TouchDoubleTap || key == CommandKey.TouchPressAndTap || key == CommandKey.TouchTwoFingerTap;
    }
  }
}
