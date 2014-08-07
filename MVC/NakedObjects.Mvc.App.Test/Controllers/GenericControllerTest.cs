// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using MvcTestApp.Tests.Util;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.App.Controllers;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class GenericControllerTest : AcceptanceTestCase {

        [TestFixtureSetUp]
        public void SetupTest() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
            InitializeNakedObjectsFramework();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public void StartTest() {
           
        }

        [TearDown]
        public void EndTest() {
            NakedObjectsContext.ObjectPersistor.Reset();
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
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

        protected override IServicesInstaller ContributedActions {
            get {
                return new ServicesInstaller(new object[] {
                    new OrderContributedActions(),
                    new CustomerContributedActions()
                });
            }
        }


        protected override IObjectPersistorInstaller Persistor {
            get {
                var installer = new EntityPersistorInstaller();
                installer.ForceContextSet();
                return installer;
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

        private static void AssertIsDialogViewOfAction(ViewResult result, string actionName) {
            Assert.AreEqual("ActionDialog", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof (FindViewModel), data.Model);
            Assert.AreEqual(actionName, ((FindViewModel) data.Model).ContextAction.Name);
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
            return FrameworkHelper.GetObjectId(GetBoundedInstance<T>(title).GetDomainObject());
        }

        private static FormCollection GetFormForFindEmployeeByName(INakedObject employeeRepo, string firstName, string secondName) {
            INakedObjectAction actionFindEmployeeByname = employeeRepo.Specification.GetObjectActions().Single(a => a.Id == "FindEmployeeByName");

            INakedObjectActionParameter parmFirstName = actionFindEmployeeByname.Parameters[0];
            INakedObjectActionParameter parmSecondName = actionFindEmployeeByname.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(actionFindEmployeeByname, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(actionFindEmployeeByname, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private static FormCollection GetFormForFindSalesPersonByName(INakedObject salesRepo, string firstName, string secondName) {
            INakedObjectAction action = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");

            INakedObjectActionParameter parmFirstName = action.Parameters[0];
            INakedObjectActionParameter parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(action, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(action, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private static FormCollection GetFormForFindContactByName(INakedObject contactRepo, string firstName, string secondName) {
            INakedObjectAction action = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");

            INakedObjectActionParameter parmFirstName = action.Parameters[0];
            INakedObjectActionParameter parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId(action, parmFirstName);
            string idSecondName = IdHelper.GetParameterInputId(action, parmSecondName);

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }


        private static FormCollection GetFormForBestSpecialOffer(INakedObject productRepo, string quantity) {
            INakedObjectAction action = productRepo.Specification.GetObjectActions().Single(a => a.Id == "BestSpecialOffer");
            INakedObjectActionParameter parmQuantity = action.Parameters[0];
            string idQuantity = IdHelper.GetParameterInputId(action, parmQuantity);
            return GetForm(new Dictionary<string, string> {
                {idQuantity, quantity},
            });
        }

        private static FormCollection GetFormForChangePassword(INakedObject contact, string p1, string p2, string p3) {
            INakedObjectAction action = contact.Specification.GetObjectActions().Single(a => a.Id == "ChangePassword");
            INakedObjectActionParameter pp1 = action.Parameters[0];
            INakedObjectActionParameter pp2 = action.Parameters[1];
            INakedObjectActionParameter pp3 = action.Parameters[2];

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
            INakedObjectAction action = productRepo.Specification.GetObjectActions().Single(a => a.Id == "ListProductsBySubCategory");
            INakedObjectActionParameter parmPsc = action.Parameters[0];
            string idPsc = IdHelper.GetParameterInputId(action, parmPsc);

            return GetForm(new Dictionary<string, string> {
                {idPsc, pscId},
            });
        }

        private static FormCollection GetFormForShiftEdit(INakedObject shift,
                                                          INakedObject timePeriod,
                                                          string t1,
                                                          string t2,
                                                          out IDictionary<string, string> idToRawValue) {
            INakedObjectSpecification shiftSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (Shift));
            INakedObjectSpecification timePeriodSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (TimePeriod));

            INakedObjectAssociation assocN = shiftSpec.GetProperty("Name");
            INakedObjectAssociation assocTp = shiftSpec.GetProperty("Times");

            INakedObjectAssociation assocT1 = timePeriodSpec.GetProperty("StartTime");
            INakedObjectAssociation assocT2 = timePeriodSpec.GetProperty("EndTime");

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


        private static FormCollection GetFormForVendorEdit(INakedObject vendor,
                                                           string accountNumber,
                                                           string name,
                                                           string creditRating,
                                                           string preferredVendorStatus,
                                                           string activeFlag,
                                                           string purchasingWebServiceURL,
                                                           out IDictionary<string, string> idToRawValue) {
            INakedObjectSpecification nakedObjectSpecification = NakedObjectsContext.Reflector.LoadSpecification(typeof (Vendor));
            INakedObjectAssociation assocAN = nakedObjectSpecification.GetProperty("AccountNumber");
            INakedObjectAssociation assocN = nakedObjectSpecification.GetProperty("Name");
            INakedObjectAssociation assocCR = nakedObjectSpecification.GetProperty("CreditRating");
            INakedObjectAssociation assocPVS = nakedObjectSpecification.GetProperty("PreferredVendorStatus");
            INakedObjectAssociation assocAF = nakedObjectSpecification.GetProperty("ActiveFlag");
            INakedObjectAssociation assocPWSURL = nakedObjectSpecification.GetProperty("PurchasingWebServiceURL");

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

        public static FormCollection GetFormForStoreEdit(INakedObject store,
                                                         string storeName,
                                                         string salesPerson,
                                                         string modifiedDate,
                                                         out IDictionary<string, string> idToRawValue) {
            INakedObjectSpecification nakedObjectSpecification = NakedObjectsContext.Reflector.LoadSpecification(typeof (Store));
            INakedObjectAssociation assocSN = nakedObjectSpecification.GetProperty("Name");
            INakedObjectAssociation assocSP = nakedObjectSpecification.GetProperty("SalesPerson");
            INakedObjectAssociation assocMD = nakedObjectSpecification.GetProperty("ModifiedDate");

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

        private static FormCollection GetFormForCeditCardEdit(INakedObject creditCard,
                                                              string cardType,
                                                              string cardNumber,
                                                              string expiryMonth,
                                                              string expiryYear,
                                                              out IDictionary<string, string> idToRawValue) {
            INakedObjectSpecification nakedObjectSpecification = NakedObjectsContext.Reflector.LoadSpecification(typeof (CreditCard));
            INakedObjectAssociation assocCT = nakedObjectSpecification.GetProperty("CardType");
            INakedObjectAssociation assocCN = nakedObjectSpecification.GetProperty("CardNumber");
            INakedObjectAssociation assocEM = nakedObjectSpecification.GetProperty("ExpMonth");
            INakedObjectAssociation assocEY = nakedObjectSpecification.GetProperty("ExpYear");

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


        private static FormCollection GetFormForCreateNewEmployeeFromContact(INakedObjectAction action, string contact, out IDictionary<string, string> idToRawValue) {
            INakedObjectActionParameter parmContact = action.Parameters[0];
            string idContact = IdHelper.GetParameterInputId(action, parmContact);
            idToRawValue = new Dictionary<string, string> {
                {idContact, contact},
            };
            return GetForm(idToRawValue);
        }

        private static FormCollection GetFormForCreateNewOrder(INakedObjectAction action, string cust, bool copy, out IDictionary<string, string> idToRawValue) {
            INakedObjectActionParameter parmCust = action.Parameters[0];
            INakedObjectActionParameter parmCopy = action.Parameters[1];

            string idCust = IdHelper.GetParameterInputId(action, parmCust);
            string idCopy = IdHelper.GetParameterInputId(action, parmCopy);

            idToRawValue = new Dictionary<string, string> {
                {idCust, cust},
                {idCopy, copy.ToString()},
            };
            return GetForm(idToRawValue);
        }


        private static Employee Employee {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Employee>().First(); }
        }

        private static string EmployeeId {
            get { return FrameworkHelper.GetObjectId(Employee); }
        }

        private static Employee TransientEmployee {
            get { return NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Employee))).GetDomainObject<Employee>(); }
        }

        private static Vendor TransientVendor {
            get { return NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Vendor))).GetDomainObject<Vendor>(); }
        }

        private static Shift TransientShift {
            get { return NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Shift))).GetDomainObject<Shift>(); }
        }

        private static Individual TransientIndividual {
            get { return NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Individual))).GetDomainObject<Individual>(); }
        }

        private static NotPersistedObject NotPersistedObject {
            get {
                var repo = FrameworkHelper.GetAdaptedService("repository#MvcTestApp.Tests.Controllers.NotPersistedObject").Object as SimpleRepository<NotPersistedObject>;
                return repo.NewInstance();
            }
        }

        private static SalesOrderHeader Order {
            get { return NakedObjectsContext.ObjectPersistor.Instances<SalesOrderHeader>().First(); }
        }

        private static string OrderId {
            get { return FrameworkHelper.GetObjectId(Order); }
        }

        private static Vendor Vendor {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Vendor>().First(); }
        }

        private static Contact Contact {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Contact>().First(); }
        }

        private static Individual Individual {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Individual>().First(); }
        }

        private static Product Product {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Product>().First(); }
        }

        private static string ProductId {
            get { return FrameworkHelper.GetObjectId(Product); }
        }


        private static INakedObject EmployeeRepo {
            get { return FrameworkHelper.GetAdaptedService("EmployeeRepository"); }
        }

        private static INakedObject OrderContributedActions {
            get { return FrameworkHelper.GetAdaptedService("OrderContributedActions"); }
        }

        private static string OrderContributedActionsId {
            get { return FrameworkHelper.GetObjectId(OrderContributedActions); }
        }

        private static INakedObject ProductRepo {
            get { return FrameworkHelper.GetAdaptedService("ProductRepository"); }
        }

        private static string EmployeeRepoId {
            get { return FrameworkHelper.GetObjectId(EmployeeRepo); }
        }

        private static string ProductRepoId {
            get { return FrameworkHelper.GetObjectId(ProductRepo); }
        }

        private static INakedObject OrderRepo {
            get { return FrameworkHelper.GetAdaptedService("OrderRepository"); }
        }

        private static string OrderRepoId {
            get { return FrameworkHelper.GetObjectId(OrderRepo); }
        }

        private static INakedObject OrderContrib {
            get { return FrameworkHelper.GetAdaptedService("OrderContributedActions"); }
        }

        private static string OrderContribId {
            get { return FrameworkHelper.GetObjectId(OrderContrib); }
        }

        private static INakedObject CustomerRepo {
            get { return FrameworkHelper.GetAdaptedService("CustomerRepository"); }
        }

        private static string CustomerRepoId {
            get { return FrameworkHelper.GetObjectId(CustomerRepo); }
        }

        public static Store Store {
            get { return NakedObjectsContext.ObjectPersistor.Instances<Store>().First(); }
        }

        private static string StoreId {
            get { return FrameworkHelper.GetObjectId(Store); }
        }

        private static SalesPerson SalesPerson {
            get { return NakedObjectsContext.ObjectPersistor.Instances<SalesPerson>().First(); }
        }

        private static Store TransientStore {
            get { return NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Store))).GetDomainObject<Store>(); }
        }


        private static INakedObjectAction GetAction(INakedObject owner, string id) {
            return owner.Specification.GetObjectActions().Single(a => a.Id == id);
        }

        private static void AssertNameAndParms(ViewResult result, string name, int? count, object contextObject, INakedObjectAction contextAction, object targetObject, INakedObjectAction targetAction, string pName) {
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
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string redisplay = "DepartmentHistory=table&editMode=True";
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }


        public void EditSaveValidationOk(Vendor vendor) {
            var controller = new GenericController();
            new ContextMocks(controller);
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsContext.ObjectPersistor.CreateAdapter(vendor, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(vendor)};

            NakedObjectsContext.ObjectPersistor.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Vendor>(result);
            }
            finally {
                NakedObjectsContext.ObjectPersistor.EndTransaction();
            }
        }

        public void EditInlineSaveValidationOk(Shift shift) {
            var controller = new GenericController();
            new ContextMocks(controller);

            INakedObject adaptedShift = NakedObjectsContext.ObjectPersistor.CreateAdapter(shift, null, null);
            INakedObject adaptedTimePeriod = NakedObjectsContext.ObjectPersistor.CreateAdapter(shift.Times, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.ToString(), DateTime.Now.ToString(), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(shift)};

            NakedObjectsContext.ObjectPersistor.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Shift>(result);
            }
            finally {
                NakedObjectsContext.ObjectPersistor.EndTransaction();
            }
        }


        public void EditSaveValidationFail(Vendor vendor) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedVendor = NakedObjectsContext.ObjectPersistor.CreateAdapter(vendor, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, "", "", "", "", "", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(vendor)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (var kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(adaptedVendor, adaptedVendor.Specification.GetProperty("PreferredVendorStatus"))].Errors.Count(), 0);
            AssertIsEditViewOf<Vendor>(result);
        }


        public void EditInlineSaveValidationFail(Shift shift) {
            var controller = new GenericController();
            new ContextMocks(controller);

            INakedObject adaptedShift = NakedObjectsContext.ObjectPersistor.CreateAdapter(shift, null, null);
            INakedObject adaptedTimePeriod = NakedObjectsContext.ObjectPersistor.CreateAdapter(shift.Times, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.ToString(), "invalid", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(shift)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (var kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetInlineFieldInputId(adaptedShift.Specification.GetProperty("Times"), adaptedTimePeriod, adaptedTimePeriod.Specification.GetProperty("EndTime"))].Errors.Count(), 0);
            AssertIsEditViewOf<Shift>(result);
        }


        public void EditSaveValidationFailEmptyForm(Individual individual) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject nakedObject = NakedObjectsContext.ObjectPersistor.CreateAdapter(individual, null, null);

            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(individual)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Specification.GetProperty("Customer"))].Errors.Count(), 0);
            Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Specification.GetProperty("Contact"))].Errors.Count(), 0);
            //Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Specification.GetProperty("Customer"))].Errors[0].ErrorMessage, "Mandatory");
            Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Specification.GetProperty("Contact"))].Errors[0].ErrorMessage, "Mandatory");

            AssertIsEditViewOf<Individual>(result);
        }


        public void EditFindForObjectMultiCached(Store store) {
            var controller = new GenericController();
            var mocks = new ContextMocks(controller);

            SalesPerson salesPerson = NakedObjectsContext.ObjectPersistor.Instances<SalesPerson>().OrderBy(sp => "").First();
            mocks.HttpContext.Object.Session.AddToCache(salesPerson);
            salesPerson = NakedObjectsContext.ObjectPersistor.Instances<SalesPerson>().OrderBy(sp => "").Skip(1).First();
            mocks.HttpContext.Object.Session.AddToCache(salesPerson);

            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, store, null, null, null, "SalesPerson");
        }

        public void EditFindForObjectOneCached(Store store) {
            var controller = new GenericController();
            var mocks = new ContextMocks(controller);

            SalesPerson salesPerson = NakedObjectsContext.ObjectPersistor.Instances<SalesPerson>().First();
            mocks.HttpContext.Object.Session.AddToCache(salesPerson);

            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditFindForObject(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, store, null, null, null, "SalesPerson");
        }

        public void EditSelectForObject(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            SalesPerson salesPerson = NakedObjectsContext.ObjectPersistor.Instances<SalesPerson>().First();
            string data = "SalesPerson=" + FrameworkHelper.GetObjectId(salesPerson);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), Selector = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditActionAsFindNoParmsForObject(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction rndSpAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "RandomSalesPerson");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=RandomSalesPerson" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            // AssertNameAndParms(result, "ObjectEdit", 1, store, null, salesRepo.Object, rndSpAction, "SalesPerson");

            AssertIsEditViewOf<Store>(result);
        }


        public void EditActionAsFindParmsForObject(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction spAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }


        public void InvokeEditActionAsFindParmsForObject(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction spAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }

        public void InvokeEditActionAsFindParmsForObjectWithParms(Store store) {
            var controller = new GenericController();
            new ContextMocks(controller);

            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction spAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForFindSalesPersonByName(salesRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            AssertIsEditViewOf<Store>(result);
        }

        public void FindForActionUpdatesViewState(bool testValue) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(OrderContrib, "CreateNewOrder");
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
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(OrderContrib, "CreateNewOrder");
            Store customer = NakedObjectsContext.ObjectPersistor.Instances<Store>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "cust=" + FrameworkHelper.GetObjectId(customer);
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, Selector = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, OrderContrib.Object, action, null, null, null);
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

        public void ActionAsFindParmsForActionUpdatesViewState(bool testValue) {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(OrderContrib, "CreateNewOrder");

            INakedObjectAction findByName = CustomerRepo.GetActionLeafNode("FindStoreByName");
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
            public void CreatedCardHasBeenSaved(CreditCard card) {
                // do nothing
            }
        }

        [Test]
        public void ActionAsFindNoParmsForActionReturnMulti() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            INakedObjectAction randomContact = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "RandomContacts");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=RandomContacts" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, EmployeeRepo.Object, action, contactRepo.Object, randomContact, "contactDetails");
        }

        [Test]
        public void ActionAsFindNoParmsForActionReturnOne() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            INakedObjectAction randomContact = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "RandomContact");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=RandomContact" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, EmployeeRepo.Object, action, null, null, null);
        }

        [Test]
        public void ActionAsFindParmsForAction() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            INakedObjectAction findByName = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test]
        public void ActionAsFindParmsForActionUpdatesViewState() {
            ActionAsFindParmsForActionUpdatesViewState(true);
            ActionAsFindParmsForActionUpdatesViewState(false);
        }

        [Test]
        public void ActionAsFindParmsForActionWithDefaults() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(OrderContributedActions, "FindRate");
            INakedObject orderContribAction = FrameworkHelper.GetAdaptedService("OrderContributedActions");
            INakedObjectAction findByName = orderContribAction.Specification.GetObjectActions().Single(a => a.Id == "FindRate");
            string data = "contextObjectId=" + OrderContributedActionsId +
                          "&spec=AdventureWorksModel.CurrencyRate" +
                          "&propertyName=CurrencyRate" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindRate" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(orderContribAction);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = OrderContributedActionsId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, orderContribAction.Object, action, orderContribAction.Object, findByName, "CurrencyRate");

            Assert.AreEqual(result.ViewData["OrderContributedActions-FindRate-Currency-Input"], "US Dollar");
            Assert.AreEqual(result.ViewData["OrderContributedActions-FindRate-Currency1-Input"], "Euro");
        }

        [Test]
        public void ActionFailCrossValidation() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Contact contact = Contact;

            INakedObject adaptedContact = FrameworkHelper.GetNakedObject(contact);
            INakedObjectAction action = GetAction(adaptedContact, "ChangePassword");

            FormCollection form = GetFormForChangePassword(adaptedContact, "p1", "p2", "p3");
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(contact), ActionId = action.Id};

            var result = (ViewResult) controller.Action(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.Greater(result.ViewData.ModelState[""].Errors.Count(), 0);
            AssertIsDialogViewOfAction(result, "Change Password");
        }

        [Test]
        public void ActionGet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            SalesOrderHeader order = Order;
            INakedObjectAction action = GetAction(FrameworkHelper.GetNakedObject(order), "Recalculate");
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = FrameworkHelper.GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel);
            AssertNameAndParms(result, "ActionDialog", null, order, action, null, null, null);
        }

        [Test]
        public void ActionOnNotPersistedObject() {
            var controller = new GenericController();
            new ContextMocks(controller);
            NotPersistedObject obj = NotPersistedObject;

            string objectId = FrameworkHelper.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleAction"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void ActionOnNotPersistedObjectWithReturn() {
            var controller = new GenericController();
            new ContextMocks(controller);
            NotPersistedObject obj = NotPersistedObject;

            string objectId = FrameworkHelper.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleActionWithReturn"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
            Assert.AreEqual("aName", ((NotPersistedObject) result.ViewData.Model).Name);
        }

        [Test]
        public void CrossFieldValidationFail() {
            var controller = new GenericController();
            new ContextMocks(controller);
            IDictionary<string, string> idToRawvalue;
            INakedObjectSpecification ccSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (CreditCard));
            INakedObject cc = NakedObjectsContext.ObjectPersistor.CreateInstance(ccSpec);

            FormCollection form = GetFormForCeditCardEdit(cc, "Vista", "12345", "1", "2010", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(cc)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.Greater(result.ViewData.ModelState[""].Errors.Count(), 0);
            AssertIsEditViewOf<CreditCard>(result);
        }

        [Test]
        public void CrossFieldValidationSuccess() {
            var controller = new GenericController();
            new ContextMocks(controller);
            IDictionary<string, string> idToRawvalue;
            INakedObjectSpecification ccSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (CreditCard));
            INakedObject cc = NakedObjectsContext.ObjectPersistor.CreateInstance(ccSpec);
            cc.GetDomainObject<CreditCard>().Creator = new TestCreator();

            FormCollection form = GetFormForCeditCardEdit(cc, "Vista", "12345", "1", "2020", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(cc)};

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
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {Id = EmployeeId};
            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectGet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {Id = EmployeeId};
            var result = (ViewResult) controller.EditObject(objectModel);
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectGetWithInline() {
            var controller = new GenericController();
            new ContextMocks(controller);

            Shift shift = Employee.DepartmentHistory.First().Shift;

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(shift)};
            var result = (ViewResult) controller.EditObject(objectModel);
            AssertIsEditViewOf<Shift>(result);
        }

        [Test]
        public void EditObjectKeepTableFormat() {
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list&DepartmentHistory=table"}});
            var objectModel = new ObjectAndControlData {Id = EmployeeId};

            var result = (ViewResult) controller.EditObject(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }

        [Test]
        public void EditObjectNotPersisted() {
            var controller = new GenericController();
            new ContextMocks(controller);
            NotPersistedObject obj = NotPersistedObject;

            string objectId = FrameworkHelper.GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId};

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void EditObjectPage() {
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {
                Id = FrameworkHelper.GetObjectId(TransientEmployee),
                Redisplay = "editMode=true"
            };

            var form = new FormCollection {};

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectWithInline() {
            var controller = new GenericController();
            new ContextMocks(controller);

            Shift shift = Employee.DepartmentHistory.First().Shift;

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(shift)};
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
            var controller = new GenericController();
            new ContextMocks(controller);

            const string redisplay = "DepartmentHistory=table&editMode=True";
            Employee employee = TransientEmployee;
            Employee report1 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().OrderBy(e => e.EmployeeID).Skip(1).First();
            Employee report2 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().OrderBy(e => e.EmployeeID).Skip(2).First();
            INakedObject employeeNakedObject = FrameworkHelper.GetNakedObject(employee);
            INakedObjectAssociation collectionAssoc = employeeNakedObject.Specification.Properties.Single(p => p.Id == "DirectReports");

            var form = new FormCollection {
                {IdHelper.DisplayFormatFieldId, "Addresses=list"},
                {IdHelper.GetCollectionItemId(employeeNakedObject, collectionAssoc), FrameworkHelper.GetObjectId(report1)},
                {IdHelper.GetCollectionItemId(employeeNakedObject, collectionAssoc), FrameworkHelper.GetObjectId(report2)}
            };

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
            Assert.AreEqual(2, ((Employee) result.ViewData.Model).DirectReports.Count);
        }

        [Test]
        public void EditSaveConcurrencyFail() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            INakedObject adaptedStore = NakedObjectsContext.ObjectPersistor.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;
            string differentDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), differentDateTime, out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store)};

            NakedObjectsContext.ObjectPersistor.StartTransaction();
            try {
                controller.Edit(objectModel, form);

                Assert.Fail("Expect concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(adaptedStore, expected.SourceNakedObject);
            }
            finally {
                NakedObjectsContext.ObjectPersistor.EndTransaction();
            }
        }

        [Test]
        public void EditSaveConcurrencyOk() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            INakedObject adaptedStore = NakedObjectsContext.ObjectPersistor.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.InvariantCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store)};

            NakedObjectsContext.ObjectPersistor.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (var kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsDetailsViewOf<Store>(result);
            }
            finally {
                NakedObjectsContext.ObjectPersistor.EndTransaction();
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

        [Test]
        public void FindForAction() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
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

        [Test]
        public void FindForActionUpdatesViewState() {
            FindForActionUpdatesViewState(true);
            FindForActionUpdatesViewState(false);
        }

        [Test]
        public void GetFile() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Product product = Product;

            FileContentResult result = controller.GetFile(FrameworkHelper.GetObjectId(Product), "Photo");

            Stream stream = product.Photo.GetResourceAsStream();
            byte[] bytes;
            using (var br = new BinaryReader(stream)) {
                bytes = br.ReadBytes((int) stream.Length);
            }

            Assert.AreEqual(bytes, result.FileContents);
        }

        [Test]
        public void InitialInvokeContributedActionOnCollectionTarget() {
            var controller = new GenericController();

            new ContextMocks(controller);
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
        public void AAInitialInvokeContributedActionOnEmptyCollectionTarget() {
            var controller = new GenericController();

            new ContextMocks(controller);
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
            var warnings = NakedObjectsContext.MessageBroker.Warnings.ToArray();
            Assert.AreEqual("No objects selected", warnings.First());
        }

        [Test]
        public void InitialInvokeCovariantContributedActionOnCollectionTarget() {
            var controller = new GenericController();

            new ContextMocks(controller);
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


        [Test]
        public void InvokeActionAsFindParmsForAction() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            INakedObjectAction findByName = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test]
        public void InvokeActionAsFindParmsForActionWithParms() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            INakedObjectAction findByName = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetFormForFindContactByName(contactRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 11, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test]
        public void InvokeActionAsSaveForActionFailValidation() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            Store transientStore = TransientStore;
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            INakedObjectAction findByName = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + FrameworkHelper.GetObjectId(transientStore) +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", FrameworkHelper.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeActionAsSaveForActionPassValidation() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            INakedObject contactRepo = FrameworkHelper.GetAdaptedService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            INakedObjectAction findByName = contactRepo.Specification.GetObjectActions().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + FrameworkHelper.GetActionId(action) +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + FrameworkHelper.GetObjectId(transientVendor) +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(contactRepo);
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsContext.ObjectPersistor.CreateAdapter(transientVendor, null, null);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeActionWithMultiSelectObjects() {
            var controller = new GenericController();

            string id = FrameworkHelper.GetObjectId(Order);

            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasons",
                Id = id
            };

            var form = new FormCollection();

            form.Add("SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;1;False;;0");
            form.Add("SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;2;False;;0");

            INakedObject order = NakedObjectsContext.ObjectPersistor.CreateAdapter(Order, null, null);
            INakedObjectAssociation assocMD = order.Specification.GetProperty("ModifiedDate");
            INakedObjectAction action = order.GetActionLeafNode("AddNewSalesReasons");


            string idMD = IdHelper.GetConcurrencyActionInputId(order, action, assocMD);

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.InvariantCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeActionWithMultiSelectParseable() {
            var controller = new GenericController();

            string id = FrameworkHelper.GetObjectId(Order);

            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasonsByCategories",
                Id = id
            };

            var form = new FormCollection();

            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"1");
            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"2");

            INakedObject order = NakedObjectsContext.ObjectPersistor.CreateAdapter(Order, null, null);
            INakedObjectAssociation assocMD = order.Specification.GetProperty("ModifiedDate");
            INakedObjectAction action = order.GetActionLeafNode("AddNewSalesReasonsByCategories");


            string idMD = IdHelper.GetConcurrencyActionInputId(order, action, assocMD);

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.InvariantCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeContributedActionOnCollectionTarget() {
            var controller = new GenericController();

            new ContextMocks(controller);
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
            var controller = new GenericController();

            new ContextMocks(controller);
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
            var controller = new GenericController();

            new ContextMocks(controller);
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
            var controller = new GenericController();

            new ContextMocks(controller);
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "LastOrder",
                Id = FrameworkHelper.GetObjectId(store),
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
            var controller = new GenericController();

            new ContextMocks(controller);
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "LastOrder",
                Id = FrameworkHelper.GetObjectId(store),
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
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "CreateNewOrder",
                Id = FrameworkHelper.GetObjectId(store),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(
                new Dictionary<string, string> {
                    {"Store-CreateNewOrder-ModifiedDate-Concurrency", store.ModifiedDate.ToString(CultureInfo.InvariantCulture)}
                }));

            AssertIsDialogViewOfAction(result, "Create New Order");
            Assert.IsTrue(result.ViewData.ContainsKey("OrderContributedActions-CreateNewOrder-Customer-Select"));
            Assert.AreEqual(FrameworkHelper.GetNakedObject(store), result.ViewData["OrderContributedActions-CreateNewOrder-Customer-Select"]);
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
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            Store transientStore = TransientStore;
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction spAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&subEditObjectId=" + FrameworkHelper.GetObjectId(transientStore) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", FrameworkHelper.GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeEditActionAsSaveForObjectPassValidation() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            INakedObject adaptedStore = FrameworkHelper.GetNakedObject(store);
            IDictionary<string, string> idToRawvalue;
            INakedObject salesRepo = FrameworkHelper.GetAdaptedService("SalesRepository");
            INakedObjectAction spAction = salesRepo.Specification.GetObjectActions().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + FrameworkHelper.GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + FrameworkHelper.GetObjectId(salesRepo) +
                          "&subEditObjectId=" + FrameworkHelper.GetObjectId(transientVendor) +
                          "&contextActionId=";

            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            INakedObject adaptedVendor = NakedObjectsContext.ObjectPersistor.CreateAdapter(transientVendor, null, null);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleAction", Id = FrameworkHelper.GetObjectId(obj)};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSetWithReturn() {
            var controller = new GenericController();
            new ContextMocks(controller);
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleActionWithReturn", Id = FrameworkHelper.GetObjectId(obj)};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<NotPersistedObject>(result);
            Assert.AreEqual("aName", ((NotPersistedObject) result.ViewData.Model).Name);
        }

      

        [Test]
        public void InvokeObjectActionDefaultSet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {ActionId = "CreateNewOrder", Id = OrderContribId, InvokeAction = "action=action"};

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string>()));

            AssertIsDialogViewOfAction(result, "Create New Order");

            Assert.IsTrue(result.ViewData.ContainsKey("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            Assert.AreEqual(true, result.ViewData["OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"]);
        }

        [Test]
        public void InvokeObjectActionNoParms() {
            var controller = new GenericController();
            new ContextMocks(controller);
            SalesOrderHeader order = Order;
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = FrameworkHelper.GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string> {{"SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", order.ModifiedDate.ToString(CultureInfo.InvariantCulture)}}));

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeObjectActionParmsNotSet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedProduct = NakedObjectsContext.ObjectPersistor.CreateAdapter(Product, null, null);
            FormCollection form = GetFormForBestSpecialOffer(adaptedProduct, "");
            var objectModel = new ObjectAndControlData {ActionId = "BestSpecialOffer", Id = ProductId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDialogViewOfAction(result, "Best Special Offer");

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Product-BestSpecialOffer-Quantity-Input"));
            Assert.AreEqual("Mandatory", result.ViewData.ModelState["Product-BestSpecialOffer-Quantity-Input"].Errors[0].ErrorMessage);
        }

        [Test]
        public void InvokeObjectActionParmsSet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObject adaptedProduct = NakedObjectsContext.ObjectPersistor.CreateAdapter(Product, null, null);
            FormCollection form = GetFormForBestSpecialOffer(adaptedProduct, "1");
            var objectModel = new ObjectAndControlData {ActionId = "BestSpecialOffer", Id = ProductId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SpecialOffer>(result);
        }

        [Test]
        public void InvokeObjectActionReturnCollectionOfOneItem() {
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetFormForListProductsBySubCategory(ProductRepo, GetBoundedId<ProductSubcategory>("Hydration Packs"));
            var objectModel = new ObjectAndControlData {ActionId = "ListProductsBySubCategory", Id = ProductRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<Product>(result);
        }

        [Test]
        public void InvokeObjectActionReturnOrderedPagedCollectionAsc() {
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetFormForFindEmployeeByName(EmployeeRepo, "", "Smith");
            var objectModel = new ObjectAndControlData {ActionId = "FindEmployeeByName", Id = EmployeeRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<Employee>(result);
        }

        [Test]
        public void InvokeServiceActionParmsSet() {
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetFormForFindEmployeeByName(EmployeeRepo, "S", "Smith");
            var objectModel = new ObjectAndControlData {ActionId = "FindEmployeeByName", Id = EmployeeRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<Employee>(result);
        }

        [Test]
        public void SelectForAction() {
            var controller = new GenericController();
            new ContextMocks(controller);
            INakedObjectAction action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            Contact contactDetails = NakedObjectsContext.ObjectPersistor.Instances<Contact>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "ContactDetails=" + FrameworkHelper.GetObjectId(contactDetails);
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
        public void ViewCollectionDisplay() {
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {Id = EmployeeId};

            var result = (ViewResult) controller.Details(objectModel);

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsCancel() {
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = EmployeeId, Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsCancelTransient() {
            var controller = new GenericController();
            new ContextMocks(controller);
            FormCollection form = GetForm(new Dictionary<string, string> {{IdHelper.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(TransientEmployee), Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);
            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsRedisplay() {
            var controller = new GenericController();
            new ContextMocks(controller);
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
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {
                Id = FrameworkHelper.GetObjectId(TransientEmployee),
                Redisplay = "editMode=false"
            };

            var form = new FormCollection {};

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<Employee>(result);
        }

        [Test]
        public void ViewServiceDetails() {
            var controller = new GenericController();
            new ContextMocks(controller);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId};

            var result = (ViewResult) controller.Details(objectModel);

            AssertIsSetAfterTransactionViewOf<EmployeeRepository>(result);
        }
    }

    [TestFixture]
    public class ConcurrencyTest : AcceptanceTestCase {

        [SetUp]
        public void SetupTest() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
            InitializeNakedObjectsFramework();
        }

        [TearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
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

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {new OrderContributedActions()}); }
        }


        protected override IObjectPersistorInstaller Persistor {
            get {
                var installer = new EntityPersistorInstaller();
                installer.ForceContextSet();
                return installer;
            }
        }

        private static SalesOrderHeader Order {
            get { return NakedObjectsContext.ObjectPersistor.Instances<SalesOrderHeader>().First(); }
        }

        private static FormCollection GetForm(IDictionary<string, string> nameValues) {
            var form = new FormCollection();
            nameValues.ForEach(kvp => form.Add(kvp.Key, kvp.Value));
            return form;
        }


        [Test]
        public void InvokeObjectActionConcurrencyFail() {
            var controller = new GenericController();
            new ContextMocks(controller);
            SalesOrderHeader order = Order;
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = FrameworkHelper.GetObjectId(order),
                InvokeAction = "action=action"
            };

            try {
                controller.Action(objectModel, GetForm(new Dictionary<string, string> { { "SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.InvariantCulture) } }));
                Assert.Fail("Expected concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(order, expected.SourceNakedObject.Object);
            }
        }

        [Test]
        // in seperate test fixture because otherwise it fails on second attempt - MvcTestApp.Tests.Controllers.GenericControllerTest.EditSaveEFConcurrencyFail:
        // System.Data.EntityCommandExecutionException : An error occurred while executing the command definition. See the inner exception for details.
        //  ----> System.Data.SqlClient.SqlException : A transport-level error has occurred when sending the request to the server. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.)
        public void EditSaveEFConcurrencyFail() {
            var controller = new GenericController();
            new ContextMocks(controller);
            Store store = GenericControllerTest.Store;
            INakedObject adaptedStore = NakedObjectsContext.ObjectPersistor.CreateAdapter(store, null, null);
            IDictionary<string, string> idToRawvalue;

            FormCollection form = GenericControllerTest.GetFormForStoreEdit(adaptedStore, store.Name, FrameworkHelper.GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.InvariantCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = FrameworkHelper.GetObjectId(store)};

            NakedObjectsContext.ObjectPersistor.StartTransaction();
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

                NakedObjectsContext.ObjectPersistor.EndTransaction();

                Assert.Fail("Expect concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(store, expected.SourceNakedObject.Object);
            }
            finally {
                conn.Close();
            }
        }
    }
}