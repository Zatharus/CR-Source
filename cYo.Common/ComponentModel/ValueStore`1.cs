// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ValueStore`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class ValueStore<T> : IValueStore<T>
  {
    private Action<T> setCall;
    private Func<T> getCall;

    public ValueStore(Action<T> setCall, Func<T> getCall)
    {
      this.setCall = setCall;
      this.getCall = getCall;
    }

    public T GetValue()
    {
      if (this.getCall == null)
        throw new NotImplementedException();
      return this.getCall();
    }

    public void SetValue(T value)
    {
      if (this.setCall == null)
        throw new NotImplementedException();
      this.setCall(value);
    }
  }
}
