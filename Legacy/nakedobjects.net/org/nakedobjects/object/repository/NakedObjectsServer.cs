// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.repository.NakedObjectsServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.repository
{
  public abstract class NakedObjectsServer : NakedObjects
  {
    private static readonly org.apache.log4j.Logger LOG;

    [JavaFlags(4)]
    public NakedObjectsServer()
    {
    }

    [JavaFlags(4)]
    public override NakedObjectConfiguration configuration() => this.getLocal().configuration;

    [JavaFlags(4)]
    public override Session currentSession() => this.getLocal().session;

    [JavaFlags(1028)]
    public abstract NakedObjectsData getLocal();

    [JavaFlags(4)]
    public override NakedObjectPersistor objectPersistor() => this.getLocal().objectPersistor;

    [JavaFlags(4)]
    public override NakedObjectLoader objectLoader() => this.getLocal().objectLoader;

    [JavaFlags(4)]
    public override MessageBroker messageBroker() => this.getLocal().messageBroker;

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
      set
      {
        if (NakedObjectsServer.LOG.isDebugEnabled())
        {
          if (value != null)
            NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" set_SpecificationLoader() specificationLoader hash:").append(Long.toHexString((long) value.GetHashCode())).ToString());
          else
            NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" set_SpecificationLoader() specificationLoader: NULL").ToString());
        }
        this.setSpecificationLoader(value);
      }
    }

    public override void setConfiguration(NakedObjectConfiguration configuration)
    {
      if (NakedObjectsServer.LOG.isDebugEnabled())
      {
        if (configuration != null)
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setConfiguration() configuration hash:").append(Long.toHexString((long) configuration.GetHashCode())).ToString());
        else
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setConfiguration() configuration hash").ToString());
      }
      this.getLocal().configuration = configuration;
    }

    public override void setObjectPersistor(NakedObjectPersistor objectManager)
    {
      if (NakedObjectsServer.LOG.isDebugEnabled())
      {
        if (objectManager != null)
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setObjectPersistor() objectManager hash:").append(Long.toHexString((long) objectManager.GetHashCode())).ToString());
        else
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setObjectPersistor() objectManager hash: null").ToString());
      }
      this.getLocal().objectPersistor = objectManager;
    }

    public override void setObjectLoader(NakedObjectLoader objectLoader)
    {
      if (NakedObjectsServer.LOG.isDebugEnabled())
      {
        if (objectLoader != null)
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setObjectLoader() objectLoader hash:").append(Long.toHexString((long) objectLoader.GetHashCode())).ToString());
        else
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setObjectLoader() objectLoader hash: null").ToString());
      }
      this.getLocal().objectLoader = objectLoader;
    }

    public override void setSession(Session session)
    {
      if (NakedObjectsServer.LOG.isDebugEnabled())
      {
        if (session != null)
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setSession() session hash:").append(Long.toHexString((long) session.GetHashCode())).ToString());
        else
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setSession() session hash: null").ToString());
      }
      this.getLocal().session = session;
    }

    public override void setSpecificationLoader(NakedObjectSpecificationLoader specificationLoader)
    {
      if (NakedObjectsServer.LOG.isDebugEnabled())
      {
        if (specificationLoader != null)
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setSpecificationLoader() specificationLoader hash:").append(Long.toHexString((long) specificationLoader.GetHashCode())).ToString());
        else
          NakedObjectsServer.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" setSpecificationLoader() specificationLoader hash: null").ToString());
      }
      this.getLocal().specificationLoader = specificationLoader;
    }

    [JavaFlags(4)]
    public override NakedObjectSpecificationLoader specificationLoader() => this.getLocal().specificationLoader;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NakedObjectsServer()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
