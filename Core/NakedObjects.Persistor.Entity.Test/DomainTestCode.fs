// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.DomainTestCode

open NUnit.Framework
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NakedObjects.Core.Resolve
open System
open TestTypes
open TestCode
open System.Data.Entity.Core.Objects
open NakedObjects.Core
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Persistor.Entity.Util
open NakedObjects.Persistor.Entity.Adapter
open NakedObjects.Persistor.Entity.Component
open System.Data.Common
open System.Data.SqlClient

let First<'t when 't : not struct> persistor = First<'t> persistor
let Second<'t when 't : not struct> persistor = Second<'t> persistor

let DomainLoadTestAssembly() = 
    let obj = new Address()
    ()

let DomainSetup() = 
    DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
    DomainLoadTestAssembly()

let CanCreateEntityPersistor persistor = Assert.IsNotNull(persistor)
let CanGetInstancesGeneric persistor = GetInstancesGenericNotEmpty<ScrapReason> persistor
let CanGetInstancesByType persistor = GetInstancesByTypeNotEmpty<ScrapReason> persistor
let CanGetInstancesIsProxy persistor = GetInstancesReturnsProxies<ScrapReason> persistor
let CanGetInstancesIsNotProxy persistor = GetInstancesDoesntReturnProxies<ScrapReason> persistor
let CanCreateTransientObject persistor = CanCreateTransientObject<ScrapReason> persistor

let CreateProductSubcategory persistor = 
    let setter (psc : ProductSubcategory) = 
        psc.Name <- uniqueName()
        psc.ModifiedDate <- DateTime.Now
        psc.ProductSubcategoryID <- 1
        psc.rowguid <- Guid.NewGuid()
    CreateAndSetup persistor setter

let CreateProductCategory persistor = 
    let setter (pc : ProductCategory) = 
        pc.Name <- uniqueName()
        pc.ModifiedDate <- DateTime.Now
        pc.ProductCategoryID <- 1
        pc.rowguid <- Guid.NewGuid()
    CreateAndSetup persistor setter

let CanSaveTransientObjectWithScalarProperties persistor = 
    let setter (sr : ScrapReason) = 
        sr.Name <- uniqueName()
        sr.ModifiedDate <- DateTime.Now
    CanSaveTransientObject persistor setter

let GetOrigAndReplProductCategories persistor = 
    let psc = First<ProductSubcategory> persistor
    let origPc = psc.ProductCategory
    
    let replPc = 
        persistor.GetInstances<ProductCategory>()
        |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID)
        |> Seq.head
    (psc, origPc, replPc)

let GetOrigAndReplProductCategoriesWithResolve persistor = 
    let psc = First<ProductSubcategory> persistor
    persistor.EndTransaction()
    persistor.ResolveImmediately(GetOrAddAdapterForTest psc null)
    let origPc = psc.ProductCategory
    
    let replPc = 
        persistor.GetInstances<ProductCategory>()
        |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID)
        |> Seq.head
    (psc, origPc, replPc)

let PersistingPersistedCalledForCreateInstance persistor sto = 
    persistingCount <- 0
    persistedCount <- 0
    sto persistor
    Assert.AreEqual(1, persistingCount, "persisting")
    Assert.AreEqual(1, persistedCount, "persisted")

let PersistingPersistedCalledForCreateInstanceWithReference persistor sto = 
    persistingCount <- 0
    persistedCount <- 0
    sto persistor
    Assert.AreEqual(2, persistingCount, "persisting")
    Assert.AreEqual(2, persistedCount, "persisted")

let CanSaveTransientObjectWithPersistentReferencePropertyWithFixup persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = (First<ProductCategory> persistor)
    persistor.ResolveImmediately(GetOrAddAdapterForTest pc null)
    psc.ProductCategory <- pc
    pc.ProductSubcategories.Add psc
    CreateAndEndTransaction persistor psc

let CanSaveTransientObjectWithPersistentReferenceProperty persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = (First<ProductCategory> persistor)
    psc.ProductCategory <- pc
    CreateAndEndTransaction persistor psc

