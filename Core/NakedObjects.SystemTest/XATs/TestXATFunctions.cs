// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Menu;
using NakedObjects.Services;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedObjects.SystemTest.XATs {
    /// <summary>
    ///     Tests various functions of the XATs themselves
    /// </summary>
    [TestFixture]
    public class TestXATFunctions : AbstractSystemTest<XatDbContext> {
        #region TestEnum enum

        public enum TestEnum {
            Value1,
            Value2
        }

        #endregion

        protected override string[] Namespaces => new[] {typeof(Object1).Namespace};

        protected override Type[] Services => new[] {typeof(SimpleRepository<Object1>), typeof(MyService1), typeof(MyService2)};

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            XatDbContext.Delete();
            var context = Activator.CreateInstance<XatDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            XatDbContext.Delete();
        }

        [Test]
        public virtual void IncorrectTitle() {
            var obj1 = NewTestObject<Object1>();

            try {
                obj1.AssertTitleEquals("Yoda");
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.IsTrue failed. Expected title 'Yoda' but got 'FooBar'", e.Message);
            }
        }

        [Test]
        public virtual void InvokeActionWithIncorrectParams() {
            var obj1 = NewTestObject<Object1>();
            try {
                obj1.GetAction("Do Something").InvokeReturnObject(1, 2);
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.Fail failed. Invalid Argument(s)", e.Message);
            }
        }

        [Test]
        public virtual void TestActionAssertHasFriendlyName() {
            var obj = NewTestObject<Object1>();
            obj.GetActionById("ActionNumber1").AssertHasFriendlyName("Action Number1");
            obj.GetActionById("ActionNumber2").AssertHasFriendlyName("Action Two");
        }

        [Test]
        public virtual void TestEnumDefault() {
            var obj = NewTestObject<Object1>();

            var a1 = obj.GetAction("Do Something Else");

            var p1 = a1.Parameters.First();
            var p2 = a1.Parameters.Last();

            var def1 = p1.GetDefault();
            var def2 = p2.GetDefault();

            Assert.AreEqual(TestEnum.Value2, def1.NakedObject.Object);
            Assert.AreEqual(null, def2);
        }

        [Test]
        public virtual void TestGetAction() {
            var obj = NewTestObject<Object1>();

            var a1 = obj.GetAction("Action Number1");
            Assert.IsNotNull(a1);
            var a2 = obj.GetAction("Action Two");
            Assert.IsNotNull(a2);

            try {
                obj.GetAction("ActionNumber1");
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'ActionNumber1'", e.Message);
            }

            //Now with params
            var a3 = obj.GetAction("Action Number3", typeof(string), typeof(int));
            Assert.IsNotNull(a3);
            a3 = obj.GetAction("Action Number3"); //Params not necessary
            Assert.IsNotNull(a3);
            //And with wrong param types
            try {
                obj.GetAction("Action Number3", typeof(int), typeof(string)); //wrong way round!
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Action Number3' (with specified parameters)", e.Message);
            }

            //Now from sub-menu, with & without params
            obj.GetAction("Action Number4", "Sub1");
            obj.GetAction("Action Number4", "Sub1", typeof(string), typeof(int));
            obj.GetAction("Action Number4"); //works without specifying sub-menu
            obj.GetAction("Action Number4", typeof(string), typeof(int));
            //With wrong sub-menu
            try {
                obj.GetAction("Action Number4", "Sub2", typeof(string), typeof(int));
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.IsNotNull failed. No menu item with name: Sub2", e.Message);
            }

            //With right sub-menu & wrong params
            try {
                obj.GetAction("Action Number4", "Sub1", typeof(int), typeof(string));
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.IsTrue failed. Parameter Types do not match for action: Action Number4", e.Message);
            }
        }

        [Test]
        public virtual void TestGetActionById() {
            var obj = NewTestObject<Object1>();

            var a1 = obj.GetActionById("ActionNumber1");
            Assert.IsNotNull(a1);
            var a2 = obj.GetActionById("ActionNumber2");
            Assert.IsNotNull(a2);
            try {
                obj.GetActionById("Action Number1");
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Action Number1' (as method name)", e.Message);
            }

            //Now with params
            var a3 = obj.GetActionById("ActionNumber3", typeof(string), typeof(int));
            Assert.IsNotNull(a3);
            a3 = obj.GetActionById("ActionNumber3"); //Params not necessary
            Assert.IsNotNull(a3);

            //And with wrong param types
            try {
                obj.GetActionById("ActionNumber3", typeof(int), typeof(string)); //wrong way round!
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'ActionNumber3' (as method name & with specified parameters)", e.Message);
            }

            //Now from sub-menu, with & without params
            obj.GetActionById("ActionNumber4", "Sub1");
            obj.GetActionById("ActionNumber4", "Sub1", typeof(string), typeof(int));
            obj.GetActionById("ActionNumber4"); //works without specifying sub-menu
            obj.GetActionById("ActionNumber4", typeof(string), typeof(int));
            //With wrong sub-menu
            try {
                obj.GetActionById("ActionNumber4", "Sub2", typeof(string), typeof(int));
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.IsNotNull failed. No menu item with name: Sub2", e.Message);
            }

            //With right sub-menu & wrong params
            try {
                obj.GetActionById("ActionNumber4", "Sub1", typeof(int), typeof(string));
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.IsTrue failed. Parameter Types do not match for action with method name: ActionNumber4", e.Message);
            }
        }

        [Test]
        public virtual void TestGetProperty() {
            var culture = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var obj = NewTestObject<Object1>();

                obj.GetPropertyByName("Prop1");
                obj.GetPropertyByName("Bar");

                obj.GetPropertyById("Prop1");
                obj.GetPropertyById("Prop2");

                try {
                    obj.GetPropertyById("Bar");
                    Assert.Fail("Shouldn't get to here!");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. No Property with Id 'Bar'", e.Message);
                }

                try {
                    obj.GetPropertyByName("Prop4");
                    Assert.Fail("Shouldn't get to here!");
                }
                catch (Exception e) {
                    Assert.AreEqual("Assert.Fail failed. No Property named 'Prop4'", e.Message);
                }
            }
            finally {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [Test]
        public virtual void TestGetTestService() {
            GetTestService("My Service1");
            GetTestService("Service Two");
            //Get nonexistent service
            try {
                GetTestService("My Service3");
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.Fail failed. No such service: My Service3", e.Message);
            }

            //Get service by real name that has been overriden
            try {
                GetTestService("MyService2");
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.Fail failed. No such service: MyService2", e.Message);
            }

            GetTestService(typeof(MyService1));
            GetTestService<MyService2>();

            //Get nonexistent service
            try {
                GetTestService<MyService3>();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.Fail failed. No service of type NakedObjects.SystemTest.XATs.TestXATFunctions+MyService3", e.Message);
            }
        }

        [Test]
        public virtual void TestPropertyValue() {
            var culture = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var obj = NewTestObject<Object1>();

                obj.GetPropertyById("Prop3").SetValue("08/16/2013");

                var p1 = obj.GetPropertyById("Prop3");

                p1.AssertValueIsEqual("08/16/2013 00:00:00");
                p1.AssertTitleIsEqual("08/16/2013");
            }
            finally {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [Test]
        public virtual void TestReturnString() {
            var obj = NewTestObject<Object1>();

            var a1 = obj.GetAction("Do Return String");

            var res = a1.InvokeReturnObject();

            Assert.AreEqual("a string", res.NakedObject.Object);

            a1.Invoke();
        }

        [Test]
        public virtual void TestTooFewParms() {
            var obj = NewTestObject<Object1>();

            var a1 = obj.GetAction("Do Something");

            try {
                var res = a1.InvokeReturnObject();
                Assert.Fail("expect exception");
            }
            catch (Exception expected) {
                Assert.AreEqual("Assert.IsTrue failed. Action 'Do Something' is unusable: wrong number of parameters, got 0, expect 2", expected.Message);
            }
        }

        [Test]
        public virtual void VisiblePropertyHasSameNameAsHiddenProperty() {
            var obj1 = NewTestObject<Object1>();
            try {
                var foo = obj1.GetPropertyByName("Foo");
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                IsInstanceOfType(e, typeof(AssertFailedException));
                Assert.AreEqual("Assert.Fail failed. More than one Property named 'Foo'", e.Message);
            }
        }

        #region Nested type: MyService1

        public class MyService1 { }

        #endregion

        #region Nested type: MyService2

        [DisplayName("Service Two")]
        public class MyService2 { }

        #endregion

        #region Nested type: MyService3

        //Not registered as a service
        public class MyService3 { }

        #endregion

        #region Nested type: Object1

        public class Object1 {
            [DefaultValue(8)]
            public int Prop1 { get; set; }

            [DisplayName("Foo")]
            public string Prop2 { get; set; }

            [Mask("d")]
            public DateTime Prop3 { get; set; } = new DateTime(2013, 8, 16);

            [DisplayName("Bar")]
            public string Prop4 { get; set; }

            [Hidden(WhenTo.Always)]
            public string Foo { get; set; }

            public IDomainObjectContainer Container { set; protected get; }

            public static void Menu(IMenu menu) {
                menu.CreateSubMenu("Sub1").AddAction("ActionNumber4");
            }

            public string Title() {
                var t = Container.NewTitleBuilder();
                t.Append("FooBar");
                return t.ToString();
            }

            public Object1 DoSomething([DefaultValue(8)] int param0, [DefaultValue("Foo")] string param1) => null;

            public Object1 DoSomethingElse([DefaultValue(TestEnum.Value2)] TestEnum param0, TestEnum param1) => null;

            public string DoReturnString() => "a string";

            public void ActionNumber1() { }

            [DisplayName("Action Two")]
            public void ActionNumber2() { }

            public void ActionNumber3(string p1, int p2) { }

            public void ActionNumber4(string p1, int p2) { }
        }

        #endregion
    }

    #region Classes used by tests

    public class XatDbContext : DbContext {
        public const string DatabaseName = "TestXats";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public XatDbContext() : base(Cs) { }

        public static void Delete() => Database.Delete(Cs);
    }

    #endregion
}