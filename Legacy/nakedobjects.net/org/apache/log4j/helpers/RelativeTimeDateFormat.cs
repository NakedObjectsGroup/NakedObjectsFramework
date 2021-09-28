// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.RelativeTimeDateFormat
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;

namespace org.apache.log4j.helpers
{
  public class RelativeTimeDateFormat : DateFormat
  {
    [JavaFlags(20)]
    public readonly long startTime;

    public RelativeTimeDateFormat() => this.startTime = System.currentTimeMillis();

    public virtual StringBuffer format(
      Date date,
      StringBuffer sbuf,
      FieldPosition fieldPosition)
    {
      return sbuf.append(date.getTime() - this.startTime);
    }

    public virtual Date parse(string s, ParsePosition pos) => (Date) null;

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
