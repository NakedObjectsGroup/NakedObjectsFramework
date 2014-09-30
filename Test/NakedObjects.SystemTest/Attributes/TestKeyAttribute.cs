// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Other
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass, Ignore] //Ignore pending deletion of KeyFacet #5168
    public class TestKeyAttribute : AcceptanceTestCase
    {

        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>()
        {
            return GetTestService(typeof(T).Name + "s").GetAction("New Instance").InvokeReturnObject();
        }
        #region Setup/Teardown

        [TestInitialize()]
        public void SetupTest()
        {
            InitializeNakedObjectsFramework(this);
            
        }

        [TestCleanup()]
        public void TearDownTest()
        {
            CleanupNakedObjectsFramework(this);
            
           
        }

        #endregion

        #region "Services & Fixtures"
        protected override IFixturesInstaller Fixtures
        {
            get { return new FixturesInstaller(new object[] { }); }
        }

        protected override IServicesInstaller MenuServices
        {
            get { return new ServicesInstaller(new object[] { new SimpleRepository<Foo>() }); }
        }
        #endregion

        [TestMethod]
        public void InMemoryPersistorSetsKeyValuesAutomatically()
        {
            var foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            var id1 = foo1.GetPropertyByName("Id1").AssertValueIsEqual("0");
            var id2 = foo1.GetPropertyByName("Id2").AssertValueIsEqual("0");

            foo1.Save();
            id1.AssertTitleIsEqual("1");
            id2.AssertValueIsEqual("0");

            var foo2 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            foo2.Save();
            foo2.GetPropertyByName("Id1").AssertValueIsEqual("2");
        }


        [TestMethod]
        public void SimpleRepositoryFindByKey()
        {
            var foos = GetTestService("Foos");
            var foo1 = foos.GetAction("New Instance").InvokeReturnObject();
            foo1.GetPropertyByName("Name").SetValue("Foo 1");
            foo1.Save();
            var id1 = foo1.GetPropertyByName("Id1").AssertValueIsEqual("1");

            var foo2 = foos.GetAction("New Instance").InvokeReturnObject();
            foo2.GetPropertyByName("Name").SetValue("Foo 2");
            foo2.Save();

            var foo3 = foos.GetAction("New Instance").InvokeReturnObject();
            foo3.GetPropertyByName("Name").SetValue("Foo 3");
            foo3.Save();

            foos.GetAction("Find By Key").InvokeReturnObject(1).AssertTitleEquals("Foo 1");
            foos.GetAction("Find By Key").InvokeReturnObject(3).AssertTitleEquals("Foo 3");
        }
    }
    #region Objects used in tests

    public class Foo
    {
        [Key]
        public virtual int Id1 { get; set; }

        public virtual int Id2 { get; set; }

        [Optionally, Title]
        public virtual string Name { get; set; }
      
     }


    #endregion
}
