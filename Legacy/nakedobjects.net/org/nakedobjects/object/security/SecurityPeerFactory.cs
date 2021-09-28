// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.security.SecurityPeerFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.security;

namespace org.nakedobjects.@object.security
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ReflectionPeerFactory;")]
  public class SecurityPeerFactory : ReflectionPeerFactory
  {
    private AuthorisationManager manager;

    public virtual void setAuthorisationManager(AuthorisationManager manager) => this.manager = manager;

    public virtual AuthorisationManager AuthorisationManager
    {
      set => this.setAuthorisationManager(value);
    }

    public virtual ActionPeer createAction(ActionPeer peer) => (ActionPeer) new ActionAuthorisation(peer, this.manager);

    public virtual OneToManyPeer createField(OneToManyPeer peer) => (OneToManyPeer) new OneToManyAuthorisation(peer, this.manager);

    public virtual OneToOnePeer createField(OneToOnePeer peer) => (OneToOnePeer) new OneToOneAuthorisation(peer, this.manager);

    public virtual void init() => this.manager.init();

    public virtual void shutdown() => this.manager.shutdown();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SecurityPeerFactory securityPeerFactory = this;
      ObjectImpl.clone((object) securityPeerFactory);
      return ((object) securityPeerFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
