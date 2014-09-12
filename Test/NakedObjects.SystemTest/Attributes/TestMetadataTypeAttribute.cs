// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel.DataAnnotations;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace MetadataType {
        [TestClass]
        public class TestMetadataTypeAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
                obj = NewTestObject<Object1>();
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                obj = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object3>()
                                                              }); }
            }

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }
#endregion

            private ITestObject obj;

            [TestMethod]
            public virtual void PropertyNotIncludedInBuddyClass() {
                obj.GetPropertyByName("Prop5");
            }

            [TestMethod]
            public virtual void PropertyRenamedExternally() {
                obj.GetPropertyByName("Foo");
            }

            [TestMethod]
            public virtual void PropertyWithExternalMetadataOnly() {
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertIsVisible();
                prop2.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void PropertyWithInternalAndExternalMetadata() {
                ITestProperty prop3 = obj.GetPropertyByName("Prop3");
                prop3.AssertIsOptional();
                prop3.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void PropertyWithInternalButNoExternalMetadata() {
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertIsInvisible();
            }

            [TestMethod]
            public virtual void TitleAttributePickedUp() {
                var obj1 = NewTestObject<Object1>();
                ITestProperty prop6 = obj1.GetPropertyByName("Prop6");
                prop6.SetValue("FooBar");
                obj1.AssertTitleEquals("FooBar");

            }

            // this test need .net 4 to run 
            [TestMethod]
            public void MemberOrderPickupOnSubClass() {
                var obj3 = NewTestObject<Object3>();
                var properties = obj3.Properties;
                Assert.AreEqual(properties[0].Id, "Prop8");
                Assert.AreEqual(properties[1].Id, "Prop10");
                Assert.AreEqual(properties[2].Id, "Prop7");
                Assert.AreEqual(properties[3].Id, "Prop9");

            }
        }

        public partial class Object1 {
            [Hidden]
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            [Optionally]
            public string Prop3 { get; set; }

            public string Prop4 { get; set; }

            public string Prop5 { get; set; }

            public string Prop6 { get; set; }
        }

        [MetadataType(typeof (Object1_Metadata))]
        public partial class Object1 {}

        public class Object1_Metadata {
            public string Prop1 { get; set; }

            [Disabled]
            public string Prop2 { get; set; }

            [Disabled]
            public string Prop3 { get; set; }

            [Named("Foo")]
            public string Prop4 { get; set; }

            [Title]
            public string Prop6 { get; set; }
        }

        public partial class Object2 {
            public string Prop7 { get; set; }
            public string Prop8 { get; set; }
        }

        [MetadataType(typeof(Object2_Metadata))]
        public partial class Object2{}

        public class Object2_Metadata {
            [MemberOrder("3")]
            public string Prop7 { get; set; }
            [MemberOrder("1")]
            public string Prop8 { get; set; }
        }

        public partial class Object3 : Object2{
            public string Prop9 { get; set; }
            public string Prop10 { get; set; }
        }

        [MetadataType(typeof(Object3_Metadata))]
        public partial class Object3 { }

        public class Object3_Metadata  {
            [MemberOrder("4")]
            public string Prop9 { get; set; }
            [MemberOrder("2")]
            public string Prop10 { get; set; }
        }


    }
}