// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.CodeOnlyTestCode


open NakedFramework.Persistor.Entity.Configuration
open NakedFramework.Persistor.Entity.Util
open NakedFramework.Core
open NakedFramework.Persistor.Entity.Component
open NUnit.Framework
open System
open System.Data.Entity.ModelConfiguration
open TestCode
open TestCodeOnly
open TestTypes
open NakedFramework.Core.Error
open NakedFramework.Architecture.Component

let categorySetter codeOnlyPersistor (c : Category) = 
    c.ID <- GetNextID<Category> codeOnlyPersistor (fun i -> i.ID)
    c.Name <- uniqueName()

let productSetter codeOnlyPersistor (pr : Product) = 
    pr.ID <- GetNextID<Product> codeOnlyPersistor (fun i -> i.ID)
    pr.Name <- uniqueName()

let createProductWithID (pr : Product) id codeOnlyPersistor = 
    pr.ID <- id
    pr.Name <- uniqueName()
    CreateAndEndTransaction codeOnlyPersistor pr

let CodeFirstLoadTestAssembly() = 
    let obj = new Category()
    ()

let assemblyName = "NakedObjects.Persistor.Entity.Test.CodeOnly"

type TestConfigClass() as x = 
    inherit EntityTypeConfiguration<CountryCode>()
    do 
        let qexpr = <@ Func<CountryCode, string>(fun (cc : CountryCode) -> cc.Code) @>
        let expr = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToLambdaExpression(qexpr)
        let sink = x.HasKey(expr)
        let qexpr = <@ Func<CountryCode, string>(fun (cc : CountryCode) -> cc.Name) @>
        let expr = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToLambdaExpression(qexpr)
        let sink = x.Property(expr).IsRequired()
        ()

let seedCodeFirstDatabase (context : CodeFirstContext) = 
    let bovril = new Product()
    bovril.Name <- "Bovril"
    let marmite = new Product()
    marmite.Name <- "Marmite"
    let vegemite = new Product()
    vegemite.Name <- "Vegemite"
    let bovril = context.Products.Add(bovril)
    let marmite = context.Products.Add(marmite)
    let vegemite = context.Products.Add(vegemite)
    let food = new Category()
    food.Name <- "Food"
    food.Products.Add(bovril)
    food.Products.Add(marmite)
    food.Products.Add(vegemite)
    let food = context.Categories.Add(food)
    let address1 = new DomesticAddress()
    let address2 = new DomesticAddress()
    let address3 = new InternationalAddress()
    address1.Lines <- "22 Westleigh Drive"
    address2.Lines <- "BNR Park, Concorde Road"
    address3.Lines <- "1 Madison Avenue, New York"
    address1.Postcode <- "RG4 9LB"
    address2.Postcode <- "SL6 4AG"
    address3.Country <- "USA"
    let address1 = context.Addresses.Add(address1)
    let address2 = context.Addresses.Add(address2)
    let address3 = context.Addresses.Add(address3)
    let ted = new Person()
    ted.Name <- "Ted"
    ted.Favourite <- bovril
    ted.Address <- address1
    let ted = context.People.Add(ted)
    let bob = new Person()
    bob.Name <- "Bob"
    bob.Favourite <- marmite
    bob.Address <- address2
    let bob = context.People.Add(bob)
    let jane = new Person()
    jane.Name <- "Jane"
    jane.Favourite <- vegemite
    jane.Address <- address3
    let jane = context.People.Add(jane)
    let count = context.SaveChanges()
    Assert.AreEqual(19, count)
    ()

type CodeFirstInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<CodeFirstContext>()
    override x.Seed(context : CodeFirstContext) = seedCodeFirstDatabase context

let CodeFirstConfig(name) = 
    let c = new CodeFirstEntityContextConfiguration()
    c.DbContext <- fun () -> upcast new CodeFirstContext(name)
    c

