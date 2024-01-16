// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ViewItemRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public abstract class ViewItemRenderer : ThumbRenderer
  {
    private Color backColor = Color.Transparent;
    private Color foreColor = SystemColors.WindowText;
    private readonly List<TextLine> textLines = new List<TextLine>();

    public Color BackColor
    {
      get => this.backColor;
      set => this.backColor = value;
    }

    public Color ForeColor
    {
      get => this.foreColor;
      set => this.foreColor = value;
    }

    public Size Border { get; set; }

    public List<TextLine> TextLines => this.textLines;

    public void DisposeTextLines()
    {
      this.TextLines.ForEach((Action<TextLine>) (tl => tl.Dispose()));
      this.TextLines.Clear();
    }
  }
}