let CanSaveTransientObjectWithPersistentReferencePropertyInSeperateTransaction(persistor : EntityObjectStore) = 
    persistor.StartTransaction()
    let psc = CreateProductSubcategory persistor
    let pc = (First<ProductCategory> persistor)
    psc.ProductCategory <- pc
    persistor.EndTransaction()
    CreateAndEndTransaction persistor psc

let CanSaveTransientObjectWithTransientReferenceProperty persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = CreateProductCategory persistor
    psc.ProductCategory <- pc
    CreateAndEndTransaction persistor psc

let CanSaveTransientObjectWithTransientReferencePropertyWithFixup persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = CreateProductCategory persistor
    psc.ProductCategory <- pc
    pc.ProductSubcategories.Add psc
    CreateAndEndTransaction persistor psc

let CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = CreateProductCategory persistor
    Assert.IsFalse(EntityUtils.IsEntityProxy(psc.GetType()))
    Assert.IsFalse(EntityUtils.IsEntityProxy(pc.GetType()))
    psc.ProductCategory <- pc
    CreateAndEndTransaction persistor psc
    let proxiedpsc = 
        persistor.GetInstances<ProductSubcategory>()
        |> Seq.filter (fun i -> i.Name = psc.Name)
        |> Seq.head
    Assert.IsTrue(EntityUtils.IsEntityProxy(proxiedpsc.GetType()))
    let proxiedpc = proxiedpsc.ProductCategory
    Assert.IsTrue(EntityUtils.IsEntityProxy(proxiedpc.GetType()))

let CanSaveTransientObjectWithTransientReferencePropertyAndConfirmNoProxies persistor = 
    let psc = CreateProductSubcategory persistor
    let pc = CreateProductCategory persistor
    Assert.IsFalse(EntityUtils.IsEntityProxy(psc.GetType()))
    Assert.IsFalse(EntityUtils.IsEntityProxy(pc.GetType()))
    psc.ProductCategory <- pc
    pc.ProductSubcategories.Add psc
    CreateAndEndTransaction persistor psc
    let proxiedpsc = 
        persistor.GetInstances<ProductSubcategory>()
        |> Seq.filter (fun i -> i.Name = psc.Name)
        |> Seq.head
    Assert.IsFalse(EntityUtils.IsEntityProxy(proxiedpsc.GetType()))
    let proxiedpc = proxiedpsc.ProductCategory
    Assert.IsFalse(EntityUtils.IsEntityProxy(proxiedpc.GetType()))

let CanUpdatePersistentObjectWithScalarProperties persistor = 
    let sr = First<ScrapReason> persistor
    let origName = sr.Name
    let replName = uniqueName()
    
    let setNameAndSave name = 
        sr.Name <- name
        SaveAndEndTransaction persistor sr
        Assert.AreEqual(name, sr.Name)
    setNameAndSave replName
    setNameAndSave origName

let CanUpdatePersistentObjectWithReferenceProperties persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    
    let setCategoryAndSave pc = 
        psc.ProductCategory <- pc
        SaveAndEndTransaction persistor psc
        Assert.AreEqual(pc, psc.ProductCategory)
    setCategoryAndSave replPc
    setCategoryAndSave origPc

let CanUpdatePersistentObjectWithReferencePropertiesWithResolve persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    
    let setCategoryAndSave pc = 
        psc.ProductCategory <- pc
        persistor.ResolveImmediately(GetOrAddAdapterForTest pc null)
        pc.ProductSubcategories.Add psc
        SaveAndEndTransaction persistor psc
        Assert.AreEqual(pc, psc.ProductCategory)
    setCategoryAndSave replPc
    setCategoryAndSave origPc

let CanUpdatePersistentObjectWithReferencePropertiesAbort persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    psc.ProductCategory <- replPc
    SaveWithNoEndTransaction persistor psc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let psc1 = First<ProductSubcategory> persistor
    Assert.AreEqual(origPc, psc1.ProductCategory)

let CanUpdatePersistentObjectWithReferencePropertiesAbortWithResolve persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    psc.ProductCategory <- replPc
    SaveWithNoEndTransaction persistor psc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let psc1 = First<ProductSubcategory> persistor
    Assert.AreEqual(origPc, psc1.ProductCategory)

