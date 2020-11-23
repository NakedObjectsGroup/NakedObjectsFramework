// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedObjects.SystemTest.Method {
    [TestFixture]
    public class TestRecognisedMethods : AbstractSystemTest<MethodsDbContext> {

        protected override Type[] ObjectTypes =>
            new[] {
                typeof(Sex),
                typeof(Auto1),
                typeof(Auto2),
                typeof(Auto3),
                typeof(Choices1),
                typeof(Choices2),
                typeof(Choices3),
                typeof(Choices4),
                typeof(Clear1),
                typeof(Clear2),
                typeof(Clear3),
                typeof(Created1),
                typeof(Created2),
                typeof(Default1),
                typeof(Default2),
                typeof(Default3),
                typeof(Default4),
                typeof(Deleted1),
                typeof(Deleted2),
                typeof(Deleting1),
                typeof(Deleting2),
                typeof(Disable1),
                typeof(Disable2),
                typeof(Disable3),
                typeof(Hide1),
                typeof(Hide2),
                typeof(Hide3),
                typeof(Modify1),
                typeof(Modify2),
                typeof(Modify3),
                typeof(Modify4),
                typeof(Persisted1),
                typeof(Persisted2),
                typeof(Persisted3),
                typeof(Persisting1),
                typeof(Persisting2),
                typeof(Title1),
                typeof(Title2),
                typeof(Title3),
                typeof(Title4),
                typeof(Title5),
                typeof(Title6),
                typeof(Title7),
                typeof(Title8),
                typeof(Title9),
                typeof(Title10),
                typeof(Title11),
                typeof(Updated1),
                typeof(Updated2),
                typeof(Updating1),
                typeof(Updating2),
                typeof(Validate1),
                typeof(Validate2),
                typeof(Validate3),
                typeof(Validate4),
                typeof(Validate5)
            };

        protected override Type[] Services =>
            new[] {
                typeof(SimpleRepository<Auto1>),
                typeof(SimpleRepository<Auto2>),
                typeof(SimpleRepository<Auto3>),
                typeof(SimpleRepository<Choices1>),
                typeof(SimpleRepository<Choices2>),
                typeof(SimpleRepository<Choices3>),
                typeof(SimpleRepository<Choices4>),
                typeof(SimpleRepository<Clear1>),
                typeof(SimpleRepository<Clear2>),
                typeof(SimpleRepository<Clear3>),
                typeof(SimpleRepository<Created1>),
                typeof(SimpleRepository<Created2>),
                typeof(SimpleRepository<Default1>),
                typeof(SimpleRepository<Default2>),
                typeof(SimpleRepository<Default3>),
                typeof(SimpleRepository<Default4>),
                typeof(SimpleRepository<Deleted1>),
                typeof(SimpleRepository<Deleted2>),
                typeof(SimpleRepository<Deleting1>),
                typeof(SimpleRepository<Deleting2>),
                typeof(SimpleRepository<Disable1>),
                typeof(SimpleRepository<Disable2>),
                typeof(SimpleRepository<Disable3>),
                typeof(SimpleRepository<Hide1>),
                typeof(SimpleRepository<Hide2>),
                typeof(SimpleRepository<Hide3>),
                typeof(SimpleRepository<Modify1>),
                typeof(SimpleRepository<Modify2>),
                typeof(SimpleRepository<Modify3>),
                typeof(SimpleRepository<Modify4>),
                typeof(SimpleRepository<Persisted1>),
                typeof(SimpleRepository<Persisted2>),
                typeof(SimpleRepository<Persisted3>),
                typeof(SimpleRepository<Persisting1>),
                typeof(SimpleRepository<Persisting2>),
                typeof(SimpleRepository<Title1>),
                typeof(SimpleRepository<Title2>),
                typeof(SimpleRepository<Title3>),
                typeof(SimpleRepository<Title4>),
                typeof(SimpleRepository<Title5>),
                typeof(SimpleRepository<Title6>),
                typeof(SimpleRepository<Title7>),
                typeof(SimpleRepository<Title8>),
                typeof(SimpleRepository<Title9>),
                typeof(SimpleRepository<Title10>),
                typeof(SimpleRepository<Title11>),
                typeof(SimpleRepository<Updated1>),
                typeof(SimpleRepository<Updated2>),
                typeof(SimpleRepository<Updating1>),
                typeof(SimpleRepository<Updating2>),
                typeof(SimpleRepository<Validate1>),
                typeof(SimpleRepository<Validate2>),
                typeof(SimpleRepository<Validate3>),
                typeof(SimpleRepository<Validate4>),
                typeof(SimpleRepository<Validate5>)
            };

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            MethodsDbContext.Delete();
            var context = Activator.CreateInstance<MethodsDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            MethodsDbContext.Delete();
        }

        private void CreateAuto2(string prop1) {
            var obj2 = NewTestObject<Auto2>();
            obj2.GetPropertyByName("Prop1").SetValue(prop1);
            obj2.Save();
        }

        private void CreateChoices<T>(string prop1) {
            var obj2 = NewTestObject<T>();
            obj2.GetPropertyByName("Prop1").SetValue(prop1);
            obj2.Save();
        }

        [Test]
        public virtual void AutoCompleteParameters() {
            CreateAuto2("Bar1");
            CreateAuto2("Bar2");
            CreateAuto2("Bar3");

            var obj1 = NewTestObject<Auto1>();
            var action = obj1.GetAction("Do Something");
            var cho0 = action.Parameters[0].GetCompletions("any");
            Assert.AreEqual(1, cho0.Length);
            var cho1 = action.Parameters[1].GetCompletions("any");
            Assert.AreEqual(3, cho1.Length);

            var cho2 = action.Parameters[2].GetCompletions("bar");
            Assert.AreEqual(2, cho2.Length);
        }

        [Test]
        public virtual void AutoCompleteReferenceProperty() {
            CreateAuto2("Foo1");
            CreateAuto2("Foo2");
            CreateAuto2("Foo3");

            var obj1 = NewTestObject<Auto1>();
            var prop = obj1.GetPropertyByName("Prop3");
            var cho = prop.GetCompletions("foo");
            Assert.AreEqual(3, cho.Length);
            Assert.AreEqual("Foo1", cho[0].Title);
        }

        [Test]
        public virtual void AutoCompleteStringProperty() {
            var obj1 = NewTestObject<Auto1>();
            var prop = obj1.GetPropertyByName("Prop2");
            var cho = prop.GetCompletions("any");
            Assert.AreEqual(3, cho.Length);
            Assert.AreEqual("Fee", cho[0].Title);
        }

        [Test]
        public void CalledWhenReferencePropertyCleared() {
            var obj3 = NewTestObject<Modify3>();
            obj3.GetPropertyByName("Prop1").SetValue("Foo");
            obj3.Save();

            var obj1 = NewTestObject<Modify1>();
            var prop3 = obj1.GetPropertyByName("Prop3");
            var prop4 = obj1.GetPropertyByName("Prop4");

            prop3.AssertIsEmpty();
            prop4.AssertIsEmpty();

            prop4.SetObject(obj3);
            prop4.AssertObjectIsEqual(obj3);
            prop3.SetValue("Neutral");

            prop4.ClearObject();
            prop4.AssertIsEmpty();
            prop3.AssertValueIsEqual("Prop4 has been modified");
        }

        [Test]
        public void CalledWhenValuePropertyIsCleared() {
            var obj = NewTestObject<Modify1>();
            var prop0 = obj.GetPropertyByName("Prop0");
            var prop1 = obj.GetPropertyByName("Prop1");

            prop1.SetValue("Foo");
            prop0.SetValue("Foo");

            prop1.ClearValue();
            prop1.AssertIsEmpty();
            prop0.AssertValueIsEqual("Prop1 has been modified");
        }

        [Test]
        public void ChoicesMethodDoesNotShowUpAsAction() {
            var obj1 = NewTestObject<Choices1>();
            try {
                obj1.GetAction("Choices Prop1");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Choices Prop1'", e.Message);
            }

            try {
                obj1.GetAction("Choices 0 Do Something");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Choices 0 Do Something'", e.Message);
            }
        }

        [Test]
        public virtual void ChoicesNumericProperty() {
            var obj1 = NewTestObject<Choices1>();
            var prop = obj1.GetPropertyByName("Prop1");
            var cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Length);
            Assert.AreEqual("4", cho[0].Title);
        }

        [Test]
        public virtual void ChoicesParameters() {
            CreateChoices<Choices2>("Bar1");
            CreateChoices<Choices2>("Bar2");
            CreateChoices<Choices2>("Bar3");

            var obj1 = NewTestObject<Choices1>();
            var action = obj1.GetAction("Do Something");
            var cho0 = action.Parameters[0].GetChoices();
            Assert.AreEqual(3, cho0.Length);
            Assert.AreEqual("4", cho0[0].Title);

            var cho1 = action.Parameters[1].GetChoices();
            Assert.AreEqual(3, cho1.Length);
            Assert.AreEqual("Fee", cho1[0].Title);

            var cho2 = action.Parameters[2].GetChoices();
            Assert.AreEqual(3, cho2.Length);
            Assert.AreEqual("Bar1", cho2[0].Title);
        }

        [Test]
        public virtual void ChoicesReferenceProperty() {
            CreateChoices<Choices4>("Bar1");
            CreateChoices<Choices4>("Bar2");
            CreateChoices<Choices4>("Bar3");

            var obj1 = NewTestObject<Choices1>();
            var prop = obj1.GetPropertyByName("Prop3");
            var cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Length);
            Assert.AreEqual("Bar1", cho[0].Title);
        }

        [Test]
        public virtual void ChoicesStringProperty() {
            var obj1 = NewTestObject<Choices1>();
            var prop = obj1.GetPropertyByName("Prop2");
            var cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Length);
            Assert.AreEqual("Fee", cho[0].Title);
        }

        // Note Clear Prefix has been removed as a recognized prefix for complementary actions 
        [Test]
        public void ClearMethodDoesShowUpAsAnAction() {
            var obj1 = NewTestObject<Clear1>();
            var action = obj1.GetAction("Clear Prop1");
            action.AssertHasFriendlyName("Clear Prop1");
        }

        [Test]
        public void CreatedCalled() {
            var obj1 = NewTestObject<Created1>();
            var dom1 = (Created1) obj1.GetDomainObject();
            Assert.IsTrue(dom1.CreatedCalled);
        }

        [Test]
        public void CreatedDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Created1>();
            try {
                obj1.GetAction("Created");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Created'", e.Message);
            }
        }

        [Test]
        public void DefaultMethodDoesNotShowUpAsAction() {
            var obj = NewTestObject<Default1>();
            try {
                obj.GetAction("Default Prop1");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Default Prop1'", e.Message);
            }

            try {
                obj.GetAction("Default 0 Do Something");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Default 0 Do Something'", e.Message);
            }
        }

        [Test]
        public void DefaultNumericMethodOverAnnotation() {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop4");
            var def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [Test]
        public virtual void DefaultNumericProperty() {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop1");
            var def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [Test]
        public void DefaultParameters() {
            //Set up choices
            var obj2 = NewTestObject<Default4>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar1");
            obj2.Save();
            obj2 = NewTestObject<Default4>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar2");
            obj2.Save();
            obj2 = NewTestObject<Default4>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar3");
            obj2.Save();

            var obj1 = NewTestObject<Default1>();
            var action = obj1.GetAction("Do Something");
            var def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            var def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);

            var def2 = action.Parameters[2].GetDefault().Title;
            Assert.IsNotNull(def2);
            Assert.AreEqual("Bar2", def2);
        }

        [Test]
        public void DefaultParametersOverAnnotation() {
            var obj1 = NewTestObject<Default1>();
            var action = obj1.GetAction("Do Something Else");
            var def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            var def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);
        }

        [Test]
        public virtual void DefaultReferenceProperty() {
            //Set up choices
            var obj2 = NewTestObject<Default2>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar1");
            obj2.Save();
            obj2 = NewTestObject<Default2>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar2");
            obj2.Save();
            obj2 = NewTestObject<Default2>();
            obj2.GetPropertyByName("Prop1").SetValue("Bar3");
            obj2.Save();

            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop3");
            var def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Bar2", def);
        }

        [Test]
        public void DefaultStringMethodOverAnnotation() {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop5");
            var def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }

        [Test]
        public virtual void DefaultStringProperty() {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop2");
            var def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }

        [Test]
        public void DeletedCalled() {
            var obj1 = NewTestObject<Deleted1>();
            var dom1 = (Deleted1) obj1.GetDomainObject();
            obj1.Save();

            Assert.IsFalse(Deleted1.DeletedCalled);
            obj1.GetAction("Delete").Invoke();
            Assert.IsTrue(Deleted1.DeletedCalled);
            Deleted1.DeletedCalled = false;
        }

        [Test]
        public void DeletedDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Deleted1>();
            try {
                obj1.GetAction("Deleted");
                Assert.Fail("Should not get here");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleted'", e.Message);
            }
        }

        [Test]
        public void DeletingCalled() {
            var obj1 = NewTestObject<Deleting1>();
            var dom1 = (Deleting1) obj1.GetDomainObject();
            obj1.Save();

            Assert.IsFalse(Deleting1.DeletingCalled);

            obj1.GetAction("Delete").InvokeReturnObject();

            Assert.IsTrue(Deleting1.DeletingCalled);
        }

        [Test]
        public void DeletingDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Deleting1>();
            try {
                obj1.GetAction("Deleting");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleting'", e.Message);
            }
        }

        [Test]
        public void DisableAction() {
            var obj = NewTestObject<Disable3>();
            obj.GetPropertyByName("Prop4").SetValue("avalue");
            obj.GetPropertyByName("Prop6").SetValue("avalue");
            obj.Save();
            obj.GetAction("Action1").AssertIsEnabled();

            obj.GetPropertyByName("Prop4").SetValue("Disable 6");
            obj.GetAction("Action4").AssertIsDisabled();
        }

        [Test]
        public void DisableActionDefault() {
            var obj = NewTestObject<Disable2>();
            obj.GetAction("Action1").AssertIsDisabled();
            obj.GetAction("Action2").AssertIsDisabled();
        }

        [Test]
        public void DisableActionDefaultDoesNotDisableProperties() {
            var obj = NewTestObject<Disable2>();
            obj.GetPropertyByName("Prop1").AssertIsModifiable();
            obj.GetPropertyByName("Prop2").AssertIsModifiable();
            //obj.GetPropertyByName("Prop4").AssertIsModifiable(); - collection disabled by default
        }

        [Test]
        public void DisableActionDefaultDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Disable2>();
            try {
                obj.GetAction("Disable Action Default");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Action Default'", e.Message);
            }
        }

        [Test]
        public void DisableActionDefaultOverriddenByActionLevelMethod() {
            var obj = NewTestObject<Disable2>();
            obj.GetAction("Action3").AssertIsEnabled();
        }

        [Test]
        public void DisableMethodDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Disable3>();
            try {
                obj.GetAction("Disable Prop6");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Prop6'", e.Message);
            }
        }

        [Test] //Pending #9228
        public void DisableMethodsWithParamsNotRecognised() {
            var obj = NewTestObject<Disable3>();
            obj.GetAction("Disable Action2");
            obj.GetAction("Disable Action3");
            obj.GetAction("Disable Prop7");
            obj.GetAction("Disable Prop8");
        }

        [Test]
        public void DisableProperty() {
            var obj = NewTestObject<Disable3>();
            var prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsModifiable();
            obj.Save();
            prop6.AssertIsModifiable();

            var prop4 = obj.GetPropertyByName("Prop4");
            prop4.SetValue("Disable 6");
            prop6.AssertIsUnmodifiable();
        }

        [Test]
        public void DisablePropertyDefault() {
            var obj = NewTestObject<Disable1>();
            obj.GetPropertyByName("Prop1").AssertIsUnmodifiable();
            obj.GetPropertyByName("Prop2").AssertIsUnmodifiable();
            obj.GetPropertyByName("Prop4").AssertIsUnmodifiable();
        }

        [Test]
        public void DisablePropertyDefaultDoesNotDisableActions() {
            var obj = NewTestObject<Disable1>();
            obj.GetAction("Action1").AssertIsEnabled();
            obj.GetAction("Action2").AssertIsEnabled();
        }

        [Test]
        public void DisablePropertyDefaultDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Disable1>();
            try {
                obj.GetAction("Disable Property Default");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Property Default'", e.Message);
            }
        }

        [Test]
        public void DisablePropertyPropertyOverriddenByPropertyLevelMethod() {
            var obj = NewTestObject<Disable1>();
            obj.GetPropertyByName("Prop3").AssertIsModifiable();
            //obj.GetPropertyByName("Prop5").AssertIsModifiable(); - collection disabled by default
        }

        [Test]
        public void HideAction() {
            var obj = NewTestObject<Hide3>();
            obj.Save();
            obj.GetAction("Do Something").AssertIsVisible();
            obj.GetPropertyByName("Prop4").SetValue("Hide 6");
            obj.GetAction("Do Something").AssertIsInvisible();
        }

        [Test]
        public void HideActionDefault() {
            var obj = NewTestObject<Hide2>();
            obj.GetAction("Action1").AssertIsInvisible();
            obj.GetAction("Action2").AssertIsInvisible();
        }

        [Test]
        public void HideActionDefaultDoesNotHideProperties() {
            var obj = NewTestObject<Hide2>();
            obj.GetPropertyByName("Prop1").AssertIsVisible();
            obj.GetPropertyByName("Prop2").AssertIsVisible();
            obj.GetPropertyByName("Prop4").AssertIsVisible();
        }

        [Test]
        public void HideActionDefaultDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Hide2>();
            try {
                obj.GetAction("Hide Action Default");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Action Default'", e.Message);
            }
        }

        [Test]
        public void HideActionDefaultOverriddenByActionLevelMethod() {
            var obj = NewTestObject<Hide2>();
            obj.GetAction("Action3").AssertIsVisible();
        }

        [Test]
        public void HideMethodDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Hide3>();
            try {
                obj.GetAction("Hide Prop6");
                Assert.Fail("'Hide Prop6' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Prop6'", e.Message);
            }
        }

        [Test] // pending #9228
        public void HideMethodsWithParamsNotRecognised() {
            var obj = NewTestObject<Hide3>();
            obj.GetAction("Hide Prop8");
            obj.GetAction("Hide Prop9");
            obj.GetAction("Hide Action1");
            obj.GetAction("Hide Action2");
        }

        [Test]
        public void HideProperty() {
            var obj = NewTestObject<Hide3>();
            var prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsVisible();
            obj.Save();
            prop6.AssertIsVisible();

            var prop4 = obj.GetPropertyByName("Prop4");
            prop4.SetValue("Hide 6");
            prop6.AssertIsInvisible();
        }

        [Test]
        public void HidePropertyDefault() {
            var obj = NewTestObject<Hide1>();
            obj.GetPropertyByName("Prop1").AssertIsInvisible();
            obj.GetPropertyByName("Prop2").AssertIsInvisible();
            obj.GetPropertyByName("Prop4").AssertIsInvisible();
        }

        [Test]
        public void HidePropertyDefaultDoesNotHideActions() {
            var obj = NewTestObject<Hide1>();
            obj.GetAction("Action1").AssertIsVisible();
            obj.GetAction("Action2").AssertIsVisible();
        }

        [Test]
        public void HidePropertyDefaultDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Hide1>();
            try {
                obj.GetAction("Hide Property Default");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Property Default'", e.Message);
            }
        }

        [Test]
        public void HidePropertyPropertyOverriddenByPropertyLevelMethod() {
            var obj = NewTestObject<Hide1>();
            obj.GetPropertyByName("Prop3").AssertIsVisible();
            obj.GetPropertyByName("Prop5").AssertIsVisible();
        }

        [Test]
        public virtual void ITitleBuilderTestAllAppendsAndConcats() {
            var culture = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var obj = NewTestObject<Title10>();
                obj.AssertTitleEquals("x& y y t1 t2 t3$ t1ct1*t1: no dateno date 04/02/2007 Female Not Specified");
            }
            finally {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [Test]
        public void LowerCaseCreatedNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Created2>();
            var dom1 = (Created2) obj1.GetDomainObject();
            Assert.IsFalse(dom1.CreatedCalled);
            obj1.GetAction("Created");
        }

        [Test]
        public void LowerCaseDeletedNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Deleted2>();
            var dom1 = (Deleted2) obj1.GetDomainObject();
            Assert.IsFalse(Deleted2.DeletedCalled);

            try {
                obj1.GetAction("Deleted");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleted'", e.Message);
            }
        }

        [Test]
        public void LowerCaseDeletingNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Deleting2>().Save();
            var dom1 = (Deleting2) obj1.GetDomainObject();

            //Check method is visible as an action
            obj1.GetAction("Deleting").AssertIsVisible();

            Assert.IsFalse(Deleting2.DeletingCalled);
            obj1.GetAction("Delete").InvokeReturnObject();
            Assert.IsFalse(Deleting2.DeletingCalled); //Still false
        }

        [Test]
        public void LowerCaseNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Updated2>();
            var dom1 = (Updated2) obj1.GetDomainObject();
            Assert.IsFalse(Updated2.UpdatedCalled);
            obj1.GetAction("Updated");
        }

        [Test]
        public void LowerCasePersistedNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Persisted2>();
            var dom1 = (Persisted2) obj1.GetDomainObject();
            Assert.IsFalse(dom1.PersistedCalled);
            obj1.GetAction("Persisted");
        }

        [Test]
        public void LowerCaseUpdatingNotRecognisedAndShowsAsAction() {
            var obj1 = NewTestObject<Updating2>();
            obj1.GetAction("Updating");
        }

        [Test]
        public void ModifyMethodDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Modify1>();
            try {
                obj1.GetAction("Modify Prop1");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Modify Prop1'", e.Message);
            }
        }

        [Test]
        public void ModifyMethodOnReferenceProperty() {
            var obj3 = NewTestObject<Modify3>();
            obj3.GetPropertyByName("Prop1").SetValue("Foo");
            obj3.Save();

            var obj1 = NewTestObject<Modify1>();
            var prop3 = obj1.GetPropertyByName("Prop3");
            var prop4 = obj1.GetPropertyByName("Prop4");

            prop3.AssertIsEmpty();
            prop4.AssertIsEmpty();

            prop4.SetObject(obj3);
            prop4.AssertObjectIsEqual(obj3);
            prop3.AssertValueIsEqual("Prop4 has been modified");
        }

        [Test]
        public void ModifyMethodOnValueProperty() {
            var obj = NewTestObject<Modify1>();
            var prop0 = obj.GetPropertyByName("Prop0");
            var prop1 = obj.GetPropertyByName("Prop1");

            prop0.AssertIsEmpty();
            prop1.AssertIsEmpty();

            prop1.SetValue("Foo");
            prop1.AssertValueIsEqual("Foo");
            prop0.AssertValueIsEqual("Prop1 has been modified");
        }

        [Test]
        public virtual void ObjectWithSimpleToString() {
            var obj = NewTestObject<Title1>();
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Bar");
            obj.AssertTitleEquals("Bar");
            obj.Save();
            obj.AssertTitleEquals("Bar");
        }

        [Test]
        public void PersistedCalled() {
            var obj1 = NewTestObject<Persisted1>();
            var dom1 = (Persisted1) obj1.GetDomainObject();
            try {
                obj1.Save();
                Assert.Fail("Shouldn't get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("Persisted called", e.Message);
            }
        }

        [Test]
        public void PersistedDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Persisted1>();
            try {
                obj1.GetAction("Persisted");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisted'", e.Message);
            }
        }

        [Test]
        public void PersistedMarkedAsIgnoredIsNotCalledAndIsNotAnAction() {
            var obj = NewTestObject<Persisted3>();
            obj.Save(); //Would throw an exception if the Persisted had been called.
            try {
                obj.GetAction("Persisted");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisted'", e.Message);
            }
        }

        [Test]
        public void PersistingCalled() {
            var obj1 = NewTestObject<Persisting1>();
            var dom1 = (Persisting1) obj1.GetDomainObject();
            Assert.IsFalse(Persisting1.PersistingCalled);

            obj1.Save();

            Assert.IsTrue(Persisting1.PersistingCalled);
        }

        [Test]
        public void PersistingDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Persisting1>();
            try {
                obj1.GetAction("Persisting");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisting'", e.Message);
            }
        }

        [Test]
        public void RecognisedAutoCompleteMethodDoesNotShowUpAsAction() {
            var obj1 = NewTestObject<Auto1>();
            try {
                var act = obj1.GetAction("Auto Complete Prop2");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete Prop2'", e.Message);
            }

            try {
                var act = obj1.GetAction("Auto Complete Prop3");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete Prop3'", e.Message);
            }

            try {
                obj1.GetAction("Auto Complete0 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete0 Do Something'", e.Message);
            }

            try {
                obj1.GetAction("Auto Complete1 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete1 Do Something'", e.Message);
            }

            try {
                obj1.GetAction("Auto Complete2 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete2 Do Something'", e.Message);
            }
        }

        [Test]
        public virtual void TitleMethod() {
            var obj = NewTestObject<Title3>();
            obj.AssertTitleEquals("Untitled Title3");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        [Test]
        public virtual void TitleMethodMarkedIgnoredIsNotCalled() {
            var obj = NewTestObject<Title11>();
            obj.AssertTitleEquals("Untitled Title 11");
        }

        [Test]
        public virtual void TitleMethodTakesPrecedenceOverToString() {
            var obj = NewTestObject<Title4>();
            Equals("Bar", obj.GetDomainObject().ToString());
            obj.AssertTitleEquals("Untitled Title4");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        [Test]
        public virtual void ToStringRecognisedAsATitle() {
            var obj = NewTestObject<Title5>();
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Bar");
            obj.AssertTitleEquals("Bar");
            obj.Save();
            obj.AssertTitleEquals("Bar");
        }

        [Test]
        public void UnmatchedAutoCompleteMethodShowsUpAsAction() {
            var obj3 = NewTestObject<Auto3>();
            obj3.GetAction("Auto Complete Prop1");
            obj3.GetAction("Auto Complete Prop2");
            obj3.GetAction("Auto Complete Prop3");
            obj3.GetAction("Auto Complete 0 Do Something");
            obj3.GetAction("Auto Complete 1 Do Something");
            obj3.GetAction("Auto Complete 2 Do Somting");
            obj3.GetAction("Auto Complete 3 Do Something");
        }

        [Test]
        public void UnmatchedChoicesMethodShowsUpAsAction() {
            var obj3 = NewTestObject<Choices3>();
            obj3.GetAction("Choices Prop1");
            obj3.GetAction("Choices Prop2");
            obj3.GetAction("Choices Prop3");
            obj3.GetAction("Choices 0 Do Somthing");
            obj3.GetAction("Choices 0 Do Something");
        }

        [Test]
        public void UnmatchedClearMethodShowsUpAsAnAction() {
            var obj2 = NewTestObject<Clear2>();
            obj2.GetAction("Clear Prop2");
            obj2.GetAction("Clear Prop3");
            obj2.GetAction("Clear Prop4");
        }

        [Test]
        public void UnmatchedDefaultMethodShowsUpAsAction() {
            var obj = NewTestObject<Default3>();
            obj.GetAction("Default Prop1");
            obj.GetAction("Default Prop2");
            obj.GetAction("Default 0 Do Somthing");
            obj.GetAction("Default 0 Do Something");
        }

        [Test]
        public void UnmatchedDisableMethodShowsUpAsAction() {
            var obj = NewTestObject<Disable3>();
            obj.GetAction("Disable Prop1");
            obj.GetAction("Disable Prop4");
        }

        [Test]
        public void UnmatchedHideMethodShowsUpAsAnAction() {
            var obj = NewTestObject<Hide3>();
            obj.GetAction("Hide Prop7");
            obj.GetAction("Hide Prop4");
            obj.GetAction("Hide Do Something Else");
            obj.GetAction("Hide Do Somthing Else");
        }

        [Test]
        public void UnmatchedModifyMethodShowsUpAsAnAction() {
            var obj2 = NewTestObject<Modify2>();
            obj2.GetAction("Modify Prop2");
            obj2.GetAction("Modify Prop3");
            obj2.GetAction("Modify Prop4");
        }

        [Test]
        public void UnmatchedValidateMethodShowsUpAsAnAction() {
            var obj = NewTestObject<Validate3>();
            obj.GetAction("Validate Prop1");
            obj.GetAction("Validate Prop2");
            obj.GetAction("Validate Prop3");
            obj.GetAction("Validate Do Something");
            obj.GetAction("Validate 0 Do Something");
            obj.GetAction("Validate 1 Do Something");
        }

        [Test]
        public void UpdatedCalled() {
            var obj1 = NewTestObject<Updated1>();
            var dom1 = (Updated1) obj1.GetDomainObject();
            obj1.Save();
            try {
                NakedObjectsFramework.TransactionManager.StartTransaction();
                obj1.GetPropertyByName("Prop1").SetValue("Foo");
                NakedObjectsFramework.TransactionManager.EndTransaction();

                Assert.Fail("Shouldn't get to here");
            }
            catch (Exception) { }
        }

        [Test]
        public void UpdatedDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Updated1>();
            try {
                obj1.GetAction("Updated");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Updated'", e.Message);
            }
        }

        [Test]
        public void UpdatingCalled() {
            var obj1 = NewTestObject<Updating1>();
            var dom1 = (Updating1) obj1.GetDomainObject();
            obj1.Save();
            try {
                NakedObjectsFramework.TransactionManager.StartTransaction();

                obj1.GetPropertyByName("Prop1").SetValue("Foo");
                NakedObjectsFramework.TransactionManager.EndTransaction();

                Assert.Fail("Should not get to here");
            }
            catch (Exception) { }
        }

        [Test]
        public void UpdatingDoesNotShowUpAsAnAction() {
            var obj1 = NewTestObject<Updating1>();
            try {
                obj1.GetAction("Updating");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Updating'", e.Message);
            }
        }

        [Test]
        public virtual void UsingITitleBuilderObjectConstructor() {
            var obj = NewTestObject<Title8>();
            obj.AssertTitleEquals("TB8");
        }

        [Test]
        public virtual void UsingITitleBuilderObjectConstructorWithNullAndDefault() {
            var obj = NewTestObject<Title9>();
            obj.AssertTitleEquals("TB9");
        }

        [Test]
        public virtual void UsingITitleBuilderStringConstructor() {
            var obj = NewTestObject<Title7>();
            obj.AssertTitleEquals("TB7");
        }

        [Test]
        public virtual void UsingITitleBuilderZeroParamConstructor() {
            var obj = NewTestObject<Title6>();
            obj.AssertTitleEquals("TB6");
        }

        //Test added because > 6 params relies on reflection rather than a delegate
        [Test]
        public virtual void ValidateActionWithMoreThanSixParams() {
            var obj = NewTestObject<Validate1>();
            var action = obj.GetAction("Do Something With Many Params");
            Assert.AreEqual(7, action.Parameters.Length);

            action.AssertIsInvalidWithParms("y", "x", "x", "x", "x", "x", "x");
            action.AssertIsValidWithParms("x", "x", "x", "x", "x", "x", "x");
            action.Invoke("x", "x", "x", "x", "x", "x", "x");
        }

        [Test]
        public virtual void ValidateCrossValidationFail4() {
            var obj = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value2");

            try {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. Expect prop1 == prop2", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationFail5A() {
            var obj = NewTestObject<Validate5>();
            var obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value2");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            try {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationFail5B() {
            var obj = NewTestObject<Validate5>();
            var obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("0");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            try {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationFail5C() {
            var obj = NewTestObject<Validate5>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").ClearObject();

            try {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationSuccess4() {
            var obj = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.Save();
        }

        [Test]
        public virtual void ValidateCrossValidationSuccess5() {
            var obj = NewTestObject<Validate5>();
            var obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            obj.Save();
        }

        [Test]
        public void ValidateMethodDoesNotShowUpAsAnAction() {
            var obj = NewTestObject<Validate1>();
            try {
                obj.GetAction("Validate Prop1");
                Assert.Fail("'Validate Prop1' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Prop1'", e.Message);
            }

            try {
                obj.GetAction("Validate0 Do Something");
                Assert.Fail("'Validate0 Do Something' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate0 Do Something'", e.Message);
            }

            try {
                obj.GetAction("Validate Do Something Else");
                Assert.Fail("'Validate Do Something Else' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Do Something Else'", e.Message);
            }
        }

        [Test]
        public virtual void ValidateNumericalProperty() {
            var obj = NewTestObject<Validate1>();
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertFieldEntryInvalid("2").AssertLastMessageIs("Value must be between 3 & 10");
            prop1.AssertFieldEntryInvalid("11").AssertLastMessageIs("Value must be between 3 & 10");
            prop1.SetValue("6").AssertLastMessageIs("");
            try {
                prop1.SetValue("11");
                Assert.Fail();
            }
            catch (Exception) {
                prop1.SetValue("7");
            }
        }

        [Test]
        public void ValidateParameterMarkedIgnoreIsNotUsedAndDoesNotShowAsAction() {
            var obj1 = NewTestObject<Validate1>();
            var action = obj1.GetAction("Do Something More");

            action.AssertIsValidWithParms(2);
            action.AssertIsValidWithParms(11);

            try {
                obj1.GetAction("Validate Do Something More");
                Assert.Fail("'Validate Do Something More' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Do Something More'", e.Message);
            }
        }

        [Test]
        public void ValidateParametersCollectively() {
            var obj1 = NewTestObject<Validate1>();
            var action = obj1.GetAction("Do Something Else");

            var obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            var obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            action.InvokeReturnObject(5, "abar", obj2a);

            action.AssertIsInvalidWithParms(2, "abar", obj2a).AssertLastMessageIs("Something amiss");
            action.AssertIsInvalidWithParms(5, "bar", obj2a).AssertLastMessageIs("Something amiss");
            action.AssertIsInvalidWithParms(5, "abar", obj2b).AssertLastMessageIs("Something amiss");
        }

        [Test]
        public void ValidateParametersIndividually() {
            var obj1 = NewTestObject<Validate1>();
            var action = obj1.GetAction("Do Something");

            var obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            var obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            action.InvokeReturnObject(5, "abar", obj2a);

            action.AssertIsInvalidWithParms(2, "abar", obj2a).AssertLastMessageIs("Value must be between 3 & 10");
            action.AssertIsInvalidWithParms(5, "bar", obj2a).AssertLastMessageIs("Value must start with a");
            action.AssertIsInvalidWithParms(5, "abar", obj2b).AssertLastMessageIs("Invalid Object");
        }

        [Test]
        public virtual void ValidatePropertyMarkedNakedObjectsIgnoreIsNotEffectiveAndDoesNotShowAsAction() {
            var obj = NewTestObject<Validate1>();
            var prop1 = obj.GetPropertyByName("Prop4");
            prop1.AssertFieldEntryIsValid("2");
            prop1.AssertFieldEntryIsValid("11");

            try {
                obj.GetAction("Validate Prop4");
                Assert.Fail("'Validate Prop4' is showing as an action");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Prop4'", e.Message);
            }
        }

        [Test]
        public virtual void ValidateReferenceProperty() {
            var obj1 = NewTestObject<Validate1>();
            var obj1Prop3 = obj1.GetPropertyByName("Prop3");

            var obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            var obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            obj1Prop3.AssertSetObjectInvalid(obj2b).AssertLastMessageIs("Invalid Object");
            obj1Prop3.AssertSetObjectIsValid(obj2a).AssertLastMessageIs("");

            try {
                obj1Prop3.SetObject(obj2b);
                Assert.Fail();
            }
            catch (Exception) {
                obj1Prop3.SetObject(obj2a);
            }
        }

        [Test]
        public virtual void ValidateStringProperty() {
            var obj = NewTestObject<Validate1>();
            var prop1 = obj.GetPropertyByName("Prop2");
            prop1.AssertFieldEntryInvalid("foo").AssertLastMessageIs("Value must start with a");
            prop1.AssertFieldEntryInvalid("bar").AssertLastMessageIs("Value must start with a");
            prop1.SetValue("afoo").AssertLastMessageIs("");
            try {
                prop1.SetValue("bar");
                Assert.Fail();
            }
            catch (Exception) {
                prop1.SetValue("abar");
            }
        }
    }

    #region Classes used in test

    public class MethodsDbContext : DbContext {
        public const string DatabaseName = "Tests";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public MethodsDbContext() : base(Cs) { }

        public DbSet<Auto1> Auto1 { get; set; }
        public DbSet<Auto2> Auto2 { get; set; }
        public DbSet<Auto3> Auto3 { get; set; }
        public DbSet<Choices1> Choices1 { get; set; }
        public DbSet<Choices2> Choices2 { get; set; }
        public DbSet<Choices3> Choices3 { get; set; }
        public DbSet<Choices4> Choices4 { get; set; }
        public DbSet<Clear1> Clear1 { get; set; }
        public DbSet<Clear2> Clear2 { get; set; }
        public DbSet<Clear3> Clear3 { get; set; }
        public DbSet<Created1> Created1 { get; set; }
        public DbSet<Created2> Created2 { get; set; }
        public DbSet<Default1> Default1 { get; set; }
        public DbSet<Default2> Default2 { get; set; }
        public DbSet<Default3> Default3 { get; set; }
        public DbSet<Default4> Default4 { get; set; }
        public DbSet<Deleted1> Deleted1 { get; set; }
        public DbSet<Deleted2> Deleted2 { get; set; }
        public DbSet<Deleting1> Deleting1 { get; set; }
        public DbSet<Deleting2> Deleting2 { get; set; }
        public DbSet<Disable1> Disable1 { get; set; }
        public DbSet<Disable2> Disable2 { get; set; }
        public DbSet<Disable3> Disable3 { get; set; }
        public DbSet<Hide1> Hide1 { get; set; }
        public DbSet<Hide2> Hide2 { get; set; }
        public DbSet<Hide3> Hide3 { get; set; }
        public DbSet<Modify1> Modify1 { get; set; }
        public DbSet<Modify2> Modify2 { get; set; }
        public DbSet<Modify3> Modify3 { get; set; }
        public DbSet<Modify4> Modify4 { get; set; }
        public DbSet<Persisted1> Persisted1 { get; set; }
        public DbSet<Persisted2> Persisted2 { get; set; }
        public DbSet<Persisted3> Persisted3 { get; set; }
        public DbSet<Persisting1> Persisting1 { get; set; }
        public DbSet<Persisting2> Persisting2 { get; set; }
        public DbSet<Title1> Title1 { get; set; }
        public DbSet<Title2> Title2 { get; set; }
        public DbSet<Title3> Title3 { get; set; }
        public DbSet<Title4> Title4 { get; set; }
        public DbSet<Title5> Title5 { get; set; }
        public DbSet<Title6> Title6 { get; set; }
        public DbSet<Title7> Title7 { get; set; }
        public DbSet<Title8> Title8 { get; set; }
        public DbSet<Title9> Title9 { get; set; }
        public DbSet<Title10> Title10 { get; set; }
        public DbSet<Title11> Title11 { get; set; }
        public DbSet<Updated1> Updated1 { get; set; }
        public DbSet<Updated2> Updated2 { get; set; }
        public DbSet<Updating1> Updating1 { get; set; }
        public DbSet<Updating2> Updating2 { get; set; }
        public DbSet<Validate1> Validate1 { get; set; }
        public DbSet<Validate2> Validate2 { get; set; }
        public DbSet<Validate3> Validate3 { get; set; }
        public DbSet<Validate4> Validate4 { get; set; }
        public DbSet<Validate5> Validate5 { get; set; }

        public static void Delete() => Database.Delete(Cs);
    }

    #region AutoComplete

    public class Auto1 {
        public virtual int Id { get; set; }

        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Auto2 Prop3 { get; set; }
        public virtual Auto2 Prop4 { get; set; }

        public IList<string> AutoCompleteProp2(string autoCompleteParm) => new List<string> {"Fee", "Foo", "Fuu"};

        public IQueryable<Auto2> AutoCompleteProp3(string autoCompleteParm) {
            return Container.Instances<Auto2>().Where(a => a.Prop1.ToUpper().Contains(autoCompleteParm.ToUpper()));
        }

        #region Do Something

        public void DoSomething(Auto1 param0, string param1, Auto2 param2) { }

        public Auto1 AutoComplete0DoSomething(string autoCompleteParm) => this;

        public IList<string> AutoComplete1DoSomething(string autoCompleteParm) => AutoCompleteProp2(autoCompleteParm);

        public IQueryable<Auto2> AutoComplete2DoSomething(string autoCompleteParm) => Container.Instances<Auto2>().Take(2);

        #endregion
    }

    public class Auto2 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Auto3 {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Auto2 Prop4 { get; set; }

        //Prop1 not a valid type for auto-complete
        public IQueryable<int> AutoCompleteProp1() => null;

        //Return type does not match
        public IQueryable<Auto2> AutoCompleteProp2(string autoCompleteParm) => null;

        //No corresponding property
        public IQueryable<Auto2> AutoCompleteProp3(string autoCompleteParm) => null;

        //List of domain object not valid
        public IList<Auto2> AutoCompleteProp4(string autoCompleteParm) => null;

        public void DoSomething(int param0, Auto2 param1, Auto2 param2) { }

        //param not a valid type for auto-complete
        public IQueryable<int> AutoComplete0DoSomething(string autoCompleteParm) => null;

        //Return type does not match
        public IQueryable<Auto3> AutoComplete1DoSomething(string autoCompleteParm) => null;

        //Action name mis-spelled
        public IQueryable<Auto2> AutoComplete2DoSomting(string autoCompleteParm) => null;

        //No corresponding param
        public IQueryable<Auto2> AutoComplete3DoSomething(string autoCompleteParm) => null;
    }

    #endregion

    #region Choices

    public class Choices1 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public Choices4 Prop3 { get; set; }

        public List<int> ChoicesProp1() => new List<int> {4, 8, 9};

        public List<string> ChoicesProp2() => new List<string> {"Fee", "Foo", "Fuu"};

        public List<Choices4> ChoicesProp3() => Container.Instances<Choices4>().ToList();

        #region Do Something

        public void DoSomething(int param0, string param1, Choices2 param2) { }

        public List<int> Choices0DoSomething() => ChoicesProp1();

        public List<string> Choices1DoSomething() => ChoicesProp2();

        public List<Choices2> Choices2DoSomething() => Container.Instances<Choices2>().ToList();

        #endregion
    }

    public class Choices2 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Choices3 {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public List<string> ChoicesProp1() => null;

        public string ChoicesProp2() => null;

        public string ChoicesProp3() => null;

        public void DoSomething(int param0, string param1, Choices2 param2) { }

        public List<int> Choices0DoSomthing() => null;

        public List<string> Choices0DoSomething() => null;
    }

    public class Choices4 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Clear

    public class Clear1 {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Clear3 Prop4 { get; set; }

        public void ClearProp1() {
            Prop1 = null;
            Prop0 = "Prop1 has been cleared";
        }

        public void ClearProp4() {
            Prop4 = null;
            Prop3 = "Prop4 has been cleared";
        }
    }

    public class Clear2 {
        public virtual int Id { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        //Has param
        public void ClearProp2(string value) { }

        //Non-void method
        public bool ClearProp3() => false;

        //No corresponding Prop4
        public void ClearProp4() { }
    }

    public class Clear3 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Created

    public class Created1 {
        public bool CreatedCalled;
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void Created() {
            CreatedCalled = true;
        }
    }

    public class Created2 {
        public bool CreatedCalled;
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void created() {
            CreatedCalled = true;
        }
    }

    #endregion

    #region Default

    public class Default1 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Default2 Prop3 { get; set; }

        [DefaultValue(7)]
        public virtual int Prop4 { get; set; }

        [DefaultValue("Bar")]
        public virtual string Prop5 { get; set; }

        public int DefaultProp1() => 8;

        public string DefaultProp2() => "Foo";

        public Default2 DefaultProp3() {
            return Container.Instances<Default2>().FirstOrDefault(x => x.Prop1 == "Bar2");
        }

        public int DefaultProp4() => 8;

        public string DefaultProp5() => "Foo";

        #region Do Something

        public void DoSomething(int param0, string param1, Default4 param2) { }

        public int Default0DoSomething() => DefaultProp1();

        public string Default1DoSomething() => DefaultProp2();

        public Default4 Default2DoSomething() {
            return Container.Instances<Default4>().FirstOrDefault(x => x.Prop1 == "Bar2");
        }

        #endregion

        #region Do Something Else

        public void DoSomethingElse([DefaultValue(7)] int param0,
                                    [DefaultValue("Bar")] string param1) { }

        public int Default0DoSomethingElse() => DefaultProp1();

        public string Default1DoSomethingElse() => DefaultProp2();

        #endregion
    }

    public class Default2 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Default3 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual int DefaultProp1() => 0;

        public virtual string DefaultProp2() => null;

        public void DoSomething(int param0, string param1, Default2 param2) { }

        public string Default0DoSomthing(int param0) => null;

        public string Default0DoSomething(decimal param0) => null;
    }

    public class Default4 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Deleted

    public class Deleted1 {
        public static bool DeletedCalled;
        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Deleted() {
            DeletedCalled = true;
        }

        public void Delete() {
            Container.DisposeInstance(this);
        }
    }

    public class Deleted2 {
        public static bool DeletedCalled;

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void Deleted() {
            DeletedCalled = true;
        }
    }

    #endregion

    #region Deleting

    public class Deleting1 {
        public static bool DeletingCalled;

        public Deleting1() => DeletingCalled = false;

        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        public void Deleting() {
            DeletingCalled = true;
        }

        public void Delete() {
            Container.DisposeInstance(this);
        }
    }

    public class Deleting2 {
        public static bool DeletingCalled;

        public Deleting2() => DeletingCalled = false;

        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        public void deleting() {
            DeletingCalled = true;
        }

        public void Delete() {
            Container.DisposeInstance(this);
        }
    }

    #endregion

    #region Disable

    public class Disable1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public Disable1 Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual ICollection<Disable1> Prop4 { get; set; } = new List<Disable1>();

        public virtual ICollection<Disable1> Prop5 { get; set; } = new List<Disable1>();

        public string DisablePropertyDefault() => "This property has been disabled by default";

        public string DisableProp3() => null;

        public string DisableProp5() => null;

        public void Action1() { }

        public void Action2(string param) { }
    }

    public class Disable2 {
        public virtual int Id { get; set; }
        public virtual string Prop1 { get; set; }

        public virtual Disable1 Prop2 { get; set; }

        public virtual ICollection<Disable1> Prop4 { get; set; } = new List<Disable1>();

        public string DisableActionDefault() => "This property has been disabled by default";

        public void Action1() { }

        public void Action2(string param) { }

        public void Action3() { }

        public string DisableAction3() => null;
    }

    public class Disable3 {
        public virtual int Id { get; set; }

        [Disabled(WhenTo.Never)]
        [Optionally]
        public virtual string Prop4 { get; set; }

        [Optionally]
        public virtual string Prop6 { get; set; }

        [Optionally]
        public virtual string Prop7 { get; set; }

        [Optionally]
        public virtual string Prop8 { get; set; }

        public string DisableProp6() {
            if (Prop4 == "Disable 6") {
                return "Disabled Message";
            }

            return null;
        }

        public string DisableProp1() => null;

        public bool DisableProp4() => false;

        public void Action1() { }

        //OK
        public string DisableAction1() => DisableProp6();

        public void Action2(string parm1) { }

        //Disable should not take any parms  -  even matching ones
        public string DisableAction2(string parm1) => "x";

        public void Action3() { }

        //Disable should not take any parms  -  even matching ones
        public string DisableAction3(int parm1) => "x";

        //Disable should not take any parms  -  even matching ones
        public string DisableProp7(string parm1) => "x";

        //Disable should not take any parms  -  even matching ones
        public string DisableProp8(int parm1) => "x";

        public void Action4() { }

        public string DisableAction4() => "disabled";
    }

    #endregion

    #region Hide

    public class Hide1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual Hide1 Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual ICollection<Hide1> Prop4 { get; set; } = new List<Hide1>();

        public virtual ICollection<Hide1> Prop5 { get; set; } = new List<Hide1>();

        public bool HidePropertyDefault() => true;

        public bool HideProp3() => false;

        public bool HideProp5() => false;

        public void Action1() { }

        public void Action2(string param) { }
    }

    public class Hide2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual Hide1 Prop2 { get; set; }

        public virtual ICollection<Hide1> Prop4 { get; set; } = new List<Hide1>();

        public bool HideActionDefault() => true;

        public void Action1() { }

        public void Action2(string param) { }

        public void Action3() { }

        public bool HideAction3() => false;
    }

    public class Hide3 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop4 { get; set; }

        [Optionally]
        public virtual string Prop6 { get; set; }

        [Optionally]
        public virtual string Prop8 { get; set; }

        [Optionally]
        public virtual string Prop9 { get; set; }

        public bool HideProp6() => Prop4 == "Hide 6";

        public void DoSomething() { }

        public bool HideDoSomething() => HideProp6();

        public void DoSomethingElse() { }

        public bool HideProp7() => false;

        public string HideProp4() => null;

        public bool HideDoSomthingElse() => false;

        public string HideDoSomethingElse() => null;

        public void Action1() { }

        public bool HideAction1(string parm1) => true;

        public void Action2(string parm1) { }

        public bool HideAction2(string parm1) => true;

        public bool HideProp8(int prop8) => true;

        public bool HideProp9(string prop9) => true;
    }

    #endregion

    #region Modify

    public class Modify1 {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Modify3 Prop4 { get; set; }

        public void ModifyProp1(string value) {
            Prop1 = value;
            Prop0 = "Prop1 has been modified";
        }

        public void ModifyProp4(Modify3 value) {
            Prop4 = value;
            Prop3 = "Prop4 has been modified";
        }
    }

    public class Modify2 {
        public virtual int Id { get; set; }

        public virtual string Prop2 { get; set; }

        //Has the wrong type of parameter

        public virtual string Prop3 { get; set; }
        public void ModifyProp2(int value) { }

        //Non-void method
        public bool ModifyProp3(string value) => false;

        //No corresponding Prop4
        public void ModifyProp4(string value) { }
    }

    public class Modify3 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Modify4 {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Modify3 Prop4 { get; set; }

        public void ModifyProp1(string value) {
            Prop1 = value;
            Prop0 = "Prop1 has been modified";
        }

        public void ModifyProp4(Modify3 value) {
            Prop4 = value;
            Prop3 = "Prop4 has been modified";
        }

        public void ClearProp1() {
            Prop1 = null;
        }

        public void ClearProp4() {
            Prop4 = null;
        }
    }

    #endregion

    #region Persisted

    public class Persisted1 {
        public virtual int Id { get; set; }

        public void Persisted() {
            throw new DomainException("Persisted called");
        }
    }

    public class Persisted2 {
        public bool PersistedCalled;

        public virtual int Id { get; set; }

        public void persisted() {
            throw new DomainException("persisted called");
        }
    }

    public class Persisted3 {
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public void Persisted() {
            throw new DomainException("Persisted");
        }
    }

    #endregion

    #region Persisting

    public class Persisting1 {
        public static bool PersistingCalled;

        public Persisting1() => PersistingCalled = false;

        public virtual int Id { get; set; }

        public void Persisting() {
            PersistingCalled = true;
        }
    }

    public class Persisting2 {
        public static bool PersistingCalled;

        public virtual int Id { get; set; }

        public void peristing() {
            PersistingCalled = true;
        }
    }

    #endregion

    #region Title & ToString

    public class Title1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => Prop1;
    }

    public class Title2 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Bar";
    }

    public class Title3 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public string Title() => Prop1;
    }

    public class Title4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title() => Prop1;

        public override string ToString() => "Bar";
    }

    public class Title5 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => Prop1;
    }

    public class Title6 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString() {
            var tb = Container.NewTitleBuilder();
            tb.Append("TB6");
            return tb.ToString();
        }
    }

    public class Title7 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString() {
            var tb = Container.NewTitleBuilder("TB7");
            return tb.ToString();
        }
    }

    public class Title8 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString() {
            var t1 = Container.NewTransientInstance<Title1>();
            t1.Prop1 = "TB8";
            Container.Persist(ref t1);
            var tb = Container.NewTitleBuilder(t1);
            return tb.ToString();
        }
    }

    public class Title9 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString() {
            var tb = Container.NewTitleBuilder(null, "TB9");
            return tb.ToString();
        }
    }

    public class Title10 {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString() {
            //ToString()
            var t1 = Container.NewTransientInstance<Title1>();
            t1.Prop1 = "t1";
            Container.Persist(ref t1);

            //TitleAttribute
            var t2 = Container.NewTransientInstance<Title1>();
            t2.Prop1 = "t2";
            Container.Persist(ref t2);

            //TitleMethod
            var t3 = Container.NewTransientInstance<Title1>();
            t3.Prop1 = "t3";
            Container.Persist(ref t3);

            var tb = Container.NewTitleBuilder();
            tb.Append("&", "x"); //  "x";
            tb.Append("&", "y"); //  "x& y";
            tb.Append("y"); //  "x& y y";
            //Append objects, each having different title mechanisms
            tb.Append(t1); //"x& y y t1"
            tb.Append(t2); //"x& y y t1 t2"
            tb.Append(t3); //"x& y y t1 t2 t3"
            tb.Append("%", null); //no change
            tb.Append("$", t1); //"x& y y t1 t2 t3$ t1"
            tb.Concat("c"); //"x& y y t1 t2 t3$ t1c"
            tb.Concat(t1); //"x& y y t1 t2 t3$ t1ct1"
            tb.Concat(null); //unchanged
            tb.Concat("*", t1); //"x& y y t1 t2 t3$ t1ct1*t1"
            tb.Concat("*", null); //"x& y y t1 t2 t3$ t1ct1*t1"

            tb.Append(":", null, "d", null); //unchanged
            tb.Concat(":", null, "d", null); //unchanged
            tb.Append(":", null, "d", "no date"); //"x& y y t1 t2 t3$ t1ct1*t1: no date"
            tb.Concat(":", null, "d", "no date"); //"x& y y t1 t2 t3$ t1ct1*t1: no date no date"

            tb.Append(new DateTime(2007, 4, 2), "d", null); //"x& y y t1 t2 t3$ t1ct1*t1: no date no date 02/04/2007"
            tb.Append(Sex.Female); //"x& y y t1 t2 t3$ t1ct1*t1: no date no date 02/04/2007 Female"
            tb.Append(Sex.NotSpecified); //"x& y y t1 t2 t3$ t1ct1*t1: no date no date 02/04/2007 Female Not Specified"
            return tb.ToString();
        }
    }

    public class Title11 {
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public string Title() => throw new Exception("Title method should not have been called");
    }

    public enum Sex {
        Male = 1,
        Female = 2,
        Unknown = 3,
        NotSpecified = 4
    }

    #endregion

    #region Updated

    public class Updated1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Updated() {
            throw new DomainException("Update called");
        }
    }

    public class Updated2 {
        public static bool UpdatedCalled;

        public Updated2() => UpdatedCalled = false;

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void updated() {
            UpdatedCalled = true;
        }
    }

    #endregion

    #region Updating

    public class Updating1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Updating() {
            throw new DomainException("Updating called");
        }
    }

    public class Updating2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void updating() {
            throw new DomainException("Updating called");
        }
    }

    #endregion

    #region Validate

    public class Validate1 {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Validate2 Prop3 { get; set; }

        public virtual int Prop4 { get; set; }

        public string ValidateProp1(int value) {
            if (value < 3 || value > 10) {
                return "Value must be between 3 & 10";
            }

            return null;
        }

        public string ValidateProp2(string value) {
            if (!value.StartsWith("a")) {
                return "Value must start with a";
            }

            return null;
        }

        public string ValidateProp3(Validate2 value) {
            if (!value.Prop1.StartsWith("a")) {
                return "Invalid Object";
            }

            return null;
        }

        [NakedObjectsIgnore]
        public string ValidateProp4(int value) {
            if (value < 3 || value > 10) {
                return "Value must be between 3 & 10";
            }

            return null;
        }

        public void DoSomethingMore(int param0) { }

        [NakedObjectsIgnore]
        public string Validate0DoSomethingMore(int value) => ValidateProp1(value);

        #region Do Something

        public void DoSomething(int param0, string param1, Validate2 param2) { }

        public string Validate0DoSomething(int value) => ValidateProp1(value);

        public string Validate1DoSomething(string value) => ValidateProp2(value);

        public string Validate2DoSomething(Validate2 value) => ValidateProp3(value);

        #endregion

        #region Do Something Else

        public void DoSomethingElse(int param0, string param1, Validate2 param2) { }

        public string ValidateDoSomethingElse(int param0, string param1, Validate2 param2) {
            if (ValidateProp1(param0) != null || ValidateProp2(param1) != null || ValidateProp3(param2) != null) {
                return "Something amiss";
            }

            return null;
        }

        #endregion

        #region DoSomethingWithManyParams

        public void DoSomethingWithManyParams(string param0, string param1, string param2, string param3, string param4, string param5, string param6) { }

        public string ValidateDoSomethingWithManyParams(string param0, string param1, string param2, string param3, string param4, string param5, string param6) => param0 != "x" ? "Invalid" : null;

        #endregion
    }

    public class Validate2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Validate3 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public string ValidateProp1(int value) => null;

        public bool ValidateProp2(string value) => false;

        public string ValidateProp3(string value) => null;

        public void DoSomething(int par1) { }

        public string ValidateDoSomething(decimal par1) => null;

        public string Validate0DoSomething(bool par1) => null;

        public string Validate1DoSomething(int par1) => null;
    }

    public class Validate4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public string Validate(string prop1, string prop2) {
            if (prop1 != prop2) {
                return "Expect prop1 == prop2";
            }

            return null;
        }
    }

    public class Validate5 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual int Prop3 { get; set; }

        [Optionally]
        public virtual Validate4 Prop4 { get; set; }

        public string Validate(Validate4 prop4, string prop1, int prop3, string prop2) {
            if (prop1 != prop2 || prop3 == 0 || prop4 == null) {
                return "Condition Fail";
            }

            return null;
        }
    }

    #endregion

    #endregion
}