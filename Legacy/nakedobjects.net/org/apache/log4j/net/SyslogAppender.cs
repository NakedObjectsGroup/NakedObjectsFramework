// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SyslogAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.net
{
  public class SyslogAppender : AppenderSkeleton
  {
    public const int LOG_KERN = 0;
    public const int LOG_USER = 8;
    public const int LOG_MAIL = 16;
    public const int LOG_DAEMON = 24;
    public const int LOG_AUTH = 32;
    public const int LOG_SYSLOG = 40;
    public const int LOG_LPR = 48;
    public const int LOG_NEWS = 56;
    public const int LOG_UUCP = 64;
    public const int LOG_CRON = 72;
    public const int LOG_AUTHPRIV = 80;
    public const int LOG_FTP = 88;
    public const int LOG_LOCAL0 = 128;
    public const int LOG_LOCAL1 = 136;
    public const int LOG_LOCAL2 = 144;
    public const int LOG_LOCAL3 = 152;
    public const int LOG_LOCAL4 = 160;
    public const int LOG_LOCAL5 = 168;
    public const int LOG_LOCAL6 = 176;
    public const int LOG_LOCAL7 = 184;
    [JavaFlags(28)]
    public const int SYSLOG_HOST_OI = 0;
    [JavaFlags(28)]
    public const int FACILITY_OI = 1;
    [JavaFlags(24)]
    public const string TAB = "    ";
    [JavaFlags(0)]
    public int syslogFacility;
    [JavaFlags(0)]
    public string facilityStr;
    [JavaFlags(0)]
    public bool facilityPrinting;
    [JavaFlags(0)]
    public SyslogQuietWriter sqw;
    [JavaFlags(0)]
    public string syslogHost;

    public SyslogAppender()
    {
      this.syslogFacility = 8;
      this.facilityPrinting = false;
      this.initSyslogFacilityStr();
    }

    public SyslogAppender(Layout layout, int syslogFacility)
    {
      this.syslogFacility = 8;
      this.facilityPrinting = false;
      this.layout = layout;
      this.syslogFacility = syslogFacility;
      this.initSyslogFacilityStr();
    }

    public SyslogAppender(Layout layout, string syslogHost, int syslogFacility)
      : this(layout, syslogFacility)
    {
      this.setSyslogHost(syslogHost);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void close()
    {
      this.closed = true;
      this.sqw = (SyslogQuietWriter) null;
    }

    private void initSyslogFacilityStr()
    {
      this.facilityStr = SyslogAppender.getFacilityString(this.syslogFacility);
      if (this.facilityStr == null)
      {
        ((PrintStream) java.lang.System.err).println(new StringBuffer().append("\"").append(this.syslogFacility).append("\" is an unknown syslog facility. Defaulting to \"USER\".").ToString());
        this.syslogFacility = 8;
        this.facilityStr = "user:";
      }
      else
        this.facilityStr = new StringBuffer().append(this.facilityStr).append(":").ToString();
    }

    public static string getFacilityString(int syslogFacility)
    {
      switch (syslogFacility)
      {
        case 0:
          return "kern";
        case 8:
          return "user";
        case 16:
          return "mail";
        case 24:
          return "daemon";
        case 32:
          return "auth";
        case 40:
          return "syslog";
        case 48:
          return "lpr";
        case 56:
          return "news";
        case 64:
          return "uucp";
        case 72:
          return "cron";
        case 80:
          return "authpriv";
        case 88:
          return "ftp";
        case 128:
          return "local0";
        case 136:
          return "local1";
        case 144:
          return "local2";
        case 152:
          return "local3";
        case 160:
          return "local4";
        case 168:
          return "local5";
        case 176:
          return "local6";
        case 184:
          return "local7";
        default:
          return (string) null;
      }
    }

    public static int getFacility(string facilityName)
    {
      if (facilityName != null)
        facilityName = StringImpl.trim(facilityName);
      if (StringImpl.equalsIgnoreCase("KERN", facilityName))
        return 0;
      if (StringImpl.equalsIgnoreCase("USER", facilityName))
        return 8;
      if (StringImpl.equalsIgnoreCase("MAIL", facilityName))
        return 16;
      if (StringImpl.equalsIgnoreCase("DAEMON", facilityName))
        return 24;
      if (StringImpl.equalsIgnoreCase("AUTH", facilityName))
        return 32;
      if (StringImpl.equalsIgnoreCase("SYSLOG", facilityName))
        return 40;
      if (StringImpl.equalsIgnoreCase("LPR", facilityName))
        return 48;
      if (StringImpl.equalsIgnoreCase("NEWS", facilityName))
        return 56;
      if (StringImpl.equalsIgnoreCase("UUCP", facilityName))
        return 64;
      if (StringImpl.equalsIgnoreCase("CRON", facilityName))
        return 72;
      if (StringImpl.equalsIgnoreCase("AUTHPRIV", facilityName))
        return 80;
      if (StringImpl.equalsIgnoreCase("FTP", facilityName))
        return 88;
      if (StringImpl.equalsIgnoreCase("LOCAL0", facilityName))
        return 128;
      if (StringImpl.equalsIgnoreCase("LOCAL1", facilityName))
        return 136;
      if (StringImpl.equalsIgnoreCase("LOCAL2", facilityName))
        return 144;
      if (StringImpl.equalsIgnoreCase("LOCAL3", facilityName))
        return 152;
      if (StringImpl.equalsIgnoreCase("LOCAL4", facilityName))
        return 160;
      if (StringImpl.equalsIgnoreCase("LOCAL5", facilityName))
        return 168;
      if (StringImpl.equalsIgnoreCase("LOCAL6", facilityName))
        return 176;
      return StringImpl.equalsIgnoreCase("LOCAL7", facilityName) ? 184 : -1;
    }

    public override void append(LoggingEvent @event)
    {
      if (!this.isAsSevereAsThreshold((Priority) @event.getLevel()))
        return;
      if (this.sqw == null)
      {
        this.errorHandler.error(new StringBuffer().append("No syslog host is set for SyslogAppedender named \"").append(this.name).append("\".").ToString());
      }
      else
      {
        string @string = new StringBuffer().append(!this.facilityPrinting ? "" : this.facilityStr).append(this.layout.format(@event)).ToString();
        this.sqw.setLevel(@event.getLevel().getSyslogEquivalent());
        this.sqw.write(@string);
        string[] throwableStrRep = @event.getThrowableStrRep();
        if (throwableStrRep == null)
          return;
        int length = throwableStrRep.Length;
        if (length <= 0)
          return;
        this.sqw.write(throwableStrRep[0]);
        for (int index = 1; index < length; ++index)
          this.sqw.write(new StringBuffer().append("    ").append(StringImpl.substring(throwableStrRep[index], 1)).ToString());
      }
    }

    public override void activateOptions()
    {
    }

    public override bool requiresLayout() => true;

    public virtual void setSyslogHost(string syslogHost)
    {
      this.sqw = new SyslogQuietWriter((Writer) new SyslogWriter(syslogHost), this.syslogFacility, this.errorHandler);
      this.syslogHost = syslogHost;
    }

    public virtual string getSyslogHost() => this.syslogHost;

    public virtual void setFacility(string facilityName)
    {
      if (facilityName == null)
        return;
      this.syslogFacility = SyslogAppender.getFacility(facilityName);
      if (this.syslogFacility == -1)
      {
        ((PrintStream) java.lang.System.err).println(new StringBuffer().append("[").append(facilityName).append("] is an unknown syslog facility. Defaulting to [USER].").ToString());
        this.syslogFacility = 8;
      }
      this.initSyslogFacilityStr();
      if (this.sqw == null)
        return;
      this.sqw.setSyslogFacility(this.syslogFacility);
    }

    public virtual string getFacility() => SyslogAppender.getFacilityString(this.syslogFacility);

    public virtual void setFacilityPrinting(bool on) => this.facilityPrinting = on;

    public virtual bool getFacilityPrinting() => this.facilityPrinting;
  }
}
