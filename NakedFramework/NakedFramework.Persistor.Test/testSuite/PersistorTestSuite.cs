// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NUnit.Framework;
using TestData;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.TestSuite {
    /// <summary>
    ///     Prerequisite - TestData Fixture run and NakedObjects framework setup
    /// </summary>
    public class PersistorTestSuite {
        private readonly INakedObjectsFramework framework;

        public PersistorTestSuite(INakedObjectsFramework framework) => this.framework = framework;

        #region helpers

        private ILifecycleManager LifecycleManager => framework.LifecycleManager;

        private IObjectPersistor Persistor => framework.Persistor;

        private ITransactionManager TransactionManager => framework.TransactionManager;

        private IMetamodelManager Metamodel => framework.MetamodelManager;

        private INakedObjectAdapter AdapterFor(object domainObject) => framework.NakedObjectManager.CreateAdapter(domainObject, null, null);

        private Person GetPerson(int id) {
            return framework.Persistor.Instances<Person>().Single(p => p.PersonId == id);
        }

        private static void AssertIsPerson(Person person, int id) {
            Assert.IsNotNull(person, "Failed to get instance");
            Assert.AreEqual(id, person.PersonId);
        }

        private Person ChangeScalarOnPerson(int id) {
            var person = GetPerson(id);
            var originalName = person.Name;
            TransactionManager.StartTransaction();
            person.Name = Guid.NewGuid().ToString();
            TransactionManager.EndTransaction();
            person.ResetEvents();
            TransactionManager.StartTransaction();
            person.Name = originalName;
            TransactionManager.EndTransaction();
            return person;
        }

        private Product GetProduct(int id) {
            return Persistor.Instances<Product>().Single(p => p.Id == id);
        }

        private Person ChangeReferenceOnPerson(int id) {
            var person = GetPerson(id);
            person.ResetEvents();
            TransactionManager.StartTransaction();
            person.FavouriteProduct = GetProduct(3);
            TransactionManager.EndTransaction();
            return person;
        }

        private Person AddToCollectionOnPersonOne(Person personToAdd) {
            var person = GetPerson(1);
            person.ResetEvents();
            TransactionManager.StartTransaction();
            person.AddToRelatives(personToAdd);
            TransactionManager.EndTransaction();
            return person;
        }

        private Person RemoveFromCollectionOnPersonOne(Person personToRemove) {
            var person = GetPerson(1);
            person.ResetEvents();
            TransactionManager.StartTransaction();
            person.RemoveFromRelatives(personToRemove);
            TransactionManager.EndTransaction();
            return person;
        }

        private Person ClearCollectionOnPerson(int id) {
            var person = GetPerson(id);
            person.ResetEvents();
            TransactionManager.StartTransaction();
            person.ClearRelatives();
            TransactionManager.EndTransaction();
            return person;
        }

        private Product GetProductFromPersonOne() {
            TransactionManager.StartTransaction();
            var person1 = GetPerson(1);
            var product = person1.FavouriteProduct;
            // ReSharper disable once UnusedVariable
            var name = product.Name; // to ensure product is resolved
            TransactionManager.EndTransaction();
            return product;
        }

        private Person GetPersonFromPersonOneCollection() {
            TransactionManager.StartTransaction();
            var relative = GetPerson(1).Relatives.First();
            TransactionManager.EndTransaction();
            return relative;
        }

        private Person CreateNewTransientPerson() {
            var nextIndex = Persistor.Instances<Person>().Select(p => p.PersonId).Max() + 1;
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Person));
            var newPersonAdapter = LifecycleManager.CreateInstance(spec);
            var person = (Person) newPersonAdapter.Object;
            person.PersonId = nextIndex;
            return person;
        }

        private Order CreateNewTransientOrder() {
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Order));
            var newOrderAdapter = LifecycleManager.CreateInstance(spec);
            var order = (Order) newOrderAdapter.Object;
            order.OrderId = 0;
            return order;
        }

        private OrderFail CreateNewTransientOrderFail() {
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(OrderFail));
            var newOrderAdapter = LifecycleManager.CreateInstance(spec);
            var order = (OrderFail) newOrderAdapter.Object;
            order.OrderFailId = 0;
            return order;
        }

        private Product CreateNewTransientProduct() {
            var nextIndex = Persistor.Instances<Product>().Select(p => p.Id).Max() + 1;
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Product));
            var newProductAdapter = LifecycleManager.CreateInstance(spec);
            var product = (Product) newProductAdapter.Object;
            product.Id = nextIndex;
            return product;
        }

        private Pet CreateNewTransientPet() {
            var nextIndex = Persistor.Instances<Pet>().Select(p => p.PetId).Max() + 1;
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Pet));
            var newPetAdapter = LifecycleManager.CreateInstance(spec);
            var pet = (Pet) newPetAdapter.Object;
            pet.PetId = nextIndex;
            return pet;
        }

        private INakedObjectAdapter Save(object toSave) {
            TransactionManager.StartTransaction();
            var adapterForToSave = AdapterFor(toSave);
            LifecycleManager.MakePersistent(adapterForToSave);
            TransactionManager.EndTransaction();
            return adapterForToSave;
        }

        private void Delete(object toDelete) {
            TransactionManager.StartTransaction();
            var adapterForToDelete = AdapterFor(toDelete);
            Persistor.DestroyObject(adapterForToDelete);
            TransactionManager.EndTransaction();
        }

        private Address ChangeScalarOnAddress() {
            var adaptedAddress = GetAdaptedAddress(GetPerson(1));
            var address = (Address) adaptedAddress.Object;
            var original1 = address.Line1;
            TransactionManager.StartTransaction();
            address.Line1 = Guid.NewGuid().ToString();
            TransactionManager.EndTransaction();
            address.ResetEvents();
            TransactionManager.StartTransaction();
            address.Line1 = original1;
            TransactionManager.EndTransaction();
            return address;
        }

        private INakedObjectAdapter GetAdaptedAddress(Person person) {
            var personAdapter = AdapterFor(person);
            return ((IObjectSpec) personAdapter.Spec).GetProperty("Address").GetNakedObject(personAdapter);
        }

        private INakedObjectAdapter GetAdaptedRelatives(Person person) {
            TransactionManager.StartTransaction();
            var personAdapter = AdapterFor(person);
            TransactionManager.EndTransaction();
            return ((IObjectSpec) personAdapter.Spec).GetProperty("Relatives").GetNakedObject(personAdapter);
        }

        #endregion

        #region tests

        public void GetInstanceFromInstancesOfT() {
            var person = GetPerson(1);
            AssertIsPerson(person, 1);
        }

        public void GetInstanceFromInstancesOfType() {
            var person = Persistor.Instances(typeof(Person)).Cast<Person>().Single(p => p.PersonId == 1);
            AssertIsPerson(person, 1);
        }

        public void GetInstanceFromInstancesOfSpecification() {
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Person));
            var person = Persistor.Instances(spec).Cast<Person>().Single(p => p.PersonId == 1);
            AssertIsPerson(person, 1);
        }

        public void GetInstanceIsAlwaysSameObject() {
            var spec = (IObjectSpec) Metamodel.GetSpecification(typeof(Person));
            var person1 = GetPerson(1);
            var person2 = Persistor.Instances(typeof(Person)).Cast<Person>().Single(p => p.PersonId == 1);
            var person3 = Persistor.Instances(spec).Cast<Person>().Single(p => p.PersonId == 1);
            Assert.AreSame(person1, person2);
            Assert.AreSame(person2, person3);
        }

        public void GetInstanceResolveStateIsPersistent() {
            var adapter = AdapterFor(GetPerson(1));
            Assert.IsTrue(adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(adapter.Oid.IsTransient, "is transient");
        }

        public void GetInstanceHasVersion() {
            var adapter = AdapterFor(GetPerson(1));
            Assert.IsNotNull(adapter.Version, "should have version");
        }

        public void ChangeScalarOnPersistentCallsUpdatingUpdated() {
            var person = ChangeScalarOnPerson(1);
            Assert.AreEqual(1, person.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person.GetEvents()["Updated"], "updated");
        }

        public void ChangeReferenceOnPersistentCallsUpdatingUpdated() {
            var person = ChangeReferenceOnPerson(7);
            Assert.AreEqual(1, person.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person.GetEvents()["Updated"], "updated");
        }

        public void UpdatedDoesntCallPersistedAtOnce() {
            var person = GetPerson(1);
            person.PersistInUpdated();
            ChangeScalarOnPerson(1);
            Assert.AreEqual(1, person.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person.GetEvents()["Updated"], "updated");
        }

        public void AddToCollectionOnPersistent() {
            var person1 = GetPerson(1);
            var person4 = GetPerson(4);
            var countbefore = person1.Relatives.Count;
            AddToCollectionOnPersonOne(person4);
            Assert.AreEqual(countbefore + 1, person1.Relatives.Count);
        }

        public void AddToCollectionOnPersistentCallsUpdatingUpdated() {
            var person6 = GetPerson(6);
            var person1 = AddToCollectionOnPersonOne(person6);
            Assert.AreEqual(1, person1.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person1.GetEvents()["Updated"], "updated");
        }

        public void RemoveFromCollectionOnPersistent() {
            var person1 = GetPerson(1);
            var person2 = GetPerson(2);
            var countbefore = person1.Relatives.Count;
            RemoveFromCollectionOnPersonOne(person2);
            Assert.AreEqual(countbefore - 1, person1.Relatives.Count);
        }

        public void RemoveFromCollectionOnPersistentCallsUpdatingUpdated() {
            var person8 = GetPerson(8);
            var person1 = RemoveFromCollectionOnPersonOne(person8);
            Assert.AreEqual(1, person1.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person1.GetEvents()["Updated"], "updated");
        }

        public void ClearCollectionOnPersistent() {
            var person = ClearCollectionOnPerson(6);
            Assert.AreEqual(0, person.Relatives.Count);
        }

        public void ClearCollectionOnPersistentCallsUpdatingUpdated() {
            var person = ClearCollectionOnPerson(8);
            Assert.AreEqual(1, person.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person.GetEvents()["Updated"], "updated");
        }

        public void LoadObjectReturnSameObject() {
            var person1 = GetPerson(1);
            var adapter1 = AdapterFor(person1);
            var adapter2 = Persistor.LoadObject(adapter1.Oid, (IObjectSpec) adapter1.Spec);
            Assert.AreSame(person1, adapter2.Object);
        }

        public void PersistentObjectHasContainerInjected() {
            var person1 = GetPerson(1);
            Assert.IsTrue(person1.HasContainer, "no container injected");
        }

        public void PersistentObjectHasServiceInjected() {
            var person1 = GetPerson(1);
            Assert.IsTrue(person1.HasMenuService, "no service injected");
        }

        public void PersistentObjectHasLoadingLoadedCalled() {
            TransactionManager.StartTransaction();
            var person1 = GetPerson(1);
            TransactionManager.EndTransaction();
            Assert.AreEqual(1, person1.GetEvents()["Loading"], "loading");
            Assert.AreEqual(1, person1.GetEvents()["Loaded"], "loaded");
        }

        public void CanAccessReferenceProperty() {
            var person1 = GetPerson(1);
            var product = person1.FavouriteProduct;
            Assert.IsNotNull(product, "Failed to access instance");
            Assert.AreEqual("ProductOne", product.Name);
        }

        public void CanAccessCollectionProperty() {
            var relatives = GetPerson(1).Relatives;
            Assert.IsNotNull(relatives, "Failed to access collection");
            Assert.Greater(relatives.Count, 0, "no items in collection");
        }

        public void CollectionPropertyCollectionResolveStateIsPersistent() {
            var relativesAdapter = GetAdaptedRelatives(GetPerson(1));
            Assert.IsTrue(relativesAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsNotNull(relativesAdapter.Oid, "is  null");
            Assert.IsInstanceOf(typeof(IAggregateOid), relativesAdapter.Oid, "is not aggregate");
        }

        public void EmptyCollectionPropertyCollectionResolveStateIsPersistent() {
            var relativesAdapter = GetAdaptedRelatives(GetPerson(2));
            Assert.IsTrue(relativesAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsNotNull(relativesAdapter.Oid, "is  null");
            Assert.IsInstanceOf(typeof(IAggregateOid), relativesAdapter.Oid, "is not aggregate");
        }

        public void ReferencePropertyHasLoadingLoadedCalled() {
            var product = GetProductFromPersonOne();
            Assert.AreEqual(1, product.GetEvents()["Loading"], "loading");
            Assert.AreEqual(1, product.GetEvents()["Loaded"], "loaded");
        }

        public void ReferencePropertyObjectHasContainerInjected() {
            var product = GetProductFromPersonOne();
            Assert.IsTrue(product.HasContainer, "no container injected");
        }

        public void ReferencePropertyObjectHasServiceInjected() {
            var product = GetProductFromPersonOne();
            Assert.IsTrue(product.HasMenuService, "no service injected");
        }

        public void ReferencePropertyObjectResolveStateIsPersistent() {
            var adapter = AdapterFor(GetProductFromPersonOne());
            Assert.IsTrue(adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(adapter.Oid.IsTransient, "is transient");
        }

        public void ReferencePropertyObjectHasVersion() {
            var adapter = AdapterFor(GetProductFromPersonOne());
            Assert.IsNotNull(adapter.Version, "should have version");
        }

        public void CollectionPropertyHasLoadingLoadedCalled() {
            var person = GetPersonFromPersonOneCollection();
            Assert.AreEqual(1, person.GetEvents()["Loading"], "loading");
            Assert.AreEqual(1, person.GetEvents()["Loaded"], "loaded");
        }

        public void CollectionPropertyObjectHasContainerInjected() {
            var person = GetPersonFromPersonOneCollection();
            Assert.IsTrue(person.HasContainer, "no container injected");
        }

        public void CollectionPropertyObjectHasMenuServiceInjected() {
            var person = GetPersonFromPersonOneCollection();
            Assert.IsTrue(person.HasMenuService, "no menu service injected");
        }

        public void CollectionPropertyObjectHasSystemServiceInjected() {
            var person = GetPersonFromPersonOneCollection();
            Assert.IsTrue(person.HasSystemService, "no system service injected");
        }

        public void CollectionPropertyObjectHasContributedServiceInjected() {
            var person = GetPersonFromPersonOneCollection();

            Assert.IsTrue(person.HasContributedActions, "no contributed service injected");
        }

        public void CollectionPropertyObjectResolveStateIsPersistent() {
            var adapter = AdapterFor(GetPersonFromPersonOneCollection());
            Assert.IsTrue(adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(adapter.Oid.IsTransient, "is transient");
        }

        public void CollectionPropertyObjectHasVersion() {
            var adapter = AdapterFor(GetPersonFromPersonOneCollection());
            Assert.IsNotNull(adapter.Version, "should have version");
        }

        public void NewObjectIsCreated() {
            var person = CreateNewTransientPerson();
            Assert.IsNotNull(person);
        }

        public void NewObjectHasCreatedCalled() {
            var person = CreateNewTransientPerson();
            Assert.AreEqual(1, person.GetEvents()["Created"], "created");
        }

        public void NewObjectIsTransient() {
            var adapter = AdapterFor(CreateNewTransientPerson());
            Assert.IsTrue(adapter.ResolveState.IsTransient(), "should be transient");
            Assert.IsTrue(adapter.Oid.IsTransient, "is persistent");
        }

        public void NewObjectHasVersion() {
            var adapter = AdapterFor(CreateNewTransientPerson());
            Assert.IsNotNull(adapter.Version, "should have version");
        }

        public void NewObjectHasContainerInjected() {
            var person = CreateNewTransientPerson();
            Assert.IsTrue(person.HasContainer, "no container injected");
        }

        public void NewObjectHasServiceInjected() {
            var person = CreateNewTransientPerson();
            Assert.IsTrue(person.HasMenuService, "no service injected");
        }

        public void SaveNewObjectWithScalars() {
            var person = CreateNewTransientPerson();
            person.Name = Guid.NewGuid().ToString();
            var adapter = Save(person);
            Assert.IsTrue(adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(adapter.Oid.IsTransient, "is transient");
        }

        public void SaveNewObjectWithValidate() {
            var person = CreateNewTransientPerson();
            person.ChangeName("fail");

            try {
                Save(person);
                Assert.Fail();
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception /*expected*/) { }
        }

        public void ChangeObjectWithValidate() {
            var person = CreateNewTransientPerson();
            person.Name = Guid.NewGuid().ToString();
            person = Save(person).GetDomainObject<Person>();

            try {
                TransactionManager.StartTransaction();
                person.Name = "fail";
                TransactionManager.EndTransaction();
                Assert.Fail();
            }
            catch (PersistFailedException /*expected*/) { }
        }

        public void SaveNewObjectWithTransientReference() {
            var person = CreateNewTransientPerson();
            var product = CreateNewTransientProduct();
            person.Name = Guid.NewGuid().ToString();
            product.Name = Guid.NewGuid().ToString();
            person.FavouriteProduct = product;
            var personAdapter = Save(person);
            // use new person to avoid EF quirk 
            person = personAdapter.GetDomainObject<Person>();

            Assert.IsTrue(personAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(personAdapter.Oid.IsTransient, "is transient");
            var productAdapter = AdapterFor(person.FavouriteProduct);
            Assert.IsTrue(productAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(productAdapter.Oid.IsTransient, "is transient");
        }

        public void SaveNewObjectWithTransientReferenceInvalid() {
            var person = CreateNewTransientPerson();
            var product = CreateNewTransientProduct();
            person.Name = Guid.NewGuid().ToString();

            try {
                // should fail as product name is not set 
                person.FavouriteProduct = product;

                // for EF 
                var adapter = AdapterFor(person);

                if (adapter.ValidToPersist() != null) {
                    throw new PersistFailedException("");
                }

                Assert.Fail();
            }
            catch (PersistFailedException /*expected*/) { }
        }

        public void SaveNewObjectWithTransientReferenceObjectInvalid() {
            var person = CreateNewTransientPerson();
            var pet = CreateNewTransientPet();

            pet.Name = Guid.NewGuid().ToString();
            person.Name = Guid.NewGuid().ToString();

            try {
                // should fail as owner is not set 
                person.Pet = pet;

                // for EF 
                var adapter = AdapterFor(person);

                if (adapter.ValidToPersist() != null) {
                    throw new PersistFailedException("");
                }

                Assert.Fail();
            }
            catch (PersistFailedException /*expected*/) { }
        }

        public void SaveNewObjectWithTransientReferenceValidateAssocInvalid() {
            var person = CreateNewTransientPerson();
            var pet = CreateNewTransientPet();

            pet.Name = Guid.NewGuid().ToString();
            person.Name = "Cruella";

            try {
                pet.Owner = person;
                person.Pet = pet;

                // for EF 
                var adapter = AdapterFor(person);

                if (adapter.ValidToPersist() != null) {
                    throw new PersistFailedException("");
                }

                Assert.Fail();
            }
            catch (PersistFailedException /*expected*/) { }
        }

        public void SaveNewObjectWithPersistentReference() {
            var person = CreateNewTransientPerson();
            var product = GetProduct(2);
            person.Name = Guid.NewGuid().ToString();
            person.FavouriteProduct = product;
            var personAdapter = Save(person);
            Assert.IsTrue(personAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(personAdapter.Oid.IsTransient, "is transient");
            var productAdapter = AdapterFor(product);
            Assert.IsTrue(productAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(productAdapter.Oid.IsTransient, "is transient");
        }

        public void SaveNewObjectWithPersistentReferenceInSeperateTransaction() {
            TransactionManager.StartTransaction();
            var person = CreateNewTransientPerson();
            var product = GetProduct(2);
            person.Name = Guid.NewGuid().ToString();
            person.FavouriteProduct = product;
            TransactionManager.EndTransaction();
            var personAdapter = Save(person);
            Assert.IsTrue(personAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(personAdapter.Oid.IsTransient, "is transient");
            var productAdapter = AdapterFor(product);
            Assert.IsTrue(productAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(productAdapter.Oid.IsTransient, "is transient");
        }

        public void SaveNewObjectWithTransientCollectionItem() {
            var person1 = CreateNewTransientPerson();
            var person2 = CreateNewTransientPerson();
            person1.Name = Guid.NewGuid().ToString();
            person2.Name = Guid.NewGuid().ToString();
            person1.Relatives.Add(person2);
            var person1Adapter = Save(person1);
            // use new person to avoid EF quirk 
            person1 = person1Adapter.GetDomainObject<Person>();
            person2 = person1.Relatives.Single();
            Assert.IsTrue(person1Adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(person1Adapter.Oid.IsTransient, "is transient");
            var person2Adapter = AdapterFor(person2);
            Assert.IsTrue(person2Adapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(person2Adapter.Oid.IsTransient, "is transient");

            var collectionAdapter = ((IObjectSpec) person1Adapter.Spec).GetProperty("Relatives").GetNakedObject(person1Adapter);
            Assert.IsTrue(collectionAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(collectionAdapter.ResolveState.IsGhost(), "should not be ghost");
        }

        public void SaveNewObjectWithPersistentItemCollectionItem() {
            var person1 = CreateNewTransientPerson();
            var person2 = GetPerson(2);
            person1.Name = Guid.NewGuid().ToString();
            person1.Relatives.Add(person2);
            var personAdapter = Save(person1);
            Assert.IsTrue(personAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(personAdapter.Oid.IsTransient, "is transient");
            var productAdapter = AdapterFor(person2);
            Assert.IsTrue(productAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(productAdapter.Oid.IsTransient, "is transient");

            var collectionAdapter = ((IObjectSpec) personAdapter.Spec).GetProperty("Relatives").GetNakedObject(personAdapter);
            Assert.IsTrue(collectionAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(collectionAdapter.ResolveState.IsGhost(), "should not be ghost");
        }

        public void SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() {
            TransactionManager.StartTransaction();
            var person1 = CreateNewTransientPerson();
            var person2 = GetPerson(2);
            person1.Name = Guid.NewGuid().ToString();
            person1.Relatives.Add(person2);
            TransactionManager.EndTransaction();
            var personAdapter = Save(person1);
            Assert.IsTrue(personAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(personAdapter.Oid.IsTransient, "is transient");
            var productAdapter = AdapterFor(person2);
            Assert.IsTrue(productAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(productAdapter.Oid.IsTransient, "is transient");

            var collectionAdapter = ((IObjectSpec) personAdapter.Spec).GetProperty("Relatives").GetNakedObject(personAdapter);
            Assert.IsTrue(collectionAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(collectionAdapter.ResolveState.IsGhost(), "should not be ghost");
        }

        public void SaveNewObjectCallsPersistingPersisted() {
            var person = CreateNewTransientPerson();
            person.Name = Guid.NewGuid().ToString();
            person.UpdateInPersisting();
            var adapter = Save(person);
            Assert.AreEqual(1, person.GetEvents()["Persisting"], "persisting");
            Assert.AreEqual(0, person.GetEvents()["Updating"], "persisting");

            // handle quirk in EF which swaps out object on save 
            // fix this when EF updated
            var personAfter = (Person) adapter.Object;
            Assert.AreEqual(1, personAfter.GetEvents()["Persisted"], "persisted");
            Assert.AreEqual(0, personAfter.GetEvents()["Updated"], "persisted");
        }

        public void SaveNewObjectCallsPersistingPersistedRecursively() {
            Assert.AreEqual(0, Persistor.Instances<Order>().Count());

            var order = CreateNewTransientOrder();
            order.Name = Guid.NewGuid().ToString();
            var adapter = Save(order);
            Assert.AreEqual(1, order.GetEvents()["Persisting"], "persisting");
            Assert.AreEqual(5, Persistor.Instances<Order>().Count());

            Assert.IsTrue(Persistor.Instances<Order>().All(i => i.PersistingCalled));

            // handle quirk in EF which swaps out object on save 
            // fix this when EF updated
            var orderAfter = (Order) adapter.Object;
            Assert.AreEqual(1, orderAfter.GetEvents()["Persisted"], "persisted");
            Assert.AreEqual(1, orderAfter.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, orderAfter.GetEvents()["Updated"], "updated");
        }

        public void SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax() {
            var order = CreateNewTransientOrderFail();
            order.Name = Guid.NewGuid().ToString();

            try {
                Save(order);
                Assert.Fail("Expect exception");
            }
            catch (NakedObjectDomainException e) {
                // expected
                Assert.AreEqual("Max number of commit cycles exceeded. Either increase MaxCommitCycles on installer or identify Updated/Persisted loop. Possible types : TestData.OrderFail", e.Message);
            }
        }

        public void SaveNewObjectCallsPersistingPersistedRecursivelyFails() {
            Assert.AreEqual(0, Persistor.Instances<OrderFail>().Count());

            var order = CreateNewTransientOrderFail();
            order.Name = Guid.NewGuid().ToString();

            try {
                Save(order);
                Assert.Fail("Expect exception");
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) {
                // expected
            }

            Assert.AreEqual(0, Persistor.Instances<OrderFail>().Count());
        }

        public void SaveNewObjectTransientReferenceCallsPersistingPersisted() {
            var person = CreateNewTransientPerson();
            var product = CreateNewTransientProduct();
            person.Name = Guid.NewGuid().ToString();
            product.Name = Guid.NewGuid().ToString();
            person.FavouriteProduct = product;
            var adapter = Save(person);
            Assert.AreEqual(1, product.GetEvents()["Persisting"], "persisting");

            // handle quirk in EF which swaps out object on save 
            // fix this when EF updated
            var personAfter = (Person) adapter.Object;
            var productAfter = personAfter.FavouriteProduct;
            Assert.AreEqual(1, productAfter.GetEvents()["Persisted"], "persisted");
        }

        public void SaveNewObjectTransientCollectionItemCallsPersistingPersisted() {
            var person1 = CreateNewTransientPerson();
            var person2 = CreateNewTransientPerson();
            person1.Name = Guid.NewGuid().ToString();
            person2.Name = Guid.NewGuid().ToString();
            person1.Relatives.Add(person2);
            var adapter = Save(person1);
            Assert.AreEqual(1, person2.GetEvents()["Persisting"], "persisting");

            // handle quirk in EF which swaps out object on save 
            // fix this when EF updated
            var personAfter = (Person) adapter.Object;
            var personAfter1 = personAfter.Relatives.Single();
            Assert.AreEqual(1, personAfter1.GetEvents()["Persisted"], "persisted");
        }

        public void GetInlineInstance() {
            var addressAdapter = GetAdaptedAddress(GetPerson(1));
            Assert.IsTrue(addressAdapter.ResolveState.IsPersistent(), "should be persistent");
            Assert.IsFalse(addressAdapter.Oid.IsTransient, "is transient");
        }

        public void InlineObjectHasContainerInjected() {
            var address = GetPerson(1).Address;
            Assert.IsTrue(address.HasContainer, "no container injected");
        }

        public void InlineObjectHasServiceInjected() {
            var address = GetPerson(1).Address;
            Assert.IsTrue(address.HasMenuService, "no service injected");
        }

        public void InlineObjectHasParentInjected() {
            var address = GetPerson(1).Address;
            Assert.IsTrue(address.HasParent, "no parent injected");
            Assert.IsTrue(address.ParentIsType(typeof(Person)), "parent wrong type");
        }

        public void InlineObjectHasVersion() {
            var addressAdapter = GetAdaptedAddress(GetPerson(1));
            Assert.IsNotNull(addressAdapter.Version, "should have version");
        }

        public void InlineObjectHasLoadingLoadedCalled() {
            TransactionManager.StartTransaction();
            var addressAdapter = GetAdaptedAddress(GetPerson(1));
            var address = addressAdapter.GetDomainObject<Address>();
            TransactionManager.EndTransaction();
            Assert.AreEqual(1, address.GetEvents()["Loading"], "loading");
            Assert.AreEqual(1, address.GetEvents()["Loaded"], "loaded");
        }

        public void CreateTransientInlineInstance() {
            var addressAdapter = GetAdaptedAddress(CreateNewTransientPerson());
            Assert.IsTrue(addressAdapter.ResolveState.IsResolved(), "should be resolved");
            Assert.IsTrue(addressAdapter.Oid.IsTransient, "is persistent");
        }

        public void TransientInlineObjectHasContainerInjected() {
            var address = CreateNewTransientPerson().Address;
            Assert.IsTrue(address.HasContainer, "no container injected");
        }

        public void TransientInlineObjectHasServiceInjected() {
            var address = CreateNewTransientPerson().Address;
            Assert.IsTrue(address.HasMenuService, "no service injected");
        }

        public void TransientInlineObjectHasParentInjected() {
            var address = CreateNewTransientPerson().Address;
            Assert.IsTrue(address.HasParent, "no parent injected");
            Assert.IsTrue(address.ParentIsType(typeof(Person)), "parent wrong type");
        }

        public void TrainsientInlineObjectHasVersion() {
            var addressAdapter = GetAdaptedAddress(CreateNewTransientPerson());
            Assert.IsNotNull(addressAdapter.Version, "should have version");
        }

        public void InlineObjectCallsCreated() {
            var person = CreateNewTransientPerson();
            Assert.AreEqual(1, person.Address.GetEvents()["Created"], "created");
        }

        public void SaveInlineObjectCallsPersistingPersisted() {
            var person = CreateNewTransientPerson();
            person.Address.Line1 = Guid.NewGuid().ToString();
            person.Address.Line2 = Guid.NewGuid().ToString();
            var adapter = Save(person);
            Assert.AreEqual(1, person.GetEvents()["Persisting"], "persisting");

            // handle quirk in EF which swaps out object on save 
            // fix this when EF updated
            var personAfter = (Person) adapter.Object;
            Assert.AreEqual(1, personAfter.GetEvents()["Persisted"], "persisted");
        }

        public void ChangeScalarOnInlineObjectCallsUpdatingUpdated() {
            var address = ChangeScalarOnAddress();
            Assert.AreEqual(1, address.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, address.GetEvents()["Updated"], "updated");
        }

        public void RefreshResetsObject() {
            var person1 = GetPerson(1);
            var name = person1.Name;
            person1.Name = Guid.NewGuid().ToString();
            Persistor.Refresh(AdapterFor(person1));
            Assert.AreEqual(name, person1.Name);
            Assert.AreEqual(1, person1.GetEvents()["Updating"], "updating");
            Assert.AreEqual(1, person1.GetEvents()["Updated"], "updated");
        }

        public void GetKeysReturnsKeys() {
            var person1 = GetPerson(1);
            var key = Persistor.GetKeys(person1.GetType());

            Assert.AreEqual(1, key.Length);
            Assert.AreEqual(person1.GetType().GetProperty("PersonId").Name, key[0].Name);
        }

        public void FindByKey() {
            var person1 = GetPerson(1);
            var person = Persistor.FindByKeys(typeof(Person), new object[] {1}).Object;

            Assert.AreEqual(person1, person);
        }

        public void CreateAndDeleteNewObjectWithScalars() {
            var person = CreateNewTransientPerson();
            var name = Guid.NewGuid().ToString();
            person.Name = name;

            var adapter = Save(person);

            var person1 = Persistor.Instances<Person>().SingleOrDefault(p => p.Name == name);
            Assert.IsNotNull(person1);

            Delete(adapter.Object);

            var person2 = Persistor.Instances<Person>().SingleOrDefault(p => p.Name == name);

            Assert.IsNull(person2);
        }

        public void DeleteObjectCallsDeletingDeleted() {
            var person = CreateNewTransientPerson();
            var name = Guid.NewGuid().ToString();
            person.Name = name;

            var adapter = Save(person);
            person = adapter.GetDomainObject<Person>();

            Delete(person);

            Assert.AreEqual(1, person.GetEvents()["Deleting"], "deleting");
            Assert.AreEqual(1, person.GetEvents()["Deleted"], "deleted");
        }

        public void CountCollectionOnPersistent() {
            var person1 = GetPerson(1);
            var count1 = person1.Relatives.Count;
            var adapter = AdapterFor(person1);
            var count2 = Persistor.CountField(adapter, "Relatives");
            Assert.AreEqual(count1, count2);
        }

        public void CountUnResolvedCollectionOnPersistent() {
            var person1 = GetPerson(1);
            var adapter = AdapterFor(person1);
            var count1 = Persistor.CountField(adapter, "Relatives");
            var count2 = person1.Relatives.Count;
            Assert.AreEqual(count1, count2);
        }

        public void CountEmptyCollectionOnTransient() {
            var person1 = CreateNewTransientPerson();
            var count1 = person1.Relatives.Count;
            var adapter = AdapterFor(person1);
            var count2 = Persistor.CountField(adapter, "Relatives");
            Assert.AreEqual(count1, count2);
        }

        public void CountCollectionOnTransient() {
            var person1 = CreateNewTransientPerson();
            var person4 = GetPerson(4);
            AddToCollectionOnPersonOne(person4);
            var count1 = person1.Relatives.Count;
            var adapter = AdapterFor(person1);
            var count2 = Persistor.CountField(adapter, "Relatives");
            Assert.AreEqual(count1, count2);
        }

        #endregion
    }
}