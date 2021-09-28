// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestObjectDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestObject;")]
  public abstract class TestObjectDecorator : TestObject
  {
    private readonly TestObject wrappedObject;

    public TestObjectDecorator(TestObject wrappedObject) => this.wrappedObject = wrappedObject;

    public virtual void assertActionExists(string name) => this.wrappedObject.assertActionExists(name);

    public virtual void assertActionExists(string name, TestNaked parameter) => this.wrappedObject.assertActionExists(name, parameter);

    public virtual void assertActionExists(string name, TestNaked[] parameters) => this.wrappedObject.assertActionExists(name, parameters);

    public virtual void assertActionInvisible(string name) => this.wrappedObject.assertActionInvisible(name);

    public virtual void assertActionInvisible(string name, TestNaked parameter) => this.wrappedObject.assertActionInvisible(name, parameter);

    public virtual void assertActionInvisible(string name, TestNaked[] parameters) => this.wrappedObject.assertActionInvisible(name, parameters);

    public virtual void assertActionUnusable(string name) => this.wrappedObject.assertActionUnusable(name);

    public virtual void assertActionUnusable(string name, TestNaked parameter) => this.wrappedObject.assertActionUnusable(name, parameter);

    public virtual void assertActionUnusable(string name, TestNaked[] parameters) => this.wrappedObject.assertActionUnusable(name, parameters);

    public virtual void assertActionUsable(string name) => this.wrappedObject.assertActionUsable(name);

    public virtual void assertActionUsable(string name, TestNaked parameter) => this.wrappedObject.assertActionUsable(name, parameter);

    public virtual void assertActionUsable(string name, TestNaked[] parameters) => this.wrappedObject.assertActionUsable(name, parameters);

    public virtual void assertActionVisible(string name) => this.wrappedObject.assertActionVisible(name);

    public virtual void assertActionVisible(string name, TestNaked parameter) => this.wrappedObject.assertActionVisible(name, parameter);

    public virtual void assertActionVisible(string name, TestNaked[] parameters) => this.wrappedObject.assertActionVisible(name, parameters);

    public virtual void assertEmpty(string fieldName) => this.wrappedObject.assertEmpty(fieldName);

    public virtual void assertEmpty(string message, string fieldName) => this.wrappedObject.assertEmpty(message, fieldName);

    public virtual void assertFieldContains(string fieldName, object expectedValue) => this.wrappedObject.assertFieldContains(fieldName, expectedValue);

    public virtual void assertFieldContains(string fieldName, string expectedValue) => this.wrappedObject.assertFieldContains(fieldName, expectedValue);

    public virtual void assertFieldContains(string message, string fieldName, object expectedValue) => this.wrappedObject.assertFieldContains(message, fieldName, expectedValue);

    public virtual void assertFieldContains(string message, string fieldName, string expectedValue) => this.wrappedObject.assertFieldContains(message, fieldName, expectedValue);

    public virtual void assertFieldContains(
      string message,
      string fieldName,
      TestObject expectedView)
    {
      this.wrappedObject.assertFieldContains(message, fieldName, expectedView);
    }

    public virtual void assertFieldContains(string fieldName, TestObject expectedView) => this.wrappedObject.assertFieldContains(fieldName, expectedView);

    public virtual void assertFieldContainsType(string fieldName, string expectedType) => this.wrappedObject.assertFieldContainsType(fieldName, expectedType);

    public virtual void assertFieldContainsType(
      string message,
      string fieldName,
      string expectedType)
    {
      this.wrappedObject.assertFieldContainsType(fieldName, expectedType);
    }

    public virtual void assertFieldContainsType(
      string message,
      string fieldName,
      string title,
      string expectedType)
    {
      this.wrappedObject.assertFieldContainsType(message, fieldName, title, expectedType);
    }

    public virtual void assertFieldDoesNotContain(string fieldName, NakedObject testValue) => this.wrappedObject.assertFieldDoesNotContain(fieldName, testValue);

    public virtual void assertFieldDoesNotContain(string fieldName, string testValue) => this.wrappedObject.assertFieldDoesNotContain(fieldName, testValue);

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      NakedObject testValue)
    {
      this.wrappedObject.assertFieldDoesNotContain(message, fieldName, testValue);
    }

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      string testValue)
    {
      this.wrappedObject.assertFieldDoesNotContain(message, fieldName, testValue);
    }

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      TestObject testView)
    {
      this.wrappedObject.assertFieldDoesNotContain(message, fieldName, testView);
    }

    public virtual void assertFieldDoesNotContain(string fieldName, TestObject testView) => this.wrappedObject.assertFieldDoesNotContain(fieldName, testView);

    public virtual void assertFieldEntryCantParse(string fieldName, string value) => this.wrappedObject.assertFieldEntryCantParse(fieldName, value);

    public virtual void assertFieldEntryInvalid(string fieldName, string value) => this.wrappedObject.assertFieldEntryInvalid(fieldName, value);

    public virtual void assertFieldExists(string fieldName) => this.wrappedObject.assertFieldExists(fieldName);

    public virtual void assertFieldInvisible(string fieldName) => this.wrappedObject.assertFieldInvisible(fieldName);

    public virtual void assertFieldModifiable(string fieldName) => this.wrappedObject.assertFieldModifiable(fieldName);

    public virtual void assertFieldUnmodifiable(string fieldName) => this.wrappedObject.assertFieldUnmodifiable(fieldName);

    public virtual void assertFieldVisible(string fieldName) => this.wrappedObject.assertFieldVisible(fieldName);

    public virtual void assertFirstElementInField(string fieldName, string expected) => this.wrappedObject.assertFirstElementInField(fieldName, expected);

    public virtual void assertFirstElementInField(
      string message,
      string fieldName,
      string expected)
    {
      this.wrappedObject.assertFirstElementInField(message, fieldName, expected);
    }

    public virtual void assertFirstElementInField(
      string message,
      string fieldName,
      TestObject expected)
    {
      this.wrappedObject.assertFirstElementInField(message, fieldName, expected);
    }

    public virtual void assertFirstElementInField(string fieldName, TestObject expected) => this.wrappedObject.assertFirstElementInField(fieldName, expected);

    public virtual void assertLastElementInField(string fieldName, string expected) => this.wrappedObject.assertLastElementInField(fieldName, expected);

    public virtual void assertLastElementInField(string message, string fieldName, string expected) => this.wrappedObject.assertLastElementInField(message, fieldName, expected);

    public virtual void assertLastElementInField(
      string message,
      string fieldName,
      TestObject expected)
    {
      this.wrappedObject.assertLastElementInField(message, fieldName, expected);
    }

    public virtual void assertLastElementInField(string fieldName, TestObject expected) => this.wrappedObject.assertLastElementInField(fieldName, expected);

    public virtual void assertNoOfElements(string collectionName, int noOfElements) => this.wrappedObject.assertNoOfElements(collectionName, noOfElements);

    public virtual void assertNoOfElementsNotEqual(string collectionName, int noOfElements) => this.wrappedObject.assertNoOfElementsNotEqual(collectionName, noOfElements);

    public virtual void assertNotEmpty(string fieldName) => this.wrappedObject.assertNotEmpty(fieldName);

    public virtual void assertNotEmpty(string message, string fieldName) => this.wrappedObject.assertNotEmpty(message, fieldName);

    public virtual void assertTitleEquals(string expectedTitle) => this.wrappedObject.assertTitleEquals(expectedTitle);

    public virtual void assertTitleEquals(string message, string expectedTitle) => this.wrappedObject.assertTitleEquals(message, expectedTitle);

    public virtual void assertType(string expectedType) => this.wrappedObject.assertType(expectedType);

    public virtual void assertType(string message, string expectedType) => this.wrappedObject.assertType(message, expectedType);

    public virtual void associate(string fieldName, TestObject draggedView) => this.wrappedObject.associate(fieldName, draggedView);

    public virtual void clearAssociation(string fieldName) => this.wrappedObject.clearAssociation(fieldName);

    public virtual void clearAssociation(string fieldName, string title) => this.wrappedObject.clearAssociation(fieldName, title);

    public virtual void fieldEntry(string name, string value) => this.wrappedObject.fieldEntry(name, value);

    public virtual Action getAction(string name) => this.wrappedObject.getAction(name);

    public virtual TestObject getAssociation(string title) => this.wrappedObject.getAssociation(title);

    public virtual TestNaked getField(string fieldName) => this.wrappedObject.getField(fieldName);

    public virtual TestObject getField(string fieldName, string title) => this.wrappedObject.getField(fieldName, title);

    public virtual TestObject getFieldAsObject(string fieldName) => this.wrappedObject.getFieldAsObject(fieldName);

    public virtual TestValue getFieldAsValue(string fieldName) => this.wrappedObject.getFieldAsValue(fieldName);

    public virtual string getFieldTitle(string field) => this.wrappedObject.getFieldTitle(field);

    public virtual Naked getForNaked() => this.wrappedObject.getForNaked();

    public virtual object getForObject() => this.wrappedObject.getForObject();

    public virtual string getTitle() => this.wrappedObject.getTitle();

    public virtual TestObject invokeAction(string name) => this.wrappedObject.invokeAction(name);

    public virtual TestObject invokeAction(string name, TestNaked parameter) => this.wrappedObject.invokeAction(name, parameter);

    public virtual TestObject invokeAction(string name, TestNaked[] parameters) => this.wrappedObject.invokeAction(name, parameters);

    public virtual TestObject invokeActionReturnObject(
      string name,
      TestNaked[] parameters)
    {
      return this.wrappedObject.invokeActionReturnObject(name, parameters);
    }

    public virtual TestCollection invokeActionReturnCollection(
      string name,
      TestNaked[] parameters)
    {
      return this.wrappedObject.invokeActionReturnCollection(name, parameters);
    }

    public virtual void setForNaked(Naked @object) => this.wrappedObject.setForNaked(@object);

    public virtual void testField(string fieldName, string expectedValue) => this.wrappedObject.testField(fieldName, expectedValue);

    public virtual void testField(string fieldName, string setValue, string expectedValue) => this.wrappedObject.testField(fieldName, setValue, expectedValue);

    public virtual void testField(string fieldName, TestObject expectedObject) => this.wrappedObject.testField(fieldName, expectedObject);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TestObjectDecorator testObjectDecorator = this;
      ObjectImpl.clone((object) testObjectDecorator);
      return ((object) testObjectDecorator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
