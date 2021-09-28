// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.AbstractSystem
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.@object.help;
using org.nakedobjects.@object.loader;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.repository;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.application;
using org.nakedobjects.application.system;
using org.nakedobjects.application.valueholder;
using org.nakedobjects.reflector.java;
using org.nakedobjects.reflector.java.fixture;
using org.nakedobjects.reflector.java.reflect;
using org.nakedobjects.utility.configuration;
using org.nakedobjects.viewer.skylark;
using System.ComponentModel;

namespace org.nakedobjects.system
{
  public abstract class AbstractSystem
  {
    private const string DEFAULT_CONFIG = "nakedobjects.properties";
    private const string SHOW_EXPLORATION_OPTIONS = "viewer.lightweight.show-exploration";
    private static readonly Logger LOG;
    [JavaFlags(4)]
    public PropertiesConfiguration configuration;
    private NakedObjectsClient nakedObjects;

    [JavaFlags(4)]
    public virtual void init()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual NakedObjectSpecificationLoader createReflector()
    {
      HelpManagerAssist helpManagerAssist = new HelpManagerAssist();
      helpManagerAssist.setDecorated((HelpManager) new SimpleHelpManager());
      HelpPeerFactory helpPeerFactory = new HelpPeerFactory();
      helpPeerFactory.setHelpManager((HelpManager) helpManagerAssist);
      int length = 2;
      ReflectionPeerFactory[] reflectionPeerFactoryArray = length >= 0 ? new ReflectionPeerFactory[length] : throw new NegativeArraySizeException();
      reflectionPeerFactoryArray[0] = (ReflectionPeerFactory) helpPeerFactory;
      reflectionPeerFactoryArray[1] = (ReflectionPeerFactory) new TransactionPeerFactory();
      ReflectionPeerFactory[] factories = reflectionPeerFactoryArray;
      JavaSpecificationLoader specificationLoader = new JavaSpecificationLoader();
      specificationLoader.setReflectionPeerFactories(factories);
      return (NakedObjectSpecificationLoader) specificationLoader;
    }

    [JavaFlags(4)]
    public virtual NakedObjectLoader createLoader()
    {
      SystemClock systemClock = new SystemClock();
      org.nakedobjects.application.valueholder.Date.setClock((Clock) systemClock);
      org.nakedobjects.application.valueholder.Time.setClock((Clock) systemClock);
      DateTime.setClock((Clock) systemClock);
      org.nakedobjects.application.value.Date.setClock((Clock) systemClock);
      JavaObjectFactory javaObjectFactory = new JavaObjectFactory();
      JavaBusinessObjectContainer businessObjectContainer = new JavaBusinessObjectContainer();
      javaObjectFactory.setContainer((BusinessObjectContainer) businessObjectContainer);
      ObjectLoaderImpl objectLoaderImpl = new ObjectLoaderImpl();
      objectLoaderImpl.setObjectFactory((ObjectFactory) javaObjectFactory);
      objectLoaderImpl.setPojoAdapterMap((PojoAdapterMap) new PojoAdapterHashMap());
      objectLoaderImpl.setIdentityAdapterMap((IdentityAdapterMap) new IdentityAdapterHashMap());
      objectLoaderImpl.setAdapterFactory((AdapterFactory) new JavaAdapterFactory());
      return (NakedObjectLoader) objectLoaderImpl;
    }

    [JavaFlags(1028)]
    public abstract NakedObjectPersistor createPersistor();

    [JavaFlags(4)]
    public virtual void setupFixtures()
    {
    }

    [JavaFlags(4)]
    public virtual void shutdown() => this.nakedObjects.shutdown();

    [JavaFlags(4)]
    public virtual void installFixtures(string key)
    {
      JavaFixtureBuilder javaFixtureBuilder = new JavaFixtureBuilder();
      string str = this.configuration.getString(key);
      if (str == null)
        return;
      StringTokenizer stringTokenizer = new StringTokenizer(str, ",");
      while (stringTokenizer.hasMoreTokens())
      {
        Fixture fixture = (Fixture) StartUp.loadComponent(StringImpl.trim(stringTokenizer.nextToken()), Class.FromType(typeof (JavaFixture)));
        javaFixtureBuilder.addFixture(fixture);
      }
      javaFixtureBuilder.installFixtures();
    }

    [JavaFlags(4)]
    public virtual NakedObjectStore installObjectStore(string key) => (NakedObjectStore) StartUp.loadComponent(this.configuration.getString(key));

    [JavaFlags(4)]
    public virtual void displayUserInterface()
    {
      SkylarkViewer skylarkViewer = new SkylarkViewer();
      ViewUpdateNotifier updateNotifier = new ViewUpdateNotifier();
      skylarkViewer.setUpdateNotifier(updateNotifier);
      skylarkViewer.setShutdownListener((ObjectViewingMechanismListener) new AbstractSystem.\u0031(this));
      string str = this.configuration.getString("nakedobjects.classes");
      if (str == null)
        throw new StartupException("No classes specified");
      DefaultApplicationContext applicationContext = new DefaultApplicationContext();
      StringTokenizer stringTokenizer = new StringTokenizer(str, ",");
      while (stringTokenizer.hasMoreTokens())
        applicationContext.addClass(StringImpl.trim(stringTokenizer.nextToken()));
      skylarkViewer.setApplication((UserContext) applicationContext);
      skylarkViewer.setExploration(true);
      skylarkViewer.init();
    }

    [JavaFlags(4)]
    public virtual NakedObjectsClient createRepository() => new NakedObjectsClient();

    private void setUpLocale()
    {
      string str = NakedObjects.getConfiguration().getString("locale");
      if (str != null)
      {
        int num = StringImpl.indexOf(str, 95);
        Locale locale = num != -1 ? new Locale(StringImpl.substring(str, 0, num), StringImpl.substring(str, num + 1)) : new Locale(str, "");
        Locale.setDefault(locale);
        if (AbstractSystem.LOG.isInfoEnabled())
          AbstractSystem.LOG.info((object) new StringBuffer().append("locale set to ").append((object) locale).ToString());
      }
      if (!AbstractSystem.LOG.isDebugEnabled())
        return;
      AbstractSystem.LOG.debug((object) new StringBuffer().append("locale is ").append((object) Locale.getDefault()).ToString());
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractSystem()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractSystem abstractSystem = this;
      ObjectImpl.clone((object) abstractSystem);
      return ((object) abstractSystem).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/event/ObjectViewingMechanismListener;")]
    [Inner]
    public class \u0031 : ObjectViewingMechanismListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private AbstractSystem this\u00240;

      public virtual void viewerClosing()
      {
        this.this\u00240.shutdown();
        java.lang.System.exit(0);
      }

      public \u0031(AbstractSystem _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        AbstractSystem.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
