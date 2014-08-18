// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.ModelTestCode
open NUnit.Framework
open NakedObjects.EntityObjectStore
open ModelFirst
open TestCode
open TestTypes
open System
open NakedObjects
open NakedObjects.Architecture.Adapter
open NakedObjects.Architecture.Reflect
open System.Data.Entity.Core.Objects

let ModelConfig = 
  let pc = new PocoEntityContextConfiguration()
  pc.ContextName <- "Model1Container"
  pc.DefaultMergeOption <- MergeOption.AppendOnly
  pc

type TestInjector() = 
    interface IContainerInjector with 
        member x.InitDomainObject obj = 
            let container = new NakedObjects.Reflector.DotNet.DotNetDomainObjectContainer(null)
            let prop = obj.GetType().GetProperty("Container")
            if prop <> null then 
                prop.SetValue(obj, container, null)    
            let service = new NakedObjects.Services.SimpleRepository<Person>()
            let prop = obj.GetType().GetProperty("Service")
            if prop <> null then 
                prop.SetValue(obj, service, null)
        member x.InitInlineObject(root : Object, inlineObject : Object) = ()
            
let setupPersistorForInjectorTesting (p : EntityObjectStore) = 
    p.SetupForTesting(new TestInjector(),
                      EntityObjectStore.CreateAdapterDelegate(AdapterForTest),
                      EntityObjectStore.ReplacePocoDelegate(ReplacePocoForTest), 
                      EntityObjectStore.RemoveAdapterDelegate(RemoveAdapterForTest), 
                      EntityObjectStore.CreateAggregatedAdapterDelegate(AggregateAdapterForTest), 
                      EntityObjectStore.NotifyUiDelegate(NotifyUIForTest),
                      Action<INakedObject>(updated), 
                      Action<INakedObject>(updating),
                      Action<INakedObject>(persisted), 
                      Action<INakedObject>(persisting),
                      Action<INakedObject>(handleLoadingTest),
                      EventHandler(savingChangesHandler),             
                      Func<Type, NakedObjects.Architecture.Spec.INakedObjectSpecification>(loadSpecificationHandler))
    p.Reset()
    p
    
let resetForInjectorPersistor (p : EntityObjectStore) =
    p.Reset()
    setupPersistorForInjectorTesting p


let ModelLoadTestAssembly() = 
    let obj = new Person()
    ()
      
let ModelSetup() = ModelLoadTestAssembly()

let CanCreateEntityPersistor persistor = Assert.IsNotNull(persistor)

let setter (persistor : EntityObjectStore) (person : Person)  = 
    person.Id <- GetNextID<Person> persistor (fun i -> i.Id)
    person.ComplexProperty.Firstname <- uniqueName()
    person.ComplexProperty.Surname <- uniqueName()      
    person.ComplexProperty_1.s1 <- uniqueName()
    person.ComplexProperty_1.s2 <- uniqueName()     

let CanGetInstanceWithComplexType (persistor : EntityObjectStore) = 
    let person = persistor.GetInstances<Person>() |> Seq.head
    Assert.IsNotNull(person)

let CanUpdateInstanceWithComplexType (persistor : EntityObjectStore) = 
    let person = persistor.GetInstances<Person>() |> Seq.head
    let name = person.ComplexProperty
    let (on1, on2) = (name.Firstname, name.Surname)
    let (un1, un2) = (uniqueName(),  uniqueName()) 
    
    let toggleAndCheckNames (n1, n2) = 
        name.Firstname <- n1
        name.Surname <- n2
        SaveAndEndTransaction persistor person
        let person1 = persistor.GetInstances<Person>() |> Seq.filter (fun p -> p.Id = person.Id) |> Seq.head
        Assert.AreEqual(n1, person1.ComplexProperty.Firstname)
        Assert.AreEqual(n2, person1.ComplexProperty.Surname)   
    
    toggleAndCheckNames (un1, un2)
    toggleAndCheckNames (on1, on2)
 
let CanCreateInstanceWithComplexType (persistor : EntityObjectStore) =   
    CanSaveTransientObject persistor (setter persistor)    

     
let ModelCanGetContextForCollection persistor = CanGetContextForCollection<Person> persistor

let ModelCanGetContextForNonGenericCollection persistor = CanGetContextForNonGenericCollection<Person> persistor

let ModelCanGetContextForArray persistor = CanGetContextForArray<Person> persistor

let ModelCanGetContextForType persistor =  CanGetContextForType<Person> persistor

let ModelCanGetContextForComplexType persistor =  
    CanGetContextForType<Person> (resetForInjectorPersistor persistor)
    CanGetContextForType<NameType>  persistor
 
let CheckContainer (person : Person) = 
    let name = person.ComplexProperty
    Assert.IsNotNull(name.Container)
    Assert.IsInstanceOf(typeof<NakedObjects.IDomainObjectContainer>, name.Container)
 
let CheckService (person : Person) = 
    let name = person.ComplexProperty
    Assert.IsNotNull(name.Service)
    Assert.IsInstanceOf(typeof<NakedObjects.Services.SimpleRepository<Person>>, name.Service)
 
let CanInjectContainerOnNewInstance persistor  = 
    let person = CreateAndSetup<Person> persistor (setter persistor)
    CheckContainer person 

let CanInjectServiceOnNewInstance persistor  = 
    let person = CreateAndSetup<Person> persistor (setter persistor)
    CheckService person 