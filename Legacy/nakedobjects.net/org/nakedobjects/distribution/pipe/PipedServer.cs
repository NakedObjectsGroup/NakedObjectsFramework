// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.pipe.PipedServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.log4j;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.distribution.pipe
{
  public class PipedServer
  {
    private static readonly Logger LOG;
    private Distribution facade;
    private PipedConnection communication;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void run()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void setConnection(PipedConnection communication) => this.communication = communication;

    public virtual void setFacade(Distribution facade) => this.facade = facade;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static PipedServer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      PipedServer pipedServer = this;
      ObjectImpl.clone((object) pipedServer);
      return ((object) pipedServer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
