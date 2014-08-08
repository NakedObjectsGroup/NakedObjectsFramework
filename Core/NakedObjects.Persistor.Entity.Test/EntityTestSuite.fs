// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.EntityTestSuite
open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open NakedObjects.Services
open System
open NakedObjects.EntityObjectStore
open NakedObjects.Core.Context
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Adapter
open System.Collections.Generic
open NakedObjects.Architecture.Util
open NakedObjects.Persistor.TestData
open TestData
open NakedObjects.Persistor.TestSuite
open TestTypes
open TestData
        
let assemblyName = "NakedObjects.Persistor.Test.Data"

#if AV
let datasourceName = "(local)\SQL2012SP1"
#else
let datasourceName = ".\SQLEXPRESS"
#endif

let LoadTestAssembly() = 
    let obj = new Person()
    ()

let Config =    
    let c = new CodeFirstEntityContextConfiguration()
    c.DbContext <- fun () -> upcast new TestDataContext()
    c

//let Config =    
//    LoadTestAssembly()
//    let a = AppDomain.CurrentDomain.GetAssemblies() |> Seq.filter (fun i -> i.GetName().Name = assemblyName) |> Seq.nth 0
//    let c = new CodeOnlyEntityContextConfiguration()
//    c.DefaultMergeOption <- MergeOption.AppendOnly
//    c.DataSource <- datasourceName
//    c.DomainAssemblies <- [|a|]  
//    c

let db = 
    let p = new EntityObjectStore([|(box Config :?> EntityContextConfiguration)|], new EntityOidGenerator(NakedObjectsContext.Reflector), NakedObjectsContext.Reflector)
    p


type TestDataInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<TestDataContext>() 
    override x.Seed ( context : TestDataContext) =

        let newPerson id name product : Person  = 
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
            product.ModifiedDate <- DateTime.Now
            let product = context.Products.Add (product)
            product
      
        let newPet id name person : Pet =
            let pet = new Pet()
            pet.PetId <- id
            pet.Name <- name
            pet.Owner <- person
            let pet = context.Pets.Add (pet)
            pet

        let newOrder id name : Order = 
            let order = new Order()
            order.OrderId <- id
            order.Name <- name
            order
      

        let product1 = newProduct 1 "ProductOne"
        let product2 = newProduct 2 "ProductTwo"
        let product3 = newProduct 3  "ProductThree"
        let product4 = newProduct 4  "ProductFour"

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

        let products = [|product1; product2|]
        let persons = [|person1; person2; person3; person4; person5; person6; person7; person8; person9; person10; person11|]

        person1.Relatives.Add(person2);
        person1.Relatives.Add(person3);
        person1.Relatives.Add(person7);
        person1.Relatives.Add(person8);

        person6.Relatives.Add(person9);
        person7.Relatives.Add(person10);
        person8.Relatives.Add(person11);

        person1.Address.Line1 <- "L1";
        person1.Address.Line2 <- "L2";
        person2.Address.Line1 <- "L1";
        person2.Address.Line2 <- "L2";
        person3.Address.Line1 <- "L1";
        person3.Address.Line2 <- "L2";
        person4.Address.Line1 <- "L1";
        person4.Address.Line2 <- "L2";
      
        products |> Seq.iter (fun x -> x.ResetEvents())
        persons |> Seq.iter (fun x -> x.ResetEvents())
        persons |> Seq.iter (fun x -> x.Address.ResetEvents())
    
