// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Priority
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class Priority
  {
    [JavaFlags(0)]
    public int level;
    [JavaFlags(0)]
    public string levelStr;
    [JavaFlags(0)]
    public int syslogEquivalent;
    public const int OFF_INT = 2147483647;
    public const int FATAL_INT = 50000;
    public const int ERROR_INT = 40000;
    public const int WARN_INT = 30000;
    public const int INFO_INT = 20000;
    public const int DEBUG_INT = 10000;
    public const int ALL_INT = -2147483648;
    public static readonly Priority FATAL;
    public static readonly Priority ERROR;
    public static readonly Priority WARN;
    public static readonly Priority INFO;
    public static readonly Priority DEBUG;

    [JavaFlags(4)]
    public Priority(int level, string levelStr, int syslogEquivalent)
    {
      this.level = level;
      this.levelStr = levelStr;
      this.syslogEquivalent = syslogEquivalent;
    }

    public override bool Equals(object o) => o is Priority && this.level == ((Priority) o).level;

    [JavaFlags(17)]
    public int getSyslogEquivalent() => this.syslogEquivalent;

    public virtual bool isGreaterOrEqual(Priority r) => this.level >= r.level;

    [Obsolete(null, false)]
    public static Priority[] getAllPossiblePriorities()
    {
      int length = 5;
      Priority[] priorityArray = length >= 0 ? new Priority[length] : throw new NegativeArraySizeException();
      priorityArray[0] = Priority.FATAL;
      priorityArray[1] = Priority.ERROR;
      priorityArray[2] = (Priority) Level.WARN;
      priorityArray[3] = Priority.INFO;
      priorityArray[4] = Priority.DEBUG;
      return priorityArray;
    }

    [JavaFlags(17)]
    public override sealed string ToString() => this.levelStr;

    [JavaFlags(17)]
    public int toInt() => this.level;

    [Obsolete(null, false)]
    public static Priority toPriority(string sArg) => (Priority) Level.toLevel(sArg);

    public static Priority toPriority(int val) => Priority.toPriority(val, Priority.DEBUG);

    public static Priority toPriority(int val, Priority defaultPriority) => (Priority) Level.toLevel(val, (Level) defaultPriority);

    public static Priority toPriority(string sArg, Priority defaultPriority) => (Priority) Level.toLevel(sArg, (Level) defaultPriority);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Priority()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Priority priority = this;
      ObjectImpl.clone((object) priority);
      return ((object) priority).MemberwiseClone();
    }
  }
}
