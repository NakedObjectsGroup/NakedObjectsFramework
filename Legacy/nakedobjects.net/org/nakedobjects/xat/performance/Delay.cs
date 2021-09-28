// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.Delay
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.util;
using java.lang;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.xat.performance
{
  public class Delay : Profiler
  {
    private static bool addDelay;

    public Delay(string name)
      : base(name)
    {
    }

    public static void setAddDelay(bool on) => Delay.addDelay = on;

    public static void userDelay(int min, int max)
    {
      if (!Delay.addDelay)
        return;
      int num = min * 1000 + Utilities.doubleToInt((double) (max - min) * Math.random() * 1000.0);
      try
      {
        Thread.sleep((long) num);
      }
      catch (InterruptedException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Delay()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
