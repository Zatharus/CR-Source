// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.Disposer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class Disposer : DisposableObject
  {
    private readonly Action method;
    private readonly bool eatErrors;

    public Disposer(Action method, bool eatErrors = false)
    {
      this.method = method;
      this.eatErrors = eatErrors;
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing)
          this.method();
      }
      catch
      {
        if (!this.eatErrors)
          throw;
      }
      base.Dispose(disposing);
    }
  }
}
