using NakedFramework.RATL.Classic.Helpers;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using NakedObjects.Resources;
using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;
using System;

namespace NakedFramework.RATL.Classic.NonDocumenting; 

public class TestProperty : ITestProperty {
    private IMember member;

    public TestProperty(IMember member, AcceptanceTestCase acceptanceTestCase) {
        this.member = member;
        AcceptanceTestCase = acceptanceTestCase;
    }

    public AcceptanceTestCase AcceptanceTestCase { get; }

    public string Name => member.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.friendlyName);

    public string Id => member.GetId();

    public string Title {
        get {
            var mask = member.SafeGetExtension(ExtensionsApi.ExtensionKeys.x_ro_nof_mask)?.ToString();
            if (string.IsNullOrEmpty(mask)) {
                return Content.Title;
            }

            var rawValue = Content is TestValue testValue ? testValue.Value : null;
            var toString = rawValue?.GetType().GetMethod("ToString", new[] { typeof(string) });
            return toString?.Invoke(rawValue, new object[] { mask }) as string;
        }
    }

    public string LastMessage { get; private set; }
    public ITestNaked GetDefault() => throw new NotImplementedException();

    public ITestNaked[] GetChoices() {
        if (member is PropertyMember property) {
            var details = property.GetDetails(AcceptanceTestCase.TestInvokeOptions()).Result;
            return RATLHelpers.GetChoices(details, AcceptanceTestCase);
        }

        return Array.Empty<ITestNaked>();
    }

    public ITestNaked[] GetCompletions(string autoCompleteParm) => throw new NotImplementedException();

    //public ITestProperty SetValue(string textEntry) {
    //    if (member is PropertyMember property) {
    //        property.Wrapped["value"] = new JValue(textEntry);
    //    }

    //    return this;
    //}



    public ITestNaked Content =>
        member switch {
            IProperty property when property.GetLinkValue() is { } link => new TestObject(ROSIApi.GetObject(link.GetHref(), AcceptanceTestCase.TestInvokeOptions()).Result, AcceptanceTestCase),
            IProperty property => new TestValue(property.ConvertValue()),
            CollectionMember collection => new TestCollection(collection.GetDetails(AcceptanceTestCase.TestInvokeOptions()).Result, AcceptanceTestCase),
            IHasValue collection => new TestCollection(collection, AcceptanceTestCase),
            _ => null
        };

    public ITestObject ContentAsObject => (ITestObject)Content;

    public ITestCollection ContentAsCollection => (ITestCollection)Content;

    public ITestProperty SetObject(ITestObject testObject) {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        Assert.IsFalse(IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");

        try {
            var details = member switch {
                PropertyMember property => property.GetDetails(AcceptanceTestCase.TestInvokeOptions()).Result,
                PropertyDetails pd => pd,
                CollectionMember or CollectionDetails => null,
                _ => null
            };

            if (details is not null) {
                member = details.SetValue(testObject?.Value, AcceptanceTestCase.TestInvokeOptions()).Result;
            }
            else {
                Assert.IsFalse(true, $"Cannot SetObject {testObject} in the field {Id}: Always disabled");
            }
        }
        catch (AggregateException ex) {
            LastMessage = ex.InnerException?.Message ?? "";
            Assert.Fail($"Cannot SetObject {testObject} in the field {Id}: {LastMessage}");
        }

        return this;
    }

    public ITestProperty RemoveFromCollection(ITestObject testObject)
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        //Assert.IsTrue(field.IsCollection, "Cannot remove from non collection");

        //INakedObject testNakedObject = testObject.NakedObject;

        //Assert.IsTrue(testNakedObject.Specification.IsOfType(field.Specification),
        //    string.Format("Can't clear a {0} from the {1} field (which accepts {2})", testObject.NakedObject.Specification.ShortName, Name, field.Specification));

        //INakedObject nakedObject = owningObject.NakedObject;

        //if (!(field is IOneToManyAssociation))
        //{
        //    throw new UnknownTypeException(field);
        //}
        //IConsent valid = new Veto("Always disabled");

        //Assert.IsFalse(valid.IsVetoed, string.Format("Can't remove {0} from the field {1} within {2}: {3}", testNakedObject, field, nakedObject, valid.Reason));
        return this;
    }


    /// <summary>
    ///     Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
    ///     option that each object field offers by default.
    /// </summary>
    public ITestProperty ClearObject()
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        Assert.IsFalse(IsParseable, "Clear(..) not allowed on value target field; use SetValue(..) instead");

        SetObject(null);

        //INakedObject nakedObject = field.GetNakedObject(owningObject.NakedObject);
        //if (nakedObject != null)
        //{
        //    if (field is IOneToOneAssociation)
        //    {
        //        ((IOneToOneAssociation)field).SetAssociation(owningObject.NakedObject, null);
        //    }
        //    else
        //    {
        //        Assert.Fail("Clear(..) not allowed on collection target field");
        //    }
        //}
        return this;
    }

    public ITestProperty SetValue(string textEntry)
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        try {
            var details = member switch {
                PropertyMember property => property.GetDetails(AcceptanceTestCase.TestInvokeOptions()).Result,
                PropertyDetails pd => pd,
                CollectionMember or CollectionDetails => null,
                _ => null
            };

            if (details is not null) {
                member = details.SetValue(textEntry, AcceptanceTestCase.TestInvokeOptions()).Result;
            }
            else {
                Assert.IsFalse(true, $"Cannot SetValue {textEntry} in the field {Id}: Always disabled");
            }
        }
        catch (AggregateException ex) {
            LastMessage = ex.InnerException?.Message ?? "";
            Assert.Fail($"Cannot SetValue {textEntry} in the field {Id}: {LastMessage}");
        }
        return this;
    }

    public ITestProperty ClearValue()
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        SetValue(null);
        return this;
    }

    public ITestProperty TestField(string setValue, string expected)
    {
        SetValue(setValue);
        Assert.AreEqual($"Field '{Name}' contains unexpected value", expected, Content.Title);
        return this;
    }

    public ITestProperty TestField(ITestObject expected)
    {
        SetObject(expected);
        Assert.AreEqual(expected, Content);
        return this;
    }

    private void ResetLastMessage()
    {
        LastMessage = string.Empty;
    }

    private bool IsNotParseable => !IsParseable;

    private bool IsParseable => ReturnType is "boolean" or "number" or "string";

    public ITestProperty AssertCannotParse(string text)
    {
        AssertIsVisible();
        AssertIsModifiable();

        //try {
        //    SetValue(text);
        //}

        //INakedObject valueObject = field.GetNakedObject(owningObject.NakedObject);

        //Assert.IsNotNull(valueObject, "Field '" + Name + "' contains null, but should contain an INakedObject object");
        //try
        //{
        //    var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
        //    parseableFacet.ParseTextEntry(text);
        //    Assert.Fail("Content was unexpectedly parsed");
        //}
        //catch (InvalidEntryException /*expected*/)
        //{
        //    expected
        //}
        return this;
    }

    public ITestProperty AssertFieldEntryInvalid(string text)
    {
        return IsNotParseable ? AssertNotParseable() : AssertParseableFieldEntryInvalid(text);
    }

    public ITestProperty AssertFieldEntryIsValid(string text)
    {
        return IsNotParseable ? AssertNotParseable() : AssertParseableFieldEntryIsValid(text);
    }

    public ITestProperty AssertSetObjectInvalid(ITestObject testObject)
    {
        try
        {
            AssertSetObjectIsValid(testObject);
        }
        catch (AssertFailedException)
        {
            // expected 
            return this;
        }
        Assert.Fail($"Object {testObject} was allowed in field {Id} : expected it to be invalid");
        return this;
    }

    public ITestProperty AssertSetObjectIsValid(ITestObject testObject)
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        //Assert.IsFalse(field.Specification.IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");
        //INakedObject testNakedObject = testObject.NakedObject;
        //Assert.IsTrue(testNakedObject.Specification.IsOfType(field.Specification), string.Format("Can't drop a {0} on to the {1} field (which accepts {2})", testObject.NakedObject.Specification.ShortName, Name, field.Specification));
        //INakedObject nakedObject = owningObject.NakedObject;
        //IConsent valid;
        //if (field is IOneToOneAssociation)
        //{
        //    valid = ((IOneToOneAssociation)field).IsAssociationValid(nakedObject, testNakedObject);
        //}
        //else if (field is IOneToManyAssociation)
        //{
        //    valid = new Veto("Always disabled");
        //}
        //else
        //{
        //    throw new UnknownTypeException(field);
        //}
        //LastMessage = valid.Reason;

        //Assert.IsFalse(valid.IsVetoed, string.Format("Cannot SetObject {0} in the field {1} within {2}: {3}", testNakedObject, field, nakedObject, valid.Reason));
        return this;
    }

    public ITestProperty AssertIsInvisible()
    {
        //bool canAccess = field.IsVisible(NakedObjectsContext.Session, owningObject.NakedObject);
        //Assert.IsFalse(canAccess, "Field '" + Name + "' is visible");
        return this;
    }

    public ITestProperty AssertIsVisible() {
        Assert.IsTrue(member is not null, $"Field '{Name}' is invisible");
        return this;
    }

    public ITestProperty AssertIsMandatory() {
        var isMandatory = IsOptional;
        Assert.IsTrue(!IsOptional, $"Field '{Id}' is optional");
        return this;
    }

    private bool IsOptional => member.GetExtensions().GetExtension<bool>(ExtensionsApi.ExtensionKeys.optional);

    public ITestProperty AssertIsOptional()
    {
        Assert.IsTrue(IsOptional, $"Field '{Id}' is mandatory");
        return this;
    }

    private string Description => member.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.description);

    private string ReturnType => member.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType);

    public ITestProperty AssertIsDescribedAs(string expected)
    {
        AssertIsVisible();
        Assert.IsTrue(expected.Equals(Description), $"Description expected: '{expected}' actual: '{Description}'");
        return this;
    }

    public ITestProperty AssertIsModifiable()
    {
        AssertIsVisible();
        ResetLastMessage();

        //IConsent isUsable = field.IsUsable(NakedObjectsContext.Session, owningObject.NakedObject);
        //LastMessage = isUsable.Reason;

        //bool canUse = isUsable.IsAllowed;
        //Assert.IsTrue(canUse, "Field '" + Name + "' in " + owningObject.NakedObject + " is unmodifiable");
        return this;
    }

    public ITestProperty AssertIsUnmodifiable()
    {
        //ResetLastMessage();
        //IConsent isUsable = field.IsUsable(NakedObjectsContext.Session, owningObject.NakedObject);
        //LastMessage = isUsable.Reason;

        //bool canUse = isUsable.IsAllowed;
        //Assert.IsFalse(canUse, "Field '" + Name + "' in " + owningObject.NakedObject + " is modifiable");
        return this;
    }

    private bool IsEmpty() {
        return Content == null || string.IsNullOrEmpty(Content.Title);
    }

    public ITestProperty AssertIsNotEmpty()
    {
        AssertIsVisible();
        Assert.IsFalse(IsEmpty(), $"Expected '{Name}' to contain something but it was empty");
        return this;
    }

    public ITestProperty AssertIsEmpty()
    {
        AssertIsVisible();
        Assert.IsTrue(IsEmpty(), $"Expected '{Name}' to be empty");
        return this;
    }

    public ITestProperty AssertValueIsEqual(string expected) {
        AssertIsVisible();
        Assert.AreEqual(expected, Content.Title);
        return this;
    }

    public ITestProperty AssertTitleIsEqual(string expected) {
        AssertIsVisible();
        Assert.AreEqual(expected, Title);
        return this;
    }

    public ITestProperty AssertObjectIsEqual(ITestNaked expected)
    {
        AssertIsVisible();
        Assert.AreEqual(expected, Content);
        return this;
    }

    public ITestProperty AssertIsValidToSave()
    {
        //if (field.IsMandatory && field.IsVisible(NakedObjectsContext.Session, owningObject.NakedObject) && field.IsUsable(NakedObjectsContext.Session, owningObject.NakedObject).IsAllowed)
        //{
        //    Assert.IsFalse(field.IsEmpty(owningObject.NakedObject), "Cannot save object as mandatory field " + " '" + Name + "' is empty");
        //    Assert.IsTrue(field.GetNakedObject(owningObject.NakedObject).ValidToPersist() == null);
        //}
        //if (field.IsCollection)
        //{
        //    field.GetNakedObject(owningObject.NakedObject).GetAsEnumerable().ForEach(no => Assert.AreEqual(no.ValidToPersist(), null));
        //}

        return this;
    }

    public ITestProperty AssertLastMessageIs(string message)
    {
        Assert.IsTrue(message.Equals(LastMessage), "Last message expected: '" + message + "' actual: '" + LastMessage + "'");
        return this;
    }

    public ITestProperty AssertLastMessageContains(string message)
    {
        Assert.IsTrue(LastMessage.Contains(message), "Last message expected to contain: '" + message + "' actual: '" + LastMessage + "'");
        return this;
    }

    public ITestProperty AssertParseableFieldEntryInvalid(string text)
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        //INakedObject nakedObject = owningObject.NakedObject;
        //INakedObject existingValue = field.GetNakedObject(nakedObject);
        //var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
        //try
        //{
        //    INakedObject newValue = parseableFacet.ParseTextEntry(text);
        //    IConsent isAssociationValid = ((IOneToOneAssociation)field).IsAssociationValid(owningObject.NakedObject, newValue);
        //    LastMessage = isAssociationValid.Reason;
        //    Assert.IsFalse(isAssociationValid.IsAllowed, "Content was unexpectedly validated");
        //}
        //catch (InvalidEntryException)
        //{
        //    // expected 
        //}
        return this;
    }

    public ITestProperty AssertParseableFieldEntryIsValid(string text)
    {
        AssertIsVisible();
        AssertIsModifiable();
        ResetLastMessage();

        //INakedObject nakedObject = owningObject.NakedObject;
        //INakedObject existingValue = field.GetNakedObject(nakedObject);
        //var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
        //INakedObject newValue = parseableFacet.ParseTextEntry(text);
        //IConsent isAssociationValid = ((IOneToOneAssociation)field).IsAssociationValid(owningObject.NakedObject, newValue);
        //LastMessage = isAssociationValid.Reason;
        //Assert.IsTrue(isAssociationValid.IsAllowed, "Content was unexpectedly invalidated");
        return this;
    }

    private ITestProperty AssertNotParseable()
    {
        Assert.Fail("Not a parseable field");
        return this;
    }
}