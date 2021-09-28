// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ActionPeer;")]
  public class InternalAction : InternalMember, ActionPeer
  {
    [JavaFlags(24)]
    public static readonly org.apache.log4j.Logger LOG;
    private readonly Method actionMethod;
    private readonly int paramCount;
    private Action.Type type;

    public InternalAction(string className, string name, Action.Type type, Method action)
    {
      this.type = type;
      this.actionMethod = action;
      this.paramCount = action.getParameterTypes().Length;
      this.identifeir = (MemberIdentifier) new MemberIdentifierImpl(className, name, this.getParameterTypes());
    }

    public virtual ActionParameterSet createParameterSet(
      NakedReference @object,
      Naked[] parameters)
    {
      throw new UnexpectedCallException();
    }

    public virtual Naked execute(NakedReference inObject, Naked[] parameters)
    {
      if (parameters.Length != this.paramCount)
        InternalAction.LOG.error((object) new StringBuffer().append((object) this.actionMethod).append(" requires ").append(this.paramCount).append(" parameters, not ").append(parameters.Length).ToString());
      try
      {
        if (InternalAction.LOG.isDebugEnabled())
          InternalAction.LOG.debug((object) new StringBuffer().append("action: invoke ").append((object) inObject).append(".").append((object) this.getIdentifier()).ToString());
        int length = parameters.Length;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        for (int index = 0; index < parameters.Length; ++index)
          objArray[index] = parameters[index] != null ? parameters[index].getObject() : (object) null;
        object @object = this.actionMethod.invoke(inObject.getObject(), objArray);
        if (InternalAction.LOG.isDebugEnabled())
          InternalAction.LOG.debug((object) new StringBuffer().append("  action result ").append(@object).ToString());
        if (@object != null && @object is Naked)
          return (Naked) @object;
        if (@object != null)
          return (Naked) NakedObjects.getObjectLoader().createAdapterForTransient(@object);
      }
      catch (InvocationTargetException ex)
      {
        ((Throwable) ex).fillInStackTrace();
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        InternalAction.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.actionMethod).ToString(), (Throwable) ex);
      }
      return (Naked) null;
    }

    public virtual void debugData(DebugString debugString) => debugString.appendln("method", (object) this.actionMethod);

    public virtual string getName() => (string) null;

    public virtual int getParameterCount() => this.paramCount;

    public virtual NakedObjectSpecification[] getParameterTypes()
    {
      Class[] parameterTypes = this.actionMethod.getParameterTypes();
      int length = parameterTypes.Length;
      NakedObjectSpecification[] objectSpecificationArray = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameterTypes.Length; ++index)
        objectSpecificationArray[index] = this.nakedClass(parameterTypes[index]);
      return objectSpecificationArray;
    }

    public virtual NakedObjectSpecification getReturnType()
    {
      Class returnType = this.actionMethod.getReturnType();
      return returnType != Void.TYPE && returnType != Class.FromType(typeof (NakedError)) ? this.nakedClass(returnType) : (NakedObjectSpecification) null;
    }

    public virtual Action.Target getTarget() => Action.DEFAULT;

    public virtual int getActionGraphDepth() => 2;

    public virtual Action.Type getType() => this.type;

    public virtual Consent isParameterSetValid(NakedReference @object, Naked[] parameters) => (Consent) Allow.DEFAULT;

    public virtual bool isAuthorised(Session session) => true;

    public virtual bool isOnInstance() => ((Modifier.isStatic(this.actionMethod.getModifiers()) ? 1 : 0) ^ 1) != 0;

    public virtual Consent isAvailable(NakedReference target) => (Consent) Allow.DEFAULT;

    public virtual Consent isVisible(NakedReference target) => (Consent) Allow.DEFAULT;

    private NakedObjectSpecification nakedClass(Class returnType) => NakedObjects.getSpecificationLoader().loadSpecification(returnType.getName());

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InternalAction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
