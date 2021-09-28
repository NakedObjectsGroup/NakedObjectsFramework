// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestObjectFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterface]
  public interface TestObjectFactory
  {
    TestValue createParamerTestValue(object value);

    TestClass createTestClass(NakedClass cls);

    TestCollection createTestCollection(NakedCollection instances);

    TestObject createTestObject(NakedObject @object);

    TestObject createTestObject(NakedObject field, Hashtable cache);

    TestValue createTestValue(NakedValue @object);

    Documentor getDocumentor();

    void testEnding();

    void testStarting(string className, string methodName);
  }
}
