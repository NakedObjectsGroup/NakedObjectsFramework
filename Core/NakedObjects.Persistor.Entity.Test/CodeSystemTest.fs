// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.CodeSystemTest
open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open TestCodeOnly
open NakedObjects.Services
open System
open NakedObjects.EntityObjectStore
open NakedObjects.Core.Context
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Adapter
open SystemTestCode
open System.Collections.Generic
open CodeOnlyTestCode
open NakedObjects
open TestCode
open NakedObjects.Architecture.Util
open NakedObjects.Architecture.Persist
open Microsoft.Practices.Unity
            
[<TestFixture>]
type CodeSystemTests() =
    inherit  NakedObjects.Xat.AcceptanceTestCase()    

    override x.RegisterTypes(container) = 
        base.RegisterTypes(container)
        let config = new EntityObjectStoreConfiguration()
        let f = (fun () -> new CodeFirstContext("CodeSystemTest") :> Data.Entity.DbContext)
        let ignore = config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) 
        let ignore = container.RegisterInstance(typeof<EntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager()))
        ()
   
    [<TestFixtureSetUpAttribute>]
    member x.SetupFixture() =       
         CodeFirstSetup()
         x.InitializeNakedObjectsFramework()

    [<SetUp>]
    member x.SetupTest() =        
        x.StartTest()
      
    [<TearDown>]
    member x.TearDownTest() =   ()

    [<TestFixtureTearDown>]
    member x.TearDownFixture() = 
        x.CleanupNakedObjectsFramework()

    override x.MenuServices 
        with get() : IServicesInstaller  =
            let service = new SimpleRepository<Person>() 
            box (new ServicesInstaller([|(box service)|])) :?> IServicesInstaller
               
    member x.GetPersonDomainObject() = 
       let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>() 
       pp |>  Seq.head
      
    member x.GetCategoryDomainObject() = 
       let cc = x.NakedObjectsFramework.ObjectPersistor.Instances<Category>()
       cc |>  Seq.head
        
    member x.CreatePerson() = 
       let setter (p : Person) = 
           p.Name <- uniqueName()
       SystemTestCode.CreateAndSetup<Person> setter x.NakedObjectsFramework
     
    member x.CreateProduct() = 
       let setter (pr : Product) = 
           pr.Name <- uniqueName()
       SystemTestCode.CreateAndSetup<Product> setter x.NakedObjectsFramework
                     
    [<Test>]
    member x.GetService() = 
        let service = x.NakedObjectsFramework.ObjectPersistor.GetService("repository#TestCodeOnly.Person")
        Assert.IsNotNull(service.Object)     
               
    [<Test>]
    member x.GetCollectionDirectly() = 
        let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>()
        Assert.Greater(pp |> Seq.length, 0)
              
    [<Test>]
    member x.GetInstanceDirectly() = 
       let p = x.GetPersonDomainObject()
       Assert.IsNotNull(p)
       Assert.AreEqual("Ted", p.Name)
            
    [<Test>]
    member x.CheckIdentitiesAreConsistent() = 
       let ctx = x.NakedObjectsFramework
       let getp sel  =  ctx.ObjectPersistor.Instances<Person>() |> sel
       let (p1, p2, p3, p4) = (getp Seq.head, getp (Seq.skip 1 >> Seq.head), getp Seq.head, getp (Seq.skip 1 >> Seq.head))
       Assert.AreSame(p1, p3)
       Assert.AreSame(p2, p4)
       Assert.AreNotSame(p1, p2)
       Assert.AreNotSame(p2, p3)
                 
    [<Test>]
    member x.CheckResolveStateOfPersistentObject() = 
       let p = x.GetPersonDomainObject()
       IsNotNullAndPersistent p x.NakedObjectsFramework
      
      
    [<Test>]
    member x.CheckResolveStateOfTransientObject() = 
       let p = Create<Person>(x.NakedObjectsFramework)
       IsNotNullAndTransient p x.NakedObjectsFramework
       
    [<Test>]
    member x.CheckResolveStateOfReference() = 
       let p = x.GetPersonDomainObject()
       let pr = p.Favourite
       IsNotNullAndPersistent pr x.NakedObjectsFramework
              
    [<Test>]
    member x.GetCollectionIndirectly() = 
       let c = x.GetCategoryDomainObject()
       Assert.IsNotNull(c)
       Assert.Greater(c.Products |> Seq.length, 0)
           
    [<Test>]
    member x.GetInstanceIndirectly() = 
       let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>() |> System.Linq.Enumerable.ToArray
       let p = System.Linq.Enumerable.Where (pp, (fun (p : Person) -> p.Favourite <> null)) |> Seq.head
       Assert.IsNotNull(p)
       IsNotNullAndPersistent p.Favourite x.NakedObjectsFramework
           
    [<Test>]
    member x.CheckIdentitiesAreConsistentWhenNavigating() = 
       let p = x.GetPersonDomainObject()
       let pr = p.Favourite
       let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>() |> Seq.filter (fun i -> i.Favourite = pr) |> Seq.head
       Assert.AreSame(p, pp)
           
    [<Test>]
    member x.DirectlyLoadedObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        Assert.IsNotNull(p.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
   
    [<Test>]
    member x.LazyLoadedObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        let pr = p.Favourite
        Assert.IsNotNull(pr.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, pr.ExposeContainerForTest())
        
                     
    [<Test>]
    member x.CreateNewObjectWithScalars() =    
       let p = x.CreatePerson()
       save  p x.NakedObjectsFramework
       IsNotNullAndPersistent p x.NakedObjectsFramework
          
    [<Test>]
    member x.CreateNewObjectWithPersistentReference() =         
       let pNo = x.CreatePerson()
       let p = box pNo.Object :?> Person
       let pr = x.NakedObjectsFramework.ObjectPersistor.Instances<Product>() |> Seq.head
       p.Favourite <- pr
       save pNo x.NakedObjectsFramework
       IsNotNullAndPersistent pNo x.NakedObjectsFramework
           
    [<Test>]
    member x.CreateNewObjectWithTransientReference() = 
       let pNo = x.CreatePerson()
       let p = box pNo.Object :?> Person
       let prNo = x.CreateProduct()
       let pr = prNo.Object :?> Product 
       p.Favourite <- pr
       save pNo x.NakedObjectsFramework
       IsNotNullAndPersistent pNo x.NakedObjectsFramework
       IsNotNullAndPersistent prNo x.NakedObjectsFramework
            
    [<Test>]
    member x.CreatedObjectCallsCreated() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let m = p.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
        Assert.AreEqual(1, findValue "Created"   , "created")    
      
    
    [<Test>]
    member x.CreatedObjectHasContainer() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        Assert.IsNotNull(p.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
        save pNo x.NakedObjectsFramework
        let p = pNo.Object :?> Person
        Assert.IsNotNull(p.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
        
    [<Test>]
    member x.UpdateScalarOnPersistentObject() =
        let p1 = x.GetPersonDomainObject()
        let changeName() = 
            let newName = uniqueName()
            p1.Name <-  newName
        makeAndSaveChanges changeName x.NakedObjectsFramework
        let p2 = x.GetPersonDomainObject()
        Assert.AreEqual(p1.Name, p2.Name)
     
    [<Test>]
    member x.UpdateScalarOnPersistentObjectCallsUpdatingUpdated() =
        let p1 = x.GetPersonDomainObject()
        let changeName() =  p1.Name <- uniqueName()
        makeAndSaveChanges changeName x.NakedObjectsFramework    
        let m = p1.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
      
        Assert.AreEqual(1, findValue "Updating"  , "updating")
        Assert.AreEqual(1, findValue "Updated"   , "updated")
        
     
    [<Test>]
    member x.UpdateScalarOnPersistentObjectNotifiesUI() =
        x.NakedObjectsFramework.UpdateNotifier.EnsureEmpty()
        let p1 = x.GetPersonDomainObject()
        let changeName() = 
            let newName = uniqueName()
            p1.Name <-  newName
        makeAndSaveChanges changeName x.NakedObjectsFramework
        let updates =   CollectionUtils.ToEnumerable<INakedObject>(x.NakedObjectsFramework.UpdateNotifier.AllChangedObjects())
        Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box p1))
               
    [<Test>]
    member x.UpdateReferenceOnPersistentObject() =
        let p1 = x.GetPersonDomainObject()
        let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Product>()
        let pr = pp |> Seq.filter (fun i -> p1.Favourite <> i) |> Seq.head     
        let changeFav() = 
            p1.Favourite <- pr
        makeAndSaveChanges changeFav x.NakedObjectsFramework
        let p2 = x.GetPersonDomainObject()
        Assert.AreEqual(p1.Favourite, p2.Favourite)
     
    [<Test>]
    member x.UpdateReferenceOnPersistentObjectNotifiesUI() =
        x.NakedObjectsFramework.UpdateNotifier.EnsureEmpty()
        let p1 = x.GetPersonDomainObject()
        let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Product>()
        let pr = pp |> Seq.toList |> Seq.filter (fun i -> p1.Favourite <> i) |> Seq.head     
        let changeFav() = 
            p1.Favourite <- pr
        makeAndSaveChanges changeFav x.NakedObjectsFramework
        let updates =   CollectionUtils.ToEnumerable<INakedObject>(x.NakedObjectsFramework.UpdateNotifier.AllChangedObjects())
        Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box p1))
                         
    [<Test>]
    member x.UpdateReferenceOnPersistentObjectCallsUpdatingUpdated() =
        let p1 = x.GetPersonDomainObject()
        let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Product>() |> Seq.toList
        let pr = pp |> Seq.filter (fun i -> p1.Favourite <> i) |> Seq.head     
        let changeFav() =  p1.Favourite <- pr
        makeAndSaveChanges changeFav x.NakedObjectsFramework

        let m = p1.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
       
        Assert.AreEqual(1, findValue "Updating"  , "updating")
        Assert.AreEqual(1, findValue "Updated"   , "updated")
      
        
    [<Test>]
    member x.SavingNewObjectCallsPersistingPersisted() =    
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        save pNo x.NakedObjectsFramework
        let m1 = p.GetCallbackStatus()
        let fv (map : IDictionary<string, int>) key = 
            let entry = map |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
        let findValue = fv m1 
       
        Assert.AreEqual(1, findValue "Persisting", "persisting")
        Assert.AreEqual(0, findValue "Persisted" , "persisted")
        
        let p = pNo.Object :?> Person
        let m2 = p.GetCallbackStatus()
        let findValue = fv m2
     
        Assert.AreEqual(0, findValue "Persisting", "persisting")
        Assert.AreEqual(1, findValue "Persisted" , "persisted")

    [<Test>]
    member x.CreateAndRetrieveCountryCode() =
        let ctx = x.NakedObjectsFramework
        let createCC() =
            let setter (cc : CountryCode) = 
                cc.Code <- uniqueName()
                cc.Name <- uniqueName()
            SystemTestCode.CreateAndSetup<CountryCode> setter ctx
        let nocc = createCC() 
        let cc = nocc.GetDomainObject<CountryCode>()
        save nocc x.NakedObjectsFramework
        let cc1 =  x.NakedObjectsFramework.ObjectPersistor.Instances<CountryCode>() |> Seq.head
        Assert.AreEqual(cc.Code, cc1.Code)
        Assert.AreEqual(cc.Name, cc1.Name)
        () 

    [<Test>]
    member x.CountryCodeNameIsRequired() =
        let ctx = x.NakedObjectsFramework
        let createCC() =
            let setter (cc : CountryCode) = 
                cc.Code <- uniqueName()
            SystemTestCode.CreateAndSetup<CountryCode> setter ctx
        let nocc = createCC() 
        try
            save nocc x.NakedObjectsFramework
            Assert.Fail()
        with 
            | expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)       
        () 