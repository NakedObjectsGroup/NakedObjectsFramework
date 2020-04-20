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
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ITestAction = NakedObjects.Xat.ITestAction;

namespace NakedObjects.SystemTest.Method
{
    [TestFixture]
    public class TestRecognisedMethods : AbstractSystemTest<MethodsDbContext>
    {
        #region Setup/Teardown

        [OneTimeSetUp]
        public void ClassInitialize()
        {
            MethodsDbContext.Delete();
            var context = Activator.CreateInstance<MethodsDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
        }

        [SetUp()]
        public void SetUp()
        {

            StartTest();
        }

        #endregion

        #region Configuration

        protected override string[] Namespaces
        {
            get { return new[] { typeof(Auto1).Namespace }; }
        }

        protected override Type[] Types
        {
            get { return new[] { typeof(Sex) }; }
        }

        protected override object[] MenuServices
        {
            get
            {
                return new object[] {
                    new SimpleRepository<Auto1>(),
                    new SimpleRepository<Auto2>(),
                    new SimpleRepository<Auto3>(),
                    new SimpleRepository<Choices1>(),
                    new SimpleRepository<Choices2>(),
                    new SimpleRepository<Choices3>(),
                    new SimpleRepository<Choices4>(),
                    new SimpleRepository<Clear1>(),
                    new SimpleRepository<Clear2>(),
                    new SimpleRepository<Clear3>(),
                    new SimpleRepository<Created1>(),
                    new SimpleRepository<Created2>(),
                    new SimpleRepository<Default1>(),
                    new SimpleRepository<Default2>(),
                    new SimpleRepository<Default3>(),
                    new SimpleRepository<Default4>(),
                    new SimpleRepository<Deleted1>(),
                    new SimpleRepository<Deleted2>(),
                    new SimpleRepository<Deleting1>(),
                    new SimpleRepository<Deleting2>(),
                    new SimpleRepository<Disable1>(),
                    new SimpleRepository<Disable2>(),
                    new SimpleRepository<Disable3>(),
                    new SimpleRepository<Hide1>(),
                    new SimpleRepository<Hide2>(),
                    new SimpleRepository<Hide3>(),
                    new SimpleRepository<Modify1>(),
                    new SimpleRepository<Modify2>(),
                    new SimpleRepository<Modify3>(),
                    new SimpleRepository<Modify4>(),
                    new SimpleRepository<Persisted1>(),
                    new SimpleRepository<Persisted2>(),
                    new SimpleRepository<Persisted3>(),
                    new SimpleRepository<Persisting1>(),
                    new SimpleRepository<Persisting2>(),
                    new SimpleRepository<Title1>(),
                    new SimpleRepository<Title2>(),
                    new SimpleRepository<Title3>(),
                    new SimpleRepository<Title4>(),
                    new SimpleRepository<Title5>(),
                    new SimpleRepository<Title6>(),
                    new SimpleRepository<Title7>(),
                    new SimpleRepository<Title8>(),
                    new SimpleRepository<Title9>(),
                    new SimpleRepository<Title10>(),
                    new SimpleRepository<Title11>(),
                    new SimpleRepository<Updated1>(),
                    new SimpleRepository<Updated2>(),
                    new SimpleRepository<Updating1>(),
                    new SimpleRepository<Updating2>(),
                    new SimpleRepository<Validate1>(),
                    new SimpleRepository<Validate2>(),
                    new SimpleRepository<Validate3>(),
                    new SimpleRepository<Validate4>(),
                    new SimpleRepository<Validate5>()
                };
            }
        }

        #endregion

        #region AutoComplete

        [Test]
        public void RecognisedAutoCompleteMethodDoesNotShowUpAsAction()
        {
            var obj1 = NewTestObject<Auto1>();
            try
            {
                var act = obj1.GetAction("Auto Complete Prop2");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete Prop2'", e.Message);
            }
            try
            {
                var act = obj1.GetAction("Auto Complete Prop3");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete Prop3'", e.Message);
            }

            try
            {
                obj1.GetAction("Auto Complete0 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete0 Do Something'", e.Message);
            }
            try
            {
                obj1.GetAction("Auto Complete1 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete1 Do Something'", e.Message);
            }
            try
            {
                obj1.GetAction("Auto Complete2 Do Something");
                Assert.Fail("Should not get to here!");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Auto Complete2 Do Something'", e.Message);
            }
        }

        [Test]
        public void UnmatchedAutoCompleteMethodShowsUpAsAction()
        {
            ITestObject obj3 = NewTestObject<Auto3>();
            obj3.GetAction("Auto Complete Prop1");
            obj3.GetAction("Auto Complete Prop2");
            obj3.GetAction("Auto Complete Prop3");
            obj3.GetAction("Auto Complete 0 Do Something");
            obj3.GetAction("Auto Complete 1 Do Something");
            obj3.GetAction("Auto Complete 2 Do Somting");
            obj3.GetAction("Auto Complete 3 Do Something");
        }

        private void CreateAuto2(string prop1)
        {
            var obj2 = NewTestObject<Auto2>();
            obj2.GetPropertyByName("Prop1").SetValue(prop1);
            obj2.Save();
        }

        [Test]
        public virtual void AutoCompleteParameters()
        {
            CreateAuto2("Bar1");
            CreateAuto2("Bar2");
            CreateAuto2("Bar3");

            var obj1 = NewTestObject<Auto1>();
            var action = obj1.GetAction("Do Something");
            ITestNaked[] cho0 = action.Parameters[0].GetCompletions("any");
            Assert.AreEqual(1, cho0.Count());
            ITestNaked[] cho1 = action.Parameters[1].GetCompletions("any");
            Assert.AreEqual(3, cho1.Count());

            ITestNaked[] cho2 = action.Parameters[2].GetCompletions("bar");
            Assert.AreEqual(2, cho2.Count());
        }

        [Test]
        public virtual void AutoCompleteReferenceProperty()
        {
            CreateAuto2("Foo1");
            CreateAuto2("Foo2");
            CreateAuto2("Foo3");

            var obj1 = NewTestObject<Auto1>();
            ITestProperty prop = obj1.GetPropertyByName("Prop3");
            ITestNaked[] cho = prop.GetCompletions("foo");
            Assert.AreEqual(3, cho.Count());
            Assert.AreEqual("Foo1", cho[0].Title);
        }

        [Test]
        public virtual void AutoCompleteStringProperty()
        {
            var obj1 = NewTestObject<Auto1>();
            ITestProperty prop = obj1.GetPropertyByName("Prop2");
            ITestNaked[] cho = prop.GetCompletions("any");
            Assert.AreEqual(3, cho.Count());
            Assert.AreEqual("Fee", cho[0].Title);
        }

        #endregion

        #region Choices

        [Test]
        public void ChoicesMethodDoesNotShowUpAsAction()
        {
            var obj1 = NewTestObject<Choices1>();
            try
            {
                obj1.GetAction("Choices Prop1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Choices Prop1'", e.Message);
            }

            try
            {
                obj1.GetAction("Choices 0 Do Something");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Choices 0 Do Something'", e.Message);
            }
        }

