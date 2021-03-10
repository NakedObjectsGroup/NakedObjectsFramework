// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.EntityTestSuite

open NakedFramework.Core.Authentication
open NakedObjects.Persistor.TestData
open NakedObjects.Persistor.TestSuite
open NakedFramework.Persistor.Entity.Configuration
open NakedFramework.Persistor.Entity.Component
open NakedObjects.Services
open NakedObjects.TestTypes
open NUnit.Framework
open System
open System.Data.Entity.Core.Objects.DataClasses
open System.Security.Principal
open TestCode
open TestData
open Microsoft.Extensions.Configuration
open NakedFramework.Xat.TestCase
open NakedFramework.DependencyInjection.Extensions

let assemblyName = "NakedFramework.Persistor.Test.Data"

let LoadTestAssembly() = 
    let obj = new Person()
    ()

let Config = 
    let c = new CodeFirstEntityContextConfiguration()
    c.DbContext <- fun () -> upcast new TestDataContext(csTDCO)
    c

let db =
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    //c.ContextConfiguration <- [| Config  |]
    let p = getEntityObjectStore c
    p

type TestDataInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<TestDataContext>()
    override x.Seed(context : TestDataContext) = 
        let newPerson id name product : Person = 
            let person = new Person()
            person.PersonId <- id
            person.Name <- name
            person.FavouriteProduct <- product
            let person = context.People.Add(person)
            person
        
        let newProduct id name : Product = 
            let product = new Product()
            product.Id <- id
            product.Name <- name
            product.ModifiedDate <- DateTime.Now.ToBinary().ToString()
            let product = context.Products.Add(product)
            product
        
        let newPet id name person : Pet = 
            let pet = new Pet()
            pet.PetId <- id
            pet.Name <- name
            pet.Owner <- person
            let pet = context.Pets.Add(pet)
            pet
        
        let newOrder id name : Order = 
            let order = new Order()
            order.OrderId <- id
            order.Name <- name
            order
        
        let product1 = newProduct 1 "ProductOne"
        let product2 = newProduct 2 "ProductTwo"
        let product3 = newProduct 3 "ProductThree"
        let product4 = newProduct 4 "ProductFour"
        let person1 = newPerson 1 "PersonOne" product1
        let person2 = newPerson 2 "PersonTwo" product1
        let person3 = newPerson 3 "PersonThree" product2
        let person4 = newPerson 4 "PersonFour" product2
        let person5 = newPerson 5 "PersonFive" product2
        let person6 = newPerson 6 "PersonSix" product2
        let person7 = newPerson 7 "PersonSeven" product2
        let person8 = newPerson 8 "PersonEight" product2
        let person9 = newPerson 9 "PersonNine" product2
        let person10 = newPerson 10 "PersonTen" product2
        let person11 = newPerson 11 "PersonEleven" product2
        let person12 = newPerson 12 "PersonTwelve" product4
        let person13 = newPerson 13 "PersonThirteen" product4
        let person14 = newPerson 14 "PersonFourteen" product4
        let person15 = newPerson 15 "PersonFifteen" product4
        let person16 = newPerson 16 "PersonSixteen" product4
        let person17 = newPerson 17 "PersonSeventeen" product4
        let person18 = newPerson 18 "PersonEighteen" product4
        let person19 = newPerson 19 "PersonNineteen" product4
        let person20 = newPerson 20 "PersonTwenty" product4
        let person21 = newPerson 21 "PersonTwentyOne" product4
        let person22 = newPerson 22 "PersonTwentyTwo" product4
        let pet1 = newPet 1 "PetOne" person1
        let products = [| product1; product2 |]
        let persons = [| person1; person2; person3; person4; person5; person6; person7; person8; person9; person10; person11 |]
        person1.Relatives.Add(person2)
        person1.Relatives.Add(person3)
        person1.Relatives.Add(person7)
        person1.Relatives.Add(person8)
        person6.Relatives.Add(person9)
        person7.Relatives.Add(person10)
        person8.Relatives.Add(person11)
        person1.Address.Line1 <- "L1"
        person1.Address.Line2 <- "L2"
        person2.Address.Line1 <- "L1"
        person2.Address.Line2 <- "L2"
        person3.Address.Line1 <- "L1"
        person3.Address.Line2 <- "L2"
        person4.Address.Line1 <- "L1"
        person4.Address.Line2 <- "L2"
        products |> Seq.iter (fun x -> x.ResetEvents())
        persons |> Seq.iter (fun x -> x.ResetEvents())
        persons |> Seq.iter (fun x -> x.Address.ResetEvents())

