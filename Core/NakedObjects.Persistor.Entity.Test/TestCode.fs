// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.TestCode

open NUnit.Framework

open Moq
open NakedObjects.Architecture.Component
open NakedObjects.Core.Component
open NakedObjects.Core.Configuration
open NakedObjects.Core.Authentication
open NakedObjects.Persistor.Entity.Util
open NakedObjects.Persistor.Entity.Adapter
open NakedObjects.Persistor.Entity.Component
open System
open System.Collections.Generic
open System.Collections
open System.Security.Principal
open TestTypes
open Microsoft.Extensions.Logging

let resetPersistor (p : EntityObjectStore) = 
    p.SetupContexts()
    setupPersistorForTesting p

let getEntityObjectStore (config) = 
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    ReflectorConfiguration.NoValidate <- true
    let c = new ReflectorConfiguration( [||], [||],[||])
    let mlf = new Mock<ILoggerFactory>();
    let ml = new Mock<ILogger<DomainObjectContainerInjector>>();
    let i = new DomainObjectContainerInjector(c, mlf.Object, ml.Object)
    let m = mockMetamodelManager.Object
    let nom = (new Mock<INakedObjectManager>()).Object
    new EntityObjectStore(s, config, new EntityOidGenerator(m, mlf.Object), m, i, nom)

let CreateAndSetup<'t when 't : not struct> (p : EntityObjectStore) setter = 
    let inst = p.CreateInstance<'t>(null)
    setter inst
    GetOrAddAdapterForTest inst null |> ignore
    inst

let uniqueName() = Guid.NewGuid().ToString()

let CreateAndEndTransaction (p : EntityObjectStore) o = 
    p.ExecuteCreateObjectCommand(GetOrAddAdapterForTest o null) 
    p.EndTransaction()

let SaveAndEndTransaction (p : EntityObjectStore) o = 
    p.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o null) 
    p.EndTransaction()

let SaveWithNoEndTransaction (p : EntityObjectStore) o = 
    p.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o null) 
    ()

let GetInstancesGenericNotEmpty<'t when 't : not struct>(p : EntityObjectStore) = 
    let count = p.GetInstances<'t>() |> Seq.length
    Assert.Greater(count, 0)

let GetInstancesByTypeNotEmpty<'t when 't : not struct>(p : EntityObjectStore) = 
    let count = Seq.cast<'t> (p.GetInstances(typeof<'t>)) |> Seq.length
    Assert.Greater(count, 0)

let GetInstancesReturnsProxies<'t when 't : not struct>(p : EntityObjectStore) = 
    let instances = p.GetInstances<'t>()
    Assert.IsTrue(instances |> Seq.forall (fun i -> EntityUtils.IsEntityProxy(i.GetType())))

let GetInstancesDoesntReturnProxies<'t when 't : not struct>(p : EntityObjectStore) = 
    let instances = p.GetInstances<'t>()
    Assert.IsFalse(instances |> Seq.forall (fun i -> EntityUtils.IsEntityProxy(i.GetType())))

let CanCreateTransientObject<'t when 't : not struct>(p : EntityObjectStore) = 
    let transientInstance = p.CreateInstance<'t>(null)
    Assert.IsNotNull(transientInstance)
    Assert.IsFalse(EntityUtils.IsEntityProxy(transientInstance.GetType()))

let CanSaveTransientObject<'t when 't : not struct> p setter = 
    let sr = CreateAndSetup<'t> p setter
    CreateAndEndTransaction p sr

let checkCountAndType classes (typ : Type) = 
    Assert.Greater(classes |> Seq.length, 0)
    Assert.IsTrue(classes |> Seq.forall (fun i -> typ.IsAssignableFrom(i.GetType())))

let First<'t when 't : not struct>(p : EntityObjectStore) = p.GetInstances<'t>() |> Seq.head
let Second<'t when 't : not struct>(p : EntityObjectStore) = p.GetInstances<'t>() |> Seq.item 1

let GetMaxID<'t when 't : not struct> (p : EntityObjectStore) fGetID = 
    (p.GetInstances<'t>()
     |> Seq.map fGetID
     |> Seq.max)
    + 0

let GetNextID<'t when 't : not struct> (p : EntityObjectStore) fGetID = (GetMaxID<'t> p fGetID) + 1

let CanGetObjectByKey<'t when 't : not struct> (p : EntityObjectStore) keys = 
    let key = new EntityOid(mockMetamodelManager.Object, typeof<'t>, keys, false)
    let obj = p.GetObjectByKey(key, typeof<'t>)
    Assert.IsNotNull(obj)

let CanGetContextForCollection<'t when 't : not struct>(persistor : EntityObjectStore) = 
    let testCollection = new List<'t>()
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForNonGenericCollection<'t when 't : not struct>(persistor : EntityObjectStore) = 
    let testCollection = new ArrayList()
    let header = persistor.GetInstances<'t>() |> Seq.head
    testCollection.Add(header) |> ignore
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForArray<'t when 't : not struct>(persistor : EntityObjectStore) = 
    let header = persistor.GetInstances<'t>() |> Seq.head
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest [| header |] null, false)

let CanGetContextForType<'t when 't : not struct>(persistor : EntityObjectStore) = 
    let test = persistor.CreateInstance<'t>(null)
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest test null, false)
