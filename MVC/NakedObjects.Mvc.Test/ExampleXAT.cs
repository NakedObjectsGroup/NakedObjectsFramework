using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;
using NakedObjects.Xat.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace web_mvc_viewer_test
{

    //You can create a new class of this form by invoking Add > New Item > Naked Objects > XAT
    [TestClass()]
    public class ExampleXAT : AcceptanceTestCase
    {

        #region Constructors
        public ExampleXAT(string name) : base(name) { }

        public ExampleXAT() : this(typeof(ExampleXAT).Name) { }
        #endregion

        #region Run configuration

        //Set up the properties in this region exactly the same way as in your Run class
        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(new object[] { });
            }
        }

        protected override IServicesInstaller ContributedActions
        {
            get
            {
                return new ServicesInstaller(new object[] { });
            }
        }

        protected override IServicesInstaller SystemServices
        {
            get
            {
                return new ServicesInstaller(new object[] { });
            }
        }

        protected override IFixturesInstaller Fixtures
        {
            get
            {
                return new FixturesInstaller(new object[] { });
            }
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get { return new EntityPersistorInstaller(); }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework(); // This must be the first line in the method
            // Optional: use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
			// This uses SQL Server Management Objects (SMO) - you may need to install a SQL Server feature pack if SMO is not present on your 
			// machine.
        }

        [TestCleanup()]
        public void Cleanup()
        {
            CleanupNakedObjectsFramework();
        }

        #endregion

        [TestMethod()]
        public virtual void ExampleTestMethod()
        {
            // This code is just to illustrate the XAT idioms: the test will not run without a matching model project (an Expenses Processing application)
            ITestObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof(string)).InvokeReturnObject("My November Expenses Claim");

            claim.GetPropertyByName("Description").AssertIsVisible().AssertIsModifiable();
            claim.GetPropertyByName("Date Created").AssertIsVisible().AssertIsUnmodifiable();
            //...

            claim.GetAction("Create New Expense Item").AssertIsVisible().AssertIsInvalidWithParms(null);
            claim.GetAction("Copy An Existing Expense Item").AssertIsVisible().AssertIsInvalidWithParms(null);
            claim.GetAction("Copy All Expense Items From Another Claim").AssertIsVisible();
            //...

            claim.GetAction("Approve Items").AssertIsVisible();
            //...
            claim.GetAction("Return To Claimant", typeof(string)).AssertIsVisible().AssertIsInvalidWithParms("");

            ITestParameter[] tp1 = claim.GetAction("Copy All Expense Items From Another Claim").Parameters;

            tp1[0].AssertIsMandatory().AssertIsNamed("Claim or Template").AssertIsDescribedAs("");
            tp1[1].AssertIsOptional().AssertIsNamed("New date to apply to all items").AssertIsDescribedAs("");

            ITestParameter[] tp2 = claim.GetAction("Create New Claim From This").Parameters;

            tp2[0].AssertIsMandatory().AssertIsNamed("Description").AssertIsDescribedAs("");
            tp2[1].AssertIsOptional().AssertIsNamed("New date to apply to all items").AssertIsDescribedAs("");

            ITestNaked[] choices = claim.GetAction("Create New Expense Item").Parameters[0].GetChoices();

            Assert.IsTrue(choices.Length == 8, "Wrong number of choices in choice array");

            ((ITestObject)(choices[0])).AssertIsImmutable().AssertIsDescribedAs("");

            claim.GetAction("Submit").AssertIsInvalidWithParms(null, true);
            claim.GetAction("Approve Items").AssertIsInvalidWithParms(true);
            claim.GetAction("Query Items").AssertIsInvalidWithParms("", true);
            claim.GetAction("Reject Items").AssertIsInvalidWithParms("", true);

        }
    }
}