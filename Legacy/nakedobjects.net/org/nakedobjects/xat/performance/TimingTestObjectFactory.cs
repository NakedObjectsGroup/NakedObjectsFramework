// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.TimingTestObjectFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat.performance
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestObjectFactory;")]
  public class TimingTestObjectFactory : TestObjectFactory
  {
    private static TimingDocumentor documentor;

    public virtual TestClass createTestClass(NakedClass cls) => (TestClass) new TimingTestClass((TestClass) new TestClassImpl(cls, (TestObjectFactory) this), TimingTestObjectFactory.documentor);

    public virtual TestObject createTestObject(NakedObject @object) => (TestObject) new TimingTestObject((TestObject) new TestObjectImpl(@object, (TestObjectFactory) this), TimingTestObjectFactory.documentor);

    public virtual TestCollection createTestCollection(NakedCollection collection) => (TestCollection) new TestCollectionImpl(collection);

    public virtual TestObject createTestObject(NakedObject field, Hashtable viewCache) => (TestObject) new TimingTestObject((TestObject) new TestObjectImpl(field, viewCache, (TestObjectFactory) this), TimingTestObjectFactory.documentor);

    public virtual TestValue createTestValue(NakedValue @object) => (TestValue) new TestValueImpl(@object);

    public virtual void testStarting(string className, string methodName)
    {
    }

    public virtual Documentor getDocumentor()
    {
      if (TimingTestObjectFactory.documentor == null)
        TimingTestObjectFactory.documentor = new TimingDocumentor("\\tmp\\timing");
      return (Documentor) TimingTestObjectFactory.documentor;
    }

    public virtual void testEnding()
    {
    }

    public virtual TestValue createParamerTestValue(object value) => (TestValue) new ParameterValueImpl(value);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TimingTestObjectFactory testObjectFactory = this;
      ObjectImpl.clone((object) testObjectFactory);
      return ((object) testObjectFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
