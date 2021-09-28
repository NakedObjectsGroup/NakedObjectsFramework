// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.SimpleLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j
{
  public class SimpleLayout : Layout
  {
    [JavaFlags(0)]
    public StringBuffer sbuf;

    public SimpleLayout() => this.sbuf = new StringBuffer(128);

    public override void activateOptions()
    {
    }

    public override string format(LoggingEvent @event)
    {
      this.sbuf.setLength(0);
      this.sbuf.append(@event.getLevel().ToString());
      this.sbuf.append(" - ");
      this.sbuf.append(@event.getRenderedMessage());
      this.sbuf.append(Layout.LINE_SEP);
      return this.sbuf.ToString();
    }

    public override bool ignoresThrowable() => true;
  }
}
