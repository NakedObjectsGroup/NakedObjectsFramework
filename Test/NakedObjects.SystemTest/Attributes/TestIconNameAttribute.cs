// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Icon {
        [TestClass]
        public class TestIconNameAttribute : AbstractSystemTest {
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
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {
                          new SimpleRepository<Object1>(),
                          new SimpleRepository<Object2>(),
                          new SimpleRepository<Object3>(),
                          new SimpleRepository<Object4>()
                          }); }
            }
#endregion

            [TestMethod]
            public virtual void NoIconSpecified()
            {
                var obj = NewTestObject<Object1>();
                Assert.AreEqual("Default", obj.IconName);
            }

            [TestMethod]
            public virtual void IconNamedAttribute()
            {
                var obj = NewTestObject<Object2>();
                Assert.AreEqual("Foo", obj.IconName);
            }

            [TestMethod]
            public virtual void IconNameMethod()
            {
                var obj = NewTestObject<Object3>();
                Assert.AreEqual("Bar", obj.IconName);
            }

            [TestMethod]
            public virtual void MethodOverridesAttribute()
            {
                var obj = NewTestObject<Object4>();
                Assert.AreEqual("Bar", obj.IconName);
            }


        }

        #region Classes used by test

        public class Object1 {
            
            public string Prop1 { get; set; }


        }

        [IconName("Foo")]
        public class Object2
        {

            public string Prop1 { get; set; }


        }

        public class Object3
        {
            public string IconName()
            {
                return "Bar";
            }

            public string Prop1 { get; set; }
        }

        [IconName("Foo")]
        public class Object4
        {
            public string IconName()
            {
                return "Bar";
            }

            public string Prop1 { get; set; }


        }
        #endregion
    }
} //end of root namespace