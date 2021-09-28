// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.pipe.NakedObjectsPipe
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;

namespace org.nakedobjects.distribution.pipe
{
  public class NakedObjectsPipe : NakedObjectsServer
  {
    private Thread server;
    private NakedObjectsData serverData;
    private NakedObjectsData clientData;

    public virtual void setServer(Thread server) => this.server = server;

    public static NakedObjects createInstance() => NakedObjects.getInstance() == null ? (NakedObjects) new NakedObjectsPipe() : NakedObjects.getInstance();

    [JavaFlags(4)]
    public override NakedObjectsData getLocal() => Thread.currentThread() == this.server ? this.serverData : this.clientData;

    public override string getDebugTitle() => new StringBuffer().append("Naked Objects Repository ").append(Thread.currentThread().getName()).ToString();

    public NakedObjectsPipe()
    {
      this.serverData = new NakedObjectsData();
      this.clientData = new NakedObjectsData();
    }
  }
}
