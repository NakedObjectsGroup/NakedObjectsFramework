// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.system.ExplorationClock
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System;
using System.Globalization;

namespace Legacy.NakedObjects.Application.System
{
  public class ExplorationClock : SystemClock
  { 
      //private Calendar time;

    public static ExplorationClock initialize() {
        return new ExplorationClock();
    }

    public override long getTime() {
            //return this.time == null ? base.getTime() : this.time.getTime().getTime();
            throw new NotImplementedException();
        }

    public virtual void setTime(int hour, int min)
    {
      //this.getCalendar();
      //this.time.set(11, hour);
      //this.time.set(12, min);
    }

    public virtual void setDate(int year, int month, int day)
    {
      //this.getCalendar();
      //this.time.set(1, year);
      //this.time.set(2, month - 1);
      //this.time.set(5, day);
    }

    private void getCalendar()
    {
      //if (this.time != null)
      //  return;
      //this.time = Calendar.getInstance();
    }

    public virtual void reset() {
       // this.time = (Calendar)null;
    }

    public override string ToString() {
            //  return this.time.getTime().ToString();
            throw new NotImplementedException();
        }

    public ExplorationClock() {
       // this.time = (Calendar)null;
    }
  }
}
