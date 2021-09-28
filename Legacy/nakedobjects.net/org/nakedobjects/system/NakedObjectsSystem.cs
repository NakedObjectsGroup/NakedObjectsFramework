// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.NakedObjectsSystem
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.@object.help;
using org.nakedobjects.@object.loader;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.persistence.objectstore.inmemory;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.repository;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.application;
using org.nakedobjects.reflector.java;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.reflector.java.reflect;
using org.nakedobjects.utility;
using org.nakedobjects.utility.configuration;
using org.nakedobjects.viewer.skylark;
using System.ComponentModel;

namespace org.nakedobjects.system
{
  public class NakedObjectsSystem
  {
    private const string LOGGING_PROPERTIES = "nakedobjects.properties";
    private const string DEFAULT_CONFIG = "nakedobjects.properties";
    private static readonly org.apache.log4j.Logger LOG;
    private SplashWindow splash;
    private string configurationFile;

    public virtual void setConfigurationFile(string configurationFile) => this.configurationFile = configurationFile;

    public virtual void init()
    {
      PropertyConfigurator.configure("nakedobjects.properties");
      AboutNakedObjects.logVersion();
      string name = ObjectImpl.getClass((object) this).getName();
      StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
      this.splash = (SplashWindow) null;
      try
      {
        PropertiesConfiguration configuration = new PropertiesConfiguration((ConfigurationLoader) new PropertiesFileLoader(this.configurationFile, false));
        if (!configuration.getBoolean("nosplash", false))
          this.splash = new SplashWindow();
        this.setUpLocale(configuration.getString("locale"));
        ObjectStorePersistor objectPersistor = this.createObjectPersistor();
        NakedObjectSpecificationLoader specificationLoader = this.createSpecificationLoader();
        AdapterFactory reflectorFactory = this.createReflectorFactory();
        ObjectLoaderImpl objectLoader = this.createObjectLoader();
        this.setupNakedObjects(configuration, objectPersistor, specificationLoader, reflectorFactory, objectLoader).init();
      }
      finally
      {
        if (this.splash != null)
          this.splash.removeAfterDelay(4);
      }
    }

    private NakedObjectsClient setupNakedObjects(
      PropertiesConfiguration configuration,
      ObjectStorePersistor objectManager,
      NakedObjectSpecificationLoader specificationLoader,
      AdapterFactory adapterFactory,
      ObjectLoaderImpl objectLoader)
    {
      NakedObjectsClient nakedObjectsClient = new NakedObjectsClient();
      nakedObjectsClient.setObjectPersistor((NakedObjectPersistor) objectManager);
      nakedObjectsClient.setSpecificationLoader(specificationLoader);
      nakedObjectsClient.setConfiguration((NakedObjectConfiguration) configuration);
      nakedObjectsClient.setObjectLoader((NakedObjectLoader) objectLoader);
      nakedObjectsClient.setSession((Session) new SimpleSession());
      return nakedObjectsClient;
    }

    [JavaFlags(4)]
    public virtual ObjectLoaderImpl createObjectLoader()
    {
      BusinessObjectContainer container = (BusinessObjectContainer) new JavaBusinessObjectContainer();
      JavaObjectFactory javaObjectFactory = new JavaObjectFactory();
      javaObjectFactory.setContainer(container);
      JavaAdapterFactory javaAdapterFactory = new JavaAdapterFactory();
      ObjectLoaderImpl objectLoaderImpl = new ObjectLoaderImpl();
      objectLoaderImpl.setAdapterFactory((AdapterFactory) javaAdapterFactory);
      objectLoaderImpl.setObjectFactory((ObjectFactory) javaObjectFactory);
      objectLoaderImpl.setPojoAdapterMap((PojoAdapterMap) new PojoAdapterHashMap());
      objectLoaderImpl.setIdentityAdapterMap((IdentityAdapterMap) new IdentityAdapterHashMap());
      return objectLoaderImpl;
    }

    [JavaFlags(4)]
    public virtual AdapterFactory createReflectorFactory() => (AdapterFactory) new JavaAdapterFactory();