let CanUpdatePersistentObjectWithReferencePropertiesAbortWithFixup persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    origPc.ProductSubcategories.Remove(psc)  |> ignore
    psc.ProductCategory <- replPc
    replPc.ProductSubcategories.Add(psc)
    SaveWithNoEndTransaction persistor psc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let psc1 = First<ProductSubcategory> persistor
    persistor.ResolveImmediately(GetOrAddAdapterForTest psc1 null)
    Assert.AreEqual(origPc, psc1.ProductCategory)

let CanUpdatePersistentObjectWithReferencePropertiesDoFixup persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    
    let swapSubcatsWithFixup (oldPc : ProductCategory) (newPc : ProductCategory) = 
        let b = oldPc.ProductSubcategories.Remove(psc)
        psc.ProductCategory <- newPc
        newPc.ProductSubcategories.Add(psc)
        SaveAndEndTransaction persistor psc
        Assert.AreEqual(newPc, psc.ProductCategory)
    swapSubcatsWithFixup origPc replPc
    swapSubcatsWithFixup replPc origPc

let CanUpdatePersistentObjectWithReferencePropertiesDoFixupWithResolve persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    
    let swapSubcatsWithFixup (oldPc : ProductCategory) (newPc : ProductCategory) = 
        oldPc.ProductSubcategories.Remove(psc)  |> ignore
        psc.ProductCategory <- newPc
        newPc.ProductSubcategories.Add(psc)
        SaveAndEndTransaction persistor psc
        Assert.AreEqual(newPc, psc.ProductCategory)
    swapSubcatsWithFixup origPc replPc
    swapSubcatsWithFixup replPc origPc

let CanUpdatePersistentObjectWithCollectionProperties persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    
    let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) = 
        oldPc.ProductSubcategories.Remove(psc)  |> ignore
        newPc.ProductSubcategories.Add(psc)
        SaveAndEndTransaction persistor origPc
        SaveAndEndTransaction persistor newPc
        Assert.AreEqual(newPc, psc.ProductCategory)
    swapSubcatsForCollection origPc replPc
    swapSubcatsForCollection replPc origPc

let CanUpdatePersistentObjectWithCollectionPropertiesAbort persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    origPc.ProductSubcategories.Remove(psc)  |> ignore
    replPc.ProductSubcategories.Add(psc)
    SaveWithNoEndTransaction persistor origPc
    SaveWithNoEndTransaction persistor replPc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let (psc1, origPc1, replPc1) = GetOrigAndReplProductCategories persistor
    Assert.IsTrue(origPc1.ProductSubcategories.Contains(psc1))
    Assert.IsFalse(replPc1.ProductSubcategories.Contains(psc1))

let CanUpdatePersistentObjectWithCollectionPropertiesAbortWithResolve persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    origPc.ProductSubcategories.Remove(psc)  |> ignore
    replPc.ProductSubcategories.Add(psc)
    SaveWithNoEndTransaction persistor origPc
    SaveWithNoEndTransaction persistor replPc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let (psc1, origPc1, replPc1) = GetOrigAndReplProductCategories persistor
    Assert.IsTrue(origPc1.ProductSubcategories.Contains(psc1))
    Assert.IsFalse(replPc1.ProductSubcategories.Contains(psc1))

let CanUpdatePersistentObjectWithCollectionPropertiesAbortWithFixup persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    origPc.ProductSubcategories.Remove(psc)  |> ignore
    replPc.ProductSubcategories.Add(psc)
    psc.ProductCategory <- replPc
    SaveWithNoEndTransaction persistor origPc
    SaveWithNoEndTransaction persistor replPc
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let (psc1, origPc1, replPc1) = GetOrigAndReplProductCategoriesWithResolve persistor
    Assert.IsTrue(origPc1.ProductSubcategories.Contains(psc1))
    Assert.IsFalse(replPc1.ProductSubcategories.Contains(psc1))

let CanUpdatePersistentObjectWithCollectionPropertiesDoFixup persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategories persistor
    
    let swapSubcatsForCollectionFixup (oldPc : ProductCategory) (newPc : ProductCategory) = 
        oldPc.ProductSubcategories.Remove(psc)  |> ignore
        newPc.ProductSubcategories.Add(psc)
        psc.ProductCategory <- newPc
        SaveAndEndTransaction persistor origPc
        SaveAndEndTransaction persistor newPc
        Assert.AreEqual(newPc, psc.ProductCategory)
    swapSubcatsForCollectionFixup origPc replPc
    swapSubcatsForCollectionFixup replPc origPc

