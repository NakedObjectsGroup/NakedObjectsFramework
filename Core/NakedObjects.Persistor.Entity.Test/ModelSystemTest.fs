// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.ModelSystemTest

open NUnit.Framework
open SimpleDatabase
open NakedObjects.Services
open NakedObjects.Core.Util
open SystemTestCode
open NakedObjects
open TestCode
open System.Collections.Generic
open NakedObjects.Architecture.Configuration
open NakedObjects.Core.Configuration
open System.Data.Entity.Core.Objects.DataClasses
open System.Linq
open NakedObjects.Architecture.Adapter
open NakedObjects.Persistor.Entity.Configuration
open System
open Microsoft.Extensions.DependencyInjection;


[<TestFixture>]
type ModelSystemTests() = 
    inherit NakedObjects.Xat.AcceptanceTestCase()
    
    override x.Persistor = 
        let config = new EntityObjectStoreConfiguration()
        config.EnforceProxies <- false
                       
        // let cs = "Server=(localdb)\MSSQLLocalDB;Initial Catalog=ModelFirst;Integrated Security=True;"
        let cs = "Data Source=.\SQLEXPRESS;Initial Catalog=ModelFirst;Integrated Security=True;"

        let f = (fun () -> new SimpleDatabaseDbContext(cs) :> Data.Entity.DbContext)
        config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
        config

    override x.Services = [| typeof<SimpleRepository<Person>> |]

    override x.Types = [| typeof<SimpleDatabase.Fruit>; typeof<List<SimpleDatabase.Food>> |]

    override x.Namespaces = [| "SimpleDatabase" |]

    
    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = NakedObjects.Xat.AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = ()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() = NakedObjects.Xat.AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    override x.MenuServices = 
        let service = new SimpleRepository<Person>()
        [| (box service) |]
    
    member x.CreatePerson() = 
        let setter (p : Person) = 
            p.Id <- x.GetNextPersonID()
            p.ComplexProperty.Firstname <- uniqueName()
            p.ComplexProperty.Surname <- uniqueName()
            p.ComplexProperty_1.s1 <- uniqueName()
            p.ComplexProperty_1.s2 <- uniqueName()
        SystemTestCode.CreateAndSetup<Person> setter x.NakedObjectsFramework
    
    member x.GetPersonDomainObject() = 
        let pp : Person [] = box (x.NakedObjectsFramework.Persistor.Instances<Person>().ToArray()) :?> Person []
        pp
        |> Seq.filter (fun p -> p.Id = 1)
        |> Seq.head
    
    member x.GetNextPersonID() = 
        let pp = x.NakedObjectsFramework.Persistor.Instances<Person>()
        (pp
         |> Seq.map (fun i -> i.Id)
         |> Seq.max)
        + 1
    
    [<Test>]
    member x.GetService() = 
        let service = x.NakedObjectsFramework.ServicesManager.GetService("SimpleRepository-Person")
        Assert.IsNotNull(service.Object)
    
    [<Test>]
    member x.GetCollectionDirectly() = 
        let pp = x.NakedObjectsFramework.Persistor.Instances<Person>()
        Assert.Greater(pp |> Seq.length, 0)
    
    [<Test>]
    member x.GetInstanceDirectly() = 
        let sr = x.GetPersonDomainObject()
        IsNotNullAndPersistent sr x.NakedObjectsFramework
    
    [<Test>]
    member x.GetAggregateInstance() = 
        let n = x.GetPersonDomainObject().ComplexProperty
        IsNotNullAndPersistentAggregate n x.NakedObjectsFramework
    
    [<Test>]
    member x.GetTransientAggregateInstance() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        IsNotNullAndTransientAggregate co x.NakedObjectsFramework
    
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
        Assert.AreEqual(0, findValue "Persisted", "persisted")
        let p = pNo.Object :?> Person
        let m2 = p.GetCallbackStatus()
        let findValue = fv m2
        Assert.AreEqual(0, findValue "Persisting", "persisting")
        Assert.AreEqual(1, findValue "Persisted", "persisted")
    
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
        Assert.AreEqual(1, findValue "Persisted", "persisted")
    
   
    [<Test>]
    member x.ComplexTypeObjectCallsLoadingLoaded() = 
        x.NakedObjectsFramework.TransactionManager.StartTransaction()
        let p = x.GetPersonDomainObject()
        let co = p.ComplexProperty
        x.NakedObjectsFramework.TransactionManager.EndTransaction()
        let m = co.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        Assert.AreEqual(1, findValue "Loading", "loading")
        Assert.AreEqual(1, findValue "Loaded", "loaded")
    
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
            (ctx.Persistor.Instances<Fruit>()
             |> Seq.map (fun i -> i.Id)
             |> Seq.max)
            + 1
        
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
        save (SystemTestCode.CreateAndSetup<Person> setter ctx) ctx
    
    [<Test>]
    member x.AddToCollectionNotifiesUI() = 
        let ctx = x.NakedObjectsFramework
        
        let GetNextFruitID() = 
            (ctx.Persistor.Instances<Fruit>()
             |> Seq.map (fun i -> i.Id)
             |> Seq.max)
            + 1
        
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
        let p = noP.Object :?> Person
        ctx.TransactionManager.StartTransaction()
        p.Food.Add(fruit)
        ctx.TransactionManager.EndTransaction()

