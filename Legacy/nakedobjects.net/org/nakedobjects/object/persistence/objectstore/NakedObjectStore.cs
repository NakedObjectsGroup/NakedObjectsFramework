// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.NakedObjectStore
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.persistence.objectstore
{
  [JavaInterface]
  [JavaInterfaces("2;org/nakedobjects/object/NakedObjectsComponent;org/nakedobjects/utility/DebugInfo;")]
  public interface NakedObjectStore : NakedObjectsComponent, DebugInfo
  {
    void abortTransaction();

    CreateObjectCommand createCreateObjectCommand(NakedObject @object);

    DestroyObjectCommand createDestroyObjectCommand(NakedObject @object);

    SaveObjectCommand createSaveObjectCommand(NakedObject @object);

    void endTransaction();

    NakedObject[] getInstances(InstancesCriteria criteria);

    NakedObject[] getInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses);

    NakedClass getNakedClass(string name);

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    NakedObject getObject(Oid oid, NakedObjectSpecification hint);

    bool hasInstances(NakedObjectSpecification specification, bool includeSubclasses);

    string name();

    int numberOfInstances(NakedObjectSpecification specification, bool includedSubclasses);

    void resolveField(NakedObject @object, NakedObjectField field);

    void resolveImmediately(NakedObject @object);

    void execute(PersistenceCommand[] commands);

    void startTransaction();

    void reset();

    Transaction createTransaction();
  }
}
