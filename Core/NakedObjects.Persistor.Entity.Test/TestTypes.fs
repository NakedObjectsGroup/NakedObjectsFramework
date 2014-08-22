// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.TestTypes

open NUnit.Framework
open NakedObjects.EntityObjectStore
open NakedObjects.Architecture.Util
open NakedObjects.Architecture.Reflect
open NakedObjects.Architecture.Spec
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Facets.Actcoll.Typeof
open System
open NakedObjects.Architecture.Adapter
open System.Collections.Generic
open NakedObjects.Architecture.Persist
open System.Reflection
open NakedObjects.Persistor
open NakedObjects.Architecture.Facets
open NakedObjects.Architecture.Security
open NakedObjects.Core.Context

let injectedObjects = new List<Object>()

type MockReflector() = 
    interface INakedObjectReflector with 
        member x.AllSpecifications with get() = [||]
        member x.IgnoreCase with get() = false
        member x.Init() = ()
        member x.Shutdown() = ()
        member x.LoadSpecification(typ : Type) : INakedObjectSpecification = null
        member x.LoadSpecification(str : string) : INakedObjectSpecification = null
        member x.InstallServiceSpecifications(types : Type[]) = ()
        member x.PopulateContributedActions(services : INakedObject[]) = ()
        member x.NonSystemServices with get() = [||]
                                   and set(arr) = ()
      


type MockInjector() = 
    interface IContainerInjector with 
        member x.InitDomainObject obj = 
            injectedObjects.Add obj
        member x.InitInlineObject(root : Object, inlineObject : Object) = ()

