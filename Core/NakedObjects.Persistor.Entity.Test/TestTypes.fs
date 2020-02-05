// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.TestTypes

open Moq
open NakedObjects.Architecture.Adapter
open NakedObjects.Architecture.Component
open NakedObjects.Architecture.Spec
open NakedObjects.Core.Resolve
open NakedObjects.Persistor.Entity.Adapter
open NakedObjects.Persistor.Entity.Component
open System
open System.Collections.Generic
open System.Reflection

// connection strings 

#if APPVEYOR 

let csAW = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=AdventureWorks;Integrated Security=True;"
let csAWMARS = "Data source=(LocalDB)\MSSQLLocalDB;initial catalog=AdventureWorks;integrated security=True;MultipleActiveResultSets=True;"
let csMD = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=AMultiDatabaseTests;Integrated Security=True;"
let csMF = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ModelFirst;Integrated Security=True;"
let csCO = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=CodeOnlyTests;Integrated Security=True;"
let csCOCE = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=CodeOnlyCeTests;Integrated Security=True;"
let csCS = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=CodeSystemTest;Integrated Security=True;"

#else

let csAW = "Data Source=.\SQLEXPRESS;Initial Catalog=AdventureWorks;Integrated Security=True;"
let csAWMARS = "Data source=.\SQLEXPRESS;initial catalog=AdventureWorks;integrated security=True;MultipleActiveResultSets=True;"
let csMD = "Data Source=.\SQLEXPRESS;Initial Catalog=AMultiDatabaseTests;Integrated Security=True;"
let csMF = "Data Source=.\SQLEXPRESS;Initial Catalog=ModelFirst;Integrated Security=True;"
let csCO = "Data Source=.\SQLEXPRESS;Initial Catalog=CodeOnlyTests;Integrated Security=True;"
let csCOCE = "Data Source=.\SQLEXPRESS;Initial Catalog=CodeOnlyCeTests;Integrated Security=True;"
let csCS = "Data Source=.\SQLEXPRESS;Initial Catalog=CodeSystemTest;Integrated Security=True;"

#endif


let injectedObjects = new List<Object>()
let mockInjector = new Mock<IDomainObjectInjector>()
let testInjector = mockInjector.Object

mockInjector.Setup(fun x -> x.InjectInto(It.IsAny<obj>())).Callback<obj> (fun o -> injectedObjects.Add o) |> ignore

let mockNakedObjectSpecification = new Mock<IObjectSpec>()
let testNakedObjectSpecification = mockNakedObjectSpecification.Object

mockNakedObjectSpecification.Setup(fun x -> x.ContainsFacet()).Returns(false) |> ignore
mockNakedObjectSpecification.Setup(fun x -> x.ContainsFacet(null)).Returns(false) |> ignore

let mockMetamodelManager = new Mock<IMetamodelManager>()
let objects = new Dictionary<Object, INakedObjectAdapter>()

let mutable updatedCount = 0
let mutable updatingCount = 0
let mutable persistedCount = 0
let mutable persistingCount = 0

let AddAdapter (ob : obj) oid = 
    let mockNakedObject = new Mock<INakedObjectAdapter>()
    let testNakedObject = mockNakedObject.Object
    let dobj = ob
    
    let eoid : IOid = 
        if oid = null then ((box (new EntityOid(mockMetamodelManager.Object, ob.GetType(), [| box 0 |], true))) :?> IOid)
        else oid
    
    let rsm = new ResolveStateMachine(testNakedObject, null)
    mockNakedObject.Setup(fun no -> no.Object).Returns(dobj) |> ignore
    mockNakedObject.Setup(fun no -> no.Oid).Returns(eoid) |> ignore
    mockNakedObject.Setup(fun no -> no.ResolveState).Returns(rsm) |> ignore
    mockNakedObject.Setup(fun no -> no.Updating()).Callback(fun () -> updatingCount <- updatingCount + 1) |> ignore
    mockNakedObject.Setup(fun no -> no.Updated()).Callback(fun () -> updatedCount <- updatedCount + 1) |> ignore
    mockNakedObject.Setup(fun no -> no.Persisting()).Callback(fun () -> persistingCount <- persistingCount + 1) |> ignore
    mockNakedObject.Setup(fun no -> no.Persisted()).Callback(fun () -> persistedCount <- persistedCount + 1) |> ignore

    match oid with
    | null -> testNakedObject.ResolveState.Handle Events.InitializeTransientEvent
    | _ -> testNakedObject.ResolveState.Handle Events.InitializePersistentEvent
    objects.Add(ob, testNakedObject)
    testNakedObject

let GetOrAddAdapterForTest obj oid = 
    if objects.ContainsKey(obj) then 
        let adapter = objects.[obj]
        adapter
    else 
        let adapter = AddAdapter obj oid
        adapter

let AdapterForTest (oid : IOid) obj = GetOrAddAdapterForTest obj oid
let TransientAdapterForTest obj = GetOrAddAdapterForTest obj null
let ReplacePocoForTest (nakedObject : INakedObjectAdapter) (o : Object) = ()
let RemoveAdapterForTest(nakedObject : INakedObjectAdapter) = ()
let AggregateAdapterForTest (nakedObject : INakedObjectAdapter) (prop : PropertyInfo) (obj : Object) : INakedObjectAdapter = GetOrAddAdapterForTest obj null
let NotifyUIForTest(nakedObject : INakedObjectAdapter) = ()
let loadSpecificationHandler (t : Type) : IObjectSpec = testNakedObjectSpecification


let handleLoadingTest (nakedObject : INakedObjectAdapter) = 
    if nakedObject.ResolveState.IsPartResolving() then nakedObject.ResolveState.Handle Events.EndPartResolvingEvent
    else nakedObject.ResolveState.Handle Events.EndResolvingEvent

let savingChangesHandler (sender : Object) (e : EventArgs) = ()
let mutable setProxyingAndDeferredLoading = true

let setupPersistorForTesting (p : EntityObjectStore) = 
    p.SetupForTesting
        (testInjector, EntityObjectStore.CreateAdapterDelegate(AdapterForTest), EntityObjectStore.ReplacePocoDelegate(ReplacePocoForTest), 
         EntityObjectStore.RemoveAdapterDelegate(RemoveAdapterForTest), EntityObjectStore.CreateAggregatedAdapterDelegate(AggregateAdapterForTest), 
         Action<INakedObjectAdapter>(handleLoadingTest), EventHandler(savingChangesHandler), Func<Type, IObjectSpec>(loadSpecificationHandler))
    p.SetupContexts()
    p.SetProxyingAndDeferredLoading setProxyingAndDeferredLoading
    p
