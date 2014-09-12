// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace DefaultValue {
        [TestClass]
        public class TestDefaultValueAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework(this);
                
                obj1 = NewTestObject<Object1>();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework(this);
                
                obj1 = null;
            }

            #endregion

            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] { }); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] { new SimpleRepository<Object1>() }); }
            }

            private ITestObject obj1;

            public class Object1 {
                [DefaultValue(8)]
                public int Prop1 { get; set; }

                [DefaultValue("Foo")]
                public string Prop2 { get; set; }

            
                public void DoSomething([DefaultValue(8)] int param0,
                                        [DefaultValue("Foo")] string param1) { }
            }

            [TestMethod]
            public virtual void DefaultNumericProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop1");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("8", def);
            }

            [TestMethod]
            public void DefaultParameters() {
                ITestAction action = obj1.GetAction("Do Something");
                string def0 = action.Parameters[0].GetDefault().Title;
                Assert.IsNotNull(def0);
                Assert.AreEqual("8", def0);

                string def1 = action.Parameters[1].GetDefault().Title;
                Assert.IsNotNull(def1);
                Assert.AreEqual("Foo", def1);       
            }

            [TestMethod]
            public virtual void DefaultStringProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop2");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("Foo", def);
            }
        }
    }
}
//end of root namespace