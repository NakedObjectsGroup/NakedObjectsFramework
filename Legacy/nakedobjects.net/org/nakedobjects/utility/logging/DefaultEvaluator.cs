// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.DefaultEvaluator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.log4j;
using org.apache.log4j.spi;

namespace org.nakedobjects.utility.logging
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/apache/log4j/spi/TriggeringEventEvaluator;")]
  public class DefaultEvaluator : TriggeringEventEvaluator
  {
    public virtual bool isTriggeringEvent(LoggingEvent @event) => @event.getLevel().isGreaterOrEqual((Priority) Level.ERROR);

    [JavaFlags(0)]
    public DefaultEvaluator()
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DefaultEvaluator defaultEvaluator = this;
      ObjectImpl.clone((object) defaultEvaluator);
      return ((object) defaultEvaluator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
