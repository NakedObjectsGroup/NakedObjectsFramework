// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.JavaAcceptanceTestCase
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.@object.loader;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.persistence.objectstore.inmemory;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.repository;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.application;
using org.nakedobjects.reflector.java;
using org.nakedobjects.reflector.java.fixture;
using org.nakedobjects.reflector.java.reflect;
using org.nakedobjects.utility;
using org.nakedobjects.xat;
using System;
using System.ComponentModel;

namespace org.nakedobjects.system
{
  public abstract class JavaAcceptanceTestCase : AcceptanceTestCase
  {
    private static readonly org.apache.log4j.Logger LOG;
    private const bool PROFILER_ON = false;
    private static Profiler classProfiler;
    private Profiler methodProfiler;

    public JavaAcceptanceTestCase(string name)
      : base(name)
    {
      this.methodProfiler = new Profiler("method");
      if (JavaAcceptanceTestCase.classProfiler == null)
      {
        JavaAcceptanceTestCase.classProfiler = new Profiler("class");
      }
      else
      {
        JavaAcceptanceTestCase.classProfiler.reset();
        JavaAcceptanceTestCase.classProfiler.start();
      }
    }

    [JavaFlags(4)]
    public override FixtureBuilder createFixtureBuilder() => (FixtureBuilder) new JavaFixtureBuilder();

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        base.Finalize();
        org.apache.log4j.Logger.getLogger(Class.FromType(typeof (JavaAcceptanceTestCase))).info((object) "finalizing test case");
      }
      catch (Exception ex)
      {
      }
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Exception;")]
    public override void setUp()
    {
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test set up ").append(this.getName()).ToString());
      this.methodProfiler.start();
      base.setUp();
      this.methodProfiler.stop();
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test set up complete ").append(this.getName()).append(" ").append(this.methodProfiler.timeLog()).ToString());
    }

    [JavaThrownExceptions("1;java/lang/Exception;")]
    [JavaFlags(4)]
    public override void tearDown()
    {
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test tear down ").append(this.getName()).ToString());
      this.methodProfiler.reset();
      this.methodProfiler.start();
      base.tearDown();
      this.methodProfiler.stop();
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test tear down complete ").append(this.getName()).append(" ").append(this.methodProfiler.timeLog()).ToString());
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void runTest()
    {
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test run ").append(this.getName()).ToString());
      this.methodProfiler.reset();
      this.methodProfiler.start();
      base.runTest();
      this.methodProfiler.stop();
      JavaAcceptanceTestCase.LOG.info((object) new StringBuffer().append("test run complete ").append(this.getName()).append(" ").append(this.methodProfiler.timeLog()).ToString());
    }

    [JavaFlags(20)]
    public override sealed void setupFramework(NakedObjectsClient nakedObjects)
    {
      JavaBusinessObjectContainer businessObjectContainer = new JavaBusinessObjectContainer();
      JavaObjectFactory javaObjectFactory = new JavaObjectFactory();
      javaObjectFactory.setContainer((BusinessObjectContainer) businessObjectContainer);
      TransientObjectStore transientObjectStore = new TransientObjectStore();
      OidGenerator oidGenerator = (OidGenerator) new TimeBasedOidGenerator();
      DefaultPersistAlgorithm persistAlgorithm = new DefaultPersistAlgorithm();
      persistAlgorithm.setOidGenerator(oidGenerator);
      ObjectStorePersistor objectStorePersistor = new ObjectStorePersistor();
      objectStorePersistor.setObjectStore((NakedObjectStore) transientObjectStore);
      objectStorePersistor.setPersistAlgorithm((PersistAlgorithm) persistAlgorithm);
      nakedObjects.setObjectPersistor((NakedObjectPersistor) objectStorePersistor);
      int length = 1;
      ReflectionPeerFactory[] reflectionPeerFactoryArray = length >= 0 ? new ReflectionPeerFactory[length] : throw new NegativeArraySizeException();
      reflectionPeerFactoryArray[0] = (ReflectionPeerFactory) new TransactionPeerFactory();
      ReflectionPeerFactory[] factories = reflectionPeerFactoryArray;
      JavaSpecificationLoader specificationLoader = new JavaSpecificationLoader();
      specificationLoader.setReflectionPeerFactories(factories);
      nakedObjects.setSpecificationLoader((NakedObjectSpecificationLoader) specificationLoader);
      ObjectLoaderImpl objectLoaderImpl = new ObjectLoaderImpl();
      nakedObjects.setObjectLoader((NakedObjectLoader) objectLoaderImpl);
      objectLoaderImpl.setObjectFactory((ObjectFactory) javaObjectFactory);
      objectLoaderImpl.setPojoAdapterMap((PojoAdapterMap) new PojoAdapterHashMap());
      objectLoaderImpl.setIdentityAdapterMap((IdentityAdapterMap) new IdentityAdapterHashMap());
      objectLoaderImpl.setAdapterFactory((AdapterFactory) new JavaAdapterFactory());
      objectStorePersistor.init();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaAcceptanceTestCase()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