[<TestFixture>]
type EntityTestSuite() =
    inherit  NakedObjects.Xat.AcceptanceTestCase()    

    member x.ClearOldTestData() =
       ()
           
    [<TestFixtureSetUp>]
    member x.SetupFixture() = 
         System.Data.Entity.Database.SetInitializer(new TestDataInitializer())
            
    [<SetUp>]
    member x.Setup() =     
        x.InitializeNakedObjectsFramework()
    
    [<TearDown>]
    member x.TearDown() = 
        x.CleanupNakedObjectsFramework()

    override x.Fixtures 
        with get() : IFixturesInstaller =
            box (new FixturesInstaller([|box (new TestDataFixture())|])) :?> IFixturesInstaller
            
    override x.MenuServices 
        with get() : IServicesInstaller  =
            let service = new SimpleRepository<Person>() 
            box (new ServicesInstaller([|(box service)|])) :?> IServicesInstaller
               
    override x.ContributedActions 
        with get() : IServicesInstaller  =
            let service = new SimpleRepository<Product>() 
            box (new ServicesInstaller([|(box service)|])) :?> IServicesInstaller

    override x.SystemServices 
        with get() : IServicesInstaller  =
            let service = new SimpleRepository<Address>() 
            box (new ServicesInstaller([|(box service)|])) :?> IServicesInstaller

    override x.Persistor
        with get() : IObjectPersistorInstaller = 
            let inst = new EntityPersistorInstaller()
            ignore  (inst.UsingCodeFirstContext(Func<Data.Entity.DbContext>(fun () -> upcast new TestDataContext()) ))
            box (inst) :?> IObjectPersistorInstaller
                               
    [<Test>] member x.CanAccessCollectionProperty() =  PersistorTestSuite.CanAccessCollectionProperty()
    [<Test>] member x.GetInstanceFromInstancesOfT() =  PersistorTestSuite.GetInstanceFromInstancesOfT()
    [<Test>] member x.GetInstanceFromInstancesOfType() =  PersistorTestSuite.GetInstanceFromInstancesOfType()
    [<Test>] member x.GetInstanceFromInstancesOfSpecification() =  PersistorTestSuite.GetInstanceFromInstancesOfSpecification()
    [<Test>] member x.GetInstanceIsAlwaysSameObject() =  PersistorTestSuite.GetInstanceIsAlwaysSameObject()
    [<Test>] member x.GetInstanceResolveStateIsPersistent() =  PersistorTestSuite.GetInstanceResolveStateIsPersistent()
    [<Test>] member x.GetInstanceHasVersion() =  PersistorTestSuite.GetInstanceHasVersion()
    [<Test>] member x.ChangeScalarOnPersistentNotifiesUi() =  PersistorTestSuite.ChangeScalarOnPersistentNotifiesUi()
    [<Test>] member x.ChangeScalarOnPersistentCallsUpdatingUpdated() =  PersistorTestSuite.ChangeScalarOnPersistentCallsUpdatingUpdated()
    [<Test>] member x.ChangeReferenceOnPersistentNotifiesUi() =  PersistorTestSuite.ChangeReferenceOnPersistentNotifiesUi()
    [<Test>] member x.ChangeReferenceOnPersistentCallsUpdatingUpdated() =  PersistorTestSuite.ChangeReferenceOnPersistentCallsUpdatingUpdated()
    [<Test>] member x.LoadObjectReturnSameObject() =  PersistorTestSuite.LoadObjectReturnSameObject()   
    [<Test>] member x.PersistentObjectHasContainerInjected() =  PersistorTestSuite.PersistentObjectHasContainerInjected()
    [<Test>] member x.PersistentObjectHasServiceInjected() =  PersistorTestSuite.PersistentObjectHasServiceInjected()
    [<Test>] member x.PersistentObjectHasLoadingLoadedCalled() =  PersistorTestSuite.PersistentObjectHasLoadingLoadedCalled()
    [<Test>] member x.CanAccessReferenceProperty() =  PersistorTestSuite.CanAccessReferenceProperty()
    [<Test>] member x.ReferencePropertyHasLoadingLoadedCalled() =  PersistorTestSuite.ReferencePropertyHasLoadingLoadedCalled()
    [<Test>] member x.ReferencePropertyObjectHasContainerInjected() =  PersistorTestSuite.ReferencePropertyObjectHasContainerInjected()
    [<Test>] member x.ReferencePropertyObjectHasServiceInjected() =  PersistorTestSuite.ReferencePropertyObjectHasServiceInjected()
    [<Test>] member x.ReferencePropertyObjectResolveStateIsPersistent() =  PersistorTestSuite.ReferencePropertyObjectResolveStateIsPersistent()
    [<Test>] member x.ReferencePropertyObjectHasVersion() =  PersistorTestSuite.ReferencePropertyObjectHasVersion()   
    [<Test>] member x.CollectionPropertyHasLoadingLoadedCalled() =  PersistorTestSuite.CollectionPropertyHasLoadingLoadedCalled()
    [<Test>] member x.CollectionPropertyObjectHasContainerInjected() =  PersistorTestSuite.CollectionPropertyObjectHasContainerInjected()
    [<Test>] member x.CollectionPropertyObjectHasMenuServiceInjected() =  PersistorTestSuite.CollectionPropertyObjectHasMenuServiceInjected()
    [<Test>] member x.CollectionPropertyObjectHasContributedServiceInjected() =  PersistorTestSuite.CollectionPropertyObjectHasContributedServiceInjected()
    [<Test>] member x.CollectionPropertyObjectHasSystemServiceInjected() =  PersistorTestSuite.CollectionPropertyObjectHasSystemServiceInjected()
    [<Test>] member x.CollectionPropertyObjectResolveStateIsPersistent() =  PersistorTestSuite.CollectionPropertyObjectResolveStateIsPersistent()
    [<Test>] member x.CollectionPropertyObjectHasVersion() =  PersistorTestSuite.CollectionPropertyObjectHasVersion()
    [<Test>] member x.CollectionPropertyCollectionResolveStateIsPersistent() =  PersistorTestSuite.CollectionPropertyCollectionResolveStateIsPersistent()   
    [<Test>] member x.AddToCollectionOnPersistent() =  PersistorTestSuite.AddToCollectionOnPersistent()
    [<Test>] member x.AddToCollectionOnPersistentCallsUpdatingUpdated() =  PersistorTestSuite.AddToCollectionOnPersistentCallsUpdatingUpdated()
    [<Test>] member x.AddToCollectionOnPersistentNotifiesUi() =  PersistorTestSuite.AddToCollectionOnPersistentNotifiesUi()  
    [<Test>] member x.RemoveFromCollectionOnPersistent() =  PersistorTestSuite.RemoveFromCollectionOnPersistent()
    [<Test>] member x.RemoveFromCollectionOnPersistentCallsUpdatingUpdated() =  PersistorTestSuite.RemoveFromCollectionOnPersistentCallsUpdatingUpdated()
    [<Test>] member x.RemoveFromCollectionOnPersistentNotifiesUi() =  PersistorTestSuite.RemoveFromCollectionOnPersistentNotifiesUi()
    [<Test>] member x.ClearCollectionOnPersistent() =  PersistorTestSuite.ClearCollectionOnPersistent()
    [<Test>] member x.ClearCollectionOnPersistentCallsUpdatingUpdated() =  PersistorTestSuite.ClearCollectionOnPersistentCallsUpdatingUpdated()
    [<Test>] member x.ClearFromCollectionOnPersistentNotifiesUi() =  PersistorTestSuite.ClearCollectionOnPersistentNotifiesUi()
    [<Test>] member x.NewObjectHasContainerInjected() =  PersistorTestSuite.NewObjectHasContainerInjected()
    [<Test>] member x.NewObjectHasCreatedCalled() =  PersistorTestSuite.NewObjectHasCreatedCalled()
    [<Test>] member x.NewObjectHasServiceInjected() =  PersistorTestSuite.NewObjectHasServiceInjected()
    [<Test>] member x.NewObjectHasVersion() =  PersistorTestSuite.NewObjectHasVersion()
    [<Test>] member x.NewObjectIsCreated() =  PersistorTestSuite.NewObjectIsCreated()
    [<Test>] member x.NewObjectIsTransient() =  PersistorTestSuite.NewObjectIsTransient() 
    [<Test>] member x.SaveNewObjectCallsPersistingPersisted() =  PersistorTestSuite.SaveNewObjectCallsPersistingPersisted()
    [<Test>] member x.SaveNewObjectCallsPersistingPersistedRecursively() =  PersistorTestSuite.SaveNewObjectCallsPersistingPersistedRecursively()
    [<Test>] member x.SaveNewObjectCallsPersistingPersistedRecursivelyFails() =  PersistorTestSuite.SaveNewObjectCallsPersistingPersistedRecursivelyFails()

    [<Test>] 
    member x.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax() =  
        EntityObjectStore.MaximumCommitCycles <- 1     
        PersistorTestSuite.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax()
        EntityObjectStore.MaximumCommitCycles <- 10

    [<Test>] member x.SaveNewObjectTransientCollectionItemCallsPersistingPersisted() =  PersistorTestSuite.SaveNewObjectTransientCollectionItemCallsPersistingPersisted()
    [<Test>] member x.SaveNewObjectTransientReferenceCallsPersistingPersisted() =  PersistorTestSuite.SaveNewObjectTransientReferenceCallsPersistingPersisted()
    [<Test>] member x.SaveNewObjectWithPersistentItemCollectionItem() =  PersistorTestSuite.SaveNewObjectWithPersistentItemCollectionItem()
    [<Test>] member x.SaveNewObjectWithPersistentReference() =  PersistorTestSuite.SaveNewObjectWithPersistentReference()
    [<Test>] member x.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() =  PersistorTestSuite.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction()
    [<Test>] member x.SaveNewObjectWithPersistentReferenceInSeperateTransaction() =  PersistorTestSuite.SaveNewObjectWithPersistentReferenceInSeperateTransaction()    
    [<Test>] member x.SaveNewObjectWithScalars() =  PersistorTestSuite.SaveNewObjectWithScalars()
    [<Test>] member x.SaveNewObjectWithValidate() =  PersistorTestSuite.SaveNewObjectWithValidate()
    [<Test>] member x.ChangeObjectWithValidate() =  PersistorTestSuite.ChangeObjectWithValidate()
    [<Test>] member x.SaveNewObjectWithTransientCollectionItem() =  PersistorTestSuite.SaveNewObjectWithTransientCollectionItem()
    [<Test>] member x.SaveNewObjectWithTransientReference() =  PersistorTestSuite.SaveNewObjectWithTransientReference()  
    [<Test>] member x.EmptyCollectionPropertyCollectionResolveStateIsPersistent() =  PersistorTestSuite.EmptyCollectionPropertyCollectionResolveStateIsPersistent()
    [<Test>] member x.GetInlineInstance() =     PersistorTestSuite.GetInlineInstance() 
    [<Test>] member x.InlineObjectHasContainerInjected() =     PersistorTestSuite.InlineObjectHasContainerInjected()        
    [<Test>] member x.InlineObjectHasServiceInjected() =      PersistorTestSuite.InlineObjectHasServiceInjected()        
    [<Test>] member x.InlineObjectHasParentInjected() =     PersistorTestSuite.InlineObjectHasParentInjected()         
    [<Test>] member x.InlineObjectHasVersion() =      PersistorTestSuite.InlineObjectHasVersion()           
    [<Test>] member x.InlineObjectHasLoadingLoadedCalled() =     PersistorTestSuite.InlineObjectHasLoadingLoadedCalled()       
    [<Test>] member x.CreateTransientInlineInstance() =    PersistorTestSuite.CreateTransientInlineInstance()       
    [<Test>] member x.TransientInlineObjectHasContainerInjected() =      PersistorTestSuite.TransientInlineObjectHasContainerInjected()           
    [<Test>] member x.TransientInlineObjectHasServiceInjected() =     PersistorTestSuite.TransientInlineObjectHasServiceInjected()       
    [<Test>] member x.TransientInlineObjectHasParentInjected() =    PersistorTestSuite.TransientInlineObjectHasParentInjected()         
    [<Test>] member x.TransientInlineObjectHasVersion() =    PersistorTestSuite.TrainsientInlineObjectHasVersion()                 
    [<Test>] member x.InlineObjectCallsCreated() =     PersistorTestSuite.InlineObjectCallsCreated()          
    [<Test>] member x.SaveInlineObjectCallsPersistingPersisted() =     PersistorTestSuite.SaveInlineObjectCallsPersistingPersisted() 
    [<Test>] member x.ChangeScalarOnInlineObjectCallsUpdatingUpdated() =    PersistorTestSuite.ChangeScalarOnInlineObjectCallsUpdatingUpdated() 
    [<Test>] member x.UpdateInlineObjectUpdatesUi() =    PersistorTestSuite.UpdateInlineObjectUpdatesUi() 
    [<Test>] member x.RefreshResetsObject() = PersistorTestSuite.RefreshResetsObject()     
    [<Test>] member x.GetKeysReturnsKeys() = PersistorTestSuite.GetKeysReturnsKeys()   
    [<Test>] member x.FindByKey() = PersistorTestSuite.FindByKey()       
    [<Test>] member x.CreateAndDeleteNewObjectWithScalars() =  PersistorTestSuite.CreateAndDeleteNewObjectWithScalars()
    [<Test>] member x.DeleteObjectCallsDeletingDeleted() = PersistorTestSuite.DeleteObjectCallsDeletingDeleted()
    [<Test>] member x.CountCollectionOnPersistent() = PersistorTestSuite.CountCollectionOnPersistent()
    [<Test>] member x.CountUnResolvedCollectionOnPersistent() = PersistorTestSuite.CountUnResolvedCollectionOnPersistent()
    [<Test>] member x.CountCollectionOnTransient() = PersistorTestSuite.CountCollectionOnTransient()
    [<Test>] member x.CountEmptyCollectionOnTransient() = PersistorTestSuite.CountEmptyCollectionOnTransient()
    [<Test>] member x.SaveNewObjectWithTransientReferenceInvalid() = PersistorTestSuite.SaveNewObjectWithTransientReferenceInvalid()
    [<Test>] member x.SaveNewObjectWithTransientReferenceObjectInvalid() = PersistorTestSuite.SaveNewObjectWithTransientReferenceObjectInvalid()
    [<Test>] member x.SaveNewObjectWithTransientReferenceValidateAssocInvalid() = PersistorTestSuite.SaveNewObjectWithTransientReferenceValidateAssocInvalid()


   