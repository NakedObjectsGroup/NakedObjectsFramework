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

let efCorePersistor = 
    //EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EFCorePersistorConfiguration()
    let f = Func<DbContext> (fun () -> new EFCoreAdventureWorksEntities(csAWMARS) :> DbContext)
    //c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    c.Contexts <- [| f |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

let efCoreOverwritePersistor =
    //EFCoreObjectStoreConfiguration.NoValidate <- true
    let c = new EFCorePersistorConfiguration()
    let f = Func<DbContext> (fun () -> new EFCoreAdventureWorksEntities(csAWMARS) :> DbContext)
    //c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    c.Contexts <- [| f |]
    //c.DefaultMergeOption <- MergeOption.OverwriteChanges
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

//[<TestFixture>]
//type EFCoreDomainTests() = 
//    inherit DomainTests()

//    override x.persistor = efCorePersistor :> IObjectStore
    
//    override x.overwritePersistor = efCoreOverwritePersistor :> IObjectStore

    

