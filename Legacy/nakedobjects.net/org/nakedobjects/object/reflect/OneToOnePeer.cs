// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.OneToOnePeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/FieldPeer;")]
  [JavaInterface]
  public interface OneToOnePeer : FieldPeer
  {
    TypedNakedCollection proposedOptions(NakedReference target);

    void clearAssociation(NakedObject inObject, NakedObject associate);

    Naked getAssociation(NakedObject inObject);

    void initAssociation(NakedObject inObject, NakedObject associate);

    void initValue(NakedObject inObject, object associate);

    bool isObject();

    void setAssociation(NakedObject inObject, NakedObject associate);

    void setValue(NakedObject inObject, object associate);

    Consent isAssociationValid(NakedObject inObject, NakedObject value);

    Consent isValueValid(NakedObject inObject, NakedValue value);
  }
}