    [JavaFlags(4)]
    public virtual NakedObjectSpecificationLoader createSpecificationLoader()
    {
      JavaSpecificationLoader specificationLoader = new JavaSpecificationLoader();
      int length = 2;
      ReflectionPeerFactory[] reflectionPeerFactoryArray = length >= 0 ? new ReflectionPeerFactory[length] : throw new NegativeArraySizeException();
      reflectionPeerFactoryArray[0] = (ReflectionPeerFactory) new TransactionPeerFactory();
      reflectionPeerFactoryArray[1] = (ReflectionPeerFactory) new HelpPeerFactory();
      ReflectionPeerFactory[] factories = reflectionPeerFactoryArray;
      specificationLoader.setReflectionPeerFactories(factories);
      return (NakedObjectSpecificationLoader) specificationLoader;
    }

    [JavaFlags(4)]
    public virtual ObjectStorePersistor createObjectPersistor()
    {
      TransientObjectStore transientObjectStore = new TransientObjectStore();
      OidGenerator oidGenerator = (OidGenerator) new SimpleOidGenerator();
      DefaultPersistAlgorithm persistAlgorithm = new DefaultPersistAlgorithm();
      persistAlgorithm.setOidGenerator(oidGenerator);
      ObjectStorePersistor objectStorePersistor = new ObjectStorePersistor();
      objectStorePersistor.setObjectStore((NakedObjectStore) transientObjectStore);
      objectStorePersistor.setPersistAlgorithm((PersistAlgorithm) persistAlgorithm);
      return objectStorePersistor;
    }

    private void setUpLocale(string localeSpec)
    {
      if (localeSpec != null)
      {
        int num = StringImpl.indexOf(localeSpec, 95);
        Locale locale = num != -1 ? new Locale(StringImpl.substring(localeSpec, 0, num), StringImpl.substring(localeSpec, num + 1)) : new Locale(localeSpec, "");
        Locale.setDefault(locale);
        if (NakedObjectsSystem.LOG.isInfoEnabled())
          NakedObjectsSystem.LOG.info((object) new StringBuffer().append("Locale set to ").append((object) locale).ToString());
      }
      if (!NakedObjectsSystem.LOG.isDebugEnabled())
        return;
      NakedObjectsSystem.LOG.debug((object) new StringBuffer().append("locale is ").append((object) Locale.getDefault()).ToString());
    }

    public virtual void displayUserInterface(string[] classes) => this.displayUserInterface(classes, (string) null);

    public virtual void displayUserInterface(string[] classes, string title)
    {
      SkylarkViewer skylarkViewer = new SkylarkViewer();
      skylarkViewer.setUpdateNotifier(new ViewUpdateNotifier());
      skylarkViewer.setExploration(true);
      skylarkViewer.setShutdownListener((ObjectViewingMechanismListener) new NakedObjectsSystem.\u0031(this));
      if (title != null)
        skylarkViewer.setTitle(title);
      DefaultApplicationContext applicationContext = new DefaultApplicationContext();
      for (int index = 0; index < classes.Length; ++index)
        applicationContext.addClass(classes[index]);
      skylarkViewer.setApplication((UserContext) applicationContext);
      skylarkViewer.init();
    }

    public virtual void clearSplash()
    {
      if (this.splash == null)
        return;
      this.splash.toFront();
      this.splash.removeAfterDelay(4);
    }

    public NakedObjectsSystem() => this.configurationFile = "nakedobjects.properties";

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static NakedObjectsSystem()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NakedObjectsSystem nakedObjectsSystem = this;
      ObjectImpl.clone((object) nakedObjectsSystem);
      return ((object) nakedObjectsSystem).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/nakedobjects/event/ObjectViewingMechanismListener;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : ObjectViewingMechanismListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private NakedObjectsSystem this\u00240;

      public virtual void viewerClosing()
      {
        ((PrintStream) java.lang.System.@out).println("EXITED");
        java.lang.System.exit(0);
      }

      public \u0031(NakedObjectsSystem _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        NakedObjectsSystem.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