let CodeFirstCeConfig(name) = 
    let c = new CodeFirstEntityContextConfiguration()
    c.DbContext <- fun () -> upcast new CodeFirstContext(name)
    c

let CodeFirstSetup() = 
    System.Data.Entity.Database.SetInitializer(new CodeFirstInitializer())
    ()

let CodeFirstCeSetup() = 
    System.Data.Entity.Database.SetInitializer(new CodeFirstInitializer())
    ()

let CanCreateEntityPersistor codeOnlyPersistor = Assert.IsNotNull(codeOnlyPersistor)
let CanGetInstancesGeneric codeOnlyPersistor = GetInstancesGenericNotEmpty<Product> codeOnlyPersistor
let CanGetInstancesByType codeOnlyPersistor = GetInstancesByTypeNotEmpty<Product> codeOnlyPersistor
let CanGetInstancesIsProxy codeOnlyPersistor = GetInstancesReturnsProxies<Product> codeOnlyPersistor
let CanCreateTransientObject codeOnlyPersistor = CanCreateTransientObject<Product> codeOnlyPersistor
let CanSaveTransientObjectWithScalarProperties codeOnlyPersistor = CanSaveTransientObject codeOnlyPersistor (productSetter codeOnlyPersistor)


let CreateCountryCode code name (codeOnlyPersistor : IObjectStore) = 
    let createCC() = 
        let setter (cc : CountryCode) = 
            cc.Code <- code
            cc.Name <- name
        CreateAndSetup<CountryCode> codeOnlyPersistor setter
    
    let nocc = createCC()
    CreateAndEndTransaction codeOnlyPersistor nocc

let CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt(codeOnlyPersistor : IObjectStore) = 
    CreateCountryCode "keyCode1" "countryName1" codeOnlyPersistor
    try 
        CreateCountryCode "keyCode1" "countryName2" codeOnlyPersistor
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    CreateCountryCode "keyCode2" "countryName2" codeOnlyPersistor
    ()

let CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore(codeOnlyPersistor : IObjectStore) = 
    CreateCountryCode "keyCode3" "countryName1" codeOnlyPersistor
    try 
        CreateCountryCode "keyCode3" "countryName2" codeOnlyPersistor
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    let p2 = codeOnlyPersistor.CreateInstance<Product>(null)
    createProductWithID p2 (GetNextID<Product> codeOnlyPersistor (fun i -> i.ID)) codeOnlyPersistor

let CanNavigateReferences codeOnlyPersistor = 
    let c = First<Category> codeOnlyPersistor
    let pr = c.Products |> Seq.head
    Assert.IsNotNull(pr)

let CanSaveTransientObjectWithPersistentReferenceProperty codeOnlyPersistor = 
    let c = CreateAndSetup<Category> codeOnlyPersistor (categorySetter codeOnlyPersistor)
    let pr = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    CreateAndEndTransaction codeOnlyPersistor pr
    c.Products.Add(pr)
    SaveAndEndTransaction codeOnlyPersistor c

let CanSaveTransientObjectWithTransientReferenceProperty codeOnlyPersistor = 
    let c = CreateAndSetup<Category> codeOnlyPersistor (categorySetter codeOnlyPersistor)
    let pr = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    c.Products.Add(pr)
    CreateAndEndTransaction codeOnlyPersistor c

let CanSaveTransientObjectWithTransientCollection codeOnlyPersistor = 
    let c = CreateAndSetup<Category> codeOnlyPersistor (categorySetter codeOnlyPersistor)
    let pr0 = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    let pr1 = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    c.Products.Add(pr0)
    c.Products.Add(pr1)
    CreateAndEndTransaction codeOnlyPersistor c

let CanUpdatePersistentObjectWithScalarProperties codeOnlyPersistor = 
    let pr = First<Product> codeOnlyPersistor
    let origName = pr.Name
    let replName = uniqueName()
    
    let setNameAndSave name = 
        pr.Name <- name
        SaveAndEndTransaction codeOnlyPersistor pr
        Assert.AreEqual(name, pr.Name)
    setNameAndSave replName
    setNameAndSave origName

