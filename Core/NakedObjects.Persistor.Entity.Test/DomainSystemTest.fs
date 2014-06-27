// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.DomainSystemTest
open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open AdventureWorksModel
open NakedObjects.Services
open System
open NakedObjects.EntityObjectStore
open NakedObjects.Core.Context
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Adapter
open SystemTestCode
open TestCode
open NakedObjects.Architecture.Util
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.Query

[<TestFixture>]
type DomainSystemTests() =
    inherit  NakedObjects.Xat.AcceptanceTestCase()    

    [<SetUp>]
    member x.SetupTest() = x.InitializeNakedObjectsFramework()
   
    [<TearDown>]
    member x.TearDownTest() = x.CleanupNakedObjectsFramework()

    override x.MenuServices = 
        let service = new SimpleRepository<ScrapReason>()
        box (new ServicesInstaller([| (box service) |])) :?> IServicesInstaller
        
    override x.Persistor = 
        let epi = new EntityPersistorInstaller()
        epi.ForceContextSet()
        box epi :?> IObjectPersistorInstaller
       
    
    member x.GetScrapReasonDomainObject() = 
        let srs = NakedObjectsContext.ObjectPersistor.Instances<ScrapReason>()
        query <@ srs
                 |> Seq.filter (fun s -> s.ScrapReasonID = 2s)
                 |> Seq.head @>
    
    member x.CreatePC() = 
        let setter (pc : ProductCategory) = 
            pc.Name <- uniqueName()
            pc.ModifiedDate <- DateTime.Now
            pc.ProductCategoryID <- 1
            pc.rowguid <- Guid.NewGuid()
        SystemTestCode.CreateAndSetup<ProductCategory> setter
    
    member x.CreatePSC() = 
        let setter (psc : ProductSubcategory) = 
            psc.Name <- uniqueName()
            psc.ModifiedDate <- DateTime.Now
            psc.ProductSubcategoryID <- 1
            psc.rowguid <- Guid.NewGuid()
        SystemTestCode.CreateAndSetup<ProductSubcategory> setter

    [<Test>]
    member x.GetService() = 
        let srService = NakedObjectsContext.ObjectPersistor.GetService("repository#AdventureWorksModel.ScrapReason")
        Assert.IsNotNull(srService.Object)     
        
    [<Test>]
    member x.GetCollectionDirectly() = 
        let srs = NakedObjectsContext.ObjectPersistor.Instances<ScrapReason>()
        Assert.Greater(query <@ srs |> Seq.length @>, 0)
        //Assert.Greater(srs |> Seq.length, 0)


    [<Test>]
    member x.CheckInstanceProperty() = 
       let sr = x.GetScrapReasonDomainObject()
       Assert.IsNotNull(sr)
       Assert.AreEqual("Color incorrect", sr.Name)
       
    [<Test>]
    member x.CheckItemIdentities() = 
       let getSrByKey (key : Int16) =  query <@ NakedObjectsContext.ObjectPersistor.Instances<ScrapReason>() |> Seq.filter (fun s -> s.ScrapReasonID = key) |> Seq.head @>
       //let getSrByKey (key : Int16) =  NakedObjectsContext.ObjectPersistor.Instances<ScrapReason>() |> Seq.filter (fun s -> s.ScrapReasonID = key) |> Seq.head
       
       let (sr1, sr2, sr3, sr4) = (getSrByKey 1s, getSrByKey 2s, getSrByKey 1s, getSrByKey 2s)
       Assert.AreSame(sr1, sr3)
       Assert.AreSame(sr2, sr4)
       Assert.AreNotSame(sr1, sr2)
       Assert.AreNotSame(sr2, sr3)
             
    [<Test>]
    member x.CheckPersistentResolveState() = 
       let sr = x.GetScrapReasonDomainObject()
       IsNotNullAndPersistent sr
       
    [<Test>]
    member x.CheckTransientResolveState() = 
       let pc = x.CreatePC()
       IsNotNullAndTransient pc
              
    [<Test>]
    member x.GetCollectionIndirectly() = 
       let sr = x.GetScrapReasonDomainObject()
       Assert.IsNotNull(sr)
       Assert.Greater(sr.WorkOrders |> Seq.length, 0)
       
    [<Test>]
    member x.GetCollectionItemIndirectly() = 
       let wo = x.GetScrapReasonDomainObject().WorkOrders |> Seq.head
       IsNotNullAndPersistent wo
       
    [<Test>]
    member x.CheckReferenceIdentities() = 
       let wo = query <@ NakedObjectsContext.ObjectPersistor.Instances<WorkOrder>() |> Seq.filter (fun w -> w.ScrapReason <> null) |> Seq.head @>
       //let wo = NakedObjectsContext.ObjectPersistor.Instances<WorkOrder>() |> Seq.filter (fun w -> w.ScrapReason <> null) |> Seq.head 
       Assert.IsNotNull(wo)
       IsNotNullAndPersistent wo.ScrapReason
       
    [<Test>]
    member x.CheckCollectionIdentities() = 
       let sr = x.GetScrapReasonDomainObject()
       let wo = sr.WorkOrders |> Seq.head
       Assert.AreSame(sr, wo.ScrapReason) 
                 
    [<Test>]
    member x.CreateNewObjectWithScalars() =    
       let pc = x.CreatePC()
       save pc
       IsNotNullAndPersistent pc
        
    [<Test>]
    member x.CreateNewObjectWithPersistentReference() =         
       let pscNo = x.CreatePSC()
       let psc = box pscNo.Object :?> ProductSubcategory
       let pc = query <@ NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.head @>
       //let pc =  NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.head 
       psc.ProductCategory <- pc
       save pscNo
       IsNotNullAndPersistent pscNo 
       
    [<Test>]
    member x.CreateNewObjectWithTransientReference() = 
       let pscNo = x.CreatePSC()
       let psc =  pscNo.Object :?> ProductSubcategory
       let pcNo = x.CreatePC()
       let pc = pcNo.Object :?> ProductCategory 
       psc.ProductCategory <- pc
       save pscNo
       IsNotNullAndPersistent pscNo 
       IsNotNullAndPersistent pcNo 
       
    [<Test>]
    member x.AddObjectToPersistentCollection() =
        let psc = query <@  NakedObjectsContext.ObjectPersistor.Instances<ProductSubcategory>() |> Seq.head @>
        //let psc = NakedObjectsContext.ObjectPersistor.Instances<ProductSubcategory>() |> Seq.head 
        let origPc = psc.ProductCategory
        let replPc =  query <@  NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID) |> Seq.head  @>  
        //let replPc =   NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID) |> Seq.head  
        let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) =
            NakedObjectsContext.ObjectPersistor.StartTransaction()  
            let b = oldPc.ProductSubcategories.Remove(psc)
            newPc.ProductSubcategories.Add(psc)
            NakedObjectsContext.ObjectPersistor.EndTransaction()       
            Assert.AreEqual(newPc, psc.ProductCategory)      
        swapSubcatsForCollection origPc replPc 
        swapSubcatsForCollection replPc origPc   
        
    [<Test>]
    member x.AddObjectToPersistentCollectionNotifiesUI() =
        let psc = query <@ NakedObjectsContext.ObjectPersistor.Instances<ProductSubcategory>() |> Seq.head @>
        //let psc =  NakedObjectsContext.ObjectPersistor.Instances<ProductSubcategory>() |> Seq.head 
        let origPc = psc.ProductCategory
        let replPc =  query <@ NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID) |> Seq.head @>    
        //let replPc =   NakedObjectsContext.ObjectPersistor.Instances<ProductCategory>() |> Seq.filter (fun i -> i.ProductCategoryID <> origPc.ProductCategoryID) |> Seq.head 
        let swapSubcatsForCollection (oldPc : ProductCategory) (newPc : ProductCategory) =
            NakedObjectsContext.ObjectPersistor.StartTransaction()  
            let b = oldPc.ProductSubcategories.Remove(psc)
            newPc.ProductSubcategories.Add(psc)
            NakedObjectsContext.ObjectPersistor.EndTransaction()       
            Assert.AreEqual(newPc, psc.ProductCategory)      
        NakedObjectsContext.UpdateNotifier.EnsureEmpty()
        swapSubcatsForCollection origPc replPc 
        swapSubcatsForCollection replPc origPc   
        let updates = Seq.toList (CollectionUtils.ToEnumerable<INakedObject>(NakedObjectsContext.UpdateNotifier.AllChangedObjects()))
        Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box origPc), "original PC")
        Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box replPc), "repl PC")
        Assert.IsTrue(updates |> Seq.exists (fun i -> i.Object = box psc), "PSC")