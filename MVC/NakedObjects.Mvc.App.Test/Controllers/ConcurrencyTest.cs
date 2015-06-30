using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using Moq;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Component;
using NakedObjects.Core;
using NakedObjects.Core.Component;
using NakedObjects.Core.Util;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Controllers {

    [TestFixture]
    public class ConcurrencyTest : AcceptanceTestCase {
        private GenericController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get {
                return new[] {
                    "AdventureWorksModel", "MvcTestApp.Tests.Controllers"
                };
            }
        }

        public Store Store {
            get { return NakedObjectsFramework.Persistor.Instances<Store>().First(); }
        }

        protected override object[] MenuServices {
            get {
                return (new object[] {
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
                    new SimpleRepository<NotPersistedObject>()
                });
            }
        }

        protected override object[] ContributedActions {
            get { return (new object[] { new OrderContributedActions() }); }
        }

        private SalesOrderHeader Order {
            get { return NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().First(); }
        }

        private IIdHelper IdHelper {
            get {
                return new IdHelper();
            }
        }

        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            StartTest();

            controller = new GenericController(Surface, IdHelper);
            mocks = new ContextMocks(controller);
        }

        protected IFrameworkFacade Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        protected override void StartTest() {
            Surface = GetConfiguredContainer().Resolve<IFrameworkFacade>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
            MessageBroker = NakedObjectsFramework.MessageBroker;
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration { EnforceProxies = false };
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IOidTranslator, OidTranslatorSemiColonSeparatedList>(new PerResolveLifetimeManager());
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
//            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

       

        public FormCollection GetFormForStoreEdit(IObjectFacade store,
                                                  string storeName,
                                                  string salesPerson,
                                                  string modifiedDate,
                                                  out IDictionary<string, string> idToRawValue) {
            var nakedObjectSpecification = store.Specification;
            var assocSN = nakedObjectSpecification.Properties.Single( p => p.Id == "Name");
            var assocSP = nakedObjectSpecification.Properties.Single(p => p.Id == "SalesPerson");
            var assocMD = nakedObjectSpecification.Properties.Single(p => p.Id == "ModifiedDate");

            string idSN = IdHelper.GetFieldInputId(store, (assocSN));
            string idSP = IdHelper.GetFieldInputId(store, (assocSP));
            string idMD = IdHelper.GetConcurrencyFieldInputId((store), (assocMD));

            idToRawValue = new Dictionary<string, string> {
                {idSN, storeName},
                {idSP, salesPerson},
                {idMD, modifiedDate}
            };

            return GetForm(idToRawValue);
        }

        private static FormCollection GetForm(IDictionary<string, string> nameValues) {
            var form = new FormCollection();
            nameValues.ForEach(kvp => form.Add(kvp.Key, kvp.Value));
            return form;
        }

        [Test]
        //[Ignore]
        //RP: Can't figure out how to add a ConcurrencyCheck (attribute or fluent) into
        //Store, or even into Customer, without getting a model validation error
        // in seperate test fixture because otherwise it fails on second attempt - MvcTestApp.Tests.Controllers.GenericControllerTest.EditSaveEFConcurrencyFail:
        // System.Data.EntityCommandExecutionException : An error occurred while executing the command definition. See the inner exception for details.
        //  ----> System.Data.SqlClient.SqlException : A transport-level error has occurred when sending the request to the server. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.)
        public void EditSaveEFConcurrencyFail() {
            Store store = Store;
            IObjectFacade adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;

            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.InvariantCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData { Id = NakedObjectsFramework.GetObjectId(store) };

            NakedObjectsFramework.TransactionManager.StartTransaction();
            var conn = new SqlConnection(@"Data Source=" + Constants.Server + @";Initial Catalog=AdventureWorks;Integrated Security=True");

            conn.Open();

            try {
                controller.Edit(objectModel, form);

                // change store in database 

                string updateStore = string.Format("update Sales.Store set ModifiedDate = GETDATE() where Name = '{0}'", store.Name);

                string updateCustomer = string.Format("update Sales.Customer set ModifiedDate = GETDATE() From Sales.Store as ss inner join Sales.Customer as sc on ss.CustomerID = sc.CustomerID  where ss.Name = '{0}'", store.Name);

                using (var cmd = new SqlCommand(updateStore) { Connection = conn }) {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand(updateCustomer) { Connection = conn }) {
                    cmd.ExecuteNonQuery();
                }

                NakedObjectsFramework.TransactionManager.EndTransaction();

                Assert.Fail("Expect concurrency exception");
            }
            catch (PreconditionFailedNOSException expected) {
                Assert.AreSame(store, expected.SourceNakedObject.Object);
            }
            finally {
                conn.Close();
            }
        }

        [Test] //As above
        //[Ignore]
        public void InvokeObjectActionConcurrencyFail() {
            SalesOrderHeader order = Order;
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = NakedObjectsFramework.GetObjectId(order),
                InvokeAction = "action=action"
            };

            try {
                controller.Action(objectModel, GetForm(new Dictionary<string, string> { { "SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.InvariantCulture) } }));
                Assert.Fail("Expected concurrency exception");
            }
            catch (PreconditionFailedNOSException expected) {
                Assert.AreSame(order, expected.SourceNakedObject.Object);
            }
        }
    }
}