// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.TestCode

open NUnit.Framework
open Moq
open NakedFramework.Architecture.Component
open NakedFramework.Core.Component
open NakedFramework.Core.Authentication
open NakedFramework.Persistor.EF6.Component
open System
open System.Collections.Generic
open System.Collections
open System.Security.Principal
open Microsoft.Extensions.Logging
open NakedObjects.Reflector.Configuration
open NakedObjects.Core.Component
open NakedFramework.Persistor.EFCore.Component
open NakedFramework.Core.Persist
open NakedFramework.Core.Util
open TestTypes


let resetPersistor (p : IObjectStore) : IObjectStore =
    match p with 
    | :? EF6ObjectStore as eos -> 
        eos.SetupContexts()
        (setupPersistorForTesting eos) :> IObjectStore
    | :? EFCoreObjectStore as ecos -> 
        ecos.SetupContexts()
        (setupEFCorePersistorForTesting ecos) :> IObjectStore
    | _ -> p

let getEntityObjectStore (config) = 
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    ObjectReflectorConfiguration.NoValidate <- true
    let c = new ObjectReflectorConfiguration( [||], [||])
    let serviceList = new AllServiceList ([|  new ServiceList(c.Services)|])
    let mlf = new Mock<ILoggerFactory>();
    let ml = new Mock<ILogger<DomainObjectContainerInjector>>();
    let i = new DomainObjectContainerInjector(serviceList, mlf.Object, ml.Object)
    let m = mockMetamodelManager.Object
    let nom = (new Mock<INakedObjectManager>()).Object
    let log = (new Mock<ILogger<EF6ObjectStore>>()).Object;
    new EF6ObjectStore(s, config, new DatabaseOidGenerator(m, mlf.Object), m, i, nom, log)

let getEFCoreObjectStore (config) = 
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    ObjectReflectorConfiguration.NoValidate <- true
    let c = new ObjectReflectorConfiguration( [||], [||])
    let serviceList = new AllServiceList ([|  new ServiceList(c.Services)|])
    let mlf = new Mock<ILoggerFactory>();
    let ml = new Mock<ILogger<DomainObjectContainerInjector>>();
    let i = new DomainObjectContainerInjector(serviceList, mlf.Object, ml.Object)
    let m = mockMetamodelManager.Object
    let nom = (new Mock<INakedObjectManager>()).Object
    let log = (new Mock<ILogger<EFCoreObjectStore>>()).Object;
    new EFCoreObjectStore(config, new DatabaseOidGenerator(m, mlf.Object), nom,  s, m, i, log)

let CreateAndSetup<'t when 't : not struct> (p : IObjectStore) setter = 
    let inst = p.CreateInstance<'t>(null)
    setter inst
    GetOrAddAdapterForTest inst null |> ignore
    inst

let uniqueName() = Guid.NewGuid().ToString()

let CreateAndEndTransaction (p : IObjectStore) o = 
    p.ExecuteCreateObjectCommand(GetOrAddAdapterForTest o null) 
    p.EndTransaction()

let SaveAndEndTransaction (p : IObjectStore) o = 
    p.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o null) 
    p.EndTransaction()

let SaveWithNoEndTransaction (p : IObjectStore) o = 
    p.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o null) 
    ()

let GetInstancesGenericNotEmpty<'t when 't : not struct>(p : IObjectStore) = 
    let count = p.GetInstances<'t>() |> Seq.length
    Assert.Greater(count, 0)

let GetInstancesByTypeNotEmpty<'t when 't : not struct>(p : IObjectStore) = 
    let count = Seq.cast<'t> (p.GetInstances(typeof<'t>)) |> Seq.length
    Assert.Greater(count, 0)

let GetInstancesReturnsProxies<'t when 't : not struct>(p : IObjectStore) = 
    let instances = p.GetInstances<'t>()
    Assert.IsTrue(instances |> Seq.forall (fun i -> FasterTypeUtils.IsEF6OrCoreProxy(i.GetType())))

let GetInstancesDoesntReturnProxies<'t when 't : not struct>(p : IObjectStore) = 
    let instances = p.GetInstances<'t>()
    Assert.IsFalse(instances |> Seq.forall (fun i -> FasterTypeUtils.IsEF6OrCoreProxy(i.GetType())))

let CanCreateTransientObject<'t when 't : not struct>(p : IObjectStore) = 
    let transientInstance = p.CreateInstance<'t>(null)
    Assert.IsNotNull(transientInstance)
    Assert.IsFalse(FasterTypeUtils.IsEF6Proxy(transientInstance.GetType()))

let CanSaveTransientObject<'t when 't : not struct> p setter = 
    let sr = CreateAndSetup<'t> p setter
    CreateAndEndTransaction p sr

let checkCountAndType classes (typ : Type) = 
    Assert.Greater(classes |> Seq.length, 0)
    Assert.IsTrue(classes |> Seq.forall (fun i -> typ.IsAssignableFrom(i.GetType())))

let First<'t when 't : not struct>(p : IObjectStore) = p.GetInstances<'t>() |> Seq.head
let Second<'t when 't : not struct>(p : IObjectStore) = p.GetInstances<'t>() |> Seq.item 1

let GetMaxID<'t when 't : not struct> (p : IObjectStore) fGetID = 
    (p.GetInstances<'t>()
     |> Seq.map fGetID
     |> Seq.max)
    + 0

let GetNextID<'t when 't : not struct> (p : IObjectStore) fGetID = (GetMaxID<'t> p fGetID) + 1

let getObjectByKey(persistor :IObjectStore) key (typ : Type) = 
    match persistor with 
    | :? EF6ObjectStore as eos -> eos.GetObjectByKey(key, typ)
    | :? EFCoreObjectStore as efos -> efos.GetObjectByKey(key, typ) 
    | _ -> null

let CanGetObjectByKey<'t when 't : not struct> (p : IObjectStore) keys =
    let testLogger = (new Mock<ILogger<DatabaseOid>>()).Object;
    let key = new DatabaseOid(mockMetamodelManager.Object, typeof<'t>, keys, false, testLogger)
    let obj = getObjectByKey p key typeof<'t>
    Assert.IsNotNull(obj)

let CanGetContextForCollection<'t when 't : not struct>(persistor : IObjectStore) = 
    let testCollection = new List<'t>()
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForNonGenericCollection<'t when 't : not struct>(persistor : IObjectStore) = 
    let testCollection = new ArrayList()
    let header = persistor.GetInstances<'t>() |> Seq.head
    testCollection.Add(header) |> ignore
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest testCollection null, false)

let CanGetContextForArray<'t when 't : not struct>(persistor : IObjectStore) = 
    let header = persistor.GetInstances<'t>() |> Seq.head
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest [| header |] null, false)

let CanGetContextForType<'t when 't : not struct>(persistor : IObjectStore) = 
    let test = persistor.CreateInstance<'t>(null)
    persistor.LoadComplexTypesIntoNakedObjectFramework(GetOrAddAdapterForTest test null, false)
