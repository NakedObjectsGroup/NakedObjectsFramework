// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Time
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.apache.log4j;
using org.nakedobjects.application.system;
using System;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class Time : Magnitude
  {
    private static Clock clock;
    private static readonly DateFormat ISO_LONG;
    private static readonly DateFormat ISO_SHORT;
    private static readonly Logger LOG;
    private static readonly DateFormat LONG_FORMAT;
    private static readonly DateFormat MEDIUM_FORMAT;
    public const int MINUTE = 60;
    public const int HOUR = 3600;
    public const int DAY = 86400;
    private const long serialVersionUID = 1;
    private static readonly DateFormat SHORT_FORMAT;
    private static readonly TimeZone timeZone;
    private static readonly long zero;
    private Date date;

    [JavaFlags(8)]
    public static long getZero() => Time.zero / 1000L;

    public static void setClock(Clock clock) => Time.clock = clock;

    public Time()
    {
      if (Time.clock == null)
        throw new org.nakedobjects.application.ApplicationException("Clock not set up");
      this.setValue(new Date(Time.clock.getTime()));
    }

    public Time(int hour, int minute) => this.setValue(hour, minute);

    public Time(Time time) => this.date = time.date;

    public virtual void add(int hours, int minutes)
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.add(12, minutes);
      instance.add(11, hours);
      this.set(instance);
    }

    public virtual Calendar calendarValue()
    {
      if (this.date == null)
        return (Calendar) null;
      Calendar instance = Calendar.getInstance();
      instance.setTimeZone(Time.timeZone);
      instance.setTime(this.date);
      return instance;
    }

    private void checkTime(int hour, int minute, int second)
    {
      if (hour < 0 || hour > 23)
        throw new IllegalArgumentException("Hour must be in the range 0 - 23 inclusive");
      if (minute < 0 || minute > 59)
        throw new IllegalArgumentException("Minute must be in the range 0 - 59 inclusive");
      if (second < 0 || second > 59)
        throw new IllegalArgumentException("Second must be in the range 0 - 59 inclusive");
    }

    public override void clear() => this.date = (Date) null;

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is Time))
        throw new IllegalArgumentException("Can only copy the value of  a Date object");
      this.date = ((Time) @object)?.date;
    }

    private Calendar createCalendar()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTimeZone(Time.timeZone);
      instance.set(14, 0);
      instance.set(13, 0);
      instance.set(5, 1);
      instance.clear(9);
      instance.clear(10);
      instance.set(2, 0);
      instance.set(1, 1970);
      return instance;
    }

    public virtual Date dateValue() => this.date == null ? (Date) null : this.date;

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is Time))
        return false;
      Time time = (Time) obj;
      return time.isEmpty() && this.isEmpty() || time.date.Equals((object) this.date);
    }

    [Obsolete(null, false)]
    public virtual Date getDate() => this.date;

    public virtual int getHour()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTimeZone(Time.timeZone);
      instance.setTime(this.date);
      return instance.get(10);
    }

    public virtual int getMinute()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTimeZone(Time.timeZone);
      instance.setTime(this.date);
      return instance.get(12);
    }

    public override bool isEmpty() => this.date == null;

    public override bool isEqualTo(Magnitude time)
    {
      if (!(time is Time))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return this.date != null && this.date.Equals((object) ((Time) time).date);
    }

    public override bool isLessThan(Magnitude time)
    {
      if (!(time is Time))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return this.date != null && !time.isEmpty() && this.date.before(((Time) time).date);
    }

    public virtual long longValue() => this.date.getTime() / 1000L;

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      if (StringImpl.equals(StringImpl.trim(text), (object) ""))
      {
        this.clear();
      }
      else
      {
        text = StringImpl.trim(text);
        string lowerCase = StringImpl.toLowerCase(text);
        Calendar calendar = this.createCalendar();
        if (!StringImpl.equals(lowerCase, (object) "now"))
        {
          if (StringImpl.startsWith(lowerCase, "+"))
          {
            int num = Integer.valueOf(StringImpl.substring(lowerCase, 1)).intValue();
            calendar.setTime(this.date);
            calendar.add(11, num);
          }
          else if (StringImpl.startsWith(lowerCase, "-"))
          {
            int num = Integer.valueOf(StringImpl.substring(lowerCase, 1)).intValue();
            calendar.setTime(this.date);
            calendar.add(11, -num);
          }
          else
          {
            int length = 5;
            DateFormat[] dateFormatArray1 = length >= 0 ? new DateFormat[length] : throw new NegativeArraySizeException();
            dateFormatArray1[0] = Time.LONG_FORMAT;
            dateFormatArray1[1] = Time.MEDIUM_FORMAT;
            dateFormatArray1[2] = Time.SHORT_FORMAT;
            dateFormatArray1[3] = Time.ISO_LONG;
            dateFormatArray1[4] = Time.ISO_SHORT;
            DateFormat[] dateFormatArray2 = dateFormatArray1;
            for (int index = 0; index < dateFormatArray2.Length; ++index)
            {
              try
              {
                calendar.setTime(dateFormatArray2[index].parse(text));
                break;
              }
              catch (ParseException ex)
              {
                if (index + 1 == dateFormatArray2.Length)
                  throw new ValueParseException(new StringBuffer().append("Invalid time '").append(text).append("' for locale ").append((object) Locale.getDefault()).ToString(), (Throwable) ex);
              }
            }
          }
        }
        this.set(calendar);
      }
    }

    public virtual void reset() => this.setValue(new Date(Time.clock.getTime()));

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Integer.valueOf(StringImpl.substring(data, 0, 2)).intValue(), Integer.valueOf(StringImpl.substring(data, 2)).intValue());
    }

    public override string asEncodedString()
    {
      Calendar calendar = this.calendarValue();
      if (calendar == null)
        return "NULL";
      StringBuffer stringBuffer = new StringBuffer(4);
      int num1 = calendar.get(11);
      stringBuffer.append(num1 > 9 ? "" : "0");
      stringBuffer.append(num1);
      int num2 = calendar.get(12);
      stringBuffer.append(num2 > 9 ? "" : "0");
      stringBuffer.append(num2);
      return stringBuffer.ToString();
    }

    private void set(Calendar cal)
    {
      cal.set(14, 0);
      cal.set(13, 0);
      cal.set(5, 1);
      cal.set(2, 0);
      cal.set(1, 1970);
      this.date = cal.getTime();
    }

    public virtual void setValue(int hour, int minute)
    {
      this.checkTime(hour, minute, 0);
      Calendar calendar = this.createCalendar();
      calendar.setTimeZone(Time.timeZone);
      calendar.set(11, hour);
      calendar.set(12, minute);
      this.set(calendar);
    }

    public virtual void setValue(Date date)
    {
      if (date == null)
      {
        this.date = (Date) null;
      }
      else
      {
        Calendar instance = Calendar.getInstance();
        instance.setTime(date);
        this.set(instance);
      }
    }

    public virtual void setValue(long time)
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(new Date(time * 1000L));
      this.set(instance);
    }

    public virtual void setValue(Time time)
    {
      if (time == null || time.date == null)
        this.date = (Date) null;
      else
        this.date = new Date(time.date.getTime());
    }

    public override Title title() => new Title(this.date != null ? Time.SHORT_FORMAT.format(this.date) : "");

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Time()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
