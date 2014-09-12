// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using NakedObjects.Xat;

namespace ValidateProgrammaticUpdates {
     
    public class TestService {
        public IDomainObjectContainer Container { set; protected get; }

        public void SaveObject1(Object1 obj) {
            Container.Persist(ref obj);
        }

        public void SaveObject2(Object2 obj) {
            Container.Persist(ref obj);
        }
    }

    [TestClass]
    public class TestValidateProgrammaticUpdatesAttribute : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            
            obj1 = NewTestObject<Object1>();
            obj2 = NewTestObject<Object2>();
        }

        [TestCleanup]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            
            obj1 = null;
            obj2 = null;
        }

        #endregion

        #region "Services & Fixtures"

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] { new SimpleRepository<Object1>(), new SimpleRepository<Object2>(), new TestService() }); }
        }

        #endregion

        private ITestObject obj1;
        private ITestObject obj2;

        [TestMethod]
        public virtual void ValidateObjectSave() {
            ITestService service = GetTestService(typeof (TestService));

            try {
                (obj1.GetDomainObject() as Object1).Prop1 = "fail";

                service.GetAction("Save Object1").InvokeReturnObject(obj1);
                Assert.Fail();
            }
            catch (Exception /*expected*/) { }
        }

        [TestMethod]
        public virtual void ValidateObjectCrossValidationSave() {
            ITestService service = GetTestService(typeof(TestService));

            try {
                (obj2.GetDomainObject() as Object2).Prop1 = "fail";
                service.GetAction("Save Object2").InvokeReturnObject(obj2);
                Assert.Fail();
            }
            catch (Exception /*expected*/) { }
        }

        [TestMethod]
        public virtual void ValidateObjectChange() {
            ITestService service = GetTestService(typeof(TestService));

            service.GetAction("Save Object1").InvokeReturnObject(obj1);

            try {
                (obj1.GetDomainObject() as Object1).Prop1 = "fail";
                Assert.Fail();
            }
            catch (Exception /*expected*/) { }
        }

        [TestMethod]
        public virtual void ValidateObjectCrossValidationChange() {
            ITestService service = GetTestService(typeof(TestService));

            service.GetAction("Save Object2").InvokeReturnObject(obj2);

            try {
                (obj2.GetDomainObject() as Object2).Prop1 = "fail";
                Assert.Fail();
            }
            catch (Exception /*expected*/) { }
        }


    }

    [NakedObjects.ValidateProgrammaticUpdates]
    public class Object1 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public string ValidateProp1(string prop1) {
            return prop1 == "fail" ? "fail" : null;
        }

        [Optionally]
        public string Prop2 { get; set; }
    }

    [NakedObjects.ValidateProgrammaticUpdates]
    public class Object2 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        [Optionally]
        public virtual string Prop2 { get; set; }

        public string Validate(string prop1, string prop2) {
            return prop1 == "fail" ? "fail" : null;
        }
    }

}

//end of root namespace