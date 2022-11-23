// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.CodeSystemTest

open NakedObjects
open NakedFramework.Core.Util
open NakedObjects.Services
open NUnit.Framework
open System
open System.Collections.Generic
open TestCodeOnly
open Microsoft.Extensions.Configuration
open NakedFramework.Test.TestCase
open NakedFramework.Core.Error
open NakedFramework.DependencyInjection.Extensions
open TestTypes
open SystemTestCode
open TestCode
open CodeOnlyTestCode

[<TestFixture>]
type CodeSystemTests() = 
    inherit AcceptanceTestCase()

    override x.ContextCreators = 
        [|  Func<IConfiguration, Data.Entity.DbContext> (fun (c : IConfiguration) -> new CodeFirstContext(csCS) :> Data.Entity.DbContext) |]
        

    override x.Services =  [| typeof<SimpleRepository<Person>> |]

    override x.AddNakedFunctions = Action<NakedFrameworkOptions> (fun (builder) -> ());

    override x.ObjectTypes = [| typeof<Address>;
                                typeof<Category>;
                                typeof<Person>;
                                typeof<Product>;
                                typeof<AbstractTestCode>;
                                typeof<CountryCode> |]

    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = 
        CodeFirstSetup()
        AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = x.EndTest()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() = AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    member x.GetPersonDomainObject() = 
        let pp = x.NakedFramework.Persistor.Instances<Person>()
        pp |> Seq.head
    
    member x.GetCategoryDomainObject() = 
        let cc = x.NakedFramework.Persistor.Instances<Category>()
        cc |> Seq.head
    
    member x.CreatePerson() = 
        let setter (p : Person) = p.Name <- uniqueName()
        SystemTestCode.CreateAndSetup<Person> setter x.NakedFramework
    
    member x.CreateProduct() = 
        let setter (pr : Product) = pr.Name <- uniqueName()
        SystemTestCode.CreateAndSetup<Product> setter x.NakedFramework
    
    [<Test>]
    member x.GetService() = 
        let service = x.NakedFramework.ServicesManager.GetService("SimpleRepository-Person")
        Assert.IsNotNull(service.Object)
    
    [<Test>]
    member x.GetCollectionDirectly() = 
        let pp = x.NakedFramework.Persistor.Instances<Person>()
        Assert.Greater(pp |> Seq.length, 0)
    
    [<Test>]
    member x.GetInstanceDirectly() = 
        let p = x.GetPersonDomainObject()
        Assert.IsNotNull(p)
        Assert.AreEqual("Ted", p.Name)
    
    [<Test>]
    member x.CheckIdentitiesAreConsistent() = 
        let ctx = x.NakedFramework
        let getp sel = ctx.Persistor.Instances<Person>() |> sel
        let (p1, p2, p3, p4) = (getp Seq.head, getp (Seq.skip 1 >> Seq.head), getp Seq.head, getp (Seq.skip 1 >> Seq.head))
        Assert.AreSame(p1, p3)
        Assert.AreSame(p2, p4)
        Assert.AreNotSame(p1, p2)
        Assert.AreNotSame(p2, p3)
    
    [<Test>]
    member x.CheckResolveStateOfPersistentObject() = 
        let p = x.GetPersonDomainObject()
        IsNotNullAndPersistent p x.NakedFramework
    
    [<Test>]
    member x.CheckResolveStateOfTransientObject() = 
        let p = Create<Person>(x.NakedFramework)
        IsNotNullAndTransient p x.NakedFramework
    
    [<Test>]
    member x.CheckResolveStateOfReference() = 
        let p = x.GetPersonDomainObject()
        let pr = p.Favourite
        IsNotNullAndPersistent pr x.NakedFramework
    
    [<Test>]
    member x.GetCollectionIndirectly() = 
        let c = x.GetCategoryDomainObject()
        Assert.IsNotNull(c)
        Assert.Greater(c.Products |> Seq.length, 0)
    
    [<Test>]
    member x.GetInstanceIndirectly() = 
        let pp = x.NakedFramework.Persistor.Instances<Person>() |> System.Linq.Enumerable.ToArray
        let p = System.Linq.Enumerable.Where(pp, (fun (p : Person) -> p.Favourite <> null)) |> Seq.head
        Assert.IsNotNull(p)
        IsNotNullAndPersistent p.Favourite x.NakedFramework
    
    [<Test>]
    member x.CheckIdentitiesAreConsistentWhenNavigating() = 
        let p = x.GetPersonDomainObject()
        let pr = p.Favourite
        
        let pp = 
            x.NakedFramework.Persistor.Instances<Person>()
            |> Seq.filter (fun i -> i.Favourite = pr)
            |> Seq.head
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
        save p x.NakedFramework
        IsNotNullAndPersistent p x.NakedFramework
    
    [<Test>]
    member x.CreateNewObjectWithPersistentReference() = 
        let pNo = x.CreatePerson()
        let p = box pNo.Object :?> Person
        let pr = x.NakedFramework.Persistor.Instances<Product>() |> Seq.head
        p.Favourite <- pr
        save pNo x.NakedFramework
        IsNotNullAndPersistent pNo x.NakedFramework
    
    [<Test>]
    member x.CreateNewObjectWithTransientReference() = 
        let pNo = x.CreatePerson()
        let p = box pNo.Object :?> Person
        let prNo = x.CreateProduct()
        let pr = prNo.Object :?> Product
        p.Favourite <- pr
        save pNo x.NakedFramework
        IsNotNullAndPersistent pNo x.NakedFramework
        IsNotNullAndPersistent prNo x.NakedFramework
    
    [<Test>]
    member x.CreatedObjectCallsCreated() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        let m = p.GetCallbackStatus()
        
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
        save pNo x.NakedFramework
        let p = pNo.Object :?> Person
        Assert.IsNotNull(p.ExposeContainerForTest())
        Assert.IsInstanceOf(typeof<IDomainObjectContainer>, p.ExposeContainerForTest())
    
    [<Test>]
    member x.UpdateScalarOnPersistentObject() = 
        let p1 = x.GetPersonDomainObject()
        
        let changeName() = 
            let newName = uniqueName()
            p1.Name <- newName
        makeAndSaveChanges changeName x.NakedFramework
        let p2 = x.GetPersonDomainObject()
        Assert.AreEqual(p1.Name, p2.Name)
    
    [<Test>]
    member x.UpdateScalarOnPersistentObjectCallsUpdatingUpdated() = 
        let p1 = x.GetPersonDomainObject()
        let changeName() = p1.Name <- uniqueName()
        makeAndSaveChanges changeName x.NakedFramework
        let m = p1.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        Assert.AreEqual(1, findValue "Updating", "updating")
        Assert.AreEqual(1, findValue "Updated", "updated")
    
    [<Test>]
    member x.UpdateReferenceOnPersistentObject() = 
        let p1 = x.GetPersonDomainObject()
        let pp = x.NakedFramework.Persistor.Instances<Product>()
        
        let pr = 
            pp
            |> Seq.filter (fun i -> p1.Favourite <> i)
            |> Seq.head
        
        let changeFav() = p1.Favourite <- pr
        makeAndSaveChanges changeFav x.NakedFramework
        let p2 = x.GetPersonDomainObject()
        Assert.AreEqual(p1.Favourite, p2.Favourite)
  
    [<Test>]
    member x.UpdateReferenceOnPersistentObjectCallsUpdatingUpdated() = 
        let p1 = x.GetPersonDomainObject()
        let pp = x.NakedFramework.Persistor.Instances<Product>() |> Seq.toList
        
        let pr = 
            pp
            |> Seq.filter (fun i -> p1.Favourite <> i)
            |> Seq.head
        
        let changeFav() = p1.Favourite <- pr
        makeAndSaveChanges changeFav x.NakedFramework
        let m = p1.GetCallbackStatus()
        
        let findValue key = 
            let entry = m |> Seq.find (fun kvp -> kvp.Key = key)
            entry.Value
        Assert.AreEqual(1, findValue "Updating", "updating")
        Assert.AreEqual(1, findValue "Updated", "updated")
    
    [<Test>]
    member x.SavingNewObjectCallsPersistingPersisted() = 
        let pNo = x.CreatePerson()
        let p = pNo.Object :?> Person
        save pNo x.NakedFramework
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
    member x.CreateAndRetrieveCountryCode() = 
        let ctx = x.NakedFramework
        
        let createCC() = 
            let setter (cc : CountryCode) = 
                cc.Code <- uniqueName()
                cc.Name <- uniqueName()
            SystemTestCode.CreateAndSetup<CountryCode> setter ctx
        
        let nocc = createCC()
        let cc = nocc.GetDomainObject<CountryCode>()
        save nocc x.NakedFramework
        let cc1 = x.NakedFramework.Persistor.Instances<CountryCode>() |> Seq.head
        Assert.AreEqual(cc.Code, cc1.Code)
        Assert.AreEqual(cc.Name, cc1.Name)
        ()
    
    [<Test>]
    member x.CountryCodeNameIsRequired() = 
        let ctx = x.NakedFramework
        
        let createCC() = 
            let setter (cc : CountryCode) = cc.Code <- uniqueName()
            SystemTestCode.CreateAndSetup<CountryCode> setter ctx
        
        let nocc = createCC()
        try 
            save nocc x.NakedFramework
            Assert.Fail()
        with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
        () 