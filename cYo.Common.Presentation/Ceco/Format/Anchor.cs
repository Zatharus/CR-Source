// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.Anchor
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class Anchor : Span
  {
    public Anchor() => this.Initialize();

    public Anchor(params Inline[] inlines)
      : base(inlines)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.ForeColor = Color.Red;
      this.MouseCursor = Cursors.Hand;
    }

    public string HRef { get; set; }

    protected override void OnMouseEnter()
    {
      base.OnMouseEnter();
      this.FontStyle = FontStyle.Bold | FontStyle.Underline;
    }

    protected override void OnMouseLeave()
    {
      base.OnMouseLeave();
      this.FontStyle = FontStyle.Regular;
    }
  }
}
