// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ActionPeer;")]
  public class JavaAction : JavaMember, ActionPeer
  {
    [JavaFlags(24)]
    public static readonly org.apache.log4j.Logger LOG;
    private readonly Method actionMethod;
    private bool isInstanceMethod;
    private readonly int paramCount;
    private readonly NakedObjectSpecification[] parameters;
    private Action.Target target;
    private Action.Type type;

    public JavaAction(
      MemberIdentifier identifier,
      Action.Type type,
      NakedObjectSpecification[] parameters,
      Action.Target target,
      Method action,
      Method about)
      : base(identifier, about)
    {
      this.type = type;
      this.parameters = parameters;
      this.actionMethod = action;
      this.target = target;
      this.paramCount = action.getParameterTypes().Length;
      this.isInstanceMethod = ((Modifier.isStatic(this.actionMethod.getModifiers()) ? 1 : 0) ^ 1) != 0;
    }

    public virtual ActionParameterSet createParameterSet(
      NakedReference @object,
      Naked[] parameters)
    {
      Hint hint = this.getHint(@object, parameters);
      switch (hint)
      {
        case SimpleActionAbout _:
          SimpleActionAbout simpleActionAbout = (SimpleActionAbout) hint;
          return (ActionParameterSet) new ActionParameterSetImpl(simpleActionAbout.getDefaultParameterValues(), simpleActionAbout.getOptions(), simpleActionAbout.getParameterLabels(), simpleActionAbout.getRequired());
        case DefaultHint _:
          return (ActionParameterSet) null;
        default:
          throw new ReflectionException();
      }
    }

    public virtual void debugData(DebugString debugString)
    {
      debugString.appendln("Identifier", (object) this.getIdentifier());
      debugString.appendln("Action method", (object) this.actionMethod);
      if (this.getAboutMethod() == null)
        return;
      debugString.appendln("About method", (object) this.getAboutMethod());
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectiveActionException;")]
    public virtual Naked execute(NakedReference inObject, Naked[] parameters)
    {
      if (parameters.Length != this.paramCount)
        JavaAction.LOG.error((object) new StringBuffer().append((object) this.actionMethod).append(" requires ").append(this.paramCount).append(" parameters, not ").append(parameters.Length).ToString());
      try
      {
        int length = parameters.Length;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        for (int index = 0; index < parameters.Length; ++index)
          objArray[index] = parameters[index] != null ? parameters[index].getObject() : (object) null;
        object obj = this.actionMethod.invoke(inObject?.getObject(), objArray);
        if (JavaAction.LOG.isDebugEnabled())
          JavaAction.LOG.debug((object) new StringBuffer().append(" action result ").append(obj).ToString());
        if (obj == null)
          return (Naked) null;
        NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (object)));
        return (Naked) NakedObjects.getObjectLoader().createAdapterForCollection(obj, specification) ?? (Naked) NakedObjects.getObjectLoader().getAdapterFor(obj) ?? (Naked) NakedObjects.getObjectLoader().createAdapterForTransient(obj);
      }
      catch (InvocationTargetException ex)
      {
        if (ex.getTargetException() is TransactionException)
          throw new ReflectiveActionException(new StringBuffer().append("TransactionException thrown while executing ").append((object) this.actionMethod).append(" ").append(ex.getTargetException().getMessage()).ToString(), ex.getTargetException());
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.actionMethod).ToString(), ex);
        return (Naked) null;
      }
      catch (IllegalAccessException ex)
      {
        throw new ReflectiveActionException(new StringBuffer().append("Illegal access of ").append((object) this.actionMethod).ToString(), (Throwable) ex);
      }
    }

    public virtual string getDescription() => "";

    public override object getExtension(Class cls) => (object) null;

    private Hint getHint(NakedReference @object, Naked[] parameters)
    {
      if (parameters == null)
      {
        int length = 0;
        parameters = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      }
      if (parameters.Length != this.paramCount)
        JavaAction.LOG.error((object) new StringBuffer().append((object) this.actionMethod).append(" requires ").append(this.paramCount).append(" parameters, not ").append(parameters.Length).ToString());
      Method aboutMethod = this.getAboutMethod();
      if (aboutMethod == null)
        return (Hint) new DefaultHint();
      try
      {
        SimpleActionAbout simpleActionAbout = new SimpleActionAbout(NakedObjects.getCurrentSession(), @object.getObject(), (object[]) parameters);
        if (StringImpl.equals(aboutMethod.getName(), (object) "aboutActionDefault"))
        {
          Method method = aboutMethod;
          object obj = @object.getObject();
          int length = 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = (object) simpleActionAbout;
          method.invoke(obj, objArray);
        }
        else
        {
          int length = parameters.Length + 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = (object) simpleActionAbout;
          for (int index = 1; index < objArray.Length; ++index)
            objArray[index] = parameters[index - 1] != null ? parameters[index - 1].getObject() : (object) null;
          aboutMethod.invoke(@object.getObject(), objArray);
        }
        if (simpleActionAbout == null)
        {
          JavaAction.LOG.error((object) new StringBuffer().append("no about returned from ").append((object) aboutMethod).append(" allowing action by default.").ToString());
          return (Hint) new DefaultHint();
        }
        if (StringImpl.equals(simpleActionAbout.getDescription(), (object) ""))
          simpleActionAbout.setDescription(new StringBuffer().append("Invoke action ").append((object) this.getIdentifier()).ToString());
        return (Hint) simpleActionAbout;
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) aboutMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaAction.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) aboutMethod).ToString(), (Throwable) ex);
      }
      return (Hint) new DefaultHint();
    }

    public virtual string getName() => (string) null;

    public virtual int getParameterCount() => this.paramCount;

    public virtual NakedObjectSpecification[] getParameterTypes() => this.parameters;

    public virtual NakedObjectSpecification getReturnType()
    {
      Class returnType = this.actionMethod.getReturnType();
      return returnType != Void.TYPE && returnType != Class.FromType(typeof (NakedError)) ? this.specification(returnType) : (NakedObjectSpecification) null;
    }

    public virtual Action.Target getTarget() => this.target;

    public virtual int getActionGraphDepth() => 1;

    public virtual Action.Type getType() => this.type;

    public virtual Consent isParameterSetValid(NakedReference @object, Naked[] parameters) => this.getHint(@object, parameters).canUse();

    public virtual bool isAuthorised(Session session) => true;

    public virtual bool isOnInstance() => this.isInstanceMethod;

    public virtual Consent isAvailable(NakedReference target) => (Consent) Allow.DEFAULT;

    public virtual Consent isVisible(NakedReference target) => (Consent) Allow.DEFAULT;

    private NakedObjectSpecification specification(Class returnType) => NakedObjects.getSpecificationLoader().loadSpecification(returnType.getName());

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      Class[] parameterTypes = this.actionMethod.getParameterTypes();
      if (parameterTypes.Length == 0)
        stringBuffer.append("none");
      for (int index = 0; index < parameterTypes.Length; ++index)
      {
        if (index > 0)
          stringBuffer.append("/");
        stringBuffer.append((object) parameterTypes[index]);
      }
      return new StringBuffer().append("JavaAction [method=").append(this.actionMethod.getName()).append(",type=").append(this.type.getName()).append(",parameters=").append((object) stringBuffer).append("]").ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaAction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
