module NakedFramework.DomainTest

open NUnit.Framework
open NakedObjects.DomainTest
open NakedFramework.Architecture.Component
open NakedFramework.Persistor.EFCore.Configuration
open NakedObjects.TestCode
open NakedObjects.TestTypes
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open Microsoft.EntityFrameworkCore
open System
open NakedObjects.DomainTestCode
open NakedFramework.Persistor.EFCore.Component
open System.Data.Common

let efCorePersistor = 
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f = Func<DbContext> (fun () -> 
        let c = new EFCoreAdventureWorksEntities(csAWMARS)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

let efCoreOverwritePersistor =
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f = Func<DbContext> (fun () -> 
        let c = new EFCoreAdventureWorksEntities(csAWMARS)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f |]
    //c.DefaultMergeOption <- MergeOption.OverwriteChanges
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p


//[<TestFixture>]
//type EFCoreDomainTests() = 
//    inherit DomainTests()

//    [<OneTimeTearDown>]
//    member x.TearDown() = 
//        match x.persistor with 
//        | :? EFCoreObjectStore as eos -> eos.SetupContexts()
//        | _ -> ()

//    override x.persistor = efCorePersistor :> IObjectStore
    
//    override x.overwritePersistor = efCoreOverwritePersistor :> IObjectStore

    