let CanUpdatePersistentObjectWithCollectionPropertiesWithResolve persistor = 
    let (psc, origPc, replPc) = GetOrigAndReplProductCategoriesWithResolve persistor
    
    let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) = 
        oldPc.ProductSubcategories.Remove(psc)  |> ignore
        newPc.ProductSubcategories.Add(psc)
        SaveAndEndTransaction persistor origPc
        SaveAndEndTransaction persistor newPc
        Assert.AreEqual(newPc, psc.ProductCategory)
    swapSubcatsForCollection origPc replPc
    swapSubcatsForCollection replPc origPc

let CanNavigateReferences persistor = 
    let sr = First<ScrapReason> persistor
    let wo = sr.WorkOrders |> Seq.head
    let sr1 = wo.ScrapReason
    Assert.AreEqual(sr, sr1)

let CanNavigateReferencesNoProxies(persistor : EntityObjectStore) = 
    persistor.StartTransaction()
    let sr = First<ScrapReason> persistor
    Assert.AreEqual(0, sr.WorkOrders.Count)
    persistor.EndTransaction()
    persistor.ResolveImmediately(GetOrAddAdapterForTest sr null)
    let wo = sr.WorkOrders |> Seq.head
    let sr1 = wo.ScrapReason
    Assert.AreEqual(sr, sr1)

let setNameAndSave persistor (sr : ScrapReason) name = 
    sr.Name <- name
    SaveAndEndTransaction persistor sr
    Assert.AreEqual(name, sr.Name)

let CanUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt persistor = 
    let sr0 = First<ScrapReason> persistor
    let sr1 = Second<ScrapReason> persistor
    let origName = sr0.Name
    try 
        setNameAndSave persistor sr0 sr1.Name
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    let replName = uniqueName()
    setNameAndSave persistor sr0 replName
    setNameAndSave persistor sr0 origName

let CanUpdatePersistentObjectWithScalarPropertiesAbort persistor = 
    let sr0 = First<ScrapReason> persistor
    let origName = sr0.Name
    let newName = uniqueName()
    sr0.Name <- newName
    SaveWithNoEndTransaction persistor sr0
    persistor.AbortTransaction()
    persistor.EndTransaction()
    let sr1 = 
        persistor.GetInstances<ScrapReason>()
        |> Seq.filter (fun i -> i.ScrapReasonID = sr0.ScrapReasonID)
        |> Seq.head
    Assert.AreEqual(origName, sr1.Name)

let CanUpdatePersistentObjectWithScalarPropertiesIgnore persistor = 
    let sr0 = First<ScrapReason> persistor
    let sr1 = Second<ScrapReason> persistor
    try 
        setNameAndSave persistor sr0 sr1.Name
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    let origName = sr1.Name
    let replName = uniqueName()
    setNameAndSave persistor sr1 replName
    setNameAndSave persistor sr1 origName

let createWithName persistor (sr : ScrapReason) name = 
    sr.Name <- name
    sr.ModifiedDate <- DateTime.Now
    CreateAndEndTransaction persistor sr

let CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt(persistor : EntityObjectStore) = 
    let sr0 = persistor.CreateInstance<ScrapReason>(null)
    let sr1 = Second<ScrapReason> persistor
    try 
        createWithName persistor sr0 sr1.Name
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    createWithName persistor sr0 (uniqueName())

let CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore(persistor : EntityObjectStore) = 
    let sr0 = persistor.CreateInstance<ScrapReason>(null)
    let sr1 = Second<ScrapReason> persistor
    try 
        createWithName persistor sr0 sr1.Name
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    let sr2 = persistor.CreateInstance<ScrapReason>(null)
    createWithName persistor sr2 (uniqueName())

let CanPersistingPersistedCalledForCreateInstance persistor = PersistingPersistedCalledForCreateInstance persistor CanSaveTransientObjectWithScalarProperties
let CanPersistingPersistedCalledForCreateInstanceWithReference persistor = 
    PersistingPersistedCalledForCreateInstanceWithReference persistor CanSaveTransientObjectWithTransientReferencePropertyWithFixup

