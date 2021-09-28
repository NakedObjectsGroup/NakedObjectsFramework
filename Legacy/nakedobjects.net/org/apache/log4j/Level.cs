// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Level
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class Level : Priority
  {
    public static readonly Level OFF;
    public static readonly Level FATAL;
    public static readonly Level ERROR;
    public static readonly Level WARN;
    public static readonly Level INFO;
    public static readonly Level DEBUG;
    public static readonly Level ALL;

    [JavaFlags(4)]
    public Level(int level, string levelStr, int syslogEquivalent)
      : base(level, levelStr, syslogEquivalent)
    {
    }

    public static Level toLevel(string sArg) => Level.toLevel(sArg, Level.DEBUG);

    public static Level toLevel(int val) => Level.toLevel(val, Level.DEBUG);

    public static Level toLevel(int val, Level defaultLevel)
    {
      switch (val)
      {
        case int.MinValue:
          return Level.ALL;
        case 10000:
          return Level.DEBUG;
        case 20000:
          return Level.INFO;
        case 30000:
          return Level.WARN;
        case 40000:
          return Level.ERROR;
        case 50000:
          return Level.FATAL;
        case int.MaxValue:
          return Level.OFF;
        default:
          return defaultLevel;
      }
    }

    public static Level toLevel(string sArg, Level defaultLevel)
    {
      if (sArg == null)
        return defaultLevel;
      string upperCase = StringImpl.toUpperCase(sArg);
      if (StringImpl.equals(upperCase, (object) "ALL"))
        return Level.ALL;
      if (StringImpl.equals(upperCase, (object) "DEBUG"))
        return Level.DEBUG;
      if (StringImpl.equals(upperCase, (object) "INFO"))
        return Level.INFO;
      if (StringImpl.equals(upperCase, (object) "WARN"))
        return Level.WARN;
      if (StringImpl.equals(upperCase, (object) "ERROR"))
        return Level.ERROR;
      if (StringImpl.equals(upperCase, (object) "FATAL"))
        return Level.FATAL;
      return StringImpl.equals(upperCase, (object) "OFF") ? Level.OFF : defaultLevel;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Level()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
