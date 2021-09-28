// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ReflectionPeerBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect
{
  public class ReflectionPeerBuilder
  {
    private static readonly Logger LOG;
    private ReflectionPeerFactory[] factories;

    public virtual void setFactories(ReflectionPeerFactory[] factories)
    {
      this.factories = factories;
      if (ReflectionPeerBuilder.LOG.isDebugEnabled())
        ReflectionPeerBuilder.LOG.debug((object) "Reflection peers");
      for (int index = 0; index < factories.Length; ++index)
      {
        if (ReflectionPeerBuilder.LOG.isDebugEnabled())
          ReflectionPeerBuilder.LOG.debug((object) new StringBuffer().append("  ").append((object) factories[index]).ToString());
      }
    }

    public virtual ReflectionPeerFactory[] Factories
    {
      set => this.setFactories(value);
    }

    public virtual Action createAction(string className, ActionPeer actionPeer)
    {
      ActionPeer actionPeer1 = actionPeer;
      for (int index = this.factories.Length - 1; index >= 0; index += -1)
        actionPeer1 = this.factories[index].createAction(actionPeer1);
      return (Action) new ActionImpl(className, actionPeer1.getIdentifier().getName(), actionPeer1);
    }

    public virtual NakedObjectField createField(
      string className,
      OneToManyPeer fieldPeer)
    {
      OneToManyPeer oneToManyPeer = fieldPeer;
      for (int index = this.factories.Length - 1; index >= 0; index += -1)
        oneToManyPeer = this.factories[index].createField(oneToManyPeer);
      return (NakedObjectField) new OneToManyAssociationImpl(className, oneToManyPeer.getIdentifier().getName(), oneToManyPeer.getType(), oneToManyPeer);
    }

    public virtual NakedObjectField createField(
      string className,
      OneToOnePeer fieldPeer)
    {
      OneToOnePeer oneToOnePeer = fieldPeer;
      for (int index = this.factories.Length - 1; index >= 0; index += -1)
        oneToOnePeer = this.factories[index].createField(oneToOnePeer);
      return (NakedObjectField) new OneToOneAssociationImpl(className, oneToOnePeer.getIdentifier().getName(), oneToOnePeer.getType(), oneToOnePeer);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ReflectionPeerBuilder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ReflectionPeerBuilder reflectionPeerBuilder = this;
      ObjectImpl.clone((object) reflectionPeerBuilder);
      return ((object) reflectionPeerBuilder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
