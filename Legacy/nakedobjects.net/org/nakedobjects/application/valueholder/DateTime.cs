// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.DateTime
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.nakedobjects.application.system;
using System;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class DateTime : Magnitude
  {
    private const long serialVersionUID = 1;
    private static readonly DateFormat SHORT_FORMAT;
    private static readonly DateFormat MEDIUM_FORMAT;
    private static readonly DateFormat LONG_FORMAT;
    private static readonly DateFormat ISO_LONG;
    private static readonly DateFormat ISO_SHORT;
    [JavaFlags(130)]
    [NonSerialized]
    private DateFormat format;
    private bool isNull;
    private Date date;
    private static Clock clock;

    public static void setClock(Clock clock)
    {
      DateTime.clock = clock;
      DateTime.ISO_LONG.setLenient(false);
      DateTime.ISO_SHORT.setLenient(false);
      DateTime.LONG_FORMAT.setLenient(false);
      DateTime.MEDIUM_FORMAT.setLenient(false);
      DateTime.SHORT_FORMAT.setLenient(false);
    }

    public DateTime()
    {
      this.format = DateTime.MEDIUM_FORMAT;
      this.isNull = true;
      if (DateTime.clock == null)
        throw new org.nakedobjects.application.ApplicationException("Clock not set up");
      this.setValue(new Date(DateTime.clock.getTime()));
      this.isNull = false;
    }

    [Obsolete(null, false)]
    public DateTime(int year, int month, int day, int hour, int minute)
      : this(year, month, day, hour, minute, 0)
    {
    }

    public DateTime(int year, int month, int day, int hour, int minute, int second)
    {
      this.format = DateTime.MEDIUM_FORMAT;
      this.isNull = true;
      this.setValue(year, month, day, hour, minute, second);
      this.isNull = false;
    }

    public DateTime(DateTime timeStamp)
    {
      this.format = DateTime.MEDIUM_FORMAT;
      this.isNull = true;
      this.date = timeStamp.date;
      this.isNull = timeStamp.isNull;
    }

    public virtual void add(int hours, int minutes, int seconds)
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.add(13, seconds);
      instance.add(12, minutes);
      instance.add(11, hours);
      this.set(instance);
    }

    private void checkTime(int year, int month, int day, int hour, int minute, int second)
    {
      if (month < 1 || month > 12)
        throw new IllegalArgumentException(new StringBuffer().append("Month must be in the range 1 - 12 inclusive ").append(month).ToString());
      Calendar instance = Calendar.getInstance();
      instance.set(year, month - 1, 0);
      int maximum = instance.getMaximum(5);
      if (day < 1 || day > maximum)
        throw new IllegalArgumentException(new StringBuffer().append("Day must be in the range 1 - ").append(maximum).append(" inclusive ").append(day).ToString());
      if (hour < 0 || hour > 23)
        throw new IllegalArgumentException(new StringBuffer().append("Hour must be in the range 0 - 23 inclusive ").append(hour).ToString());
      if (minute < 0 || minute > 59)
        throw new IllegalArgumentException(new StringBuffer().append("Minute must be in the range 0 - 59 inclusive ").append(minute).ToString());
      if (second < 0 || second > 59)
        throw new IllegalArgumentException(new StringBuffer().append("Second must be in the range 0 - 59 inclusive ").append(second).ToString());
    }

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      this.date = @object is DateTime ? ((DateTime) @object).date : throw new IllegalArgumentException("Can only copy the value of  a TimeStamp object");
      this.isNull = ((DateTime) @object).isNull;
    }

    private Calendar createCalendar()
    {
      Calendar instance = Calendar.getInstance();
      instance.set(14, 0);
      return instance;
    }

    public virtual Date dateValue() => this.isNull ? (Date) null : this.date;

    public override bool Equals(object obj) => obj is DateTime ? ((DateTime) obj).date.Equals((object) this.date) : base.Equals(obj);

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude timeStamp)
    {
      if (!(timeStamp is DateTime))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return this.isNull ? timeStamp.isEmpty() : this.date.Equals((object) ((DateTime) timeStamp).date);
    }

    public override bool isLessThan(Magnitude timeStamp)
    {
      if (!(timeStamp is DateTime))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return !this.isNull && !timeStamp.isEmpty() && this.date.before(((DateTime) timeStamp).date);
    }

    public virtual int getDay()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance.get(5);
    }

    public virtual int getMonth()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance.get(2) + 1;
    }

    public virtual int getYear()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance.get(1);
    }

    public virtual int getHour()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance.get(10);
    }

    public virtual int getMinute()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance.get(12);
    }

    public virtual long longValue() => this.date.getTime();

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
        if (!StringImpl.equals(lowerCase, (object) "today") && !StringImpl.equals(lowerCase, (object) "now"))
        {
          if (StringImpl.startsWith(lowerCase, "+"))
          {
            int num = Integer.valueOf(StringImpl.substring(lowerCase, 1)).intValue();
            calendar.setTime(this.date);
            calendar.add(10, num);
          }
          else if (StringImpl.startsWith(lowerCase, "-"))
          {
            int num = Integer.valueOf(StringImpl.substring(lowerCase, 1)).intValue();
            calendar.setTime(this.date);
            calendar.add(10, -num);
          }
          else
          {
            int length = 5;
            DateFormat[] dateFormatArray1 = length >= 0 ? new DateFormat[length] : throw new NegativeArraySizeException();
            dateFormatArray1[0] = DateTime.LONG_FORMAT;
            dateFormatArray1[1] = DateTime.MEDIUM_FORMAT;
            dateFormatArray1[2] = DateTime.SHORT_FORMAT;
            dateFormatArray1[3] = DateTime.ISO_LONG;
            dateFormatArray1[4] = DateTime.ISO_SHORT;
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
                  throw new ValueParseException(new StringBuffer().append("Invalid timeStamp ").append(text).ToString(), (Throwable) ex);
              }
            }
          }
        }
        this.set(calendar);
        this.isNull = false;
      }
    }

    public virtual void reset()
    {
      this.date = new Date(DateTime.clock.getTime());
      this.isNull = false;
    }

    private void set(Calendar cal) => this.date = cal.getTime();

    public virtual void setValue(Date date)
    {
      if (date == null)
      {
        this.isNull = true;
      }
      else
      {
        Calendar instance = Calendar.getInstance();
        instance.setTime(date);
        instance.set(14, 0);
        this.date = instance.getTime();
      }
    }

    public virtual void setValue(long time)
    {
      this.isNull = false;
      this.date.setTime(time);
    }

    public virtual void setValue(DateTime timeStamp)
    {
      if (timeStamp == null)
        this.isNull = true;
      else
        this.date = new Date(timeStamp.date.getTime());
    }

    public virtual void setValue(int year, int month, int day, int hour, int minute, int second)
    {
      this.checkTime(year, month, day, hour, minute, second);
      Calendar calendar = this.createCalendar();
      calendar.set(5, day);
      calendar.set(2, month - 1);
      calendar.set(1, year);
      calendar.set(11, hour);
      calendar.set(12, minute);
      calendar.set(13, second);
      calendar.set(14, 0);
      this.set(calendar);
    }

    public override Title title() => new Title(!this.isNull ? this.format.format(this.date) : "");

    public virtual Calendar calendarValue()
    {
      if (this.isNull)
        return (Calendar) null;
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance;
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Integer.valueOf(StringImpl.substring(data, 0, 4)).intValue(), Integer.valueOf(StringImpl.substring(data, 4, 6)).intValue(), Integer.valueOf(StringImpl.substring(data, 6, 8)).intValue(), Integer.valueOf(StringImpl.substring(data, 8, 10)).intValue(), Integer.valueOf(StringImpl.substring(data, 10, 12)).intValue(), Integer.valueOf(StringImpl.substring(data, 12, 14)).intValue());
    }

    public override string asEncodedString()
    {
      if (this.isEmpty())
        return "NULL";
      Calendar calendar = this.calendarValue();
      StringBuffer stringBuffer = new StringBuffer(8);
      string str = StringImpl.valueOf(calendar.get(1));
      stringBuffer.append(StringImpl.substring("0000", 0, 4 - StringImpl.length(str)));
      stringBuffer.append(str);
      int num1 = calendar.get(2) + 1;
      stringBuffer.append(num1 > 9 ? "" : "0");
      stringBuffer.append(num1);
      int num2 = calendar.get(5);
      stringBuffer.append(num2 > 9 ? "" : "0");
      stringBuffer.append(num2);
      int num3 = calendar.get(11);
      stringBuffer.append(num3 > 9 ? "" : "0");
      stringBuffer.append(num3);
      int num4 = calendar.get(12);
      stringBuffer.append(num4 > 9 ? "" : "0");
      stringBuffer.append(num4);
      int num5 = calendar.get(13);
      stringBuffer.append(num5 > 9 ? "" : "0");
      stringBuffer.append(num5);
      return stringBuffer.ToString();
    }

    public override string ToString() => new StringBuffer().append((object) this.title()).append(" ").append(this.longValue()).append(" [DateTime]").ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DateTime()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
