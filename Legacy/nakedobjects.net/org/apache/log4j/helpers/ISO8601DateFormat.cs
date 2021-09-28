// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.ISO8601DateFormat
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class ISO8601DateFormat : AbsoluteTimeDateFormat
  {
    private static long lastTime;
    private static char[] lastTimeString;

    public ISO8601DateFormat()
    {
    }

    public ISO8601DateFormat(TimeZone timeZone)
      : base(timeZone)
    {
    }

    public override StringBuffer format(
      Date date,
      StringBuffer sbuf,
      FieldPosition fieldPosition)
    {
      long time = date.getTime();
      int num1 = (int) (time % 1000L);
      if (time - (long) num1 != ISO8601DateFormat.lastTime)
      {
        ((Calendar) this.calendar).setTime(date);
        int num2 = sbuf.length();
        int num3 = ((Calendar) this.calendar).get(1);
        sbuf.append(num3);
        string str;
        switch (((Calendar) this.calendar).get(2))
        {
          case 0:
            str = "-01-";
            break;
          case 1:
            str = "-02-";
            break;
          case 2:
            str = "-03-";
            break;
          case 3:
            str = "-04-";
            break;
          case 4:
            str = "-05-";
            break;
          case 5:
            str = "-06-";
            break;
          case 6:
            str = "-07-";
            break;
          case 7:
            str = "-08-";
            break;
          case 8:
            str = "-09-";
            break;
          case 9:
            str = "-10-";
            break;
          case 10:
            str = "-11-";
            break;
          case 11:
            str = "-12-";
            break;
          default:
            str = "-NA-";
            break;
        }
        sbuf.append(str);
        int num4 = ((Calendar) this.calendar).get(5);
        if (num4 < 10)
          sbuf.append('0');
        sbuf.append(num4);
        sbuf.append(' ');
        int num5 = ((Calendar) this.calendar).get(11);
        if (num5 < 10)
          sbuf.append('0');
        sbuf.append(num5);
        sbuf.append(':');
        int num6 = ((Calendar) this.calendar).get(12);
        if (num6 < 10)
          sbuf.append('0');
        sbuf.append(num6);
        sbuf.append(':');
        int num7 = ((Calendar) this.calendar).get(13);
        if (num7 < 10)
          sbuf.append('0');
        sbuf.append(num7);
        sbuf.append(',');
        sbuf.getChars(num2, sbuf.length(), ISO8601DateFormat.lastTimeString, 0);
        ISO8601DateFormat.lastTime = time - (long) num1;
      }
      else
        sbuf.append(ISO8601DateFormat.lastTimeString);
      if (num1 < 100)
        sbuf.append('0');
      if (num1 < 10)
        sbuf.append('0');
      sbuf.append(num1);
      return sbuf;
    }

    public override Date parse(string s, ParsePosition pos) => (Date) null;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ISO8601DateFormat()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