        [Test]
        public virtual void ChoicesNumericProperty()
        {
            var obj1 = NewTestObject<Choices1>();
            ITestProperty prop = obj1.GetPropertyByName("Prop1");
            ITestNaked[] cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Count());
            Assert.AreEqual("4", cho[0].Title);
        }

        private void CreateChoices<T>(string prop1)
        {
            var obj2 = NewTestObject<T>();
            obj2.GetPropertyByName("Prop1").SetValue(prop1);
            obj2.Save();
        }

        [Test]
        public virtual void ChoicesParameters()
        {
            CreateChoices<Choices2>("Bar1");
            CreateChoices<Choices2>("Bar2");
            CreateChoices<Choices2>("Bar3");

            var obj1 = NewTestObject<Choices1>();
            ITestAction action = obj1.GetAction("Do Something");
            ITestNaked[] cho0 = action.Parameters[0].GetChoices();
            Assert.AreEqual(3, cho0.Count());
            Assert.AreEqual("4", cho0[0].Title);

            ITestNaked[] cho1 = action.Parameters[1].GetChoices();
            Assert.AreEqual(3, cho1.Count());
            Assert.AreEqual("Fee", cho1[0].Title);

            ITestNaked[] cho2 = action.Parameters[2].GetChoices();
            Assert.AreEqual(3, cho2.Count());
            Assert.AreEqual("Bar1", cho2[0].Title);
        }

