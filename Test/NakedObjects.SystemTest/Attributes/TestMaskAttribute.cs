// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace MaskAttribute {
        [TestClass]
        public class TestMaskAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
                obj = null;
                prop1 = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>()
                                                              });
                }
            }
            #endregion

            private ITestObject obj;
            private ITestProperty prop1;
            private ITestProperty prop2;

            [TestMethod]
            public virtual void CMaskOnDecimalProperty() {
                obj = NewTestObject<Object2>();
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("32.70");
                var dom = (Object2) obj.GetDomainObject();
                StringAssert.Equals("32.7", dom.Prop1.ToString());
                StringAssert.Equals("32.70", prop1.Content.Title);
                StringAssert.Equals("£32.70", prop1.Title);
                prop1.AssertTitleIsEqual("£32.70");
                prop1.AssertValueIsEqual("32.70");
            }

            [TestMethod]
            public virtual void DMaskOnDateProperty() {
                obj = NewTestObject<Object1>();
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("23/09/2009 11:34:50");
                prop2 = obj.GetPropertyByName("Prop2");
                prop2.SetValue("23/09/2009 11:34:50");
                var dom = (Object1) obj.GetDomainObject();
                StringAssert.Equals("23/09/2009 11:34:50", dom.Prop1.ToString());
                StringAssert.Equals("23/09/2009 11:34:50", prop1.Content.Title);
                StringAssert.Equals("23/09/2009 11:34:50", dom.Prop2.ToString());
                StringAssert.Equals("23/09/2009", prop2.Content.Title);
                prop1.AssertTitleIsEqual("23/09/2009 11:34:50");
                prop1.AssertValueIsEqual("23/09/2009 11:34:50");
                prop2.AssertTitleIsEqual("23/09/2009");
                prop2.AssertValueIsEqual("23/09/2009 11:34:50");
            }
        }

        public class Object1 {
            public DateTime Prop1 { get; set; }

            [NakedObjects.Mask("d")]
            public DateTime Prop2 { get; set; }

            public void DoSomething([NakedObjects.Mask("d")] DateTime d1) {}
        }

        public class Object2 {
            [NakedObjects.Mask("c")]
            public decimal Prop1 { get; set; }
        }
    }
} //end of root namespace