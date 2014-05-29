// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.SystemTestCode
open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open NakedObjects.Services
open System
open NakedObjects.EntityObjectStore
open NakedObjects.Core.Context
open NakedObjects.Core.Persist
open NakedObjects.Architecture.Resolve
open NakedObjects.Architecture.Persist
open NakedObjects.Architecture.Adapter

let getNo (obj : obj) = 
    match obj with 
    | :? INakedObject as no -> no
    | _ -> NakedObjectsContext.ObjectPersistor.CreateAdapter(obj, null, null)

let IsPersistentObject obj = 
    let no = getNo obj
    Assert.IsNotNull(no)
    Assert.IsTrue(no.ResolveState |> StateHelperUtils.IsPersistent)

let IsPersistentOid obj =     
    let oid = (getNo obj).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<EntityOid>, oid)
    Assert.IsFalse(oid.IsTransient)

let IsTransientObject obj = 
    let no = getNo obj
    Assert.IsNotNull(no)
    Assert.IsTrue(no.ResolveState |> StateHelperUtils.IsTransient)

let IsTransientOid obj =     
    let oid = (getNo obj).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<EntityOid>, oid)
    Assert.IsTrue(oid.IsTransient)

let IsPersistentAggregateOid obj =     
    let oid = (getNo obj).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<AggregateOid>, oid)
    Assert.IsFalse(oid.IsTransient)

let IsTransientAggregateOid obj =     
    let oid = (getNo obj).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<AggregateOid>, oid)
    Assert.IsTrue(oid.IsTransient)

let IsNotNullAndPersistent obj = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj
    IsPersistentOid obj

let IsNotNullAndTransient obj = 
    Assert.IsNotNull(obj)
    IsTransientObject obj
    IsTransientOid obj

let IsNotNullAndPersistentAggregate obj = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj
    IsPersistentAggregateOid obj

let IsNotNullAndTransientAggregate obj = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj
    IsTransientAggregateOid obj

let Create<'t when 't : not struct>() =  
    let spec = NakedObjectsContext.Reflector.LoadSpecification(typeof<'t>)
    NakedObjectsContext.ObjectPersistor.CreateInstance(spec)

let CreateAndSetup<'t when 't : not struct> setter =  
    let no = Create<'t>()
    let inst = box (no.Object) :?> 't
    setter inst
    no
    
let makeAndSaveChanges change = 
    NakedObjectsContext.ObjectPersistor.StartTransaction()
    change()
    NakedObjectsContext.ObjectPersistor.EndTransaction()    
        
let save no =
    let saveNo() =  no |> NakedObjectsContext.ObjectPersistor.MakePersistent
    makeAndSaveChanges saveNo