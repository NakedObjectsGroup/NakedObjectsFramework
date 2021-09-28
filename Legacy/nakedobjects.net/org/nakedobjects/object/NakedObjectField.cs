// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectMember;")]
  [JavaInterface]
  public interface NakedObjectField : NakedObjectMember
  {
    Naked get(NakedObject fromObject);

    new Class[] getExtensions();

    NakedObjectSpecification getSpecification();

    bool isCollection();

    bool isDerived();

    bool isEmpty(NakedObject adapter);

    bool isHidden();

    bool isHiddenInTableViews();

    bool isMandatory();

    bool isObject();

    bool isValue();
  }
}
