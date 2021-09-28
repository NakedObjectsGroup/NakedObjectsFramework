// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.OneToManyAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectField;")]
  [JavaInterface]
  public interface OneToManyAssociation : NakedObjectField
  {
    void removeElement(NakedObject inObject, NakedObject associate);

    void clearCollection(NakedObject inObject);

    void initElement(NakedObject inObject, NakedObject associate);

    void initCollection(NakedObject inObject, NakedObject[] instances);

    void addElement(NakedObject inObject, NakedObject associate);

    Consent validToAdd(NakedObject container, NakedObject element);

    Consent validToRemove(NakedObject container, NakedObject element);
  }
}
