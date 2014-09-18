// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using NakedObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace NakedObjects.SystemTest
{
    [TestClass]
    public class TestAttributes : AbstractSystemTest2<MyDbContext>
    {
        #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestAttributes());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestAttributes());
            Database.Delete(MyDbContext.DatabaseName);
        }

        private ITestObject obj1;
        private ITestObject describedAs1;
        private ITestObject describedAs2;
        private ITestObject description1;
        private ITestObject description2;
        private ITestObject displayname1;
        private ITestObject hidden1;
        private ITestObject named1;

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            obj1 = NewTestObject<Default1>();
            describedAs1 = NewTestObject<Describedas1>();
            describedAs2 = NewTestObject<Describedas2>();
            description1 = NewTestObject<Description1>();
            description2 = NewTestObject<Description2>();
            displayname1 = NewTestObject<Displayname1>();
            hidden1 = NewTestObject<Hidden1>();
            named1 = NewTestObject<Named1>();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            obj1 = null;
            describedAs1 = null;
            describedAs2 = null;
            description1 = null;
            description2 = null;
            displayname1 = null;
            hidden1 = null;
            named1 = null;
        }

        #endregion

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new SimpleRepository<Default1>(),
                    new SimpleRepository<Describedas1>(),
                    new SimpleRepository<Describedas2>(),
                    new SimpleRepository<Description1>(),
                    new SimpleRepository<Description2>(),
                     new SimpleRepository<Disabled1>(),
                     new SimpleRepository<Displayname1>(),
                     new SimpleRepository<Hidden1>(),
                     new SimpleRepository<Iconname1>(),
                     new SimpleRepository<Iconname2>(),
                     new SimpleRepository<Iconname3>(),
                     new SimpleRepository<Iconname4>(),
                     new SimpleRepository<Immutable1>(),
                     new SimpleRepository<Immutable2>(),
                     new SimpleRepository<Immutable3>(),
                     new SimpleRepository<Mask1>(),
                     new SimpleRepository<Mask2>(),
                    new SimpleRepository<Maxlength1>(),
                    new SimpleRepository<Maxlength2>(),
                    new SimpleRepository<Named1>(),
                    new SimpleRepository<Range1>(),
                    new SimpleRepository<Regex1>(),
                    new SimpleRepository<Regex2>()
                     );
            }
        }

        #region Default
        [TestMethod]
        public virtual void DefaultNumericProperty()
        {
            ITestProperty prop = obj1.GetPropertyByName("Prop1");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("8", def);
        }

        [TestMethod]
        public void DefaultParameters()
        {
            ITestAction action = obj1.GetAction("Do Something");
            string def0 = action.Parameters[0].GetDefault().Title;
            Assert.IsNotNull(def0);
            Assert.AreEqual("8", def0);

            string def1 = action.Parameters[1].GetDefault().Title;
            Assert.IsNotNull(def1);
            Assert.AreEqual("Foo", def1);
        }

        [TestMethod]
        public virtual void DefaultStringProperty()
        {
            ITestProperty prop = obj1.GetPropertyByName("Prop2");
            string def = prop.GetDefault().Title;
            Assert.IsNotNull(def);
            Assert.AreEqual("Foo", def);
        }
        #endregion

        #region DescribedAs
        [TestMethod]
        public virtual void DescribedAsAppliedToAction()
        {
            ITestAction action = describedAs1.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("Hex");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToObject()
        {
            describedAs1.AssertIsDescribedAs("Foo");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToParameter()
        {
            ITestAction action = describedAs1.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("Yop");
        }

        [TestMethod]
        public virtual void DescribedAsAppliedToProperty()
        {
            ITestProperty prop = describedAs1.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("Bar");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToAction()
        {
            ITestAction action = describedAs2.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToObject()
        {
            describedAs2.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToParameter()
        {
            ITestAction action = describedAs2.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescribedAsAppliedToProperty()
        {
            ITestProperty prop = describedAs2.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("");
        }
        #endregion

        #region Description
        [TestMethod]
        public virtual void DescriptionAppliedToAction()
        {
            ITestAction action = description1.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("Hex");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToObject()
        {
            description1.AssertIsDescribedAs("Foo");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToParameter()
        {
            ITestAction action = description1.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("Yop");
        }

        [TestMethod]
        public virtual void DescriptionAppliedToProperty()
        {
            ITestProperty prop = description1.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("Bar");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToAction()
        {
            ITestAction action = description2.GetAction("Do Something");
            Assert.IsNotNull(action);
            action.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToObject()
        {
            description2.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToParameter()
        {
            ITestAction action = description2.GetAction("Do Something");
            ITestParameter param = action.Parameters[0];
            param.AssertIsDescribedAs("");
        }

        [TestMethod]
        public virtual void NullDescriptionAppliedToProperty()
        {
            ITestProperty prop = description2.GetPropertyByName("Prop1");
            prop.AssertIsDescribedAs("");
        }
        #endregion

        #region Disabled
        private ITestObject NewTransientDisabled1()
        {
            return GetTestService("Disabled1s").GetAction("New Instance").InvokeReturnObject();
        }

        [TestMethod]
        public virtual void Disabled()
        {
            var obj = NewTransientDisabled1();
            ITestProperty prop1 = obj.GetPropertyByName("Prop1");
            prop1.AssertIsUnmodifiable();
            obj.Save();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledAlways()
        {
            var obj = NewTransientDisabled1(); ITestProperty prop5 = obj.GetPropertyByName("Prop5");
            prop5.AssertIsUnmodifiable();
            obj.Save();
            prop5.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledNever()
        {
            var obj = NewTransientDisabled1(); ITestProperty prop4 = obj.GetPropertyByName("Prop4");
            prop4.AssertIsModifiable();
            obj.Save();
            prop4.AssertIsModifiable();
        }

        [TestMethod]
        public virtual void DisabledOncePersisted()
        {
            var obj = NewTransientDisabled1(); ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertIsModifiable();
            obj.Save();
            prop2.AssertIsUnmodifiable();
        }

        [TestMethod]
        public virtual void DisabledUntilPersisted()
        {
            var obj = NewTransientDisabled1(); ITestProperty prop3 = obj.GetPropertyByName("Prop3");
            prop3.AssertIsUnmodifiable();
            obj.Save();
            prop3.AssertIsModifiable();
        }
        #endregion

        #region DisplayName
        [TestMethod]
        public virtual void DisplayNameAppliedToAction()
        {
            ITestAction hex = displayname1.GetAction("Hex");
            Assert.IsNotNull(hex);
            StringAssert.Equals("Hex", hex.Name);
        }

        [TestMethod]
        public virtual void DisplayNameAppliedToObject()
        {
            displayname1.AssertTitleEquals("Untitled Foo");
        }
        #endregion

        #region Hidden

        [TestMethod]
        public virtual void Hidden()
        {
            ITestProperty prop1 = hidden1.GetPropertyByName("Prop1");
            prop1.AssertIsInvisible();

            hidden1.Save();
            prop1.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenAlways()
        {
            ITestProperty prop5 = hidden1.GetPropertyByName("Prop5");
            prop5.AssertIsInvisible();
            hidden1.Save();
            prop5.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenNever()
        {
            ITestProperty prop4 = hidden1.GetPropertyByName("Prop4");
            prop4.AssertIsVisible();
            hidden1.Save();
            prop4.AssertIsVisible();
        }

        [TestMethod]
        public virtual void HiddenOncePersisted()
        {
            ITestProperty prop2 = hidden1.GetPropertyByName("Prop2");
            prop2.AssertIsVisible();
            hidden1.Save();
            prop2.AssertIsInvisible();
        }

        [TestMethod]
        public virtual void HiddenUntilPersisted()
        {
            ITestProperty prop3 = hidden1.GetPropertyByName("Prop3");
            prop3.AssertIsInvisible();
            hidden1.Save();
            prop3.AssertIsVisible();
        }
        #endregion

        #region Immutable

        [TestMethod]
        public virtual void ObjectImmutable()
        {
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
        public virtual void TestObjectImmutableOncePersisted()
        {
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
        public virtual void CMaskOnDecimalProperty()
        {
            var mask1 = NewTestObject<Mask2>();
            var prop1 = mask1.GetPropertyByName("Prop1");
            prop1.SetValue("32.70");
            var dom = (Mask2)mask1.GetDomainObject();
            StringAssert.Equals("32.7", dom.Prop1.ToString());
            StringAssert.Equals("32.70", prop1.Content.Title);
            StringAssert.Equals("£32.70", prop1.Title);
            prop1.AssertTitleIsEqual("£32.70");
            prop1.AssertValueIsEqual("32.70");
        }

        [TestMethod]
        public virtual void DMaskOnDateProperty()
        {
            var mask1 = NewTestObject<Mask1>();
            var prop1 = mask1.GetPropertyByName("Prop1");
            prop1.SetValue("23/09/2009 11:34:50");
            var prop2 = mask1.GetPropertyByName("Prop2");
            prop2.SetValue("23/09/2009 11:34:50");
            var dom = (Mask1)mask1.GetDomainObject();
            StringAssert.Equals("23/09/2009 11:34:50", dom.Prop1.ToString());
            StringAssert.Equals("23/09/2009 11:34:50", prop1.Content.Title);
            StringAssert.Equals("23/09/2009 11:34:50", dom.Prop2.ToString());
            StringAssert.Equals("23/09/2009", prop2.Content.Title);
            prop1.AssertTitleIsEqual("23/09/2009 11:34:50");
            prop1.AssertValueIsEqual("23/09/2009 11:34:50");
            prop2.AssertTitleIsEqual("23/09/2009");
            prop2.AssertValueIsEqual("23/09/2009 11:34:50");
        }
        #endregion

        #region MaxLength
        [TestMethod]
        public virtual void NakedObjectsMaxLengthOnProperty()
        {
            ITestObject obj = NewTestObject<Maxlength1>();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertFieldEntryInvalid("12345678");
            prop2.AssertFieldEntryInvalid("1234567 ");
            prop2.SetValue("1234567");
        }

        [TestMethod]
        public virtual void NakedObjectsMaxLengthOnParm()
        {
            ITestObject obj = NewTestObject<Maxlength1>();
            ITestAction act = obj.GetAction("Action");

            act.AssertIsInvalidWithParms("123456789");
            act.AssertIsInvalidWithParms("12345678 ");
            act.AssertIsValidWithParms("12345678");
        }

        [TestMethod]
        public virtual void ComponentModelMaxLengthOnProperty()
        {
            ITestObject obj = NewTestObject<Maxlength2>();
            ITestProperty prop2 = obj.GetPropertyByName("Prop2");
            prop2.AssertFieldEntryInvalid("12345678");
            prop2.AssertFieldEntryInvalid("1234567 ");
            prop2.SetValue("1234567");
        }

        [TestMethod]
        public virtual void ComponentModelMaxLengthOnParm()
        {
            ITestObject obj = NewTestObject<Maxlength2>();
            ITestAction act = obj.GetAction("Action");

            act.AssertIsInvalidWithParms("123456789");
            act.AssertIsInvalidWithParms("12345678 ");
            act.AssertIsValidWithParms("12345678");
        }
        #endregion

        #region Named
        [TestMethod]
        public virtual void NamedAppliedToAction()
        {
            ITestAction hex = named1.GetAction("Hex");
            Assert.IsNotNull(hex);
            StringAssert.Equals("Hex", hex.Name);
        }

        [TestMethod]
        public virtual void NamedAppliedToObject()
        {
            named1.AssertTitleEquals("Untitled Foo");
        }

        [TestMethod]
        public virtual void NamedAppliedToParameter()
        {
            ITestAction hex = named1.GetAction("Hex");
            ITestParameter param = hex.Parameters[0];
            StringAssert.Equals("Yop", param.Name);
        }

        [TestMethod]
        public virtual void NamedAppliedToProperty()
        {
            ITestProperty bar = named1.GetPropertyByName("Bar");
            Assert.IsNotNull(bar);
        }
        #endregion

        #region Range
        private static string todayMinus31 = DateTime.Today.AddDays(-31).ToShortDateString();
        private static string todayMinus30 = DateTime.Today.AddDays(-30).ToShortDateString();
        private static string todayMinus1 = DateTime.Today.AddDays(-1).ToShortDateString();
        private static string today = DateTime.Today.ToShortDateString();
        private static string todayPlus1 = DateTime.Today.AddDays(1).ToShortDateString();
        private static string todayPlus30 = DateTime.Today.AddDays(30).ToShortDateString();
        private static string todayPlus31 = DateTime.Today.AddDays(31).ToShortDateString();


        [TestMethod]
        public virtual void RangeOnNumericProperties()
        {
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

        private void NumericPropertyRangeTest(string name)
        {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById(name);
            try
            {
                prop.AssertFieldEntryInvalid("-2");
                prop.AssertFieldEntryInvalid("11");
                prop.SetValue("1");
            }
            catch
            {
                Console.WriteLine("Failed " + name);
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnDateProperties()
        {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById("Prop25");
            try
            {
                prop.AssertFieldEntryInvalid(todayMinus31);
                prop.AssertFieldEntryIsValid(todayMinus30);
                prop.AssertFieldEntryIsValid(today);
                prop.AssertFieldEntryInvalid(todayPlus1);
                prop.SetValue(todayMinus1);
            }
            catch
            {
                Console.WriteLine("Failed " + "Prop25");
                throw;
            }


            prop = obj.GetPropertyById("Prop26");
            try
            {
                prop.AssertFieldEntryInvalid(today);
                prop.AssertFieldEntryIsValid(todayPlus1);
                prop.AssertFieldEntryIsValid(todayPlus30);
                prop.AssertFieldEntryInvalid(todayPlus31);
                prop.SetValue(todayPlus1);
            }
            catch
            {
                Console.WriteLine("Failed " + "Prop25");
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnDateParms()
        {
            ITestObject obj = NewTestObject<Range1>();
            ITestAction act = obj.GetAction("Action 24");
            try
            {
                act.AssertIsInvalidWithParms(todayMinus31);
                act.AssertIsValidWithParms(todayMinus30);
                act.AssertIsValidWithParms(today);
                act.AssertIsInvalidWithParms(todayPlus1);
                act.InvokeReturnObject(todayMinus1);
            }
            catch
            {
                Console.WriteLine("Failed " + "Action24");
                throw;
            }

            act = obj.GetAction("Action 25");
            try
            {
                act.AssertIsInvalidWithParms(today);
                act.AssertIsValidWithParms(todayPlus1);
                act.AssertIsValidWithParms(todayPlus30);
                act.AssertIsInvalidWithParms(todayPlus31);
                act.InvokeReturnObject(todayPlus1);
            }
            catch
            {
                Console.WriteLine("Failed " + "Action25");
                throw;
            }
        }

        private void NumericParmRangeTest(string name)
        {
            ITestObject obj = NewTestObject<Range1>();
            ITestAction act = obj.GetAction(name);

            try
            {
                act.AssertIsInvalidWithParms(4);
                act.AssertIsValidWithParms(5);
                act.AssertIsValidWithParms(6);
                act.AssertIsInvalidWithParms(7);
            }
            catch
            {
                Console.WriteLine("Failed " + name);
                throw;
            }
        }

        [TestMethod]
        public virtual void RangeOnNumericParms()
        {
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
        public virtual void UnsupportedRangeOnProperty()
        {
            ITestObject obj = NewTestObject<Range1>();
            ITestProperty prop = obj.GetPropertyById("Prop24");
            prop.SetValue("-2");
            prop.SetValue("11");
            prop.SetValue("1");
        }

        [TestMethod]
        public virtual void UnsupportedRangeOnParm()
        {
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
    }

    #region Classes used in test

    public class MyDbContext : DbContext
    {
        public const string DatabaseName = "TestAttributes";
        public MyDbContext() : base(DatabaseName) { }

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

        public DbSet<Named1> Named1s { get; set; }
        public DbSet<Range1> Range1s { get; set; }
            public DbSet<Regex1> Regex1s { get; set; }
        public DbSet<Regex2> Regex2s { get; set; }
    }

    #region Default
    public class Default1
    {
        public virtual int Id { get; set; }

        [DefaultValue(8)]
        public virtual int Prop1 { get; set; }

        [DefaultValue("Foo")]
        public virtual string Prop2 { get; set; }


        public virtual void DoSomething([DefaultValue(8)] int param0,
                                [DefaultValue("Foo")] string param1) { }
    }
    #endregion

    #region DescribedAs
    [DescribedAs("Foo")]
    public class Describedas1
    {
        public virtual int Id { get; set; }

        [DescribedAs("Bar")]
        public virtual string Prop1 { get; set; }

        [DescribedAs("Hex")]
        public void DoSomething([DescribedAs("Yop")] string param1) { }
    }

    public class Describedas2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) { }
    }
    #endregion

    #region Description
    [System.ComponentModel.Description("Foo")]
    public class Description1
    {
        public virtual int Id { get; set; }

        [System.ComponentModel.Description("Bar")]
        public virtual string Prop1 { get; set; }

        [System.ComponentModel.Description("Hex")]
        public void DoSomething([System.ComponentModel.Description("Yop")] string param1) { }
    }

    public class Description2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) { }
    }
    #endregion

    #region Disabled
    public class Disabled1
    {
        public Disabled1()
        {
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

        public string DisableProp6()
        {
            if (Prop4 == "Disable 6")
            {
                return "Disabled Message";
            }
            return null;
        }
    }
    #endregion

    #region DisplayName
    [DisplayName("Foo")]
    public class Displayname1
    {

        public virtual int Id { get; set; }

        [DisplayName("Bar")]
        public virtual string Prop1 { get; set; }

        [DisplayName("Hex")]
        public virtual void DoSomething(string param1) { }
    }
    #endregion

    #region Hidden
    public class Hidden1
    {
        public Hidden1()
        {
            // initialise all fields 
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = string.Empty;
        }

        public virtual int Id { get; set; }


        public virtual string Prop0 { get; set; }

        [Hidden]
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
    public class Iconname1
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    [IconName("Foo")]
    public class Iconname2
    {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Iconname3
    {
        public virtual int Id { get; set; }

        public string IconName()
        {
            return "Bar";
        }

        public virtual string Prop1 { get; set; }
    }

    [IconName("Foo")]
    public class Iconname4
    {
        public virtual int Id { get; set; }

        public string IconName()
        {
            return "Bar";
        }

        public virtual string Prop1 { get; set; }
    }
    #endregion

    #region Immutable
    public class Immutable1
    {
        public Immutable1()
        {
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

        public string DisableProp6()
        {
            if (Prop4 == "Disable 6")
            {
                return "Disabled Message";
            }
            return null;
        }
    }

    [Immutable]
    public class Immutable2 : Immutable1
    {
        public void ChangeProp1()
        {
            Prop1 = "Foo";
        }
    }

    [Immutable(WhenTo.OncePersisted)]
    public class Immutable3 : Immutable1 { }
    #endregion

    #region Mask
    public class Mask1
    {

        public virtual int Id { get; set; }

        public virtual DateTime Prop1 { get; set; }

        [NakedObjects.Mask("d")]
        public virtual DateTime Prop2 { get; set; }

        public void DoSomething([NakedObjects.Mask("d")] DateTime d1) { }
    }

    public class Mask2
    {

        public virtual int Id { get; set; }

        [NakedObjects.Mask("c")]
        public virtual decimal Prop1 { get; set; }
    }
    #endregion

#region MaxLength
    public class Maxlength1
    {

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) { }
    }

    public class Maxlength2
    {

        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) { }
    }
#endregion

#region Named
    [Named("Foo")]
    public class Named1
    {

        public virtual int Id { get; set; }

        [Named("Bar")]
        public virtual string Prop1 { get; set; }

        [Named("Hex")]
        public void DoSomething([Named("Yop")] string param1) { }
    }
#endregion

#region Range
    public class Range1
    {
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

        [Range(typeof(string), "1", "10")]
        public virtual int Prop24 { get; set; }

        [Range(-30, 0)]
        public virtual DateTime Prop25 { get; set; }

        [Range(1, 30)]
        public virtual DateTime Prop26 { get; set; }

        public void Action1([Range(5, 6)]sbyte parm) { }
        public void Action2([Range(5, 6)]short parm) { }
        public void Action3([Range(5, 6)]int parm) { }
        public void Action4([Range(5, 6)]long parm) { }
        public void Action5([Range(5, 6)]byte parm) { }
        public void Action6([Range(5, 6)]ushort parm) { }
        public void Action7([Range(5, 6)]uint parm) { }
        public void Action8([Range(5, 6)]ulong parm) { }
        public void Action9([Range(5, 6)]float parm) { }
        public void Action10([Range(5, 6)]double parm) { }
        public void Action11([Range(5, 6)]decimal parm) { }
        public void Action12([Range(5d, 6d)]sbyte parm) { }
        public void Action13([Range(5d, 6d)]short parm) { }
        public void Action14([Range(5d, 6d)]int parm) { }
        public void Action15([Range(5d, 6d)]long parm) { }
        public void Action16([Range(5d, 6d)]byte parm) { }
        public void Action17([Range(5d, 6d)]ushort parm) { }
        public void Action18([Range(5d, 6d)]uint parm) { }
        public void Action19([Range(5d, 6d)]ulong parm) { }
        public void Action20([Range(5d, 6d)]float parm) { }
        public void Action21([Range(5d, 6d)]double parm) { }
        public void Action22([Range(5d, 6d)]decimal parm) { }
        public void Action23([Range(typeof(string), "5", "6")]int parm) { }

        public void Action24([Range(-30, 0)] DateTime parm) { }
        public void Action25([Range(1, 30)] DateTime parm) { }

    }
#endregion

#region Regex
    public class Regex1
    {

        public virtual int Id { get; set; }

        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }

    public class Regex2
    {
        public virtual int Id { get; set; }

        [RegularExpression(@"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }
#endregion
    #endregion
}