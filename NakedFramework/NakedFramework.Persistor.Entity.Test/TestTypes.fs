// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.TestTypes

open Moq
open NakedFramework.Architecture.Adapter
open NakedFramework.Architecture.Component
open NakedFramework.Architecture.Spec
open NakedFramework.Core.Resolve
open NakedFramework.Persistor.Entity.Component
open System
open System.Collections.Generic
open System.Reflection
open Microsoft.Extensions.Logging
open NakedFramework.Persistor.EFCore.Component
open NakedFramework.Core.Persist

let appveyorServer = @"Data Source=(local)\SQL2017;"
let localServer =  @"Data Source=(localdb)\MSSQLLocalDB;"

#if APPVEYOR 
let server = appveyorServer
#else
let server = localServer
#endif

let csAW = server + @"Initial Catalog=AdventureWorks;Integrated Security=True;"
let csAWMARS = server + @"initial catalog=AdventureWorks;integrated security=True;MultipleActiveResultSets=True;"
let csMD = server + @"Initial Catalog=AMultiDatabaseTests;Integrated Security=True;"
let csMF = server + @"Initial Catalog=ModelFirst;Integrated Security=True;"
let csCO = server + @"Initial Catalog=CodeOnlyTests;Integrated Security=True;"
let csEFCO = server + @"Initial Catalog=EFCodeOnlyTests;Integrated Security=True;"
let csCOCE = server + @"Initial Catalog=CodeOnlyCeTests;Integrated Security=True;"
let csCS = server + @"Initial Catalog=CodeSystemTest;Integrated Security=True;"
let csTDCO = server + @"Initial Catalog=TestDataCodeOnly;Integrated Security=True;"

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
let mockLogger = new Mock<ILogger<ResolveStateMachine>>();

let mutable updatedCount = 0
let mutable updatingCount = 0
let mutable persistedCount = 0
let mutable persistingCount = 0

let AddAdapter (ob : obj) oid = 
    let mockNakedObject = new Mock<INakedObjectAdapter>()
    let testNakedObject = mockNakedObject.Object
    let mockLogger = new Mock<ILogger<EntityOid>>();
    let testLogger  = mockLogger.Object;
    let dobj = ob
    
    let eoid : IOid = 
        if oid = null then ((box (new EntityOid(mockMetamodelManager.Object, ob.GetType(), [| box 0 |], true, testLogger))) :?> IOid)
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
        (testInjector, 
         Func<IOid, obj, INakedObjectAdapter> AdapterForTest, 
         Action<INakedObjectAdapter, obj>  ReplacePocoForTest, 
         Action<INakedObjectAdapter>  RemoveAdapterForTest, 
         Func<INakedObjectAdapter, PropertyInfo, obj, INakedObjectAdapter> AggregateAdapterForTest, 
         Action<INakedObjectAdapter> handleLoadingTest, 
         Action<obj, EventArgs> savingChangesHandler, 
         Func<Type, IObjectSpec> loadSpecificationHandler)
    p.SetupContexts()
    p.SetProxyingAndDeferredLoading setProxyingAndDeferredLoading
    p

let setupEFCorePersistorForTesting (p : EFCoreObjectStore) = 
    p.SetupForTesting
        (testInjector, 
         Func<IOid, obj, INakedObjectAdapter> AdapterForTest, 
         Action<INakedObjectAdapter, obj>  ReplacePocoForTest, 
         Action<INakedObjectAdapter>  RemoveAdapterForTest, 
         Func<INakedObjectAdapter, PropertyInfo, obj, INakedObjectAdapter> AggregateAdapterForTest, 
         Action<INakedObjectAdapter> handleLoadingTest, 
         Action<obj> (fun (o) -> ()), 
         Func<Type, IObjectSpec> loadSpecificationHandler)
    p.SetupContexts()
    //p.SetProxyingAndDeferredLoading setProxyingAndDeferredLoading
    p