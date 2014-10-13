// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Xat {
    public class TestProperty : ITestProperty {
        private readonly ITestObjectFactory factory;
        private readonly INakedObjectManager manager;
        private readonly ILifecycleManager lifecycleManager;
        private readonly ISession session;
        private readonly IObjectPersistor persistor;
        private readonly INakedObjectAssociation field;
        private readonly ITestHasActions owningObject;

        public TestProperty(ILifecycleManager lifecycleManager, ISession session, IObjectPersistor persistor, INakedObjectAssociation field, ITestHasActions owningObject, ITestObjectFactory factory, INakedObjectManager manager) {
            this.lifecycleManager = lifecycleManager;
            this.session = session;
            this.persistor = persistor;
            this.field = field;
            this.owningObject = owningObject;
            this.factory = factory;
            this.manager = manager;
        }

        #region ITestProperty Members

        public string Name {
            get { return field.GetName(); }
        }

        public string Id {
            get { return field.Id; }
        }

        public string Title {
            get { return field.PropertyTitle(field.GetNakedObject(owningObject.NakedObject), manager); }
        }

        public ITestNaked Content {
            get {
                INakedObject fieldValue = field.GetNakedObject(owningObject.NakedObject);
                if (fieldValue != null && fieldValue.ResolveState.IsResolvable()) {
                    persistor.ResolveImmediately(fieldValue);
                }

                return factory.CreateTestNaked(fieldValue);
            }
        }

        public ITestObject ContentAsObject {
            get { return (ITestObject) Content; }
        }

        public ITestCollection ContentAsCollection {
            get { return (ITestCollection) Content; }
        }

        public ITestNaked GetDefault() {
            INakedObject defaultValue = field.GetDefault(owningObject.NakedObject);
            return factory.CreateTestNaked(defaultValue);
        }

        public ITestNaked[] GetChoices() {
            INakedObject[] choices = ((NakedObjectAssociationAbstract) field).GetChoices(owningObject.NakedObject, null);
            return choices.Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked[] GetCompletions(string autoCompleteParm) {
            INakedObject[] completions = ((NakedObjectAssociationAbstract) field).GetCompletions(owningObject.NakedObject, autoCompleteParm);
            return completions.Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestProperty SetObject(ITestObject testObject) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsFalse(field.Specification.IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");

            INakedObject testNakedObject = testObject.NakedObject;

            Assert.IsTrue(testNakedObject.Specification.IsOfType(field.Specification), string.Format("Can't drop a {0} on to the {1} field (which accepts {2})", testObject.NakedObject.Specification.ShortName, Name, field.Specification));

            INakedObject nakedObject = owningObject.NakedObject;

            IConsent valid;
            if (field is IOneToOneAssociation) {
                valid = ((IOneToOneAssociation) field).IsAssociationValid(nakedObject, testNakedObject);
            }
            else if (field is IOneToManyAssociation) {
                valid = new Veto("Always disabled");
            }
            else {
                throw new UnknownTypeException(field);
            }

            LastMessage = valid.Reason;
            Assert.IsFalse(valid.IsVetoed, string.Format("Cannot SetObject {0} in the field {1} within {2}: {3}", testNakedObject, field, nakedObject, valid.Reason));

            if (field is IOneToOneAssociation) {
                ((IOneToOneAssociation) field).SetAssociation(nakedObject, testNakedObject);
            }
           
            return this;
        }

        public ITestProperty RemoveFromCollection(ITestObject testObject) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsTrue(field.IsCollection, "Cannot remove from non collection");

            INakedObject testNakedObject = testObject.NakedObject;

            Assert.IsTrue(testNakedObject.Specification.IsOfType(field.Specification),
                string.Format("Can't clear a {0} from the {1} field (which accepts {2})", testObject.NakedObject.Specification.ShortName, Name, field.Specification));

            INakedObject nakedObject = owningObject.NakedObject;

            if (!(field is IOneToManyAssociation)) {
                throw new UnknownTypeException(field);
            }
            IConsent valid = new Veto("Always disabled");
            

            Assert.IsFalse(valid.IsVetoed, string.Format("Can't remove {0} from the field {1} within {2}: {3}", testNakedObject, field, nakedObject, valid.Reason));
            return this;
        }

        public string LastMessage { get; private set; }

        /// <summary>
        ///     Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
        ///     option that each object field offers by default.
        /// </summary>
        public ITestProperty ClearObject() {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsFalse(field.Specification.IsParseable, "Clear(..) not allowed on value target field; use SetValue(..) instead");

            INakedObject nakedObject = field.GetNakedObject(owningObject.NakedObject);
            if (nakedObject != null) {
                if (field is IOneToOneAssociation) {
                    ((IOneToOneAssociation) field).SetAssociation(owningObject.NakedObject, null);
                }
                else {
                    Assert.Fail("Clear(..) not allowed on collection target field");
                }
            }
            return this;
        }

        public ITestProperty SetValue(string textEntry) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            INakedObject nakedObject = owningObject.NakedObject;
            try {
                INakedObject existingValue = field.GetNakedObject(nakedObject);

                var parseableFacet = field.Specification.GetFacet<IParseableFacet>();

                INakedObject newValue = parseableFacet.ParseTextEntry(textEntry, manager);

                IConsent consent = ((IOneToOneAssociation) field).IsAssociationValid(nakedObject, newValue);
                LastMessage = consent.Reason;

                Assert.IsFalse(consent.IsVetoed, string.Format("Content: '{0}' is not valid. Reason: {1}", textEntry, consent.Reason));

                if (textEntry.Trim().Equals("")) {
                    ((IOneToOneAssociation) field).SetAssociation(nakedObject, null);
                }
                else {
                    ((IOneToOneAssociation) field).SetAssociation(nakedObject, newValue);
                }
            }
            catch (InvalidEntryException) {
                Assert.Fail("Entry not recognised " + textEntry);
            }
            return this;
        }

        public ITestProperty ClearValue() {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            INakedObject nakedObject = owningObject.NakedObject;
            try {
                IConsent consent = ((IOneToOneAssociation) field).IsAssociationValid(nakedObject, null);
                LastMessage = consent.Reason;
                Assert.IsFalse(consent.IsVetoed, string.Format("Content: 'null' is not valid. Reason: {0}", consent.Reason));
                ((IOneToOneAssociation) field).SetAssociation(nakedObject, null);
            }
            catch (InvalidEntryException) {
                Assert.Fail("Null Entry not recognised ");
            }
            return this;
        }

        public ITestProperty TestField(string setValue, string expected) {
            SetValue(setValue);
            Assert.AreEqual("Field '" + Name + "' contains unexpected value", expected, Content.Title);
            return this;
        }

        public ITestProperty TestField(ITestObject expected) {
            SetObject(expected);
            Assert.AreEqual(expected.NakedObject, Content.NakedObject);
            return this;
        }

        private void ResetLastMessage() {
            LastMessage = string.Empty;
        }

        #endregion

        private bool IsNotParseable() {
            return field.Specification.GetFacet<IParseableFacet>() == null;
        }

        #region Asserts

        public ITestProperty AssertCannotParse(string text) {
            AssertIsVisible();
            AssertIsModifiable();

            INakedObject valueObject = field.GetNakedObject(owningObject.NakedObject);

            Assert.IsNotNull(valueObject, "Field '" + Name + "' contains null, but should contain an INakedObject object");
            try {
                var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
                parseableFacet.ParseTextEntry(text, manager);
                Assert.Fail("Content was unexpectedly parsed");
            }
            catch (InvalidEntryException /*expected*/) {
                // expected
            }
            return this;
        }

        public ITestProperty AssertFieldEntryInvalid(string text) {
            return IsNotParseable() ? AssertNotParseable() : AssertParseableFieldEntryInvalid(text);
        }

        public ITestProperty AssertFieldEntryIsValid(string text) {
            return IsNotParseable() ? AssertNotParseable() : AssertParseableFieldEntryIsValid(text);
        }

        public ITestProperty AssertSetObjectInvalid(ITestObject testObject) {
            try {
                AssertSetObjectIsValid(testObject);
            }
            catch (AssertFailedException) {
                // expected 
                return this;
            }
            Assert.Fail("Object {0} was allowed in field {1} : expected it to be invalid", testObject, field);
            return this;
        }

        public ITestProperty AssertSetObjectIsValid(ITestObject testObject) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsFalse(field.Specification.IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");
            INakedObject testNakedObject = testObject.NakedObject;
            Assert.IsTrue(testNakedObject.Specification.IsOfType(field.Specification), string.Format("Can't drop a {0} on to the {1} field (which accepts {2})", testObject.NakedObject.Specification.ShortName, Name, field.Specification));
            INakedObject nakedObject = owningObject.NakedObject;
            IConsent valid;
            if (field is IOneToOneAssociation) {
                valid = ((IOneToOneAssociation) field).IsAssociationValid(nakedObject, testNakedObject);
            }
            else if (field is IOneToManyAssociation) {
                valid = new Veto("Always disabled");
            }
            else {
                throw new UnknownTypeException(field);
            }
            LastMessage = valid.Reason;

            Assert.IsFalse(valid.IsVetoed, string.Format("Cannot SetObject {0} in the field {1} within {2}: {3}", testNakedObject, field, nakedObject, valid.Reason));
            return this;
        }

        public ITestProperty AssertIsInvisible() {
            bool canAccess = field.IsVisible(owningObject.NakedObject);
            Assert.IsFalse(canAccess, "Field '" + Name + "' is visible");
            return this;
        }

        public ITestProperty AssertIsVisible() {
            bool canAccess = field.IsVisible(owningObject.NakedObject);
            Assert.IsTrue(canAccess, "Field '" + Name + "' is invisible");
            return this;
        }

        public ITestProperty AssertIsMandatory() {
            Assert.IsTrue(field.IsMandatory, "Field '" + field.Id + "' is optional");
            return this;
        }

        public ITestProperty AssertIsOptional() {
            Assert.IsTrue(!field.IsMandatory, "Field '" + field.Id + "' is mandatory");
            return this;
        }

        public ITestProperty AssertIsDescribedAs(string expected) {
            AssertIsVisible();
            Assert.IsTrue(expected.Equals(field.Description), "Description expected: '" + expected + "' actual: '" + field.Description + "'");
            return this;
        }

        public ITestProperty AssertIsModifiable() {
            AssertIsVisible();
            ResetLastMessage();

            IConsent isUsable = field.IsUsable(owningObject.NakedObject);
            LastMessage = isUsable.Reason;

            bool canUse = isUsable.IsAllowed;
            Assert.IsTrue(canUse, "Field '" + Name + "' in " + owningObject.NakedObject + " is unmodifiable");
            return this;
        }

        public ITestProperty AssertIsUnmodifiable() {
            ResetLastMessage();
            IConsent isUsable = field.IsUsable(owningObject.NakedObject);
            LastMessage = isUsable.Reason;

            bool canUse = isUsable.IsAllowed;
            Assert.IsFalse(canUse, "Field '" + Name + "' in " + owningObject.NakedObject + " is modifiable");
            return this;
        }

        public ITestProperty AssertIsNotEmpty() {
            AssertIsVisible();
            Assert.IsFalse(field.IsEmpty(owningObject.NakedObject), "Expected" + " '" + Name + "' to contain something but it was empty");
            return this;
        }

        public ITestProperty AssertIsEmpty() {
            AssertIsVisible();
            Assert.IsTrue(field.IsEmpty(owningObject.NakedObject), "Expected" + " '" + Name + "' to be empty");
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

        public ITestProperty AssertObjectIsEqual(ITestNaked expected) {
            AssertIsVisible();
            Assert.AreEqual(expected, Content);
            return this;
        }

        public ITestProperty AssertIsValidToSave() {
            if (field.IsMandatory && field.IsVisible(owningObject.NakedObject) && field.IsUsable(owningObject.NakedObject).IsAllowed) {
                Assert.IsFalse(field.IsEmpty(owningObject.NakedObject), "Cannot save object as mandatory field " + " '" + Name + "' is empty");
                Assert.IsTrue(field.GetNakedObject(owningObject.NakedObject).ValidToPersist() == null);
            }
            if (field.IsCollection) {
                field.GetNakedObject(owningObject.NakedObject).GetAsEnumerable(manager).ForEach(no => Assert.AreEqual(no.ValidToPersist(), null));
            }

            return this;
        }

        public ITestProperty AssertLastMessageIs(string message) {
            Assert.IsTrue(message.Equals(LastMessage), "Last message expected: '" + message + "' actual: '" + LastMessage + "'");
            return this;
        }

        public ITestProperty AssertLastMessageContains(string message) {
            Assert.IsTrue(LastMessage.Contains(message), "Last message expected to contain: '" + message + "' actual: '" + LastMessage + "'");
            return this;
        }

        public ITestProperty AssertParseableFieldEntryInvalid(string text) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            INakedObject nakedObject = owningObject.NakedObject;
            INakedObject existingValue = field.GetNakedObject(nakedObject);
            var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
            try {
                INakedObject newValue = parseableFacet.ParseTextEntry(text, manager);
                IConsent isAssociationValid = ((IOneToOneAssociation) field).IsAssociationValid(owningObject.NakedObject, newValue);
                LastMessage = isAssociationValid.Reason;
                Assert.IsFalse(isAssociationValid.IsAllowed, "Content was unexpectedly validated");
            }
            catch (InvalidEntryException) {
                // expected 
            }
            return this;
        }

        public ITestProperty AssertParseableFieldEntryIsValid(string text) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            INakedObject nakedObject = owningObject.NakedObject;
            INakedObject existingValue = field.GetNakedObject(nakedObject);
            var parseableFacet = field.Specification.GetFacet<IParseableFacet>();
            INakedObject newValue = parseableFacet.ParseTextEntry(text, manager);
            IConsent isAssociationValid = ((IOneToOneAssociation) field).IsAssociationValid(owningObject.NakedObject, newValue);
            LastMessage = isAssociationValid.Reason;
            Assert.IsTrue(isAssociationValid.IsAllowed, "Content was unexpectedly invalidated");
            return this;
        }

        private ITestProperty AssertNotParseable() {
            Assert.Fail("Not a parseable field");
            return this;
        }

        #endregion
    }
}