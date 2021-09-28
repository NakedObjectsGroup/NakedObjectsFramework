// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.NotLogging
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
  public class NotLogging
  {
    [JavaFlags(8)]
    public static int runLength;
    [JavaFlags(24)]
    public const int INITIAL_HASH_SIZE = 101;
    [JavaFlags(8)]
    public static string SHORT_MSG;
    [JavaFlags(8)]
    public static Category SHORT_CAT;
    [JavaFlags(8)]
    public static Category MEDIUM_CAT;
    [JavaFlags(8)]
    public static Category LONG_CAT;
    [JavaFlags(8)]
    public static Category INEXISTENT_SHORT_CAT;
    [JavaFlags(8)]
    public static Category INEXISTENT_MEDIUM_CAT;
    [JavaFlags(8)]
    public static Category INEXISTENT_LONG_CAT;
    [JavaFlags(8)]
    public static Category[] CAT_ARRAY;

    [JavaFlags(8)]
    public static void Usage()
    {
      ((PrintStream) java.lang.System.err).println("Usage: java org.apache.log4j.test.NotLogging true|false runLength\ntrue indicates shipped code, false indicates code in development  where runLength is an int representing the run length of loops\nWe suggest that runLength be at least 100'000.");
      java.lang.System.exit(1);
    }

    public static void main(string[] argv)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      if (argv.Length != 2)
        NotLogging.Usage();
      NotLogging.ProgramInit(argv);
      ((PrintStream) java.lang.System.@out).println();
      for (int index = 0; index < NotLogging.CAT_ARRAY.Length; ++index)
      {
        double num = NotLogging.SimpleMessage(NotLogging.CAT_ARRAY[index], NotLogging.SHORT_MSG, (long) NotLogging.runLength);
        ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Simple argument,          ").append(num).append(" micros. Cat: ").append(NotLogging.CAT_ARRAY[index].getName()).ToString());
      }
      ((PrintStream) java.lang.System.@out).println();
      for (int index = 0; index < NotLogging.CAT_ARRAY.Length; ++index)
      {
        double num = NotLogging.FullyOptimizedComplexMessage(NotLogging.CAT_ARRAY[index], (long) NotLogging.runLength);
        ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Fully optimized complex,  ").append(num).append(" micros. Cat: ").append(NotLogging.CAT_ARRAY[index].getName()).ToString());
      }
      ((PrintStream) java.lang.System.@out).println();
      for (int index = 0; index < NotLogging.CAT_ARRAY.Length; ++index)
      {
        double num = NotLogging.ComplexMessage(NotLogging.CAT_ARRAY[index], (long) NotLogging.runLength);
        ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Complex message argument, ").append(num).append(" micros. Cat: ").append(NotLogging.CAT_ARRAY[index].getName()).ToString());
      }
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static void ProgramInit(string[] args)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static double SimpleMessage(Category category, string msg, long runLength)
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; (long) index < runLength; ++index)
        category.debug((object) msg);
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) runLength;
    }

    [JavaFlags(8)]
    public static double FullyOptimizedComplexMessage(Category category, long runLength)
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; (long) index < runLength; ++index)
      {
        if (category.isDebugEnabled())
          category.debug((object) new StringBuffer().append("Message").append(index).append(" bottles of beer standing on the wall.").ToString());
      }
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) runLength;
    }

    [JavaFlags(8)]
    public static double ComplexMessage(Category category, long runLength)
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; (long) index < runLength; ++index)
        category.debug((object) new StringBuffer().append("Message").append(index).append(" bottles of beer standing on the wall.").ToString());
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) runLength;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NotLogging()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NotLogging notLogging = this;
      ObjectImpl.clone((object) notLogging);
      return ((object) notLogging).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
