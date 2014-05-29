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
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Helpers;

namespace NakedObjects.Mvc.App {
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
                    new WorkOrderRepository()
                    );
            }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new OrderContributedActions(), new CustomerContributedActions()); }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new SimpleEncryptDecrypt()); }
        }

        private static Type[] AdventureWorksTypes() {
            var allTypes =  AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof(AWDomainObject) && !t.IsAbstract).ToArray();
        }

        protected override IObjectPersistorInstaller Persistor {
            get {
                // Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"); //For in-memory database
                // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>()); //Optional behaviour for CodeFirst
                var installer = new EntityPersistorInstaller();

                installer.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
                installer.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });
                //installer.UsingCodeFirstContext(() => new MyDbContext()).AssociateTypes(CodeFirstTypes);  //For Code First

                return installer;
            }
        }

        public static void Run() {
            Assert.AssertTrue("Rest root may not be empty", RestRoot == null || RestRoot.Trim().Length > 0);
            new RunWeb().Start();
        }
    }
}