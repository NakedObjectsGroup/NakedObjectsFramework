// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using AdventureWorksModel;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;
using NakedObjects.Rest.App.Demo.Models;
using RestfulObjects.Bootstrap;

namespace NakedObjects.Rest.App.Demo.App_Start {
	public class RunWeb : RunRest {

	    // Return an empty string to support REST on the root application path. 
        // Return a non empty string to support REST at that URL segment
        public static string RestRoot {
            get { return "";  }
        } 

		protected override NakedObjectsContext Context {
			get { return HttpContextContext.CreateInstance(); }
		}

	    protected override IServicesInstaller MenuServices {
	        get {
	            return new ServicesInstaller(
                    new CustomerRepository(),
	                new OrderRepository(),
	                new ProductRepository(),
	                new EmployeeRepository(),
	                new SalesRepository(),
	                new SpecialOfferRepository(),
	                new ContactRepository(),
	                new VendorRepository(),
	                new PurchaseOrderRepository(),
	                new WorkOrderRepository(),
	                new TestHelperService());
	        }
	    }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new OrderContributedActions(), new CustomerContributedActions() }); }
        }

		protected override IServicesInstaller SystemServices {
			get { return new ServicesInstaller(); }
		}

        private static Type[] AdventureWorksTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof(AWDomainObject) && !t.IsAbstract).ToArray();
        }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                // Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"); //For in-memory database
                // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>()); //Optional behaviour for CodeFirst
                var installer = new EntityPersistorInstaller();

                installer.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
                installer.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });
                // installer.UsingCodeFirstContext(() => new MyDbContext()).AssociateTypes(CodeFirstTypes);  //For Code First

                return installer;
            }
        }

		public static void Run() {
		    Assert.AssertNotNull("Rest root may not be null", RestRoot);
			new RunWeb().Start();
		}
	}
}