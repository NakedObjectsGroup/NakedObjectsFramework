// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.Profiler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;
using System.Threading;

namespace org.nakedobjects.utility
{
  public class Profiler
  {
    private const string DELIMITER = "\t";
    private static NumberFormat floatFormat;
    private static NumberFormat integerFormat;
    private static int nextId;
    private static int nextThread;
    [JavaFlags(12)]
    public static ProfilerSystem profilerSystem;
    private static Hashtable threads;
    private long elapsedTime;
    private readonly int id;
    private long memory;
    private readonly string name;
    private long start;
    private readonly string thread;
    private bool timing;

    public static string memoryLog()
    {
      long num = Profiler.memory();
      return new StringBuffer().append(Profiler.integerFormat.format(num)).append(" bytes").ToString();
    }

    private static long memory() => Profiler.profilerSystem.memory();

    private static long time() => Profiler.profilerSystem.time();

    public static void setProfilerSystem(ProfilerSystem profilerSystem) => Profiler.profilerSystem = profilerSystem;

    public Profiler(string name)
    {
      this.elapsedTime = 0L;
      this.start = 0L;
      this.timing = false;
      this.name = name;
      object obj = (object) Class.FromType(typeof (Profiler));
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        int nextId;
        Profiler.nextId = (nextId = Profiler.nextId) + 1;
        this.id = nextId;
      }
      finally
      {
        Monitor.Exit(obj);
      }
      Thread thread = Thread.currentThread();
      string str = \u003CVerifierFix\u003E.genCastToString(Profiler.threads.get((object) thread));
      if (str != null)
      {
        this.thread = str;
      }
      else
      {
        StringBuffer stringBuffer = new StringBuffer().append("t");
        int nextThread;
        Profiler.nextThread = (nextThread = Profiler.nextThread) + 1;
        int num = nextThread;
        this.thread = stringBuffer.append(num).ToString();
        Profiler.threads.put((object) thread, (object) this.thread);
      }
      this.memory = Profiler.memory();
    }

    public virtual long getElapsedTime() => this.timing ? Profiler.time() - this.start : this.elapsedTime;

    public virtual long getMemoryUsage() => Profiler.memory() - this.memory;

    public virtual string getName() => this.name;

    public virtual string log() => new StringBuffer().append(this.id).append("\t").append(this.thread).append("\t").append(this.getName()).append("\t").append(this.getMemoryUsage()).append("\t").append(this.getElapsedTime()).ToString();

    public virtual void reset()
    {
      this.elapsedTime = 0L;
      this.start = Profiler.time();
      this.memory = Profiler.memory();
    }

    public virtual void start()
    {
      this.start = Profiler.time();
      this.timing = true;
    }

    public virtual void stop()
    {
      this.timing = false;
      this.elapsedTime += Profiler.time() - this.start;
    }

    public virtual string memoryUsageLog() => new StringBuffer().append(Profiler.integerFormat.format(this.getMemoryUsage())).append(" bytes").ToString();

    public virtual string timeLog() => new StringBuffer().append(Profiler.floatFormat.format((double) this.getElapsedTime() / 1000.0)).append(" secs").ToString();

    public override string ToString() => new StringBuffer().append(this.getElapsedTime()).append("ms - ").append(this.name).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Profiler()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Profiler profiler = this;
      ObjectImpl.clone((object) profiler);
      return ((object) profiler).MemberwiseClone();
    }
  }
}
