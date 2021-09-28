// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.AbsoluteTimeDateFormat
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class AbsoluteTimeDateFormat : DateFormat
  {
    public const string ABS_TIME_DATE_FORMAT = "ABSOLUTE";
    public const string DATE_AND_TIME_DATE_FORMAT = "DATE";
    public const string ISO8601_DATE_FORMAT = "ISO8601";
    private static long previousTime;
    private static char[] previousTimeWithoutMillis;

    public AbsoluteTimeDateFormat() => this.setCalendar(Calendar.getInstance());

    public AbsoluteTimeDateFormat(TimeZone timeZone) => this.setCalendar(Calendar.getInstance(timeZone));

    public virtual StringBuffer format(
      Date date,
      StringBuffer sbuf,
      FieldPosition fieldPosition)
    {
      long time = date.getTime();
      int num1 = (int) (time % 1000L);
      if (time - (long) num1 != AbsoluteTimeDateFormat.previousTime)
      {
        ((Calendar) this.calendar).setTime(date);
        int num2 = sbuf.length();
        int num3 = ((Calendar) this.calendar).get(11);
        if (num3 < 10)
          sbuf.append('0');
        sbuf.append(num3);
        sbuf.append(':');
        int num4 = ((Calendar) this.calendar).get(12);
        if (num4 < 10)
          sbuf.append('0');
        sbuf.append(num4);
        sbuf.append(':');
        int num5 = ((Calendar) this.calendar).get(13);
        if (num5 < 10)
          sbuf.append('0');
        sbuf.append(num5);
        sbuf.append(',');
        sbuf.getChars(num2, sbuf.length(), AbsoluteTimeDateFormat.previousTimeWithoutMillis, 0);
        AbsoluteTimeDateFormat.previousTime = time - (long) num1;
      }
      else
        sbuf.append(AbsoluteTimeDateFormat.previousTimeWithoutMillis);
      if (num1 < 100)
        sbuf.append('0');
      if (num1 < 10)
        sbuf.append('0');
      sbuf.append(num1);
      return sbuf;
    }

    public virtual Date parse(string s, ParsePosition pos) => (Date) null;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static AbsoluteTimeDateFormat()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
