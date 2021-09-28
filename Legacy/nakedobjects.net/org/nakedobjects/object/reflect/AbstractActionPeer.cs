// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.AbstractActionPeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ActionPeer;")]
  public abstract class AbstractActionPeer : ActionPeer
  {
    private readonly ActionPeer decorated;

    public AbstractActionPeer(ActionPeer decorated) => this.decorated = decorated;

    public virtual ActionParameterSet createParameterSet(
      NakedReference @object,
      Naked[] parameters)
    {
      return this.decorated.createParameterSet(@object, parameters);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectiveActionException;")]
    public virtual Naked execute(NakedReference @object, Naked[] parameters) => this.decorated.execute(@object, parameters);

    public virtual void debugData(DebugString debugString) => this.decorated.debugData(debugString);

    public virtual string getDescription() => this.decorated.getDescription();

    public virtual object getExtension(Class cls) => this.decorated.getExtension(cls);

    public virtual Class[] getExtensions() => this.decorated.getExtensions();

    public virtual string getHelp() => this.decorated.getHelp();

    public virtual MemberIdentifier getIdentifier() => this.decorated.getIdentifier();

    public virtual string getName() => this.decorated.getName();

    public virtual int getParameterCount() => this.decorated.getParameterCount();

    public virtual NakedObjectSpecification[] getParameterTypes() => this.decorated.getParameterTypes();

    public virtual NakedObjectSpecification getReturnType() => this.decorated.getReturnType();

    public virtual Action.Target getTarget() => this.decorated.getTarget();

    public virtual int getActionGraphDepth() => this.decorated.getActionGraphDepth();

    public virtual Action.Type getType() => this.decorated.getType();

    public virtual bool isAuthorised(Session session) => this.decorated.isAuthorised(session);

    public virtual Consent isAvailable(NakedReference target) => this.decorated.isAvailable(target);

    public virtual bool isOnInstance() => this.decorated.isOnInstance();

    public virtual Consent isParameterSetValid(NakedReference @object, Naked[] parameters) => this.decorated.isParameterSetValid(@object, parameters);

    public virtual Consent isVisible(NakedReference target) => this.decorated.isVisible(target);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractActionPeer abstractActionPeer = this;
      ObjectImpl.clone((object) abstractActionPeer);
      return ((object) abstractActionPeer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
