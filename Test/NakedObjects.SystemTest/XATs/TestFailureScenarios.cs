// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using System.Data.Entity;

namespace NakedObjects.SystemTest.XATs {
        /// <summary>
        ///     Tests various functions of the XATs themselves
        /// </summary>
        [TestClass]
        public class TestFailureScenarios : AbstractSystemTest2<XatDbContext> {
            #region Setup/Teardown
            [ClassInitialize]
            public static void ClassInitialize(TestContext tc)
            {
                InitializeNakedObjectsFramework(new TestFailureScenarios());
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
                CleanupNakedObjectsFramework(new TestFailureScenarios());
                Database.Delete(XatDbContext.DatabaseName);
            }

            [TestInitialize()]
            public void TestInitialize()
            {
                StartTest();
            }

            [TestCleanup()]
            public void TestCleanup()
            {
            }

            #endregion

            public enum TestEnum {
                Value1,
                Value2
            };

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>()}); }
            }

            [TestMethod]
            public virtual void AttemptToGetANonExistantService() {
                try {
                    GetTestService("AwolService");
                    Assert.Fail("Should not get to here");
                }
                catch (Exception e) {
                    Assert.IsInstanceOfType(e, typeof (AssertFailedException));
                    Assert.AreEqual("Assert.Fail failed. No such service: AwolService", e.Message);
                }
            }

            [TestMethod]
            public virtual void InvokeActionWithIncorrectParams() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    obj1.GetAction("Do Something").InvokeReturnObject(1, 2);
                    Assert.Fail("Should not get to here");
                }
                catch (Exception e) {
                    Assert.IsInstanceOfType(e, typeof (AssertFailedException));
                    Assert.AreEqual("Assert.Fail failed. Invalid Argument(s)", e.Message);
                }
            }


            [TestMethod]
            public virtual void IncorrectTitle() {
                ITestObject obj1 = NewTestObject<Object1>();

                try {
                    obj1.AssertTitleEquals("Yoda");
                    Assert.Fail("Should not get to here");
                }
                catch (Exception e) {
                    Assert.IsInstanceOfType(e, typeof (AssertFailedException));
                    Assert.AreEqual("Assert.IsTrue failed. Expected title 'Yoda' but got 'FooBar'", e.Message);
                }
            }

            [TestMethod]
            public virtual void VisiblePropertyHasSameNameAsHiddenProperty() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    ITestProperty foo = obj1.GetPropertyByName("Foo");
                    Assert.Fail("Should not get to here");
                }
                catch (Exception e) {
                    Assert.IsInstanceOfType(e, typeof (AssertFailedException));
                    Assert.AreEqual("Assert.Fail failed. More than one Property named 'Foo'", e.Message);
                }
            }

            [TestMethod]
            public virtual void TestEnumDefault() {
                ITestObject obj = NewTestObject<Object1>();

                ITestAction a1 = obj.GetAction("Do Something Else");

                ITestParameter p1 = a1.Parameters.First();
                ITestParameter p2 = a1.Parameters.Last();

                ITestNaked def1 = p1.GetDefault();
                ITestNaked def2 = p2.GetDefault();

                Assert.AreEqual(TestEnum.Value2, def1.NakedObject.Object);
                Assert.AreEqual(null, def2);
            }

            [TestMethod]
            public virtual void TestReturnString() {
                ITestObject obj = NewTestObject<Object1>();

                ITestAction a1 = obj.GetAction("Do Return String");

                ITestObject res = a1.InvokeReturnObject();

                Assert.AreEqual("a string", res.NakedObject.Object);

                a1.Invoke();
            }

            [TestMethod]
            public virtual void TestTooFewParms() {
                ITestObject obj = NewTestObject<Object1>();

                ITestAction a1 = obj.GetAction("Do Something");

                try {
                    ITestObject res = a1.InvokeReturnObject();
                    Assert.Fail("expect exception");
                }
                catch (Exception expected) {
                    Assert.AreEqual("Assert.IsTrue failed. Action 'Do Something' is unusable: wrong number of parameters, got 0, expect 2", expected.Message);
                }
            }

            [TestMethod]
            public virtual void TestPropertyValue() {
                ITestObject obj = NewTestObject<Object1>();

                ITestProperty p1 = obj.GetPropertyById("Prop3");

                p1.AssertValueIsEqual("16/08/2013 00:00:00");
                p1.AssertTitleIsEqual("16/08/2013");
            }

            public class Object1 {
                private DateTime prop3 = new DateTime(2013, 8, 16);


                [DefaultValue(8)]
                public int Prop1 { get; set; }

                [DisplayName("Foo")]
                public string Prop2 { get; set; }


                [Mask("d")]
                public DateTime Prop3 {
                    get { return prop3; }
                    set { prop3 = value; }
                }

                [Hidden]
                public string Foo { get; set; }

                public string Title() {
                    var t = new TitleBuilder();
                    t.Append("FooBar");
                    return t.ToString();
                }


                public Object1 DoSomething([DefaultValue(8)] int param0, [DefaultValue("Foo")] string param1) {
                    return null;
                }

                public Object1 DoSomethingElse([DefaultValue(TestEnum.Value2)] TestEnum param0, TestEnum param1) {
                    return null;
                }

                public string DoReturnString() {
                    return "a string";
                }
            }
        }

#region Classes used by tests
        public class XatDbContext : DbContext
    {
        public const string DatabaseName = "TestXats";
        public XatDbContext() : base(DatabaseName) { }

    }
#endregion
}