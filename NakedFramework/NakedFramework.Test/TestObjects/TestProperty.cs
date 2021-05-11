// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Reflect;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Test.Interface;

namespace NakedFramework.Test.TestObjects {
    public class TestProperty : ITestProperty {
        private readonly ITestObjectFactory factory;
        private readonly IAssociationSpec field;
        private readonly INakedObjectsFramework framework;
        private readonly INakedObjectManager manager;
        private readonly ITestHasActions owningObject;
        private readonly IObjectPersistor persistor;

        public TestProperty(IAssociationSpec field, ITestHasActions owningObject, ITestObjectFactory factory, INakedObjectsFramework framework) {
            persistor = framework.Persistor;
            this.field = field;
            this.owningObject = owningObject;
            this.factory = factory;
            this.framework = framework;
            manager = framework.NakedObjectManager;
        }

        private void ResetLastMessage() {
            LastMessage = string.Empty;
        }

        private bool IsNotParseable() => field.ReturnSpec.GetFacet<IParseableFacet>() == null;

        #region ITestProperty Members

        public string Name => field.Name;

        public string Id => field.Id;

        public string Title => field.PropertyTitle(field.GetNakedObject(owningObject.NakedObject), framework);

        public ITestNaked Content {
            get {
                var fieldValue = field.GetNakedObject(owningObject.NakedObject);
                if (fieldValue != null && fieldValue.ResolveState.IsResolvable()) {
                    persistor.ResolveImmediately(fieldValue);
                }

                return factory.CreateTestNaked(fieldValue);
            }
        }

        public ITestObject ContentAsObject => (ITestObject) Content;

        public ITestCollection ContentAsCollection => (ITestCollection) Content;

        public ITestNaked GetDefault() {
            var defaultValue = field.GetDefault(owningObject.NakedObject);
            return factory.CreateTestNaked(defaultValue);
        }

