// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.html.HtmlTestObjectFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using System;

namespace org.nakedobjects.xat.html
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestObjectFactory;")]
  public class HtmlTestObjectFactory : TestObjectFactory
  {
    private static HtmlDocumentor documentor;

    public virtual TestClass createTestClass(NakedClass cls) => (TestClass) new HtmlTestClass((TestClass) new TestClassImpl(cls, (TestObjectFactory) this), HtmlTestObjectFactory.documentor);

    public virtual TestObject createTestObject(NakedObject @object) => (TestObject) new HtmlTestObject((TestObject) new TestObjectImpl(@object, (TestObjectFactory) this), HtmlTestObjectFactory.documentor);

    public virtual TestCollection createTestCollection(NakedCollection collection) => (TestCollection) new TestCollectionImpl(collection);

    public virtual TestObject createTestObject(NakedObject field, Hashtable viewCache) => (TestObject) new HtmlTestObject((TestObject) new TestObjectImpl(field, viewCache, (TestObjectFactory) this), HtmlTestObjectFactory.documentor);

    public virtual TestValue createTestValue(NakedValue @object) => (TestValue) new HtmlTestValue(new TestValueImpl(@object), HtmlTestObjectFactory.documentor);

    public virtual void testStarting(string className, string methodName)
    {
      string name = StringImpl.substring(className, StringImpl.lastIndexOf(className, 46) + 1);
      HtmlTestObjectFactory.documentor.open(className, name);
      HtmlTestObjectFactory.documentor.title(methodName);
    }

    public virtual void testEnding() => HtmlTestObjectFactory.documentor.doc("<hr>");

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        HtmlTestObjectFactory.documentor.close();
        // ISSUE: explicit finalizer call
        base.Finalize();
      }
      catch (Exception ex)
      {
      }
    }

    public virtual TestValue createParamerTestValue(object value) => (TestValue) new ParameterValueImpl(value);

    public virtual Documentor getDocumentor()
    {
      if (HtmlTestObjectFactory.documentor == null)
        HtmlTestObjectFactory.documentor = new HtmlDocumentor("tmp/");
      return (Documentor) HtmlTestObjectFactory.documentor;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      HtmlTestObjectFactory testObjectFactory = this;
      ObjectImpl.clone((object) testObjectFactory);
      return ((object) testObjectFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
