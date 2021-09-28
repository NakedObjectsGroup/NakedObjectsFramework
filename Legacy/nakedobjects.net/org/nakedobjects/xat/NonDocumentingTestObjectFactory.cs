// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.NonDocumentingTestObjectFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestObjectFactory;")]
  public class NonDocumentingTestObjectFactory : TestObjectFactory
  {
    public virtual TestValue createParamerTestValue(object value) => (TestValue) new ParameterValueImpl(value);

    public virtual TestClass createTestClass(NakedClass cls) => (TestClass) new TestClassImpl(cls, (TestObjectFactory) this);

    public virtual TestCollection createTestCollection(NakedCollection instances) => (TestCollection) new TestCollectionImpl(instances);

    public virtual TestObject createTestObject(NakedObject @object) => (TestObject) new TestObjectImpl(@object, (TestObjectFactory) this);

    public virtual TestObject createTestObject(NakedObject field, Hashtable viewCache) => (TestObject) new TestObjectImpl(field, viewCache, (TestObjectFactory) this);

    public virtual TestValue createTestValue(NakedValue @object) => (TestValue) new TestValueImpl(@object);

    public virtual Documentor getDocumentor() => (Documentor) new NullDocumentor();

    public virtual void testEnding()
    {
    }

    public virtual void testStarting(string className, string methodName)
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NonDocumentingTestObjectFactory testObjectFactory = this;
      ObjectImpl.clone((object) testObjectFactory);
      return ((object) testObjectFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
