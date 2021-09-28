// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  public interface NakedObjectSpecification
  {
    void addSubclass(NakedObjectSpecification specification);

    void clearDirty(NakedObject @object);

    void deleted(NakedObject @object);

    NakedObjectField[] getAccessibleFields();

    NakedObjectField[] getAccessibleFieldsForCollectiveView();

    Action[] getButtonActions();

    Action getClassAction(Action.Type type, string name);

    Action getClassAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters);

    Action[] getClassActions(Action.Type type);

    Hint getClassHint();

    object getExtension(Class cls);

    Class[] getExtensions();

    NakedObjectField getField(string name);

    object getFieldExtension(string name, Class cls);

    Class[] getFieldExtensions(string name);

    NakedObjectField[] getFields();

    string getFullName();

    Action getObjectAction(Action.Type type, string name);

    Action getObjectAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters);

    Action[] getObjectActions(Action.Type type);

    string getPluralName();

    string getShortName();

    string getSingularName();

    string getTitle(NakedObject naked);

    NakedObjectField[] getVisibleFields(NakedObject @object);

    bool hasSubclasses();

    NakedObjectSpecification[] interfaces();

    void introspect();

    bool isAbstract();

    bool isCollection();

    bool isDirty(NakedObject @object);

    bool isLookup();

    bool isObject();

    bool isOfType(NakedObjectSpecification specification);

    bool isValue();

    void markDirty(NakedObject @object);

    Persistable persistable();

    NakedObjectSpecification[] subclasses();

    NakedObjectSpecification superclass();
  }
}
