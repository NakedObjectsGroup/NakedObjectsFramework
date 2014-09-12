// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes
{
    namespace StringLength
    {
        [TestClass]
        public class TestStringLengthAttribute : AbstractSystemTest
        {
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
                get { return new ServicesInstaller(new object[] { new SimpleRepository<Object1>() }); }
            }
            #endregion

            [TestMethod]
            public virtual void StringLengthOnProperty()
            {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertFieldEntryInvalid("12345678");
                prop2.AssertFieldEntryInvalid("1234567 ");
                prop2.SetValue("1234567");
            }

            [TestMethod]
            public virtual void StringLengthOnParm() {
                ITestObject obj = NewTestObject<Object1>();
                ITestAction act = obj.GetAction("Action");

                act.AssertIsInvalidWithParms("123456789");
                act.AssertIsInvalidWithParms("12345678 ");
                act.AssertIsValidWithParms("12345678");
            }
        }

      

        public class Object1 {
            public string Prop1 { get; set; }

            [System.ComponentModel.DataAnnotations.StringLength(7)]
            public string Prop2 { get; set; }

            public void Action([System.ComponentModel.DataAnnotations.StringLength(8)]string parm) { }
        }
    }
} //end of root namespace