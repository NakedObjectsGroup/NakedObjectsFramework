using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;
using NakedObjects.Xat.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$

{

    [TestClass()]
    public class $safeitemname$ : AcceptanceTestCase
    {

        #region Run configuration
         //Set up the properties in this region exactly the same way as in your Run class

	protected override IServicesInstaller MenuServices
	{
		get
		{
			return new ServicesInstaller();
		}
	}


        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
		var installer = new EntityPersistorInstaller();
                installer.UsingCodeFirstContext(() => new MyDbContext());
                return installer;
            }
        }
        #endregion

        #region Initialize and Cleanup

        [TestInitialize()]
        public void Initialize()
        {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup()]
        public void  Cleanup()
        {
		CleanupNakedObjectsFramework();
        }

        #endregion

    }
}