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

let getNo (obj : obj) (ctx : INakedObjectsFramework) = 
    match obj with
    | :? INakedObject as no -> no
    | _ -> ctx.LifecycleManager.CreateAdapter(obj, null, null)

let IsPersistentObject obj ctx = 
    let no = getNo obj ctx
    Assert.IsNotNull(no)
    Assert.IsTrue(no.ResolveState |> StateHelperUtils.IsPersistent)

let IsPersistentOid obj ctx = 
    let oid = (getNo obj ctx).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<EntityOid>, oid)
    Assert.IsFalse(oid.IsTransient)

let IsTransientObject obj ctx = 
    let no = getNo obj ctx
    Assert.IsNotNull(no)
    Assert.IsTrue(no.ResolveState |> StateHelperUtils.IsTransient)

let IsTransientOid obj ctx = 
    let oid = (getNo obj ctx).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<EntityOid>, oid)
    Assert.IsTrue(oid.IsTransient)

let IsPersistentAggregateOid obj ctx = 
    let oid = (getNo obj ctx).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<AggregateOid>, oid)
    Assert.IsFalse(oid.IsTransient)

let IsTransientAggregateOid obj ctx = 
    let oid = (getNo obj ctx).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<AggregateOid>, oid)
    Assert.IsTrue(oid.IsTransient)

let IsNotNullAndPersistent obj ctx = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj ctx
    IsPersistentOid obj ctx

let IsNotNullAndTransient obj ctx = 
    Assert.IsNotNull(obj)
    IsTransientObject obj ctx
    IsTransientOid obj ctx

let IsNotNullAndPersistentAggregate obj ctx = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj ctx
    IsPersistentAggregateOid obj ctx

let IsNotNullAndTransientAggregate obj ctx = 
    Assert.IsNotNull(obj)
    IsPersistentObject obj ctx
    IsTransientAggregateOid obj ctx

let Create<'t when 't : not struct>(ctx : INakedObjectsFramework) = 
    let spec = ctx.Metadata.GetSpecification(typeof<'t>)
    ctx.LifecycleManager.CreateInstance(spec)

let CreateAndSetup<'t when 't : not struct> setter ctx = 
    let no = Create<'t>(ctx)
    let inst = box (no.Object) :?> 't
    setter inst
    no

let makeAndSaveChanges change (ctx : INakedObjectsFramework) = 
    ctx.LifecycleManager.StartTransaction()
    change()
    ctx.LifecycleManager.EndTransaction()

let save no (ctx : INakedObjectsFramework) = 
    let saveNo() = no |> ctx.LifecycleManager.MakePersistent
    makeAndSaveChanges saveNo ctx