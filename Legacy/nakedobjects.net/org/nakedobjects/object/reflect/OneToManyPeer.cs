// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.OneToManyPeer
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
  public interface OneToManyPeer : FieldPeer
  {
    void addAssociation(NakedObject inObject, NakedObject associate);

    NakedCollection getAssociations(NakedObject inObject);

    void initAssociation(NakedObject inObject, NakedObject associate);

    void initOneToManyAssociation(NakedObject inObject, NakedObject[] instances);

    void removeAllAssociations(NakedObject inObject);

    void removeAssociation(NakedObject inObject, NakedObject associate);

    Consent isAddValid(NakedObject container, NakedObject element);

    Consent isRemoveValid(NakedObject container, NakedObject element);
  }
}
