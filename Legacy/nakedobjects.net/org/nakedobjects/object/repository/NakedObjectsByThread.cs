// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.repository.NakedObjectsByThread
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;
using System.ComponentModel;
using System.Threading;

namespace org.nakedobjects.@object.repository
{
  public class NakedObjectsByThread : NakedObjectsServer
  {
    private static readonly Logger LOG;
    private Hashtable threads;

    public static NakedObjects createInstance() => NakedObjects.getInstance() == null ? (NakedObjects) new NakedObjectsByThread() : NakedObjects.getInstance();

    private NakedObjectsByThread() => this.threads = new Hashtable();

    [JavaFlags(4)]
    public override NakedObjectsData getLocal()
    {
      Thread thread = Thread.currentThread();
      object threads1 = (object) this.threads;
      \u003CCorArrayWrapper\u003E.Enter(threads1);
      NakedObjectsData nakedObjectsData;
      try
      {
        nakedObjectsData = (NakedObjectsData) this.threads.get((object) thread);
        if (nakedObjectsData == null)
        {
          nakedObjectsData = new NakedObjectsData();
          object threads2 = (object) this.threads;
          \u003CCorArrayWrapper\u003E.Enter(threads2);
          try
          {
            this.threads.put((object) thread, (object) nakedObjectsData);
          }
          finally
          {
            Monitor.Exit(threads2);
          }
          if (NakedObjectsByThread.LOG.isInfoEnabled())
            NakedObjectsByThread.LOG.info((object) new StringBuffer().append("  creating local ").append((object) nakedObjectsData).append("; now have ").append(this.threads.size()).append(" locals").ToString());
        }
      }
      finally
      {
        Monitor.Exit(threads1);
      }
      return nakedObjectsData;
    }

    public override string getDebugTitle() => new StringBuffer().append("Naked Objects Repository ").append(Thread.currentThread().getName()).ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NakedObjectsByThread()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
