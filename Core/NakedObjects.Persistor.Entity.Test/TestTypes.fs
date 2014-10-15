// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.TestTypes

open NUnit.Framework
open NakedObjects.EntityObjectStore
open NakedObjects.Architecture.Component
open NakedObjects.Architecture.Spec
open NakedObjects.Architecture.Resolve
open System
open NakedObjects.Architecture.Adapter
open System.Collections.Generic
open System.Reflection
open NakedObjects.Architecture.Facets
open NakedObjects.Architecture.Security
open NakedObjects.Core.Reflect
open Moq

let injectedObjects = new List<Object>()
let mockInjector = new Mock<IContainerInjector>()
let testInjector = mockInjector.Object

mockInjector.Setup(fun x -> x.InitDomainObject(It.IsAny<obj>())).Callback<obj> (fun o -> injectedObjects.Add o) |> ignore

let mockNakedObjectSpecification = new Mock<INakedObjectSpecification>()
let testNakedObjectSpecification = mockNakedObjectSpecification.Object

mockNakedObjectSpecification.Setup(fun x -> x.ContainsFacet()).Returns(false) |> ignore
mockNakedObjectSpecification.Setup(fun x -> x.ContainsFacet(null)).Returns(false) |> ignore
mockNakedObjectSpecification.Setup(fun x -> x.AddFacet(It.IsAny<IFacet>())).Callback<IFacet> 
    (fun f -> Assert.IsInstanceOf<Objects.Aggregated.IComplexTypeFacet>(f)) |> ignore

let mockMetamodelManager = new Mock<IMetamodelManager>()
let objects = new Dictionary<Object, INakedObject>()

let AddAdapter (ob : obj) oid = 
    let mockNakedObject = new Mock<INakedObject>()
    let testNakedObject = mockNakedObject.Object
    let dobj = ob
    
    let eoid : IOid = 
        if oid = null then ((box (new EntityOid(mockMetamodelManager.Object, ob.GetType(), [| box 0 |], true))) :?> IOid)
        else oid
    
    let rsm = new ResolveStateMachine(testNakedObject, null)
    mockNakedObject.Setup(fun no -> no.Object).Returns(dobj) |> ignore
    mockNakedObject.Setup(fun no -> no.Oid).Returns(eoid) |> ignore
    mockNakedObject.Setup(fun no -> no.ResolveState).Returns(rsm) |> ignore
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
let ReplacePocoForTest (nakedObject : INakedObject) (o : Object) = ()
let RemoveAdapterForTest(nakedObject : INakedObject) = ()
let AggregateAdapterForTest (nakedObject : INakedObject) (prop : PropertyInfo) (obj : Object) : INakedObject = GetOrAddAdapterForTest obj null
let NotifyUIForTest(nakedObject : INakedObject) = ()
let loadSpecificationHandler (t : Type) : INakedObjectSpecification = testNakedObjectSpecification
let mutable updatedCount = 0
let mutable updatingCount = 0
let mutable persistedCount = 0
let mutable persistingCount = 0

let updated (nakedObject : INakedObject) (sess : ISession) = 
    updatedCount <- updatedCount + 1
    ()

let updating (nakedObject : INakedObject) (sess : ISession) = 
    updatingCount <- updatingCount + 1
    ()

let persisted (nakedObject : INakedObject) (sess : ISession) = 
    persistedCount <- persistedCount + 1
    ()

let persisting (nakedObject : INakedObject) (sess : ISession) = 
    persistingCount <- persistingCount + 1
    ()

let handleLoadingTest (nakedObject : INakedObject) = 
    if nakedObject.ResolveState.IsPartResolving() then nakedObject.ResolveState.Handle Events.EndPartResolvingEvent
    else nakedObject.ResolveState.Handle Events.EndResolvingEvent

let savingChangesHandler (sender : Object) (e : EventArgs) = ()
let mutable setProxyingAndDeferredLoading = true

let setupPersistorForTesting (p : EntityObjectStore) = 
    p.SetupForTesting
        (testInjector, EntityObjectStore.CreateAdapterDelegate(AdapterForTest), EntityObjectStore.ReplacePocoDelegate(ReplacePocoForTest), 
         EntityObjectStore.RemoveAdapterDelegate(RemoveAdapterForTest), EntityObjectStore.CreateAggregatedAdapterDelegate(AggregateAdapterForTest), 
         EntityObjectStore.NotifyUiDelegate(NotifyUIForTest), Action<INakedObject, ISession>(updated), Action<INakedObject, ISession>(updating), 
         Action<INakedObject, ISession>(persisted), Action<INakedObject, ISession>(persisting), Action<INakedObject>(handleLoadingTest), 
         EventHandler(savingChangesHandler), Func<Type, INakedObjectSpecification>(loadSpecificationHandler))
    p.Reset()
    p.SetProxyingAndDeferredLoading setProxyingAndDeferredLoading
    p
