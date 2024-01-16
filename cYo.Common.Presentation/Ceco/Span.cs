// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Span
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class Span : Inline
  {
    private InlineCollection inlines;

    public Span()
    {
    }

    public Span(params Inline[] inlines) => this.Inlines.AddRange((IEnumerable<Inline>) inlines);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.inlines != null)
      {
        foreach (DisposableObject inline in (Collection<Inline>) this.inlines)
          inline.Dispose();
      }
      base.Dispose(disposing);
    }

    public InlineCollection Inlines
    {
      get
      {
        if (this.inlines == null)
        {
          this.inlines = new InlineCollection();
          this.inlines.Changed += new CollectionChangeEventHandler(this.inlines_Changed);
        }
        return this.inlines;
      }
    }

    public IList<Inline> GetSubItems(bool includeOwn)
    {
      List<Inline> items = new List<Inline>();
      if (includeOwn)
        items.Add((Inline) this);
      Span.AddItems(this, (ICollection<Inline>) items);
      return (IList<Inline>) items;
    }

    public void OffsetInlines(Point offset)
    {
      if (offset.IsEmpty)
        return;
      foreach (Inline subItem in (IEnumerable<Inline>) this.GetSubItems(false))
      {
        subItem.X += offset.X;
        subItem.Y += offset.Y;
      }
    }

    public override bool IsNode => this.inlines != null;

    public override Inline GetHitItem(Point location, Point hitPoint)
    {
      Rectangle bounds = this.Bounds;
      bounds.Offset(location);
      foreach (Inline subItem in (IEnumerable<Inline>) this.GetSubItems(false))
      {
        Inline hitItem = subItem.GetHitItem(bounds.Location, hitPoint);
        if (hitItem != null)
          return hitItem;
      }
      return base.GetHitItem(location, hitPoint);
    }

    protected override void InvokeLayout(LayoutType type)
    {
      base.InvokeLayout(type);
      if (type != LayoutType.Full)
        return;
      foreach (Inline subItem in (IEnumerable<Inline>) this.GetSubItems(false))
        subItem.PendingLayout = LayoutType.Full;
    }

    private void inlines_Changed(object sender, CollectionChangeEventArgs e)
    {
      Inline element = (Inline) e.Element;
      switch (e.Action)
      {
        case CollectionChangeAction.Add:
          element.ParentInline = (Inline) this;
          break;
        case CollectionChangeAction.Remove:
          element.ParentInline = (Inline) null;
          break;
      }
      this.InvokeLayout(LayoutType.Full);
    }

    private static void AddItems(Span span, ICollection<Inline> items)
    {
      if (span == null || span.inlines == null)
        return;
      foreach (Inline inline in (Collection<Inline>) span.inlines)
      {
        items.Add(inline);
        if (!inline.IsBlock)
          Span.AddItems(inline as Span, items);
      }
    }
  }
}
