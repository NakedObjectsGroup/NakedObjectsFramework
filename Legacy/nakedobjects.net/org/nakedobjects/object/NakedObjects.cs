// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjects
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("2;org/nakedobjects/object/NakedObjectsComponent;org/nakedobjects/utility/DebugInfo;")]
  public abstract class NakedObjects : NakedObjectsComponent, DebugInfo
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static NakedObjects singleton;

    public static DebugInfo debug() => (DebugInfo) NakedObjects.getInstance();

    public static NakedObjectConfiguration getConfiguration() => NakedObjects.getInstance().configuration();

    public static Session getCurrentSession() => NakedObjects.getInstance().currentSession();

    [JavaFlags(12)]
    public static NakedObjects getInstance() => NakedObjects.singleton;

    public static MessageBroker getMessageBroker() => NakedObjects.getInstance().messageBroker();

    public static NakedObjectLoader getObjectLoader() => NakedObjects.getInstance().objectLoader();

    public static NakedObjectPersistor getObjectPersistor() => NakedObjects.getInstance().objectPersistor();

    public static NakedObjectSpecificationLoader getSpecificationLoader() => NakedObjects.getInstance().specificationLoader();

    public static void reset() => NakedObjects.singleton = (NakedObjects) null;

    public virtual void shutdown()
    {
      if (NakedObjects.LOG.isInfoEnabled())
        NakedObjects.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" shutting down ").append((object) NakedObjects.getInstance()).ToString());
      this.objectPersistor().shutdown();
      this.objectLoader().shutdown();
      this.specificationLoader().shutdown();
      this.clearReferences();
    }

    [JavaFlags(4)]
    public NakedObjects()
    {
      NakedObjects.reset();
      NakedObjects.singleton = NakedObjects.singleton == null ? this : throw new NakedObjectRuntimeException("Naked Objects Repository already set up");
    }

    private void clearReferences()
    {
      this.setObjectPersistor((NakedObjectPersistor) null);
      this.setObjectLoader((NakedObjectLoader) null);
      this.setSpecificationLoader((NakedObjectSpecificationLoader) null);
      this.setConfiguration((NakedObjectConfiguration) null);
      this.setSession((Session) null);
    }

    [JavaFlags(1028)]
    public abstract NakedObjectConfiguration configuration();

    [JavaFlags(1028)]
    public abstract Session currentSession();

    public virtual void debugData(DebugString debug)
    {
      debug.appendln(AboutNakedObjects.getFrameworkName());
      debug.appendln(new StringBuffer().append(AboutNakedObjects.getFrameworkVersion()).append(AboutNakedObjects.getFrameworkBuild()).ToString());
      debug.appendln();
      debug.appendln("configuration", (object) ObjectImpl.getClass((object) this.configuration()).getName());
      debug.appendln("session", this.currentSession() != null ? (object) ObjectImpl.getClass((object) this.currentSession()).getName() : (object) "null");
      debug.appendln("instance", (object) ObjectImpl.getClass((object) this.specificationLoader()).getName());
      debug.appendln("loader", (object) ObjectImpl.getClass((object) this.objectLoader()).getName());
      debug.appendln("persistor", (object) ObjectImpl.getClass((object) this.objectPersistor()).getName());
    }

    public virtual void init()
    {
      if (NakedObjects.LOG.isInfoEnabled())
        NakedObjects.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" initialising ").append((object) this).ToString());
      Assert.assertNotNull("no object persistor set up", (object) NakedObjects.getObjectPersistor());
      Assert.assertNotNull("no configuration set up", (object) NakedObjects.getConfiguration());
      Assert.assertNotNull("no object loader set up", (object) NakedObjects.getObjectLoader());
      Assert.assertNotNull("no specification loader set up", (object) NakedObjects.getSpecificationLoader());
      NakedObjects.getSpecificationLoader().init();
      NakedObjects.getObjectLoader().init();
      NakedObjects.getObjectPersistor().init();
    }

    [JavaFlags(1028)]
    public abstract MessageBroker messageBroker();

    [JavaFlags(1028)]
    public abstract NakedObjectLoader objectLoader();

    [JavaFlags(1028)]
    public abstract NakedObjectPersistor objectPersistor();

    public abstract void setConfiguration(NakedObjectConfiguration configuration);

    public abstract void setObjectLoader(NakedObjectLoader loader);

    public abstract void setObjectPersistor(NakedObjectPersistor objectManager);

    public abstract void setSession(Session session);

    public abstract void setSpecificationLoader(NakedObjectSpecificationLoader loader);

    [JavaFlags(1028)]
    public abstract NakedObjectSpecificationLoader specificationLoader();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NakedObjects()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NakedObjects nakedObjects = this;
      ObjectImpl.clone((object) nakedObjects);
      return ((object) nakedObjects).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract string getDebugTitle();
  }
}