        [Test]
        public virtual void ChoicesReferenceProperty()
        {
            CreateChoices<Choices4>("Bar1");
            CreateChoices<Choices4>("Bar2");
            CreateChoices<Choices4>("Bar3");

            var obj1 = NewTestObject<Choices1>();
            ITestProperty prop = obj1.GetPropertyByName("Prop3");
            ITestNaked[] cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Count());
            Assert.AreEqual("Bar1", cho[0].Title);
        }

        [Test]
        public virtual void ChoicesStringProperty()
        {
            var obj1 = NewTestObject<Choices1>();
            ITestProperty prop = obj1.GetPropertyByName("Prop2");
            ITestNaked[] cho = prop.GetChoices();
            Assert.AreEqual(3, cho.Count());
            Assert.AreEqual("Fee", cho[0].Title);
        }

        [Test]
        public void UnmatchedChoicesMethodShowsUpAsAction()
        {
            var obj3 = NewTestObject<Choices3>();
            obj3.GetAction("Choices Prop1");
            obj3.GetAction("Choices Prop2");
            obj3.GetAction("Choices Prop3");
            obj3.GetAction("Choices 0 Do Somthing");
            obj3.GetAction("Choices 0 Do Something");
        }

        #endregion

        #region Clear

        // Note Clear Prefix has been removed as a recognised prefix for complementary actions 
        [Test]
        public void ClearMethodDoesShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Clear1>();
            var action = obj1.GetAction("Clear Prop1");
            action.AssertHasFriendlyName("Clear Prop1");
        }


        [Test]
        public void UnmatchedClearMethodShowsUpAsAnAction()
        {
            ITestObject obj2 = NewTestObject<Clear2>();
            obj2.GetAction("Clear Prop2");
            obj2.GetAction("Clear Prop3");
            obj2.GetAction("Clear Prop4");
        }

        #endregion

        #region Created

        [Test]
        public void CreatedCalled()
        {
            ITestObject obj1 = NewTestObject<Created1>();
            var dom1 = (Created1)obj1.GetDomainObject();
            Assert.IsTrue(dom1.CreatedCalled);
        }

        [Test]
        public void CreatedDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Created1>();
            try
            {
                obj1.GetAction("Created");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Created'", e.Message);
            }
        }

        [Test]
        public void LowerCaseCreatedNotRecognisedAndShowsAsAction()
        {
            ITestObject obj1 = NewTestObject<Created2>();
            var dom1 = (Created2)obj1.GetDomainObject();
            Assert.IsFalse(dom1.CreatedCalled);
            obj1.GetAction("Created");
        }

        #endregion

        #region Default

        [Test]
        public void DefaultMethodDoesNotShowUpAsAction()
        {
            var obj = NewTestObject<Default1>();
            try
            {
                obj.GetAction("Default Prop1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Default Prop1'", e.Message);
            }
            try
            {
                obj.GetAction("Default 0 Do Something");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Default 0 Do Something'", e.Message);
            }
        }

        [Test]
        public virtual void DefaultNumericProperty()
        {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop1");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [Test]
        public void DefaultParameters()
        {
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
            string def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            string def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);

            string def2 = action.Parameters[2].GetDefault().Title;
            Assert.IsNotNull(def2);
            Assert.AreEqual("Bar2", def2);
        }

        [Test]
        public virtual void DefaultReferenceProperty()
        {
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
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Bar2", def);
        }

        [Test]
        public virtual void DefaultStringProperty()
        {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop2");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }

        [Test]
        public void UnmatchedDefaultMethodShowsUpAsAction()
        {
            var obj = NewTestObject<Default3>();
            obj.GetAction("Default Prop1");
            obj.GetAction("Default Prop2");
            obj.GetAction("Default 0 Do Somthing");
            obj.GetAction("Default 0 Do Something");
        }

        [Test]
        public void DefaultNumericMethodOverAnnotation()
        {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop4");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [Test]
        public void DefaultStringMethodOverAnnotation()
        {
            var obj1 = NewTestObject<Default1>();
            var prop = obj1.GetPropertyByName("Prop5");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }

        [Test]
        public void DefaultParametersOverAnnotation()
        {
            var obj1 = NewTestObject<Default1>();
            var action = obj1.GetAction("Do Something Else");
            string def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            string def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);
        }

        #endregion

        #region Deleted

        [Test]
        public void DeletedCalled()
        {
            ITestObject obj1 = NewTestObject<Deleted1>();
            var dom1 = (Deleted1)obj1.GetDomainObject();
            obj1.Save();

            Assert.IsFalse(Deleted1.DeletedCalled);
            obj1.GetAction("Delete").Invoke();
            Assert.IsTrue(Deleted1.DeletedCalled);
            Deleted1.DeletedCalled = false;
        }

        [Test]
        public void DeletedDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Deleted1>();
            try
            {
                obj1.GetAction("Deleted");
                Assert.Fail("Should not get here");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleted'", e.Message);
            }
        }

        [Test]
        public void LowerCaseDeletedNotRecognisedAndShowsAsAction()
        {
            ITestObject obj1 = NewTestObject<Deleted2>();
            var dom1 = (Deleted2)obj1.GetDomainObject();
            Assert.IsFalse(Deleted2.DeletedCalled);

            try
            {
                obj1.GetAction("Deleted");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleted'", e.Message);
            }
        }

        #endregion

        #region Deleting

        [Test]
        public void DeletingCalled()
        {
            ITestObject obj1 = NewTestObject<Deleting1>();
            var dom1 = (Deleting1)obj1.GetDomainObject();
            obj1.Save();

            Assert.IsFalse(Deleting1.DeletingCalled);

            obj1.GetAction("Delete").InvokeReturnObject();

            Assert.IsTrue(Deleting1.DeletingCalled);
        }

        [Test]
        public void DeletingDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Deleting1>();
            try
            {
                obj1.GetAction("Deleting");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Deleting'", e.Message);
            }
        }

        [Test]
        public void LowerCaseDeletingNotRecognisedAndShowsAsAction()
        {
            ITestObject obj1 = NewTestObject<Deleting2>().Save();
            var dom1 = (Deleting2)obj1.GetDomainObject();

            //Check method is visible as an action
            obj1.GetAction("Deleting").AssertIsVisible();

            Assert.IsFalse(Deleting2.DeletingCalled);
            obj1.GetAction("Delete").InvokeReturnObject();
            Assert.IsFalse(Deleting2.DeletingCalled); //Still false
        }

        #endregion

        #region Disable

        [Test]
        public void DisableAction()
        {
            ITestObject obj = NewTestObject<Disable3>();
            obj.GetPropertyByName("Prop4").SetValue("avalue");
            obj.GetPropertyByName("Prop6").SetValue("avalue");
            obj.Save();
            obj.GetAction("Action1").AssertIsEnabled();

            obj.GetPropertyByName("Prop4").SetValue("Disable 6");
            obj.GetAction("Action4").AssertIsDisabled();
        }

        [Test]
        public void DisableMethodDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Disable3>();
            try
            {
                obj.GetAction("Disable Prop6");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Prop6'", e.Message);
            }
        }

        [Test]
        public void DisableProperty()
        {
            ITestObject obj = NewTestObject<Disable3>();
            ITestProperty prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsModifiable();
            obj.Save();
            prop6.AssertIsModifiable();

            ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.SetValue("Disable 6");
            prop6.AssertIsUnmodifiable();
        }

        [Test]
        public void UnmatchedDisableMethodShowsUpAsAction()
        {
            ITestObject obj = NewTestObject<Disable3>();
            obj.GetAction("Disable Prop1");
            obj.GetAction("Disable Prop4");
        }

        [Test] //Pending #9228
        public void DisableMethodsWithParamsNotRecognised()
        {
            ITestObject obj = NewTestObject<Disable3>();
            obj.GetAction("Disable Action2");
            obj.GetAction("Disable Action3");
            obj.GetAction("Disable Prop7");
            obj.GetAction("Disable Prop8");
        }

        [Test]
        public void DisableActionDefault()
        {
            ITestObject obj = NewTestObject<Disable2>();
            obj.GetAction("Action1").AssertIsDisabled();
            obj.GetAction("Action2").AssertIsDisabled();
        }

        [Test]
        public void DisableActionDefaultDoesNotDisableProperties()
        {
            ITestObject obj = NewTestObject<Disable2>();
            obj.GetPropertyByName("Prop1").AssertIsModifiable();
            obj.GetPropertyByName("Prop2").AssertIsModifiable();
            //obj.GetPropertyByName("Prop4").AssertIsModifiable(); - collection disabled by default
        }

        [Test]
        public void DisableActionDefaultDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Disable2>();
            try
            {
                obj.GetAction("Disable Action Default");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Action Default'", e.Message);
            }
        }

        [Test]
        public void DisableActionDefaultOverriddenByActionLevelMethod()
        {
            ITestObject obj = NewTestObject<Disable2>();
            obj.GetAction("Action3").AssertIsEnabled();
        }

        [Test]
        public void DisablePropertyDefault()
        {
            ITestObject obj = NewTestObject<Disable1>();
            obj.GetPropertyByName("Prop1").AssertIsUnmodifiable();
            obj.GetPropertyByName("Prop2").AssertIsUnmodifiable();
            obj.GetPropertyByName("Prop4").AssertIsUnmodifiable();
        }

        [Test]
        public void DisablePropertyDefaultDoesNotDisableActions()
        {
            ITestObject obj = NewTestObject<Disable1>();
            obj.GetAction("Action1").AssertIsEnabled();
            obj.GetAction("Action2").AssertIsEnabled();
        }

        [Test]
        public void DisablePropertyDefaultDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Disable1>();
            try
            {
                obj.GetAction("Disable Property Default");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Disable Property Default'", e.Message);
            }
        }

        [Test]
        public void DisablePropertyPropertyOverriddenByPropertyLevelMethod()
        {
            ITestObject obj = NewTestObject<Disable1>();
            obj.GetPropertyByName("Prop3").AssertIsModifiable();
            //obj.GetPropertyByName("Prop5").AssertIsModifiable(); - collection disabled by default
        }

        #endregion

        #region Hide

        [Test]
        public void HideAction()
        {
            ITestObject obj = NewTestObject<Hide3>();
            obj.Save();
            obj.GetAction("Do Something").AssertIsVisible();
            obj.GetPropertyByName("Prop4").SetValue("Hide 6");
            obj.GetAction("Do Something").AssertIsInvisible();
        }

        [Test]
        public void HideMethodDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Hide3>();
            try
            {
                obj.GetAction("Hide Prop6");
                Assert.Fail("'Hide Prop6' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Prop6'", e.Message);
            }
        }

        [Test]
        public void HideProperty()
        {
            ITestObject obj = NewTestObject<Hide3>();
            ITestProperty prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsVisible();
            obj.Save();
            prop6.AssertIsVisible();

            ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.SetValue("Hide 6");
            prop6.AssertIsInvisible();
        }

        [Test]
        public void UnmatchedHideMethodShowsUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Hide3>();
            obj.GetAction("Hide Prop7");
            obj.GetAction("Hide Prop4");
            obj.GetAction("Hide Do Something Else");
            obj.GetAction("Hide Do Somthing Else");
        }

        [Test] // pending #9228
        public void HideMethodsWithParamsNotRecognised()
        {
            ITestObject obj = NewTestObject<Hide3>();
            obj.GetAction("Hide Prop8");
            obj.GetAction("Hide Prop9");
            obj.GetAction("Hide Action1");
            obj.GetAction("Hide Action2");
        }

        [Test]
        public void HideActionDefault()
        {
            ITestObject obj = NewTestObject<Hide2>();
            obj.GetAction("Action1").AssertIsInvisible();
            obj.GetAction("Action2").AssertIsInvisible();
        }

        [Test]
        public void HideActionDefaultDoesNotHideProperties()
        {
            ITestObject obj = NewTestObject<Hide2>();
            obj.GetPropertyByName("Prop1").AssertIsVisible();
            obj.GetPropertyByName("Prop2").AssertIsVisible();
            obj.GetPropertyByName("Prop4").AssertIsVisible();
        }

        [Test]
        public void HideActionDefaultDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Hide2>();
            try
            {
                obj.GetAction("Hide Action Default");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Action Default'", e.Message);
            }
        }

        [Test]
        public void HideActionDefaultOverriddenByActionLevelMethod()
        {
            ITestObject obj = NewTestObject<Hide2>();
            obj.GetAction("Action3").AssertIsVisible();
        }

        [Test]
        public void HidePropertyDefault()
        {
            ITestObject obj = NewTestObject<Hide1>();
            obj.GetPropertyByName("Prop1").AssertIsInvisible();
            obj.GetPropertyByName("Prop2").AssertIsInvisible();
            obj.GetPropertyByName("Prop4").AssertIsInvisible();
        }

        [Test]
        public void HidePropertyDefaultDoesNotHideActions()
        {
            ITestObject obj = NewTestObject<Hide1>();
            obj.GetAction("Action1").AssertIsVisible();
            obj.GetAction("Action2").AssertIsVisible();
        }

        [Test]
        public void HidePropertyDefaultDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Hide1>();
            try
            {
                obj.GetAction("Hide Property Default");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Property Default'", e.Message);
            }
        }

        [Test]
        public void HidePropertyPropertyOverriddenByPropertyLevelMethod()
        {
            ITestObject obj = NewTestObject<Hide1>();
            obj.GetPropertyByName("Prop3").AssertIsVisible();
            obj.GetPropertyByName("Prop5").AssertIsVisible();
        }

        #endregion

        #region Modify

        [Test]
        public void ModifyMethodDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Modify1>();
            try
            {
                obj1.GetAction("Modify Prop1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Modify Prop1'", e.Message);
            }
        }

        [Test]
        public void ModifyMethodOnReferenceProperty()
        {
            ITestObject obj3 = NewTestObject<Modify3>();
            obj3.GetPropertyByName("Prop1").SetValue("Foo");
            obj3.Save();

            ITestObject obj1 = NewTestObject<Modify1>();
            ITestProperty prop3 = obj1.GetPropertyByName("Prop3");
            ITestProperty prop4 = obj1.GetPropertyByName("Prop4");

            prop3.AssertIsEmpty();
            prop4.AssertIsEmpty();

            prop4.SetObject(obj3);
            prop4.AssertObjectIsEqual(obj3);
            prop3.AssertValueIsEqual("Prop4 has been modified");
        }

        [Test]
        public void ModifyMethodOnValueProperty()
        {
            ITestObject obj = NewTestObject<Modify1>();
            ITestProperty prop0 = obj.GetPropertyByName("Prop0");
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");

            prop0.AssertIsEmpty();
            prop1.AssertIsEmpty();

            prop1.SetValue("Foo");
            prop1.AssertValueIsEqual("Foo");
            prop0.AssertValueIsEqual("Prop1 has been modified");
        }

        [Test]
        public void CalledWhenReferencePropertyCleared()
        {
            ITestObject obj3 = NewTestObject<Modify3>();
            obj3.GetPropertyByName("Prop1").SetValue("Foo");
            obj3.Save();

            ITestObject obj1 = NewTestObject<Modify1>();
            ITestProperty prop3 = obj1.GetPropertyByName("Prop3");
            ITestProperty prop4 = obj1.GetPropertyByName("Prop4");

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
        public void CalledWhenValuePropertyIsCleared()
        {
            ITestObject obj = NewTestObject<Modify1>();
            ITestProperty prop0 = obj.GetPropertyByName("Prop0");
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");

            prop1.SetValue("Foo");
            prop0.SetValue("Foo");

            prop1.ClearValue();
            prop1.AssertIsEmpty();
            prop0.AssertValueIsEqual("Prop1 has been modified");
        }

        [Test]
        public void UnmatchedModifyMethodShowsUpAsAnAction()
        {
            ITestObject obj2 = NewTestObject<Modify2>();
            obj2.GetAction("Modify Prop2");
            obj2.GetAction("Modify Prop3");
            obj2.GetAction("Modify Prop4");
        }

        #endregion

        #region Persisted

        [Test]
        public void LowerCasePersistedNotRecognisedAndShowsAsAction()
        {
            ITestObject obj1 = NewTestObject<Persisted2>();
            var dom1 = (Persisted2)obj1.GetDomainObject();
            Assert.IsFalse(dom1.PersistedCalled);
            obj1.GetAction("Persisted");
        }

        [Test]
        public void PersistedCalled()
        {
            ITestObject obj1 = NewTestObject<Persisted1>();
            var dom1 = (Persisted1)obj1.GetDomainObject();
            try
            {
                obj1.Save();
                Assert.Fail("Shouldn't get to here");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Persisted called", e.Message);
            }
        }

        [Test]
        public void PersistedDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Persisted1>();
            try
            {
                obj1.GetAction("Persisted");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisted'", e.Message);
            }
        }

        [Test]
        public void PersistedMarkedAsIgnoredIsNotCalledAndIsNotAnAction()
        {
            ITestObject obj = NewTestObject<Persisted3>();
            obj.Save(); //Would throw an exception if the Persisted had been called.
            try
            {
                obj.GetAction("Persisted");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisted'", e.Message);
            }
        }

        #endregion

        #region Persisting

        [Test]
        public void PersistingCalled()
        {
            ITestObject obj1 = NewTestObject<Persisting1>();
            var dom1 = (Persisting1)obj1.GetDomainObject();
            Assert.IsFalse(Persisting1.PersistingCalled);

            obj1.Save();

            Assert.IsTrue(Persisting1.PersistingCalled);
        }

        [Test]
        public void PersistingDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Persisting1>();
            try
            {
                obj1.GetAction("Persisting");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Persisting'", e.Message);
            }
        }

        #endregion

        #region Title & ToString

        [Test]
        public virtual void ObjectWithSimpleToString()
        {
            var obj = NewTestObject<Title1>();
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Bar");
            obj.AssertTitleEquals("Bar");
            obj.Save();
            obj.AssertTitleEquals("Bar");
        }

        [Test]
        public virtual void TitleMethod()
        {
            var obj = NewTestObject<Title3>();
            obj.AssertTitleEquals("Untitled Title3");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        [Test]
        public virtual void TitleMethodTakesPrecedenceOverToString()
        {
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
        public virtual void ToStringRecognisedAsATitle()
        {
            var obj = NewTestObject<Title5>();
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Bar");
            obj.AssertTitleEquals("Bar");
            obj.Save();
            obj.AssertTitleEquals("Bar");
        }

        [Test]
        public virtual void UsingITitleBuilderZeroParamConstructor()
        {
            var obj = NewTestObject<Title6>();
            obj.AssertTitleEquals("TB6");
        }

        [Test]
        public virtual void UsingITitleBuilderStringConstructor()
        {
            var obj = NewTestObject<Title7>();
            obj.AssertTitleEquals("TB7");
        }

        [Test]
        public virtual void UsingITitleBuilderObjectConstructor()
        {
            var obj = NewTestObject<Title8>();
            obj.AssertTitleEquals("TB8");
        }

        [Test]
        public virtual void UsingITitleBuilderObjectConstructorWithNullAndDefault()
        {
            var obj = NewTestObject<Title9>();
            obj.AssertTitleEquals("TB9");
        }

        [Test]
        public virtual void ITitleBuilderTestAllAppendsAndConcats()
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var obj = NewTestObject<Title10>();
                obj.AssertTitleEquals("x& y y t1 t2 t3$ t1ct1*t1: no dateno date 04/02/2007 Female Not Specified");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [Test]
        public virtual void TitleMethodMarkedIgnoredIsNotCalled()
        {
            var obj = NewTestObject<Title11>();
            obj.AssertTitleEquals("Untitled Title 11");
        }

        #endregion

        #region Updated

        [Test]
        public void LowerCaseNotRecognisedAndShowsAsAction()
        {
            var obj1 = NewTestObject<Updated2>();
            var dom1 = (Updated2)obj1.GetDomainObject();
            Assert.IsFalse(Updated2.UpdatedCalled);
            obj1.GetAction("Updated");
        }

        [Test]
        [Ignore("investigate")]
        public void UpdatedCalled()
        {
            var obj1 = NewTestObject<Updated1>();
            var dom1 = (Updated1)obj1.GetDomainObject();
            obj1.Save();
            try
            {
                obj1.GetPropertyByName("Prop1").SetValue("Foo");
                Assert.Fail("Shouldn't get to here");
            }
            catch (Exception) { }
        }

        [Test]
        public void UpdatedDoesNotShowUpAsAnAction()
        {
            var obj1 = NewTestObject<Updated1>();
            try
            {
                obj1.GetAction("Updated");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Updated'", e.Message);
            }
        }

        #endregion

        #region Updating

        [Test]
        public void LowerCaseUpdatingNotRecognisedAndShowsAsAction()
        {
            ITestObject obj1 = NewTestObject<Updating2>();
            obj1.GetAction("Updating");
        }

        [Test]
        [Ignore("investigate")]
        public void UpdatingCalled()
        {
            ITestObject obj1 = NewTestObject<Updating1>();
            var dom1 = (Updating1)obj1.GetDomainObject();
            obj1.Save();

            try
            {
                obj1.GetPropertyByName("Prop1").SetValue("Foo");
                Assert.Fail("Should not get to here");
            }
            catch (Exception) { }
        }

        [Test]
        public void UpdatingDoesNotShowUpAsAnAction()
        {
            ITestObject obj1 = NewTestObject<Updating1>();
            try
            {
                obj1.GetAction("Updating");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Updating'", e.Message);
            }
        }

        #endregion

        #region Validate

        [Test]
        public void UnmatchedValidateMethodShowsUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Validate3>();
            obj.GetAction("Validate Prop1");
            obj.GetAction("Validate Prop2");
            obj.GetAction("Validate Prop3");
            obj.GetAction("Validate Do Something");
            obj.GetAction("Validate 0 Do Something");
            obj.GetAction("Validate 1 Do Something");
        }

        [Test]
        public void ValidateMethodDoesNotShowUpAsAnAction()
        {
            ITestObject obj = NewTestObject<Validate1>();
            try
            {
                obj.GetAction("Validate Prop1");
                Assert.Fail("'Validate Prop1' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Prop1'", e.Message);
            }

            try
            {
                obj.GetAction("Validate0 Do Something");
                Assert.Fail("'Validate0 Do Something' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate0 Do Something'", e.Message);
            }

            try
            {
                obj.GetAction("Validate Do Something Else");
                Assert.Fail("'Validate Do Something Else' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Do Something Else'", e.Message);
            }
        }

        [Test]
        public virtual void ValidateNumericalProperty()
        {
            ITestObject obj = NewTestObject<Validate1>();
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertFieldEntryInvalid("2").AssertLastMessageIs("Value must be between 3 & 10");
            prop1.AssertFieldEntryInvalid("11").AssertLastMessageIs("Value must be between 3 & 10");
            prop1.SetValue("6").AssertLastMessageIs("");
            try
            {
                prop1.SetValue("11");
                Assert.Fail();
            }
            catch (Exception)
            {
                prop1.SetValue("7");
            }
        }

        [Test]
        public virtual void ValidatePropertyMarkedNakedObjectsIgnoreIsNotEffectiveAndDoesNotShowAsAction()
        {
            ITestObject obj = NewTestObject<Validate1>();
            ITestProperty prop1 = obj.GetPropertyByName("Prop4");
            prop1.AssertFieldEntryIsValid("2");
            prop1.AssertFieldEntryIsValid("11");

            try
            {
                obj.GetAction("Validate Prop4");
                Assert.Fail("'Validate Prop4' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Prop4'", e.Message);
            }
        }

        [Test]
        public virtual void ValidateStringProperty()
        {
            ITestObject obj = NewTestObject<Validate1>();
            ITestProperty prop1 = obj.GetPropertyByName("Prop2");
            prop1.AssertFieldEntryInvalid("foo").AssertLastMessageIs("Value must start with a");
            prop1.AssertFieldEntryInvalid("bar").AssertLastMessageIs("Value must start with a");
            prop1.SetValue("afoo").AssertLastMessageIs("");
            try
            {
                prop1.SetValue("bar");
                Assert.Fail();
            }
            catch (Exception)
            {
                prop1.SetValue("abar");
            }
        }

        [Test]
        public virtual void ValidateReferenceProperty()
        {
            ITestObject obj1 = NewTestObject<Validate1>();
            ITestProperty obj1Prop3 = obj1.GetPropertyByName("Prop3");

            ITestObject obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            ITestObject obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            obj1Prop3.AssertSetObjectInvalid(obj2b).AssertLastMessageIs("Invalid Object");
            obj1Prop3.AssertSetObjectIsValid(obj2a).AssertLastMessageIs("");

            try
            {
                obj1Prop3.SetObject(obj2b);
                Assert.Fail();
            }
            catch (Exception)
            {
                obj1Prop3.SetObject(obj2a);
            }
        }

        [Test]
        public void ValidateParametersIndividually()
        {
            ITestObject obj1 = NewTestObject<Validate1>();
            ITestAction action = obj1.GetAction("Do Something");

            ITestObject obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            ITestObject obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            action.InvokeReturnObject(new object[] { 5, "abar", obj2a });

            action.AssertIsInvalidWithParms(new object[] { 2, "abar", obj2a }).AssertLastMessageIs("Value must be between 3 & 10");
            action.AssertIsInvalidWithParms(new object[] { 5, "bar", obj2a }).AssertLastMessageIs("Value must start with a");
            action.AssertIsInvalidWithParms(new object[] { 5, "abar", obj2b }).AssertLastMessageIs("Invalid Object");
        }

        [Test]
        public void ValidateParameterMarkedIgnoreIsNotUsedAndDoesNotShowAsAction()
        {
            ITestObject obj1 = NewTestObject<Validate1>();
            ITestAction action = obj1.GetAction("Do Something More");

            action.AssertIsValidWithParms(2);
            action.AssertIsValidWithParms(11);

            try
            {
                obj1.GetAction("Validate Do Something More");
                Assert.Fail("'Validate Do Something More' is showing as an action");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Validate Do Something More'", e.Message);
            }
        }

        [Test]
        public void ValidateParametersCollectively()
        {
            ITestObject obj1 = NewTestObject<Validate1>();
            ITestAction action = obj1.GetAction("Do Something Else");

            ITestObject obj2a = NewTestObject<Validate2>();
            obj2a.GetPropertyByName("Prop1").SetValue("a");
            ITestObject obj2b = NewTestObject<Validate2>();
            obj2b.GetPropertyByName("Prop1").SetValue("b");

            action.InvokeReturnObject(new object[] { 5, "abar", obj2a });

            action.AssertIsInvalidWithParms(new object[] { 2, "abar", obj2a }).AssertLastMessageIs("Something amiss");
            action.AssertIsInvalidWithParms(new object[] { 5, "bar", obj2a }).AssertLastMessageIs("Something amiss");
            action.AssertIsInvalidWithParms(new object[] { 5, "abar", obj2b }).AssertLastMessageIs("Something amiss");
        }

        [Test]
        public virtual void ValidateCrossValidationFail4()
        {
            ITestObject obj = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value2");

            try
            {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. Expect prop1 == prop2", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationSuccess4()
        {
            ITestObject obj = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.Save();
        }

        [Test]
        public virtual void ValidateCrossValidationFail5A()
        {
            ITestObject obj = NewTestObject<Validate5>();
            ITestObject obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value2");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            try
            {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationFail5B()
        {
            ITestObject obj = NewTestObject<Validate5>();
            ITestObject obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("0");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            try
            {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationFail5C()
        {
            ITestObject obj = NewTestObject<Validate5>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").ClearObject();

            try
            {
                obj.Save();
                Assert.Fail("Expect exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. Condition Fail", e.Message);
            }
        }

        [Test]
        public virtual void ValidateCrossValidationSuccess5()
        {
            ITestObject obj = NewTestObject<Validate5>();
            ITestObject obj4 = NewTestObject<Validate4>();
            obj.GetPropertyByName("Prop1").SetValue("value1");
            obj.GetPropertyByName("Prop2").SetValue("value1");
            obj.GetPropertyByName("Prop3").SetValue("1");
            obj.GetPropertyByName("Prop4").SetObject(obj4);

            obj.Save();
        }

        //Test added because > 6 params relies on reflection rather than a delegate
        [Test]
        public virtual void ValidateActionWithMoreThanSixParams()
        {
            ITestObject obj = NewTestObject<Validate1>();
            ITestAction action = obj.GetAction("Do Something With Many Params");
            Assert.AreEqual(7, action.Parameters.Count());

            action.AssertIsInvalidWithParms(new object[] { "y", "x", "x", "x", "x", "x", "x" });
            action.AssertIsValidWithParms(new object[] { "x", "x", "x", "x", "x", "x", "x" });
            action.Invoke(new object[] { "x", "x", "x", "x", "x", "x", "x" });
        }

        #endregion
    }

    #region Classes used in test

    public class MethodsDbContext : DbContext
    {

        private static readonly string Cs = @$"Data Source={NakedObjects.SystemTest.Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

        public static void Delete() => System.Data.Entity.Database.Delete(Cs);

        public const string DatabaseName = "Tests";
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
    }

    #region AutoComplete

    public class Auto1
    {
        public virtual int Id { get; set; }

        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Auto2 Prop3 { get; set; }
        public virtual Auto2 Prop4 { get; set; }

        public IList<string> AutoCompleteProp2(string autoCompleteParm)
        {
            return new List<string> { "Fee", "Foo", "Fuu" };
        }

        public IQueryable<Auto2> AutoCompleteProp3(string autoCompleteParm)
        {
            return Container.Instances<Auto2>().Where(a => a.Prop1.ToUpper().Contains(autoCompleteParm.ToUpper()));
        }

        #region Do Something

        public void DoSomething(Auto1 param0, string param1, Auto2 param2) { }

        public Auto1 AutoComplete0DoSomething(string autoCompleteParm)
        {
            return this;
        }

        public IList<string> AutoComplete1DoSomething(string autoCompleteParm)
        {
            return AutoCompleteProp2(autoCompleteParm);
        }

        public IQueryable<Auto2> AutoComplete2DoSomething(string autoCompleteParm)
        {
            return Container.Instances<Auto2>().Take(2);
        }

        #endregion
    }

    public class Auto2
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Auto3
    {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Auto2 Prop4 { get; set; }

        //Prop1 not a valid type for auto-complete
        public IQueryable<int> AutoCompleteProp1()
        {
            return null;
        }

        //Return type does not match
        public IQueryable<Auto2> AutoCompleteProp2(string autoCompleteParm)
        {
            return null;
        }

        //No corresponding property
        public IQueryable<Auto2> AutoCompleteProp3(string autoCompleteParm)
        {
            return null;
        }

        //List of domain object not valid
        public IList<Auto2> AutoCompleteProp4(string autoCompleteParm)
        {
            return null;
        }

        public void DoSomething(int param0, Auto2 param1, Auto2 param2) { }

        //param not a valid type for auto-complete
        public IQueryable<int> AutoComplete0DoSomething(string autoCompleteParm)
        {
            return null;
        }

        //Return type does not match
        public IQueryable<Auto3> AutoComplete1DoSomething(string autoCompleteParm)
        {
            return null;
        }

        //Action name mis-spelled
        public IQueryable<Auto2> AutoComplete2DoSomting(string autoCompleteParm)
        {
            return null;
        }

        //No corresponding param
        public IQueryable<Auto2> AutoComplete3DoSomething(string autoCompleteParm)
        {
            return null;
        }
    }

    #endregion

    #region Choices

    public class Choices1
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public Choices4 Prop3 { get; set; }

        public List<int> ChoicesProp1()
        {
            return new List<int> { 4, 8, 9 };
        }

        public List<string> ChoicesProp2()
        {
            return new List<string> { "Fee", "Foo", "Fuu" };
        }

        public List<Choices4> ChoicesProp3()
        {
            return Container.Instances<Choices4>().ToList();
        }

        #region Do Something

        public void DoSomething(int param0, string param1, Choices2 param2) { }

        public List<int> Choices0DoSomething()
        {
            return ChoicesProp1();
        }

        public List<string> Choices1DoSomething()
        {
            return ChoicesProp2();
        }

        public List<Choices2> Choices2DoSomething()
        {
            return Container.Instances<Choices2>().ToList();
        }

        #endregion
    }

    public class Choices2
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Choices3
    {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public List<string> ChoicesProp1()
        {
            return null;
        }

        public string ChoicesProp2()
        {
            return null;
        }

        public string ChoicesProp3()
        {
            return null;
        }

        public void DoSomething(int param0, string param1, Choices2 param2) { }

        public List<int> Choices0DoSomthing()
        {
            return null;
        }

        public List<string> Choices0DoSomething()
        {
            return null;
        }
    }

    public class Choices4
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Clear

    public class Clear1
    {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Clear3 Prop4 { get; set; }

        public void ClearProp1()
        {
            Prop1 = null;
            Prop0 = "Prop1 has been cleared";
        }

        public void ClearProp4()
        {
            Prop4 = null;
            Prop3 = "Prop4 has been cleared";
        }
    }

    public class Clear2
    {
        public virtual int Id { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        //Has param
        public void ClearProp2(string value) { }

        //Non-void method
        public bool ClearProp3()
        {
            return false;
        }

        //No corresponding Prop4
        public void ClearProp4() { }
    }

    public class Clear3
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Created

    public class Created1
    {
        public bool CreatedCalled;
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void Created()
        {
            CreatedCalled = true;
        }
    }

    public class Created2
    {
        public bool CreatedCalled;
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void created()
        {
            CreatedCalled = true;
        }
    }

    #endregion

    #region Default

    public class Default1
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Default2 Prop3 { get; set; }

        [DefaultValue(7)]
        public virtual int Prop4 { get; set; }

        [DefaultValue("Bar")]
        public virtual string Prop5 { get; set; }

        public int DefaultProp1()
        {
            return 8;
        }

        public string DefaultProp2()
        {
            return "Foo";
        }

        public Default2 DefaultProp3()
        {
            return Container.Instances<Default2>().Where(x => x.Prop1 == "Bar2").FirstOrDefault();
        }

        public int DefaultProp4()
        {
            return 8;
        }

        public string DefaultProp5()
        {
            return "Foo";
        }

        #region Do Something

        public void DoSomething(int param0, string param1, Default4 param2) { }

        public int Default0DoSomething()
        {
            return DefaultProp1();
        }

        public string Default1DoSomething()
        {
            return DefaultProp2();
        }

        public Default4 Default2DoSomething()
        {
            return Container.Instances<Default4>().Where(x => x.Prop1 == "Bar2").FirstOrDefault();
        }

        #endregion

        #region Do Something Else

        public void DoSomethingElse([DefaultValue(7)] int param0,
                                    [DefaultValue("Bar")] string param1)
        { }

        public int Default0DoSomethingElse()
        {
            return DefaultProp1();
        }

        public string Default1DoSomethingElse()
        {
            return DefaultProp2();
        }

        #endregion
    }

    public class Default2
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Default3
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual int DefaultProp1()
        {
            return 0;
        }

        public virtual string DefaultProp2()
        {
            return null;
        }

        public void DoSomething(int param0, string param1, Default2 param2) { }

        public string Default0DoSomthing(int param0)
        {
            return null;
        }

        public string Default0DoSomething(decimal param0)
        {
            return null;
        }
    }

    public class Default4
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    #endregion

    #region Deleted

    public class Deleted1
    {
        public static bool DeletedCalled;
        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Deleted()
        {
            DeletedCalled = true;
        }

        public void Delete()
        {
            Container.DisposeInstance(this);
        }
    }

    public class Deleted2
    {
        public static bool DeletedCalled;

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void Deleted()
        {
            DeletedCalled = true;
        }
    }

    #endregion

    #region Deleting

    public class Deleting1
    {
        public static bool DeletingCalled;

        public Deleting1()
        {
            DeletingCalled = false;
        }

        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        public void Deleting()
        {
            DeletingCalled = true;
        }

        public void Delete()
        {
            Container.DisposeInstance(this);
        }
    }

    public class Deleting2
    {
        public static bool DeletingCalled;

        public Deleting2()
        {
            DeletingCalled = false;
        }

        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        public void deleting()
        {
            DeletingCalled = true;
        }

        public void Delete()
        {
            Container.DisposeInstance(this);
        }
    }

    #endregion

    #region Disable

    public class Disable1
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public Disable1 Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual ICollection<Disable1> Prop4 { get; set; } = new List<Disable1>();

        public virtual ICollection<Disable1> Prop5 { get; set; } = new List<Disable1>();

        public string DisablePropertyDefault()
        {
            return "This property has been disabled by default";
        }

        public string DisableProp3()
        {
            return null;
        }

        public string DisableProp5()
        {
            return null;
        }

        public void Action1() { }

        public void Action2(string param) { }
    }

    public class Disable2
    {
        public virtual int Id { get; set; }
        public virtual string Prop1 { get; set; }

        public virtual Disable1 Prop2 { get; set; }

        public virtual ICollection<Disable1> Prop4 { get; set; } = new List<Disable1>();

        public string DisableActionDefault()
        {
            return "This property has been disabled by default";
        }

        public void Action1() { }

        public void Action2(string param) { }

        public void Action3() { }

        public string DisableAction3()
        {
            return null;
        }
    }

    public class Disable3
    {
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

        public string DisableProp6()
        {
            if (Prop4 == "Disable 6")
            {
                return "Disabled Message";
            }
            return null;
        }

        public string DisableProp1()
        {
            return null;
        }

        public bool DisableProp4()
        {
            return false;
        }

        public void Action1() { }

        //OK
        public string DisableAction1()
        {
            return DisableProp6();
        }

        public void Action2(string parm1) { }

        //Disable should not take any parms  -  even matching ones
        public string DisableAction2(string parm1)
        {
            return "x";
        }

        public void Action3() { }

        //Disable should not take any parms  -  even matching ones
        public string DisableAction3(int parm1)
        {
            return "x";
        }

        //Disable should not take any parms  -  even matching ones
        public string DisableProp7(string parm1)
        {
            return "x";
        }

        //Disable should not take any parms  -  even matching ones
        public string DisableProp8(int parm1)
        {
            return "x";
        }

        public void Action4() { }

        public string DisableAction4()
        {
            return "disabled";
        }
    }

    #endregion

    #region Hide

    public class Hide1
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual Hide1 Prop2 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual ICollection<Hide1> Prop4 { get; set; } = new List<Hide1>();

        public virtual ICollection<Hide1> Prop5 { get; set; } = new List<Hide1>();

        public bool HidePropertyDefault()
        {
            return true;
        }

        public bool HideProp3()
        {
            return false;
        }

        public bool HideProp5()
        {
            return false;
        }

        public void Action1() { }

        public void Action2(string param) { }
    }

    public class Hide2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual Hide1 Prop2 { get; set; }

        public virtual ICollection<Hide1> Prop4 { get; set; } = new List<Hide1>();

        public bool HideActionDefault()
        {
            return true;
        }

        public void Action1() { }

        public void Action2(string param) { }

        public void Action3() { }

        public bool HideAction3()
        {
            return false;
        }
    }

    public class Hide3
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop4 { get; set; }

        [Optionally]
        public virtual string Prop6 { get; set; }

        [Optionally]
        public virtual string Prop8 { get; set; }

        [Optionally]
        public virtual string Prop9 { get; set; }

        public bool HideProp6()
        {
            return Prop4 == "Hide 6";
        }

        public void DoSomething() { }

        public bool HideDoSomething()
        {
            return HideProp6();
        }

        public void DoSomethingElse() { }

        public bool HideProp7()
        {
            return false;
        }

        public string HideProp4()
        {
            return null;
        }

        public bool HideDoSomthingElse()
        {
            return false;
        }

        public string HideDoSomethingElse()
        {
            return null;
        }

        public void Action1() { }

        public bool HideAction1(string parm1)
        {
            return true;
        }

        public void Action2(string parm1) { }

        public bool HideAction2(string parm1)
        {
            return true;
        }

        public bool HideProp8(int prop8)
        {
            return true;
        }

        public bool HideProp9(string prop9)
        {
            return true;
        }
    }

    #endregion

    #region Modify

    public class Modify1
    {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Modify3 Prop4 { get; set; }

        public void ModifyProp1(string value)
        {
            Prop1 = value;
            Prop0 = "Prop1 has been modified";
        }

        public void ModifyProp4(Modify3 value)
        {
            Prop4 = value;
            Prop3 = "Prop4 has been modified";
        }
    }

    public class Modify2
    {
        public virtual int Id { get; set; }

        public virtual string Prop2 { get; set; }

        //Has the wrong type of parameter

        public virtual string Prop3 { get; set; }
        public void ModifyProp2(int value) { }

        //Non-void method
        public bool ModifyProp3(string value)
        {
            return false;
        }

        //No corresponding Prop4
        public void ModifyProp4(string value) { }
    }

    public class Modify3
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }
    }

    public class Modify4
    {
        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual string Prop3 { get; set; }

        public virtual Modify3 Prop4 { get; set; }

        public void ModifyProp1(string value)
        {
            Prop1 = value;
            Prop0 = "Prop1 has been modified";
        }

        public void ModifyProp4(Modify3 value)
        {
            Prop4 = value;
            Prop3 = "Prop4 has been modified";
        }

        public void ClearProp1()
        {
            Prop1 = null;
        }

        public void ClearProp4()
        {
            Prop4 = null;
        }
    }

    #endregion

    #region Persisted

    public class Persisted1
    {
        public virtual int Id { get; set; }

        public void Persisted()
        {
            throw new DomainException("Persisted called");
        }
    }

    public class Persisted2
    {
        public bool PersistedCalled;

        public virtual int Id { get; set; }

        public void persisted()
        {
            throw new DomainException("persisted called");
        }
    }

    public class Persisted3
    {
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public void Persisted()
        {
            throw new DomainException("Persisted");
        }
    }

    #endregion

    #region Persisting

    public class Persisting1
    {
        public static bool PersistingCalled;

        public Persisting1()
        {
            PersistingCalled = false;
        }

        public virtual int Id { get; set; }

        public void Persisting()
        {
            PersistingCalled = true;
        }
    }

    public class Persisting2
    {
        public static bool PersistingCalled;

        public virtual int Id { get; set; }

        public void peristing()
        {
            PersistingCalled = true;
        }
    }

    #endregion

    #region Title & ToString

    public class Title1
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString()
        {
            return Prop1;
        }
    }

    public class Title2
    {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public override string ToString()
        {
            return "Bar";
        }
    }

    public class Title3
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public string Title()
        {
            return Prop1;
        }
    }

    public class Title4
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title()
        {
            return Prop1;
        }

        public override string ToString()
        {
            return "Bar";
        }
    }

    public class Title5
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString()
        {
            return Prop1;
        }
    }

    public class Title6
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString()
        {
            var tb = Container.NewTitleBuilder();
            tb.Append("TB6");
            return tb.ToString();
        }
    }

    public class Title7
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString()
        {
            var tb = Container.NewTitleBuilder("TB7");
            return tb.ToString();
        }
    }

    public class Title8
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString()
        {
            var t1 = Container.NewTransientInstance<Title1>();
            t1.Prop1 = "TB8";
            Container.Persist(ref t1);
            var tb = Container.NewTitleBuilder(t1);
            return tb.ToString();
        }
    }

    public class Title9
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString()
        {
            var tb = Container.NewTitleBuilder(null, "TB9");
            return tb.ToString();
        }
    }

    public class Title10
    {
        public IDomainObjectContainer Container { set; protected get; }

        public virtual int Id { get; set; }

        public override string ToString()
        {
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
            tb.Concat(":", null, "d", "no date"); //"x& y y t1 t2 t3$ t1ct1*t1: no dateno date"

            tb.Append(new DateTime(2007, 4, 2), "d", null); //"x& y y t1 t2 t3$ t1ct1*t1: no dateno date 02/04/2007"
            tb.Append(Sex.Female); //"x& y y t1 t2 t3$ t1ct1*t1: no dateno date 02/04/2007 Female"
            tb.Append(Sex.NotSpecified); //"x& y y t1 t2 t3$ t1ct1*t1: no dateno date 02/04/2007 Female Not Specified"
            return tb.ToString();
        }
    }

    public class Title11
    {
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public string Title()
        {
            throw new Exception("Title method should not have been called");
        }
    }

    public enum Sex
    {
        Male = 1,
        Female = 2,
        Unknown = 3,
        NotSpecified = 4
    }

    #endregion

    #region Updated

    public class Updated1
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Updated()
        {
            throw new DomainException("Update called");
        }
    }

    public class Updated2
    {
        public static bool UpdatedCalled;

        public Updated2()
        {
            UpdatedCalled = false;
        }

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void updated()
        {
            UpdatedCalled = true;
        }
    }

    #endregion

    #region Updating

    public class Updating1
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Updating()
        {
            throw new DomainException("Updating called");
        }
    }

    public class Updating2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public void updating()
        {
            throw new DomainException("Updating called");
        }
    }

    #endregion

    #region Validate

    public class Validate1
    {
        public virtual int Id { get; set; }

        public virtual int Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual Validate2 Prop3 { get; set; }

        public virtual int Prop4 { get; set; }

        public string ValidateProp1(int value)
        {
            if (value < 3 || value > 10)
            {
                return "Value must be between 3 & 10";
            }
            return null;
        }

        public string ValidateProp2(string value)
        {
            if (!value.StartsWith("a"))
            {
                return "Value must start with a";
            }
            return null;
        }

        public string ValidateProp3(Validate2 value)
        {
            if (!value.Prop1.StartsWith("a"))
            {
                return "Invalid Object";
            }
            return null;
        }

        [NakedObjectsIgnore]
        public string ValidateProp4(int value)
        {
            if (value < 3 || value > 10)
            {
                return "Value must be between 3 & 10";
            }
            return null;
        }

        public void DoSomethingMore(int param0) { }

        [NakedObjectsIgnore]
        public string Validate0DoSomethingMore(int value)
        {
            return ValidateProp1(value);
        }

        #region Do Something

        public void DoSomething(int param0, string param1, Validate2 param2) { }

        public string Validate0DoSomething(int value)
        {
            return ValidateProp1(value);
        }

        public string Validate1DoSomething(string value)
        {
            return ValidateProp2(value);
        }

        public string Validate2DoSomething(Validate2 value)
        {
            return ValidateProp3(value);
        }

        #endregion

        #region Do Something Else

        public void DoSomethingElse(int param0, string param1, Validate2 param2) { }

        public string ValidateDoSomethingElse(int param0, string param1, Validate2 param2)
        {
            if (ValidateProp1(param0) != null || ValidateProp2(param1) != null || ValidateProp3(param2) != null)
            {
                return "Something amiss";
            }
            return null;
        }

        #endregion

        #region DoSomethingWithManyParams

        public void DoSomethingWithManyParams(string param0, string param1, string param2, string param3, string param4, string param5, string param6) { }

        public string ValidateDoSomethingWithManyParams(string param0, string param1, string param2, string param3, string param4, string param5, string param6)
        {
            return param0 != "x" ? "Invalid" : null;
        }

        #endregion
    }

    public class Validate2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Validate3
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public string ValidateProp1(int value)
        {
            return null;
        }

        public bool ValidateProp2(string value)
        {
            return false;
        }

        public string ValidateProp3(string value)
        {
            return null;
        }

        public void DoSomething(int par1) { }

        public string ValidateDoSomething(decimal par1)
        {
            return null;
        }

        public string Validate0DoSomething(bool par1)
        {
            return null;
        }

        public string Validate1DoSomething(int par1)
        {
            return null;
        }
    }

    public class Validate4
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public string Validate(string prop1, string prop2)
        {
            if (prop1 != prop2)
            {
                return "Expect prop1 == prop2";
            }
            return null;
        }
    }

    public class Validate5
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public virtual string Prop2 { get; set; }

        public virtual int Prop3 { get; set; }

        [Optionally]
        public virtual Validate4 Prop4 { get; set; }

        public string Validate(Validate4 prop4, string prop1, int prop3, string prop2)
        {
            if (prop1 != prop2 || prop3 == 0 || prop4 == null)
            {
                return "Condition Fail";
            }
            return null;
        }
    }

    #endregion

    #endregion
}