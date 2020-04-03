//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Data.Entity;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NakedObjects.Architecture.Menu;
//using NakedObjects.Persistor.Entity.Configuration;
//using NakedObjects.Services;
//using NakedObjects.SystemTest.PolymorphicAssociations;
//using NakedObjects.SystemTest.Reflect;
//using Microsoft.Practices.Unity;


//namespace NakedObjects.SystemTest.PolymorphicNavigator {
//    [TestClass]
//    public class TestPolymorphicNavigatorWithTypeCodeMapper : TestPolymorphicNavigatorAbstract {
//        protected override string[] Namespaces {
//            get { return new[] {typeof (PolymorphicPayment).Namespace}; }
//        }

//        [TestMethod]
//        public void SetPolymorphicPropertyOnTransientObject() {
//            base.SetPolymorphicPropertyOnTransientObject("CUS");
//        }

//        [TestMethod]
//        public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
//            base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
//        }

//        [TestMethod]
//        public void SetPolymorphicPropertyOnPersistentObject() {
//            base.SetPolymorphicPropertyOnPersistentObject("CUS");
//        }

//        [TestMethod]
//        public void ChangePolymorphicPropertyOnPersistentObject() {
//            ChangePolymorphicPropertyOnPersistentObject("CUS", "SUP");
//        }

//        [TestMethod]
//        public override void ClearPolymorphicProperty() {
//            base.ClearPolymorphicProperty();
//        }

//        [TestMethod]
//        public void PolymorphicCollectionAddMutlipleItemsOfOneType() {
//            base.PolymorphicCollectionAddMutlipleItemsOfOneType("INV");
//        }

//        [TestMethod]
//        public void PolymorphicCollectionAddDifferentItems() {
//            base.PolymorphicCollectionAddDifferentItems("INV", "EXP");
//        }

//        [TestMethod]
//        public override void AttemptToAddSameItemTwice() {
//            base.AttemptToAddSameItemTwice();
//        }

//        [TestMethod]
//        public override void RemoveItem() {
//            base.RemoveItem();
//        }

//        [TestMethod]
//        public override void AttemptToRemoveNonExistentItem() {
//            base.AttemptToRemoveNonExistentItem();
//        }

//        [TestMethod]
//        public override void FindOwnersForObject() {
//            base.FindOwnersForObject();
//        }

//        #region

//        private const string databaseName = "TestPolymorphicNavigatorWithTypeCodeMapper";

//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
//            config.UsingCodeFirstContext(() => new PolymorphicNavigationContext(databaseName));
//            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
//            container.RegisterType<IMenuFactory, ReflectorTest.NullMenuFactory>();
//        }

//        [ClassCleanup]
//        public static void DeleteDatabase() {
//            Database.Delete(databaseName);
//        }

//        private static bool fixturesRun;

//        [TestInitialize()]
//        public void TestInitialize() {
//            InitializeNakedObjectsFrameworkOnce();
//            if (!fixturesRun) {
//                RunFixtures();
//                fixturesRun = true;
//            }
//            StartTest();
//        }

//        protected override object[] Fixtures {
//            get { return new object[] {new FixtureEntities(), new FixtureLinksUsingTypeCode()}; }
//        }

//        protected override object[] SystemServices {
//            get { return new object[] {new Services.PolymorphicNavigator(), new SimpleTypeCodeMapper()}; }
//        }

//        #endregion
//    }

//    public class SimpleTypeCodeMapper : ITypeCodeMapper {
//        #region ITypeCodeMapper Members

//        public Type TypeFromCode(string code) {
//            if (code == "CUS") { return typeof (CustomerAsPayee); }
//            if (code == "SUP") { return typeof (SupplierAsPayee); }
//            if (code == "INV") { return typeof (InvoiceAsPayableItem); }
//            if (code == "EXP") { return typeof (ExpenseClaimAsPayableItem); }
//            throw new DomainException("Code not recognised: " + code);
//        }

//        public string CodeFromType(Type type) {
//            if (type == typeof (CustomerAsPayee)) { return "CUS"; }
//            if (type == typeof (SupplierAsPayee)) { return "SUP"; }
//            if (type == typeof (InvoiceAsPayableItem)) { return "INV"; }
//            if (type == typeof (ExpenseClaimAsPayableItem)) { return "EXP"; }
//            throw new DomainException("Type not recognised: " + type);
//        }

//        #endregion
//    }
//}