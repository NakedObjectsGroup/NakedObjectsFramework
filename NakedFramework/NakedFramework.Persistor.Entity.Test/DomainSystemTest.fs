// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.DomainSystemTest

open NakedFramework.Persistor.EF6.Configuration
open NakedObjects.TestTypes
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NakedObjects.Services
open NUnit.Framework
open System
open System.Data.Entity.Core.Objects.DataClasses
open SystemTestCode
open TestCode
open Microsoft.Extensions.Configuration
open NakedFramework.Test.TestCase
open NakedFramework.DependencyInjection.Extensions

[<TestFixture>]
type DomainSystemTests() = 
    inherit AcceptanceTestCase()


    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    override x.ContextInstallers = 
        [|  Func<IConfiguration, Data.Entity.DbContext> (fun (c : IConfiguration) -> new AdventureWorksEntities(csAWMARS) :> Data.Entity.DbContext) |]

    override x.Services =  [| typeof<SimpleRepository<ScrapReason>> |]

    override x.ObjectTypes = [| typeof<ProductReview>;
                                typeof<ProductDocument>;
                                typeof<ProductCostHistory>;
                                typeof<ProductListPriceHistory>;
                                typeof<TransactionHistory>;
                                typeof<UnitMeasure>;
                                typeof<ProductModel>;
                                typeof<PurchaseOrderDetail>;
                                typeof<ProductInventory>;
                                typeof<ProductVendor>;
                                typeof<ShoppingCartItem>;
                                typeof<SpecialOfferProduct>;
                                typeof<ProductProductPhoto>;
                                typeof<BillOfMaterial>;
                                typeof<WorkOrderRouting>;
                                typeof<Product>;
                                typeof<ScrapReason>;
                                typeof<WorkOrder>;
                                typeof<ProductSubcategory>;
                                typeof<ProductCategory>;
                                typeof<Location>;
                                typeof<SpecialOffer>;
                                typeof<Document>;
                                typeof<ProductPhoto>;
                                typeof<PurchaseOrderHeader>;
                                typeof<ProductModelProductDescriptionCulture>;
                                typeof<SalesOrderDetail>;
                                typeof<ProductModelIllustration>;
                                typeof<Vendor>;
                                typeof<Culture>;
                                typeof<VendorContact>;
                                typeof<Employee>;
                                typeof<ProductDescription>;
                                typeof<VendorAddress>;
                                typeof<SalesOrderHeader>;
                                typeof<Illustration>;
                                typeof<ShipMethod>;
                                typeof<SalesTerritory>;
                                typeof<Customer>;
                                typeof<CreditCard>;
                                typeof<EmployeePayHistory>;
                                typeof<JobCandidate>;
                                typeof<Contact>;
                                typeof<EmployeeDepartmentHistory>;
                                typeof<AddressType>;
                                typeof<EmployeeAddress>;
                                typeof<SalesPerson>;
                                typeof<ContactType>;
                                typeof<CurrencyRate>;
                                typeof<Address>;
                                typeof<SalesOrderHeaderSalesReason>;
                                typeof<ContactCreditCard>;
                                typeof<StateProvince>;
                                typeof<Individual>;
                                typeof<Department>;
                                typeof<CustomerAddress>;
                                typeof<SalesPersonQuotaHistory>;
                                typeof<Store>;
                                typeof<SalesTerritoryHistory>;
                                typeof<StoreContact>;
                                typeof<Shift>;
                                typeof<SalesReason>;
                                typeof<Currency>;
                                typeof<SalesTaxRate>;
                                typeof<CountryRegion>;
                                typeof<CountryRegionCurrency> |]
    
    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = ()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() = AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    member x.GetScrapReasonDomainObject() = 
        let srs = x.NakedObjectsFramework.Persistor.Instances<ScrapReason>()
        srs
        |> Seq.filter (fun s -> s.ScrapReasonID = 2s)
        |> Seq.head
    
    member x.CreatePC() = 
        let setter (pc : ProductCategory) = 
            pc.Name <- uniqueName()
            pc.ModifiedDate <- DateTime.Now
            pc.ProductCategoryID <- 1
            pc.rowguid <- Guid.NewGuid()
        SystemTestCode.CreateAndSetup<ProductCategory> setter x.NakedObjectsFramework
    
    member x.CreatePSC() = 
        let setter (psc : ProductSubcategory) = 
            psc.Name <- uniqueName()
            psc.ModifiedDate <- DateTime.Now
            psc.ProductSubcategoryID <- 1
            psc.rowguid <- Guid.NewGuid()
        SystemTestCode.CreateAndSetup<ProductSubcategory> setter x.NakedObjectsFramework
    
    [<Test>]
    member x.GetService() = 
        let srService = x.NakedObjectsFramework.ServicesManager.GetService("SimpleRepository-ScrapReason")
        Assert.IsNotNull(srService.Object)
    
    [<Test>]
    member x.GetCollectionDirectly() = 
        let srs = x.NakedObjectsFramework.Persistor.Instances<ScrapReason>()
        Assert.Greater(srs |> Seq.length, 0)
    
    [<Test>]
    member x.CheckInstanceProperty() = 
        let sr = x.GetScrapReasonDomainObject()
        Assert.IsNotNull(sr)
        Assert.AreEqual("Color incorrect", sr.Name)
    
    [<Test>]
    member x.CheckItemIdentities() = 
        let ctx = x.NakedObjectsFramework
        
        let getSrByKey (key : Int16) = 
            ctx.Persistor.Instances<ScrapReason>()
            |> Seq.filter (fun s -> s.ScrapReasonID = key)
            |> Seq.head
        
        let (sr1, sr2, sr3, sr4) = (getSrByKey 1s, getSrByKey 2s, getSrByKey 1s, getSrByKey 2s)
        Assert.AreSame(sr1, sr3)
        Assert.AreSame(sr2, sr4)
        Assert.AreNotSame(sr1, sr2)
        Assert.AreNotSame(sr2, sr3)
    
    [<Test>]
    member x.CheckPersistentResolveState() = 
        let sr = x.GetScrapReasonDomainObject()
        IsNotNullAndPersistent sr x.NakedObjectsFramework
    
    [<Test>]
    member x.CheckTransientResolveState() = 
        let pc = x.CreatePC()
        IsNotNullAndTransient pc x.NakedObjectsFramework
    
    [<Test>]
    member x.GetCollectionIndirectly() = 
        let sr = x.GetScrapReasonDomainObject()
        Assert.IsNotNull(sr)
        Assert.Greater(sr.WorkOrders |> Seq.length, 0)
    
    [<Test>]
    member x.GetCollectionItemIndirectly() = 
        let wo = x.GetScrapReasonDomainObject().WorkOrders |> Seq.head
        IsNotNullAndPersistent wo x.NakedObjectsFramework
    
    [<Test>]
    member x.CheckReferenceIdentities() = 
        let wo = 
            x.NakedObjectsFramework.Persistor.Instances<WorkOrder>()
            |> Seq.filter (fun w -> w.ScrapReason <> null)
            |> Seq.head
        Assert.IsNotNull(wo)
        IsNotNullAndPersistent wo.ScrapReason x.NakedObjectsFramework
    
    [<Test>]
    member x.CheckCollectionIdentities() = 
        let sr = x.GetScrapReasonDomainObject()
        let wo = sr.WorkOrders |> Seq.head
        Assert.AreSame(sr, wo.ScrapReason)
    
    [<Test>]
    member x.CreateNewObjectWithScalars() = 
        let pc = x.CreatePC()
        save pc x.NakedObjectsFramework
        IsNotNullAndPersistent pc x.NakedObjectsFramework
    
    [<Test>]
    member x.CreateNewObjectWithPersistentReference() = 
        let pscNo = x.CreatePSC()
        let psc = box pscNo.Object :?> ProductSubcategory
        let pc = x.NakedObjectsFramework.Persistor.Instances<ProductCategory>() |> Seq.head
        psc.ProductCategory <- pc
        save pscNo x.NakedObjectsFramework
        IsNotNullAndPersistent pscNo x.NakedObjectsFramework
    
    [<Test>]
    member x.CreateNewObjectWithTransientReference() = 
        let pscNo = x.CreatePSC()
        let psc = pscNo.Object :?> ProductSubcategory
        let pcNo = x.CreatePC()
        let pc = pcNo.Object :?> ProductCategory
        psc.ProductCategory <- pc
        save pscNo x.NakedObjectsFramework
        IsNotNullAndPersistent pscNo x.NakedObjectsFramework
        IsNotNullAndPersistent pcNo x.NakedObjectsFramework
    
    [<Test>]
    member x.AddObjectToPersistentCollection() = 
        let psc = x.NakedObjectsFramework.Persistor.Instances<ProductSubcategory>() |> Seq.head
        let origPc = psc.ProductCategory
        
        let replPc = 
            x.NakedObjectsFramework.Persistor.Instances<ProductCategory>()
            |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID)
            |> Seq.head
        
        let ctx = x.NakedObjectsFramework
        
        let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) = 
            ctx.TransactionManager.StartTransaction()
            let b = oldPc.ProductSubcategories.Remove(psc)
            newPc.ProductSubcategories.Add(psc)
            ctx.TransactionManager.EndTransaction()
            Assert.AreEqual(newPc, psc.ProductCategory)
        swapSubcatsForCollection origPc replPc
        swapSubcatsForCollection replPc origPc
    
    [<Test>]
    member x.AddObjectToPersistentCollectionNotifiesUI() = 
        let psc = x.NakedObjectsFramework.Persistor.Instances<ProductSubcategory>() |> Seq.head
        let origPc = psc.ProductCategory
        
        let replPc = 
            x.NakedObjectsFramework.Persistor.Instances<ProductCategory>()
            |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID)
            |> Seq.head
        
        let ctx = x.NakedObjectsFramework
        
        let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) = 
            ctx.TransactionManager.StartTransaction()
            let b = oldPc.ProductSubcategories.Remove(psc)
            newPc.ProductSubcategories.Add(psc)
            ctx.TransactionManager.EndTransaction()
            Assert.AreEqual(newPc, psc.ProductCategory)
        swapSubcatsForCollection origPc replPc
        swapSubcatsForCollection replPc origPc
