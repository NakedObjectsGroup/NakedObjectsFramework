// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.system.SystemClock
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.application.system
{
  [JavaInterfaces("1;org/nakedobjects/application/system/Clock;")]
  public class SystemClock : Clock
  {
    public SystemClock()
    {
      Date.setClock((Clock) this);
      Time.setClock((Clock) this);
      DateTime.setClock((Clock) this);
      TimeStamp.setClock((Clock) this);
    }

    public virtual long getTime() => System.currentTimeMillis();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SystemClock systemClock = this;
      ObjectImpl.clone((object) systemClock);
      return ((object) systemClock).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
