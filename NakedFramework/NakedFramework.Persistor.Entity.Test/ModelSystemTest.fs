﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.ModelSystemTest

open NakedObjects
open NakedObjects.Services
open NUnit.Framework
open SimpleDatabase
open System
open System.Collections.Generic
open System.Linq
open Microsoft.Extensions.Configuration
open NakedFramework.Test.TestCase
open NakedFramework.DependencyInjection.Extensions
open TestTypes
open SystemTestCode
open TestCode
open NUnit.Framework.Legacy

[<TestFixture>]
type ModelSystemTests() = 
    inherit AcceptanceTestCase()

    override x.AddNakedFunctions = Action<NakedFrameworkOptions> (fun (builder) -> ());

    override x.EnforceProxies = false

    override x.ContextCreators = 
        [|  Func<IConfiguration, Data.Entity.DbContext> (fun (c : IConfiguration) -> new SimpleDatabaseDbContext(csMF) :> Data.Entity.DbContext) |]


    override x.Services = [| typeof<SimpleRepository<Person>> |]

    override x.ObjectTypes = [| typeof<SimpleDatabase.Food>;
                                typeof<SimpleDatabase.Fruit>;
                                typeof<SimpleDatabase.Person>;
                                typeof<SimpleDatabase.NameType>;
                                typeof<SimpleDatabase.ComplexType1>;
                                typeof<SimpleDatabase.AbstractTestCode>  |]
  
    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = ()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() = AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    member x.CreatePerson() = 
        let setter (p : Person) = 
            p.Id <- x.GetNextPersonID()
            p.ComplexProperty.Firstname <- uniqueName()
            p.ComplexProperty.Surname <- uniqueName()
            p.ComplexProperty_1.s1 <- uniqueName()
            p.ComplexProperty_1.s2 <- uniqueName()
        SystemTestCode.CreateAndSetup<Person> setter x.NakedFramework
    
    member x.GetPersonDomainObject() = 
        let pp : Person [] = box (x.NakedFramework.Persistor.Instances<Person>().ToArray()) :?> Person []
        pp
        |> Seq.filter (fun p -> p.Id = 1)
        |> Seq.head
    
    member x.GetNextPersonID() = 
        let pp = x.NakedFramework.Persistor.Instances<Person>()
        (pp
         |> Seq.map (fun i -> i.Id)
         |> Seq.max)
        + 1
    
    [<Test>]
    member x.GetService() = 
        let service = x.NakedFramework.ServicesManager.GetService("SimpleRepository-Person")
        ClassicAssert.IsNotNull(service.Object)
    
    [<Test>]
    member x.GetCollectionDirectly() = 
        let pp = x.NakedFramework.Persistor.Instances<Person>()
        ClassicAssert.Greater(pp |> Seq.length, 0)
    
    [<Test>]
    member x.GetInstanceDirectly() = 
        let sr = x.GetPersonDomainObject()
        IsNotNullAndPersistent sr x.NakedFramework
    
    [<Test>]
    member x.GetAggregateInstance() = 
        let n = x.GetPersonDomainObject().ComplexProperty
        IsNotNullAndPersistentAggregate n x.NakedFramework
    
    [<Test>]
    member x.GetTransientAggregateInstance() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        IsNotNullAndTransientAggregate co x.NakedFramework
    
    [<Test>]
    member x.DirectlyLoadedObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        ClassicAssert.IsNotNull(p.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
    
    [<Test>]
    member x.ComplexObjectHasContainer() = 
        let p = x.GetPersonDomainObject()
        let pr = p.ComplexProperty
        ClassicAssert.IsNotNull(pr.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, pr.ExposeContainerForTest())
    
    [<Test>]
    member x.ComplexObjectHasParent() = 
        let p = x.GetPersonDomainObject()
        let pr = p.ComplexProperty
        ClassicAssert.IsNotNull(pr.Parent)
        ClassicAssert.IsInstanceOf(typeof<Person>, pr.Parent)
    
    [<Test>]
    member x.CreateNewObjectWithComplexType() = 
        let pNo = x.CreatePerson()
        save pNo x.NakedFramework
        IsNotNullAndPersistent pNo x.NakedFramework
        let p = pNo.Object :?> Person
        IsNotNullAndPersistentAggregate p.ComplexProperty x.NakedFramework
        IsNotNullAndPersistentAggregate p.ComplexProperty_1 x.NakedFramework
    
    [<Test>]
    member x.CreatedObjectCallsCreatedPersistingPersisted() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        save pNo x.NakedFramework
        let m1 = p.GetCallbackStatus()
        
        let fv (map : IDictionary<string, int>) key = 
            let entry = map |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        
        let findValue = fv m1
        ClassicAssert.AreEqual(1, findValue "Persisting", "persisting")
        ClassicAssert.AreEqual(0, findValue "Persisted", "persisted")
        let p = pNo.Object :?> Person
        let m2 = p.GetCallbackStatus()
        let findValue = fv m2
        ClassicAssert.AreEqual(0, findValue "Persisting", "persisting")
        ClassicAssert.AreEqual(1, findValue "Persisted", "persisted")
    
    [<Test>]
    member x.ComplexTypeObjectCallsCreated() = 
        let pNo = x.CreatePerson()
        save pNo x.NakedFramework
        let p = pNo.Object :?> Person
        let m = p.ComplexProperty.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        ClassicAssert.AreEqual(1, findValue "Created", "created")
    
    [<Test>]
    member x.CreatedObjectHasContainer() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        ClassicAssert.IsNotNull(p.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
        save pNo x.NakedFramework
        let p = pNo.Object :?> Person
        ClassicAssert.IsNotNull(p.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
    
    [<Test>]
    member x.CreatedComplexObjectHasContainer() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        ClassicAssert.IsNotNull(co.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, co.ExposeContainerForTest())
        save pNo x.NakedFramework
        let co = p.ComplexProperty
        ClassicAssert.IsNotNull(co.ExposeContainerForTest())
        ClassicAssert.IsInstanceOf(typeof<IDomainObjectContainer>, co.ExposeContainerForTest())
    
    [<Test>]
    member x.CreatedComplexObjectHasParent() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let co = p.ComplexProperty
        ClassicAssert.IsNotNull(co.Parent)
        ClassicAssert.IsInstanceOf(typeof<Person>, co.Parent)
        save pNo x.NakedFramework
        let co = p.ComplexProperty
        ClassicAssert.IsNotNull(co.Parent)
        ClassicAssert.IsInstanceOf(typeof<Person>, co.Parent)
    
    [<Test>]
    member x.ComplexTypeObjectCallsPersistingPersisted() = 
        let pNo = x.CreatePerson()
        save pNo x.NakedFramework
        let p = pNo.Object :?> Person
        let m = p.ComplexProperty.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        ClassicAssert.AreEqual(1, findValue "Persisting", "persisting")
        ClassicAssert.AreEqual(1, findValue "Persisted", "persisted")
    
   
    [<Test>]
    member x.ComplexTypeObjectCallsLoadingLoaded() = 
        x.NakedFramework.TransactionManager.StartTransaction()
        let p = x.GetPersonDomainObject()
        let co = p.ComplexProperty
        x.NakedFramework.TransactionManager.EndTransaction()
        let m = co.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        ClassicAssert.AreEqual(1, findValue "Loading", "loading")
        ClassicAssert.AreEqual(1, findValue "Loaded", "loaded")
    
    [<Test>]
    member x.LazyIntrospectionOfInheritedType() = 
        // this test is to cover bug #937
        let p = x.GetPersonDomainObject()
        let f = p.Food |> Seq.head
        ClassicAssert.IsInstanceOf(typeof<Fruit>, f)
    
    [<Test>]
    member x.SavePersonWithInheritedTypeProperty() = 
        let ctx = x.NakedFramework
        
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
        let ctx = x.NakedFramework
        
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

