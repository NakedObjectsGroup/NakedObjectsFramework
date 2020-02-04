// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.AMultiDomainDatabaseTest

open NUnit.Framework
open TestTypes
open TestCode
open MultiDatabaseTestCode
open CodeOnlyTestCode
open SimpleDatabase
open NakedObjects.Persistor.Entity.Configuration
open System
open TestCodeOnly
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly;

let multiDomainDatabasePersistor = 
    EntityObjectStoreConfiguration.NoValidate <- true

    let c = new EntityObjectStoreConfiguration()

    let csOne = "Data Source=.\SQLEXPRESS;Initial Catalog=ModelFirst;Integrated Security=True;"

    let csTwo = "Data Source=.\SQLEXPRESS;Initial Catalog=AMultiDatabaseTests;Integrated Security=True;"

    let f = (fun () -> new SimpleDatabaseDbContext(csOne) :> Data.Entity.DbContext)
    c.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
    c.UsingCodeFirstContext((CodeFirstConfig csTwo).DbContext) |> ignore

    let csThree = "data source=.\SQLEXPRESS;initial catalog=AdventureWorks;integrated security=True;MultipleActiveResultSets=True;"

    let f1 = (fun () -> new AdventureWorksEntities(csThree) :> Data.Entity.DbContext)
    c.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f1)) |> ignore

    let p = getEntityObjectStore c
    setupPersistorForTesting p

let DomainLoadTestAssembly() = 
    let obj = new Address()
    let obj = new Person()
    ()

let Setup() = DomainLoadTestAssembly()

[<TestFixture>]
type AMultiDomainDatabaseTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = Setup()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor multiDomainDatabasePersistor
        
        [<Test>]
        member x.TestCanQueryEachConnection() = CanQueryEachDomainConnection multiDomainDatabasePersistor
        
        [<Test>]     
        member x.TestCanQueryEachConnectionMultiTimes() = CanQueryEachDomainConnectionMultiTimes multiDomainDatabasePersistor
    end
