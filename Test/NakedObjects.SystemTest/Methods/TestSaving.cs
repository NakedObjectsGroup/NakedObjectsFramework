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
    namespace Saving {
        [TestClass]
        public class TestSaving : AbstractSystemTest {
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
                Assert.IsFalse(dom1.SavingCalled);

                try {
                    obj1.GetAction("saving");
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void SavingCalled() {
                ITestObject obj1 = NewTestObject<Object1>();
                var dom1 = (Object1) obj1.GetDomainObject();
                Assert.IsFalse(dom1.SavingCalled);

                obj1.Save();

                Assert.IsTrue(dom1.SavingCalled);
            }

            [TestMethod]
            public void SavingDoesNotShowUpAsAnAction() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    obj1.GetAction("Saving");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }
        }

        public class Object1 {
            public bool SavingCalled;

            [Optionally]
            public string Prop1 { get; set; }

            public void Saving() {
                SavingCalled = true;
            }
        }

        public class Object2 {
            public bool SavingCalled;

            public string Prop1 { get; set; }

            public void saving() {
                SavingCalled = true;
            }
        }
    }
}