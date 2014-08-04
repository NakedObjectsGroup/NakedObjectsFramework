// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.TestCode
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
open TestTypes
open System.Linq
open System.Collections
open NakedObjects.Architecture.Util
open Microsoft.FSharp.Linq
open NakedObjects.Core.Context



let resetPersistor (p : EntityObjectStore) =
    p.Reset()
    setupPersistorForTesting p

let CreateAndSetup<'t when 't : not struct> (p : EntityObjectStore) setter =  
    let inst = p.CreateInstance<'t>()
    setter inst
    let n1 = GetOrAddAdapterForTest inst null
    inst

let uniqueName() = Guid.NewGuid().ToString()

let CreateAndEndTransaction  (p : EntityObjectStore) o =
    let cmd = p.CreateCreateObjectCommand(GetOrAddAdapterForTest o null)
    p.EndTransaction()

let SaveAndEndTransaction  (p : EntityObjectStore) o =
    let cmd = p.CreateSaveObjectCommand(GetOrAddAdapterForTest o null)
    p.EndTransaction()

let SaveWithNoEndTransaction  (p : EntityObjectStore) o =
    let cmd = p.CreateSaveObjectCommand(GetOrAddAdapterForTest o null)
    ()

let GetInstancesGenericNotEmpty<'t when 't : not struct> (p: EntityObjectStore)  =
    let count =   p.GetInstances<'t>()  |> Seq.length
    //let count =   p.GetInstances<'t>() |> Seq.length 
    Assert.Greater(count, 0) 
 
let GetInstancesByTypeNotEmpty<'t when 't : not struct> (p: EntityObjectStore)  =
    let count = Seq.cast<'t>(p.GetInstances(typeof<'t>)) |> Seq.length
    Assert.Greater(count, 0)
 
let GetInstancesReturnsProxies<'t when 't : not struct> (p: EntityObjectStore)  =
    let instances = p.GetInstances<'t>()
    Assert.IsTrue(instances |> Seq.forall (fun i ->  EntityUtils.IsEntityProxy(i.GetType())))

let GetInstancesDoesntReturnProxies<'t when 't : not struct> (p: EntityObjectStore)  =
    let instances = p.GetInstances<'t>()
    Assert.IsFalse(instances |> Seq.forall (fun i ->  EntityUtils.IsEntityProxy(i.GetType())))
    
let CanCreateTransientObject<'t when 't : not struct> (p: EntityObjectStore)  =
     let transientInstance = p.CreateInstance<'t>()
     Assert.IsNotNull(transientInstance)
     Assert.IsFalse(EntityUtils.IsEntityProxy(transientInstance.GetType()))
    
let CanSaveTransientObject<'t when 't : not struct> p setter =  
     let sr = CreateAndSetup<'t> p setter
     CreateAndEndTransaction p sr
   
let checkCountAndType classes  (typ : Type)  =
    Assert.Greater(classes |> Seq.length, 0)
    Assert.IsTrue(classes |> Seq.forall (fun i -> typ.IsAssignableFrom(i.GetType())))

let First<'t when 't : not struct> (p : EntityObjectStore) = 
   p.GetInstances<'t>() |> Seq.head 
   //p.GetInstances<'t>() |> Seq.head

let Second<'t when 't : not struct> (p : EntityObjectStore) = 
    p.GetInstances<'t>() |> Seq.nth 1
    
let GetMaxID<'t when 't : not struct> (p : EntityObjectStore) fGetID  = 
   (p.GetInstances<'t>() |> Seq.map fGetID |> Seq.max) + 0
      
let GetNextID<'t when 't : not struct> (p : EntityObjectStore) fGetID  = 
    (GetMaxID<'t> p fGetID)  + 1

let CanGetObjectByKey<'t when 't : not struct>  (p : EntityObjectStore) keys = 
    let key = new EntityOid(new MockReflector(), typeof<'t>, keys, false) 
    let obj = p.GetObjectByKey(key, typeof<'t>) 
    Assert.IsNotNull(obj)
    
let CanGetContextForCollection<'t when 't : not struct> (persistor : EntityObjectStore) =
    let testCollection = new List<'t>()
    persistor.LoadComplexTypes (GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForNonGenericCollection<'t when 't : not struct> (persistor : EntityObjectStore) =
    let testCollection = new ArrayList()
    let header = persistor.GetInstances<'t>() |> Seq.head 
    let i = testCollection.Add(header)
    persistor.LoadComplexTypes (GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForArray<'t when 't : not struct> (persistor : EntityObjectStore) =
    let header = persistor.GetInstances<'t>() |> Seq.head 
    persistor.LoadComplexTypes (GetOrAddAdapterForTest [|header|] null, false)

let CanGetContextForType<'t when 't : not struct> (persistor : EntityObjectStore) =
    let test = persistor.CreateInstance<'t>()
    persistor.LoadComplexTypes (GetOrAddAdapterForTest test null, false)
