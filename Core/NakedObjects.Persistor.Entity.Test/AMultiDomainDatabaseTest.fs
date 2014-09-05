module NakedObjects.AMultiDomainDatabaseTest
open NUnit.Framework
open TestTypes
open NakedObjects.EntityObjectStore
open TestCode
open System
open MultiDatabaseTestCode
open DomainTestCode
open CodeOnlyTestCode
open AdventureWorksModel
open ModelFirst
open NakedObjects.Core.Context
open NakedObjects.Core.Security
open System.Security.Principal
open NakedObjects.Reflector.DotNet

let multiDomainDatabasePersistor =
    let r = new MockReflector() 
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    c.UsingEdmxContext "Model1Container" |> ignore
    c.UsingCodeFirstContext ((CodeFirstConfig "AMultiDatabaseTests").DbContext) |> ignore
    //c.ContextConfiguration <- [|(box PocoConfig :?> EntityContextConfiguration);(box ModelTestCode.ModelConfig :?> EntityContextConfiguration)|]
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(r), r, i)
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