let CanUpdatePersistentObjectWithScalarPropertiesAbort codeOnlyPersistor = 
    let pr0 = First<Product> codeOnlyPersistor
    let origName = pr0.Name
    let newName = uniqueName()
    pr0.Name <- newName
    SaveWithNoEndTransaction codeOnlyPersistor pr0
    codeOnlyPersistor.AbortTransaction()
    codeOnlyPersistor.EndTransaction()
    let pr1 = First<Product> codeOnlyPersistor
    Assert.AreEqual(origName, pr1.Name)

let CanUpdatePersistentObjectWithReferenceProperties(codeOnlyPersistor : IObjectStore) = 
    let person = First<Person> codeOnlyPersistor
    let origFav = person.Favourite
    
    let replFav = 
        codeOnlyPersistor.GetInstances<Product>()
        |> Seq.filter (fun i -> i.ID <> origFav.ID)
        |> Seq.head
    
    let setFavouriteAndSave f = 
        person.Favourite <- f
        SaveAndEndTransaction codeOnlyPersistor person
        Assert.AreEqual(f, person.Favourite)
    
    setFavouriteAndSave replFav
    setFavouriteAndSave origFav

let CanUpdatePersistentObjectWithReferencePropertiesAbort(codeOnlyPersistor : IObjectStore) = 
    let person = First<Person> codeOnlyPersistor
    let origFav = person.Favourite
    
    let replFav = 
        codeOnlyPersistor.GetInstances<Product>()
        |> Seq.filter (fun i -> i.ID <> origFav.ID)
        |> Seq.head
    person.Favourite <- replFav
    SaveWithNoEndTransaction codeOnlyPersistor person
    codeOnlyPersistor.AbortTransaction()
    codeOnlyPersistor.EndTransaction()
    let person1 = First<Person> codeOnlyPersistor
    Assert.AreEqual(origFav, person1.Favourite)

let CanUpdatePersistentObjectWithCollectionProperties codeOnlyPersistor = 
    let c = First<Category> codeOnlyPersistor
    let origPr = c.Products |> Seq.head
    let replPr = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    
    let swapProducts oldPr newPr = 
        let b = c.Products.Remove(oldPr)
        c.Products.Add(newPr)
        SaveAndEndTransaction codeOnlyPersistor c
        Assert.IsTrue(c.Products |> Seq.exists (fun i -> i = newPr))
    swapProducts origPr replPr
    swapProducts replPr origPr

let CanPersistingPersistedCalledForCreateInstance(codeOnlyPersistor : IObjectStore) = 
    let nextId = GetNextID<Product> codeOnlyPersistor (fun i -> i.ID)
    persistingCount <- 0
    persistedCount <- 0
    let pr = codeOnlyPersistor.CreateInstance<Product>(null)
    createProductWithID pr nextId codeOnlyPersistor
    Assert.AreEqual(1, persistingCount, "persisting")
    Assert.AreEqual(1, persistedCount, "persisted")

let CanPersistingPersistedCalledForCreateInstanceWithCollection codeOnlyPersistor = 
    persistingCount <- 0
    persistedCount <- 0
    let c = CreateAndSetup<Category> codeOnlyPersistor (categorySetter codeOnlyPersistor)
    let pr = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    c.Products.Add(pr)
    CreateAndEndTransaction codeOnlyPersistor c
    Assert.AreEqual(2, persistingCount, "persisting")
    Assert.AreEqual(2, persistedCount, "persisted")

let CanUpdatingUpdatedCalledForChange codeOnlyPersistor = 
    updatingCount <- 0
    updatedCount <- 0
    let pr = First<Product> codeOnlyPersistor
    pr.Name <- uniqueName()
    SaveAndEndTransaction codeOnlyPersistor pr
    Assert.AreEqual(1, updatingCount, "updating")
    Assert.AreEqual(1, updatedCount, "updated")

