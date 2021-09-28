// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.help.HelpPeerFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.help;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.help
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ReflectionPeerFactory;")]
  public class HelpPeerFactory : ReflectionPeerFactory
  {
    private HelpManager manager;

    public virtual void setHelpManager(HelpManager manager) => this.manager = manager;

    public virtual HelpManager HelpManager
    {
      set => this.setHelpManager(value);
    }

    public virtual ActionPeer createAction(ActionPeer peer) => (ActionPeer) new ActionHelp(peer, this.manager);

    public virtual OneToManyPeer createField(OneToManyPeer peer) => (OneToManyPeer) new OneToManyHelp(peer, this.manager);

    public virtual OneToOnePeer createField(OneToOnePeer peer) => (OneToOnePeer) new OneToOneHelp(peer, this.manager);

    public virtual void init() => this.manager.init();

    public virtual void shutdown() => this.manager.shutdown();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      HelpPeerFactory helpPeerFactory = this;
      ObjectImpl.clone((object) helpPeerFactory);
      return ((object) helpPeerFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
