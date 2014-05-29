using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Services;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.UnitTests.Models;

namespace NakedObjects.Mvc.Selenium.Test.Data {
    public class RunWeb : RunMvc {

		// Return null for no REST support
		// Return a non empty string to support REST at that URL segment
        public static string RestRoot {
            get { return null; }
        }

        protected override NakedObjectsContext Context {
            get { return HttpContextContext.CreateInstance(); }
        }

        protected override bool StoreTransientsInSession {
            get { return true; }
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Person>()
                                                              });
            }
        }

        protected override IServicesInstaller ContributedActions {
            get {
                return new ServicesInstaller(new object[] {
                                                                  //Add your services here
                                                              });
            }
        }


        protected override IServicesInstaller SystemServices {
            get {
                return new ServicesInstaller(new object[] {
                                                                  //Add your services here
                                                              });
            }
        }

        //For code first operation, replace this property using the 'codefirst' snippet
        protected override IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller(); }
        }
       

        public static void Run() {
		    Assert.AssertTrue("Rest root may not be empty", RestRoot == null || RestRoot.Trim().Length > 0);
            new RunWeb().Start();
        }
    }
}