type MockNakedObjectSpecification() = 
    interface INakedObjectSpecification with 
        member x.FullName with get() = ""
        member x.PluralName with get() = ""
        member x.ShortName with get() = ""
        member x.Description with get() = ""
        member x.SingularName with get() = ""
        member x.UntitledName with get() = ""    
        member x.IsParseable with get() = false
        member x.IsEncodeable with get() = false
        member x.IsAggregated with get() = false
        member x.IsCollection with get() = false
        member x.IsObject with get() = false
        member x.IsAbstract with get() = false
        member x.IsInterface with get() = false
        member x.IsService with get() = false
        member x.HasNoIdentity with get() = false
        member x.IsQueryable with get() = false
        member x.IsVoid with get() = false
        member x.IsASet with get() = false
        member x.IsViewModel with get() = false
        member x.GetIconName(forObject : INakedObject ) = ""
        member x.GetTitle(nakedObject : INakedObject, m ) = ""
        member x.ValidToPersist(transientObject : INakedObject, sess : ISession )  : IConsent = null
        member x.Persistable with get() : Persistable = null
        member x.CreateObject(persistor : INakedObjectPersistor) : obj = null   
        member x.GetBoundedSet(persistor : INakedObjectPersistor) : System.Collections.IEnumerable = null
        member x.MarkAsService() = ()
        member x.GetInvariantString(nakedObject : INakedObject ) = ""
       
    interface IActionContainer with 
       
        member x.GetObjectActions() : INakedObjectAction[] = null
        member x.GetRelatedServiceActions() : INakedObjectAction[] = null
       
    interface IPropertyContainer with 
        member x.GetProperty (id : string) : INakedObjectAssociation = null        
        member x.Properties with get() : INakedObjectAssociation[] = null 
        member x.ValidateMethods() : INakedObjectValidation[] = null
    interface IHierarchical with 
        member x.AddSubclass (specification : INakedObjectSpecification) = ()
        member x.HasSubclasses with get() : bool = false
        member x.Interfaces with get() : INakedObjectSpecification[] = null 
        member x.IsOfType (specification : INakedObjectSpecification) = false
        member x.Subclasses with get() : INakedObjectSpecification[] = null 
        member x.Superclass with get() : INakedObjectSpecification = null 
    interface IDefaultProvider with 
        member x.DefaultValue with get() : obj = null
    interface IFacetHolder with
        member x.AddFacet (facet : IFacet) = ( Assert.IsInstanceOf<Objects.Aggregated.IComplexTypeFacet>(facet)   )  // tests adition of complextype facet 
        member x.AddFacet (facet : IMultiTypedFacet) = ()
        member x.ContainsFacet() : bool = false
        member x.ContainsFacet(typ : Type) : bool = false  // leave false so complextype facet will be added 
        member x.FacetTypes with get() : Type[] = null
        member x.GetFacet (typ : Type) : IFacet = null
        member x.GetFacet<'T when 'T :> IFacet>() : 'T when 'T :> IFacet  = ( box(null) :?> 'T  )
        member x.GetFacets (filter : IFacetFilter) : IFacet[] = null
        member x.Identifier with get() : IIdentifier = null
        member x.RemoveFacet (facet : IFacet) = ()
        member x.RemoveFacet (facetType : Type) = ()

type MockNakedObject(obj, oid) =
    let domainObject = obj 
    let mutable rsm : ResolveStateMachine = null
    let mutable eoid : IOid = oid
    interface INakedObject with
        member x.Object with get() = domainObject
        member x.Specification with get() : INakedObjectSpecification = null  
        member x.Oid 
            with get() : IOid =
                match eoid with 
                | null -> eoid <- ((box (new EntityOid(new MockReflector(), obj.GetType(), [|box 0|], true))) :?> IOid)
                | _ -> ()
                eoid         
        member x.ResolveState 
            with get() : ResolveStateMachine = 
                match rsm with 
                | null -> rsm <- new ResolveStateMachine(x, null, null)
                | _ -> ()          
                rsm 
        member x.CheckLock (v : IVersion) = ()
        member x.Version with get() : IVersion = null
        member x.OptimisticLock with set(v : IVersion) = ()
        member x.TypeOfFacet with get() : ITypeOfFacet  = null
                             and set(tof : ITypeOfFacet) = ()
        member x.IconName() : string = ""
        member x.TitleString() : string = ""
        member x.InvariantString() : string = ""
        member x.ReplacePoco(obj : Object) = ()
        member x.ValidToPersist() : string = null
        member x.SetATransientOid (oid : IOid) = ()

type MockPersistedObjectAdder(p : EntityObjectStore) = 
    let persistor = p
    interface IPersistedObjectAdder with 
        member x.AddPersistedObject(nakedObject : INakedObject) =
            let cmd = persistor.CreateCreateObjectCommand(nakedObject, null, null)
            ()
        member x.MadePersistent(nakedObject : INakedObject) =            
            nakedObject.ResolveState.Handle Events.StartResolvingEvent
            nakedObject.ResolveState.Handle Events.EndResolvingEvent
            


let objects = new Dictionary<Object, INakedObject>()
    
let AddAdapter obj  oid =
    let adapter = (box (new MockNakedObject(obj, oid))) :?> INakedObject
    match oid with 
    | null ->  adapter.ResolveState.Handle Events.InitializeTransientEvent
    | _ -> adapter.ResolveState.Handle Events.InitializePersistentEvent      
    objects.Add(obj, adapter)
    adapter
 
let GetOrAddAdapterForTest obj  oid =
    if objects.ContainsKey(obj) then
        let adapter = objects.[obj]
        adapter
    else 
        let adapter = AddAdapter obj oid 
        adapter
        
let AdapterForTest (oid : IOid) obj  = GetOrAddAdapterForTest obj oid 

let TransientAdapterForTest obj  = GetOrAddAdapterForTest obj null 
 
let ReplacePocoForTest (nakedObject : INakedObject) (o : Object) = ()
 
let RemoveAdapterForTest (nakedObject : INakedObject) = ()
 
let AggregateAdapterForTest (nakedObject : INakedObject) (prop : PropertyInfo) (obj : Object) : INakedObject = 
    GetOrAddAdapterForTest obj null    
 
let NotifyUIForTest (nakedObject : INakedObject) = ()

let loadSpecificationHandler (t : Type) : INakedObjectSpecification  = upcast new MockNakedObjectSpecification()

let mutable updatedCount = 0
let mutable updatingCount = 0
let mutable persistedCount = 0
let mutable persistingCount = 0

 
let updated (nakedObject : INakedObject) (sess : ISession) (p : INakedObjectPersistor) = 
    updatedCount <- updatedCount + 1
    ()
 
let updating (nakedObject : INakedObject) (sess : ISession) (p : INakedObjectPersistor) = 
    updatingCount <- updatingCount + 1
    ()
 
let persisted (nakedObject : INakedObject) (sess : ISession) (p : INakedObjectPersistor) = 
    persistedCount <- persistedCount + 1
    ()
 
let persisting (nakedObject : INakedObject) (sess : ISession) (p : INakedObjectPersistor) = 
    persistingCount <- persistingCount + 1
    ()

let handleLoadingTest(nakedObject : INakedObject) = 
   if nakedObject.ResolveState.IsPartResolving() then 
      nakedObject.ResolveState.Handle Events.EndPartResolvingEvent
   else 
      nakedObject.ResolveState.Handle Events.EndResolvingEvent
   
let savingChangesHandler (sender: Object) (e : EventArgs) = ()

let mutable setProxyingAndDeferredLoading = true

let setupPersistorForTesting (p : EntityObjectStore) = 
    p.SetupForTesting(new MockInjector(),
                      EntityObjectStore.CreateAdapterDelegate(AdapterForTest),
                      EntityObjectStore.ReplacePocoDelegate(ReplacePocoForTest), 
                      EntityObjectStore.RemoveAdapterDelegate(RemoveAdapterForTest), 
                      EntityObjectStore.CreateAggregatedAdapterDelegate(AggregateAdapterForTest), 
                      EntityObjectStore.NotifyUiDelegate(NotifyUIForTest),
                      Action<INakedObject, ISession, INakedObjectPersistor>(updated), 
                      Action<INakedObject, ISession, INakedObjectPersistor>(updating),
                      Action<INakedObject, ISession, INakedObjectPersistor>(persisted), 
                      Action<INakedObject, ISession, INakedObjectPersistor>(persisting),
                      Action<INakedObject>(handleLoadingTest),
                      EventHandler(savingChangesHandler),
                      Func<Type, INakedObjectSpecification>(loadSpecificationHandler))
    p.Reset()
    p.SetProxyingAndDeferredLoading setProxyingAndDeferredLoading
    p

