// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


module NakedObjects.AMultiDomainDatabaseTest

open NUnit.Framework
open TestTypes
open NakedObjects.EntityObjectStore
open MultiDatabaseTestCode
open CodeOnlyTestCode
open AdventureWorksModel
open ModelFirst
open NakedObjects.Core.Context
open NakedObjects.Core.Security
open System.Security.Principal
open NakedObjects.Reflector.DotNet
open NakedObjects.Architecture.Persist
open Moq

let multiDomainDatabasePersistor =
    let r = mockMetamodelManager.Object
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    let nom = (new Mock<INakedObjectManager>()).Object
    c.UsingEdmxContext "Model1Container" |> ignore
    c.UsingCodeFirstContext ((CodeFirstConfig "AMultiDatabaseTests").DbContext) |> ignore
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(r), r, i, nom)
    setupPersistorForTesting p


let DomainLoadTestAssembly() = 
    let obj = new Address()
    let obj = new Person()
    ()
      
let Setup() =
    DomainLoadTestAssembly()

[<TestFixture>]
type AMultiDomainDatabaseTests() = class              
    [<TestFixtureSetUp>] member x.Setup() = Setup()
    [<Test>] member x.TestCreateEntityPersistor() = CanCreateEntityPersistor multiDomainDatabasePersistor   
    [<Test>] member x.TestCanQueryEachConnection() = CanQueryEachDomainConnection multiDomainDatabasePersistor 
    [<Test>] member x.TestCanQueryEachConnectionMultiTimes() = CanQueryEachDomainConnectionMultiTimes multiDomainDatabasePersistor
    
end        

