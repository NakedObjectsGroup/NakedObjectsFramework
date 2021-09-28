// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.value.Date
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using org.nakedobjects.application.system;
using System.ComponentModel;

namespace org.nakedobjects.application.value
{
  public class Date : Magnitude
  {
    private static Clock clock;
    private static readonly DateFormat ISO_LONG;
    private static readonly DateFormat ISO_SHORT;
    private static readonly DateFormat LONG_FORMAT;
    private static readonly DateFormat MEDIUM_FORMAT;
    private const long serialVersionUID = 1;
    private static readonly DateFormat SHORT_FORMAT;
    private Date date;

    public static void setClock(Clock clock)
    {
      Date.clock = clock;
      Date.ISO_LONG.setLenient(false);
      Date.ISO_SHORT.setLenient(false);
      Date.LONG_FORMAT.setLenient(false);
      Date.MEDIUM_FORMAT.setLenient(false);
      Date.SHORT_FORMAT.setLenient(false);
    }

    public Date() => this.today();

    public Date(Date date) => this.setValue(date);

    public Date(int year, int month, int day) => this.setValue(year, month, day);

    public virtual void add(int years, int months, int days)
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.add(5, days);
      instance.add(2, months);
      instance.add(1, years);
      this.set(instance);
    }

    public virtual string asEncodedString()
    {
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
      return stringBuffer.ToString();
    }

    public virtual Calendar calendarValue()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      return instance;
    }

    private void checkDate(int year, int month, int day)
    {
      if (month < 1 || month > 12)
        throw new IllegalArgumentException("Month must be in the range 1 - 12 inclusive");
      Calendar instance = Calendar.getInstance();
      instance.set(year, month - 1, 0);
      int maximum = instance.getMaximum(5);
      if (day < 1 || day > maximum)
        throw new IllegalArgumentException(new StringBuffer().append("Day must be in the range 1 - ").append(maximum).append(" inclusive: ").append(day).ToString());
    }

    private void clearDate(Calendar cal)
    {
      cal.set(10, 0);
      cal.set(11, 0);
      cal.set(12, 0);
      cal.set(13, 0);
      cal.set(9, 0);
      cal.set(14, 0);
    }

    public virtual Date dateValue() => new Date(this.date.getTime());

    public override bool Equals(object obj) => obj is Date ? ((Date) obj).date.Equals((object) this.date) : base.Equals(obj);

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

    public override bool isEqualTo(Magnitude date) => date is Date ? this.date.Equals((object) ((Date) date).date) : throw new IllegalArgumentException("Parameter must be of type Time");

    public override bool isLessThan(Magnitude date) => date is Date ? this.date.before(((Date) date).date) : throw new IllegalArgumentException("Parameter must be of type Time");

    public virtual long longValue() => this.date.getTime();

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public virtual void parseUserEntry(string dateString)
    {
      dateString = StringImpl.trim(dateString);
      string str = StringImpl.toLowerCase(dateString);
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      this.clearDate(instance);
      if (StringImpl.equals(str, (object) "today") || StringImpl.equals(str, (object) "now"))
      {
        this.today();
      }
      else
      {
        if (StringImpl.startsWith(str, "+"))
        {
          int num1 = 5;
          int num2 = 1;
          if (StringImpl.endsWith(str, "w"))
          {
            num2 = 7;
            str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
          }
          else if (StringImpl.endsWith(str, "m"))
          {
            num1 = 2;
            str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
          }
          int num3 = Integer.valueOf(StringImpl.substring(str, 1)).intValue();
          instance.setTime(this.date);
          instance.add(num1, num3 * num2);
        }
        else if (StringImpl.startsWith(str, "-"))
        {
          int num4 = 5;
          int num5 = 1;
          if (StringImpl.endsWith(str, "w"))
          {
            num5 = 7;
            str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
          }
          else if (StringImpl.endsWith(str, "m"))
          {
            num4 = 2;
            str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
          }
          int num6 = Integer.valueOf(StringImpl.substring(str, 1)).intValue();
          instance.setTime(this.date);
          instance.add(num4, -num6 * num5);
        }
        else
        {
          int length = 5;
          DateFormat[] dateFormatArray1 = length >= 0 ? new DateFormat[length] : throw new NegativeArraySizeException();
          dateFormatArray1[0] = Date.LONG_FORMAT;
          dateFormatArray1[1] = Date.MEDIUM_FORMAT;
          dateFormatArray1[2] = Date.SHORT_FORMAT;
          dateFormatArray1[3] = Date.ISO_LONG;
          dateFormatArray1[4] = Date.ISO_SHORT;
          DateFormat[] dateFormatArray2 = dateFormatArray1;
          for (int index = 0; index < dateFormatArray2.Length; ++index)
          {
            try
            {
              instance.setTime(dateFormatArray2[index].parse(dateString));
              break;
            }
            catch (ParseException ex)
            {
              if (index + 1 == dateFormatArray2.Length)
                throw new ValueParseException(new StringBuffer().append("Invalid date ").append(dateString).ToString(), (Throwable) ex);
            }
          }
        }
        this.set(instance);
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;java/lang/ClassNotFoundException;")]
    public virtual void readExternal(ObjectInput @in) => this.date.setTime(((DataInput) @in).readLong());

    public virtual void reset() => this.today();

    public virtual void restoreFromEncodedString(string data) => this.setValue(Integer.valueOf(StringImpl.substring(data, 0, 4)).intValue(), Integer.valueOf(StringImpl.substring(data, 4, 6)).intValue(), Integer.valueOf(StringImpl.substring(data, 6)).intValue());

    private bool sameAs(Date @as, int field)
    {
      Calendar instance1 = Calendar.getInstance();
      instance1.setTime(this.date);
      Calendar instance2 = Calendar.getInstance();
      instance2.setTime(@as.date);
      return instance1.get(field) == instance2.get(field);
    }

    public virtual bool sameDayAs(Date @as) => this.sameAs(@as, 6);

    public virtual bool sameMonthAs(Date @as) => this.sameAs(@as, 2);

    public virtual bool sameWeekAs(Date @as) => this.sameAs(@as, 3);

    public virtual bool sameYearAs(Date @as) => this.sameAs(@as, 1);

    private void set(Calendar cal) => this.date = cal.getTime();

    public virtual void setValue(Date date) => this.setValue(new Date(date.longValue()));

    public virtual void setValue(int year, int month, int day)
    {
      this.checkDate(year, month, day);
      Calendar instance = Calendar.getInstance();
      this.clearDate(instance);
      instance.set(year, month - 1, day);
      this.set(instance);
    }

    public virtual void setValue(Date date)
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(date);
      instance.set(10, 0);
      instance.set(12, 0);
      instance.set(13, 0);
      instance.set(14, 0);
      this.set(instance);
    }

    public virtual string title() => Date.MEDIUM_FORMAT.format(this.date);

    public virtual void today()
    {
      Calendar instance = Calendar.getInstance();
      Date date = Date.clock != null ? new Date(Date.clock.getTime()) : throw new ApplicationException("Clock not set up");
      instance.setTime(date);
      this.clearDate(instance);
      this.set(instance);
    }

    public virtual void toStartOfMonth()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.set(2, instance.getMinimum(2));
      this.date = instance.getTime();
    }

    public virtual void toStartOfWeek()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.set(8, instance.getMinimum(8));
      this.date = instance.getTime();
    }

    public virtual void toStartOfYear()
    {
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      instance.set(6, instance.getMinimum(6));
      this.date = instance.getTime();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Date()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
