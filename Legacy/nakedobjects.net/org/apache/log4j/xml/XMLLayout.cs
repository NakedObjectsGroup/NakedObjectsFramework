// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.XMLLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;

namespace org.apache.log4j.xml
{
  public class XMLLayout : Layout
  {
    private readonly int DEFAULT_SIZE;
    private readonly int UPPER_LIMIT;
    private StringBuffer buf;
    private bool locationInfo;

    public virtual void setLocationInfo(bool flag) => this.locationInfo = flag;

    public virtual bool getLocationInfo() => this.locationInfo;

    public override void activateOptions()
    {
    }

    public override string format(LoggingEvent @event)
    {
      if (this.buf.capacity() > 2048)
        this.buf = new StringBuffer(256);
      else
        this.buf.setLength(0);
      this.buf.append("<log4j:event logger=\"");
      this.buf.append(@event.getLoggerName());
      this.buf.append("\" timestamp=\"");
      this.buf.append(@event.timeStamp);
      this.buf.append("\" level=\"");
      this.buf.append((object) @event.getLevel());
      this.buf.append("\" thread=\"");
      this.buf.append(@event.getThreadName());
      this.buf.append("\">\r\n");
      this.buf.append("<log4j:message><![CDATA[");
      Transform.appendEscapingCDATA(this.buf, @event.getRenderedMessage());
      this.buf.append("]]></log4j:message>\r\n");
      string ndc = @event.getNDC();
      if (ndc != null)
      {
        this.buf.append("<log4j:NDC><![CDATA[");
        this.buf.append(ndc);
        this.buf.append("]]></log4j:NDC>\r\n");
      }
      string[] throwableStrRep = @event.getThrowableStrRep();
      if (throwableStrRep != null)
      {
        this.buf.append("<log4j:throwable><![CDATA[");
        for (int index = 0; index < throwableStrRep.Length; ++index)
        {
          this.buf.append(throwableStrRep[index]);
          this.buf.append("\r\n");
        }
        this.buf.append("]]></log4j:throwable>\r\n");
      }
      if (this.locationInfo)
      {
        LocationInfo locationInformation = @event.getLocationInformation();
        this.buf.append("<log4j:locationInfo class=\"");
        this.buf.append(locationInformation.getClassName());
        this.buf.append("\" method=\"");
        this.buf.append(Transform.escapeTags(locationInformation.getMethodName()));
        this.buf.append("\" file=\"");
        this.buf.append(locationInformation.getFileName());
        this.buf.append("\" line=\"");
        this.buf.append(locationInformation.getLineNumber());
        this.buf.append("\"/>\r\n");
      }
      this.buf.append("</log4j:event>\r\n\r\n");
      return this.buf.ToString();
    }

    public override bool ignoresThrowable() => false;

    public XMLLayout()
    {
      this.DEFAULT_SIZE = 256;
      this.UPPER_LIMIT = 2048;
      this.buf = new StringBuffer(256);
      this.locationInfo = false;
    }
  }
}
