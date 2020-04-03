//using NakedObjects.Boot;
//using NakedObjects.Core.NakedObjectsSystem;
//using NakedObjects.Services;
//using NakedObjects.Xat;
//using NUnit.Framework;

//namespace NakedObjects.SystemTest.i18n {

//        [TestFixture]
//        public class TestI18N : AbstractSystemTest {
//            #region Setup/Teardown

//            [SetUp]
//            public void SetupTest() {
//                SetUpNakedObjectsFramework();
//                base.StartMethodProfiling();
//            }

//            [TearDown]
//            public void TearDownTest() {
//                TearDownNakedObjectsFramework();
//                base.StopMethodProfiling();
//            }

//            #endregion


//            #region "Run configuration"
//            protected override IFixturesInstaller Fixtures {
//                get { return new FixturesInstaller(new object[] {}); }
//            }

//            protected override IServicesInstaller Services {
//                get {
//                    return new ServicesInstaller(new object[] {
//                                                                  new SimpleRepository<Object1>()
//                                                              });
//                }
//            }

//            protected override string Culture
//            {
//                get
//                {
//                    return "fr";
//                }
//            }

//            protected override string LocalizedResourceNamespace
//            {
//                get
//                {
//                    return "NakedObjects.SystemTest";
//                }
//            }

//            #endregion


//            [Test]
//            public void PropertyNameLocalised()
//            {
//                var obj = NewTestObject<Object1>();
//                var prop1 = obj.GetPropertyById("Property1");
//                StringAssert.IsMatch("Foo", prop1.Name);
//                var prop2 = obj.GetPropertyById("Property2");
//                StringAssert.IsMatch("Bar", prop2.Name);
//            }
//        }

//    public class Object1
//    {
//        public virtual string Property1 { get; set; }

//        public virtual string Property2 { get; set; }
//    }
//} //end of root namespace