let CanUpdatingUpdatedCalledForChange persistor = 
    updatingCount <- 0
    updatedCount <- 0
    let sr = First<ScrapReason> persistor
    sr.Name <- uniqueName()
    SaveAndEndTransaction persistor sr
    Assert.AreEqual(1, updatingCount, "updating")
    Assert.AreEqual(1, updatedCount, "updated")

let CanGetKeyForType(persistor : EntityObjectStore) = 
    let keys = persistor.GetKeys(typeof<ScrapReason>)
    Assert.AreEqual(1, keys.Length)
    Assert.AreEqual("ScrapReasonID", keys.[0].Name)

let CanGetKeysForType(persistor : EntityObjectStore) = 
    let keys = persistor.GetKeys(typeof<SalesOrderHeaderSalesReason>)
    Assert.AreEqual(2, keys.Length)
    Assert.AreEqual("SalesOrderID", keys.[0].Name)
    Assert.AreEqual("SalesReasonID", keys.[1].Name)

let CanContainerInjectionCalledForNewInstance persistor = 
    injectedObjects.Clear()
    let pc = CreateProductCategory persistor
    Assert.IsTrue(injectedObjects.Contains(pc))

let CanContainerInjectionCalledForGetInstance persistor = 
    injectedObjects.Clear()
    let sr = First<ScrapReason> persistor
    Assert.IsTrue(injectedObjects.Contains(sr))

let noEntryFor (persistor : EntityObjectStore) (p : Product) (so : SpecialOffer) = 
    let hasEntry = 
        persistor.GetInstances<SpecialOfferProduct>() 
        |> Seq.exists (fun i -> (i.Product <> null && i.Product.ProductID = p.ProductID) && (i.SpecialOffer <> null && i.SpecialOffer.SpecialOfferID = so.SpecialOfferID))
    not (hasEntry)

let CanCreateManyToMany(persistor : EntityObjectStore) = 
    let prs = persistor.GetInstances<Product>()
    let sos = persistor.GetInstances<SpecialOffer>()
    let noEntryFor = noEntryFor persistor
    
    let getPandSO() = 
        prs
        |> Seq.filter (fun p -> (sos |> Seq.exists (fun so -> noEntryFor p so)))
        |> Seq.map (fun p -> 
               (p, 
                (sos
                 |> Seq.filter (fun so -> noEntryFor p so)
                 |> Seq.head)))
        |> Seq.head
    
    let (pr, so) = getPandSO()
    let sop = persistor.CreateInstance<SpecialOfferProduct>(null)
    sop.ModifiedDate <- DateTime.Now
    sop.rowguid <- Guid.NewGuid()
    sop.Product <- pr
    sop.SpecialOffer <- so
    CreateAndEndTransaction persistor sop
    Assert.IsFalse(noEntryFor pr so)

let CanCreateManyToManyWithFixup(persistor : EntityObjectStore) = 
    let prs = persistor.GetInstances<Product>()
    let sos = persistor.GetInstances<SpecialOffer>()
    let noEntryFor = noEntryFor persistor
    
    let getPandSO() = 
        prs
        |> Seq.filter (fun p -> (sos |> Seq.exists (fun so -> noEntryFor p so)))
        |> Seq.map (fun p -> 
               (p, 
                (sos
                 |> Seq.filter (fun so -> noEntryFor p so)
                 |> Seq.head)))
        |> Seq.head
    persistor.StartTransaction()
    let (pr, so) = getPandSO()
    persistor.EndTransaction()
    let no1 = GetOrAddAdapterForTest pr null
    let no2 = GetOrAddAdapterForTest so null
    persistor.ResolveImmediately(no1)
    persistor.ResolveImmediately(no2)
    let sop = persistor.CreateInstance<SpecialOfferProduct>(null)
    sop.ModifiedDate <- DateTime.Now
    sop.rowguid <- Guid.NewGuid()
    sop.Product <- pr
    sop.SpecialOffer <- so
    pr.SpecialOfferProducts.Add sop
    so.SpecialOfferProducts.Add sop
    CreateAndEndTransaction persistor sop
    Assert.IsFalse(noEntryFor pr so)

