// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.RollingCalendar
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;

namespace org.apache.log4j
{
  [JavaFlags(32)]
  public class RollingCalendar : GregorianCalendar
  {
    [JavaFlags(0)]
    public int type;

    [JavaFlags(0)]
    public RollingCalendar() => this.type = -1;

    [JavaFlags(0)]
    public RollingCalendar(TimeZone tz, Locale locale)
      : base(tz, locale)
    {
      this.type = -1;
    }

    [JavaFlags(0)]
    public virtual void setType(int type) => this.type = type;

    public virtual long getNextCheckMillis(Date now) => this.getNextCheckDate(now).getTime();

    public virtual Date getNextCheckDate(Date now)
    {
      ((Calendar) this).setTime(now);
      switch (this.type)
      {
        case 0:
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          this.add(12, 1);
          break;
        case 1:
          ((Calendar) this).set(12, 0);
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          this.add(11, 1);
          break;
        case 2:
          ((Calendar) this).set(12, 0);
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          if (((Calendar) this).get(11) < 12)
          {
            ((Calendar) this).set(11, 12);
            break;
          }
          ((Calendar) this).set(11, 0);
          this.add(5, 1);
          break;
        case 3:
          ((Calendar) this).set(11, 0);
          ((Calendar) this).set(12, 0);
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          this.add(5, 1);
          break;
        case 4:
          ((Calendar) this).set(7, ((Calendar) this).getFirstDayOfWeek());
          ((Calendar) this).set(11, 0);
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          this.add(3, 1);
          break;
        case 5:
          ((Calendar) this).set(5, 1);
          ((Calendar) this).set(11, 0);
          ((Calendar) this).set(13, 0);
          ((Calendar) this).set(14, 0);
          this.add(2, 1);
          break;
        default:
          throw new IllegalStateException("Unknown periodicity type.");
      }
      return ((Calendar) this).getTime();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
