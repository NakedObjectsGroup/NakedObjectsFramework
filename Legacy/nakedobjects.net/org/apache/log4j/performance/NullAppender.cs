// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.NullAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.apache.log4j.spi;

namespace org.apache.log4j.performance
{
  public class NullAppender : AppenderSkeleton
  {
    public static string s;
    public string t;

    public NullAppender()
    {
    }

    public NullAppender(Layout layout) => this.layout = layout;

    public override void close()
    {
    }

    public override void doAppend(LoggingEvent @event)
    {
      if (this.layout == null)
        return;
      this.t = this.layout.format(@event);
      NullAppender.s = this.t;
    }

    public override void append(LoggingEvent @event)
    {
    }

    public override bool requiresLayout() => true;
  }
}
