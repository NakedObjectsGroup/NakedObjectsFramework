// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace NakedObjects.SystemTest.Attributes {
    namespace RegEx {
        [TestClass, Ignore]
        public class TestRegExAttribute : AbstractSystemTest {
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

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>()}); }
            }
            #endregion

            [TestMethod]
            public virtual void SimpleRegExAttributeOnProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty email = obj.GetPropertyByName("Email");
                email.AssertFieldEntryInvalid("richard @hotmail.com");
                email.AssertFieldEntryInvalid("richardpAThotmail.com");
                email.AssertFieldEntryInvalid("richardp@hotmail;com");
                email.AssertFieldEntryInvalid("richardp@hotmail.com (preferred)");
                email.AssertFieldEntryInvalid("personal richardp@hotmail.com");
                email.SetValue("richard@hotmail.com");
            }

            [TestMethod]
            public virtual void SimpleRegularExpressionAttributeOnProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty email = obj.GetPropertyByName("Email");
                email.AssertFieldEntryInvalid("richard @hotmail.com");
                email.AssertFieldEntryInvalid("richardpAThotmail.com");
                email.AssertFieldEntryInvalid("richardp@hotmail;com");
                email.AssertFieldEntryInvalid("richardp@hotmail.com (preferred)");
                email.AssertFieldEntryInvalid("personal richardp@hotmail.com");
                email.SetValue("richard@hotmail.com");
            }
        }

        public class Object1 {
            [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
            public virtual string Email { get; set; }
        }

        public class Object2 {
            [RegularExpression(@"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
            public virtual string Email { get; set; }
        }
    }
}