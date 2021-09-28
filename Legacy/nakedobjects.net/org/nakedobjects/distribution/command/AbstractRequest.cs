// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.AbstractRequest
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.distribution.command
{
  [JavaInterfaces("1;org/nakedobjects/distribution/command/Request;")]
  public abstract class AbstractRequest : Request
  {
    [JavaFlags(4)]
    public object response;
    private static int nextId;
    [JavaFlags(20)]
    public readonly int id;
    [JavaFlags(20)]
    public readonly Session session;

    public AbstractRequest(Session session)
    {
      int nextId;
      AbstractRequest.nextId = (nextId = AbstractRequest.nextId) + 1;
      this.id = nextId;
      this.session = session;
    }

    [JavaFlags(17)]
    public virtual void setResponse(object response) => this.response = response;

    public virtual object getResponse() => this.response;

    public virtual int getId() => this.id;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractRequest()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractRequest abstractRequest = this;
      ObjectImpl.clone((object) abstractRequest);
      return ((object) abstractRequest).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void execute(Distribution distribution);
  }
}
