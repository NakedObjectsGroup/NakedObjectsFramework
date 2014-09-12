// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Persisted {
        [TestClass]
        public class TestPersisted : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
                //Set up any common variables here
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                //Tear down any common variables here
            }

            #endregion

            #region "Services & Fixtures"
            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>(), new SimpleRepository<Object2>()}); }
            }

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }
            #endregion

            [TestMethod]
            public void LowerCaseNotRecognisedAndShowsAsAction() {
                ITestObject obj1 = NewTestObject<Object2>();
                var dom1 = (Object2) obj1.GetDomainObject();
                Assert.IsFalse(dom1.PersistedCalled);

                try {
                    obj1.GetAction("persisted");
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void PersistedCalled() {
                ITestObject obj1 = NewTestObject<Object1>();
                var dom1 = (Object1) obj1.GetDomainObject();
                Assert.IsFalse(dom1.PersistedCalled);

                obj1.Save();

                Assert.IsTrue(dom1.PersistedCalled);
            }

            [TestMethod]
            public void PersistedDoesNotShowUpAsAnAction() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    obj1.GetAction("Persisted");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }
        }

        public class Object1 {
            public bool PersistedCalled;

            [Optionally]
            public string Prop1 { get; set; }

            public void Persisted() {
                PersistedCalled = true;
            }
        }

        public class Object2 {
            public bool PersistedCalled;

            public string Prop1 { get; set; }

            public void peristed() {
                PersistedCalled = true;
            }
        }
    }
}