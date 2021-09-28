// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/object/NakedReference;")]
  public interface NakedObject : NakedReference
  {
    Consent canAdd(OneToManyAssociation field, NakedObject nakedObject);

    void clearAssociation(NakedObjectField specification, NakedObject @ref);

    void clearCollection(OneToManyAssociation association);

    void clearValue(OneToOneAssociation association);

    NakedObject getAssociation(OneToOneAssociation field);

    Naked getField(NakedObjectField field);

    NakedObjectField[] getFields();

    NakedValue getValue(OneToOneAssociation field);

    NakedObjectField[] getVisibleFields();

    void initAssociation(NakedObjectField field, NakedObject associatedObject);

    void initAssociation(OneToManyAssociation association, NakedObject[] instances);

    void initValue(OneToOneAssociation field, object @object);

    bool isEmpty(NakedObjectField field);

    Consent isValid(OneToOneAssociation field, NakedObject nakedObject);

    Consent isValid(OneToOneAssociation field, NakedValue nakedValue);

    Consent isVisible(NakedObjectField field);

    Consent isAvailable(NakedObjectField field);

    void setAssociation(NakedObjectField field, NakedObject associatedObject);

    void setValue(OneToOneAssociation field, object @object);
  }
}