let CanGetKeyForType(codeOnlyPersistor : IObjectStore) = 
    let keys = codeOnlyPersistor.GetKeys(typeof<Product>)
    Assert.AreEqual(1, keys.Length)
    Assert.AreEqual("ID", keys.[0].Name)

let GetNextAddressID codeOnlyPersistor = GetNextID<Address> codeOnlyPersistor (fun i -> i.ID)

let CanCreateDomesticSubclass(codeOnlyPersistor : IObjectStore) = 
    let address = codeOnlyPersistor.CreateInstance<DomesticAddress>(null)
    address.ID <- GetNextAddressID codeOnlyPersistor
    address.Lines <- uniqueName()
    address.Postcode <- uniqueName()
    CreateAndEndTransaction codeOnlyPersistor address

let CanCreateInternationalSubclass(codeOnlyPersistor : IObjectStore) = 
    let address = codeOnlyPersistor.CreateInstance<InternationalAddress>(null)
    address.ID <- GetNextAddressID codeOnlyPersistor
    address.Lines <- uniqueName()
    address.Country <- uniqueName()
    CreateAndEndTransaction codeOnlyPersistor address

let CanCreateBaseClass(codeOnlyPersistor : IObjectStore) = 
    let address = codeOnlyPersistor.CreateInstance<Address>(null)
    address.ID <- GetNextAddressID codeOnlyPersistor
    address.Lines <- uniqueName()
    CreateAndEndTransaction codeOnlyPersistor address

let CanGetBaseClassGeneric(codeOnlyPersistor : IObjectStore) = 
    let baseClasses = codeOnlyPersistor.GetInstances<Address>()
    checkCountAndType baseClasses typeof<Address>

let CanGetBaseClassByType(codeOnlyPersistor : IObjectStore) = 
    let baseClasses = codeOnlyPersistor.GetInstances(typeof<Address>) |> Seq.cast<Address>
    checkCountAndType baseClasses typeof<Address>

let CanGetDomesticSubclassClassGeneric(codeOnlyPersistor : IObjectStore) = 
    let subClasses = codeOnlyPersistor.GetInstances<DomesticAddress>()
    checkCountAndType subClasses typeof<DomesticAddress>

let CanGetInternationalSubclassClassGeneric(codeOnlyPersistor : IObjectStore) = 
    let subClasses = codeOnlyPersistor.GetInstances<InternationalAddress>()
    checkCountAndType subClasses typeof<InternationalAddress>

let CanGetDomesticSubclassClassByType(codeOnlyPersistor : IObjectStore) = 
    let subClasses = codeOnlyPersistor.GetInstances(typeof<DomesticAddress>) |> Seq.cast<DomesticAddress>
    checkCountAndType subClasses typeof<DomesticAddress>

let CanGetInternationalSubclassClassByType(codeOnlyPersistor : IObjectStore) = 
    let subClasses = codeOnlyPersistor.GetInstances(typeof<InternationalAddress>) |> Seq.cast<InternationalAddress>
    checkCountAndType subClasses typeof<InternationalAddress>

let CanNavigateToSubclass(codeOnlyPersistor : IObjectStore) = 
    let getPersonWithName name = 
        codeOnlyPersistor.GetInstances<Person>()
        |> Seq.filter (fun i -> i.Name = name)
        |> Seq.head
    
    let p1 = getPersonWithName "Ted"
    let a1 = p1.Address
    checkCountAndType (a1 |> Seq.singleton) typeof<DomesticAddress>
    let p2 = getPersonWithName "Bob"
    let a2 = p2.Address
    checkCountAndType (a2 |> Seq.singleton) typeof<DomesticAddress>
    let p3 = getPersonWithName "Jane"
    let a3 = p3.Address
    checkCountAndType (a3 |> Seq.singleton) typeof<InternationalAddress>

let CanGetClassWithNonPersistedBase codeOnlyPersistor = 
    let person = First<Person> codeOnlyPersistor
    Assert.IsNotNull(person)

