//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System.Data.Entity;
//
//using NakedObjects.Architecture.Menu;
//using NakedObjects.Persistor.Entity.Configuration;
//using NakedObjects.SystemTest.PolymorphicAssociations;
//using NakedObjects.SystemTest.Reflect;
//using Microsoft.Practices.Unity;


//namespace NakedObjects.SystemTest.PolymorphicNavigator {
//    [TestFixture]
//    public class TestPolymorphicNavigator : TestPolymorphicNavigatorAbstract {
//        private const string databaseName = "TestPolymorphicNavigator";

//        protected override string[] Namespaces {
//            get { return new[] {typeof (PolymorphicPayment).Namespace}; }
//        }

//        [Test]
//        public void SetPolymorphicPropertyOnTransientObject() {
//            base.SetPolymorphicPropertyOnTransientObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
//        }

//        [Test]
//        public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
//            base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
//        }

//        [Test]
//        public void SetPolymorphicPropertyOnPersistentObject() {
//            SetPolymorphicPropertyOnPersistentObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
//        }

//        [Test]
//        public void ChangePolymorphicPropertyOnPersistentObject() {
//            ChangePolymorphicPropertyOnPersistentObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee", "NakedObjects.SystemTest.PolymorphicAssociations.SupplierAsPayee");
//        }

//        [Test]
//        public override void ClearPolymorphicProperty() {
//            base.ClearPolymorphicProperty();
//        }

//        [Test]
//        public void PolymorphicCollectionAddMutlipleItemsOfOneType() {
//            base.PolymorphicCollectionAddMutlipleItemsOfOneType("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
//        }

//        [Test]
//        public void PolymorphicCollectionAddDifferentItems() {
//            base.PolymorphicCollectionAddDifferentItems("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem", "NakedObjects.SystemTest.PolymorphicAssociations.ExpenseClaimAsPayableItem");
//        }

//        [Test]
//        public override void AttemptToAddSameItemTwice() {
//            base.AttemptToAddSameItemTwice();
//        }

//        [Test]
//        public override void RemoveItem() {
//            base.RemoveItem();
//        }

//        [Test]
//        public override void AttemptToRemoveNonExistentItem() {
//            base.AttemptToRemoveNonExistentItem();
//        }

//        [Test]
//        public override void FindOwnersForObject() {
//            base.FindOwnersForObject();
//        }

//        #region SetUp

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

//        [SetUp()]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            if (!fixturesRun) {
//                RunFixtures();
//                fixturesRun = true;
//            }
//            StartTest();
//        }

//        protected override object[] Fixtures {
//            get { return new object[] {new FixtureEntities(), new FixtureLinksUsingTypeName()}; }
//        }

//        #endregion
//    }
//}