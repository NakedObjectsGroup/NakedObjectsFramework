// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectPersistor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  [JavaInterfaces("2;org/nakedobjects/object/NakedObjectsComponent;org/nakedobjects/utility/DebugInfo;")]
  public interface NakedObjectPersistor : NakedObjectsComponent, DebugInfo
  {
    void abortTransaction();

    void addObjectChangedListener(DirtyObjectSet listener);

    TypedNakedCollection allInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses);

    NakedObject createPersistentInstance(NakedObjectSpecification specification);

    NakedObject createPersistentInstance(string className);

    NakedObject createTransientInstance(NakedObjectSpecification specification);

    NakedObject createTransientInstance(string className);

    void destroyObject(NakedObject @object);

    void endTransaction();

    [JavaThrownExceptions("1;org/nakedobjects/object/UnsupportedFindException;")]
    TypedNakedCollection findInstances(InstancesCriteria criteria);

    NakedClass getNakedClass(NakedObjectSpecification specification);

    NakedObject getObject(Oid oid, NakedObjectSpecification spec);

    bool hasInstances(NakedObjectSpecification specification, bool includeSubclasses);

    void makePersistent(NakedObject @object);

    int numberOfInstances(NakedObjectSpecification specification, bool includeSubclasses);

    void objectChanged(NakedObject @object);

    void reset();

    void reload(NakedObject @object);

    void refresh(NakedReference root, Hashtable nonRefreshOids);

    void resolveImmediately(NakedObject @object);

    void resolveField(NakedObject @object, NakedObjectField field);

    void saveChanges();

    void startTransaction();
  }
}
