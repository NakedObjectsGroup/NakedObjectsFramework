// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.TimeStamp
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.nakedobjects.application.system;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class TimeStamp : Magnitude
  {
    private static readonly DateFormat ISO_LONG;
    private bool isNull;
    private Date date;
    private static Clock clock;

    public static void setClock(Clock clock) => TimeStamp.clock = clock;

    public TimeStamp()
    {
      this.isNull = true;
      if (TimeStamp.clock == null)
        throw new ApplicationException("Clock not set up");
      this.reset();
      this.isNull = false;
    }

    public TimeStamp(TimeStamp timeStamp)
    {
      this.isNull = true;
      this.date = timeStamp.date;
      this.isNull = timeStamp.isNull;
    }

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      this.date = @object is TimeStamp ? ((TimeStamp) @object).date : throw new IllegalArgumentException("Can only copy the value of  a TimeStamp object");
      this.isNull = ((TimeStamp) @object).isNull;
    }

    private Calendar createCalendar() => Calendar.getInstance();

    public virtual Date dateValue() => this.isNull ? (Date) null : this.date;

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude timeStamp)
    {
      if (!(timeStamp is TimeStamp))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return this.isNull ? timeStamp.isEmpty() : this.date.Equals((object) ((TimeStamp) timeStamp).date);
    }

    public override bool isLessThan(Magnitude timeStamp)
    {
      if (!(timeStamp is TimeStamp))
        throw new IllegalArgumentException("Parameter must be of type Time");
      return !this.isNull && !timeStamp.isEmpty() && this.date.before(((TimeStamp) timeStamp).date);
    }

    public virtual long longValue() => this.date.getTime();

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
    }

    public virtual void reset()
    {
      this.date = new Date(TimeStamp.clock.getTime());
      this.isNull = false;
    }

    private void set(Calendar cal) => this.date = cal.getTime();

    public override Title title() => new Title(!this.isNull ? TimeStamp.ISO_LONG.format(this.date) : "");

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
      {
        this.clear();
      }
      else
      {
        int num1 = Integer.valueOf(StringImpl.substring(data, 0, 4)).intValue();
        int num2 = Integer.valueOf(StringImpl.substring(data, 4, 6)).intValue();
        int num3 = Integer.valueOf(StringImpl.substring(data, 6, 8)).intValue();
        int num4 = Integer.valueOf(StringImpl.substring(data, 8, 10)).intValue();
        int num5 = Integer.valueOf(StringImpl.substring(data, 10, 12)).intValue();
        int num6 = Integer.valueOf(StringImpl.substring(data, 12, 14)).intValue();
        int num7 = Integer.valueOf(StringImpl.substring(data, 14, 17)).intValue();
        Calendar calendar = this.createCalendar();
        calendar.set(5, num3);
        calendar.set(2, num2 - 1);
        calendar.set(1, num1);
        calendar.set(11, num4);
        calendar.set(12, num5);
        calendar.set(13, num6);
        calendar.set(14, num7);
        this.set(calendar);
      }
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
      int num6 = calendar.get(14);
      stringBuffer.append(num6 > 99 ? "" : "0");
      stringBuffer.append(num6 > 9 ? "" : "0");
      stringBuffer.append(num6);
      return stringBuffer.ToString();
    }

    public override string ToString() => new StringBuffer().append((object) this.title()).append(" ").append(this.longValue()).append(" [TimeStamp]").ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TimeStamp()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
