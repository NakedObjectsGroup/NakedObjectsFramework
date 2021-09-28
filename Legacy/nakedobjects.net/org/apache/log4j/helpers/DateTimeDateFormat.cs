// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.DateTimeDateFormat
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.text;
using java.util;

namespace org.apache.log4j.helpers
{
  public class DateTimeDateFormat : AbsoluteTimeDateFormat
  {
    [JavaFlags(0)]
    public string[] shortMonths;

    public DateTimeDateFormat() => this.shortMonths = new DateFormatSymbols().getShortMonths();

    public DateTimeDateFormat(TimeZone timeZone)
      : this()
    {
      this.setCalendar(Calendar.getInstance(timeZone));
    }

    public override StringBuffer format(
      Date date,
      StringBuffer sbuf,
      FieldPosition fieldPosition)
    {
      ((Calendar) this.calendar).setTime(date);
      int num1 = ((Calendar) this.calendar).get(5);
      if (num1 < 10)
        sbuf.append('0');
      sbuf.append(num1);
      sbuf.append(' ');
      sbuf.append(this.shortMonths[((Calendar) this.calendar).get(2)]);
      sbuf.append(' ');
      int num2 = ((Calendar) this.calendar).get(1);
      sbuf.append(num2);
      sbuf.append(' ');
      return base.format(date, sbuf, fieldPosition);
    }

    public override Date parse(string s, ParsePosition pos) => (Date) null;
  }
}
