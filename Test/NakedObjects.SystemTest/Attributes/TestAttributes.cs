// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using SystemTest.Attributes;
using AdventureWorksModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Architecture.Spec;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Attributes {
    [TestClass]
    public class TestAttributes : AbstractSystemTest<AttributesDbContext> {
        protected override string[] Namespaces {
            get { return new[] {"NakedObjects.SystemTest.Attributes", "SystemTest.Attributes"}; }
        }

        protected override Type[] Services {
            get {
                return new[] {
                    typeof (SimpleRepository<Default1>),
                    typeof (SimpleRepository<Describedas1>),
                    typeof (SimpleRepository<Describedas2>),
                    typeof (SimpleRepository<Description1>),
                    typeof (SimpleRepository<Description2>),
                    typeof (SimpleRepository<Disabled1>),
                    typeof (SimpleRepository<Displayname1>),
                    typeof (SimpleRepository<Hidden1>),
                    typeof (SimpleRepository<Iconname1>),
                    typeof (SimpleRepository<Iconname2>),
                    typeof (SimpleRepository<Iconname3>),
                    typeof (SimpleRepository<Iconname4>),
                    typeof (SimpleRepository<Immutable1>),
                    typeof (SimpleRepository<Immutable2>),
                    typeof (SimpleRepository<Immutable3>),
                    typeof (SimpleRepository<Mask1>),
                    typeof (SimpleRepository<Mask2>),
                    typeof (SimpleRepository<Maxlength1>),
                    typeof (SimpleRepository<Maxlength2>),
                    typeof (SimpleRepository<NakedObjectsIgnore1>),
                    typeof (SimpleRepository<NakedObjectsIgnore2>), //But this one won't be visible
                    typeof (SimpleRepository<NakedObjectsIgnore3>),
                    typeof (SimpleRepository<NakedObjectsIgnore4>),
                    typeof (SimpleRepository<NakedObjectsIgnore5>),
                    typeof (SimpleRepository<NakedObjectsIgnore6>),
                    typeof (SimpleRepository<NakedObjectsIgnore7>),
                    typeof (SimpleRepository<Named1>),
                    typeof (SimpleRepository<Range1>),
                    typeof (SimpleRepository<Regex1>),
                    typeof (SimpleRepository<Regex2>),
                    typeof (SimpleRepository<Memberorder1>),
                    typeof (SimpleRepository<Memberorder2>),
                    typeof (SimpleRepository<Stringlength1>),
                    typeof (SimpleRepository<Title1>),
                    typeof (SimpleRepository<Title2>),
                    typeof (SimpleRepository<Title3>),
                    typeof (SimpleRepository<Title4>),
                    typeof (SimpleRepository<Title5>),
                    typeof (SimpleRepository<Title6>),
                    typeof (SimpleRepository<Title7>),
                    typeof (SimpleRepository<Title8>),
                    typeof (SimpleRepository<Title9>),
                    typeof (SimpleRepository<Validateprogrammaticupdates1>),
                    typeof (SimpleRepository<Validateprogrammaticupdates2>),
                    typeof (TestServiceValidateProgrammaticUpdates),
                    typeof (SimpleRepository<Contributee>),
                    typeof (SimpleRepository<Contributee2>),
                    typeof (SimpleRepository<Contributee3>),
                    typeof (TestServiceContributedAction),
                    typeof (SimpleRepository<FinderAction1>),
                    typeof (TestServiceFinderAction)
                };
            }
        }

        #region FinderAction

        [TestMethod]
        public virtual void ActionsIncludedInFinderMenu() {
            var service = (TestServiceFinderAction) GetTestService(typeof (TestServiceFinderAction)).NakedObject.Object;
            FinderAction1 obj = service.NewObject1();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(obj, null, null);
            var finderActions = ((IObjectSpec) adapter.Spec).GetFinderActions();

            Assert.AreEqual(3, finderActions.Count());
            Assert.AreEqual("Finder Action1", finderActions[0].Name);
            Assert.AreEqual("Finder Action2", finderActions[1].Name);
            Assert.AreEqual("Finder Action3", finderActions[2].Name);
        }

        #endregion

        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestAttributes());
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc) {
            Database.Delete(AttributesDbContext.DatabaseName);
            var context = Activator.CreateInstance<AttributesDbContext>();

            context.Database.Create();
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        #region ContributedAction

        [TestMethod]
        public virtual void Contributed() {
            var obj = NewTestObject<Contributee>().GetDomainObject();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(obj, null, null);
            var actions = adapter.Spec.GetActions();

            Assert.AreEqual(1, actions.Count());
            Assert.IsTrue(actions[0] is IActionSpec);
            Assert.AreEqual("Contributed Action", actions[0].Name);

            //Test that the actions show up on the TestObject as test actions
            var testActions = NewTestObject<Contributee>().Actions;
            Assert.AreEqual(1, testActions.Count());
            Assert.AreEqual("Contributed Action", testActions[0].Name);
        }

        [TestMethod]
        public virtual void CollectionContributed() {
            var testObj = NewTestObject<Contributee2>();
            var obj = testObj.GetDomainObject();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(obj, null, null);
            var actions = (adapter.Spec as IObjectSpec).GetCollectionContributedActions();
            var testActions = testObj.Actions;

            Assert.AreEqual(3, actions.Count());
            Assert.IsTrue(actions[0] is IActionSpec);
            Assert.AreEqual("Collection Contributed Action", actions[0].Name);
            Assert.AreEqual("Collection Contributed Action1", actions[1].Name);
            Assert.AreEqual("Collection Contributed Action2", actions[2].Name);
        }

        [TestMethod]
        public virtual void CollectionContributedToSubClass() {
            var obj = NewTestObject<Contributee3>().GetDomainObject();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(obj, null, null);
            var actions = (adapter.Spec as IObjectSpec).GetCollectionContributedActions();

            Assert.AreEqual(3, actions.Count());
            Assert.IsTrue(actions[0] is IActionSpec);
            Assert.AreEqual("Collection Contributed Action", actions[0].Name);
            Assert.AreEqual("Collection Contributed Action1", actions[1].Name);
            Assert.AreEqual("Collection Contributed Action2", actions[2].Name);
        }

        [TestMethod]
        public virtual void CollectionContributedNotToAnotherClass() {
            var obj = NewTestObject<Contributee>().GetDomainObject();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(obj, null, null);
            var actions = (adapter.Spec as IObjectSpec).GetCollectionContributedActions();

            Assert.AreEqual(0, actions.Count());
        }

        #endregion

        #region Default

        [TestMethod]
        public virtual void DefaultNumericProperty() {
            var default1 = NewTestObject<Default1>();
            ITestProperty prop = default1.GetPropertyByName("Prop1");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [TestMethod]
        public void DefaultParameters() {
            var default1 = NewTestObject<Default1>();
            ITestAction action = default1.GetAction("Do Something");
            string def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            string def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);
        }

        [TestMethod]
        public virtual void DefaultStringProperty() {
            var default1 = NewTestObject<Default1>();
            ITestProperty prop = default1.GetPropertyByName("Prop2");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }

        #endregion

        #region DescribedAs

        [TestMethod]
        public virtual void DescribedAsAppliedToAction() {
            var describedAs1 = NewTestObject<Describedas1>();
            ITestAction action = describedAs1.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("Hex");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToObject() {
            var describedAs1 = NewTestObject<Describedas1>();
            describedAs1.AssertIsDescribedAs("Foo");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToParameter() {
            var describedAs1 = NewTestObject<Describedas1>();
            ITestAction action = describedAs1.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("Yop");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToProperty() {
            var describedAs1 = NewTestObject<Describedas1>();
            ITestProperty prop = describedAs1.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("Bar");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToAction() {
            var describedAs2 = NewTestObject<Describedas2>();
            ITestAction action = describedAs2.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToObject() {
            var describedAs2 = NewTestObject<Describedas2>();
            describedAs2.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToParameter() {
            var describedAs2 = NewTestObject<Describedas2>();
            ITestAction action = describedAs2.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToProperty() {
            var describedAs2 = NewTestObject<Describedas2>();
            ITestProperty prop = describedAs2.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("");
        }

        #endregion

        #region Description

        [TestMethod]
        public virtual void DescriptionAppliedToAction() {
            var description1 = NewTestObject<Description1>();
            ITestAction action = description1.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("Hex");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToObject() {
            var description1 = NewTestObject<Description1>();
            description1.AssertIsDescribedAs("Foo");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToParameter() {
            var description1 = NewTestObject<Description1>();
            ITestAction action = description1.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("Yop");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToProperty() {
            var description1 = NewTestObject<Description1>();
            ITestProperty prop = description1.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("Bar");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToAction() {
            var description2 = NewTestObject<Description2>();
            ITestAction action = description2.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToObject() {
            var description2 = NewTestObject<Description2>();
            description2.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToParameter() {
            var description2 = NewTestObject<Description2>();
            ITestAction action = description2.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToProperty() {
            var description2 = NewTestObject<Description2>();
            ITestProperty prop = description2.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("");
        }

        #endregion

        #region Disabled

        private ITestObject NewTransientDisabled1() {
            return GetTestService("Disabled1s").GetAction("New Instance").InvokeReturnObject();
        }

        [TestMethod]
        public virtual void Disabled() {
            var obj = NewTransientDisabled1();
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertIsUnmodifiable();
            obj.Save();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledAlways() {
            var obj = NewTransientDisabled1();
            ITestProperty prop5 = obj.GetPropertyByName("Prop5");
            prop5.AssertIsUnmodifiable();
            obj.Save();
            prop5.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledNever() {
            var obj = NewTransientDisabled1();
            ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.AssertIsModifiable();
            obj.Save();
            prop4.AssertIsModifiable();
        }

        [TestMethod]
        public virtual void DisabledOncePersisted() {
            var obj = NewTransientDisabled1();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertIsModifiable();
            obj.Save();
            prop2.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledUntilPersisted() {
            var obj = NewTransientDisabled1();
            ITestProperty prop3 = obj.GetPropertyByName("Prop3");
            prop3.AssertIsUnmodifiable();
            obj.Save();
            prop3.AssertIsModifiable();
        }

        #endregion

        #region DisplayName

        [TestMethod]
        public virtual void DisplayNameAppliedToAction() {
            var displayname1 = NewTestObject<Displayname1>();
            ITestAction hex = displayname1.GetAction("Hex");
            Assert.IsNotNull(hex);
            Equals("Hex", hex.Name);
        }

        [TestMethod]
        public virtual void DisplayNameAppliedToObject() {
            var displayname1 = NewTestObject<Displayname1>();
            displayname1.AssertTitleEquals("Untitled Foo");
        }

        #endregion

        #region Hidden

        [TestMethod]
        public virtual void Hidden() {
            var hidden1 = NewTestObject<Hidden1>();
            ITestProperty prop1 = hidden1.GetPropertyByName("Prop1");
            prop1.AssertIsInvisible();

            hidden1.Save();
            prop1.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenAlways() {
            var hidden1 = NewTestObject<Hidden1>();
            ITestProperty prop5 = hidden1.GetPropertyByName("Prop5");
            prop5.AssertIsInvisible();
            hidden1.Save();
            prop5.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenNever() {
            var hidden1 = NewTestObject<Hidden1>();
            ITestProperty prop4 = hidden1.GetPropertyByName("Prop4");
            prop4.AssertIsVisible();
            hidden1.Save();
            prop4.AssertIsVisible();
        }

        [TestMethod]
        public virtual void HiddenOncePersisted() {
            var hidden1 = NewTestObject<Hidden1>();
            ITestProperty prop2 = hidden1.GetPropertyByName("Prop2");
            prop2.AssertIsVisible();
            hidden1.Save();
            prop2.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenUntilPersisted() {
            var hidden1 = NewTestObject<Hidden1>();
            ITestProperty prop3 = hidden1.GetPropertyByName("Prop3");
            prop3.AssertIsInvisible();
            hidden1.Save();
            prop3.AssertIsVisible();
        }

        #endregion

        #region Immutable

        [TestMethod]
        public virtual void ObjectImmutable() {
            ITestObject obj = NewTestObject<Immutable2>();
            ITestProperty prop0 = obj.GetPropertyByName("Prop0");
            prop0.AssertIsUnmodifiable();
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertIsUnmodifiable();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertIsUnmodifiable();
            ITestProperty prop3 = obj.GetPropertyByName("Prop3");
            prop3.AssertIsUnmodifiable();
            ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.AssertIsUnmodifiable();
            ITestProperty prop5 = obj.GetPropertyByName("Prop5");
            prop5.AssertIsUnmodifiable();
            ITestProperty prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsUnmodifiable();
            obj.Save();
            prop0.AssertIsUnmodifiable();
            prop1.AssertIsUnmodifiable();
            prop2.AssertIsUnmodifiable();
            prop3.AssertIsUnmodifiable();
            prop4.AssertIsUnmodifiable();
            prop5.AssertIsUnmodifiable();
            prop6.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void TestObjectImmutableOncePersisted() {
            ITestObject obj = NewTestObject<Immutable3>();
            ITestProperty prop0 = obj.GetPropertyByName("Prop0");
            prop0.AssertIsModifiable();
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertIsUnmodifiable();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertIsModifiable();
            ITestProperty prop3 = obj.GetPropertyByName("Prop3");
            prop3.AssertIsUnmodifiable();
            ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.AssertIsModifiable();
            ITestProperty prop5 = obj.GetPropertyByName("Prop5");
            prop5.AssertIsUnmodifiable();
            ITestProperty prop6 = obj.GetPropertyByName("Prop6");
            prop6.AssertIsModifiable();

            obj.Save();

            prop0.AssertIsUnmodifiable();
            prop1.AssertIsUnmodifiable();
            prop2.AssertIsUnmodifiable();
            prop3.AssertIsUnmodifiable();
            prop4.AssertIsUnmodifiable();
            prop5.AssertIsUnmodifiable();
            prop6.AssertIsUnmodifiable();
        }

        #endregion

        #region Mask

        [TestMethod]
        public virtual void CMaskOnDecimalProperty() {
            var culture = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var mask1 = NewTestObject<Mask2>();
                var prop1 = mask1.GetPropertyByName("Prop1");
                prop1.SetValue("32.70");
                var dom = (Mask2)mask1.GetDomainObject();
                Equals("32.7", dom.Prop1.ToString());
                Equals("32.70", prop1.Content.Title);
                Equals("¤32.70", prop1.Title);
                prop1.AssertTitleIsEqual("¤32.70");
                prop1.AssertValueIsEqual("32.70");
            }
            finally {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [TestMethod]
        public virtual void DMaskOnDateProperty() {
            var mask1 = NewTestObject<Mask1>();
            var prop1 = mask1.GetPropertyByName("Prop1");
            prop1.SetValue("23/09/2009 11:34:50");
            var prop2 = mask1.GetPropertyByName("Prop2");
            prop2.SetValue("23/09/2009 11:34:50");
            var dom = (Mask1) mask1.GetDomainObject();
            Equals("23/09/2009 11:34:50", dom.Prop1.ToString());
            Equals("23/09/2009 11:34:50", prop1.Content.Title);
            Equals("23/09/2009 11:34:50", dom.Prop2.ToString());
            Equals("23/09/2009", prop2.Content.Title);
            prop1.AssertTitleIsEqual("23/09/2009 11:34:50");
            prop1.AssertValueIsEqual("23/09/2009 11:34:50");
            prop2.AssertTitleIsEqual("23/09/2009");
            prop2.AssertValueIsEqual("23/09/2009 11:34:50");
        }

        #endregion

        #region MaxLength

        [TestMethod]
        public virtual void NakedObjectsMaxLengthOnProperty() {
            ITestObject obj = NewTestObject<Maxlength1>();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertFieldEntryInvalid("12345678");
            prop2.AssertFieldEntryInvalid("1234567 ");
            prop2.SetValue("1234567");
        }

        [TestMethod]
        public virtual void NakedObjectsMaxLengthOnParm() {
            ITestObject obj = NewTestObject<Maxlength1>();
            ITestAction act = obj.GetAction("Action");

            act.AssertIsInvalidWithParms("123456789");
            act.AssertIsInvalidWithParms("12345678 ");
            act.AssertIsValidWithParms("12345678");
        }

        [TestMethod]
        public virtual void ComponentModelMaxLengthOnProperty() {
            ITestObject obj = NewTestObject<Maxlength2>();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertFieldEntryInvalid("12345678");
            prop2.AssertFieldEntryInvalid("1234567 ");
            prop2.SetValue("1234567");
        }

        [TestMethod]
        public virtual void ComponentModelMaxLengthOnParm() {
            ITestObject obj = NewTestObject<Maxlength2>();
            ITestAction act = obj.GetAction("Action");

            act.AssertIsInvalidWithParms("123456789");
            act.AssertIsInvalidWithParms("12345678 ");
            act.AssertIsValidWithParms("12345678");
        }

        #endregion

        #region Scope of reflection (NakedObjectsType, NakedObjectsIgnore, NakedObjectsInclude

        [TestMethod]
        public virtual void NakedObjectsIgnore_OnIndividualMembers() {
            var obj = NewTestObject<NakedObjectsIgnore1>();
            //Note: numbers will change to 3 & 1 when NakedObjectsType
            //is re-introduced and commented back in
            Assert.AreEqual(5, obj.Properties.Count());
            Assert.AreEqual(3, obj.Actions.Count());
        }

        [TestMethod, Ignore] //Pending re-introduction of NakedObjectsType attribute
        public virtual void NakedObjectsType_ReflectOver_All() {
            var obj = NewTestObject<NakedObjectsIgnore3>();
            //results should be same as for NakedObjectsIgnore1:
            Assert.AreEqual(3, obj.Properties.Count());
            Assert.AreEqual(1, obj.Actions.Count());
        }

        [TestMethod, Ignore] //Pending re-introduction of NakedObjectsType attribute
        public virtual void NakedObjectsType_ReflectOver_TypeOnly() {
            var obj = NewTestObject<NakedObjectsIgnore4>();
            Assert.AreEqual(0, obj.Actions.Count());
            Assert.AreEqual(0, obj.Properties.Count());
        }

        [TestMethod, Ignore] //Pending re-introduction of NakedObjectsType attribute
        public virtual void NakedObjectsType_ReflectOver_ExplicitlyIncludedMembersOnly() {
            var obj = NewTestObject<NakedObjectsIgnore5>();
            Assert.AreEqual(1, obj.Actions.Count());
            Assert.AreEqual(2, obj.Properties.Count());
        }

        [TestMethod, Ignore] //Pending re-introduction of NakedObjectsType attribute
        public virtual void NakedObjectsType_ReflectOver_TypeOnly_OnSubClass() {
            var obj = NewTestObject<NakedObjectsIgnore6>();
            Assert.AreEqual(0, obj.Actions.Count());
            Assert.AreEqual(0, obj.Properties.Count());
        }

        [TestMethod, Ignore] //Pending re-introduction of NakedObjectsType attribute
        public virtual void NakedObjectsType_ReflectOver_ExplicitlyIncludedMembersOnly_OnSubClass() {
            var obj = NewTestObject<NakedObjectsIgnore7>();
            Assert.AreEqual(2, obj.Actions.Count());
            Assert.AreEqual(3, obj.Properties.Count());
        }

        #endregion

        #region Named

        [TestMethod]
        public virtual void NamedAppliedToAction() {
            var named1 = NewTestObject<Named1>();
            ITestAction hex = named1.GetAction("Hex");
            Assert.IsNotNull(hex);
            Equals("Hex", hex.Name);
        }

        [TestMethod]
        public virtual void NamedAppliedToObject() {
            var named1 = NewTestObject<Named1>();
            named1.AssertTitleEquals("Untitled Foo");
        }

        [TestMethod]
        public virtual void NamedAppliedToParameter() {
            var named1 = NewTestObject<Named1>();
            ITestAction hex = named1.GetAction("Hex");
            ITestParameter param = hex.Parameters[0];
            Equals("Yop", param.Name);
        }

        [TestMethod]
        public virtual void NamedAppliedToProperty() {
            var named1 = NewTestObject<Named1>();
            ITestProperty bar = named1.GetPropertyByName("Bar");
            Assert.IsNotNull(bar);
        }

        #endregion

        #region Range

        private static readonly string todayMinus31 = DateTime.Today.AddDays(-31).ToShortDateString();
        private static readonly string todayMinus30 = DateTime.Today.AddDays(-30).ToShortDateString();
        private static readonly string todayMinus1 = DateTime.Today.AddDays(-1).ToShortDateString();
        private static readonly string today = DateTime.Today.ToShortDateString();
        private static readonly string todayPlus1 = DateTime.Today.AddDays(1).ToShortDateString();
        private static readonly string todayPlus30 = DateTime.Today.AddDays(30).ToShortDateString();
        private static readonly string todayPlus31 = DateTime.Today.AddDays(31).ToShortDateString();

        [TestMethod]
        public virtual void RangeOnNumericProperties() {
            NumericPropertyRangeTest("Prop3");
            NumericPropertyRangeTest("Prop4");
            NumericPropertyRangeTest("Prop5");
            NumericPropertyRangeTest("Prop6");
            NumericPropertyRangeTest("Prop7");
            NumericPropertyRangeTest("Prop8");
            NumericPropertyRangeTest("Prop9");
            NumericPropertyRangeTest("Prop10");
            NumericPropertyRangeTest("Prop11");
            NumericPropertyRangeTest("Prop12");
            NumericPropertyRangeTest("Prop14");
            NumericPropertyRangeTest("Prop15");
            NumericPropertyRangeTest("Prop16");
            NumericPropertyRangeTest("Prop17");
            NumericPropertyRangeTest("Prop18");
            NumericPropertyRangeTest("Prop19");
            NumericPropertyRangeTest("Prop20");
            NumericPropertyRangeTest("Prop21");
            NumericPropertyRangeTest("Prop22");
            NumericPropertyRangeTest("Prop23");
        }

        private void NumericPropertyRangeTest(string name) {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById(name);
            try {
                prop.AssertFieldEntryInvalid("-2");
                prop.AssertFieldEntryInvalid("11");
                prop.SetValue("1");
            }
            catch {
                Console.WriteLine("Failed " + name);
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnDateProperties() {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById("Prop25");
            try {
                prop.AssertFieldEntryInvalid(todayMinus31);
                prop.AssertFieldEntryIsValid(todayMinus30);
                prop.AssertFieldEntryIsValid(today);
                prop.AssertFieldEntryInvalid(todayPlus1);
                prop.SetValue(todayMinus1);
            }
            catch {
                Console.WriteLine("Failed " + "Prop25");
                throw;
            }

            prop = obj.GetPropertyById("Prop26");
            try {
                prop.AssertFieldEntryInvalid(today);
                prop.AssertFieldEntryIsValid(todayPlus1);
                prop.AssertFieldEntryIsValid(todayPlus30);
                prop.AssertFieldEntryInvalid(todayPlus31);
                prop.SetValue(todayPlus1);
            }
            catch {
                Console.WriteLine("Failed " + "Prop25");
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnDateParms() {
            ITestObject obj = NewTestObject<Range1>();
            ITestAction act = obj.GetAction("Action 24");
            try {
                act.AssertIsInvalidWithParms(todayMinus31);
                act.AssertIsValidWithParms(todayMinus30);
                act.AssertIsValidWithParms(today);
                act.AssertIsInvalidWithParms(todayPlus1);
                act.InvokeReturnObject(todayMinus1);
            }
            catch {
                Console.WriteLine("Failed " + "Action24");
                throw;
            }

            act = obj.GetAction("Action 25");
            try {
                act.AssertIsInvalidWithParms(today);
                act.AssertIsValidWithParms(todayPlus1);
                act.AssertIsValidWithParms(todayPlus30);
                act.AssertIsInvalidWithParms(todayPlus31);
                act.InvokeReturnObject(todayPlus1);
            }
            catch {
                Console.WriteLine("Failed " + "Action25");
                throw;
            }
        }

        private void NumericParmRangeTest(string name) {
            ITestObject obj = NewTestObject<Range1>();
            ITestAction act = obj.GetAction(name);

            try {
                act.AssertIsInvalidWithParms(4);
                act.AssertIsValidWithParms(5);
                act.AssertIsValidWithParms(6);
                act.AssertIsInvalidWithParms(7);
            }
            catch {
                Console.WriteLine("Failed " + name);
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnNumericParms() {
            NumericParmRangeTest("Action1");
            NumericParmRangeTest("Action2");
            NumericParmRangeTest("Action3");
            NumericParmRangeTest("Action4");
            NumericParmRangeTest("Action5");
            NumericParmRangeTest("Action6");
            NumericParmRangeTest("Action7");
            NumericParmRangeTest("Action8");
            NumericParmRangeTest("Action9");
            NumericParmRangeTest("Action 10");
            NumericParmRangeTest("Action 11");
            NumericParmRangeTest("Action 12");
            NumericParmRangeTest("Action 13");
            NumericParmRangeTest("Action 14");
            NumericParmRangeTest("Action 15");
            NumericParmRangeTest("Action 16");
            NumericParmRangeTest("Action 17");
            NumericParmRangeTest("Action 18");
            NumericParmRangeTest("Action 19");
            NumericParmRangeTest("Action 20");
            NumericParmRangeTest("Action 21");
            NumericParmRangeTest("Action 22");
        }

        [TestMethod]
        public virtual void UnsupportedRangeOnProperty() {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById("Prop24");
            prop.SetValue("-2");
            prop.SetValue("11");
            prop.SetValue("1");
        }

        [TestMethod]
        public virtual void UnsupportedRangeOnParm() {
            ITestObject obj = NewTestObject<Range1>();
            ITestAction act = obj.GetAction("Action 23");
            act.AssertIsValidWithParms(4);
            act.AssertIsValidWithParms(5);
            act.AssertIsValidWithParms(10);
        }

        #endregion

        #region Regex

        [TestMethod]
        public virtual void SimpleRegExAttributeOnProperty() {
            ITestObject obj = NewTestObject<Regex1>();
            ITestProperty email = obj.GetPropertyByName("Email");
            email.AssertFieldEntryInvalid("richard @hotmail.com");
            email.AssertFieldEntryInvalid("richardpAThotmail.com");
            email.AssertFieldEntryInvalid("richardp@hotmail;com");
            email.AssertFieldEntryInvalid("richardp@hotmail.com (preferred)");
            email.AssertFieldEntryInvalid("personal richardp@hotmail.com");
            email.SetValue("richard@hotmail.com");
        }

        [TestMethod]
        public virtual void SimpleRegularExpressionAttributeOnProperty() {
            ITestObject obj = NewTestObject<Regex1>();
            ITestProperty email = obj.GetPropertyByName("Email");
            email.AssertFieldEntryInvalid("richard @hotmail.com");
            email.AssertFieldEntryInvalid("richardpAThotmail.com");
            email.AssertFieldEntryInvalid("richardp@hotmail;com");
            email.AssertFieldEntryInvalid("richardp@hotmail.com (preferred)");
            email.AssertFieldEntryInvalid("personal richardp@hotmail.com");
            email.SetValue("richard@hotmail.com");
        }

        #endregion

        #region MemberOrder

        [TestMethod]
        public void PropertyOrder() {
            ITestObject obj2 = NewTestObject<Memberorder1>();
            obj2.AssertPropertyOrderIs("Prop2, Prop1");

            ITestProperty[] properties = obj2.Properties;
            Assert.AreEqual(properties[0].Name, "Prop2");
            Assert.AreEqual(properties[1].Name, "Prop1");
        }

        [TestMethod]
        public void PropertyOrderOnSubClass() {
            ITestObject obj3 = NewTestObject<Memberorder2>();
            obj3.AssertPropertyOrderIs("Prop2, Prop4, Prop1, Prop3");

            ITestProperty[] properties = obj3.Properties;
            Assert.AreEqual(properties[0].Name, "Prop2");
            Assert.AreEqual(properties[1].Name, "Prop4");
            Assert.AreEqual(properties[2].Name, "Prop1");
            Assert.AreEqual(properties[3].Name, "Prop3");
        }

        [TestMethod]
        public void ActionOrder() {
            ITestObject obj2 = NewTestObject<Memberorder1>();
            ITestAction[] actions = obj2.Actions;
            Assert.AreEqual(actions[0].Name, "Action2");
            Assert.AreEqual(actions[1].Name, "Action1");

            obj2.AssertActionOrderIs("Action2, Action1");
        }

        [TestMethod]
        public void ActionOrderOnSubClass() {
            ITestObject obj3 = NewTestObject<Memberorder2>();
            ITestAction[] actions = obj3.Actions;
            Assert.AreEqual(actions[0].Name, "Action2");
            Assert.AreEqual(actions[1].Name, "Action4");
            Assert.AreEqual(actions[2].Name, "Action1");
            Assert.AreEqual(actions[3].Name, "Action3");

            obj3.AssertActionOrderIs("Action2, Action4, Action1, Action3");
        }

        #endregion

        #region StringLength

        [TestMethod]
        public virtual void StringLengthOnProperty() {
            ITestObject obj = NewTestObject<Stringlength1>();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertFieldEntryInvalid("12345678");
            prop2.AssertFieldEntryInvalid("1234567 ");
            prop2.SetValue("1234567");
        }

        [TestMethod]
        public virtual void StringLengthOnParm() {
            ITestObject obj = NewTestObject<Stringlength1>();
            ITestAction act = obj.GetAction("Action");

            act.AssertIsInvalidWithParms("123456789");
            act.AssertIsInvalidWithParms("12345678 ");
            act.AssertIsValidWithParms("12345678");
        }

        #endregion

        #region Title

        [TestMethod]
        public virtual void ObjectWithTitleAttributeOnString() {
            var obj = NewTestObject<Title1>();
            obj.AssertTitleEquals("Untitled Title1");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        [TestMethod] //Error caused by change to TitleFacetViaProperti in f86f40ac on 08/10/2014
        public virtual void TitleAttributeOnReferencePropertyThatHasATitleAttribute() {
            var obj1 = NewTestObject<Title1>();
            obj1.GetPropertyByName("Prop1").SetValue("Foo");
            obj1.AssertTitleEquals("Foo");
            obj1.Save();

            var obj8 = NewTestObject<Title8>();
            obj8.AssertTitleEquals("Untitled Title8");
            var prop1 = obj8.GetPropertyByName("Prop1");
            prop1.SetObject(obj1);
            obj8.AssertTitleEquals("Foo");
        }

        [TestMethod] //Error caused by change to TitleFacetViaProperti in f86f40ac on 08/10/2014
        public virtual void TitleAttributeOnReferencePropertyThatHasATitleMethod() {
            var obj4 = NewTestObject<Title4>();
            obj4.GetPropertyByName("Prop1").SetValue("Foo");
            obj4.AssertTitleEquals("Foo");
            obj4.Save();

            var obj7 = NewTestObject<Title7>();
            obj7.AssertTitleEquals("Untitled Title7");
            var prop1 = obj7.GetPropertyByName("Prop1");
            prop1.SetObject(obj4);
            obj7.AssertTitleEquals("Foo");
        }

        [TestMethod]
        public virtual void TitleAttributeOnReferencePropertyThatHasAToString() {
            var obj2 = NewTestObject<Title2>();
            obj2.GetPropertyByName("Prop1").SetValue("Foo");
            var dom2 = (Title2) obj2.GetDomainObject();
            Equals("Foo", dom2.ToString());
            obj2.AssertTitleEquals("Foo");
            obj2.Save();

            var obj9 = NewTestObject<Title9>();
            obj9.AssertTitleEquals("Untitled Title9");
            var prop1 = obj9.GetPropertyByName("Prop1");
            prop1.SetObject(obj2);
            obj9.AssertTitleEquals("Foo");
        }

        [TestMethod]
        public virtual void TitleAttributeTakesPrecedenceOverTitleMethod() {
            var obj = NewTestObject<Title6>();
            var dom = (Title6) obj.GetDomainObject();
            Equals("Bar", dom.ToString());
            Equals("Hex", dom.Title());
            obj.AssertTitleEquals("Untitled Title6");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        [TestMethod]
        public virtual void TitleAttributeTakesPrecedenceOverToString() {
            var obj = NewTestObject<Title3>();
            Equals("Bar", obj.GetDomainObject().ToString());
            obj.AssertTitleEquals("Untitled Title3");
            var prop1 = obj.GetPropertyByName("Prop1");
            prop1.SetValue("Foo");
            obj.AssertTitleEquals("Foo");
            obj.Save();
            obj.AssertTitleEquals("Foo");
        }

        #endregion

        #region ValidateProgrammaticUpdates

        [TestMethod]
        public virtual void ValidateObjectSave() {
            ITestService service = GetTestService(typeof (TestServiceValidateProgrammaticUpdates));
            var vpu1 = NewTestObject<Validateprogrammaticupdates1>();

            try {
                (vpu1.GetDomainObject() as Validateprogrammaticupdates1).Prop1 = "fail";

                service.GetAction("Save Object1").InvokeReturnObject(vpu1);
                Assert.Fail();
            }
            catch (Exception /*expected*/) {}
        }

        [TestMethod]
        public virtual void ValidateObjectCrossValidationSave() {
            ITestService service = GetTestService(typeof (TestServiceValidateProgrammaticUpdates));
            var vpu2 = NewTestObject<Validateprogrammaticupdates2>();
            try {
                (vpu2.GetDomainObject() as Validateprogrammaticupdates2).Prop1 = "fail";
                service.GetAction("Save Object2").InvokeReturnObject(vpu2);
                Assert.Fail();
            }
            catch (Exception /*expected*/) {}
        }

        [TestMethod]
        public virtual void ValidateObjectChange() {
            ITestService service = GetTestService(typeof (TestServiceValidateProgrammaticUpdates));
            var vpu1 = NewTestObject<Validateprogrammaticupdates1>();

            service.GetAction("Save Object1").InvokeReturnObject(vpu1);

            try {
                (vpu1.GetDomainObject() as Validateprogrammaticupdates1).Prop1 = "fail";
                Assert.Fail();
            }
            catch (Exception /*expected*/) {}
        }

        [TestMethod]
        public virtual void ValidateObjectCrossValidationChange() {
            ITestService service = GetTestService(typeof (TestServiceValidateProgrammaticUpdates));
            var vpu2 = NewTestObject<Validateprogrammaticupdates2>();

            service.GetAction("Save Object2").InvokeReturnObject(vpu2);

            try {
                (vpu2.GetDomainObject() as Validateprogrammaticupdates2).Prop1 = "fail";
                Assert.Fail();
            }
            catch (Exception /*expected*/) {}
        }

        #endregion
    }

    #region Classes used in test

    public class AttributesDbContext : DbContext {
        public const string DatabaseName = "TestAttributes";
        public AttributesDbContext() : base(DatabaseName) {}

        public DbSet<Default1> Default1s { get; set; }
        public DbSet<Describedas1> DescribedAs1s { get; set; }
        public DbSet<Describedas2> DescribedAs2s { get; set; }
        public DbSet<Description1> Description1s { get; set; }
        public DbSet<Description2> Description2s { get; set; }
        public DbSet<Disabled1> Disabled1s { get; set; }
        public DbSet<Displayname1> Displayname1s { get; set; }
        public DbSet<Hidden1> Hidden1s { get; set; }
        public DbSet<Iconname1> Iconname1s { get; set; }
        public DbSet<Iconname2> Iconname2s { get; set; }
        public DbSet<Iconname3> Iconname3s { get; set; }
        public DbSet<Iconname4> Iconname4s { get; set; }
        public DbSet<Immutable1> Immutable1s { get; set; }
        public DbSet<Mask1> Mask1s { get; set; }
        public DbSet<Mask2> Mask2s { get; set; }
        public DbSet<Maxlength1> Maxlength1s { get; set; }
        public DbSet<Maxlength2> Maxlength2s { get; set; }
        public DbSet<NakedObjectsIgnore1> NakedObjectsIgnore1s { get; set; }
        public DbSet<NakedObjectsIgnore2> NakedObjectsIgnore2s { get; set; }
        public DbSet<NakedObjectsIgnore3> NakedObjectsIgnore3s { get; set; }
        public DbSet<NakedObjectsIgnore4> NakedObjectsIgnore4s { get; set; }
        public DbSet<NakedObjectsIgnore5> NakedObjectsIgnore5s { get; set; }
        public DbSet<Named1> Named1s { get; set; }
        public DbSet<Range1> Range1s { get; set; }
        public DbSet<Regex1> Regex1s { get; set; }
        public DbSet<Regex2> Regex2s { get; set; }
        public DbSet<Memberorder1> Memberorder1s { get; set; }
        public DbSet<Stringlength1> Stringlength1s { get; set; }
        public DbSet<Title1> Title1s { get; set; }
        public DbSet<Title2> Title2s { get; set; }
        public DbSet<Title3> Title3s { get; set; }
        public DbSet<Title4> Title4s { get; set; }
        public DbSet<Title5> Title5s { get; set; }
        public DbSet<Title6> Title6s { get; set; }
        public DbSet<Title7> Title7s { get; set; }
        public DbSet<Title8> Title8s { get; set; }
        public DbSet<Title9> Title9s { get; set; }
        public DbSet<Validateprogrammaticupdates1> ValidateProgrammaticUpdates1s { get; set; }
        public DbSet<Validateprogrammaticupdates2> ValidateProgrammaticUpdates2s { get; set; }
        public DbSet<Contributee> Contributees { get; set; }
        public DbSet<Contributee2> Contributee2s { get; set; }
        public DbSet<Contributee3> Contributee3s { get; set; }
        public DbSet<FinderAction1> Exclude1s { get; set; }
    }

    #region ContributedAction

    #endregion

    #region Default

    public class Default1 {
        public virtual int Id { get; set; }

        [DefaultValue(8)]
        public virtual int Prop1 { get; set; }

        [DefaultValue("Foo")]
        public virtual string Prop2 { get; set; }

        public virtual void DoSomething([DefaultValue(8)] int param0,
                                        [DefaultValue("Foo")] string param1) {}
    }

    #endregion

    #region DescribedAs

    [DescribedAs("Foo")]
    public class Describedas1 {
        public virtual int Id { get; set; }

        [DescribedAs("Bar")]
        public virtual string Prop1 { get; set; }

        [DescribedAs("Hex")]
        public void DoSomething([DescribedAs("Yop")] string param1) {}
    }

    public class Describedas2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) {}
    }

    #endregion

    #region Description

    [System.ComponentModel.Description("Foo")]
    public class Description1 {
        public virtual int Id { get; set; }

        [System.ComponentModel.Description("Bar")]
        public virtual string Prop1 { get; set; }

        [System.ComponentModel.Description("Hex")]
        public void DoSomething([System.ComponentModel.Description("Yop")] string param1) {}
    }

    public class Description2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) {}
    }

    #endregion

    #region Disabled

    public class Disabled1 {
        public Disabled1() {
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = Prop6 = "";
        }

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Disabled]
        public virtual string Prop1 { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Disabled(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Disabled(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Disabled(WhenTo.Always)]
        public virtual string Prop5 { get; set; }

        public virtual string Prop6 { get; set; }

        public string DisableProp6() {
            if (Prop4 == "Disable 6") {
                return "Disabled Message";
            }
            return null;
        }
    }

    #endregion

    #region DisplayName

    [DisplayName("Foo")]
    public class Displayname1 {
        public virtual int Id { get; set; }

        [DisplayName("Bar")]
        public virtual string Prop1 { get; set; }

        [DisplayName("Hex")]
        public virtual void DoSomething(string param1) {}
    }

    #endregion

    #region Hidden

    public class Hidden1 {
        public Hidden1() {
            // initialise all fields 
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = string.Empty;
        }

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Prop1 { get; set; }

        [Hidden(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Hidden(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Hidden(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Prop5 { get; set; }
    }

    #endregion

    #region IconName

    public class Iconname1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    [IconName("Foo")]
    public class Iconname2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Iconname3 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string IconName() {
            return "Bar";
        }
    }

    [IconName("Foo")]
    public class Iconname4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string IconName() {
            return "Bar";
        }
    }

    #endregion

    #region Immutable

    public class Immutable1 {
        public Immutable1() {
            // initialise all fields 
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = Prop6 = string.Empty;
        }

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Disabled]
        public virtual string Prop1 { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Disabled(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Disabled(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Disabled(WhenTo.Always)]
        public virtual string Prop5 { get; set; }

        public virtual string Prop6 { get; set; }

        public string DisableProp6() {
            if (Prop4 == "Disable 6") {
                return "Disabled Message";
            }
            return null;
        }
    }

    [Immutable]
    public class Immutable2 : Immutable1 {
        public void ChangeProp1() {
            Prop1 = "Foo";
        }
    }

    [Immutable(WhenTo.OncePersisted)]
    public class Immutable3 : Immutable1 {}

    #endregion

    #region Mask

    public class Mask1 {
        public virtual int Id { get; set; }

        public virtual DateTime Prop1 { get; set; }

        [Mask("d")]
        public virtual DateTime Prop2 { get; set; }

        public void DoSomething([Mask("d")] DateTime d1) {}
    }

    public class Mask2 {
        public virtual int Id { get; set; }

        [Mask("c")]
        public virtual decimal Prop1 { get; set; }
    }

    #endregion

    #region MaxLength

    public class Maxlength1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) {}
    }

    public class Maxlength2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) {}
    }

    #endregion

    #region Named

    [Named("Foo")]
    public class Named1 {
        public virtual int Id { get; set; }

        [Named("Bar")]
        public virtual string Prop1 { get; set; }

        [Named("Hex")]
        public void DoSomething([Named("Yop")] string param1) {}
    }

    #endregion

    #region NakedObjectsIgnore

    public class NakedObjectsIgnore1 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        public void Action() {}

        [NakedObjectsIgnore]
        public void ActionIgnored() {}

        public NakedObjectsIgnore2 ActionReturningIgnoredType() {
            return null;
        }

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) {}
    }

    //[NakedObjectsType(ReflectOver.None)]
    public class NakedObjectsIgnore2 {
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }
    }

    //[NakedObjectsType(ReflectOver.All)]
    public class NakedObjectsIgnore3 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        public void Action() {}

        [NakedObjectsIgnore]
        public void ActionIgnored() {}

        public NakedObjectsIgnore2 ActionReturningIgnoredType() {
            return null;
        }

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) {}
    }

    //[NakedObjectsType(ReflectOver.TypeOnlyNoMembers)]
    public class NakedObjectsIgnore4 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        public void Action() {}

        [NakedObjectsIgnore]
        public void ActionIgnored() {}

        public NakedObjectsIgnore2 ActionReturningIgnoredType() {
            return null;
        }

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) {}
    }

    //[NakedObjectsType(ReflectOver.ExplicitlyIncludedMembersOnly)]
    public class NakedObjectsIgnore5 {
        public virtual int Id { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        //[NakedObjectsInclude]
        public virtual NakedObjectsIgnore1 RefProp2 { get; set; }

        //[NakedObjectsInclude] //Should have no impact if scope is AllMembers
        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        //[NakedObjectsInclude]
        public ICollection<NakedObjectsIgnore1> Coll2 { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        //[NakedObjectsInclude]
        public void Action() {}

        public void Action2() {}

        //[NakedObjectsInclude] //Should still be ignored, because return type is ignored
        public NakedObjectsIgnore2 ActionReturningIgnoredType() {
            return null;
        }

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) {}
    }

    public class NakedObjectsIgnore6 : NakedObjectsIgnore4 {
        public virtual string Prop3 { get; set; }

        public void Action2() {}

        public void Action3() {}
    }

    public class NakedObjectsIgnore7 : NakedObjectsIgnore5 {
        public virtual string Prop3 { get; set; }

        //[NakedObjectsInclude]
        public virtual string Prop4 { get; set; }

        //[NakedObjectsInclude]
        public void Action3() {}

        public void Action4() {}
    }

    #endregion

    #region Range

    public class Range1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [Range(-1, 10)]
        public virtual short Prop3 { get; set; }

        [Range(-1, 10)]
        public virtual int Prop4 { get; set; }

        [Range(-1, 10)]
        public virtual long Prop5 { get; set; }

        [Range(1, 10)]
        public virtual byte Prop6 { get; set; }

        [Range(1, 10)]
        public virtual ushort Prop7 { get; set; }

        [Range(1, 10)]
        public virtual uint Prop8 { get; set; }

        [Range(1, 10)]
        public virtual ulong Prop9 { get; set; }

        [Range(-1, 10)]
        public virtual float Prop10 { get; set; }

        [Range(-1, 10)]
        public virtual double Prop11 { get; set; }

        [Range(-1, 10)]
        public virtual decimal Prop12 { get; set; }

        [Range(-1d, 10d)]
        public virtual short Prop14 { get; set; }

        [Range(-1d, 10d)]
        public virtual int Prop15 { get; set; }

        [Range(-1d, 10d)]
        public virtual long Prop16 { get; set; }

        [Range(1d, 10d)]
        public virtual byte Prop17 { get; set; }

        [Range(1d, 10d)]
        public virtual ushort Prop18 { get; set; }

        [Range(1d, 10d)]
        public virtual uint Prop19 { get; set; }

        [Range(1d, 10d)]
        public virtual ulong Prop20 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual float Prop21 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual double Prop22 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual decimal Prop23 { get; set; }

        [Range(typeof (string), "1", "10")]
        public virtual int Prop24 { get; set; }

        [Range(-30, 0)]
        public virtual DateTime Prop25 { get; set; }

        [Range(1, 30)]
        public virtual DateTime Prop26 { get; set; }

        public void Action1([Range(5, 6)] sbyte parm) {}
        public void Action2([Range(5, 6)] short parm) {}
        public void Action3([Range(5, 6)] int parm) {}
        public void Action4([Range(5, 6)] long parm) {}
        public void Action5([Range(5, 6)] byte parm) {}
        public void Action6([Range(5, 6)] ushort parm) {}
        public void Action7([Range(5, 6)] uint parm) {}
        public void Action8([Range(5, 6)] ulong parm) {}
        public void Action9([Range(5, 6)] float parm) {}
        public void Action10([Range(5, 6)] double parm) {}
        public void Action11([Range(5, 6)] decimal parm) {}
        public void Action12([Range(5d, 6d)] sbyte parm) {}
        public void Action13([Range(5d, 6d)] short parm) {}
        public void Action14([Range(5d, 6d)] int parm) {}
        public void Action15([Range(5d, 6d)] long parm) {}
        public void Action16([Range(5d, 6d)] byte parm) {}
        public void Action17([Range(5d, 6d)] ushort parm) {}
        public void Action18([Range(5d, 6d)] uint parm) {}
        public void Action19([Range(5d, 6d)] ulong parm) {}
        public void Action20([Range(5d, 6d)] float parm) {}
        public void Action21([Range(5d, 6d)] double parm) {}
        public void Action22([Range(5d, 6d)] decimal parm) {}
        public void Action23([Range(typeof (string), "5", "6")] int parm) {}

        public void Action24([Range(-30, 0)] DateTime parm) {}
        public void Action25([Range(1, 30)] DateTime parm) {}
    }

    #endregion

    #region Regex

    public class Regex1 {
        public virtual int Id { get; set; }

        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }

    public class Regex2 {
        public virtual int Id { get; set; }

        [RegularExpression(@"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }

    #endregion

    #region MemberOrder

    public class Memberorder1 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(3)]
        public virtual string Prop1 { get; set; }

        [MemberOrder(1)]
        public virtual string Prop2 { get; set; }

        [MemberOrder(3)]
        public virtual void Action1() {}

        [MemberOrder(1)]
        public virtual void Action2() {}
    }

    public class Memberorder2 : Memberorder1 {
        [MemberOrder(4)]
        public virtual string Prop3 { get; set; }

        [MemberOrder(2)]
        public virtual string Prop4 { get; set; }

        [MemberOrder(4)]
        public void Action3() {}

        [MemberOrder(2)]
        public void Action4() {}
    }

    #endregion

    #region StringLength

    public class Stringlength1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [StringLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([StringLength(8)] string parm) {}
    }

    #endregion

    #region Title

    public class Title1 {
        public virtual int Id { get; set; }

        [Title]
        [Optionally]
        public virtual string Prop1 { get; set; }
    }

    public class Title2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return Prop1;
        }
    }

    public class Title3 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Bar";
        }
    }

    public class Title4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title() {
            return Prop1;
        }
    }

    public class Title5 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title() {
            return Prop1;
        }

        public override string ToString() {
            return "Bar";
        }
    }

    public class Title6 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public string Title() {
            return "Hex";
        }

        public override string ToString() {
            return "Bar";
        }
    }

    public class Title7 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title4 Prop1 { get; set; }
    }

    public class Title8 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title1 Prop1 { get; set; }
    }

    public class Title9 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title2 Prop1 { get; set; }
    }

    #endregion

    #region ValidateProgrammaticUpdates

    [ValidateProgrammaticUpdates]
    public class Validateprogrammaticupdates1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        [Optionally]
        public string Prop2 { get; set; }

        public string ValidateProp1(string prop1) {
            return prop1 == "fail" ? "fail" : null;
        }
    }

    [ValidateProgrammaticUpdates]
    public class Validateprogrammaticupdates2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        [Optionally]
        public virtual string Prop2 { get; set; }

        public string Validate(string prop1, string prop2) {
            return prop1 == "fail" ? "fail" : null;
        }
    }

    public class TestServiceValidateProgrammaticUpdates {
        public IDomainObjectContainer Container { set; protected get; }

        public void SaveObject1(Validateprogrammaticupdates1 obj) {
            Container.Persist(ref obj);
        }

        public void SaveObject2(Validateprogrammaticupdates2 obj) {
            Container.Persist(ref obj);
        }
    }

    #endregion

    #region FinderAction

    #endregion

    #endregion
}

// Change the namespace of these test classes as if they start wuith 'NakedObjects' we will not introspect them

namespace SystemTest.Attributes {
    public class TestServiceContributedAction {
        public IDomainObjectContainer Container { set; protected get; }

        public void ContributedAction([ContributedAction("Test Service Contributed Action")] Contributee obj) {}

        public void NotContributedAction(Contributee obj) {}

        public IQueryable<Contributee2> AllContributee2() {
            return Container.Instances<Contributee2>();
        }

        public void CollectionContributedAction([ContributedAction] IQueryable<Contributee2> targets) {}

        public void CollectionContributedAction1([ContributedAction] IQueryable<Contributee2> targets, string parm2) {}

        public void CollectionContributedAction2([ContributedAction] IQueryable<Contributee2> targets, Contributee cont) {}

        public IQueryable<Contributee2> NotCollectionContributedAction1([ContributedAction] IQueryable<Contributee2> targets) {
            throw new NotImplementedException();
        }

        public ICollection<Contributee2> NotCollectionContributedAction2([ContributedAction] IQueryable<Contributee2> targets) {
            throw new NotImplementedException();
        }

        public void NotCollectionContributedAction3([ContributedAction] IEnumerable<Contributee2> targets) {}
    }

    public class Contributee {
        public virtual int Id { get; set; }
    }

    public class Contributee2 {
        public virtual int Id { get; set; }

        public void NativeAction() {}
    }

    public class Contributee3 : Contributee2 {
        public void NativeAction3() {}
    }

    public class TestServiceFinderAction {
        [FinderAction()]
        public FinderAction1 FinderAction1() {
            return null;
        }

        [FinderAction()]
        public ICollection<FinderAction1> FinderAction2(string s, int i) {
            return null;
        }

        [FinderAction()]
        public FinderAction1 FinderAction3(string s, FinderAction1 obj) {
            return null;
        }

        public IList<FinderAction1> Choices1FinderAction3() {
            return new List<FinderAction1> {};
        }

        //No annotation
        public FinderAction1 NotFinderAction1() {
            return null;
        }

        //No annotation
        public ICollection<FinderAction1> NotFinderAction2() {
            return null;
        }

        [FinderAction()] //Non-parseable param without choices
        public FinderAction1 NotFinderAction3(string s, FinderAction1 obj) {
            return null;
        }

        [FinderAction()] //Returns string
        public string NotFinderAction4() {
            return null;
        }

        internal FinderAction1 NewObject1() {
            return new FinderAction1();
        }
    }

    public class FinderAction1 {
        public virtual int Id { get; set; }
    }
}