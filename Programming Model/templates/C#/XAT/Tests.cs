using System;
using System.Linq;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$

{

    [TestClass()]
    public class $safeitemname$ : AcceptanceTestCase
    {

        #region Run configuration
        //Specify top-level Namespace(s) covering your domain model
        protected override string[] Namespaces {
            get { return new string[] { }; }
        }

         //Register all services here as types
        protected override Type[] Services {
            get { return new Type[] { }; }
        }

        //Create main menus here, if they need to be accessed in tests
        //protected virtual IMenu[] MainMenus(IMenuFactory factory) {
        //    return new IMenu[] {
        //    };
        //}

        //Specify any domain types that won't be reached by traversing the object graph 
        //from methods in the registered services
        protected override Type[] Types {
            get {
                return new Type[] {};
            }
        }

        protected override EntityObjectStoreConfiguration Persistor
        {
            get
            {
		        var installer = new EntityObjectStoreConfiguration();
                //installer.UsingCodeFirstContext(() => new MyDbContext());
                return installer;
            }
        }
        #endregion

        #region Initialize and Cleanup
        //private bool fixturesRun;

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework(this);
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
            //if (!fixturesRun) {
            //    RunFixtures();
            //    fixturesRun = true;
            //}
            StartTest();
        }

        [TestCleanup()]
        public void  Cleanup()
        {
        }

        //protected override object[] Fixtures {
        //    get {
        //        return new object[] { };
        //    }
        //}

        #endregion

    }
}