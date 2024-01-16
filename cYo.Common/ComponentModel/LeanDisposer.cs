// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.LeanDisposer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  internal struct LeanDisposer : IDisposable
  {
    private Action method;
    private bool eatErrors;

    public LeanDisposer(Action method, bool eatErrors = false)
    {
      this.method = method;
      this.eatErrors = eatErrors;
    }

    public void Dispose()
    {
      try
      {
        this.method();
      }
      catch (Exception ex)
      {
        if (this.eatErrors)
          return;
        throw;
      }
    }
  }
}
