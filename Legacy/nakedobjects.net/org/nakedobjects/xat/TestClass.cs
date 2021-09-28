// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestClass
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using System;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestNaked;")]
  [JavaInterface]
  public interface TestClass : TestNaked
  {
    TestObject findInstance(string title);

    TestCollection instances();

    TestObject newInstance();

    void assertActionExists(string name);

    void assertActionExists(string name, TestNaked[] parameters);

    void assertActionExists(string name, TestNaked parameter);

    void assertActionInvisible(string name);

    void assertActionInvisible(string name, TestNaked[] parameters);

    void assertActionInvisible(string name, TestNaked parameter);

    void assertActionUnusable(string name);

    void assertActionUnusable(string name, TestNaked[] parameters);

    void assertActionUnusable(string name, TestNaked parameter);

    void assertActionUsable(string name);

    void assertActionUsable(string name, TestNaked[] parameter);

    void assertActionUsable(string name, TestNaked parameter);

    void assertActionVisible(string name);

    void assertActionVisible(string name, TestNaked[] parameters);

    void assertActionVisible(string name, TestNaked parameters);

    [Obsolete(null, false)]
    TestObject invokeAction(string name);

    [Obsolete(null, false)]
    TestObject invokeAction(string name, TestNaked parameter);

    TestObject invokeAction(string name, TestNaked[] parameter);

    TestObject invokeActionReturnObject(string name, TestNaked[] parameters);

    TestCollection invokeActionReturnCollection(string name, TestNaked[] parameters);
  }
}
