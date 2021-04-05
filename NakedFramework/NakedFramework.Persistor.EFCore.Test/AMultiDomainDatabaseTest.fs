// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedFramework.AMultiDomainDatabaseTest

open NakedObjects
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly;
open NUnit.Framework
open SimpleDatabase
open System
open NakedFramework.Persistor.EFCore.Configuration
open Microsoft.EntityFrameworkCore
open NakedFramework.Architecture.Component
open TestCodeOnly
open TestTypes
open TestCode
open MultiDatabaseTestCode

//let multiDomainDatabasePersistor = 
//    EntityObjectStoreConfiguration.NoValidate <- true

//    let c = new EntityObjectStoreConfiguration()
//    let f = (fun () -> new SimpleDatabaseDbContext(csMF) :> Data.Entity.DbContext)
//    c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
//    c.UsingContext((CodeFirstConfig csMD).DbContext) |> ignore

//    let f1 = (fun () -> new AdventureWorksEntities(csAWMARS) :> Data.Entity.DbContext)
//    c.UsingContext(Func<Data.Entity.DbContext>(f1)) |> ignore

//    let p = getEntityObjectStore c
//    setupPersistorForTesting p

let multiDomainDatabasePersistor = 
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f = Func<DbContext> (fun () -> 
        let c = new EFCoreSimpleDatabaseDbContext(csMF)
        c.Create()
        c :> DbContext)
    let f1 = Func<DbContext> (fun () -> 
        let c = new EFCoreCodeFirstContext(csMD)
        c.Create()
        c :> DbContext)
    let f2 = Func<DbContext> (fun () -> 
        let c = new EFCoreAdventureWorksEntities(csAWMARS)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f; f1; f2 |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p



let DomainLoadTestAssembly() = 
    let obj = new Address()
    let obj = new Person()
    ()

let Setup() = DomainLoadTestAssembly()

[<TestFixture>]
type AMultiDomainDatabaseTests() = 
    class

        member x.multiDomainDatabasePersistor = multiDomainDatabasePersistor :> IObjectStore
        
        [<OneTimeSetUp>]
        member x.Setup() = Setup()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor x.multiDomainDatabasePersistor
        
        [<Test>]
        member x.TestCanQueryEachConnection() = CanQueryEachDomainConnection x.multiDomainDatabasePersistor
        
        [<Test>]     
        member x.TestCanQueryEachConnectionMultiTimes() = CanQueryEachDomainConnectionMultiTimes x.multiDomainDatabasePersistor
    end
