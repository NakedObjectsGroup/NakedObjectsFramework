// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestNaked;")]
  [JavaInterface]
  public interface TestObject : TestNaked
  {
    void assertActionExists(string name);

    void assertActionExists(string name, TestNaked parameter);

    void assertActionExists(string name, TestNaked[] parameters);

    void assertActionInvisible(string name);

    void assertActionInvisible(string name, TestNaked parameter);

    void assertActionInvisible(string name, TestNaked[] parameters);

    void assertActionUnusable(string name);

    void assertActionUnusable(string name, TestNaked parameter);

    void assertActionUnusable(string name, TestNaked[] parameters);

    void assertActionUsable(string name);

    void assertActionUsable(string name, TestNaked parameter);

    void assertActionUsable(string name, TestNaked[] parameter);

    void assertActionVisible(string name);

    void assertActionVisible(string name, TestNaked parameter);

    void assertActionVisible(string name, TestNaked[] parameters);

    void assertEmpty(string fieldName);

    void assertEmpty(string message, string fieldName);

    void assertFieldContains(string fieldName, object expected);

    void assertFieldContains(string fieldName, string expected);

    void assertFieldContains(string message, string fieldName, object expected);

    void assertFieldContains(string message, string fieldName, string expected);

    void assertFieldContains(string message, string fieldName, TestObject expected);

    void assertFieldContains(string fieldName, TestObject expected);

    void assertFieldContainsType(string fieldName, string expected);

    void assertFieldContainsType(string message, string fieldName, string expected);

    void assertFieldContainsType(string message, string fieldName, string title, string expected);

    void assertFieldDoesNotContain(string fieldName, NakedObject expected);

    void assertFieldDoesNotContain(string fieldName, string expected);

    void assertFieldDoesNotContain(string message, string fieldName, NakedObject expected);

    void assertFieldDoesNotContain(string message, string fieldName, string expected);

    void assertFieldDoesNotContain(string message, string fieldName, TestObject expected);

    void assertFieldDoesNotContain(string fieldName, TestObject expected);

    void assertFieldEntryCantParse(string fieldName, string value);

    void assertFieldEntryInvalid(string fieldName, string value);

    void assertFieldExists(string fieldName);

    void assertFieldInvisible(string fieldName);

    void assertFieldModifiable(string fieldName);

    void assertFieldUnmodifiable(string fieldName);

    void assertFieldVisible(string fieldName);

    void assertFirstElementInField(string fieldName, string expected);

    void assertFirstElementInField(string message, string fieldName, string expected);

    void assertFirstElementInField(string message, string fieldName, TestObject expected);

    void assertFirstElementInField(string fieldName, TestObject expected);

    void assertLastElementInField(string fieldName, string expected);

    void assertLastElementInField(string message, string fieldName, string expected);

    void assertLastElementInField(string message, string fieldName, TestObject expected);

    void assertLastElementInField(string fieldName, TestObject expected);

    void assertNoOfElements(string collectionName, int noOfElements);

    void assertNoOfElementsNotEqual(string collectionName, int noOfElements);

    void assertNotEmpty(string fieldName);

    void assertNotEmpty(string message, string fieldName);

    void assertTitleEquals(string expectedTitle);

    void assertTitleEquals(string message, string expectedTitle);

    void assertType(string expected);

    void assertType(string message, string expected);

    void associate(string fieldName, TestObject draggedView);

    void clearAssociation(string fieldName);

    void clearAssociation(string fieldName, string title);

    void fieldEntry(string name, string value);

    Action getAction(string name);

    TestObject getAssociation(string title);

    TestNaked getField(string fieldName);

    TestObject getField(string fieldName, string title);

    TestObject getFieldAsObject(string fieldName);

    TestValue getFieldAsValue(string fieldName);

    string getFieldTitle(string field);

    object getForObject();

    TestObject invokeAction(string name);

    TestObject invokeAction(string name, TestNaked parameter);

    TestObject invokeAction(string name, TestNaked[] parameter);

    TestObject invokeActionReturnObject(string @string, TestNaked[] parameter);

    TestCollection invokeActionReturnCollection(string @string, TestNaked[] parameter);

    void testField(string fieldName, string expected);

    void testField(string fieldName, string setValue, string expected);

    void testField(string fieldName, TestObject expected);
  }
}
