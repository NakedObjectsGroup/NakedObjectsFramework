// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ProxyPeerFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using junit.framework;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.distribution
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ReflectionPeerFactory;")]
  public class ProxyPeerFactory : ReflectionPeerFactory
  {
    private Distribution connection;
    private ObjectEncoder encoder;

    public virtual ActionPeer createAction(ActionPeer peer) => (ActionPeer) new ProxyAction(peer, this.connection, this.encoder, this.cacheResults(peer));

    [JavaFlags(4)]
    public virtual bool cacheResults(ActionPeer peer) => true;

    public virtual OneToManyPeer createField(OneToManyPeer peer) => (OneToManyPeer) new ProxyOneToManyAssociation(peer, this.connection, this.encoder);

    public virtual OneToOnePeer createField(OneToOnePeer peer) => (OneToOnePeer) new ProxyOneToOneAssociation(peer, this.connection, this.encoder);

    public virtual Distribution Connection
    {
      set => this.connection = value;
    }

    public virtual ObjectEncoder Encoder
    {
      set => this.encoder = value;
    }

    public virtual void setConnection(Distribution connection) => this.connection = connection;

    public virtual void setEncoder(ObjectEncoder encoder) => this.encoder = encoder;

    public virtual void init()
    {
      Assert.assertNotNull("Connection required", (object) this.connection);
      Assert.assertNotNull("Object encoder required", (object) this.encoder);
    }

    public virtual void shutdown()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ProxyPeerFactory proxyPeerFactory = this;
      ObjectImpl.clone((object) proxyPeerFactory);
      return ((object) proxyPeerFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
