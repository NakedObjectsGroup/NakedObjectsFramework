// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ActionImpl
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
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/Action;")]
  public class ActionImpl : AbstractNakedObjectMember, Action
  {
    private static readonly org.apache.log4j.Logger LOG;
    private ActionPeer peer;

    public static Action.Type getType(string type)
    {
      int length = 3;
      Action.Type[] typeArray1 = length >= 0 ? new Action.Type[length] : throw new NegativeArraySizeException();
      typeArray1[0] = Action.DEBUG;
      typeArray1[1] = Action.EXPLORATION;
      typeArray1[2] = Action.USER;
      Action.Type[] typeArray2 = typeArray1;
      for (int index = 0; index < typeArray2.Length; ++index)
      {
        if (StringImpl.equals(typeArray2[index].getName(), (object) type))
          return typeArray2[index];
      }
      throw new IllegalArgumentException();
    }

    public ActionImpl(string className, string methodName, ActionPeer actionDelegate)
      : base(methodName)
    {
      this.peer = actionDelegate;
    }

    public virtual Naked execute(NakedReference @object, Naked[] parameters)
    {
      if (ActionImpl.LOG.isDebugEnabled())
        ActionImpl.LOG.debug((object) new StringBuffer().append("execute action ").append((object) @object).append(".").append(this.getId()).ToString());
      Naked[] nakedArray;
      if (parameters == null)
      {
        int length = 0;
        nakedArray = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      }
      else
        nakedArray = parameters;
      Naked[] parameters1 = nakedArray;
      return this.peer.execute(!this.isOnInstance() ? (NakedReference) null : @object, parameters1);
    }

    public virtual Action[] getActions()
    {
      int length = 0;
      return length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
    }

    public override void debugData(DebugString debugString) => this.peer.debugData(debugString);

    public override string getDescription() => this.peer.getDescription();

    public override object getExtension(Class cls) => this.peer.getExtension((Class) null);

    public override Class[] getExtensions() => this.peer.getExtensions();

    public override string getHelp() => this.peer.getHelp();

    public override string getName() => this.peer.getName() ?? this.defaultLabel;

    public virtual int getParameterCount() => this.peer.getParameterCount();

    public virtual ActionParameterSet getParameterSet(NakedReference @object)
    {
      ActionParameterSet parameterSet = this.peer.createParameterSet(@object, this.parameterStubs());
      parameterSet?.checkParameters(this.peer.getIdentifier().ToString(), this.getParameterTypes());
      return parameterSet;
    }

    public virtual NakedObjectSpecification[] getParameterTypes() => this.peer.getParameterTypes();

    public virtual NakedObjectSpecification getReturnType() => this.peer.getReturnType();

    public virtual Action.Target getTarget() => this.peer.getTarget();

    public virtual Action.Type getType() => this.peer.getType();

    public virtual bool hasReturn() => this.getReturnType() != null;

    public override bool isAuthorised() => this.peer.isAuthorised(NakedObjects.getCurrentSession());

    public override Consent isAvailable(NakedReference target) => this.peer.isAvailable(target);

    public virtual bool isOnInstance() => this.peer.isOnInstance();

    public virtual Consent isParameterSetValid(NakedReference @object, Naked[] parameters) => this.peer.isParameterSetValid(@object, parameters);

    public override Consent isVisible(NakedReference target) => this.peer.isVisible(target);

    public virtual Naked[] parameterStubs()
    {
      int parameterCount = this.getParameterCount();
      int length = parameterCount;
      Naked[] nakedArray = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      NakedObjectSpecification[] parameterTypes = this.getParameterTypes();
      for (int index = 0; index < parameterCount; ++index)
      {
        NakedObjectSpecification specification = parameterTypes[index];
        nakedArray[index] = !specification.isValue() ? (Naked) null : (Naked) NakedObjects.getObjectLoader().createValueInstance(specification);
      }
      return nakedArray;
    }

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append("Action [");
      stringBuffer.append(base.ToString());
      stringBuffer.append(",type=");
      stringBuffer.append((object) this.getType());
      stringBuffer.append(",returns=");
      stringBuffer.append((object) this.getReturnType());
      stringBuffer.append(",parameters={");
      for (int index = 0; index < this.getParameterTypes().Length; ++index)
      {
        if (index > 0)
          stringBuffer.append(",");
        stringBuffer.append((object) this.getParameterTypes()[index]);
      }
      stringBuffer.append("}]");
      return stringBuffer.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ActionImpl()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
