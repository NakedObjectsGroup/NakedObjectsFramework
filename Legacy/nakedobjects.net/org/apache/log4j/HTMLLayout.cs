// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.HTMLLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class HTMLLayout : Layout
  {
    [JavaFlags(20)]
    public readonly int BUF_SIZE;
    [JavaFlags(20)]
    public readonly int MAX_CAPACITY;
    [JavaFlags(8)]
    public static string TRACE_PREFIX;
    private StringBuffer sbuf;
    [Obsolete(null, false)]
    public const string LOCATION_INFO_OPTION = "LocationInfo";
    public const string TITLE_OPTION = "Title";
    [JavaFlags(0)]
    public bool locationInfo;
    [JavaFlags(0)]
    public string title;

    public virtual void setLocationInfo(bool flag) => this.locationInfo = flag;

    public virtual bool getLocationInfo() => this.locationInfo;

    public virtual void setTitle(string title) => this.title = title;

    public virtual string getTitle() => this.title;

    public override string getContentType() => "text/html";

    public override void activateOptions()
    {
    }

    public override string format(LoggingEvent @event)
    {
      if (this.sbuf.capacity() > 1024)
        this.sbuf = new StringBuffer(256);
      else
        this.sbuf.setLength(0);
      this.sbuf.append(new StringBuffer().append(Layout.LINE_SEP).append("<tr>").append(Layout.LINE_SEP).ToString());
      this.sbuf.append("<td>");
      this.sbuf.append(@event.timeStamp - LoggingEvent.getStartTime());
      this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      this.sbuf.append(new StringBuffer().append("<td title=\"").append(@event.getThreadName()).append(" thread\">").ToString());
      this.sbuf.append(Transform.escapeTags(@event.getThreadName()));
      this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      this.sbuf.append("<td title=\"Level\">");
      if (@event.getLevel().Equals((object) Level.DEBUG))
      {
        this.sbuf.append("<font color=\"#339933\">");
        this.sbuf.append((object) @event.getLevel());
        this.sbuf.append("</font>");
      }
      else if (@event.getLevel().isGreaterOrEqual((Priority) Level.WARN))
      {
        this.sbuf.append("<font color=\"#993300\"><strong>");
        this.sbuf.append((object) @event.getLevel());
        this.sbuf.append("</strong></font>");
      }
      else
        this.sbuf.append((object) @event.getLevel());
      this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      this.sbuf.append(new StringBuffer().append("<td title=\"").append(@event.getLoggerName()).append(" category\">").ToString());
      this.sbuf.append(Transform.escapeTags(@event.getLoggerName()));
      this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      if (this.locationInfo)
      {
        LocationInfo locationInformation = @event.getLocationInformation();
        this.sbuf.append("<td>");
        this.sbuf.append(Transform.escapeTags(locationInformation.getFileName()));
        this.sbuf.append(':');
        this.sbuf.append(locationInformation.getLineNumber());
        this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      }
      this.sbuf.append("<td title=\"Message\">");
      this.sbuf.append(Transform.escapeTags(@event.getRenderedMessage()));
      this.sbuf.append(new StringBuffer().append("</td>").append(Layout.LINE_SEP).ToString());
      this.sbuf.append(new StringBuffer().append("</tr>").append(Layout.LINE_SEP).ToString());
      if (@event.getNDC() != null)
      {
        this.sbuf.append("<tr><td bgcolor=\"#EEEEEE\" style=\"font-size : xx-small;\" colspan=\"6\" title=\"Nested Diagnostic Context\">");
        this.sbuf.append(new StringBuffer().append("NDC: ").append(Transform.escapeTags(@event.getNDC())).ToString());
        this.sbuf.append(new StringBuffer().append("</td></tr>").append(Layout.LINE_SEP).ToString());
      }
      string[] throwableStrRep = @event.getThrowableStrRep();
      if (throwableStrRep != null)
      {
        this.sbuf.append("<tr><td bgcolor=\"#993300\" style=\"color:White; font-size : xx-small;\" colspan=\"6\">");
        this.appendThrowableAsHTML(throwableStrRep, this.sbuf);
        this.sbuf.append(new StringBuffer().append("</td></tr>").append(Layout.LINE_SEP).ToString());
      }
      return this.sbuf.ToString();
    }

    [JavaFlags(0)]
    public virtual void appendThrowableAsHTML(string[] s, StringBuffer sbuf)
    {
      if (s == null)
        return;
      int length = s.Length;
      if (length == 0)
        return;
      sbuf.append(Transform.escapeTags(s[0]));
      sbuf.append(Layout.LINE_SEP);
      for (int index = 1; index < length; ++index)
      {
        sbuf.append(HTMLLayout.TRACE_PREFIX);
        sbuf.append(Transform.escapeTags(s[index]));
        sbuf.append(Layout.LINE_SEP);
      }
    }

    public override string getHeader()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(new StringBuffer().append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<html>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<head>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<title>").append(this.title).append("</title>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<style type=\"text/css\">").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<!--").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("body, table {font-family: arial,sans-serif; font-size: x-small;}").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("th {background: #336699; color: #FFFFFF; text-align: left;}").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("-->").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("</style>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("</head>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<body bgcolor=\"#FFFFFF\" topmargin=\"6\" leftmargin=\"6\">").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<hr size=\"1\" noshade>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("Log session start time ").append((object) new Date()).append("<br>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<br>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<table cellspacing=\"0\" cellpadding=\"4\" border=\"1\" bordercolor=\"#224466\" width=\"100%\">").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<tr>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<th>Time</th>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<th>Thread</th>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<th>Level</th>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<th>Category</th>").append(Layout.LINE_SEP).ToString());
      if (this.locationInfo)
        stringBuffer.append(new StringBuffer().append("<th>File:Line</th>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<th>Message</th>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("</tr>").append(Layout.LINE_SEP).ToString());
      return stringBuffer.ToString();
    }

    public override string getFooter()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(new StringBuffer().append("</table>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append(new StringBuffer().append("<br>").append(Layout.LINE_SEP).ToString());
      stringBuffer.append("</body></html>");
      return stringBuffer.ToString();
    }

    public override bool ignoresThrowable() => false;

    public HTMLLayout()
    {
      this.BUF_SIZE = 256;
      this.MAX_CAPACITY = 1024;
      this.sbuf = new StringBuffer(256);
      this.locationInfo = false;
      this.title = "Log4J Log Messages";
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static HTMLLayout()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
