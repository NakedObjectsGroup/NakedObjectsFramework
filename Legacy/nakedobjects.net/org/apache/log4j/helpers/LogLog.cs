// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.LogLog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class LogLog
  {
    public const string DEBUG_KEY = "log4j.debug";
    [Obsolete(null, false)]
    public const string CONFIG_DEBUG_KEY = "log4j.configDebug";
    [JavaFlags(12)]
    public static bool debugEnabled;
    private static bool quietMode;
    private const string PREFIX = "log4j: ";
    private const string ERR_PREFIX = "log4j:ERROR ";
    private const string WARN_PREFIX = "log4j:WARN ";

    public static void setInternalDebugging(bool enabled) => LogLog.debugEnabled = enabled;

    public static void debug(string msg)
    {
      if (!LogLog.debugEnabled || LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("log4j: ").append(msg).ToString());
    }

    public static void debug(string msg, Throwable t)
    {
      if (!LogLog.debugEnabled || LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("log4j: ").append(msg).ToString());
      t?.printStackTrace((PrintStream) java.lang.System.@out);
    }

    public static void error(string msg)
    {
      if (LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("log4j:ERROR ").append(msg).ToString());
    }

    public static void error(string msg, Throwable t)
    {
      if (LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("log4j:ERROR ").append(msg).ToString());
      t?.printStackTrace();
    }

    public static void setQuietMode(bool quietMode) => LogLog.quietMode = quietMode;

    public static void warn(string msg)
    {
      if (LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("log4j:WARN ").append(msg).ToString());
    }

    public static void warn(string msg, Throwable t)
    {
      if (LogLog.quietMode)
        return;
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("log4j:WARN ").append(msg).ToString());
      t?.printStackTrace();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static LogLog()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      LogLog logLog = this;
      ObjectImpl.clone((object) logLog);
      return ((object) logLog).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
