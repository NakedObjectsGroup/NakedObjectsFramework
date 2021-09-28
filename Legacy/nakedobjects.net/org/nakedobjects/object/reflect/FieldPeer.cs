// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.FieldPeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/object/reflect/MemberPeer;")]
  public interface FieldPeer : MemberPeer
  {
    NakedObjectSpecification getType();

    bool isDerived();

    bool isEmpty(NakedObject inObject);

    bool isHidden();

    bool isHiddenInTableViews();

    bool isMandatory();
  }
}