let CanGetObjectBySingleKey persistor = CanGetObjectByKey<ScrapReason> persistor [| box 16s |]

let CanGetObjectByMultiKey persistor = 
    CanGetObjectByKey<CustomerAddress> persistor [| box 1
                                                    box 832 |]

let CanGetObjectByStringKey persistor = CanGetObjectByKey<CountryRegion> persistor [| box "AU" |]

let CanGetObjectByDateKey persistor = 
    let datetime = new DateTime(2001, 7, 1)
    CanGetObjectByKey<SalesPersonQuotaHistory> persistor [| box 268
                                                            box datetime |]

let CanGetManyToOneReference(persistor : EntityObjectStore) = 
    let header = persistor.GetInstances<SalesOrderHeader>() |> Seq.head
    let details = persistor.GetInstances<SalesOrderDetail>() |> Seq.filter (fun d -> d.SalesOrderID = header.SalesOrderID)
    let detail1 = details |> Seq.item 0
    let detail2 = details |> Seq.item 1
    let header1 = detail1.SalesOrderHeader
    let header2 = detail2.SalesOrderHeader
    Assert.AreSame(header, header1)
    Assert.AreSame(header1, header2)

let CanRemoteResolve(persistor : EntityObjectStore) = 
    let keys = 
        [| box 54002
           box 51409 |]
    
    let key = new EntityOid(mockMetamodelManager.Object, typeof<SalesOrderDetail>, keys, false)
    let obj = persistor.GetObjectByKey(key, typeof<SalesOrderDetail>)
    let nakedObj = GetOrAddAdapterForTest obj key
    if nakedObj.ResolveState.IsResolvable() then persistor.ResolveImmediately(nakedObj)
    let props = typeof<SalesOrderDetail>.GetProperties()
    let sink obj = ()
    props |> Seq.iter (fun p -> sink (p.GetValue(obj, null)))

let DomainCanGetContextForCollection persistor = CanGetContextForCollection<SalesOrderHeader> persistor
let DomainCanGetContextForNonGenericCollection persistor = CanGetContextForNonGenericCollection<SalesOrderHeader> persistor
let DomainCanGetContextForArray persistor = CanGetContextForArray<SalesOrderHeader> persistor
let DomainCanGetContextForType persistor = CanGetContextForType<SalesOrderHeader> persistor

let CanDetectConcurrency(persistor : EntityObjectStore) = 
    let sr1 = persistor.GetInstances<ScrapReason>() |> Seq.head
    
    let otherPersistor =
        EntityObjectStoreConfiguration.NoValidate <- true
        let c = new EntityObjectStoreConfiguration()
        let f = fun () -> new AdventureWorksEntities(csAW) :> Data.Entity.DbContext   
        c.UsingCodeFirstContext(Func<Data.Entity.DbContext>f) |> ignore
        c.DefaultMergeOption <- MergeOption.AppendOnly
        let p = getEntityObjectStore c
        setupPersistorForTesting p
    
    let sr2 = otherPersistor.GetInstances<ScrapReason>() |> Seq.head
    Assert.AreEqual(sr1.Name, sr2.Name)
    let origName = sr1.Name
    sr1.Name <- uniqueName()
    SaveAndEndTransaction persistor sr1
    try 
        sr2.Name <- uniqueName()
        SaveAndEndTransaction otherPersistor sr2
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<ConcurrencyException>, expected)
    Assert.AreEqual(sr1.Name, sr2.Name)
    sr1.Name <- origName
    SaveAndEndTransaction persistor sr1

let DataUpdateNoCustomOnPersistingError(persistor : EntityObjectStore) = 
    let setter cr (l : Location) = 
        l.Name <- uniqueName()
        l.CostRate <- cr
        l.Availability <- 1m
        l.ModifiedDate <- DateTime.Now
    try 
        let l = CreateAndSetup persistor (setter -1m)
        CreateAndEndTransaction persistor l
        Assert.Fail()
    with expected -> 
        Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
        Assert.True(expected.Message.StartsWith("Data update problem"), "unexpected error message")
    // ok set set new value after error 
    let l = CreateAndSetup persistor (setter 1m)
    CreateAndEndTransaction persistor l

