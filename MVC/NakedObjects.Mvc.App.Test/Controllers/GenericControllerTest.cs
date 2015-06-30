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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Component;
using NakedObjects.Facade;
using NakedObjects.Facade.Translation;
using NakedObjects.Facade.Utility;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;
using NUnit.Framework;
using NakedObjects.Core.Util;
using NakedObjects.Facade.Impl;
using NakedObjects.Web.Mvc.Helpers;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class GenericControllerTest : AcceptanceTestCase {
        private GenericController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get { return null; }
        }

        protected override Type[] Types {
            get {
                List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes().Where(t => t.IsPublic && t.Namespace == "AdventureWorksModel").ToList();

                allTypes.AddRange(new[] {
                    typeof (NotPersistedObject),
                    typeof (CustomerDashboard),
                    typeof (Customer[]),
                    typeof (Employee[]),
                    typeof (Store[]),
                    typeof (Individual[]),
                    typeof (SalesOrderHeader.SalesReasonCategories),
                    typeof (ActionResultModelQ<SalesOrderHeader>),
                    typeof (ActionResultModel<Store>),
                    typeof (ActionResultModelQ<Store>),
                    typeof (ActionResultModelQ<Employee>)
                });

                return allTypes.ToArray();
            }
        }

        private IObjectFacade GetService(string name) {
            return Surface.GetServices().List.SingleOrDefault(s => s.Specification.ShortName == name);
        }


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

        private Employee Employee {
            get { return NakedObjectsFramework.Persistor.Instances<Employee>().First(); }
        }

        private string EmployeeId {
            get { return GetObjectId(Employee); }
        }

        private Employee TransientEmployee {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Employee))).GetDomainObject<Employee>(); }
        }

        private Vendor TransientVendor {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Vendor))).GetDomainObject<Vendor>(); }
        }

        private Shift TransientShift {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Shift))).GetDomainObject<Shift>(); }
        }

        private Individual TransientIndividual {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Individual))).GetDomainObject<Individual>(); }
        }

        private CreditCard TransientCreditCard {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (CreditCard))).GetDomainObject<CreditCard>(); }
        }

        private NotPersistedObject NotPersistedObject {
            get {
                var repo = GetService("SimpleRepository-NotPersistedObject").Object as SimpleRepository<NotPersistedObject>;
                return repo.NewInstance();
            }
        }

        private SalesOrderHeader Order {
            get { return NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().First(); }
        }

        private string OrderId {
            get { return GetObjectId(Order); }
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
            get { return GetObjectId(Product); }
        }

        private IObjectFacade EmployeeRepo {
            get { return GetService("EmployeeRepository"); }
        }

        private IObjectFacade OrderContributedActions {
            get { return GetService("OrderContributedActions"); }
        }

        private string OrderContributedActionsId {
            get { return GetObjectId(OrderContributedActions); }
        }

        private IObjectFacade ProductRepo {
            get { return GetService("ProductRepository"); }
        }

        private string EmployeeRepoId {
            get { return GetObjectId(EmployeeRepo); }
        }

        private string ProductRepoId {
            get { return GetObjectId(ProductRepo); }
        }

        private IObjectFacade OrderRepo {
            get { return GetService("OrderRepository"); }
        }

        private string OrderRepoId {
            get { return GetObjectId(OrderRepo); }
        }

        private IObjectFacade OrderContrib {
            get { return GetService("OrderContributedActions"); }
        }

        private string OrderContribId {
            get { return GetObjectId(OrderContrib); }
        }

        private IObjectFacade CustomerRepo {
            get { return GetService("CustomerRepository"); }
        }

        private string CustomerRepoId {
            get { return GetObjectId(CustomerRepo); }
        }

        public Store Store {
            get { return NakedObjectsFramework.Persistor.Instances<Store>().First(); }
        }

        private string StoreId {
            get { return GetObjectId(Store); }
        }

        private SalesPerson SalesPerson {
            get { return NakedObjectsFramework.Persistor.Instances<SalesPerson>().First(); }
        }

        private Store TransientStore {
            get { return NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Store))).GetDomainObject<Store>(); }
        }

        private IIdHelper IdHelper {
            get { return new IdHelper(); }
        }

        private string GetObjectId(object obj) {
            return Surface.OidTranslator.GetOidTranslation(obj as IObjectFacade ?? Surface.GetObject(obj)).Encode();
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IOidTranslator, OidTranslatorSemiColonSeparatedList>(new PerResolveLifetimeManager());

        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            // DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        private static void AssertPagingData(ViewResult result, int currentPage, int pageSize, int pageTotal) {
            Assert.AreEqual(currentPage, ((Dictionary<string, int>) result.ViewData[IdConstants.PagingData])[IdConstants.PagingCurrentPage]);
            Assert.AreEqual(pageSize, ((Dictionary<string, int>) result.ViewData[IdConstants.PagingData])[IdConstants.PagingPageSize]);
            Assert.AreEqual(pageTotal, ((Dictionary<string, int>) result.ViewData[IdConstants.PagingData])[IdConstants.PagingTotal]);
        }

        private static void AssertIsCollectionViewOf<T>(ViewResult result) {
            Assert.AreEqual("StandaloneTable", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<IEnumerable<T>>(data.Model);
            Assert.IsNotInstanceOf<IQueryable<T>>(data.Model);
            Assert.IsTrue(((IEnumerable<T>) data.Model).Any());
        }

        private static void AssertIsQueryableViewOf<T>(ViewResult result) {
            Assert.AreEqual("StandaloneTable", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<IQueryable<T>>(data.Model);
            Assert.IsTrue(((IQueryable<T>) data.Model).Any());
        }

        private static void AssertIsDetailsViewOf<T>(ViewResult result) {
            Assert.AreEqual("ObjectView", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<T>(data.Model);
        }

        private static void AssertIsSetAfterTransactionViewOf<T>(ViewResult result) {
            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<T>(data.Model);
        }

        private static void AssertIsEditViewOf<T>(ViewResult result) {
            Assert.AreEqual("ObjectEdit", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<T>(data.Model);
        }

        private void AssertIsDialogViewOfAction(ViewResult result, string actionName) {
            Assert.AreEqual("ActionDialog", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<FindViewModel>(data.Model);
            Assert.AreEqual(actionName, (((FindViewModel) data.Model).ContextAction).Name);
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
            return GetObjectId(GetBoundedInstance<T>(title).GetDomainObject());
        }

        private FormCollection GetFormForFindEmployeeByName(IObjectFacade employeeRepo, string firstName, string secondName) {
            IActionFacade actionFindEmployeeByname = employeeRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindEmployeeByName");

            var parmFirstName = actionFindEmployeeByname.Parameters[0];
            var parmSecondName = actionFindEmployeeByname.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId((actionFindEmployeeByname), (parmFirstName));
            string idSecondName = IdHelper.GetParameterInputId((actionFindEmployeeByname), (parmSecondName));

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private FormCollection GetFormForFindSalesPersonByName(IObjectFacade salesRepo, string firstName, string secondName) {
            var action = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindSalesPersonByName");

            var parmFirstName = action.Parameters[0];
            var parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId((action), (parmFirstName));
            string idSecondName = IdHelper.GetParameterInputId((action), (parmSecondName));

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private FormCollection GetFormForFindContactByName(IObjectFacade contactRepo, string firstName, string secondName) {
            IActionFacade action = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");

            var parmFirstName = action.Parameters[0];
            var parmSecondName = action.Parameters[1];

            string idFirstName = IdHelper.GetParameterInputId((action), (parmFirstName));
            string idSecondName = IdHelper.GetParameterInputId((action), (parmSecondName));

            return GetForm(new Dictionary<string, string> {
                {idFirstName, firstName},
                {idSecondName, secondName}
            });
        }

        private FormCollection GetFormForBestSpecialOffer(IObjectFacade productRepo, string quantity) {
            IActionFacade action = productRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "BestSpecialOffer");
            var parmQuantity = action.Parameters[0];
            string idQuantity = IdHelper.GetParameterInputId((action), (parmQuantity));
            return GetForm(new Dictionary<string, string> {
                {idQuantity, quantity}
            });
        }

        private FormCollection GetFormForChangePassword(IObjectFacade contact, string p1, string p2, string p3) {
            IActionFacade action = contact.Specification.GetActionLeafNodes().Single(a => a.Id == "ChangePassword");
            var pp1 = action.Parameters[0];
            var pp2 = action.Parameters[1];
            var pp3 = action.Parameters[2];

            string idP1 = IdHelper.GetParameterInputId((action), (pp1));
            string idP2 = IdHelper.GetParameterInputId((action), (pp2));
            string idP3 = IdHelper.GetParameterInputId((action), (pp3));

            return GetForm(new Dictionary<string, string> {
                {idP1, p1},
                {idP2, p2},
                {idP3, p3}
            });
        }

        private FormCollection GetFormForListProductsBySubCategory(IObjectFacade productRepo, string pscId) {
            IActionFacade action = productRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "ListProductsBySubCategory");
            var parmPsc = action.Parameters[0];
            string idPsc = IdHelper.GetParameterInputId((action), (parmPsc));

            return GetForm(new Dictionary<string, string> {
                {idPsc, pscId}
            });
        }

        private FormCollection GetFormForShiftEdit(IObjectFacade shift,
                                                   IObjectFacade timePeriod,
                                                   string t1,
                                                   string t2,
                                                   out IDictionary<string, string> idToRawValue) {
            var shiftSpec = Surface.GetDomainType(typeof (Shift).FullName);
            var timePeriodSpec = Surface.GetDomainType(typeof (TimePeriod).FullName);

            var assocN = shiftSpec.Properties.Single(p => p.Id == "Name");
            IAssociationFacade assocTp = shiftSpec.Properties.Single(p => p.Id == "Times");

            IAssociationFacade assocT1 = timePeriodSpec.Properties.Single(p => p.Id == "StartTime");
            IAssociationFacade assocT2 = timePeriodSpec.Properties.Single(p => p.Id == "EndTime");

            string idN = IdHelper.GetFieldInputId((shift), (assocN));
            string idT1 = IdHelper.GetInlineFieldInputId((assocTp), (timePeriod), (assocT1));
            string idT2 = IdHelper.GetInlineFieldInputId((assocTp), (timePeriod), (assocT2));

            idToRawValue = new Dictionary<string, string> {
                {idN, Guid.NewGuid().ToString()},
                {idT1, t1},
                {idT2, t2}
            };

            return GetForm(idToRawValue);
        }

        private FormCollection GetFormForVendorEdit(IObjectFacade vendor,
                                                    string accountNumber,
                                                    string name,
                                                    string creditRating,
                                                    string preferredVendorStatus,
                                                    string activeFlag,
                                                    string purchasingWebServiceURL,
                                                    out IDictionary<string, string> idToRawValue) {
            var nakedObjectSpecification = Surface.GetDomainType(typeof (Vendor).FullName);
            var assocAN = nakedObjectSpecification.Properties.Single(p => p.Id == "AccountNumber");
            var assocN = nakedObjectSpecification.Properties.Single(p => p.Id == "Name");
            var assocCR = nakedObjectSpecification.Properties.Single(p => p.Id == "CreditRating");
            var assocPVS = nakedObjectSpecification.Properties.Single(p => p.Id == "PreferredVendorStatus");
            var assocAF = nakedObjectSpecification.Properties.Single(p => p.Id == "ActiveFlag");
            var assocPWSURL = nakedObjectSpecification.Properties.Single(p => p.Id == "PurchasingWebServiceURL");

            string idAN = IdHelper.GetFieldInputId((vendor), (assocAN));
            string idN = IdHelper.GetFieldInputId((vendor), (assocN));
            string idCR = IdHelper.GetFieldInputId((vendor), (assocCR));
            string idPVS = IdHelper.GetFieldInputId((vendor), (assocPVS));
            string idAF = IdHelper.GetFieldInputId((vendor), (assocAF));
            string idPWSURL = IdHelper.GetFieldInputId((vendor), (assocPWSURL));

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

        public FormCollection GetFormForStoreEdit(IObjectFacade store,
                                                  string storeName,
                                                  string salesPerson,
                                                  string modifiedDate,
                                                  out IDictionary<string, string> idToRawValue) {
            var nakedObjectSpecification = Surface.GetDomainType(typeof (Store).FullName);
            var assocSN = nakedObjectSpecification.Properties.SingleOrDefault(p => p.Id == "Name");
            var assocSP = nakedObjectSpecification.Properties.SingleOrDefault(p => p.Id == "SalesPerson");
            var assocMD = nakedObjectSpecification.Properties.SingleOrDefault(p => p.Id == "ModifiedDate");

            string idSN = IdHelper.GetFieldInputId((store), (assocSN));
            string idSP = IdHelper.GetFieldInputId((store), (assocSP));
            string idMD = IdHelper.GetConcurrencyFieldInputId((store), (assocMD));

            idToRawValue = new Dictionary<string, string> {
                {idSN, storeName},
                {idSP, salesPerson},
                {idMD, modifiedDate}
            };

            return GetForm(idToRawValue);
        }

        private FormCollection GetFormForCeditCardEdit(IObjectFacade creditCard,
                                                       string cardType,
                                                       string cardNumber,
                                                       string expiryMonth,
                                                       string expiryYear,
                                                       out IDictionary<string, string> idToRawValue) {
            var nakedObjectSpecification = Surface.GetDomainType((typeof (CreditCard).FullName));
            var assocCT = nakedObjectSpecification.Properties.Single(p => p.Id == "CardType");
            var assocCN = nakedObjectSpecification.Properties.Single(p => p.Id == "CardNumber");
            var assocEM = nakedObjectSpecification.Properties.Single(p => p.Id == "ExpMonth");
            var assocEY = nakedObjectSpecification.Properties.Single(p => p.Id == "ExpYear");

            string idCT = IdHelper.GetFieldInputId((creditCard), (assocCT));
            string idCN = IdHelper.GetFieldInputId((creditCard), (assocCN));
            string idEM = IdHelper.GetFieldInputId((creditCard), (assocEM));
            string idEY = IdHelper.GetFieldInputId((creditCard), (assocEY));

            idToRawValue = new Dictionary<string, string> {
                {idCT, cardType},
                {idCN, cardNumber},
                {idEM, expiryMonth},
                {idEY, expiryYear}
            };

            return GetForm(idToRawValue);
        }

        private FormCollection GetFormForCreateNewEmployeeFromContact(IActionFacade action, string contact, out IDictionary<string, string> idToRawValue) {
            var parmContact = action.Parameters[0];
            string idContact = IdHelper.GetParameterInputId((action), (parmContact));
            idToRawValue = new Dictionary<string, string> {
                {idContact, contact}
            };
            return GetForm(idToRawValue);
        }

        private FormCollection GetFormForCreateNewOrder(IActionFacade action, string cust, bool copy, out IDictionary<string, string> idToRawValue) {
            var parmCust = action.Parameters[0];
            var parmCopy = action.Parameters[1];

            string idCust = IdHelper.GetParameterInputId((action), (parmCust));
            string idCopy = IdHelper.GetParameterInputId((action), (parmCopy));

            idToRawValue = new Dictionary<string, string> {
                {idCust, cust},
                {idCopy, copy.ToString()}
            };
            return GetForm(idToRawValue);
        }

        private static IActionFacade GetAction(IObjectFacade owner, string id) {
            return owner.Specification.GetActionLeafNodes().Single(a => a.Id == id);
        }

        //private IObjectFacade GetService(string serviceName) {
        //    return facade.GetServices().List.SingleOrDefault(s => s.Specification.FullName.Split('.').Last() == serviceName);
        //}

        private static void AssertNameAndParms(ViewResult result, string name, int? count, object contextObject, IActionFacade contextAction, object targetObject, IActionFacade targetAction, string pName) {
            Assert.AreEqual(name, result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<FindViewModel>(data.Model);
            var fvm = data.Model as FindViewModel;

            if (count == null) {
                Assert.IsNull(fvm.ActionResult);
            }
            else {
                Assert.IsNotNull(fvm.ActionResult);
                Assert.AreEqual(count.Value, fvm.ActionResult.Cast<object>().Count());
            }

            Assert.AreEqual(contextObject, fvm.ContextObject);
            Assert.AreEqual(contextAction, fvm.ContextAction);
            Assert.AreEqual(targetObject, fvm.TargetObject);
            Assert.AreEqual(targetAction, fvm.TargetAction);
            Assert.AreEqual(pName, fvm.PropertyName);
        }

        public void EditRedisplay(Employee employee) {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdConstants.DisplayFormatFieldId, "Addresses=list"}});
            const string redisplay = "DepartmentHistory=table&editMode=True";
            var objectModel = new ObjectAndControlData {Id = GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }

        public void EditSaveValidationOk(Vendor vendor, bool saveAndClose = false) {
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            IObjectFacade adaptedVendor = Surface.GetObject(vendor);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(vendor)};

            if (saveAndClose) {
                const string sac = "SaveAndClose=True";
                objectModel.SaveAndClose = sac;
            }

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var actionResult = controller.Edit(objectModel, form);

                if (saveAndClose) {
                    Assert.IsInstanceOf<RedirectToRouteResult>(actionResult);
                }
                else {
                    var result = (ViewResult) actionResult;
                    foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
                        Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                        Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                    }
                    AssertIsDetailsViewOf<Vendor>(result);
                }
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        public void EditApplyActionValidationOk(Vendor vendor) {
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            IObjectFacade adaptedVendor = Surface.GetObject(vendor);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(vendor)};

            const string invokeAction = "targetActionId=CreateNewContact";
            objectModel.InvokeAction = invokeAction;

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var actionResult = controller.Edit(objectModel, form);

                var result = (ViewResult) actionResult;
                foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
                    Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                    Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
                }
                AssertIsSetAfterTransactionViewOf<Contact>(result);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        public void EditInlineSaveValidationOk(Shift shift, int i) {
            IObjectFacade adaptedShift = Surface.GetObject(shift);
            IObjectFacade adaptedTimePeriod = Surface.GetObject(shift.Times);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.AddHours(i).ToString(), DateTime.Now.AddHours(i).AddHours(8).ToString(), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(shift)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
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
            IObjectFacade adaptedVendor = Surface.GetObject(vendor);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForVendorEdit(adaptedVendor, "", "", "", "", "", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(vendor)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.IsTrue(result.ViewData.ModelState[IdHelper.GetFieldInputId(adaptedVendor, adaptedVendor.Specification.Properties.Single(p => p.Id == "PreferredVendorStatus"))].Errors.Any());
            AssertIsEditViewOf<Vendor>(result);
        }

        public void EditInlineSaveValidationFail(Shift shift) {
            IObjectFacade adaptedShift = Surface.GetObject(shift);
            IObjectFacade adaptedTimePeriod = Surface.GetObject(shift.Times);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForShiftEdit(adaptedShift, adaptedTimePeriod, DateTime.Now.ToString(), "invalid", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(shift)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
                Assert.IsTrue(result.ViewData.ModelState.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, result.ViewData.ModelState[kvp.Key].Value.RawValue);
            }
            Assert.IsTrue(result.ViewData.ModelState[IdHelper.GetInlineFieldInputId(adaptedShift.Specification.Properties.Single(p => p.Id == "Times"), adaptedTimePeriod, adaptedTimePeriod.Specification.Properties.Single(p => p.Id == "EndTime"))].Errors.Any());
            AssertIsEditViewOf<Shift>(result);
        }

        public void EditSaveValidationFailEmptyForm(Individual individual) {
            IObjectFacade nakedObject = Surface.GetObject(individual);

            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = GetObjectId(individual)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //Assert.Greater(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.Properties.Single(p => p.Id =="Customer"))].Errors.Count(), 0);
            Assert.IsTrue(result.ViewData.ModelState[IdHelper.GetFieldInputId((nakedObject), (nakedObject.Specification.Properties.Single(p => p.Id == "Contact")))].Errors.Any());
            //Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId(nakedObject, nakedObject.Spec.Properties.Single(p => p.Id =="Customer"))].Errors[0].ErrorMessage, "Mandatory");
            Assert.AreEqual(result.ViewData.ModelState[IdHelper.GetFieldInputId((nakedObject), (nakedObject.Specification.Properties.Single(p => p.Id == "Contact")))].Errors[0].ErrorMessage, "Mandatory");

            AssertIsEditViewOf<Individual>(result);
        }

        public void EditFindForObjectMultiCached(Store store) {
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().OrderBy(sp => "").First();
            mocks.HttpContext.Object.Session.AddToCache(Surface, salesPerson);
            salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().OrderBy(sp => "").Skip(1).First();
            mocks.HttpContext.Object.Session.AddToCache(Surface, salesPerson);

            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + GetObjectId(adaptedStore) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = GetObjectId(adaptedStore), Finder = data};
            var adaptedSalesPerson = Surface.GetObject(store.SalesPerson);
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, GetObjectId(adaptedSalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, store, null, null, null, "SalesPerson");
        }

        public void EditFindForObjectOneCached(Store store) {
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().First();
            mocks.HttpContext.Object.Session.AddToCache(Surface, salesPerson);

            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + GetObjectId(adaptedStore) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditFindForObject(Store store) {
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&contextActionId=";
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), Finder = data};
            FormCollection form = GetFormForStoreEdit(adaptedStore, Store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, store, null, null, null, "SalesPerson");
        }

        public void EditSelectForObject(Store store) {
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().First();
            string data = "SalesPerson=" + GetObjectId(salesPerson);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), Selector = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Store>(result);
        }

        public void EditActionAsFindNoParmsForObject(Store store) {
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            var salesRepo = GetService("SalesRepository");
            var rndSpAction = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "RandomSalesPerson");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=RandomSalesPerson" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            // AssertNameAndParms(result, "ObjectEdit", 1, store, null, salesRepo.Object, rndSpAction, "SalesPerson");

            AssertIsEditViewOf<Store>(result);
        }

        


        public void EditActionAsFindParmsForObject(Store store) {
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            var salesRepo = GetService(("SalesRepository"));
            var spAction = GetAction(salesRepo, "FindSalesPersonByName");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), ActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }

        public void InvokeEditActionAsFindParmsForObject(Store store) {
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            var salesRepo = GetService(("SalesRepository"));
            var spAction = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, store, null, salesRepo.Object, spAction, "SalesPerson");
        }

        public void InvokeEditActionAsFindParmsForObjectWithParms(Store store) {
            var salesRepo = GetService(("SalesRepository"));
            var spAction = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&contextActionId=";
            FormCollection form = GetFormForFindSalesPersonByName(salesRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            //AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            AssertIsEditViewOf<Store>(result);
        }

        private void FindForActionUpdatesViewState(bool testValue) {
            var action = GetAction(OrderContrib, "CreateNewOrder");
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + OrderContribId +
                          "&spec=AdventureWorksModel.Store" +
                          "&propertyName=cust" +
                          "&contextActionId=" + action.Id;
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, Finder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 0, OrderContrib.Object, action, null, null, "cust");
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

        public void SelectForActionUpdatesViewState(bool testValue) {
            var action = GetAction(OrderContrib, "CreateNewOrder");
            Store customer = NakedObjectsFramework.Persistor.Instances<Store>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "cust=" + GetObjectId(customer);
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, Selector = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, OrderContrib.Object, action, null, null, null);
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

        public void ActionAsFindParmsForActionUpdatesViewState(bool testValue) {
            var action = GetAction(OrderContrib, "CreateNewOrder");

            var findByName = CustomerRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindStoreByName");
            string data = "contextObjectId=" + OrderContribId +
                          "&spec=AdventureWorksModel.Store" +
                          "&propertyName=cust" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindStoreByName" +
                          "&targetObjectId=" + CustomerRepoId;
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForCreateNewOrder(action, "", testValue, out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = OrderContribId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, OrderContrib.Object, action, CustomerRepo.Object, findByName, "cust");
            AssertStateInModelStateDictionary(result, "OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input", testValue.ToString());
        }

        [Test]
        // todo fix with paging
        public void AAInitialInvokeContributedActionOnEmptyCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False",
                InvokeAction = "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"},
                {"checkboxAll", @"true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;63150;False;;0", "false,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;57033;False;;0", "false,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51798;False;;0", "false,false"}
            };

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(form));

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            string[] warnings = MessageBroker.Warnings.ToArray();
            Assert.AreEqual("No objects selected", warnings.First());
        }

        [Test]
        public void ActionAsFindNoParmsForActionReturnMulti() {
            var action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            var contactRepo = GetService("ContactRepository");
            var randomContact = GetAction(contactRepo, "RandomContacts");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=RandomContacts" +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 2, EmployeeRepo.Object, action, contactRepo.Object, randomContact, "contactDetails");
        }

        [Test]
        public void ActionAsFindNoParmsForActionReturnOne() {
            var action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            var contactRepo = GetService("ContactRepository");
            var randomContact = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "RandomContact");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=contactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=RandomContact" +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, ActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "ActionDialog", null, EmployeeRepo.Object, action, null, null, null);
        }

        [Test]
        public void ActionAsFindParmsForAction() {
            var action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            var contactRepo = GetService("ContactRepository");
            var findByName = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + GetObjectId(contactRepo);
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
            var action = GetAction(OrderContributedActions, "FindRate");
            var orderContribAction = GetService("OrderContributedActions");
            var findByName = orderContribAction.Specification.GetActionLeafNodes().Single(a => a.Id == "FindRate");
            string data = "contextObjectId=" + OrderContributedActionsId +
                          "&spec=AdventureWorksModel.CurrencyRate" +
                          "&propertyName=CurrencyRate" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindRate" +
                          "&targetObjectId=" + GetObjectId(orderContribAction);
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

            var adaptedContact = Surface.GetObject(contact);
            var action = GetAction(adaptedContact, "ChangePassword");

            FormCollection form = GetFormForChangePassword(adaptedContact, "p1", "p2", "p3");
            var objectModel = new ObjectAndControlData {Id = GetObjectId(contact), ActionId = action.Id};

            var result = (ViewResult) controller.Action(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Any());
            AssertIsDialogViewOfAction(result, "Change Password");
        }

        [Test]
        public void ActionGet() {
            SalesOrderHeader order = Order;
            var action = GetAction(Surface.GetObject(order), "Recalculate");
            var objectModel = new ObjectAndControlData {
                ActionId = "Recalculate",
                Id = GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel);
            AssertNameAndParms(result, "ActionDialog", null, order, action, null, null, null);
        }

        [Test]
        public void ActionOnNotPersistedObject() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleAction"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void ActionOnNotPersistedObjectWithReturn() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId, InvokeAction = "targetObjectId=" + objectId + "&targetActionId=SimpleActionWithReturn"};

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsDetailsViewOf<NotPersistedObject>(result);
            Assert.AreEqual("aName", ((NotPersistedObject) result.ViewData.Model).Name);
        }

        [Test]
        public void CrossFieldValidationFail() {
            IDictionary<string, string> idToRawvalue;

            var cc = TransientCreditCard;
            var adaptedCC = Surface.GetObject(cc);

            FormCollection form = GetFormForCeditCardEdit(adaptedCC, "Vista", "12345", "1", "2010", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(cc)};

            var result = (ViewResult) controller.Edit(objectModel, form);

            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.IsTrue(result.ViewData.ModelState[""].Errors.Any());
            AssertIsEditViewOf<CreditCard>(result);
        }

        [Test]
        public void CrossFieldValidationSuccess() {
            IDictionary<string, string> idToRawvalue;
            var cc = TransientCreditCard;
            var adaptedCC = Surface.GetObject(cc);
            cc.Creator = new TestCreator();

            FormCollection form = GetFormForCeditCardEdit(adaptedCC, "Vista", "12345", "1", "2020", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(cc)};

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

        [Test, Ignore] //Problem with viewing/editing the TimePeriod on Shift
        public void EditInlineSaveValidationFail() {
            EditInlineSaveValidationFail(Employee.DepartmentHistory.First().Shift);
        }

        [Test, Ignore] //Problem with viewing/editing the TimePeriod on Shift
        public void EditInlineSaveValidationFailForTransient() {
            EditInlineSaveValidationFail(TransientShift);
        }

        [Test, Ignore] //Problem with viewing/editing the TimePeriod on Shift
        public void EditInlineSaveValidationOk() {
            EditInlineSaveValidationOk(Employee.DepartmentHistory.First().Shift, 0);
        }

        [Test, Ignore] //Problem with viewing/editing the TimePeriod on Shift
        public void EditInlineSaveValidationOkForTransient() {
            EditInlineSaveValidationOk(TransientShift, 1);
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

            var objectModel = new ObjectAndControlData {Id = GetObjectId(shift)};
            var result = (ViewResult) controller.EditObject(objectModel);
            AssertIsEditViewOf<Shift>(result);
        }

        [Test]
        public void EditObjectKeepTableFormat() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdConstants.DisplayFormatFieldId, "Addresses=list&DepartmentHistory=table"}});
            var objectModel = new ObjectAndControlData {Id = EmployeeId};

            var result = (ViewResult) controller.EditObject(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
        }

        [Test]
        public void EditObjectNotPersisted() {
            NotPersistedObject obj = NotPersistedObject;

            string objectId = GetObjectId(obj);
            var objectModel = new ObjectAndControlData {Id = objectId};

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(new Dictionary<string, string>()));
            AssertIsEditViewOf<NotPersistedObject>(result);
        }

        [Test]
       
        public void EditObjectPage() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;Cycling",
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
                Id = GetObjectId(TransientEmployee),
                Redisplay = "editMode=true"
            };

            var form = new FormCollection();

            GetTestService("Customers");

            var result = (ViewResult) controller.EditObject(objectModel, form);
            AssertIsEditViewOf<Employee>(result);
        }

        [Test]
        public void EditObjectWithInline() {
            Shift shift = Employee.DepartmentHistory.First().Shift;

            var objectModel = new ObjectAndControlData {Id = GetObjectId(shift)};
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
            IObjectFacade employeeNakedObject = Surface.GetObject(employee);
            IAssociationFacade collectionAssoc = employeeNakedObject.Specification.Properties.Single(p => p.Id == "DirectReports");

            var form = new FormCollection {
                {IdConstants.DisplayFormatFieldId, "Addresses=list"},
                {IdHelper.GetCollectionItemId((employeeNakedObject), (collectionAssoc)), GetObjectId(report1)},
                {IdHelper.GetCollectionItemId((employeeNakedObject), (collectionAssoc)), GetObjectId(report2)}
            };

            var objectModel = new ObjectAndControlData {Id = GetObjectId(employee), Redisplay = redisplay};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertIsEditViewOf<Employee>(result);

            AssertStateInViewDataDictionary(result, "Addresses", "list");
            AssertStateInViewDataDictionary(result, "DepartmentHistory", "table");
            Assert.AreEqual(2, ((Employee) result.ViewData.Model).DirectReports.Count);
        }

        [Test] 
        [Ignore] // todo fix
        public void EditSaveConcurrencyFail() {
            Store store = Store;
            IObjectFacade adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            string differentDateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), differentDateTime, out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(store)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                controller.Edit(objectModel, form);

                Assert.Fail("Expect concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(adaptedStore, expected.SourceNakedObjectAdapter);
            }
            finally {
                NakedObjectsFramework.TransactionManager.EndTransaction();
            }
        }

        [Test, Ignore] //Haven't successfully added a ConcurrencyCheck to Store or Customer?
        public void EditSaveConcurrencyOk() {
            Store store = Store;
            IObjectFacade adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            FormCollection form = GetFormForStoreEdit(adaptedStore, store.Name, GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(CultureInfo.CurrentCulture), out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(store)};

            NakedObjectsFramework.TransactionManager.StartTransaction();
            try {
                var result = (ViewResult) controller.Edit(objectModel, form);

                foreach (KeyValuePair<string, string> kvp in idToRawvalue) {
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

        [Test]
        public void EditApplyActionValidationOk() {
            EditApplyActionValidationOk(Vendor);
        }

        [Test]
        public void EditSaveAndCloseValidationOkForTransient() {
            EditSaveValidationOk(TransientVendor, true);
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
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IDictionary<string, string> idToRawvalue;
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id;
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
            Product product = Product;

            FileContentResult result = controller.GetFile(GetObjectId(Product), "Photo");

            Stream stream = product.Photo.GetResourceAsStream();
            byte[] bytes;
            using (var br = new BinaryReader(stream)) {
                bytes = br.ReadBytes((int) stream.Length);
            }

            Assert.IsTrue(bytes.SequenceEqual(result.FileContents));
        }

        [Test]
        public void InitialInvokeContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False",
                InvokeAction = "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=AppendComment&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False;False;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;502;False;;0;False&contextActionId=&propertyName=&page=1&pageSize=20"},
                {"checkboxAll", @"true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;71936;False;;0", "true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;57033;False;;0", "true,false"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51798;False;;0", "true,false"}
            };

            var result = (ViewResult) controller.EditObject(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Append Comment");
        }

        // run first

        [Test]
        public void InitialInvokeCovariantContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling",
                InvokeAction = "targetActionId=ShowCustomersWithAddressInRegion&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextActionId=&propertyName=&page=1&pageSize=2"
            };

            var form = new Dictionary<string, string> {
                {"InvokeAction", "targetActionId=ShowCustomersWithAddressInRegion&targetObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextObjectId=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&contextActionId=&propertyName=&page=1&pageSize=2"},
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
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IObjectFacade contactRepo = GetService("ContactRepository");
            IActionFacade findByName = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            FormCollection form = GetForm(new Dictionary<string, string>());
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithFinderDialog", null, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test]
        public void InvokeActionAsFindParmsForActionWithParms() {
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IObjectFacade contactRepo = GetService("ContactRepository");
            IActionFacade findByName = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindContactByName" +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            FormCollection form = GetFormForFindContactByName(contactRepo, "", "Carson");
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsFinder = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 11, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
        }

        [Test]
       
        public void InvokeActionAsSaveForActionFailValidation() {
            Store store = Store;
            Store transientStore = TransientStore;
            var adaptedStore = Surface.GetObject(store);
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IObjectFacade contactRepo = GetService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            IActionFacade findByName = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + GetObjectId(transientStore) +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", GetObjectId(store.SalesPerson), store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeActionAsSaveForActionPassValidation() {
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            var adaptedStore = Surface.GetObject(store);
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            IObjectFacade contactRepo = GetService("ContactRepository");
            IDictionary<string, string> idToRawvalue;
            IActionFacade findByName = contactRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindContactByName");
            string data = "contextObjectId=" + EmployeeRepoId +
                          "&spec=AdventureWorksModel.Contact" +
                          "&propertyName=ContactDetails" +
                          "&contextActionId=" + action.Id +
                          "&targetActionId=FindContactByName" +
                          "&subEditObjectId=" + GetObjectId(transientVendor) +
                          "&targetObjectId=" + GetObjectId(contactRepo);
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            IObjectFacade adaptedVendor = Surface.GetObject(transientVendor);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = EmployeeRepoId, ActionId = action.Id, InvokeActionAsSave = data};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, EmployeeRepo.Object, action, contactRepo.Object, findByName, "ContactDetails");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]           
        public void InvokeActionWithMultiSelectObjects() {
            string id = GetObjectId(Order);

            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasons",
                Id = id
            };

            var form = new FormCollection {{"SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;1;False;;0"}, {"SalesOrderHeader-AddNewSalesReasons-Reasons-Select", @"AdventureWorksModel.SalesReason;1;System.Int32;2;False;;0"}};

            IObjectFacade order = Surface.GetObject(Order);
            IAssociationFacade assocMD = order.Specification.Properties.Single(p => p.Id == "ModifiedDate");
            IActionFacade action = order.Specification.GetActionLeafNodes().Single(a => a.Id == "AddNewSalesReasons");

            string idMD = IdHelper.GetConcurrencyActionInputId((order), (action), (assocMD));

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.CurrentCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        
        // needs to be implemented !
        public void InvokeActionWithMultiSelectParseable() {
            string id = GetObjectId(Order);

            var objectModel = new ObjectAndControlData {
                ActionId = "AddNewSalesReasonsByCategories",
                Id = id
            };

            var form = new FormCollection();

            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"1");
            form.Add("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories-Select", @"2");

            IObjectFacade order = Surface.GetObject(Order);
            IAssociationFacade assocMD = order.Specification.Properties.Single(p => p.Id == "ModifiedDate");
            IActionFacade action = order.Specification.GetActionLeafNodes().Single(a => a.Id == "AddNewSalesReasonsByCategories");

            string idMD = IdHelper.GetConcurrencyActionInputId((order), (action), (assocMD));

            form.Add(idMD, Order.ModifiedDate.ToString(CultureInfo.CurrentCulture));

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        
        // todo make collection contributed actions work
        public void InvokeContributedActionOnCollectionTarget() {
            var objectModel = new ObjectAndControlData {
                ActionId = "AppendComment",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-CommentToAppend-Input", "comment"},
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"}
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(form));

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
        }

        [Test]
        
        // todo make collection contributed actions work
        public void InvokeContributedActionOnCollectionTargetValidateFails() {
            var objectModel = new ObjectAndControlData {
                ActionId = "AppendComment",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-CommentToAppend-Input", ""},
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;71793;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;65219;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;55270;False;;0", "true"},
                {"AdventureWorksModel.SalesOrderHeader;1;System.Int32;51083;False;;0", "true"}
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(form));

            AssertIsDialogViewOfAction(result, "Append Comment");
        }

        [Test]
        [Ignore] // todo fix
        public void InvokeContributedActionOnCollectionTargetValidateFailsSingleParm() {
            var objectModel = new ObjectAndControlData {
                ActionId = "CommentAsUsersUnhappy",
                Id = @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"
            };

            var form = new Dictionary<string, string> {
                {"OrderContributedActions-AppendComment-ToOrders-Select", @"System.Linq.IQueryable%601-AdventureWorksModel.SalesOrderHeader;RecentOrders;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False;Object;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.Store;1;System.Int32;561;False;;0;False"},
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
                Id = GetObjectId(store),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(
                new Dictionary<string, string> {
                    {"Store-LastOrder-ModifiedDate-Concurrency", store.ModifiedDate.ToString(CultureInfo.CurrentCulture)}
                }));

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test] //Haven't successfully added a ConcurrencyCheck to Store or Customer?
        [Ignore] // todo fix
        public void InvokeContributedActionOnTargetConcurrencyFail() {
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "LastOrder",
                Id = GetObjectId(store),
                InvokeAction = "action=action"
            };

            try {
                var result = (ViewResult) controller.Action(objectModel, GetForm(
                    new Dictionary<string, string> {
                        {"Store-LastOrder-ModifiedDate-Concurrency", DateTime.Now.ToString(CultureInfo.CurrentCulture)}
                    }));

                Assert.Fail("Expected concurrency exception");
            }
            catch (ConcurrencyException expected) {
                Assert.AreSame(store, expected.SourceNakedObjectAdapter.Object);
            }
        }

        [Test]
        public void InvokeContributedActionOnTargetPopulatesTargetParm() {
            Store store = Store;
            var objectModel = new ObjectAndControlData {
                ActionId = "CreateNewOrder",
                Id = GetObjectId(store),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(
                new Dictionary<string, string> {
                    {"Store-CreateNewOrder-ModifiedDate-Concurrency", store.ModifiedDate.ToString(CultureInfo.CurrentCulture)}
                }));

            AssertIsDialogViewOfAction(result, "Create New Order");
            Assert.IsTrue(result.ViewData.ContainsKey("OrderContributedActions-CreateNewOrder-Customer-Select"));
            Assert.AreEqual(Surface.GetObject(store).Object, ((dynamic) result.ViewData["OrderContributedActions-CreateNewOrder-Customer-Select"]).Object);
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
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            IObjectFacade salesRepo = GetService("SalesRepository");
            IActionFacade spAction = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&subEditObjectId=" + GetObjectId(transientStore) +
                          "&contextActionId=";
            FormCollection form = GetFormForStoreEdit(adaptedStore, "", GetObjectId(store.SalesPerson), Store.ModifiedDate.ToString(), out idToRawvalue);
            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeEditActionAsSaveForObjectPassValidation() {
            Store store = Store;
            Vendor transientVendor = TransientVendor;
            var adaptedStore = Surface.GetObject(store);
            IDictionary<string, string> idToRawvalue;
            IObjectFacade salesRepo = GetService("SalesRepository");
            IActionFacade spAction = salesRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindSalesPersonByName");
            string data = "contextObjectId=" + GetObjectId(store) +
                          "&spec=AdventureWorksModel.SalesPerson" +
                          "&propertyName=SalesPerson" +
                          "&targetActionId=FindSalesPersonByName" +
                          "&targetObjectId=" + GetObjectId(salesRepo) +
                          "&subEditObjectId=" + GetObjectId(transientVendor) +
                          "&contextActionId=";

            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            IObjectFacade adaptedVendor = Surface.GetObject(transientVendor);

            FormCollection form = GetFormForVendorEdit(adaptedVendor, uniqueActNum, "AName", "1", "True", "True", "", out idToRawvalue);

            var objectModel = new ObjectAndControlData {Id = GetObjectId(store), InvokeActionAsSave = data};

            var result = (ViewResult) controller.Edit(objectModel, form);

            AssertNameAndParms(result, "FormWithSelections", 1, store, null, salesRepo.Object, spAction, "SalesPerson");
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSet() {
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleAction", Id = GetObjectId(obj)};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDetailsViewOf<NotPersistedObject>(result);
        }

        [Test]
        public void InvokeNotPersistedObjectActionParmsSetWithReturn() {
            NotPersistedObject obj = NotPersistedObject;

            FormCollection form = GetForm(new Dictionary<string, string> {{"NotPersistedObject-Name-Input", "aName"}});
            var objectModel = new ObjectAndControlData {ActionId = "SimpleActionWithReturn", Id = GetObjectId(obj)};

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
                Id = GetObjectId(order),
                InvokeAction = "action=action"
            };

            var result = (ViewResult) controller.Action(objectModel, GetForm(new Dictionary<string, string> {{"SalesOrderHeader-Recalculate-ModifiedDate-Concurrency", order.ModifiedDate.ToString(CultureInfo.CurrentCulture)}}));

            AssertIsSetAfterTransactionViewOf<SalesOrderHeader>(result);
        }

        [Test]
        public void InvokeObjectActionParmsNotSet() {
            IObjectFacade adaptedProduct = Surface.GetObject(Product);
            FormCollection form = GetFormForBestSpecialOffer(adaptedProduct, "");
            var objectModel = new ObjectAndControlData {ActionId = "BestSpecialOffer", Id = ProductId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsDialogViewOfAction(result, "Best Special Offer");

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Product-BestSpecialOffer-Quantity-Input"));
            Assert.AreEqual("Mandatory", result.ViewData.ModelState["Product-BestSpecialOffer-Quantity-Input"].Errors[0].ErrorMessage);
        }

        [Test]
        public void InvokeObjectActionParmsSet() {
            IObjectFacade adaptedProduct = Surface.GetObject(Product);
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
        // broken fix 
        public void InvokeObjectActionReturnOrderedPagedCollectionAsc() {
            var expectedCount = NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().Count(); 

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

            AssertPagingData(result, 1, 20, expectedCount);
        }

        [Test]
        // broken fix 
        public void InvokeObjectActionReturnOrderedPagedCollectionDesc() {
            var expectedCount = NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().Count(); 


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

            AssertPagingData(result, 1, 20, expectedCount);
        }

        [Test]
        // broken fix 
        public void InvokeObjectActionReturnPagedCollection() {
            var expectedCount = NakedObjectsFramework.Persistor.Instances<SalesOrderHeader>().Count(); 


            var form = new FormCollection();
            var objectModel = new ObjectAndControlData {ActionId = "HighestValueOrders", Id = OrderRepoId};

            var result = (ViewResult) controller.Action(objectModel, form);

            AssertIsQueryableViewOf<SalesOrderHeader>(result);
            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) result.ViewData.Model).Count());

            var arm = (ActionResultModel) result.ViewData.Model;

            Assert.AreEqual(20, ((IQueryable<SalesOrderHeader>) arm.Result).Count());
            Assert.AreEqual("HighestValueOrders", arm.Action.Id);

            AssertPagingData(result, 1, 20, expectedCount);
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
            IActionFacade action = GetAction(EmployeeRepo, "CreateNewEmployeeFromContact");
            Contact contactDetails = NakedObjectsFramework.Persistor.Instances<Contact>().First();
            IDictionary<string, string> idToRawvalue;
            string data = "ContactDetails=" + GetObjectId(contactDetails);
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
            FormCollection form = GetForm(new Dictionary<string, string> {
                {"CustomerRepository-ShowCustomersWithAddressInRegion-Region-Select", ""},
                {"Details", "id=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;True;System.String;cycling&page=1&pageSize=2"},
                {"CustomerRepository-ShowCustomersWithAddressInRegion-Customers-Select", "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;True;System.String;cycling"},
                {"AdventureWorksModel.Store;1;System.Int32;298;False;;0", "true"},
                {"AdventureWorksModel.Store;1;System.Int32;421;False;;0", "true"}
            });

            var objectModel = new ObjectAndControlData {
                Id = "System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling",
                Details = "id=System.Linq.IQueryable%601-AdventureWorksModel.Store;FindStoreByName;NakedObjects.Persistor.Entity.Adapter.EntityOid;8;AdventureWorksModel.CustomerRepository;1;System.Int32;0;False;;0;False;Value;System.String;cycling&page=1&pageSize=2"
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
            FormCollection form = GetForm(new Dictionary<string, string> {{IdConstants.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = EmployeeId, Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);

            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsCancelTransient() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdConstants.DisplayFormatFieldId, "Addresses=list"}});
            const string cancel = "cancel=cancel";
            var objectModel = new ObjectAndControlData {Id = GetObjectId(TransientEmployee), Cancel = cancel};

            var result = (ViewResult) controller.Details(objectModel, form);
            AssertIsSetAfterTransactionViewOf<Employee>(result);
        }

        [Test]
        public void ViewObjectDetailsRedisplay() {
            FormCollection form = GetForm(new Dictionary<string, string> {{IdConstants.DisplayFormatFieldId, "Addresses=list"}});
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
                Id = GetObjectId(TransientEmployee),
                Redisplay = "editMode=false"
            };

            var form = new FormCollection();

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

        [Test]
        public void RedirectDetails() {
            var objectModel = new ObjectAndControlData {Id = ProductId};
            var redirecturl = @"http://redirectedurl1";
            Product.SetRedirectUrl(redirecturl);

            try {
                controller.Details(objectModel);
                Assert.Fail("Expect exception");
            }
            catch (RedirectException rde) {
                Assert.AreEqual(rde.Url, redirecturl);
            }
            catch (Exception e) {
                Assert.Fail("Unexpected exception " + e);
            }
            finally {
                Product.SetRedirectUrl(null);
            }
        }

        [Test]
        public void RedirectAction() {

            var objectModel = new ObjectAndControlData {
                ActionId = "RandomProduct",
                Id = ProductRepoId,
                InvokeAction = "action=action"
            };

            var redirecturl = @"http://redirectedurl2";
            Product.SetRedirectUrl(redirecturl);

            try {
                controller.Action(objectModel, GetForm(new Dictionary<string, string>()));
                Assert.Fail("Expect exception");
            }
            catch (RedirectException rde) {
                Assert.AreEqual(rde.Url, redirecturl);
            }
            catch (Exception e) {
                Assert.Fail("Unexpected exception " + e);
            }
            finally {
                Product.SetRedirectUrl(null);
            }
        }


        #region Nested type: TestCreator

        private class TestCreator : ICreditCardCreator {
            #region ICreditCardCreator Members

            public void CreatedCardHasBeenSaved(CreditCard card) {
                // do nothing
            }

            #endregion
        }

        #endregion

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
            NakedObjectsFramework = ((dynamic) Surface).Framework;
            MessageBroker =  NakedObjectsFramework.MessageBroker;
        }

        #endregion
    }
}