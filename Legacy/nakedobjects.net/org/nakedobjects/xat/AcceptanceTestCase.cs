// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.AcceptanceTestCase
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using junit.framework;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.@object.repository;
using System;
using System.ComponentModel;

namespace org.nakedobjects.xat
{
  public abstract class AcceptanceTestCase : TestCase
  {
    private static readonly Logger LOG;
    private const string TEST_OBJECT_FACTORY = "test-object-factory";
    [JavaFlags(28)]
    public static readonly TestNaked[] NO_PARAMETERS;
    private Hashtable classes;
    private Documentor documentor;
    private TestObjectFactory testObjectFactory;
    private FixtureBuilder fixtureBuilder;
    private NakedObjectsClient nakedObjects;

    public AcceptanceTestCase()
      : this((string) null)
    {
    }

    public AcceptanceTestCase(string name)
      : base(name)
    {
      this.classes = new Hashtable();
      if (new File("xat.properties").exists())
      {
        PropertyConfigurator.configure("xat.properties");
        Logger.getLogger(Class.FromType(typeof (AcceptanceTestCase))).debug((object) new StringBuffer().append("XAT Logging enabled - first test: ").append(this.getName()).ToString());
      }
      else
        BasicConfigurator.configure();
    }

    [JavaFlags(4)]
    public virtual void append(string text) => this.docln(text);

    private void docln(string @string) => this.documentor.docln(@string);

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        AcceptanceTestCase.LOG.info((object) "finalizing acceptance test");
      }
      catch (Exception ex)
      {
      }
    }

    [JavaFlags(4)]
    public virtual TestClass getTestClass(string name)
    {
      TestClass testClass = (TestClass) this.classes.get((object) StringImpl.toLowerCase(name));
      NakedObject forNaked = (NakedObject) testClass.getForNaked();
      if (forNaked.getResolveState().isPersistent())
        NakedObjects.getObjectPersistor().resolveImmediately(forNaked);
      return testClass != null ? testClass : throw new IllegalArgumentException(new StringBuffer().append("Invalid class name ").append(name).ToString());
    }

    [JavaFlags(4)]
    public virtual void note(string text) => this.docln(text);

    public virtual void addFixture(Fixture fixture) => this.fixtureBuilder.addFixture(fixture);

    public virtual TestValue createParameterTestValue(object value) => this.testObjectFactory.createParamerTestValue(value);

    public virtual TestNaked createNullParameter(Class cls) => (TestNaked) new TestNakedNullParameter(cls);

    public virtual TestNaked createNullParameter(string cls) => (TestNaked) new TestNakedNullParameter(cls);

    public override void run(TestResult result)
    {
      AcceptanceTestCase.LOG.info((object) new StringBuffer().append("run ").append(this.getName()).ToString());
      base.run(result);
    }

    [JavaThrownExceptions("1;java/lang/Exception;")]
    [JavaFlags(4)]
    public override void setUp()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(1028)]
    public abstract FixtureBuilder createFixtureBuilder();

    [JavaFlags(1028)]
    public abstract void setupFramework(NakedObjectsClient nakedObjects);

    [JavaFlags(1028)]
    public abstract void setUpFixtures();

    [JavaFlags(4)]
    public virtual void startDocumenting() => this.documentor.start();

    [JavaFlags(4)]
    public virtual void nextStep() => this.documentor.step("");

    [JavaFlags(4)]
    public virtual void nextStep(string text) => this.documentor.step(text);

    [JavaFlags(4)]
    public virtual void firstStep()
    {
      this.startDocumenting();
      this.nextStep();
    }

    [JavaFlags(4)]
    public virtual void firstStep(string text)
    {
      this.startDocumenting();
      this.nextStep(text);
    }

    [JavaFlags(4)]
    public virtual void stopDocumenting() => this.documentor.stop();

    [JavaFlags(4)]
    public virtual void subtitle(string text) => this.documentor.subtitle(text);

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Exception;")]
    public override void tearDown()
    {
      this.documentor.stop();
      AcceptanceTestCase.LOG.info((object) new StringBuffer().append("tear down ").append(this.getName()).ToString());
      this.nakedObjects.shutdown();
      this.classes.clear();
      this.classes = (Hashtable) null;
      this.fixtureBuilder = (FixtureBuilder) null;
      this.documentor = (Documentor) null;
      this.testObjectFactory.testEnding();
      this.testObjectFactory = (TestObjectFactory) null;
      base.tearDown();
    }

    [JavaFlags(4)]
    public virtual void title(string text) => this.documentor.title(text);

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(object parameter1)
    {
      int length = 1;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(object parameter1, object parameter2)
    {
      int length = 2;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      testNakedArray[1] = this.asTestNaked(parameter2);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(
      object parameter1,
      object parameter2,
      object parameter3)
    {
      int length = 3;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      testNakedArray[1] = this.asTestNaked(parameter2);
      testNakedArray[2] = this.asTestNaked(parameter3);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(
      object parameter1,
      object parameter2,
      object parameter3,
      object parameter4)
    {
      int length = 4;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      testNakedArray[1] = this.asTestNaked(parameter2);
      testNakedArray[2] = this.asTestNaked(parameter3);
      testNakedArray[3] = this.asTestNaked(parameter4);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(
      object parameter1,
      object parameter2,
      object parameter3,
      object parameter4,
      object parameter5)
    {
      int length = 5;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      testNakedArray[1] = this.asTestNaked(parameter2);
      testNakedArray[2] = this.asTestNaked(parameter3);
      testNakedArray[3] = this.asTestNaked(parameter4);
      testNakedArray[4] = this.asTestNaked(parameter5);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked[] parameters(
      object parameter1,
      object parameter2,
      object parameter3,
      object parameter4,
      object parameter5,
      object parameter6)
    {
      int length = 6;
      TestNaked[] testNakedArray = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      testNakedArray[0] = this.asTestNaked(parameter1);
      testNakedArray[1] = this.asTestNaked(parameter2);
      testNakedArray[2] = this.asTestNaked(parameter3);
      testNakedArray[3] = this.asTestNaked(parameter4);
      testNakedArray[4] = this.asTestNaked(parameter5);
      testNakedArray[5] = this.asTestNaked(parameter6);
      return testNakedArray;
    }

    [JavaFlags(4)]
    public virtual TestNaked asTestNaked(object parameter) => parameter is TestNaked ? (TestNaked) parameter : (TestNaked) null;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AcceptanceTestCase()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
