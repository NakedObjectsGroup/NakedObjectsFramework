// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.SystemTime
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.performance
{
  public class SystemTime
  {
    [JavaFlags(8)]
    public static int RUN_LENGTH;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      double num1 = SystemTime.systemCurrentTimeLoop();
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Average System.currentTimeMillis() call took ").append(num1).ToString());
      double num2 = SystemTime.currentThreadNameloop();
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Average Thread.currentThread().getName() call took ").append(num2).ToString());
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static double systemCurrentTimeLoop()
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; index < SystemTime.RUN_LENGTH; ++index)
        java.lang.System.currentTimeMillis();
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) SystemTime.RUN_LENGTH;
    }

    [JavaFlags(8)]
    public static double currentThreadNameloop()
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; index < SystemTime.RUN_LENGTH; ++index)
        Thread.currentThread().getName();
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) SystemTime.RUN_LENGTH;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SystemTime()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SystemTime systemTime = this;
      ObjectImpl.clone((object) systemTime);
      return ((object) systemTime).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
