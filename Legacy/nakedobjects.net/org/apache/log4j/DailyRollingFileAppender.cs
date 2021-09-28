// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.DailyRollingFileAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class DailyRollingFileAppender : FileAppender
  {
    [JavaFlags(24)]
    public const int TOP_OF_TROUBLE = -1;
    [JavaFlags(24)]
    public const int TOP_OF_MINUTE = 0;
    [JavaFlags(24)]
    public const int TOP_OF_HOUR = 1;
    [JavaFlags(24)]
    public const int HALF_DAY = 2;
    [JavaFlags(24)]
    public const int TOP_OF_DAY = 3;
    [JavaFlags(24)]
    public const int TOP_OF_WEEK = 4;
    [JavaFlags(24)]
    public const int TOP_OF_MONTH = 5;
    private string datePattern;
    private string scheduledFilename;
    private long nextCheck;
    [JavaFlags(0)]
    public Date now;
    [JavaFlags(0)]
    public SimpleDateFormat sdf;
    [JavaFlags(0)]
    public RollingCalendar rc;
    [JavaFlags(0)]
    public int checkPeriod;
    [JavaFlags(24)]
    public static readonly TimeZone gmtTimeZone;

    public DailyRollingFileAppender()
    {
      this.datePattern = "'.'yyyy-MM-dd";
      this.nextCheck = java.lang.System.currentTimeMillis() - 1L;
      this.now = new Date();
      this.rc = new RollingCalendar();
      this.checkPeriod = -1;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public DailyRollingFileAppender(Layout layout, string filename, string datePattern)
      : base(layout, filename, true)
    {
      this.datePattern = "'.'yyyy-MM-dd";
      this.nextCheck = java.lang.System.currentTimeMillis() - 1L;
      this.now = new Date();
      this.rc = new RollingCalendar();
      this.checkPeriod = -1;
      this.datePattern = datePattern;
      this.activateOptions();
    }

    public virtual void setDatePattern(string pattern) => this.datePattern = pattern;

    public virtual string getDatePattern() => this.datePattern;

    public override void activateOptions()
    {
      base.activateOptions();
      if (this.datePattern != null && this.fileName != null)
      {
        this.now.setTime(java.lang.System.currentTimeMillis());
        this.sdf = new SimpleDateFormat(this.datePattern);
        int checkPeriod = this.computeCheckPeriod();
        this.printPeriodicity(checkPeriod);
        this.rc.setType(checkPeriod);
        File file = new File(this.fileName);
        this.scheduledFilename = new StringBuffer().append(this.fileName).append(((DateFormat) this.sdf).format(new Date(file.lastModified()))).ToString();
      }
      else
        LogLog.error(new StringBuffer().append("Either File or DatePattern options are not set for appender [").append(this.name).append("].").ToString());
    }

    [JavaFlags(0)]
    public virtual void printPeriodicity(int type)
    {
      switch (type)
      {
        case 0:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled every minute.").ToString());
          break;
        case 1:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled on top of every hour.").ToString());
          break;
        case 2:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled at midday and midnight.").ToString());
          break;
        case 3:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled at midnight.").ToString());
          break;
        case 4:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled at start of week.").ToString());
          break;
        case 5:
          LogLog.debug(new StringBuffer().append("Appender [").append(this.name).append("] to be rolled at start of every month.").ToString());
          break;
        default:
          LogLog.warn(new StringBuffer().append("Unknown periodicity for appender [").append(this.name).append("].").ToString());
          break;
      }
    }

    [JavaFlags(0)]
    public virtual int computeCheckPeriod()
    {
      RollingCalendar rollingCalendar = new RollingCalendar(DailyRollingFileAppender.gmtTimeZone, (Locale) Locale.ENGLISH);
      Date now = new Date(0L);
      if (this.datePattern != null)
      {
        for (int type = 0; type <= 5; ++type)
        {
          SimpleDateFormat simpleDateFormat = new SimpleDateFormat(this.datePattern);
          ((DateFormat) simpleDateFormat).setTimeZone(DailyRollingFileAppender.gmtTimeZone);
          string str1 = ((DateFormat) simpleDateFormat).format(now);
          rollingCalendar.setType(type);
          Date date = new Date(rollingCalendar.getNextCheckMillis(now));
          string str2 = ((DateFormat) simpleDateFormat).format(date);
          if (str1 != null && str2 != null && !StringImpl.equals(str1, (object) str2))
            return type;
        }
      }
      return -1;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    [JavaFlags(0)]
    public virtual void rollOver()
    {
      if (this.datePattern == null)
      {
        this.errorHandler.error("Missing DatePattern option in rollOver().");
      }
      else
      {
        string str = new StringBuffer().append(this.fileName).append(((DateFormat) this.sdf).format(this.now)).ToString();
        if (StringImpl.equals(this.scheduledFilename, (object) str))
          return;
        this.closeFile();
        File file = new File(this.scheduledFilename);
        if (file.exists())
          file.delete();
        if (new File(this.fileName).renameTo(file))
          LogLog.debug(new StringBuffer().append(this.fileName).append(" -> ").append(this.scheduledFilename).ToString());
        else
          LogLog.error(new StringBuffer().append("Failed to rename [").append(this.fileName).append("] to [").append(this.scheduledFilename).append("].").ToString());
        try
        {
          this.setFile(this.fileName, false, this.bufferedIO, this.bufferSize);
        }
        catch (IOException ex)
        {
          this.errorHandler.error(new StringBuffer().append("setFile(").append(this.fileName).append(", false) call failed.").ToString());
        }
        this.scheduledFilename = str;
      }
    }

    [JavaFlags(4)]
    public override void subAppend(LoggingEvent @event)
    {
      long num = java.lang.System.currentTimeMillis();
      if (num >= this.nextCheck)
      {
        this.now.setTime(num);
        this.nextCheck = this.rc.getNextCheckMillis(this.now);
        try
        {
          this.rollOver();
        }
        catch (IOException ex)
        {
          LogLog.error("rollOver() failed.", (Throwable) ex);
        }
      }
      base.subAppend(@event);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DailyRollingFileAppender()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
