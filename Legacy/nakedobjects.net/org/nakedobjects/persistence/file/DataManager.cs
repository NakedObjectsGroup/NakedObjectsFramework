// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.DataManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.persistence.file
{
  [JavaInterface]
  public interface DataManager
  {
    [JavaThrownExceptions("1;org/nakedobjects/persistence/file/PersistorException;")]
    SerialOid createOid();

    void getNakedClass(string name);

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    void insert(Data data);

    CollectionData loadCollectionData(SerialOid oid);

    ObjectData loadObjectData(SerialOid oid);

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    void remove(SerialOid oid);

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    void save(Data data);

    void shutdown();

    ObjectDataVector getInstances(ObjectData pattern);

    Data loadData(SerialOid oid);

    int numberOfInstances(ObjectData pattern);

    string getDebugData();
  }
}
