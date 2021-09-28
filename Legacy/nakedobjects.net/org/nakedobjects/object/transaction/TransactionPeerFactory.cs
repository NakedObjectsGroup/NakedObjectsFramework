// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.transaction.TransactionPeerFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.transaction;

namespace org.nakedobjects.@object.transaction
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ReflectionPeerFactory;")]
  public class TransactionPeerFactory : ReflectionPeerFactory
  {
    public virtual ActionPeer createAction(ActionPeer peer) => (ActionPeer) new ActionTransaction(peer);

    public virtual OneToManyPeer createField(OneToManyPeer peer) => (OneToManyPeer) new OneToManyTransaction(peer);

    public virtual OneToOnePeer createField(OneToOnePeer peer) => (OneToOnePeer) new OneToOneTransaction(peer);

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TransactionPeerFactory transactionPeerFactory = this;
      ObjectImpl.clone((object) transactionPeerFactory);
      return ((object) transactionPeerFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
