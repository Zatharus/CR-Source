// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.CommandKey
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;

#nullable disable
namespace cYo.Common.Windows
{
  [Flags]
  public enum CommandKey
  {
    None = 0,
    A = 65, // 0x00000041
    B = 66, // 0x00000042
    C = 67, // 0x00000043
    D = 68, // 0x00000044
    E = 69, // 0x00000045
    F = 70, // 0x00000046
    G = 71, // 0x00000047
    H = 72, // 0x00000048
    I = 73, // 0x00000049
    J = 74, // 0x0000004A
    K = 75, // 0x0000004B
    L = 76, // 0x0000004C
    M = 77, // 0x0000004D
    N = 78, // 0x0000004E
    O = 79, // 0x0000004F
    P = 80, // 0x00000050
    Q = 81, // 0x00000051
    R = 82, // 0x00000052
    S = 83, // 0x00000053
    T = 84, // 0x00000054
    U = 85, // 0x00000055
    V = 86, // 0x00000056
    W = 87, // 0x00000057
    X = 88, // 0x00000058
    Y = 89, // 0x00000059
    Z = 90, // 0x0000005A
    D0 = 48, // 0x00000030
    D1 = 49, // 0x00000031
    D2 = 50, // 0x00000032
    D3 = 51, // 0x00000033
    D4 = 52, // 0x00000034
    D5 = 53, // 0x00000035
    D6 = 54, // 0x00000036
    D7 = 55, // 0x00000037
    D8 = 56, // 0x00000038
    D9 = 57, // 0x00000039
    NumPad0 = 96, // 0x00000060
    NumPad1 = 97, // 0x00000061
    NumPad2 = 98, // 0x00000062
    NumPad3 = 99, // 0x00000063
    NumPad4 = 100, // 0x00000064
    NumPad5 = 101, // 0x00000065
    NumPad6 = 102, // 0x00000066
    NumPad7 = 103, // 0x00000067
    NumPad8 = 104, // 0x00000068
    NumPad9 = 105, // 0x00000069
    Enter = 13, // 0x0000000D
    Escape = 27, // 0x0000001B
    Space = 32, // 0x00000020
    Subtract = 109, // 0x0000006D
    Add = Space | K, // 0x0000006B
    Multiply = Space | J, // 0x0000006A
    Divide = 111, // 0x0000006F
    Decimal = 110, // 0x0000006E
    Up = 38, // 0x00000026
    Down = 40, // 0x00000028
    Left = 37, // 0x00000025
    Right = 39, // 0x00000027
    Tab = 9,
    Home = 36, // 0x00000024
    PageUp = 33, // 0x00000021
    PageDown = 34, // 0x00000022
    End = 35, // 0x00000023
    MediaNextTrack = 176, // 0x000000B0
    MediaPreviousTrack = 177, // 0x000000B1
    MediaPlayPause = 179, // 0x000000B3
    MediaStop = 178, // 0x000000B2
    Zoom = 251, // 0x000000FB
    MouseLeft = MediaNextTrack | L, // 0x000000FC
    MouseDoubleLeft = 253, // 0x000000FD
    MouseMiddle = 254, // 0x000000FE
    MouseDoubleMiddle = 255, // 0x000000FF
    MouseRight = 256, // 0x00000100
    MouseDoubleRight = 257, // 0x00000101
    MouseButton4 = 258, // 0x00000102
    MouseDoubleButton4 = 259, // 0x00000103
    MouseButton5 = 260, // 0x00000104
    MouseDoubleButton5 = 261, // 0x00000105
    MouseWheelUp = 262, // 0x00000106
    MouseWheelDown = 263, // 0x00000107
    MouseTiltRight = 264, // 0x00000108
    MouseTiltLeft = 265, // 0x00000109
    Gesture1 = 266, // 0x0000010A
    Gesture2 = 267, // 0x0000010B
    Gesture3 = 268, // 0x0000010C
    Gesture4 = 269, // 0x0000010D
    Gesture5 = 270, // 0x0000010E
    Gesture6 = 271, // 0x0000010F
    Gesture7 = 272, // 0x00000110
    Gesture8 = 273, // 0x00000111
    Gesture9 = 274, // 0x00000112
    GestureDouble1 = 275, // 0x00000113
    GestureDouble2 = 276, // 0x00000114
    GestureDouble3 = 277, // 0x00000115
    GestureDouble4 = 278, // 0x00000116
    GestureDouble5 = 279, // 0x00000117
    GestureDouble6 = 280, // 0x00000118
    GestureDouble7 = 281, // 0x00000119
    GestureDouble8 = 282, // 0x0000011A
    GestureDouble9 = 283, // 0x0000011B
    TouchTap = 284, // 0x0000011C
    TouchDoubleTap = 285, // 0x0000011D
    TouchPressAndTap = 286, // 0x0000011E
    TouchTwoFingerTap = 287, // 0x0000011F
    FlickLeft = MouseRight | Space, // 0x00000120
    FlickRight = 289, // 0x00000121
    Ctrl = 131072, // 0x00020000
    Shift = 65536, // 0x00010000
    Alt = 262144, // 0x00040000
    Modifiers = -65536, // 0xFFFF0000
  }
}
