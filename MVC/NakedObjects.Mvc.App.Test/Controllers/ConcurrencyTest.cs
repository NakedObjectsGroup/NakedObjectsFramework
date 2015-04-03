using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using Moq;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
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
            var mockSurface = new Mock<INakedObjectsSurface>().Object;
            controller = new GenericController(NakedObjectsFramework, mockSurface, IdHelper);
            mocks = new ContextMocks(controller);
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration { EnforceProxies = false };
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
//            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        public FormCollection GetFormForStoreEdit(INakedObjectAdapter store,
                                                  string storeName,
                                                  string salesPerson,
                                                  string modifiedDate,
                                                  out IDictionary<string, string> idToRawValue) {
            IObjectSpec nakedObjectSpecification = (IObjectSpec)NakedObjectsFramework.MetamodelManager.GetSpecification(typeof(Store));
            IAssociationSpec assocSN = nakedObjectSpecification.GetProperty("Name");
            IAssociationSpec assocSP = nakedObjectSpecification.GetProperty("SalesPerson");
            IAssociationSpec assocMD = nakedObjectSpecification.GetProperty("ModifiedDate");

            string idSN = IdHelper.GetFieldInputId(ScaffoldAdapter.Wrap(store), ScaffoldAssoc.Wrap(assocSN));
            string idSP = IdHelper.GetFieldInputId(ScaffoldAdapter.Wrap(store), ScaffoldAssoc.Wrap(assocSP));
            string idMD = IdHelper.GetConcurrencyFieldInputId(ScaffoldAdapter.Wrap(store), ScaffoldAssoc.Wrap(assocMD));

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
        // in seperate test fixture because otherwise it fails on second attempt - MvcTestApp.Tests.Controllers.GenericControllerTest.EditSaveEFConcurrencyFail:
        // System.Data.EntityCommandExecutionException : An error occurred while executing the command definition. See the inner exception for details.
        //  ----> System.Data.SqlClient.SqlException : A transport-level error has occurred when sending the request to the server. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.)
        public void EditSaveEFConcurrencyFail() {
            Store store = Store;
            INakedObjectAdapter adaptedStore = NakedObjectsFramework.NakedObjectManager.CreateAdapter(store, null, null);
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
            } catch (ConcurrencyException expected) {
                Assert.AreSame(store, expected.SourceNakedObjectAdapter.Object);
            } finally {
                conn.Close();
            }
        }

        [Test]
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
            } catch (ConcurrencyException expected) {
                Assert.AreSame(order, expected.SourceNakedObjectAdapter.Object);
            }
        }
    }
}