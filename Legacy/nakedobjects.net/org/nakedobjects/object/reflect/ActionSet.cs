// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ActionSet
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
  [JavaInterfaces("1;org/nakedobjects/object/Action;")]
  public class ActionSet : Action
  {
    private readonly string name;
    private readonly string id;
    private readonly Action[] actions;

    public ActionSet(string id, string name, Action[] actions)
    {
      this.id = id;
      this.name = name;
      this.actions = actions;
    }

    public virtual int getParameterCount() => 0;

    public virtual Action.Type getType() => Action.SET;

    public virtual Action.Target getTarget() => Action.DEFAULT;

    public virtual bool hasReturn() => false;

    public virtual bool isOnInstance() => false;

    public virtual NakedObjectSpecification[] getParameterTypes()
    {
      int length = 0;
      return length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
    }

    public virtual Naked[] parameterStubs()
    {
      int length = 0;
      return length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectSpecification getReturnType() => (NakedObjectSpecification) null;

    public virtual Naked execute(NakedReference target, Naked[] parameters) => throw new UnexpectedCallException();

    public virtual Consent isParameterSetValid(NakedReference @object, Naked[] parameters) => throw new UnexpectedCallException();

    public virtual ActionParameterSet getParameterSet(NakedReference @object) => throw new UnexpectedCallException();

    public virtual Action[] getActions() => this.actions;

    public virtual void debugData(DebugString debugString)
    {
    }

    public virtual string getDescription() => "";

    public virtual string getHelp() => "";

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual string getId() => this.id;

    public virtual string getName() => this.name;

    public virtual bool isAuthorised() => true;

    public virtual Consent isAvailable(NakedReference target) => (Consent) Allow.DEFAULT;

    public virtual Consent isVisible(NakedReference target) => (Consent) Allow.DEFAULT;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ActionSet actionSet = this;
      ObjectImpl.clone((object) actionSet);
      return ((object) actionSet).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
