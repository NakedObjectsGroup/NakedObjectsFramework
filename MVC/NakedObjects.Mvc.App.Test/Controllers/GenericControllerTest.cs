// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.DatabaseHelpers;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Services;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;
using NUnit.Framework;
using NakedObjects.Core.Util;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class GenericControllerTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new GenericController(NakedObjectsFramework);
            mocks = new ContextMocks(controller);
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingEdmxContext("Model");
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
        }

        private GenericController controller;
        private ContextMocks mocks;

        protected override object[] MenuServices {
            get {
                return new object[] {
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
                };
            }
        }

        protected override object[] ContributedActions {
            get {
                return new object[] {
                    new OrderContributedActions(),
                    new CustomerContributedActions()
                };
            }
        }


        private static void AssertPagingData(ViewResult result, int currentPage, int pageSize, int pageTotal) {
            Assert.AreEqual(currentPage, ((Dictionary<string, int>) result.ViewData[IdHelper.PagingData])[IdHelper.PagingCurrentPage]);
            Assert.AreEqual(pageSize, ((Dictionary<string, int>) result.ViewData[IdHelper.PagingData])[IdHelper.PagingPageSize]);
            Assert.AreEqual(pageTotal, ((Dictionary<string, int>) result.ViewData[IdHelper.PagingData])[IdHelper.PagingTotal]);
        }

        private static void AssertIsCollectionViewOf<T>(ViewResult result) {
            Assert.AreEqual("StandaloneTable", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (IEnumerable<T>), data.Model);
            Assert.IsNotInstanceOf(typeof (IQueryable<T>), data.Model);
            Assert.Greater(((IEnumerable<T>) data.Model).Count(), 0);
        }

        private static void AssertIsQueryableViewOf<T>(ViewResult result) {
            Assert.AreEqual("StandaloneTable", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (IQueryable<T>), data.Model);
            Assert.Greater(((IQueryable<T>) data.Model).Count(), 0);
        }

        private static void AssertIsDetailsViewOf<T>(ViewResult result) {
            Assert.AreEqual("ObjectView", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (T), data.Model);
        }

        private static void AssertIsSetAfterTransactionViewOf<T>(ViewResult result) {
            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (T), data.Model);
        }


        private static void AssertIsEditViewOf<T>(ViewResult result) {
            Assert.AreEqual("ObjectEdit", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (T), data.Model);
        }

        private void AssertIsDialogViewOfAction(ViewResult result, string actionName) {
            Assert.AreEqual("ActionDialog", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (FindViewModel), data.Model);
            Assert.AreEqual(actionName, ((FindViewModel) data.Model).ContextAction.GetName());
        }


        private static void AssertStateInViewDataDictionary(ViewResult result, string id, string state) {
            Assert.IsTrue(((string) result.ViewData[id]) == state);
        }

        private static void AssertStateInModelStateDictionary(ViewResult result, string id, string state) {
            Assert.IsTrue(result.ViewData.ModelState[id].Value.AttemptedValue == state);
        }

        private static FormCollection GetForm(IDictionary<string, string> nameValues) {
            var form = new FormCollection();
            nameValues.ForEach(kvp => form.Add(kvp.Key, kvp.Value));
            return form;
        }

        private string GetBoundedId<T>(string title) {
            return NakedObjectsFramework.GetObjectId(GetBoundedInstance<T>(title).GetDomainObject());
        }

        private static FormCollection GetFormForFindEmployeeByName(INakedObject employeeRepo, string firstName, string secondName) {
            IActionSpec actionFindEmployeeByname = employeeRepo.Spec.GetAllActions().Single(a => a.Id == "FindEmployeeByName");

            IActionParameterSpec parmFirstName = actionFindEmployeeByname.Parameters[0];
            IActionParameterSpec parmSecondName = actionFindEmployeeByname.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(actionFindEmployeeByname, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(actionFindEmployeeByname, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private static FormCollection GetFormForFindSalesPersonByName(INakedObject salesRepo, string firstName, string secondName) {
            IActionSpec action = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");

            IActionParameterSpec parmFirstName = action.Parameters[0];
            IActionParameterSpec parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(action, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(action, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private static FormCollection GetFormForFindContactByName(INakedObject contactRepo, string firstName, string secondName) {
            IActionSpec action = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");

            IActionParameterSpec parmFirstName = action.Parameters[0];
            IActionParameterSpec parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(action, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(action, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }


        private static FormCollection GetFormForBestSpecialOffer(INakedObject productRepo, string quantity) {
            IActionSpec action = productRepo.Spec.GetAllActions().Single(a => a.Id == "BestSpecialOffer");
            IActionParameterSpec parmQuantity = action.Parameters[0];
            string idQuantity = IdHelper.GetParameterInputId(action, parmQuantity);
            return GetForm(new Dictionary<string, string> {
                {idQuantity, quantity},
            });
        }

        private static FormCollection GetFormForChangePassword(INakedObject contact, string p1, string p2, string p3) {
            IActionSpec action = contact.Spec.GetAllActions().Single(a => a.Id == "ChangePassword");
            IActionParameterSpec pp1 = action.Parameters[0];
            IActionParameterSpec pp2 = action.Parameters[1];
            IActionParameterSpec pp3 = action.Parameters[2];

            string idP1 = IdHelper.GetParameterInputId(action, pp1);
            string idP2 = IdHelper.GetParameterInputId(action, pp2);
            string idP3 = IdHelper.GetParameterInputId(action, pp3);

            return GetForm(new Dictionary<string, string> {
                {idP1, p1},
                {idP2, p2},
                {idP3, p3},
            });
        }


        private static FormCollection GetFormForListProductsBySubCategory(INakedObject productRepo, string pscId) {
            IActionSpec action = productRepo.Spec.GetAllActions().Single(a => a.Id == "ListProductsBySubCategory");
            IActionParameterSpec parmPsc = action.Parameters[0];
            string idPsc = IdHelper.GetParameterInputId(action, parmPsc);

            return GetForm(new Dictionary<string, string> {
                {idPsc, pscId},
            });
        }

        private FormCollection GetFormForShiftEdit(INakedObject shift,
            INakedObject timePeriod,
            string t1,
            string t2,
            out IDictionary<string, string> idToRawValue) {
            IObjectSpec shiftSpec = NakedObjectsFramework.Metamodel.GetSpecification(typeof (Shift));
            IObjectSpec timePeriodSpec = NakedObjectsFramework.Metamodel.GetSpecification(typeof(TimePeriod));

            IAssociationSpec assocN = shiftSpec.GetProperty("Name");
            IAssociationSpec assocTp = shiftSpec.GetProperty("Times");

            IAssociationSpec assocT1 = timePeriodSpec.GetProperty("StartTime");
            IAssociationSpec assocT2 = timePeriodSpec.GetProperty("EndTime");

            string idN = IdHelper.GetFieldInputId(shift, assocN);
            string idT1 = IdHelper.GetInlineFieldInputId(assocTp, timePeriod, assocT1);
            string idT2 = IdHelper.GetInlineFieldInputId(assocTp, timePeriod, assocT2);


            idToRawValue = new Dictionary<string, string> {
                {idN, Guid.NewGuid().ToString()},
                {idT1, t1},
                {idT2, t2}
            };

            return GetForm(idToRawValue);
        }


        private FormCollection GetFormForVendorEdit(INakedObject vendor,
            string accountNumber,
            string name,
            string creditRating,
            string preferredVendorStatus,
            string activeFlag,
            string purchasingWebServiceURL,
            out IDictionary<string, string> idToRawValue) {
            IObjectSpec nakedObjectSpecification = NakedObjectsFramework.Metamodel.GetSpecification(typeof (Vendor));
            IAssociationSpec assocAN = nakedObjectSpecification.GetProperty("AccountNumber");
            IAssociationSpec assocN = nakedObjectSpecification.GetProperty("Name");
            IAssociationSpec assocCR = nakedObjectSpecification.GetProperty("CreditRating");
            IAssociationSpec assocPVS = nakedObjectSpecification.GetProperty("PreferredVendorStatus");
            IAssociationSpec assocAF = nakedObjectSpecification.GetProperty("ActiveFlag");
            IAssociationSpec assocPWSURL = nakedObjectSpecification.GetProperty("PurchasingWebServiceURL");

            string idAN = IdHelper.GetFieldInputId(vendor, assocAN);
            string idN = IdHelper.GetFieldInputId(vendor, assocN);
            string idCR = IdHelper.GetFieldInputId(vendor, assocCR);
            string idPVS = IdHelper.GetFieldInputId(vendor, assocPVS);
            string idAF = IdHelper.GetFieldInputId(vendor, assocAF);
            string idPWSURL = IdHelper.GetFieldInputId(vendor, assocPWSURL);

            idToRawValue = new Dictionary<string, string> {
                {idAN, accountNumber},
                {idN, name},
                {idCR, creditRating},
                {idPVS, preferredVendorStatus},
                {idAF, activeFlag},
                {idPWSURL, purchasingWebServiceURL}
            };

            return GetForm(idToRawValue);
        }

        public FormCollection GetFormForStoreEdit(INakedObject store,
            string storeName,
            string salesPerson,
            string modifiedDate,
            out IDictionary<string, string> idToRawValue) {
                IObjectSpec nakedObjectSpecification = NakedObjectsFramework.Metamodel.GetSpecification(typeof(Store));
            IAssociationSpec assocSN = nakedObjectSpecification.GetProperty("Name");
            IAssociationSpec assocSP = nakedObjectSpecification.GetProperty("SalesPerson");
            IAssociationSpec assocMD = nakedObjectSpecification.GetProperty("ModifiedDate");

            string idSN = IdHelper.GetFieldInputId(store, assocSN);
            string idSP = IdHelper.GetFieldInputId(store, assocSP);
            string idMD = IdHelper.GetConcurrencyFieldInputId(store, assocMD);

            idToRawValue = new Dictionary<string, string> {
                {idSN, storeName},
                {idSP, salesPerson},
                {idMD, modifiedDate}
            };

            return GetForm(idToRawValue);
        }

        private FormCollection GetFormForCeditCardEdit(INakedObject creditCard,
            string cardType,
            string cardNumber,
            string expiryMonth,
            string expiryYear,
            out IDictionary<string, string> idToRawValue) {
                IObjectSpec nakedObjectSpecification = NakedObjectsFramework.Metamodel.GetSpecification(typeof(CreditCard));
            IAssociationSpec assocCT = nakedObjectSpecification.GetProperty("CardType");
            IAssociationSpec assocCN = nakedObjectSpecification.GetProperty("CardNumber");
            IAssociationSpec assocEM = nakedObjectSpecification.GetProperty("ExpMonth");
            IAssociationSpec assocEY = nakedObjectSpecification.GetProperty("ExpYear");

            string idCT = IdHelper.GetFieldInputId(creditCard, assocCT);
            string idCN = IdHelper.GetFieldInputId(creditCard, assocCN);
            string idEM = IdHelper.GetFieldInputId(creditCard, assocEM);
            string idEY = IdHelper.GetFieldInputId(creditCard, assocEY);

            idToRawValue = new Dictionary<string, string> {
                {idCT, cardType},
                {idCN, cardNumber},
                {idEM, expiryMonth},
                {idEY, expiryYear},
            };

            return GetForm(idToRawValue);
        }


        private static FormCollection GetFormForCreateNewEmployeeFromContact(IActionSpec action, string contact, out IDictionary<string, string> idToRawValue) {
            IActionParameterSpec parmContact = action.Parameters[0];
            string idContact = IdHelper.GetParameterInputId(action, parmContact);
            idToRawValue = new Dictionary<string, string> {
                {idContact, contact},
            };
            return GetForm(idToRawValue);
        }

        private static FormCollection GetFormForCreateNewOrder(IActionSpec action, string cust, bool copy, out IDictionary<string, string> idToRawValue) {
            IActionParameterSpec parmCust = action.Parameters[0];
            IActionParameterSpec parmCopy = action.Parameters[1];

            string idCust = IdHelper.GetParameterInputId(action, parmCust);
            string idCopy = IdHelper.GetParameterInputId(action, parmCopy);

            idToRawValue = new Dictionary<string, string> {
                {idCust, cust},
                {idCopy, copy.ToString()},
            };
            return GetForm(idToRawValue);
        }


        private Employee Employee {
            get { return NakedObjectsFramework.Persistor.Instances<Employee>().First(); }
        }

        private string EmployeeId {
            get { return NakedObjectsFramework.GetObjectId(Employee); }
        }

        private Employee TransientEmployee {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof(Employee))).GetDomainObject<Employee>(); }
        }

        private Vendor TransientVendor {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof(Vendor))).GetDomainObject<Vendor>(); }
        }

        private Shift TransientShift {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof(Shift))).GetDomainObject<Shift>(); }
        }

        private Individual TransientIndividual {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof(Individual))).GetDomainObject<Individual>(); }
        }

        private NotPersistedObject NotPersistedObject {
            get {
                var repo = NakedObjectsFramework.GetAdaptedService("repository#MvcTestApp.Tests.Controllers.NotPersistedObject").Object as SimpleRepository<NotPersistedObject>;
                return repo.NewInstance();
            }
        }

        private SalesOrderHeader Order {
            get { return NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().First(); }
        }

        private string OrderId {
            get { return NakedObjectsFramework.GetObjectId(Order); }
        }

        private Vendor Vendor {
            get { return NakedObjectsFramework.Persistor.Instances<Vendor>().First(); }
        }

        private Contact Contact {
            get { return NakedObjectsFramework.Persistor.Instances<Contact>().First(); }
        }

        private Individual Individual {
            get { return NakedObjectsFramework.Persistor.Instances<Individual>().First(); }
        }

        private Product Product {
            get { return NakedObjectsFramework.Persistor.Instances<Product>().First(); }
        }

        private string ProductId {
            get { return NakedObjectsFramework.GetObjectId(Product); }
        }


        private INakedObject EmployeeRepo {
            get { return NakedObjectsFramework.GetAdaptedService("EmployeeRepository"); }
        }

        private INakedObject OrderContributedActions {
            get { return NakedObjectsFramework.GetAdaptedService("OrderContributedActions"); }
        }

        private string OrderContributedActionsId {
            get { return NakedObjectsFramework.GetObjectId(OrderContributedActions); }
        }

        private INakedObject ProductRepo {
            get { return NakedObjectsFramework.GetAdaptedService("ProductRepository"); }
        }

        private string EmployeeRepoId {
            get { return NakedObjectsFramework.GetObjectId(EmployeeRepo); }
        }

        private string ProductRepoId {
            get { return NakedObjectsFramework.GetObjectId(ProductRepo); }
        }

        private INakedObject OrderRepo {
            get { return NakedObjectsFramework.GetAdaptedService("OrderRepository"); }
        }

        private string OrderRepoId {
            get { return NakedObjectsFramework.GetObjectId(OrderRepo); }
        }

        private INakedObject OrderContrib {
            get { return NakedObjectsFramework.GetAdaptedService("OrderContributedActions"); }
        }

        private string OrderContribId {
            get { return NakedObjectsFramework.GetObjectId(OrderContrib); }
        }

        private INakedObject CustomerRepo {
            get { return NakedObjectsFramework.GetAdaptedService("CustomerRepository"); }
        }

        private string CustomerRepoId {
            get { return NakedObjectsFramework.GetObjectId(CustomerRepo); }
        }

        public Store Store {
            get { return NakedObjectsFramework.Persistor.Instances<Store>().First(); }
        }

        private string StoreId {
            get { return NakedObjectsFramework.GetObjectId(Store); }
        }

        private SalesPerson SalesPerson {
            get { return NakedObjectsFramework.Persistor.Instances<SalesPerson>().First(); }
        }

        private Store TransientStore {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof(Store))).GetDomainObject<Store>(); }
        }


        private static IActionSpec GetAction(INakedObject owner, string id) {
            return owner.Spec.GetAllActions().Single(a => a.Id == id);
        }

        private static void AssertNameAndParms(ViewResult result, string name, int? count, object contextObject, IActionSpec contextAction, object targetObject, IActionSpec targetAction, string pName) {
            Assert.AreEqual(name, result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (FindViewModel), data.Model);
            var fvm = data.Model as FindViewModel;

            if (count == null) {
                Assert.IsNull(fvm.ActionResult);
            }
            else {
                Assert.IsNotNull(fvm.ActionResult);
                Assert.AreEqual(count.Value, fvm.ActionResult.Cast<object>().Count());
            }

            Assert.AreSame(contextObject, fvm.ContextObject);
            Assert.AreSame(contextAction, fvm.ContextAction);
            Assert.AreSame(targetObject, fvm.TargetObject);
            Assert.AreSame(targetAction, fvm.TargetAction);
            Assert.AreEqual(pName, fvm.PropertyName);
        }


        public void EditRedisplay(Employee employee) {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string redisplay = "DepartmentHistory=table&editMode=True";
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }


        public void EditSaveValidationOk(Vendor vendor) {
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsFramework.Manager.CreateAdapter(vendor, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(vendor)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Vendor>(result);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        public void EditInlineSaveValidationOk(Shift shift) {
            INakedObject adaptedShift = NakedObjectsFramework.Manager.CreateAdapter(shift, null, null);
            INakedObject adaptedTimePeriod = NakedObjectsFramework.Manager.CreateAdapter(shift.Times, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.ToString(), DateTime.Now.ToString(), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(shift)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Shift>(result);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }


        public void EditSaveValidationFail(Vendor vendor) {
            INakedObject adaptedVendor = NakedObjectsFramework.Manager.CreateAdapter(vendor, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, "", "", "", "", "", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(vendor)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (var kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(adaptedVendor, adaptedVendor.Spec.GetProperty("PreferredVendorStatus"))].Errors.Count(), 0);
            AssertIsEditViewOf<Vendor>(result);
        }


        public void EditInlineSaveValidationFail(Shift shift) {
            INakedObject adaptedShift = NakedObjectsFramework.Manager.CreateAdapter(shift, null, null);
            INakedObject adaptedTimePeriod = NakedObjectsFramework.Manager.CreateAdapter(shift.Times, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.ToString(), "invalid", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(shift)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (var kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetInlineFieldInputId(adaptedShift.Spec.GetProperty("Times"), adaptedTimePeriod, adaptedTimePeriod.Spec.GetProperty("EndTime"))].Errors.Count(), 0);
            AssertIsEditViewOf<Shift>(result);
        }


        public void EditSaveValidationFailEmptyForm(Individual individual) {
            INakedObject nakedObject = NakedObjectsFramework.Manager.CreateAdapter(individual, null, null);

            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(individual)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.GetProperty("Customer"))].Errors.Count(), 0);
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.GetProperty("Contact"))].Errors.Count(), 0);
            //Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.GetProperty("Customer"))].Errors[0].ErrorMessage, "Mandatory");
            Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.GetProperty("Contact"))].Errors[0].ErrorMessage, "Mandatory");

            AssertIsEditViewOf<Individual>(result);
        }


        public void EditFindForObjectMultiCached(Store store) {
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().OrderBy(sp => "").First();
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, salesPerson);
            salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().OrderBy(sp => "").Skip(1).First();
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, salesPerson);

            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, store, null, null, null, "SalesPerson");
        }

        public void EditFindForObjectOneCached(Store store) {
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().First();
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, salesPerson);

            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditFindForObject(Store store) {
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, store, null, null, null, "SalesPerson");
        }

        public void EditSelectForObject(Store store) {
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().First();
            string data = "SalesPerson=" + NakedObjectsFramework.GetObjectId(salesPerson);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), Selector = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditActionAsFindNoParmsForObject(Store store) {
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec rndSpAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "RandomSalesPerson");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=RandomSalesPerson" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            // AssertNameAndParms(result, "ObjectEdit", 1, store, null, salesRepo.Object, rndSpAction, "SalesPerson");

            AssertIsEditViewOf<Store>(result);
        }


        public void EditActionAsFindParmsForObject(Store store) {
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec spAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }


        public void InvokeEditActionAsFindParmsForObject(Store store) {
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec spAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }

        public void InvokeEditActionAsFindParmsForObjectWithParms(Store store) {
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec spAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForFindSalesPersonByName(salesRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            AssertIsEditViewOf<Store>(result);
        }

       
        private void FindForActionUpdatesViewState(bool testValue) {
            IActionSpec action = GetAction(OrderContrib, "CreateNewOrder");
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + OrderContribId +
                          "&spec=AdventureWorksModel.Store" +
                          "&propertyName=cust" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action);
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, Finder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, OrderContrib.Object, action, null, null, "cust");
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }


        public void SelectForActionUpdatesViewState(bool testValue) {
            IActionSpec action = GetAction(OrderContrib, "CreateNewOrder");
            Store customer = NakedObjectsFramework.Persistor.Instances<Store>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "cust=" + NakedObjectsFramework.GetObjectId(customer);
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, Selector = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, OrderContrib.Object, action, null, null, null);
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

       
        public void ActionAsFindParmsForActionUpdatesViewState(bool testValue) {
            IActionSpec action = GetAction(OrderContrib, "CreateNewOrder");

            IActionSpec findByName = CustomerRepo.GetActionLeafNode("FindStoreByName");
            string data = "contextObjectId=" + OrderContribId +
                          "&spec=AdventureWorksModel.Store" +
                          "&propertyName=cust" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindStoreByName" +
                          "&targetObjectId=" + CustomerRepoId;
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, OrderContrib.Object, action, CustomerRepo.Object, findByName, "cust");
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

        private class TestCreator : ICreditCardCreator {
            #region ICreditCardCreator Members

            public void CreatedCardHasBeenSaved(CreditCard card) {
                // do nothing
            }

            #endregion
        }

        [Test]
        public void AAInitialInvokeContributedActionOnEmptyCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False",
                InvokeAction = "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"},
                {"checkboxAll", @"true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;63150;False;;0", "false,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;57033;False;;0", "false,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51798;False;;0", "false,false"},
            };

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(form));

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            var warnings = NakedObjectsFramework.MessageBroker.Warnings.ToArray();
            Assert.AreEqual("No objects selected", warnings.First());
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void ActionAsFindNoParmsForActionReturnMulti() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IActionSpec randomContact = contactRepo.Spec.GetAllActions().Single(a => a.Id == "RandomContacts");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=RandomContacts" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, EmployeeRepo.Object, action, contactRepo.Object, randomContact, "contactDetails");
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void ActionAsFindNoParmsForActionReturnOne() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IActionSpec randomContact = contactRepo.Spec.GetAllActions().Single(a => a.Id == "RandomContact");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=RandomContact" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, EmployeeRepo.Object, action, null, null, null);
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void ActionAsFindParmsForAction() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IActionSpec findByName = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void ActionAsFindParmsForActionUpdatesViewState() {
            ActionAsFindParmsForActionUpdatesViewState(true);
            ActionAsFindParmsForActionUpdatesViewState(false);
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void ActionAsFindParmsForActionWithDefaults() {
            IActionSpec action = GetAction(OrderContributedActions, "FindRate");
            INakedObject orderContribAction = NakedObjectsFramework.GetAdaptedService("OrderContributedActions");
            IActionSpec findByName = orderContribAction.Spec.GetAllActions().Single(a => a.Id == "FindRate");
            string data = "contextObjectId=" + OrderContributedActionsId +
                          "&spec=AdventureWorksModel.CurrencyRate" +
                          "&propertyName=CurrencyRate" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindRate" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(orderContribAction);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = OrderContributedActionsId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, orderContribAction.Object, action, orderContribAction.Object, findByName, "CurrencyRate");

            Assert.AreEqual(result.ViewData["OrderContributedActions-FindRate-Currency-Input"], "US Dollar");
            Assert.AreEqual(result.ViewData["OrderContributedActions-FindRate-Currency1-Input"], "Euro");
        }

        [Test]
        public void ActionFailCrossValidation() {
            Contact contact = Contact;

            INakedObject adaptedContact = NakedObjectsFramework.GetNakedObject(contact);
            IActionSpec action = GetAction(adaptedContact, "ChangePassword");

            FormCollection form = GetFormForChangePassword(adaptedContact, "p1", "p2", "p3");
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(contact), ActionId = action.Id};

            var result = (ViewResult) controller.Action(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.Greater(result.ViewData.ModelState[""].Errors.Count(), 0);
            AssertIsDialogViewOfAction(result, "Change Password");
        }

        [Test]
        public void ActionGet() {
            SalesOrderHeader order = Order;
            IActionSpec action = GetAction(NakedObjectsFramework.GetNakedObject(order), "Recalculate");
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = NakedObjectsFramework.GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel);
            AssertNameAndParms(result, "ActionDialog", null, order, action, null, null, null);
        }

        [Test]
        public void ActionOnNotPersistedObject() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = NakedObjectsFramework.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleAction"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void ActionOnNotPersistedObjectWithReturn() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = NakedObjectsFramework.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleActionWithReturn"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
            Assert.AreEqual("aName", ((NotPersistedObject) result.ViewData.Model).Name);
        }

        [Test]
        public void CrossFieldValidationFail() {
            IDictionary<string, string> idToRawvalue;
            IObjectSpec ccSpec = NakedObjectsFramework.Metamodel.GetSpecification(typeof (CreditCard));
            INakedObject cc = NakedObjectsFramework.LifecycleManager.CreateInstance(ccSpec);

            FormCollection form = GetFormForCeditCardEdit(cc, "Vista", "12345", "1", "2010", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(cc)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.Greater(result.ViewData.ModelState[""].Errors.Count(), 0);
            AssertIsEditViewOf<CreditCard>(result);
        }

        [Test]
        public void CrossFieldValidationSuccess() {
            IDictionary<string, string> idToRawvalue;
            IObjectSpec ccSpec = NakedObjectsFramework.Metamodel.GetSpecification(typeof (CreditCard));
            INakedObject cc = NakedObjectsFramework.LifecycleManager.CreateInstance(ccSpec);
            cc.GetDomainObject<CreditCard>().Creator = new TestCreator();

            FormCollection form = GetFormForCeditCardEdit(cc, "Vista", "12345", "1", "2020", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(cc)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            Assert.IsTrue(result.ViewData.ModelState.IsValid);
            AssertIsDetailsViewOf<CreditCard>(result);
        }

        [Test]
        public void EditActionAsFindNoParmsForObject() {
            EditActionAsFindNoParmsForObject(Store);
        }

        [Test]
        public void EditActionAsFindNoParmsForObjectForTransient() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            EditActionAsFindNoParmsForObject(store);
        }

        [Test]
        public void EditActionAsFindParmsForObject() {
            EditActionAsFindParmsForObject(Store);
        }

        [Test]
        public void EditActionAsFindParmsForObjectForTransient() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            EditActionAsFindParmsForObject(store);
        }

        [Test]
        public void EditFindForObject() {
            EditFindForObject(Store);
        }

        [Test]
        public void EditFindForObjectForTransient() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            EditFindForObject(store);
        }

        [Test]
        public void EditFindForObjectMultiCached() {
            EditFindForObjectMultiCached(Store);
        }

        [Test]
        public void EditFindForObjectOneCached() {
            EditFindForObjectOneCached(Store);
        }

        [Test]
        public void EditInlineSaveValidationFail() {
            EditInlineSaveValidationFail(Employee.DepartmentHistory.First().Shift);
        }

        [Test]
        public void EditInlineSaveValidationFailForTransient() {
            EditInlineSaveValidationFail(TransientShift);
        }

        [Test]
        public void EditInlineSaveValidationOk() {
            EditInlineSaveValidationOk(Employee.DepartmentHistory.First().Shift);
        }

        [Test, Ignore] // todo make more reliable
        public void EditInlineSaveValidationOkForTransient() {
            EditInlineSaveValidationOk(TransientShift);
        }

        [Test]
        public void EditObject() {
            var objectModel = new ObjectAndControlData {Id = EmployeeId};
            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectGet() {
            var objectModel = new ObjectAndControlData {Id = EmployeeId};
            var result = (ViewResult) controller.EditObject(objectModel);
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectGetWithInline() {
            Shift shift = Employee.DepartmentHistory.First().Shift;

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(shift)};
            var result = (ViewResult) controller.EditObject(objectModel);
            AssertIsEditViewOf<Shift>(result);
        }

        [Test]
        public void EditObjectKeepTableFormat() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list&DepartmentHistory=table"}});
            var objectModel = new ObjectAndControlData {Id = EmployeeId};

            var result = (ViewResult) controller.EditObject(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }

        [Test]
        public void EditObjectNotPersisted() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = NakedObjectsFramework.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId};

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void EditObjectPage() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;Cycling",
                Pager = "page=1&pageSize=3"
            };

            var form = new FormCollection {
                {"checkboxAll", "false"},
                {"AdventureWorksModel.Store;AdventureWorksModel.Store;1;System.Int32;298;False;;0", "false"},
                {"AdventureWorksModel.Store;AdventureWorksModel.Store;1;System.Int32;421;False;;0", "false"},
                {"AdventureWorksModel.Store;AdventureWorksModel.Store;1;System.Int32;478;False;;0", "false"},
                {"Pager", "page=1&pageSize=3"}
            };

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsQueryableViewOf<Store>(result);

            AssertPagingData(result, 1, 3, 8);
        }

        [Test]
        public void EditObjectRedisplay() {
            var objectModel = new ObjectAndControlData {
                Id = NakedObjectsFramework.GetObjectId(TransientEmployee),
                Redisplay = "editMode=true"
            };

            var form = new FormCollection {};

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectWithInline() {
            Shift shift = Employee.DepartmentHistory.First().Shift;

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(shift)};
            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<Shift>(result);
        }

        [Test]
        public void EditRedisplay() {
            EditRedisplay(Employee);
        }

        [Test]
        public void EditRedisplayForTransient() {
            EditRedisplay(TransientEmployee);
        }

        [Test]
        public void EditRefreshTransient() {
            const string redisplay = "DepartmentHistory=table&editMode=True";
            Employee employee = TransientEmployee;
            Employee report1 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderBy(e => e.EmployeeID).Skip(1).First();
            Employee report2 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderBy(e => e.EmployeeID).Skip(2).First();
            INakedObject employeeNakedObject = NakedObjectsFramework.GetNakedObject(employee);
            IAssociationSpec collectionAssoc = employeeNakedObject.Spec.Properties.Single(p => p.Id == "DirectReports");

            var form = new FormCollection {
                {IdHelper.DisplayFormatFieldId, "Addresses=list"},
                {IdHelper.GetCollectionItemId(employeeNakedObject, collectionAssoc), NakedObjectsFramework.GetObjectId(report1)},
                {IdHelper.GetCollectionItemId(employeeNakedObject, collectionAssoc), NakedObjectsFramework.GetObjectId(report2)}
            };

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
            Assert.AreEqual(2, ((Employee) result.ViewData.Model).DirectReports.Count);
        }

        [Test]
        public void EditSaveConcurrencyFail() {
            Store store = Store;
            INakedObject adaptedStore = NakedObjectsFramework.Manager.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;
            string differentDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), differentDateTime, out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                controller.Edit(objectModel, form);

                Assert.Fail("Expect concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(adaptedStore, expected.SourceNakedObject);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        [Test]
        public void EditSaveConcurrencyOk() {
            Store store = Store;
            INakedObject adaptedStore = NakedObjectsFramework.Manager.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.InvariantCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Store>(result);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        [Test]
        public void EditSaveValidationFail() {
            EditSaveValidationFail(Vendor);
        }

        [Test]
        public void EditSaveValidationFailEmptyForm() {
            EditSaveValidationFailEmptyForm(Individual);
        }

        [Test]
        public void EditSaveValidationFailForTransient() {
            EditSaveValidationFail(TransientVendor);
        }

        [Test]
        public void EditSaveValidationFailForTransientEmptyForm() {
            EditSaveValidationFailEmptyForm(TransientIndividual);
        }

        [Test]
        public void EditSaveValidationOk() {
            EditSaveValidationOk(Vendor);
        }

        [Test, Ignore] // todo make more reliable
        public void EditSaveValidationOkForTransient() {
            EditSaveValidationOk(TransientVendor);
        }

        [Test]
        public void EditSelectForObject() {
            EditSelectForObject(Store);
        }

        [Test]
        public void EditSelectForObjectForTransient() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            EditSelectForObject(store);
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void FindForAction() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action);
            FormCollection form = GetFormForCreateNewEmployeeFromContact(action, "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, Finder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, EmployeeRepo.Object, action, null, null, "ContactDetails");
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void FindForActionUpdatesViewState() {
            FindForActionUpdatesViewState(true);
            FindForActionUpdatesViewState(false);
        }

        [Test]
        public void GetFile() {
            Product product = Product;

            FileContentResult result = controller.GetFile(NakedObjectsFramework.GetObjectId(Product), "Photo");

            Stream stream = product.Photo.GetResourceAsStream();
            byte[] bytes;
            using (var br = new BinaryReader(stream)) {
                bytes = br.ReadBytes((int) stream.Length);
            }

            Assert.AreEqual(bytes, result.FileContents);
        }

        [Test]
        public void InitialInvokeContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False",
                InvokeAction = "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"},
                {"checkboxAll", @"true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;71936;False;;0", "true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;57033;False;;0", "true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51798;False;;0", "true,false"},
            };

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Append Comment");
        }

        // run first

        [Test]
        [Ignore] // fix !!
        public void InitialInvokeCovariantContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling",
                InvokeAction = "targetActionId=ShowCustomersWithAddressInRegion&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextActionId=&propertyName=&page=1&pageSize=2"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=ShowCustomersWithAddressInRegion&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextActionId=&propertyName=&page=1&pageSize=2"},
                {"checkboxAll", @"true,false"},
                {"AdventureWorksModel.Store;1;System.Int32;298;False;;0", "true,false"},
                {"AdventureWorksModel.Store;1;System.Int32;421;False;;0", "true,false"},
                {"AdventureWorksModel.Store;1;System.Int32;478;False;;0", "true,false"},
                {"AdventureWorksModel.Store;1;System.Int32;556;False;;0", "true,false"}
            };
            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Show Customers With Address In Region");
        }


        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void InvokeActionAsFindParmsForAction() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IActionSpec findByName = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void InvokeActionAsFindParmsForActionWithParms() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IActionSpec findByName = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetFormForFindContactByName(contactRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 11, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void InvokeActionAsSaveForActionFailValidation() {
            Store store = Store;
            Store transientStore = TransientStore;
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            IActionSpec findByName = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + NakedObjectsFramework.GetObjectId(transientStore) +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", NakedObjectsFramework.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test, Ignore] // Fix by local cache of spec/action etc in MetaModelManager
        public void InvokeActionAsSaveForActionPassValidation() {
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            IActionSpec findByName = contactRepo.Spec.GetAllActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + NakedObjectsFramework.GetObjectId(transientVendor) +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(contactRepo);
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsFramework.Manager.CreateAdapter(transientVendor, null, null);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeActionWithMultiSelectObjects() {
            string id = NakedObjectsFramework.GetObjectId(Order);


            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasons",
                Id = id
            };

            var form = new FormCollection();

            form.Add("SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;1;False;;0");
            form.Add("SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;2;False;;0");

            INakedObject order = NakedObjectsFramework.Manager.CreateAdapter(Order, null, null);
            IAssociationSpec assocMD = order.Spec.GetProperty("ModifiedDate");
            IActionSpec action = order.GetActionLeafNode("AddNewSalesReasons");


            string idMD = IdHelper.GetConcurrencyActionInputId(order, action, assocMD);

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.InvariantCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeActionWithMultiSelectParseable() {
            string id = NakedObjectsFramework.GetObjectId(Order);


            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasonsByCategories",
                Id = id
            };

            var form = new FormCollection();

            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"1");
            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"2");

            INakedObject order = NakedObjectsFramework.Manager.CreateAdapter(Order, null, null);
            IAssociationSpec assocMD = order.Spec.GetProperty("ModifiedDate");
            IActionSpec action = order.GetActionLeafNode("AddNewSalesReasonsByCategories");


            string idMD = IdHelper.GetConcurrencyActionInputId(order, action, assocMD);

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.InvariantCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                ActionId = "AppendComment",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-CommentToAppend-Input", "comment"},
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"}
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(form));

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
        }


        [Test]
        public void InvokeContributedActionOnCollectionTargetValidateFails() {
            var objectModel = new ObjectAndControlData {
                ActionId = "AppendComment",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-CommentToAppend-Input", ""},
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;71793;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;65219;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;55270;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51083;False;;0", "true"}
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Append Comment");
        }

        [Test]
        public void InvokeContributedActionOnCollectionTargetValidateFailsSingleParm() {
            var objectModel = new ObjectAndControlData {
                ActionId = "CommentAsUsersUnhappy",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;67293;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;61194;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;55270;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51083;False;;0", "true"}
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Comment As Users Unhappy");
        }

        [Test]
        public void InvokeContributedActionOnTarget() {
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "LastOrder",
                Id = NakedObjectsFramework.GetObjectId(store),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(
                new Dictionary<string, string> {
                    {"Store-LastOrder-ModifiedDate-Concurrency", store.ModifiedDate.ToString(CultureInfo.InvariantCulture)}
                }));

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeContributedActionOnTargetConcurrencyFail() {
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "LastOrder",
                Id = NakedObjectsFramework.GetObjectId(store),
                InvokeAction = "action=action"
            };

            try {
                var result = (ViewResult) controller.Action(objectModel, GetForm(
                    new Dictionary<string, string> {
                        {"Store-LastOrder-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.InvariantCulture)}
                    }));

                Assert.Fail("Expected concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(store, expected.SourceNakedObject.Object);
            }
        }


        [Test]
        public void InvokeContributedActionOnTargetPopulatesTargetParm() {
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "CreateNewOrder",
                Id = NakedObjectsFramework.GetObjectId(store),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(
                new Dictionary<string, string> {
                    {"Store-CreateNewOrder-ModifiedDate-Concurrency", store.ModifiedDate.ToString(CultureInfo.InvariantCulture)}
                }));

            AssertIsDialogViewOfAction(result, "Create New Order");
            Assert.IsTrue(result.ViewData.ContainsKey("OrderContributedActions-CreateNewOrder-Customer-Select"));
            Assert.AreEqual(NakedObjectsFramework.GetNakedObject(store), result.ViewData["OrderContributedActions-CreateNewOrder-Customer-Select"]);
        }

        [Test]
        public void InvokeEditActionAsFindParmsForObject() {
            InvokeEditActionAsFindParmsForObject(Store);
        }

        [Test]
        public void InvokeEditActionAsFindParmsForObjectForTransient() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            InvokeEditActionAsFindParmsForObject(store);
        }

        [Test]
        public void InvokeEditActionAsFindParmsForObjectForTransientWithParms() {
            Store store = TransientStore;
            store.Name = "Aname";
            store.SalesPerson = SalesPerson;

            InvokeEditActionAsFindParmsForObjectWithParms(store);
        }

        [Test]
        public void InvokeEditActionAsFindParmsForObjectWithParms() {
            InvokeEditActionAsFindParmsForObjectWithParms(Store);
        }

        [Test]
        public void InvokeEditActionAsSaveForObjectFailValidation() {
            Store store = Store;
            Store transientStore = TransientStore;
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec spAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&subEditObjectId=" + NakedObjectsFramework.GetObjectId(transientStore) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", NakedObjectsFramework.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeEditActionAsSaveForObjectPassValidation() {
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            INakedObject adaptedStore = NakedObjectsFramework.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = NakedObjectsFramework.GetAdaptedService("SalesRepository");
            IActionSpec spAction = salesRepo.Spec.GetAllActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + NakedObjectsFramework.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + NakedObjectsFramework.GetObjectId(salesRepo) +
                          "&subEditObjectId=" + NakedObjectsFramework.GetObjectId(transientVendor) +
                          "&contextActionId=";

            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsFramework.Manager.CreateAdapter(transientVendor, null, null);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSet() {
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleAction", Id = NakedObjectsFramework.GetObjectId(obj)};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSetWithReturn() {
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleActionWithReturn", Id = NakedObjectsFramework.GetObjectId(obj)};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<NotPersistedObject>(result);
            Assert.AreEqual("aName", ((NotPersistedObject) result.ViewData.Model).Name);
        }


        [Test]
        public void InvokeObjectActionDefaultSet() {
            var objectModel = new ObjectAndControlData {ActionId = "CreateNewOrder", Id = OrderContribId, InvokeAction = "action=action"};

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string>()));

            AssertIsDialogViewOfAction(result, "Create New Order");

            Assert.IsTrue(result.ViewData.ContainsKey("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            Assert.AreEqual(true, result.ViewData["OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"]);
        }

        [Test]
        public void InvokeObjectActionNoParms() {
            SalesOrderHeader order = Order;
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = NakedObjectsFramework.GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string> {{"SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", order.ModifiedDate.ToString(CultureInfo.InvariantCulture)}}));

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeObjectActionParmsNotSet() {
            INakedObject adaptedProduct = NakedObjectsFramework.Manager.CreateAdapter(Product, null, null);
            FormCollection form = GetFormForBestSpecialOffer(adaptedProduct, "");
            var objectModel = new ObjectAndControlData {ActionId = "BestSpecialOffer", Id = ProductId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDialogViewOfAction(result, "Best Special Offer");

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Product-BestSpecialOffer-Quantity-Input"));
            Assert.AreEqual("Mandatory", result.ViewData.ModelState["Product-BestSpecialOffer-Quantity-Input"].Errors[0].ErrorMessage);
        }

        [Test]
        public void InvokeObjectActionParmsSet() {
            INakedObject adaptedProduct = NakedObjectsFramework.Manager.CreateAdapter(Product, null, null);
            FormCollection form = GetFormForBestSpecialOffer(adaptedProduct, "1");
            var objectModel = new ObjectAndControlData {ActionId = "BestSpecialOffer", Id = ProductId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SpecialOffer>(result);
        }

        [Test]
        public void InvokeObjectActionReturnCollectionOfOneItem() {
            FormCollection form = GetFormForListProductsBySubCategory(ProductRepo, GetBoundedId<ProductSubcategory>("Hydration Packs"));
            var objectModel = new ObjectAndControlData {ActionId = "ListProductsBySubCategory", Id = ProductRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<Product>(result);
        }

        [Test]
        public void InvokeObjectActionReturnOrderedPagedCollectionAsc() {
            FormCollection form = GetForm(new Dictionary<string, string> {
                {"OrderRepository-OrdersByValue-Ordering-Input", "Ascending"}
            });
            var objectModel = new ObjectAndControlData {ActionId = "OrdersByValue", Id = OrderRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) result.ViewData.Model).Count());

            var arm = (ActionResultModel) result.ViewData.Model;

            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) arm.Result).Count());
            Assert.AreEqual("OrdersByValue", arm.Action.Id);

            AssertPagingData(result, 1, 20, 31465);
        }

        [Test]
        public void InvokeObjectActionReturnOrderedPagedCollectionDesc() {
            FormCollection form = GetForm(new Dictionary<string, string> {
                {"OrderRepository-OrdersByValue-Ordering-Input", "Descending"}
            });
            var objectModel = new ObjectAndControlData {ActionId = "OrdersByValue", Id = OrderRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) result.ViewData.Model).Count());

            var arm = (ActionResultModel) result.ViewData.Model;

            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) arm.Result).Count());
            Assert.AreEqual("OrdersByValue", arm.Action.Id);

            AssertPagingData(result, 1, 20, 31465);
        }

        [Test]
        public void InvokeObjectActionReturnPagedCollection() {
            var form = new FormCollection();
            var objectModel = new ObjectAndControlData {ActionId = "HighestValueOrders", Id = OrderRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) result.ViewData.Model).Count());

            var arm = (ActionResultModel) result.ViewData.Model;

            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) arm.Result).Count());
            Assert.AreEqual("HighestValueOrders", arm.Action.Id);

            AssertPagingData(result, 1, 20, 31465);
        }

        [Test]
        public void InvokeServiceActionMandatoryParmNotSet() {
            FormCollection form = GetFormForFindEmployeeByName(EmployeeRepo, "", "");
            var objectModel = new ObjectAndControlData {ActionId = "FindEmployeeByName", Id = EmployeeRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDialogViewOfAction(result, "Find Employee By Name");

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("EmployeeRepository-FindEmployeeByName-LastName-Input"));
            Assert.AreEqual("Mandatory", result.ViewData.ModelState["EmployeeRepository-FindEmployeeByName-LastName-Input"].Errors[0].ErrorMessage);
            //Assert.IsTrue(result.ViewData.ModelState.ContainsKey("EmployeeRepository-FindEmployeeByName-FirstName-Input"));
            //Assert.IsFalse(result.ViewData.ModelState["EmployeeRepository-FindEmployeeByName-FirstName-Input"].Errors.Any());
        }

        [Test]
        public void InvokeServiceActionNoParms() {
            var objectModel = new ObjectAndControlData {
                ActionId = "RandomEmployee",
                Id = EmployeeRepoId,
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string>()));

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void InvokeServiceActionOptionalParmNotSet() {
            FormCollection form = GetFormForFindEmployeeByName(EmployeeRepo, "", "Smith");
            var objectModel = new ObjectAndControlData {ActionId = "FindEmployeeByName", Id = EmployeeRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<Employee>(result);
        }

        [Test]
        public void InvokeServiceActionParmsSet() {
            FormCollection form = GetFormForFindEmployeeByName(EmployeeRepo, "S", "Smith");
            var objectModel = new ObjectAndControlData {ActionId = "FindEmployeeByName", Id = EmployeeRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<Employee>(result);
        }

        [Test]
        public void SelectForAction() {
            IActionSpec action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            Contact contactDetails = NakedObjectsFramework.Persistor.Instances<Contact>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "ContactDetails=" + NakedObjectsFramework.GetObjectId(contactDetails);
            FormCollection form = GetFormForCreateNewEmployeeFromContact(action, "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, Selector = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, EmployeeRepo.Object, action, null, null, null);
        }

        [Test]
        public void SelectForActionUpdatesViewState() {
            SelectForActionUpdatesViewState(true);
            SelectForActionUpdatesViewState(false);
        }

        [Test]
        [Ignore] // fix !!
        public void ViewCollectionDisplay() {
            FormCollection form = GetForm(new Dictionary<string, string> {
                {"CustomerRepository-ShowCustomersWithAddressInRegion-Region-Select", ""},
                {"Details", "id=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;True;System.String;cycling&page=1&pageSize=2"},
                {"CustomerRepository-ShowCustomersWithAddressInRegion-Customers-Select", "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;True;System.String;cycling"},
                {"AdventureWorksModel.Store;1;System.Int32;298;False;;0", "true"},
                {"AdventureWorksModel.Store;1;System.Int32;421;False;;0", "true"}
            });

            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling",
                Details = "id=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.EntityObjectStore.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&page=1&pageSize=2"
            };

            GetTestService("Customers");

            var result = (ViewResult) controller.Details(objectModel, form);

            AssertIsCollectionViewOf<Store>(result);
            Assert.AreEqual(2, ((IEnumerable<Store>) result.ViewData.Model).Count());
            AssertPagingData(result, 1, 2, 2);
        }

        [Test]
        public void ViewObjectDetails() {
            var objectModel = new ObjectAndControlData {Id = EmployeeId};

            var result = (ViewResult) controller.Details(objectModel);

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsCancel() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = EmployeeId, Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsCancelTransient() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(TransientEmployee), Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);
            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsRedisplay() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string redisplay = "DepartmentHistory=table";
            var objectModel = new ObjectAndControlData {Id = EmployeeId, Redisplay = redisplay};

            var result = (ViewResult) controller.Details(objectModel, form);

            AssertIsSetAfterTransactionViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }

        [Test]
        public void ViewObjectRedisplay() {
            var objectModel = new ObjectAndControlData {
                Id = NakedObjectsFramework.GetObjectId(TransientEmployee),
                Redisplay = "editMode=false"
            };

            var form = new FormCollection {};

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<Employee>(result);
        }

        [Test]
        public void ViewServiceDetails() {
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId};

            var result = (ViewResult) controller.Details(objectModel);

            AssertIsSetAfterTransactionViewOf<EmployeeRepository>(result);
        }
    }

    [TestFixture]
    public class ConcurrencyTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new GenericController(NakedObjectsFramework);
            mocks = new ContextMocks(controller);
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingEdmxContext("Model");
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
        }

        private GenericController controller;
        private ContextMocks mocks;


        public Store Store {
            get { return NakedObjectsFramework.Persistor.Instances<Store>().First(); }
        }

        public FormCollection GetFormForStoreEdit(INakedObject store,
            string storeName,
            string salesPerson,
            string modifiedDate,
            out IDictionary<string, string> idToRawValue) {
            IObjectSpec nakedObjectSpecification = NakedObjectsFramework.Metamodel.GetSpecification(typeof (Store));
            IAssociationSpec assocSN = nakedObjectSpecification.GetProperty("Name");
            IAssociationSpec assocSP = nakedObjectSpecification.GetProperty("SalesPerson");
            IAssociationSpec assocMD = nakedObjectSpecification.GetProperty("ModifiedDate");

            string idSN = IdHelper.GetFieldInputId(store, assocSN);
            string idSP = IdHelper.GetFieldInputId(store, assocSP);
            string idMD = IdHelper.GetConcurrencyFieldInputId(store, assocMD);

            idToRawValue = new Dictionary<string, string> {
                {idSN, storeName},
                {idSP, salesPerson},
                {idMD, modifiedDate}
            };

            return GetForm(idToRawValue);
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
            get { return (new object[] {new OrderContributedActions()}); }
        }


        private SalesOrderHeader Order {
            get { return NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().First(); }
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
            INakedObject adaptedStore = NakedObjectsFramework.Manager.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;

            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, NakedObjectsFramework.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.InvariantCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = NakedObjectsFramework.GetObjectId(store)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            var conn = new SqlConnection(@"Data Source=" + Constants.Server + @";Initial Catalog=AdventureWorks;Integrated Security=True");

            conn.Open();


            try {
                controller.Edit(objectModel, form);

                // change store in database 

                string updateStore = string.Format("update Sales.Store set ModifiedDate = GETDATE() where Name = '{0}'", store.Name);

                string updateCustomer = string.Format("update Sales.Customer set ModifiedDate = GETDATE() From Sales.Store as ss inner join Sales.Customer as sc on ss.CustomerID = sc.CustomerID  where ss.Name = '{0}'", store.Name);

                using (var cmd = new SqlCommand(updateStore) {Connection = conn}) {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand(updateCustomer) {Connection = conn}) {
                    cmd.ExecuteNonQuery();
                }

                NakedObjectsFramework.TransactionManager.EndTransaction();

                Assert.Fail("Expect concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(store, expected.SourceNakedObject.Object);
            }
            finally {
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
                controller.Action(objectModel, GetForm(new Dictionary<string, string> {{"SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.InvariantCulture)}}));
                Assert.Fail("Expected concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(order, expected.SourceNakedObject.Object);
            }
        }
    }
}