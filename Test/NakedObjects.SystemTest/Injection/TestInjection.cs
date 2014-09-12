// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NakedObjects.SystemTest.Injection {
    [TestClass]
    public class TestInjection : AbstractSystemTest {
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

        public class Object1 {
            public IDomainObjectContainer Container { get; set; }
        }

        public class Object2
        {
            public Service1 MyService1 { get; set; }
            public Service2 MyService2 { get; set; }
            public Service3 MyService3 { get; set; }
        }

        public class Object3
        {
            public Service1 MyService1 { get; set; }
            public NotRegisteredService MyService2 { get; set; }

        }



        public class Service1 {}

        public interface Service2 {}

        public interface Service3 { }

        public class NotRegisteredService { }

        public class ServiceImplementation : Service2, Service3 {}


        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                                                              new SimpleRepository<Object1>(),
                                                              new SimpleRepository<Object2>(),
                                                              new Service1(),
                                                              new ServiceImplementation()
                                                          });
            }
        }

        [TestMethod]
        public void InjectContainer() {
            var testObject = (Object1)NewTestObject<Object1>().GetDomainObject();
            Assert.IsNotNull(testObject.Container);
            Assert.IsInstanceOfType(testObject.Container, typeof(DotNetDomainObjectContainer));
        }

        [TestMethod]
        public void InjectService()
        {
            var testObject = (Object2) NewTestObject<Object2>().GetDomainObject();
            Assert.IsNotNull(testObject.MyService1);
            Assert.IsInstanceOfType(testObject.MyService1, typeof(Service1));
        }

        [TestMethod]
        public void InjectServiceDefinedByInterface()
        {
            var testObject = (Object2)NewTestObject<Object2>().GetDomainObject();
            Assert.IsNotNull(testObject.MyService2);
            Assert.IsInstanceOfType(testObject.MyService2,typeof(ServiceImplementation));
            Assert.IsNotNull(testObject.MyService3);
            Assert.IsInstanceOfType(testObject.MyService3, typeof(ServiceImplementation));
        }

        [TestMethod]
        public void InjectedPropertiesAreHidden()
        {
            var obj = NewTestObject<Object2>();
            try
            {
                obj.GetPropertyByName("My Service1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
            }

            var prop = obj.GetPropertyByName("My Service2");
            prop.AssertIsVisible();
        }


    }
}