let DataUpdateNoCustomOnUpdatingError(persistor : EntityObjectStore) = 
    let l = First<Location> persistor
    let origCr = l.CostRate
    
    let setCostRateAndSave cr = 
        l.CostRate <- cr
        SaveAndEndTransaction persistor l
    try 
        setCostRateAndSave -1m
        Assert.Fail()
    with expected -> 
        Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
        Assert.True(expected.Message.StartsWith("Data update problem"), "unexpected error message")
    // ok set set new value after error 
    setCostRateAndSave 1m
    // put original back 
    setCostRateAndSave origCr

let ConcurrencyNoCustomOnUpdatingError(persistor : EntityObjectStore) = 
    let l1 = persistor.GetInstances<Location>() |> Seq.head
    
    let otherPersistor = 
        EntityObjectStoreConfiguration.NoValidate <- true
        let c = new EntityObjectStoreConfiguration()
        let f = fun () -> new AdventureWorksEntities(csAW) :> Data.Entity.DbContext   
        c.UsingCodeFirstContext(Func<Data.Entity.DbContext>f) |> ignore
        c.DefaultMergeOption <- MergeOption.AppendOnly
        let p = getEntityObjectStore c
        setupPersistorForTesting p
    
    let l2 = otherPersistor.GetInstances<Location>() |> Seq.head
    Assert.AreEqual(l1.Name, l2.Name)
    let origname = l1.Name
    l1.Name <- uniqueName()
    SaveAndEndTransaction persistor l1
    try 
        l2.Name <- uniqueName()
        SaveAndEndTransaction otherPersistor l2
        Assert.Fail()
    with expected -> 
        Assert.IsInstanceOf(typeof<ConcurrencyException>, expected)
        Assert.True(expected.Message.StartsWith("The object you were viewing"), "unexpected error message")
    Assert.AreEqual(l1.Name, l2.Name)
    l1.Name <- origname
    SaveAndEndTransaction persistor l1

let OverWriteChangesOptionRefreshesObject(persistor : EntityObjectStore) = 
    let l1 = First<Location> persistor
    let origName = l1.Name
    l1.Name <- uniqueName()
    let l2 = First<Location> persistor
    Assert.AreEqual(origName, l1.Name)

let AppendOnlyOptionDoesNotRefreshObject(persistor : EntityObjectStore) = 
    let l1 = First<Location> persistor
    let origName = l1.Name
    let newName = uniqueName()
    l1.Name <- newName
    let l2 = First<Location> persistor
    Assert.AreEqual(newName, l1.Name)
    ignore (resetPersistor persistor)

let FirstLocationNonGeneric(persistor : EntityObjectStore) = 
    let o = Seq.cast<Location> (persistor.GetInstances(typeof<Location>)) |> Seq.head
    o

let OverWriteChangesOptionRefreshesObjectNonGenericGet(persistor : EntityObjectStore) = 
    let l1 = FirstLocationNonGeneric persistor
    let origName = l1.Name
    l1.Name <- uniqueName()
    let l2 = FirstLocationNonGeneric persistor
    Assert.AreEqual(origName, l1.Name)

let AppendOnlyOptionDoesNotRefreshObjectNonGenericGet(persistor : EntityObjectStore) = 
    let l1 = FirstLocationNonGeneric persistor
    let origName = l1.Name
    let newName = uniqueName()
    l1.Name <- newName
    let l2 = FirstLocationNonGeneric persistor
    Assert.AreEqual(newName, l1.Name)
    ignore (resetPersistor persistor)

let ExplicitOverwriteChangesRefreshesObject(persistor : EntityObjectStore) = 
    let l1 = First<Location> persistor
    let origName = l1.Name
    l1.Name <- uniqueName()
    let q = persistor.GetInstances<Location>()
    let oq = q :?> ObjectQuery
    oq.MergeOption <- MergeOption.OverwriteChanges
    let l2 = q |> Seq.head
    Assert.AreEqual(origName, l1.Name)

let GetKeysReturnsKey(persistor : EntityObjectStore) = 
    let l = First<Location> persistor
    let keys = persistor.GetKeys(l.GetType())
    Assert.AreEqual(1, keys |> Seq.length)
    Assert.AreSame(typeof<Location>.GetProperty("LocationID"), keys |> Seq.head)