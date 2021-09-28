// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.OneToOneAssociation
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
  public interface OneToOneAssociation : NakedObjectField
  {
    TypedNakedCollection proposedOptions(NakedReference target);

    void clearAssociation(NakedObject inObject, NakedObject associate);

    void clearValue(NakedObject inObject);

    void initAssociation(NakedObject inObject, NakedObject associate);

    void initValue(NakedObject inObject, object value);

    void setAssociation(NakedObject inObject, NakedObject associate);

    void setValue(NakedObject inObject, object value);

    Consent isAssociationValid(NakedObject inObject, NakedObject associate);

    Consent isValueValid(NakedObject inObject, NakedValue value);
  }
}
