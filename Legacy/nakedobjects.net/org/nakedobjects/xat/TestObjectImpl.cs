// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestObjectImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestObject;")]
  public class TestObjectImpl : AbstractTestObject, TestObject
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Hashtable fields;
    private NakedObject forObject;
    private Hashtable existingTestObjects;

    [JavaFlags(17)]
    public override sealed Naked getForNaked() => (Naked) this.forObject;

    [JavaFlags(20)]
    public NakedObject getForNakedObject() => this.forObject;

    [JavaFlags(17)]
    public object getForObject() => this.forObject.getObject();

    [JavaFlags(17)]
    public override sealed void setForNaked(Naked @object)
    {
      switch (@object)
      {
        case null:
        case NakedObject _:
          this.forObject = (NakedObject) @object;
          break;
        default:
          throw new IllegalArgumentException("Object must be a NakedObject");
      }
    }

    public TestObjectImpl(NakedObject @object, Hashtable viewCache, TestObjectFactory factory)
      : base(factory)
    {
      if (TestObjectImpl.LOG.isDebugEnabled())
        TestObjectImpl.LOG.debug((object) new StringBuffer().append("created test object for ").append((object) @object).ToString());
      this.setForNaked((Naked) @object);
      this.existingTestObjects = viewCache;
    }

    public TestObjectImpl(NakedObject @object, TestObjectFactory factory)
      : this(@object, new Hashtable(), factory)
    {
    }

    public virtual void assertEmpty(string fieldName) => this.assertEmpty((string) null, fieldName);

    public virtual void assertEmpty(string message, string fieldName)
    {
      NakedObjectField nakedObjectField = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(nakedObjectField);
      Naked forNaked = this.getField(fieldName, nakedObjectField).getForNaked();
      if (forNaked != null && !this.forObject.isEmpty(nakedObjectField))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" '").append(fieldName).append("' to be empty, but contained ").append(forNaked.titleString()).ToString());
    }

    private void assertFalse(string message, bool condition)
    {
      if (condition)
        throw new NakedAssertionFailedError(message);
    }

    public virtual void assertFieldContains(string fieldName, object expected) => this.assertFieldContains((string) null, fieldName, expected);

    public virtual void assertFieldContains(string fieldName, string expected) => this.assertFieldContains((string) null, fieldName, expected);

    public virtual void assertFieldContains(string message, string fieldName, object expected)
    {
      Naked forNaked = this.retrieveField(fieldName).getForNaked();
      if ((forNaked != null || expected != null) && (forNaked != null || !(expected is TestNakedNullParameter)) && !forNaked.getObject().Equals(expected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" value of ").append(expected).append(" but got ").append(forNaked.getObject()).ToString());
    }

    public virtual void assertFieldContains(string message, string fieldName, string expected)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      Naked forNaked = this.getField(fieldName).getForNaked();
      if (!(forNaked is InternalCollection))
      {
        string str = StringImpl.toString(forNaked.titleString());
        if (!StringImpl.equals(str, (object) expected))
          throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" value ").append(expected).append(" but got ").append(str).ToString());
      }
      else
      {
        InternalCollection nternalCollection = (InternalCollection) forNaked;
        for (int index = 0; index < nternalCollection.size(); ++index)
        {
          if (StringImpl.equals(StringImpl.toString(nternalCollection.elementAt(index).titleString()), (object) expected))
            return;
        }
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object titled '").append(expected).append("' but could not find it in the internal collection").ToString());
      }
    }

    public virtual void assertFieldContains(string message, string fieldName, TestObject expected)
    {
      Naked naked = this.retrieveNakedField(fieldName);
      if (expected == null)
      {
        if (naked is InternalCollection)
        {
          int num = ((NakedCollection) naked).size();
          if (num > 0)
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" '").append(fieldName).append("' collection to contain zero elements, but found ").append(num).ToString());
        }
        else if (naked != null)
          throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" an empty field, but found ").append((object) naked).ToString());
      }
      else
      {
        Naked forNaked = expected.getForNaked();
        if (naked == null)
        {
          if (forNaked != null)
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append((object) forNaked).append("  but found an empty field").ToString());
        }
        else if (naked is InternalCollection)
        {
          if (!((NakedCollection) naked).contains((NakedObject) forNaked))
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" '").append(fieldName).append("' collection to contain ").append((object) forNaked).ToString());
        }
        else if (!naked.Equals((object) forNaked))
          throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object of ").append((object) forNaked).append(" but got ").append((object) naked).ToString());
      }
    }

    public virtual void assertFieldContains(string fieldName, TestObject expected) => this.assertFieldContains((string) null, fieldName, expected);

    public virtual void assertFieldContainsType(string fieldName, string expected) => this.assertFieldContainsType((string) null, fieldName, expected);

    public virtual void assertFieldContainsType(string message, string fieldName, string expected)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      Naked forNaked = this.getField(fieldName).getForNaked();
      string str = forNaked != null || expected == null || StringImpl.equals(expected, (object) "") ? forNaked.getSpecification().getShortName() : throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" type ").append(expected).append(" but got nothing").ToString());
      if (!StringImpl.equals(str, (object) expected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" type ").append(expected).append(" but got ").append(str).ToString());
    }

    public virtual void assertFieldContainsType(
      string message,
      string fieldName,
      string title,
      string expected)
    {
      Naked forNaked = this.getField(fieldName).getForNaked();
      if (forNaked is InternalCollection)
      {
        InternalCollection nternalCollection = (InternalCollection) forNaked;
        for (int index = 0; index < nternalCollection.size(); ++index)
        {
          NakedObject nakedObject = nternalCollection.elementAt(index);
          if (StringImpl.equals(StringImpl.toString(nakedObject.titleString()), (object) title))
          {
            if (StringImpl.equals(nakedObject.getSpecification().getShortName(), (object) expected))
              return;
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object ").append(title).append(" to be of type ").append(expected).append(" but was ").append(nakedObject.getSpecification().getShortName()).ToString());
          }
        }
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object ").append(title).append(" but could not find it in the internal collection").ToString());
      }
    }

    public virtual void assertFieldDoesNotContain(string fieldName, NakedObject expected) => this.assertFieldDoesNotContain((string) null, this.simpleName(fieldName), expected);

    public virtual void assertFieldDoesNotContain(string fieldName, string expected) => this.assertFieldDoesNotContain((string) null, this.simpleName(fieldName), expected);

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      NakedObject notExpected)
    {
      NakedObject forNaked = (NakedObject) this.retrieveField(fieldName).getForNaked();
      if (notExpected == null)
      {
        if (forNaked == null)
          throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" empty field ").append(fieldName).ToString());
      }
      else if (forNaked != null && forNaked == notExpected)
        throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" value ").append((object) notExpected).append(" in field ").append(fieldName).ToString());
    }

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      string notExpected)
    {
      Naked naked = this.retrieveNakedField(fieldName);
      if (naked is InternalCollection)
      {
        InternalCollection nternalCollection = (InternalCollection) naked;
        for (int index = 0; index < nternalCollection.size(); ++index)
        {
          if (StringImpl.equals(StringImpl.toString(nternalCollection.elementAt(index).titleString()), (object) notExpected))
            throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" object titled '").append(notExpected).append("'  was found in the collection in field ").append(fieldName).ToString());
        }
      }
      else if (StringImpl.equals(naked != null ? naked.titleString() : "", (object) notExpected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" value ").append(notExpected).append(" in field ").append(fieldName).ToString());
    }

    public virtual void assertFieldDoesNotContain(
      string message,
      string fieldName,
      TestObject notExpected)
    {
      Naked naked = this.retrieveNakedField(fieldName);
      if (notExpected == null)
      {
        if (naked is InternalCollection)
        {
          if (((NakedCollection) naked).size() == 0)
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" '").append(fieldName).append("' collection to contain one or more elements, but it was empty").ToString());
        }
        else if (naked == null)
          throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" field to contain an object but it was empty").ToString());
      }
      else
      {
        Naked forNaked = notExpected.getForNaked();
        if (naked == null)
        {
          if (forNaked == null)
            throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" an object but the field ").append(fieldName).append("  is empty").ToString());
        }
        else if (naked is InternalCollection)
        {
          if (((NakedCollection) naked).contains((NakedObject) forNaked))
            throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" entry in '").append(fieldName).append("' collection: ").append((object) forNaked).ToString());
        }
        else if (naked.Equals((object) forNaked))
          throw new NakedAssertionFailedError(new StringBuffer().append(this.unexpected(message)).append(" object ").append((object) forNaked).append(" in field ").append(fieldName).ToString());
      }
    }

    public virtual void assertFieldDoesNotContain(string fieldName, TestObject notExpected) => this.assertFieldDoesNotContain((string) null, fieldName, notExpected);

    public virtual void assertFieldEntryCantParse(string fieldName, string value)
    {
      NakedObjectField field1 = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field1);
      this.assertFieldModifiable(fieldName, field1);
      if (!(this.fieldReference(fieldName).getForNaked() is NakedValue))
        throw new IllegalActionError("Can only make an entry (eg by keyboard) into a parsable field");
      Naked field2 = this.forObject.getField(field1);
      if (field2 == null)
        throw new NakedAssertionFailedError(new StringBuffer().append("Field '").append(fieldName).append("' contains null, but should contain an NakedValue object").ToString());
      try
      {
        ((NakedValue) field2 ?? NakedObjects.getObjectLoader().createValueInstance(field1.getSpecification())).parseTextEntry(value);
        throw new NakedAssertionFailedError("Value was unexpectedly parsed");
      }
      catch (TextEntryParseException ex)
      {
      }
      catch (InvalidEntryException ex)
      {
        throw new NakedAssertionFailedError("Value was unexpectedly parsed");
      }
    }

    public virtual void assertFieldEntryInvalid(string fieldName, string value)
    {
      NakedObjectField field = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field);
      this.assertFieldModifiable(fieldName, field);
      if (!(this.fieldReference(fieldName).getForNaked() is NakedValue))
        throw new IllegalActionError("Can only make an entry (eg by keyboard) into a value field");
      NakedValue valueInstance = NakedObjects.getObjectLoader().createValueInstance(field.getSpecification());
      if (valueInstance == null)
        throw new NakedAssertionFailedError(new StringBuffer().append("Field '").append(fieldName).append("' contains null, but should contain an NakedValue object").ToString());
      if (valueInstance == null)
        valueInstance = NakedObjects.getObjectLoader().createValueInstance(field.getSpecification());
      try
      {
        valueInstance.parseTextEntry(value);
        if (this.getForNakedObject().isValid((OneToOneAssociation) field, valueInstance).isAllowed())
          throw new NakedAssertionFailedError("Value was unexpectedly validated");
      }
      catch (InvalidEntryException ex)
      {
      }
    }

    public virtual void assertFieldExists(string fieldName)
    {
      try
      {
        this.getForNaked().getSpecification().getField(fieldName);
      }
      catch (NakedObjectSpecificationException ex)
      {
        throw new NakedAssertionFailedError(new StringBuffer().append("No field called '").append(fieldName).append("' in ").append(this.getForNaked().getSpecification().getFullName()).ToString());
      }
    }

    public virtual void assertFieldInvisible(string fieldName)
    {
      NakedObjectField field = this.fieldAccessorFor(fieldName);
      bool condition = field.isAuthorised() && this.getForNakedObject().isVisible(field).isAllowed();
      this.assertFalse(new StringBuffer().append("Field '").append(fieldName).append("' is visible").ToString(), condition);
    }

    public virtual void assertFieldModifiable(string fieldName)
    {
      NakedObjectField field = this.fieldAccessorFor(fieldName);
      this.assertFieldModifiable(fieldName, field);
    }

    private void assertFieldModifiable(string fieldName, NakedObjectField field)
    {
      this.assertFieldVisible(field);
      bool condition = this.getForNakedObject().isAvailable(field).isAllowed();
      this.assertTrue(new StringBuffer().append("Field '").append(fieldName).append("' in ").append((object) this.getForNaked()).append(" is unmodifiable:").ToString(), condition);
    }

    public virtual void assertFieldUnmodifiable(string fieldName)
    {
      bool condition = this.fieldAccessorFor(fieldName).isAvailable((NakedReference) this.getForNakedObject()).isAllowed();
      this.assertFalse(new StringBuffer().append("Field '").append(fieldName).append("' is modifiable").ToString(), condition);
    }

    public virtual void assertFieldVisible(string fieldName) => this.assertFieldVisible(this.fieldAccessorFor(fieldName));

    private void assertFieldVisible(NakedObjectField field)
    {
      bool condition = field.isAuthorised() && this.getForNakedObject().isVisible(field).isAllowed();
      this.assertTrue(new StringBuffer().append("Field '").append(field.getId()).append("' is invisible").ToString(), condition);
    }

    public virtual void assertFirstElementInField(string fieldName, string expected) => this.assertFirstElementInField("", fieldName, expected);

    public virtual void assertFirstElementInField(
      string message,
      string fieldName,
      string expected)
    {
      InternalCollection collection = this.getCollection(message, fieldName);
      if (collection.size() == 0)
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" a first element, but there are no elements").ToString());
      if (!StringImpl.equals(collection.elementAt(0).titleString(), (object) expected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object titled '").append(expected).append("' but found '").append(collection.elementAt(0).titleString()).append("'").ToString());
    }

    public virtual void assertFirstElementInField(
      string message,
      string fieldName,
      TestObject expected)
    {
      InternalCollection collection = this.getCollection(message, fieldName);
      if (collection.size() == 0)
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" a first element, but there are no elements").ToString());
      if (!collection.elementAt(0).Equals((object) expected.getForNaked()))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object '").append((object) expected.getForNaked()).append("' but found '").append((object) collection.elementAt(0)).append("'").ToString());
    }

    public virtual void assertFirstElementInField(string fieldName, TestObject expected) => this.assertFirstElementInField("", fieldName, expected);

    public virtual void assertLastElementInField(string fieldName, string expected) => this.assertLastElementInField("", fieldName, expected);

    public virtual void assertLastElementInField(string message, string fieldName, string expected)
    {
      InternalCollection collection = this.getCollection(message, fieldName);
      if (collection.size() == 0)
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" a last element, but there are no elements").ToString());
      int index = collection.size() - 1;
      if (!StringImpl.equals(collection.elementAt(index).titleString(), (object) expected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object titled '").append(expected).append("' but found '").append(collection.elementAt(index).titleString()).append("'").ToString());
    }

    public virtual void assertLastElementInField(
      string message,
      string fieldName,
      TestObject expected)
    {
      InternalCollection collection = this.getCollection(message, fieldName);
      if (collection.size() == 0)
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" a first element, but there are no elements").ToString());
      int index = collection.size() - 1;
      if (!collection.elementAt(index).Equals((object) expected.getForNaked()))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" object '").append((object) expected.getForNaked()).append("' but found '").append((object) collection.elementAt(index)).append("'").ToString());
    }

    public virtual void assertLastElementInField(string fieldName, TestObject expected) => this.assertLastElementInField("", fieldName, expected);

    public virtual void assertNoOfElements(string collectionName, int noOfElements)
    {
      int collectionSize = this.getCollectionSize(collectionName);
      if (collectionSize != noOfElements)
        throw new NakedAssertionFailedError(new StringBuffer().append("Excepted ").append(collectionName).append(" to contain ").append(noOfElements).append(" instead it contained ").append(collectionSize).ToString());
    }

    public virtual void assertNoOfElementsNotEqual(string collectionName, int noOfElements)
    {
      int collectionSize = this.getCollectionSize(collectionName);
      if (collectionSize == noOfElements)
        throw new NakedAssertionFailedError(new StringBuffer().append("Excepted ").append(collectionName).append(" to contain ").append(noOfElements).append(" instead it contained ").append(collectionSize).ToString());
    }

    public virtual void assertNotEmpty(string fieldName) => this.assertNotEmpty((string) null, fieldName);

    public virtual void assertNotEmpty(string message, string fieldName)
    {
      NakedObjectField field = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field);
      if (this.forObject.isEmpty(field))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" '").append(fieldName).append("' to contain something but it was empty").ToString());
    }

    public virtual void assertTitleEquals(string expectedTitle) => this.assertTitleEquals((string) null, expectedTitle);

    public virtual void assertTitleEquals(string message, string expectedTitle)
    {
      if (!StringImpl.equals(this.getTitle(), (object) expectedTitle))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" title of ").append((object) this.getForNaked()).append(" as '").append(expectedTitle).append("' but got '").append(this.getTitle()).append("'").ToString());
    }

    public virtual void assertType(string expected) => this.assertType("", expected);

    public virtual void assertType(string message, string expected)
    {
      string shortName = this.getForNaked().getSpecification().getShortName();
      if (!StringImpl.equals(shortName, (object) expected))
        throw new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" type ").append(expected).append(" but got ").append(shortName).ToString());
    }

    public virtual void associate(string fieldName, TestObject @object)
    {
      NakedObjectField field1 = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field1);
      this.assertFieldModifiable(fieldName, field1);
      TestNaked field2 = this.getField(fieldName);
      if (field2 is TestValue)
        throw new IllegalActionError("drop(..) not allowed on value target field; use fieldEntry(..) instead");
      if (field2.getForNaked() != null && !(field2.getForNaked() is InternalCollection))
        throw new IllegalActionError(new StringBuffer().append("Field already contains an object: ").append((object) field2.getForNaked()).ToString());
      NakedObjectField field3 = this.fieldAccessorFor(fieldName);
      NakedObject forNaked1 = (NakedObject) @object.getForNaked();
      if (field3.getSpecification() != null && !forNaked1.getSpecification().isOfType(field3.getSpecification()))
        throw new IllegalActionError(new StringBuffer().append("Can't drop a ").append(@object.getForNaked().getSpecification().getShortName()).append(" on to the ").append(fieldName).append(" field (which accepts ").append((object) field3.getSpecification()).append(")").ToString());
      NakedObject forNaked2 = (NakedObject) this.getForNaked();
      if (!field3.isAuthorised() || !forNaked2.isVisible(field3).isAllowed())
        throw new IllegalActionError(new StringBuffer().append("Cannot access the field ").append((object) field1).ToString());
      Consent consent;
      switch (field3)
      {
        case OneToOneAssociation _:
          this.assertEmpty(fieldName);
          consent = forNaked2.isValid((OneToOneAssociation) field3, forNaked1);
          break;
        case OneToManyAssociation _:
          consent = forNaked2.canAdd((OneToManyAssociation) field3, forNaked1);
          break;
        default:
          throw new NakedObjectRuntimeException();
      }
      if (consent.isVetoed())
        throw new IllegalActionError(new StringBuffer().append("Cannot associate ").append((object) forNaked1).append(" in the field ").append((object) field1).append(" within ").append((object) forNaked2).append(": ").append(consent.getReason()).ToString());
      forNaked2.setAssociation(field3, forNaked1);
    }

    public virtual void clearAssociation(string fieldName)
    {
      NakedObjectField field1 = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field1);
      this.assertFieldModifiable(fieldName, field1);
      if ((TestObject) this.fieldReference(fieldName) is TestValue)
        throw new IllegalActionError("set(..) not allowed on value target field; use fieldEntry(..) instead");
      NakedObject field2 = (NakedObject) this.forObject.getField(field1);
      if (field2 == null)
        return;
      this.forObject.clearAssociation(this.fieldAccessorFor(fieldName), field2);
    }

    public virtual void clearAssociation(string fieldName, string title)
    {
      NakedObjectField field1 = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field1);
      this.assertFieldModifiable(fieldName, field1);
      TestObject field2 = this.getField(fieldName, title);
      if (!(field1 is OneToManyAssociation))
        throw new IllegalActionError(new StringBuffer().append("removeReference not allowed on target field ").append(fieldName).ToString());
      Naked forNaked = field2.getForNaked();
      if (!(forNaked is NakedObject))
        throw new NakedAssertionFailedError(new StringBuffer().append("A NakedObject was to be removed from the InternalCollection, but found ").append((object) forNaked).ToString());
      ((OneToManyAssociation) field1).removeElement((NakedObject) this.getForNaked(), (NakedObject) forNaked);
    }

    public override bool Equals(object obj) => obj is TestObjectImpl && ((TestObjectImpl) obj).getForNaked() == this.getForNaked();

    [JavaFlags(18)]
    private string expected(string text) => new StringBuffer().append(text != null ? new StringBuffer().append(text).append("; e").ToString() : "E").append("xpected").ToString();

    [JavaFlags(18)]
    private string unexpected(string text) => new StringBuffer().append(text != null ? new StringBuffer().append(text).append("; u").ToString() : "U").append("nexpected").ToString();

    public virtual void fieldEntry(string fieldName, string textEntry)
    {
      NakedObjectField field = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(field);
      this.assertFieldModifiable(fieldName, field);
      NakedObject forNaked = (NakedObject) this.getForNaked();
      try
      {
        if (!field.isValue())
          throw new NakedAssertionFailedError("Field is not a value");
        NakedValue nakedValue = forNaked.getValue((OneToOneAssociation) field) ?? NakedObjects.getObjectLoader().createValueInstance(field.getSpecification());
        nakedValue.parseTextEntry(textEntry);
        if (forNaked.isValid((OneToOneAssociation) field, nakedValue).isVetoed())
          throw new NakedAssertionFailedError(new StringBuffer().append("Value is not valid: ").append(textEntry).ToString());
        forNaked.setValue((OneToOneAssociation) field, nakedValue.getObject());
      }
      catch (TextEntryParseException ex)
      {
        throw new IllegalActionError("");
      }
      catch (InvalidEntryException ex)
      {
        throw new IllegalActionError("");
      }
      NakedObjects.getObjectPersistor().saveChanges();
    }

    [JavaFlags(4)]
    public virtual NakedObjectField fieldAccessorFor(string fieldName) => this.getForNaked().getSpecification().getField(this.simpleName(fieldName)) ?? throw new NakedAssertionFailedError(new StringBuffer().append("No field called '").append(fieldName).append("' in ").append(ObjectImpl.getClass((object) this.getForNaked()).getName()).ToString());

    [JavaFlags(17)]
    public TestObject getAssociation(string title)
    {
      if (!(this.getForNaked() is NakedCollection))
        throw new IllegalActionError("selectByTitle will only select from a collection!");
      NakedCollection forNaked = (NakedCollection) this.getForNaked();
      Enumeration enumeration = forNaked.elements();
      NakedObject @object = (NakedObject) null;
      int num = 0;
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        if (StringImpl.indexOf(StringImpl.toString(nakedObject.titleString()), title) >= 0)
        {
          @object = nakedObject;
          ++num;
        }
      }
      if (num == 0)
        throw new IllegalActionError(new StringBuffer().append("selectByTitle must find an object within ").append((object) forNaked).ToString());
      if (num > 1)
        throw new IllegalActionError(new StringBuffer().append("selectByTitle must select only one object within ").append((object) forNaked).ToString());
      return this.factory.createTestObject(@object);
    }

    private InternalCollection getCollection(string message, string fieldName)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      Naked forNaked = this.getField(fieldName).getForNaked();
      if (!(forNaked is InternalCollection))
      {
        NakedAssertionFailedError assertionFailedError = new NakedAssertionFailedError(new StringBuffer().append(this.expected(message)).append(" a collection but got ").append((object) forNaked).ToString());
      }
      return (InternalCollection) forNaked;
    }

    private int getCollectionSize(string collectionName)
    {
      Naked forNaked = this.getField(collectionName).getForNaked();
      return forNaked is InternalCollection ? ((NakedCollection) forNaked).size() : throw new NakedAssertionFailedError(new StringBuffer().append(collectionName).append(" is not a collection").ToString());
    }

    [JavaFlags(4)]
    public virtual TestNaked fieldReference(string fieldName)
    {
      this.loadFieldsFromObject();
      return (TestNaked) this.fields.get((object) this.simpleName(fieldName));
    }

    private void loadFieldsFromObject()
    {
      if (this.fields != null)
        return;
      this.fields = new Hashtable();
      Naked forNaked = this.getForNaked();
      if (forNaked == null)
        return;
      this.existingTestObjects.put((object) forNaked, (object) this);
      NakedObjectField[] fields = forNaked.getSpecification().getFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        NakedObjectField field1 = fields[index];
        TestNaked testNaked = (TestNaked) null;
        Naked field2 = ((NakedObject) forNaked).getField(field1);
        if (field1.isValue())
        {
          testNaked = (TestNaked) this.factory.createTestValue((NakedValue) field2);
        }
        else
        {
          if (field2 != null)
            testNaked = (TestNaked) this.existingTestObjects.get((object) field2);
          if (testNaked == null)
            testNaked = !field1.isCollection() ? (TestNaked) this.factory.createTestObject((NakedObject) field2, this.existingTestObjects) : (TestNaked) this.factory.createTestCollection((NakedCollection) field2);
        }
        this.fields.put((object) fields[index].getId(), (object) testNaked);
      }
    }

    public virtual TestNaked getField(string fieldName)
    {
      NakedObjectField nakedObjectField = this.fieldAccessorFor(fieldName);
      this.assertFieldVisible(nakedObjectField);
      return this.getField(fieldName, nakedObjectField);
    }

    [JavaFlags(4)]
    public virtual TestNaked getField(string fieldName, NakedObjectField fieldAccessor)
    {
      TestNaked testNaked = this.fieldReference(fieldName);
      Naked field = this.forObject.getField(fieldAccessor);
      if (field is NakedObject)
      {
        NakedObject @object = (NakedObject) field;
        if (@object.getResolveState().isResolvable(ResolveState.RESOLVING))
          NakedObjects.getObjectPersistor().resolveImmediately(@object);
      }
      testNaked.setForNaked(field);
      return testNaked;
    }

    public virtual TestObject getFieldAsObject(string fieldName) => (TestObject) this.getField(fieldName);

    public virtual TestValue getFieldAsValue(string fieldName) => (TestValue) this.getField(fieldName);

    public virtual TestObject getField(string fieldName, string title)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      Naked forNaked = this.getField(fieldName).getForNaked();
      Enumeration enumeration = forNaked is InternalCollection ? ((NakedCollection) forNaked).elements() : throw new IllegalActionError("getField(String, String) only allows an object to be selected from an InternalCollection");
      NakedObject @object = (NakedObject) null;
      int num = 0;
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        if (StringImpl.equals(StringImpl.toString(nakedObject.titleString()), (object) title))
        {
          @object = nakedObject;
          ++num;
        }
      }
      if (num == 0)
        throw new IllegalActionError(new StringBuffer().append("The field '").append(fieldName).append("' must contain an object titled '").append(title).append("' within it").ToString());
      if (num > 1)
        throw new IllegalActionError(new StringBuffer().append("The field '").append(fieldName).append("' must only contain one object titled '").append(title).append("' within it").ToString());
      if (@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        NakedObjects.getObjectPersistor().resolveImmediately(@object);
      return this.factory.createTestObject(@object);
    }

    public virtual string getFieldTitle(string fieldName)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      return this.getField(fieldName).getForNaked() != null ? this.getField(fieldName).getTitle() : throw new IllegalActionError(new StringBuffer().append("No object to get title from in field ").append(fieldName).append(" within ").append((object) this.getForNaked()).ToString());
    }

    public override string getTitle()
    {
      if (this.getForNaked() == null)
        throw new IllegalActionError("??");
      return StringImpl.toString(this.getForNaked().titleString());
    }

    private TestNaked retrieveField(string fieldName)
    {
      this.assertFieldVisible(this.fieldAccessorFor(fieldName));
      return this.getField(fieldName);
    }

    private Naked retrieveNakedField(string fieldName) => this.retrieveField(fieldName).getForNaked();

    public virtual void testField(string fieldName, string expected) => this.testField(fieldName, expected, expected);

    public virtual void testField(string fieldName, string setValue, string expected)
    {
      this.fieldEntry(fieldName, setValue);
      junit.framework.Assert.assertEquals(new StringBuffer().append("Field '").append(fieldName).append("' contains unexpected value").ToString(), expected, this.getField(fieldName).getTitle());
    }

    public virtual void testField(string fieldName, TestObject expected)
    {
      this.associate(fieldName, expected);
      junit.framework.Assert.assertEquals((object) expected.getForNaked(), (object) this.getField(fieldName).getForNaked());
    }

    public override string ToString() => this.forObject == null ? (string) null : this.forObject.ToString();

    [JavaFlags(4)]
    public override Action getAction(string name, NakedObjectSpecification[] parameterClasses) => this.getForNaked().getSpecification().getObjectAction(Action.USER, name, parameterClasses);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TestObjectImpl()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