        public ITestNaked[] GetChoices() {
            var choices = ((IOneToOneAssociationSpec) field).GetChoices(owningObject.NakedObject, null);
            return choices.Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked[] GetCompletions(string autoCompleteParm) {
            var completions = ((IOneToOneAssociationSpec) field).GetCompletions(owningObject.NakedObject, autoCompleteParm);
            return completions.Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestProperty SetObject(ITestObject testObject) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsFalse(field.ReturnSpec.IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");

            var testNakedObjectAdapter = testObject.NakedObject;

            Assert.IsTrue(testNakedObjectAdapter.Spec.IsOfType(field.ReturnSpec), $"Can't drop a {testObject.NakedObject.Spec.ShortName} on to the {Name} field (which accepts {field.ReturnSpec})");

            var nakedObjectAdapter = owningObject.NakedObject;

            var valid = field switch {
                IOneToOneAssociationSpec associationSpec => associationSpec.IsAssociationValid(nakedObjectAdapter, testNakedObjectAdapter),
                IOneToManyAssociationSpec => new Veto("Always disabled"),
                _ => throw new UnknownTypeException(field)
            };

            LastMessage = valid.Reason;
            Assert.IsFalse(valid.IsVetoed, $"Cannot SetObject {testNakedObjectAdapter} in the field {field} within {nakedObjectAdapter}: {valid.Reason}");

            var spec = field as IOneToOneAssociationSpec;
            spec?.SetAssociation(nakedObjectAdapter, testNakedObjectAdapter);

            return this;
        }

        private string LastMessage { get; set; }

        /// <summary>
        ///     Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
        ///     option that each object field offers by default.
        /// </summary>
        public ITestProperty ClearObject() {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            Assert.IsFalse(field.ReturnSpec.IsParseable, "Clear(..) not allowed on value target field; use SetValue(..) instead");

            var nakedObjectAdapter = field.GetNakedObject(owningObject.NakedObject);
            if (nakedObjectAdapter != null) {
                if (field is IOneToOneAssociationSpec spec) {
                    spec.SetAssociation(owningObject.NakedObject, null);
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

            var nakedObjectAdapter = owningObject.NakedObject;
            try {
                field.GetNakedObject(nakedObjectAdapter);

                var parseableFacet = field.ReturnSpec.GetFacet<IParseableFacet>();

                var newValue = parseableFacet.ParseTextEntry(textEntry, manager);

                var consent = ((IOneToOneAssociationSpec) field).IsAssociationValid(nakedObjectAdapter, newValue);
                LastMessage = consent.Reason;

                Assert.IsFalse(consent.IsVetoed, $"Content: '{textEntry}' is not valid. Reason: {consent.Reason}");

                ((IOneToOneAssociationSpec) field).SetAssociation(nakedObjectAdapter, textEntry.Trim().Equals("") ? null : newValue);
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

            var nakedObjectAdapter = owningObject.NakedObject;
            try {
                var consent = ((IOneToOneAssociationSpec) field).IsAssociationValid(nakedObjectAdapter, null);
                LastMessage = consent.Reason;
                Assert.IsFalse(consent.IsVetoed, $"Content: 'null' is not valid. Reason: {consent.Reason}");
                ((IOneToOneAssociationSpec) field).SetAssociation(nakedObjectAdapter, null);
            }
            catch (InvalidEntryException) {
                Assert.Fail("Null Entry not recognised ");
            }

            return this;
        }

        #endregion

        #region Asserts

        public ITestProperty AssertFieldEntryInvalid(string text) => IsNotParseable() ? AssertNotParseable() : AssertParseableFieldEntryInvalid(text);

        public ITestProperty AssertFieldEntryIsValid(string text) => IsNotParseable() ? AssertNotParseable() : AssertParseableFieldEntryIsValid(text);

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

            Assert.IsFalse(field.ReturnSpec.IsParseable, "Drop(..) not allowed on value target field; use SetValue(..) instead");
            var testNakedObjectAdapter = testObject.NakedObject;
            Assert.IsTrue(testNakedObjectAdapter.Spec.IsOfType(field.ReturnSpec), $"Can't drop a {testObject.NakedObject.Spec.ShortName} on to the {Name} field (which accepts {field.ReturnSpec})");
            var nakedObjectAdapter = owningObject.NakedObject;
            var valid = field switch {
                IOneToOneAssociationSpec spec => spec.IsAssociationValid(nakedObjectAdapter, testNakedObjectAdapter),
                IOneToManyAssociationSpec => new Veto("Always disabled"),
                _ => throw new UnknownTypeException(field)
            };

            LastMessage = valid.Reason;

            Assert.IsFalse(valid.IsVetoed, $"Cannot SetObject {testNakedObjectAdapter} in the field {field} within {nakedObjectAdapter}: {valid.Reason}");
            return this;
        }

        public ITestProperty AssertIsInvisible() {
            var canAccess = field.IsVisible(owningObject.NakedObject);
            Assert.IsFalse(canAccess, "Field '" + Name + "' is visible");
            return this;
        }

        public ITestProperty AssertIsVisible() {
            var canAccess = field.IsVisible(owningObject.NakedObject);
            Assert.IsTrue(canAccess, "Field '" + Name + "' is invisible");
            return this;
        }

        public ITestProperty AssertIsMandatory() {
            Assert.IsTrue(field.IsMandatory, "Field '" + field.Id + "' is optional");
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

            var isUsable = field.IsUsable(owningObject.NakedObject);
            LastMessage = isUsable.Reason;

            var canUse = isUsable.IsAllowed;
            Assert.IsTrue(canUse, "Field '" + Name + "' in " + owningObject.NakedObject + " is unmodifiable");
            return this;
        }

        public ITestProperty AssertIsUnmodifiable() {
            ResetLastMessage();
            var isUsable = field.IsUsable(owningObject.NakedObject);
            LastMessage = isUsable.Reason;

            var canUse = isUsable.IsAllowed;
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

            if (field is IOneToManyAssociationSpec) {
                field.GetNakedObject(owningObject.NakedObject).GetAsEnumerable(manager).ForEach(no => Assert.AreEqual(no.ValidToPersist(), null));
            }

            return this;
        }

        public ITestProperty AssertLastMessageIs(string message) {
            Assert.IsTrue(message.Equals(LastMessage), "Last message expected: '" + message + "' actual: '" + LastMessage + "'");
            return this;
        }

        private ITestProperty AssertParseableFieldEntryInvalid(string text) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            var nakedObjectAdapter = owningObject.NakedObject;
            field.GetNakedObject(nakedObjectAdapter);
            var parseableFacet = field.ReturnSpec.GetFacet<IParseableFacet>();
            try {
                var newValue = parseableFacet.ParseTextEntry(text, manager);
                var isAssociationValid = ((IOneToOneAssociationSpec) field).IsAssociationValid(owningObject.NakedObject, newValue);
                LastMessage = isAssociationValid.Reason;
                Assert.IsFalse(isAssociationValid.IsAllowed, "Content was unexpectedly validated");
            }
            catch (InvalidEntryException) {
                // expected 
            }

            return this;
        }

        private ITestProperty AssertParseableFieldEntryIsValid(string text) {
            AssertIsVisible();
            AssertIsModifiable();
            ResetLastMessage();

            var nakedObjectAdapter = owningObject.NakedObject;
            field.GetNakedObject(nakedObjectAdapter);
            var parseableFacet = field.ReturnSpec.GetFacet<IParseableFacet>();
            var newValue = parseableFacet.ParseTextEntry(text, manager);
            var isAssociationValid = ((IOneToOneAssociationSpec) field).IsAssociationValid(owningObject.NakedObject, newValue);
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