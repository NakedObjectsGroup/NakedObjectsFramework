// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.AbstractXmlStoreSystem
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.persistence.file;

namespace org.nakedobjects.system
{
  [JavaFlags(1056)]
  public abstract class AbstractXmlStoreSystem : AbstractSystem
  {
    [JavaFlags(4)]
    public override NakedObjectPersistor createPersistor()
    {
      XmlObjectStore xmlObjectStore = new XmlObjectStore();
      string directory = this.configuration.getString("nakedobjects.xmlos.dir", "xml");
      xmlObjectStore.setDataManager((DataManager) new XmlDataManager(directory));
      DefaultPersistAlgorithm persistAlgorithm = new DefaultPersistAlgorithm();
      persistAlgorithm.setOidGenerator((OidGenerator) new TimeBasedOidGenerator());
      ObjectStorePersistor objectStorePersistor = new ObjectStorePersistor();
      objectStorePersistor.setObjectStore((NakedObjectStore) xmlObjectStore);
      objectStorePersistor.setPersistAlgorithm((PersistAlgorithm) persistAlgorithm);
      objectStorePersistor.setCheckObjectsForDirtyFlag(true);
      return (NakedObjectPersistor) objectStorePersistor;
    }

    [JavaFlags(0)]
    public AbstractXmlStoreSystem()
    {
    }
  }
}
