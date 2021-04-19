// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.ModelTestCode

open Moq
open NakedObjects
open NakedFramework.Architecture.Adapter
open NakedFramework.Core.Component
open NakedFramework.Core.Configuration
open NakedFramework.Persistor.EF6.Configuration
open NakedFramework.Persistor.EF6.Component
open NUnit.Framework
open SimpleDatabase
open System
open System.Data.Entity.Core.Objects
open TestCode
open TestTypes
open Microsoft.Extensions.Logging
open NakedObjects.Reflector.Configuration
open NakedFramework.Core.Component
open NakedFramework.Architecture.Framework
open NakedFramework.Architecture.Spec
open NakedObjects.Core.Component
open System.Reflection
open NakedFramework.Architecture.Component
open NakedFramework.Persistor.EFCore.Component

let ModelConfig = 
    let pc = new EF6ContextConfiguration()
    let f = (fun () -> new SimpleDatabaseDbContext("Model1Container") :> Data.Entity.DbContext)
    pc.DbContext <-  Func<Data.Entity.DbContext>(f)
    pc.DefaultMergeOption <- MergeOption.AppendOnly
    pc

ObjectReflectorConfiguration.NoValidate <- true
let config = new ObjectReflectorConfiguration([||], [| typeof<NakedObjects.Services.SimpleRepository<Person>> |] )
let serviceList = new AllServiceList ([|new ServiceList(config.Services)|])

let mockLoggerFactory = new Mock<ILoggerFactory>();
let mockLogger = new Mock<ILogger<DomainObjectContainerInjector>>();
let injector = new DomainObjectContainerInjector(serviceList, mockLoggerFactory.Object, mockLogger.Object);

injector.set_Framework (new Mock<INakedObjectsFramework>()).Object

let setupEF6PersistorForInjectorTesting (p : EF6ObjectStore) = 
    p.SetupForTesting
        (injector, 
         Func<IOid, obj, INakedObjectAdapter> AdapterForTest, 
         Action<INakedObjectAdapter, obj>  ReplacePocoForTest, 
         Action<INakedObjectAdapter>  RemoveAdapterForTest, 
         Func<INakedObjectAdapter, PropertyInfo, obj, INakedObjectAdapter> AggregateAdapterForTest, 
         Action<INakedObjectAdapter> handleLoadingTest, 
         Action<obj, EventArgs> savingChangesHandler, 
         Func<Type, IObjectSpec> loadSpecificationHandler)
    p.SetupContexts()
    p

let setupEFCorePersistorForInjectorTesting (p : EFCoreObjectStore) = 
    p.SetupForTesting
        (injector, 
         Func<IOid, obj, INakedObjectAdapter> AdapterForTest, 
         Action<INakedObjectAdapter, obj>  ReplacePocoForTest, 
         Action<INakedObjectAdapter>  RemoveAdapterForTest, 
         Func<INakedObjectAdapter, PropertyInfo, obj, INakedObjectAdapter> AggregateAdapterForTest, 
         Action<INakedObjectAdapter> handleLoadingTest, 
         Action<obj> (fun (o) -> ()), 
         Func<Type, IObjectSpec> loadSpecificationHandler)
    p.SetupContexts()
    p

let resetForEF6InjectorPersistor (p : EF6ObjectStore) = 
    p.SetupContexts()
    setupEF6PersistorForInjectorTesting p

let resetForEFCoreInjectorPersistor (p : EFCoreObjectStore) = 
    p.SetupContexts()
    setupEFCorePersistorForInjectorTesting p

let ModelLoadTestAssembly() = 
    let obj = new Person()
    ()

//let ModelSetup() = ModelLoadTestAssembly()
let CanCreateEntityPersistor persistor = Assert.IsNotNull(persistor)

let setter (persistor : IObjectStore) (person : Person) = 
    person.Id <- GetNextID<Person> persistor (fun i -> i.Id)
    person.ComplexProperty.Firstname <- uniqueName()
    person.ComplexProperty.Surname <- uniqueName()
    person.ComplexProperty_1.s1 <- uniqueName()
    person.ComplexProperty_1.s2 <- uniqueName()

let CanGetInstanceWithComplexType(persistor : IObjectStore) = 
    let person = persistor.GetInstances<Person>() |> Seq.head
    Assert.IsNotNull(person)

let CanUpdateInstanceWithComplexType(persistor : IObjectStore) = 
    let person = persistor.GetInstances<Person>() |> Seq.head
    let name = person.ComplexProperty
    let (on1, on2) = (name.Firstname, name.Surname)
    let (un1, un2) = (uniqueName(), uniqueName())
    
    let toggleAndCheckNames (n1, n2) = 
        name.Firstname <- n1
        name.Surname <- n2
        SaveAndEndTransaction persistor person
        let person1 = 
            persistor.GetInstances<Person>()
            |> Seq.filter (fun p -> p.Id = person.Id)
            |> Seq.head
        Assert.AreEqual(n1, person1.ComplexProperty.Firstname)
        Assert.AreEqual(n2, person1.ComplexProperty.Surname)
    toggleAndCheckNames (un1, un2)
    toggleAndCheckNames (on1, on2)

let CanCreateInstanceWithComplexType(persistor : IObjectStore) = CanSaveTransientObject persistor (setter persistor)
let ModelCanGetContextForCollection persistor = CanGetContextForCollection<Person> persistor
let ModelCanGetContextForNonGenericCollection persistor = CanGetContextForNonGenericCollection<Person> persistor
let ModelCanGetContextForArray persistor = CanGetContextForArray<Person> persistor
let ModelCanGetContextForType persistor = CanGetContextForType<Person> persistor

let ModelCanGetEF6ContextForComplexType (persistor : EF6ObjectStore) = 
    CanGetContextForType<Person>(resetForEF6InjectorPersistor persistor)
    CanGetContextForType<NameType> persistor

let ModelCanGetEFCoreContextForComplexType (persistor : EFCoreObjectStore) = 
    CanGetContextForType<Person>(resetForEFCoreInjectorPersistor persistor)
    CanGetContextForType<NameType> persistor

let ModelCanGetContextForComplexType (persistor : IObjectStore)  =
    match persistor with 
    | :? EF6ObjectStore as eos -> ModelCanGetEF6ContextForComplexType eos
    | :? EFCoreObjectStore as efos -> ModelCanGetEFCoreContextForComplexType efos 
    | _ -> ()

let CheckContainer(person : Person) = 
    let name = person.ComplexProperty
    Assert.IsNotNull(name.ExposeContainerForTest())
    Assert.IsInstanceOf(typeof<NakedObjects.IDomainObjectContainer>, name.ExposeContainerForTest())

let CheckService(person : Person) = 
    let name = person.ComplexProperty
    Assert.IsNotNull(name.ExposeServiceForTest())
    Assert.IsInstanceOf(typeof<NakedObjects.Services.SimpleRepository<Person>>, name.ExposeServiceForTest())

let CanInjectContainerOnNewInstance persistor = 
    let person = CreateAndSetup<Person> persistor (setter persistor)
    CheckContainer person

let CanInjectServiceOnNewInstance persistor = 
    let person = CreateAndSetup<Person> persistor (setter persistor)
    CheckService person 