// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.TTCCLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;

namespace org.apache.log4j
{
  public class TTCCLayout : DateLayout
  {
    private bool threadPrinting;
    private bool categoryPrefixing;
    private bool contextPrinting;
    [JavaFlags(20)]
    public readonly StringBuffer buf;

    public TTCCLayout()
    {
      this.threadPrinting = true;
      this.categoryPrefixing = true;
      this.contextPrinting = true;
      this.buf = new StringBuffer(256);
      this.setDateFormat("RELATIVE", (TimeZone) null);
    }

    public TTCCLayout(string dateFormatType)
    {
      this.threadPrinting = true;
      this.categoryPrefixing = true;
      this.contextPrinting = true;
      this.buf = new StringBuffer(256);
      this.setDateFormat(dateFormatType);
    }

    public virtual void setThreadPrinting(bool threadPrinting) => this.threadPrinting = threadPrinting;

    public virtual bool getThreadPrinting() => this.threadPrinting;

    public virtual void setCategoryPrefixing(bool categoryPrefixing) => this.categoryPrefixing = categoryPrefixing;

    public virtual bool getCategoryPrefixing() => this.categoryPrefixing;

    public virtual void setContextPrinting(bool contextPrinting) => this.contextPrinting = contextPrinting;

    public virtual bool getContextPrinting() => this.contextPrinting;

    public override string format(LoggingEvent @event)
    {
      this.buf.setLength(0);
      this.dateFormat(this.buf, @event);
      if (this.threadPrinting)
      {
        this.buf.append('[');
        this.buf.append(@event.getThreadName());
        this.buf.append("] ");
      }
      this.buf.append(@event.getLevel().ToString());
      this.buf.append(' ');
      if (this.categoryPrefixing)
      {
        this.buf.append(@event.getLoggerName());
        this.buf.append(' ');
      }
      if (this.contextPrinting)
      {
        string ndc = @event.getNDC();
        if (ndc != null)
        {
          this.buf.append(ndc);
          this.buf.append(' ');
        }
      }
      this.buf.append("- ");
      this.buf.append(@event.getRenderedMessage());
      this.buf.append(Layout.LINE_SEP);
      return this.buf.ToString();
    }

    public override bool ignoresThrowable() => true;
  }
}
