// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.ObjectTransferCarrier
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  [JavaFlags(32)]
  public class ObjectTransferCarrier
  {
    private object @object;
    private Oid oid;

    [JavaFlags(0)]
    public ObjectTransferCarrier(object @object, Oid oid)
    {
      this.@object = @object;
      this.oid = oid;
    }

    [JavaFlags(0)]
    public virtual object getObject() => this.@object;

    [JavaFlags(0)]
    public virtual Oid getOid() => this.oid;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ObjectTransferCarrier objectTransferCarrier = this;
      ObjectImpl.clone((object) objectTransferCarrier);
      return ((object) objectTransferCarrier).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
