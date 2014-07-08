using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Helpers;

namespace $rootnamespace$ {
    public class RunWeb : RunMvc {

		// Return null for no REST support
		// Return a non empty string to support REST at that URL segment
        public static string RestRoot {
            get { return null; }
        }

        protected override NakedObjectsContext Context {
            get { return HttpContextContext.CreateInstance(); }
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller();
            }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(); }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new SimpleEncryptDecrypt()); }
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
	            var installer = new EntityPersistorInstaller();
				// installer.UsingCodeFirstContext(() => new MyDbContext());  //to work 'Code First'
                // installer.UsingEdmxContext("MyModel"); // to work with an .edmx file
                return installer;
            }
        }

        public static void Run() {
		    Assert.AssertTrue("Rest root may not be empty", RestRoot == null || RestRoot.Trim().Length > 0);
            new RunWeb().Start();
        }
    }
}