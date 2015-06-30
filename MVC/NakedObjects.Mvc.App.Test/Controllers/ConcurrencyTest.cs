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

        public FormCollection GetFormForOrderEdit(IObjectFacade order,
                                                  SalesOrderHeader soh,
                                                  string modifiedDate,
                                                  out IDictionary<string, string> idToRawValue) {
            var nakedObjectSpecification = order.Specification;
            var assocS = nakedObjectSpecification.Properties.Single(p => p.Id == "Status");
            var assocSC = nakedObjectSpecification.Properties.Single(p => p.Id == "StoreContact");
            var assocBA = nakedObjectSpecification.Properties.Single(p => p.Id == "BillingAddress");
            var assocPO = nakedObjectSpecification.Properties.Single(p => p.Id == "PurchaseOrderNumber");
            var assocSA = nakedObjectSpecification.Properties.Single(p => p.Id == "ShippingAddress");
            var assocSM = nakedObjectSpecification.Properties.Single(p => p.Id == "ShipMethod");
            var assocAN = nakedObjectSpecification.Properties.Single(p => p.Id == "AccountNumber");
            var assocCR = nakedObjectSpecification.Properties.Single(p => p.Id == "CurrencyRate");
            var assocCC = nakedObjectSpecification.Properties.Single(p => p.Id == "CreditCard");
            var assocC = nakedObjectSpecification.Properties.Single(p => p.Id == "Comment");
            var assocSP = nakedObjectSpecification.Properties.Single(p => p.Id == "SalesPerson");
            var assocST = nakedObjectSpecification.Properties.Single(p => p.Id == "SalesTerritory");
            var assocMD = nakedObjectSpecification.Properties.Single(p => p.Id == "ModifiedDate");

            string idS = IdHelper.GetFieldInputId(order, (assocS));
            string idSC = IdHelper.GetFieldInputId(order, (assocSC));
            string idBA = IdHelper.GetFieldInputId(order, (assocBA));
            string idPO = IdHelper.GetFieldInputId(order, (assocPO));
            string idSA = IdHelper.GetFieldInputId(order, (assocSA));
            string idSM = IdHelper.GetFieldInputId(order, (assocSM));
            string idAN = IdHelper.GetFieldInputId(order, (assocAN));
            string idCR = IdHelper.GetFieldInputId(order, (assocCR));
            string idCC = IdHelper.GetFieldInputId(order, (assocCC));
            string idC = IdHelper.GetFieldInputId(order, (assocC));
            string idSP = IdHelper.GetFieldInputId(order, (assocSP));
            string idST = IdHelper.GetFieldInputId(order, (assocST));
            string idMD = IdHelper.GetConcurrencyFieldInputId((order), (assocMD));

            var ct = soh.Contact;
            var cus = soh.Customer;
            var sc = FindStoreContactForContact(ct, cus);

            idToRawValue = new Dictionary<string, string> {
                {idS, soh.Status.ToString()},
                {idSC, NakedObjectsFramework.GetObjectId(sc)},
                {idBA, NakedObjectsFramework.GetObjectId(soh.BillingAddress)},
                {idPO, soh.PurchaseOrderNumber},
                {idSA, NakedObjectsFramework.GetObjectId(soh.ShippingAddress)},
                {idSM, NakedObjectsFramework.GetObjectId(soh.ShipMethod)},
                {idAN, soh.AccountNumber},
                {idCR, ""},
                {idCC, NakedObjectsFramework.GetObjectId(soh.CreditCard)},
                {idC, Guid.NewGuid().ToString()},
                {idSP, NakedObjectsFramework.GetObjectId(soh.SalesPerson)},
                {idST, NakedObjectsFramework.GetObjectId(soh.SalesTerritory)},
                {idMD, modifiedDate}
            };

            return GetForm(idToRawValue);
        }

        private StoreContact FindStoreContactForContact(Contact contact, Customer customer) {
            IQueryable<StoreContact> query = from obj in NakedObjectsFramework.Persistor.Instances<StoreContact>()
                                             where obj.Contact.ContactID == contact.ContactID && obj.Store.CustomerId == customer.CustomerId
                                             select obj;

            return query.FirstOrDefault();
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
        //public void EditSaveEFConcurrencyFail() {
        //    Store order = Store;
        //    IObjectFacade adaptedStore = Surface.GetObject(order);
        //    IDictionary<string, string> idToRawvalue;

        //    FormCollection form = GetFormForStoreEdit(adaptedStore, order.Name, NakedObjectsFramework.GetObjectId(order.SalesPerson), order.ModifiedDate.ToString(CultureInfo.CurrentCulture), out idToRawvalue);

        //    var objectModel = new ObjectAndControlData { Id = NakedObjectsFramework.GetObjectId(order) };

        //    NakedObjectsFramework.TransactionManager.StartTransaction();
        //    var conn = new SqlConnection(@"Data Source=" + Constants.Server + @";Initial Catalog=AdventureWorks;Integrated Security=True");

        //    conn.Open();

        //    try {
        //        controller.Edit(objectModel, form);

        //        // change order in database 

        //        string updateStore = string.Format("update Sales.Store set ModifiedDate = GETDATE() where Name = '{0}'", order.Name);

        //        string updateCustomer = string.Format("update Sales.Customer set ModifiedDate = GETDATE() From Sales.Store as ss inner join Sales.Customer as sc on ss.CustomerID = sc.CustomerID  where ss.Name = '{0}'", order.Name);

        //        using (var cmd = new SqlCommand(updateStore) { Connection = conn }) {
        //            cmd.ExecuteNonQuery();
        //        }
        //        using (var cmd = new SqlCommand(updateCustomer) { Connection = conn }) {
        //            cmd.ExecuteNonQuery();
        //        }

        //        NakedObjectsFramework.TransactionManager.EndTransaction();

        //        Assert.Fail("Expect concurrency exception");
        //    }
        //    catch (PreconditionFailedNOSException expected) {
        //        Assert.AreSame(order, expected.SourceNakedObject.Object);
        //    }
        //    finally {
        //        conn.Close();
        //    }
        //}

        public void EditSaveEFConcurrencyFail() {
            SalesOrderHeader order = Order;
            IObjectFacade adaptedOrder = Surface.GetObject(order);
            IDictionary<string, string> idToRawvalue;

            FormCollection form = GetFormForOrderEdit(adaptedOrder, order, order.ModifiedDate.ToString(CultureInfo.CurrentCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData { Id = NakedObjectsFramework.GetObjectId(order) };

            NakedObjectsFramework.TransactionManager.StartTransaction();
            var conn = new SqlConnection(@"Data Source=" + Constants.Server + @";Initial Catalog=AdventureWorks;Integrated Security=True");

            conn.Open();

            try {
                controller.Edit(objectModel, form);

                // change order in database 

                string updateOrder = string.Format("update Sales.SalesOrderHeader set ModifiedDate = GETDATE() where SalesOrderID = '{0}'", order.SalesOrderID);


                using (var cmd = new SqlCommand(updateOrder) { Connection = conn }) {
                    cmd.ExecuteNonQuery();
                }
             

                Surface.End(true);

                Assert.Fail("Expect concurrency exception");
            }
            catch (PreconditionFailedNOSException expected) {
                Assert.AreSame(order, expected.SourceNakedObject.Object);
            }
            finally {
                conn.Close();
            }
        }


        [Test] //As above
        //[Ignore]
        public void AAInvokeObjectActionConcurrencyFail() {
            SalesOrderHeader order = Order;
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = NakedObjectsFramework.GetObjectId(order),
                InvokeAction = "action=action"
            };

            try {
                controller.Action(objectModel, GetForm(new Dictionary<string, string> { { "SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.CurrentCulture) } }));
                Assert.Fail("Expected concurrency exception");
            }
            catch (PreconditionFailedNOSException expected) {
                Assert.AreSame(order, expected.SourceNakedObject.Object);
            }
        }
    }
}