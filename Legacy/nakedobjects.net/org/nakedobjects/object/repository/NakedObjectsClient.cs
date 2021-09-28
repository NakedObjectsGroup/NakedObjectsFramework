// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.repository.NakedObjectsClient
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.repository
{
  public class NakedObjectsClient : NakedObjects
  {
    [JavaFlags(4)]
    public NakedObjectLoader objectLoader;
    [JavaFlags(4)]
    public NakedObjectConfiguration configuration;
    [JavaFlags(4)]
    public NakedObjectPersistor objectManager;
    [JavaFlags(4)]
    public NakedObjectSpecificationLoader specificationLoader;
    [JavaFlags(4)]
    public Session session;
    private MessageBroker messages;

    [JavaFlags(4)]
    public override NakedObjectConfiguration configuration() => this.configuration;

    [JavaFlags(4)]
    public override Session currentSession() => this.session;

    public override string getDebugTitle() => "Naked Objects Client Repository";

    [JavaFlags(4)]
    public override MessageBroker messageBroker() => this.messages;

    [JavaFlags(4)]
    public override NakedObjectPersistor objectPersistor() => this.objectManager;

    [JavaFlags(4)]
    public override NakedObjectLoader objectLoader() => this.objectLoader;

    public virtual NakedObjectConfiguration Configuration
    {
      set => this.setConfiguration(value);
    }

    public virtual NakedObjectPersistor ObjectPersistor
    {
      set => this.setObjectPersistor(value);
    }

    public virtual NakedObjectLoader ObjectLoader
    {
      set => this.setObjectLoader(value);
    }

    public virtual Session Session
    {
      set => this.setSession(value);
    }

    public virtual NakedObjectSpecificationLoader SpecificationLoader
    {
      set => this.setSpecificationLoader(value);
    }

    public override void setConfiguration(NakedObjectConfiguration configuration) => this.configuration = configuration;

    public override void setObjectPersistor(NakedObjectPersistor objectManager) => this.objectManager = objectManager;

    public override void setObjectLoader(NakedObjectLoader objectLoader) => this.objectLoader = objectLoader;

    public override void setSession(Session session) => this.session = session;

    public override void setSpecificationLoader(NakedObjectSpecificationLoader specificationLoader) => this.specificationLoader = specificationLoader;

    [JavaFlags(4)]
    public override NakedObjectSpecificationLoader specificationLoader() => this.specificationLoader;

    public NakedObjectsClient() => this.messages = (MessageBroker) new SimpleMessageBroker();
  }
}
