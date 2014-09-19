// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace MemberOrder {
        [TestClass, Ignore]
        public class TestMemberOrderAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
            }

            #endregion

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[]   {  new SimpleRepository<MemberOrder1>(),
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>()
                                                              });
                }
            }

            [TestMethod]
            public void PropertyOrder()
            {
                ITestObject obj2 = NewTestObject<MemberOrder1>();
                obj2.AssertPropertyOrderIs("Prop2, Prop1");

                ITestProperty[] properties = obj2.Properties;
                Assert.AreEqual(properties[0].Name, "Prop2");
                Assert.AreEqual(properties[1].Name, "Prop1");
            }

            [TestMethod]
            public void PropertyOrderOnSubClass() {
                ITestObject obj3 = NewTestObject<Object3>();
                obj3.AssertPropertyOrderIs("Prop2, Prop4, Prop1, Prop3");

                ITestProperty[] properties = obj3.Properties;
                Assert.AreEqual(properties[0].Name, "Prop2");
                Assert.AreEqual(properties[1].Name, "Prop4");
                Assert.AreEqual(properties[2].Name, "Prop1");
                Assert.AreEqual(properties[3].Name, "Prop3");
            }

            [TestMethod]
            public void ActionOrder()
            {
                ITestObject obj2 = NewTestObject<MemberOrder1>();
                ITestAction[] actions = obj2.Actions;
                Assert.AreEqual(actions[0].Name, "Action2");
                Assert.AreEqual(actions[1].Name, "Action1");

                obj2.AssertActionOrderIs("Action2, Action1");
            }

            [TestMethod]
            public void ActionOrderOnSubClass()
            {
                ITestObject obj3 = NewTestObject<Object3>();
                ITestAction[] actions = obj3.Actions;
                Assert.AreEqual(actions[0].Name, "Action2");
                Assert.AreEqual(actions[1].Name, "Action4");
                Assert.AreEqual(actions[2].Name, "Action1");
                Assert.AreEqual(actions[3].Name, "Action3");

                obj3.AssertActionOrderIs("Action2, Action4, Action1, Action3");
            }

            [TestMethod]
            public void ActionOrderWithSubMenus()
            {
                ITestObject obj4 = NewTestObject<Object4>();
                obj4.AssertActionOrderIs("Action1, Action4, (Sub1:Action3, Action2), (Sub2:Action5)");

                ITestAction[] actions = obj4.Actions;
                Assert.AreEqual(actions[0].Name, "Action1");
                Assert.AreEqual(actions[1].Name, "Action4");
                Assert.AreEqual(actions[2].Name, "Action3");
                Assert.AreEqual(actions[3].Name, "Action2");
                Assert.AreEqual(actions[4].Name, "Action5");

                obj4.GetAction("Action1").AssertIsVisible();
                obj4.GetAction("Action4").AssertIsVisible();
                obj4.GetAction("Action3", "Sub1").AssertIsVisible();
                obj4.GetAction("Action2", "Sub1").AssertIsVisible();
                obj4.GetAction("Action5", "Sub2").AssertIsVisible();

                try
                {
                    obj4.GetAction("Action1", "Sub1").AssertIsVisible();
                }
                catch (System.Exception e)
                {
                    Assert.AreEqual("Assert.Fail failed. No Action named 'Action1' within sub-menu 'Sub1'", e.Message);
                }
                
            }
        }

        public class MemberOrder1 {
            [MemberOrder(3)]
            public string Prop1 { get; set; }

            [MemberOrder(1)]
            public string Prop2 { get; set; }

            [MemberOrder(3)]
            public void Action1() { }

            [MemberOrder(1)]
            public void Action2() { }
        }

        public class Object3 : MemberOrder1 {
            [MemberOrder(4)]
            public string Prop3 { get; set; }

            [MemberOrder(2)]
            public string Prop4 { get; set; }

            [MemberOrder(4)]
            public void Action3() { }

            [MemberOrder(2)]
            public void Action4() { }
        }

        public class Object4
        {
            [MemberOrder(1)]
            public void Action1() { }

            [MemberOrder(Name = "Sub2", Sequence = "1")]
            public void Action5() { }

            [MemberOrder(Name="Sub1", Sequence="2")]
            public void Action2() { }

            [MemberOrder(Name = "Sub1", Sequence = "1")]
            public void Action3() { }

            [MemberOrder(2)]
            public void Action4() { } 


        }
    }
}