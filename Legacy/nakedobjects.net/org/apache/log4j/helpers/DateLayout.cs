// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.DateLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.apache.log4j.spi;
using System;

namespace org.apache.log4j.helpers
{
  public abstract class DateLayout : Layout
  {
    public const string NULL_DATE_FORMAT = "NULL";
    public const string RELATIVE_TIME_DATE_FORMAT = "RELATIVE";
    [JavaFlags(4)]
    public FieldPosition pos;
    [Obsolete(null, false)]
    public const string DATE_FORMAT_OPTION = "DateFormat";
    [Obsolete(null, false)]
    public const string TIMEZONE_OPTION = "TimeZone";
    private string timeZoneID;
    private string dateFormatOption;
    [JavaFlags(4)]
    public DateFormat dateFormat;
    [JavaFlags(4)]
    public Date date;

    [Obsolete(null, false)]
    public virtual string[] getOptionStrings()
    {
      int length = 2;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      strArray[0] = "DateFormat";
      strArray[1] = "TimeZone";
      return strArray;
    }

    [Obsolete(null, false)]
    public virtual void setOption(string option, string value)
    {
      if (StringImpl.equalsIgnoreCase(option, "DateFormat"))
      {
        this.dateFormatOption = StringImpl.toUpperCase(value);
      }
      else
      {
        if (!StringImpl.equalsIgnoreCase(option, "TimeZone"))
          return;
        this.timeZoneID = value;
      }
    }

    public virtual void setDateFormat(string dateFormat)
    {
      if (dateFormat != null)
        this.dateFormatOption = dateFormat;
      this.setDateFormat(this.dateFormatOption, TimeZone.getDefault());
    }

    public virtual string getDateFormat() => this.dateFormatOption;

    public virtual void setTimeZone(string timeZone) => this.timeZoneID = timeZone;

    public virtual string getTimeZone() => this.timeZoneID;

    public override void activateOptions()
    {
      this.setDateFormat(this.dateFormatOption);
      if (this.timeZoneID == null || this.dateFormat == null)
        return;
      this.dateFormat.setTimeZone(TimeZone.getTimeZone(this.timeZoneID));
    }

    public virtual void dateFormat(StringBuffer buf, LoggingEvent @event)
    {
      if (this.dateFormat == null)
        return;
      this.date.setTime(@event.timeStamp);
      this.dateFormat.format(this.date, buf, this.pos);
      buf.append(' ');
    }

    public virtual void setDateFormat(DateFormat dateFormat, TimeZone timeZone)
    {
      this.dateFormat = dateFormat;
      this.dateFormat.setTimeZone(timeZone);
    }

    public virtual void setDateFormat(string dateFormatType, TimeZone timeZone)
    {
      if (dateFormatType == null)
        this.dateFormat = (DateFormat) null;
      else if (StringImpl.equalsIgnoreCase(dateFormatType, "NULL"))
        this.dateFormat = (DateFormat) null;
      else if (StringImpl.equalsIgnoreCase(dateFormatType, "RELATIVE"))
        this.dateFormat = (DateFormat) new RelativeTimeDateFormat();
      else if (StringImpl.equalsIgnoreCase(dateFormatType, "ABSOLUTE"))
        this.dateFormat = (DateFormat) new AbsoluteTimeDateFormat(timeZone);
      else if (StringImpl.equalsIgnoreCase(dateFormatType, "DATE"))
        this.dateFormat = (DateFormat) new DateTimeDateFormat(timeZone);
      else if (StringImpl.equalsIgnoreCase(dateFormatType, "ISO8601"))
      {
        this.dateFormat = (DateFormat) new ISO8601DateFormat(timeZone);
      }
      else
      {
        this.dateFormat = (DateFormat) new SimpleDateFormat(dateFormatType);
        this.dateFormat.setTimeZone(timeZone);
      }
    }

    public DateLayout()
    {
      this.pos = new FieldPosition(0);
      this.date = new Date();
    }
  }
}
