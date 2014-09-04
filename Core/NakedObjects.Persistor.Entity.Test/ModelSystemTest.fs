// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.ModelSystemTest
open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open ModelFirst
open NakedObjects.Services
open System
open NakedObjects.EntityObjectStore
open NakedObjects.Core.Context
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Adapter 
open SystemTestCode
open NakedObjects
open TestCode
open System.Collections.Generic
open NakedObjects.Architecture.Util
open Microsoft.Practices.Unity


[<TestFixture>]
type ModelSystemTests() =
    inherit  NakedObjects.Xat.AcceptanceTestCase()    

    override x.RegisterTypes(container) = 
        base.RegisterTypes(container)
        let config = new EntityObjectStoreConfiguration()
        config.EnforceProxies <- false
        config.ForceContextSet()
        let ignore = container.RegisterInstance(typeof<EntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager()))
        ()
    
    [<TestFixtureSetUpAttribute>]
    member x.SetupFixture() = x.InitializeNakedObjectsFramework()
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = ()
    
    [<TestFixtureTearDown>]
    member x.TearDownFixture() = x.CleanupNakedObjectsFramework()

    override x.MenuServices = 
        let service = new SimpleRepository<Person>()
        box (new ServicesInstaller([| (box service) |])) :?> IServicesInstaller 
        
   
    member x.CreatePerson() = 
        let setter (p : Person) = 
            p.Id <- x.GetNextPersonID()
            p.ComplexProperty.Firstname <- uniqueName()
            p.ComplexProperty.Surname <- uniqueName()
            p.ComplexProperty_1.s1 <- uniqueName()
            p.ComplexProperty_1.s2 <- uniqueName()
        SystemTestCode.CreateAndSetup<Person> setter x.NakedObjectsFramework
      
    member x.GetPersonDomainObject() = 
       let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>()
       pp |> Seq.filter (fun p -> p.Id = 1) |> Seq.head     
          
    member x.GetNextPersonID() = 
       let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>()
       (pp |> Seq.map (fun i -> i.Id) |> Seq.max)   + 1 
            
    [<Test>]
    member x.GetService() = 
        let service = x.NakedObjectsFramework.ObjectPersistor.GetService("repository#ModelFirst.Person")
        Assert.IsNotNull(service.Object)     
        
    [<Test>]
    member x.GetCollectionDirectly() = 
        let pp = x.NakedObjectsFramework.ObjectPersistor.Instances<Person>()
        Assert.Greater(pp |> Seq.length , 0)   
                         
    [<Test>]
    member x.GetInstanceDirectly() = 
       let sr = x.GetPersonDomainObject()
       IsNotNullAndPersistent sr
                  
    [<Test>]
    member x.GetAggregateInstance() = 
       let n = x.GetPersonDomainObject().ComplexProperty
       IsNotNullAndPersistentAggregate n
    
    [<Test>]
    member x.GetTransientAggregateInstance() = 
       let pNo = x.CreatePerson()
       let p = pNo.Object :?> Person
       let co = p.ComplexProperty
       IsNotNullAndTransientAggregate co
           
    [<Test>]
    member x.DirectlyLoadedObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        Assert.IsNotNull(p.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
   
    [<Test>]
    member x.ComplexObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        let pr = p.ComplexProperty
        Assert.IsNotNull(pr.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, pr.ExposeContainerForTest())
      
    [<Test>]
    member x.ComplexObjectHasParent() = 
        let p = x.GetPersonDomainObject()
        let pr = p.ComplexProperty
        Assert.IsNotNull(pr.Parent)
        Assert.IsInstanceOf(typeof<Person>, pr.Parent)  
       
                                     
    [<Test>]
    member x.CreateNewObjectWithComplexType() =    
       let pNo = x.CreatePerson()
       save pNo x.NakedObjectsFramework
       IsNotNullAndPersistent pNo x.NakedObjectsFramework
       let p = pNo.Object :?> Person
       IsNotNullAndPersistentAggregate p.ComplexProperty x.NakedObjectsFramework
       IsNotNullAndPersistentAggregate p.ComplexProperty_1 x.NakedObjectsFramework   
        
    [<Test>]
    member x.CreatedObjectCallsCreatedPersistingPersisted() = 
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
    member x.ComplexTypeObjectCallsCreated() = 
        let pNo = x.CreatePerson()
        save pNo x.NakedObjectsFramework
        let p = pNo.Object :?> Person
        let m = p.ComplexProperty.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value  
        Assert.AreEqual(1, findValue "Created", "created")    
       
             
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
    member x.CreatedComplexObjectHasContainer() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        Assert.IsNotNull(co.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, co.ExposeContainerForTest())
        save pNo x.NakedObjectsFramework
        let co = p.ComplexProperty
        Assert.IsNotNull(co.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, co.ExposeContainerForTest())
        
    [<Test>]
    member x.CreatedComplexObjectHasParent() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        Assert.IsNotNull(co.Parent)
        Assert.IsInstanceOf(typeof<Person>, co.Parent)
        save pNo x.NakedObjectsFramework
        let co = p.ComplexProperty
        Assert.IsNotNull(co.Parent)
        Assert.IsInstanceOf(typeof<Person>, co.Parent)
          
    [<Test>]
    member x.ComplexTypeObjectCallsPersistingPersisted() = 
        let pNo = x.CreatePerson()
        save pNo x.NakedObjectsFramework
        let p = pNo.Object :?> Person
        let m = p.ComplexProperty.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
     
        Assert.AreEqual(1, findValue "Persisting", "persisting")
        Assert.AreEqual(1, findValue "Persisted" , "persisted")
     
    [<Test>]
    member x.UpdateComplexTypeUpdatesUI() = 
         x.NakedObjectsFramework.ObjectPersistor.StartTransaction()
         let p = x.GetPersonDomainObject()
         let co = p.ComplexProperty
         co.Firstname <- uniqueName()
         x.NakedObjectsFramework.UpdateNotifier.EnsureEmpty()
         x.NakedObjectsFramework.ObjectPersistor.EndTransaction()
         let updates =  CollectionUtils.ToEnumerable<INakedObject>(x.NakedObjectsFramework.UpdateNotifier.AllChangedObjects())
         Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box co), "complex object")
     
    [<Test>]
    member x.ComplexTypeObjectCallsLoadingLoaded() = 
        x.NakedObjectsFramework.ObjectPersistor.StartTransaction();
        let p = x.GetPersonDomainObject()
        let co = p.ComplexProperty
        x.NakedObjectsFramework.ObjectPersistor.EndTransaction();
        let m = co.GetCallbackStatus()
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
     
        Assert.AreEqual(1, findValue "Loading", "loading")
        Assert.AreEqual(1, findValue "Loaded" , "loaded")
     
    [<Test>]
    member x.LazyIntrospectionOfInheritedType() =
        // this test is to cover bug #937
        let p = x.GetPersonDomainObject()
        let f = p.Food |> Seq.head
        Assert.IsInstanceOf(typeof<Fruit>, f)
        
    [<Test>]
    member x.SavePersonWithInheritedTypeProperty() =
       let ctx = x.NakedObjectsFramework
       let GetNextFruitID() = 
            (ctx.ObjectPersistor.Instances<Fruit>() |> Seq.map (fun i -> i.Id) |> Seq.max) + 1

       let fSet (f : Fruit) =
          f.Id <- GetNextFruitID()
          f.Name <- uniqueName()
          f.Organic <- true
         
       let fruit = (SystemTestCode.CreateAndSetup<Fruit> fSet ctx).Object :?> Food
    
       let setter (p : Person) =
           p.Id <- x.GetNextPersonID() 
           p.ComplexProperty.Firstname <- uniqueName()
           p.ComplexProperty.Surname <- uniqueName()
           p.ComplexProperty_1.s1 <- uniqueName()
           p.ComplexProperty_1.s2 <- uniqueName()
           p.Food.Add fruit
           //fruit.Person <- p     
       save (SystemTestCode.CreateAndSetup<Person> setter ctx) ctx

    [<Test>]
    member x.AddToCollectionNotifiesUI() =
       let ctx = x.NakedObjectsFramework
       let GetNextFruitID() = 
            (ctx.ObjectPersistor.Instances<Fruit>() |> Seq.map (fun i -> i.Id) |> Seq.max) + 1

       let fSet (f : Fruit) =
          f.Id <- GetNextFruitID()
          f.Name <- uniqueName()
          f.Organic <- true
         
       let fruit = (SystemTestCode.CreateAndSetup<Fruit> fSet ctx).Object :?> Food
       
       let setter (p : Person) =
           p.Id <- x.GetNextPersonID() 
           p.ComplexProperty.Firstname <- uniqueName()
           p.ComplexProperty.Surname <- uniqueName()
           p.ComplexProperty_1.s1 <- uniqueName()
           p.ComplexProperty_1.s2 <- uniqueName()    
       let noP = SystemTestCode.CreateAndSetup<Person> setter ctx
       save noP ctx
       let p =  noP.Object :?> Person
       ctx.UpdateNotifier.EnsureEmpty()
       ctx.ObjectPersistor.StartTransaction()
       p.Food.Add(fruit)
       ctx.ObjectPersistor.EndTransaction()

       let updates =  CollectionUtils.ToEnumerable<INakedObject>(ctx.UpdateNotifier.AllChangedObjects())

       Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box p), "Person")
