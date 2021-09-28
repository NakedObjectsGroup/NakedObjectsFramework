// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ActionPeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/MemberPeer;")]
  [JavaInterface]
  public interface ActionPeer : MemberPeer
  {
    ActionParameterSet createParameterSet(
      NakedReference @object,
      Naked[] parameters);

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectiveActionException;")]
    Naked execute(NakedReference @object, Naked[] parameters);

    int getParameterCount();

    NakedObjectSpecification[] getParameterTypes();

    NakedObjectSpecification getReturnType();

    Action.Target getTarget();

    int getActionGraphDepth();

    Action.Type getType();

    Consent isParameterSetValid(NakedReference @object, Naked[] parameters);

    bool isOnInstance();
  }
}
