// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace AddTo {
        [TestClass]
        public class TestAddToMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework();
                //Set up any common variables here
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework();
                //Tear down any common variables here
            }

            #endregion

            #region "Services & Fixtures"
            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>()
                                                              });
                }
            }

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }
#endregion

            [TestMethod]
            public void AddToMethodDoesShowUpAsAnAction() {
                ITestObject obj1 = NewTestObject<Object1>();
                ITestAction action = obj1.GetAction("Add To Coll1");
                Assert.IsNotNull(action);
            }

            [TestMethod, Ignore] // since collections made desiabled by default
            public void AddToWithWrongArgumentTypeShouldNotBeCalled() {
                ITestObject obj2 = NewTestObject<Object2>();
                ITestObject obj4 = NewTestObject<Object4>();

                var dom4 = (Object4) obj4.GetDomainObject();
                Assert.IsFalse(dom4.AddToCalled);

                ITestProperty coll1 = obj4.GetPropertyByName("Coll1");
                coll1.SetObject(obj2);
                Assert.IsFalse(dom4.AddToCalled);
            }

            [TestMethod]
            public void UnmatchedModifyMethodShowsUpAsAnAction() {
                ITestObject obj3 = NewTestObject<Object3>();

                obj3.GetAction("Add To Coll2");
                obj3.GetAction("Add To Coll3");
                obj3.GetAction("Add To Coll4");
            }
        }

        public class Object1 {
            private List<Object2> _Coll1 = new List<Object2>();
            public bool AddToCalled;

            public List<Object2> Coll1 {
                get { return _Coll1; }
                set { _Coll1 = value; }
            }

            public void AddToColl1(Object2 value) {
                Coll1.Add(value);
                AddToCalled = true;
            }
        }

        public class Object2 {
            public string Prop1 { get; set; }
        }

        public class Object3 {
            private List<Object2> _Coll1 = new List<Object2>();

            private List<Object2> _Coll2 = new List<Object2>();
            public bool AddToCalled;

            public List<Object2> Coll1 {
                get { return _Coll1; }
                set { _Coll1 = value; }
            }

            public List<Object2> Coll2 {
                get { return _Coll2; }
                set { _Coll2 = value; }
            }

            //Lower case
            public void addToColl2(Object1 value) {}

            //No matching collection
            public void AddToColl3(Object2 value) {}

            //Non void method
            public bool AddToColl4(Object2 value) {
                return true;
            }
        }

        public class Object4 {
            private List<Object2> _Coll1 = new List<Object2>();
            public bool AddToCalled;

            public List<Object2> Coll1 {
                get { return _Coll1; }
                set { _Coll1 = value; }
            }

            public void AddToColl1(Object1 value) {
                AddToCalled = true;
            }
        }
    }
}