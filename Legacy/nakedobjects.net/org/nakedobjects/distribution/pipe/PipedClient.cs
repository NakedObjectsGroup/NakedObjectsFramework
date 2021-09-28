// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.pipe.PipedClient
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.distribution.command;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.distribution.pipe
{
  public class PipedClient : CommandClient
  {
    private PipedConnection communication;

    public virtual void setConnection(PipedConnection communication) => this.communication = communication;

    [JavaFlags(36)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override Response executeRemotely(Request request)
    {
      this.communication.setRequest(request);
      return this.communication.getResponse();
    }
  }
}