let CanGetNonPersistedClass codeOnlyPersistor = 
    try 
        let abstractPerson = First<AbstractPerson> codeOnlyPersistor
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<NakedObjectApplicationException>, expected)

let CanContainerInjectionCalledForNewInstance codeOnlyPersistor = 
    injectedObjects.Clear()
    let c = CreateAndSetup codeOnlyPersistor (categorySetter codeOnlyPersistor)
    Assert.IsTrue(injectedObjects.Contains(c))

let CanContainerInjectionCalledForGetInstance codeOnlyPersistor = 
    let p = resetPersistor codeOnlyPersistor
    injectedObjects.Clear()
    let c = First<Category> codeOnlyPersistor
    Assert.IsTrue(injectedObjects.Contains(c))

let CanSaveTransientDomesticSubclasstWithScalarProperties codeOnlyPersistor = 
    let setter (a : DomesticAddress) = 
        a.ID <- GetNextAddressID codeOnlyPersistor
        a.Lines <- uniqueName()
        a.Postcode <- uniqueName()
    CanSaveTransientObject<DomesticAddress> codeOnlyPersistor setter

let CanSaveTransientIntlSubclassWithScalarProperties codeOnlyPersistor = 
    let setter (a : InternationalAddress) = 
        a.ID <- GetNextAddressID codeOnlyPersistor
        a.Lines <- uniqueName()
        a.Country <- uniqueName()
    CanSaveTransientObject<InternationalAddress> codeOnlyPersistor setter

let CanUpdatePersistentSubclassWithScalarProperties codeOnlyPersistor = 
    let a = First<DomesticAddress> codeOnlyPersistor
    let origLines = a.Lines
    let replLines = uniqueName()
    
    let setNameAndSave lines = 
        a.Lines <- lines
        SaveAndEndTransaction codeOnlyPersistor a
        Assert.AreEqual(lines, a.Lines)
    setNameAndSave replLines
    setNameAndSave origLines

let CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies codeOnlyPersistor = 
    let c = CreateAndSetup<Category> codeOnlyPersistor (categorySetter codeOnlyPersistor)
    let pr = CreateAndSetup codeOnlyPersistor (productSetter codeOnlyPersistor)
    Assert.IsFalse(EntityUtils.IsEntityProxy(c.GetType()))
    Assert.IsFalse(EntityUtils.IsEntityProxy(pr.GetType()))
    c.Products.Add(pr)
    pr.Owningcategory <- c
    CreateAndEndTransaction codeOnlyPersistor c
    let proxiedc = 
        codeOnlyPersistor.GetInstances<Category>()
        |> Seq.filter (fun i -> i.Name = c.Name)
        |> Seq.head
    Assert.IsTrue(EntityUtils.IsEntityProxy(proxiedc.GetType()))
    let proxiedpr = proxiedc.Products |> Seq.head
    Assert.IsTrue(EntityUtils.IsEntityProxy(proxiedpr.GetType()))

let CanGetObjectBySingleKey codeOnlyPersistor = 
    let key = GetMaxID<Product> codeOnlyPersistor (fun k -> k.ID)
    CanGetObjectByKey<Product> codeOnlyPersistor [| box key |]

let CodeOnlyCanGetContextForCollection persistor = CanGetContextForCollection<Product> persistor
let CodeOnlyCanGetContextForNonGenericCollection persistor = CanGetContextForNonGenericCollection<Product> persistor
let CodeOnlyCanGetContextForArray persistor = CanGetContextForArray<Product> persistor
let CodeOnlyCanGetContextForType persistor = CanGetContextForType<Product> persistor

let GetKeysReturnsKey(persistor : IObjectStore) = 
    let l = First<Person> persistor
    let keys = persistor.GetKeys(l.GetType())
    Assert.AreEqual(1, keys |> Seq.length)
    Assert.AreSame(typeof<Person>.GetProperty("ID"), keys |> Seq.head)