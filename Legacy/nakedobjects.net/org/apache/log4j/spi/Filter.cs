// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.Filter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;org/apache/log4j/spi/OptionHandler;")]
  public abstract class Filter : OptionHandler
  {
    public Filter next;
    public const int DENY = -1;
    public const int NEUTRAL = 0;
    public const int ACCEPT = 1;

    public virtual void activateOptions()
    {
    }

    public abstract int decide(LoggingEvent @event);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Filter filter = this;
      ObjectImpl.clone((object) filter);
      return ((object) filter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
