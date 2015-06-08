// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.XATs {


    /// <summary>
    ///     Tests various functions of the XATs themselves
    /// </summary>
    [TestClass]
    public class TestFailureScenarios : AbstractSystemTest<XatDbContext> {
        #region Setup/Teardown


        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestFailureScenarios());
            Database.Delete(XatDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        #region TestEnum enum

        public enum TestEnum {
            Value1,
            Value2
        };

        #endregion

        protected override string[] Namespaces {
            get { return new[] { typeof(Object1).Namespace }; }
        }

        protected override object[] MenuServices {
            get { return (new object[] {new SimpleRepository<Object1>()}); }
        }

        protected override object[] Fixtures {
            get { return base.Fixtures; }
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

            obj.GetPropertyById("Prop3").SetValue("16/08/2013");

            ITestProperty p1 = obj.GetPropertyById("Prop3");

            

            p1.AssertValueIsEqual("16/08/2013 00:00:00");
            p1.AssertTitleIsEqual("16/08/2013");
        }

        #region Nested type: Object1

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

#pragma warning disable 618
            [Hidden]
#pragma warning restore 618
            public string Foo { get; set; }

            public IDomainObjectContainer Container { set; protected get; }

            public string Title() {
                var t = Container.NewTitleBuilder();
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

        #endregion
    }

    #region Classes used by tests

    public class XatDbContext : DbContext {
        public const string DatabaseName = "TestXats";
        public XatDbContext() : base(DatabaseName) {}

      
    }

    #endregion
}