[<TestFixture>]
type EntityTestSuite() = 
    inherit AcceptanceTestCase()

    override x.EnforceProxies = false

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ());

    override x.ContextInstallers = 
        [|  Func<IConfiguration, Data.Entity.DbContext> (fun (c : IConfiguration) -> new TestDataContext(csTDCO) :> Data.Entity.DbContext) |]

    override x.Services = [| typeof<SimpleRepository<Person>>; 
                             typeof<SimpleRepository<Product>>;
                             typeof<SimpleRepository<Address>> |]

    override x.ObjectTypes = [| typeof<TestData.TestHelper>;
                                typeof<TestData.Person>;
                                typeof<TestData.Pet>;
                                typeof<TestData.Address>;
                                typeof<TestData.Order>;
                                typeof<TestData.Product>;
                                typeof<TestData.OrderFail> |]
    
    member x.ClearOldTestData() = ()
    
    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = 
        System.Data.Entity.Database.SetInitializer(new TestDataInitializer())
        AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = x.EndTest()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() = AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    override x.Fixtures = [| box (new TestDataFixture()) |]
     
    member x.Tests = new PersistorTestSuite(x.NakedObjectsFramework)
    
    [<Test>]
    member x.CanAccessCollectionProperty() = x.Tests.CanAccessCollectionProperty()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfT() = x.Tests.GetInstanceFromInstancesOfT()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfType() = x.Tests.GetInstanceFromInstancesOfType()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfSpecification() = x.Tests.GetInstanceFromInstancesOfSpecification()
    
    [<Test>]
    member x.GetInstanceIsAlwaysSameObject() = x.Tests.GetInstanceIsAlwaysSameObject()
    
    [<Test>]
    member x.GetInstanceResolveStateIsPersistent() = x.Tests.GetInstanceResolveStateIsPersistent()
    
    [<Test>]
    member x.GetInstanceHasVersion() = x.Tests.GetInstanceHasVersion()
    
    
    [<Test>]
    member x.ChangeScalarOnPersistentCallsUpdatingUpdated() = x.Tests.ChangeScalarOnPersistentCallsUpdatingUpdated()
     
    [<Test>]
    member x.ChangeReferenceOnPersistentCallsUpdatingUpdated() = x.Tests.ChangeReferenceOnPersistentCallsUpdatingUpdated()
    
    [<Test>]
    member x.UpdatedDoesntCallPersistedAtOnce() = x.Tests.UpdatedDoesntCallPersistedAtOnce()
    
    [<Test>]
    member x.LoadObjectReturnSameObject() = x.Tests.LoadObjectReturnSameObject()
    
    [<Test>]
    member x.PersistentObjectHasContainerInjected() = x.Tests.PersistentObjectHasContainerInjected()
    
    [<Test>]
    member x.PersistentObjectHasServiceInjected() = x.Tests.PersistentObjectHasServiceInjected()
    
    [<Test>]
    member x.PersistentObjectHasLoadingLoadedCalled() = x.Tests.PersistentObjectHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CanAccessReferenceProperty() = x.Tests.CanAccessReferenceProperty()
    
    [<Test>]
    member x.ReferencePropertyHasLoadingLoadedCalled() = x.Tests.ReferencePropertyHasLoadingLoadedCalled()
    
    [<Test>]
    member x.ReferencePropertyObjectHasContainerInjected() = x.Tests.ReferencePropertyObjectHasContainerInjected()
    
    [<Test>]
    member x.ReferencePropertyObjectHasServiceInjected() = x.Tests.ReferencePropertyObjectHasServiceInjected()
    
    [<Test>]
    member x.ReferencePropertyObjectResolveStateIsPersistent() = x.Tests.ReferencePropertyObjectResolveStateIsPersistent()
    
    [<Test>]
    member x.ReferencePropertyObjectHasVersion() = x.Tests.ReferencePropertyObjectHasVersion()
    
    [<Test>]
    member x.CollectionPropertyHasLoadingLoadedCalled() = x.Tests.CollectionPropertyHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CollectionPropertyObjectHasContainerInjected() = x.Tests.CollectionPropertyObjectHasContainerInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasMenuServiceInjected() = x.Tests.CollectionPropertyObjectHasMenuServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasContributedServiceInjected() = x.Tests.CollectionPropertyObjectHasContributedServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasSystemServiceInjected() = x.Tests.CollectionPropertyObjectHasSystemServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectResolveStateIsPersistent() = x.Tests.CollectionPropertyObjectResolveStateIsPersistent()
    
    [<Test>]
    member x.CollectionPropertyObjectHasVersion() = x.Tests.CollectionPropertyObjectHasVersion()
    
    [<Test>]
    member x.CollectionPropertyCollectionResolveStateIsPersistent() = x.Tests.CollectionPropertyCollectionResolveStateIsPersistent()
    
    [<Test>]
    member x.AddToCollectionOnPersistent() = x.Tests.AddToCollectionOnPersistent()
    
    [<Test>]
    member x.AddToCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.AddToCollectionOnPersistentCallsUpdatingUpdated()
    
    [<Test>]
    member x.RemoveFromCollectionOnPersistent() = x.Tests.RemoveFromCollectionOnPersistent()
    
    [<Test>]
    member x.RemoveFromCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.RemoveFromCollectionOnPersistentCallsUpdatingUpdated()
      
    [<Test>]
    member x.ClearCollectionOnPersistent() = x.Tests.ClearCollectionOnPersistent()
    
    [<Test>]
    member x.ClearCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.ClearCollectionOnPersistentCallsUpdatingUpdated()
      
    [<Test>]
    member x.NewObjectHasContainerInjected() = x.Tests.NewObjectHasContainerInjected()
    
    [<Test>]
    member x.NewObjectHasCreatedCalled() = x.Tests.NewObjectHasCreatedCalled()
    
    [<Test>]
    member x.NewObjectHasServiceInjected() = x.Tests.NewObjectHasServiceInjected()
    
    [<Test>]
    member x.NewObjectHasVersion() = x.Tests.NewObjectHasVersion()
    
    [<Test>]
    member x.NewObjectIsCreated() = x.Tests.NewObjectIsCreated()
    
    [<Test>]
    member x.NewObjectIsTransient() = x.Tests.NewObjectIsTransient()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersisted() = x.Tests.SaveNewObjectCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursively() = x.Tests.SaveNewObjectCallsPersistingPersistedRecursively()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursivelyFails() = x.Tests.SaveNewObjectCallsPersistingPersistedRecursivelyFails()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax() = 
        EntityObjectStore.MaximumCommitCycles <- 1
        x.Tests.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax()
        EntityObjectStore.MaximumCommitCycles <- 10
    
    [<Test>]
    member x.SaveNewObjectTransientCollectionItemCallsPersistingPersisted() = x.Tests.SaveNewObjectTransientCollectionItemCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectTransientReferenceCallsPersistingPersisted() = x.Tests.SaveNewObjectTransientReferenceCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentItemCollectionItem() = x.Tests.SaveNewObjectWithPersistentItemCollectionItem()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentReference() = x.Tests.SaveNewObjectWithPersistentReference()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() = x.Tests.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentReferenceInSeperateTransaction() = x.Tests.SaveNewObjectWithPersistentReferenceInSeperateTransaction()
    
    [<Test>]
    member x.SaveNewObjectWithScalars() = x.Tests.SaveNewObjectWithScalars()
    
    // cross validate is done from facade
    //[<Test>]
    //member x.SaveNewObjectWithValidate() = x.Tests.SaveNewObjectWithValidate()
    
    [<Test>]
    member x.ChangeObjectWithValidate() = x.Tests.ChangeObjectWithValidate()
    
    [<Test>]
    member x.SaveNewObjectWithTransientCollectionItem() = x.Tests.SaveNewObjectWithTransientCollectionItem()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReference() = x.Tests.SaveNewObjectWithTransientReference()
    
    [<Test>]
    member x.EmptyCollectionPropertyCollectionResolveStateIsPersistent() = x.Tests.EmptyCollectionPropertyCollectionResolveStateIsPersistent()
    
    [<Test>]
    member x.GetInlineInstance() = x.Tests.GetInlineInstance()
    
    [<Test>]
    member x.InlineObjectHasContainerInjected() = x.Tests.InlineObjectHasContainerInjected()
    
    [<Test>]
    member x.InlineObjectHasServiceInjected() = x.Tests.InlineObjectHasServiceInjected()
    
    [<Test>]
    member x.InlineObjectHasParentInjected() = x.Tests.InlineObjectHasParentInjected()
    
    [<Test>]
    member x.InlineObjectHasVersion() = x.Tests.InlineObjectHasVersion()
    
    [<Test>]
    member x.InlineObjectHasLoadingLoadedCalled() = x.Tests.InlineObjectHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CreateTransientInlineInstance() = x.Tests.CreateTransientInlineInstance()
    
    [<Test>]
    member x.TransientInlineObjectHasContainerInjected() = x.Tests.TransientInlineObjectHasContainerInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasServiceInjected() = x.Tests.TransientInlineObjectHasServiceInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasParentInjected() = x.Tests.TransientInlineObjectHasParentInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasVersion() = x.Tests.TrainsientInlineObjectHasVersion()
    
    [<Test>]
    member x.InlineObjectCallsCreated() = x.Tests.InlineObjectCallsCreated()
    
    [<Test>]
    member x.SaveInlineObjectCallsPersistingPersisted() = x.Tests.SaveInlineObjectCallsPersistingPersisted()
    
    [<Test>]
    member x.ChangeScalarOnInlineObjectCallsUpdatingUpdated() = x.Tests.ChangeScalarOnInlineObjectCallsUpdatingUpdated()
     
    [<Test>]
    member x.RefreshResetsObject() = x.Tests.RefreshResetsObject()
    
    [<Test>]
    member x.GetKeysReturnsKeys() = x.Tests.GetKeysReturnsKeys()
    
    [<Test>]
    member x.FindByKey() = x.Tests.FindByKey()
    
    [<Test>]
    member x.CreateAndDeleteNewObjectWithScalars() = x.Tests.CreateAndDeleteNewObjectWithScalars()
    
    [<Test>]
    member x.DeleteObjectCallsDeletingDeleted() = x.Tests.DeleteObjectCallsDeletingDeleted()
    
    [<Test>]
    member x.CountCollectionOnPersistent() = x.Tests.CountCollectionOnPersistent()
    
    [<Test>]
    member x.CountUnResolvedCollectionOnPersistent() = x.Tests.CountUnResolvedCollectionOnPersistent()
    
    [<Test>]
    member x.CountCollectionOnTransient() = x.Tests.CountCollectionOnTransient()
    
    [<Test>]
    member x.CountEmptyCollectionOnTransient() = x.Tests.CountEmptyCollectionOnTransient()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceInvalid() = x.Tests.SaveNewObjectWithTransientReferenceInvalid()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceObjectInvalid() = x.Tests.SaveNewObjectWithTransientReferenceObjectInvalid()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceValidateAssocInvalid() = x.Tests.SaveNewObjectWithTransientReferenceValidateAssocInvalid()