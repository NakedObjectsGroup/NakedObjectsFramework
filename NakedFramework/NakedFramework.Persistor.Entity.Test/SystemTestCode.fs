// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.SystemTestCode


open NakedObjects.Architecture.Adapter
open NakedObjects.Core.Resolve
open NakedObjects.Persistor.Entity.Adapter
open NUnit.Framework
open NakedFramework.Architecture.Adapter

let getNo (obj : obj) (ctx : INakedObjectsFramework) = 
    match obj with
    | :? INakedObjectAdapter as no -> no
    | _ -> ctx.NakedObjectManager.CreateAdapter(obj, null, null)

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
    Assert.IsInstanceOf(typeof<IAggregateOid>, oid)
    Assert.IsFalse(oid.IsTransient)

let IsTransientAggregateOid obj ctx = 
    let oid = (getNo obj ctx).Oid
    Assert.IsNotNull(oid)
    Assert.IsInstanceOf(typeof<IAggregateOid>, oid)
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
    let spec = ctx.MetamodelManager.GetSpecification(typeof<'t>) :?> NakedObjects.Architecture.Spec.IObjectSpec
    ctx.LifecycleManager.CreateInstance(spec)

let CreateAndSetup<'t when 't : not struct> setter ctx = 
    let no = Create<'t>(ctx)
    let inst = box (no.Object) :?> 't
    setter inst
    no

let makeAndSaveChanges change (ctx : INakedObjectsFramework) = 
    ctx.TransactionManager.StartTransaction()
    change()
    ctx.TransactionManager.EndTransaction()

let save no (ctx : INakedObjectsFramework) = 
    let saveNo() = no |> ctx.LifecycleManager.MakePersistent
    makeAndSaveChanges saveNo ctx