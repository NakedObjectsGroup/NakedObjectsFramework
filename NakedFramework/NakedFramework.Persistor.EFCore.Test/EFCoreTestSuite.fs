// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedFramework.EFCoreTestSuite

open NakedObjects.Persistor.TestData
open NakedObjects.Persistor.TestSuite
open NakedObjects.Services
open NakedObjects.TestTypes
open NUnit.Framework
open System
open TestData
open Microsoft.Extensions.Configuration
open NakedFramework.Test.TestCase
open NakedFramework.DependencyInjection.Extensions
open NakedFramework.Persistor.EFCore.Extensions
open Microsoft.EntityFrameworkCore
open NakedFramework.Architecture.Component
open Microsoft.Extensions.DependencyInjection
open NakedFramework.Persistor.EFCore.Component

let assemblyName = "NakedFramework.Persistor.Test.Data"

let LoadTestAssembly() = 
    let obj = new Person()
    ()

[<TestFixture>]
type EFCoreTestSuite() = 
    inherit AcceptanceTestCase()

    override x.EnforceProxies = false

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    member x.ContextInstallers = [|Func<IConfiguration, DbContext> (fun (c) -> 
               let context = new EFCoreTestDataContext(csTDCO)
               context.Create()
               (context :> DbContext))|]
     
    member x.EFCorePersistorOptions = Action<EFCorePersistorOptions> (fun  (options) -> options.ContextInstallers <- x.ContextInstallers)

    override x.AddPersistor = Action<NakedCoreOptions> (fun (builder) -> builder.AddEFCorePersistor(x.EFCorePersistorOptions))

    override x.Services = [| typeof<SimpleRepository<Person>>; 
                             typeof<SimpleRepository<Product>>;
                             typeof<SimpleRepository<Address>> |]

    override x.ObjectTypes = [| typeof<TestData.TestHelper>;
                                typeof<TestData.Person>;
                                typeof<TestData.Pet>;
                                typeof<TestData.Address>;
                                typeof<TestData.Order>;
                                typeof<TestData.Product>;
                                typeof<TestData.OrderFail> |]
    
    member x.ClearOldTestData() = ()
           
    [<OneTimeSetUpAttribute>]
    member x.SetupFixture() = 
        (new EFCoreTestDataContext(csTDCO)).Delete()
        AcceptanceTestCase.InitializeNakedObjectsFramework(x)
    
    [<SetUp>]
    member x.SetupTest() = x.StartTest()
    
    [<TearDown>]
    member x.TearDownTest() = x.EndTest()
    
    [<OneTimeTearDown>]
    member x.TearDownFixture() =
        (new EFCoreTestDataContext(csTDCO)).Delete()
        AcceptanceTestCase.CleanupNakedObjectsFramework(x)
    
    override x.Fixtures = [| box (new TestDataFixture()) |]
     
    member x.Tests = new PersistorTestSuite(x.NakedObjectsFramework)
    
    [<Test>]
    member x.CanAccessCollectionProperty() = x.Tests.CanAccessCollectionProperty()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfT() = x.Tests.GetInstanceFromInstancesOfT()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfType() = x.Tests.GetInstanceFromInstancesOfType()
    
    [<Test>]
    member x.GetInstanceFromInstancesOfSpecification() = x.Tests.GetInstanceFromInstancesOfSpecification()
    
    [<Test>]
    member x.GetInstanceIsAlwaysSameObject() = x.Tests.GetInstanceIsAlwaysSameObject()
    
    [<Test>]
    member x.GetInstanceResolveStateIsPersistent() = x.Tests.GetInstanceResolveStateIsPersistent()
    
    [<Test>]
    member x.GetInstanceHasVersion() = x.Tests.GetInstanceHasVersion()
    
    
    [<Test>]
    member x.ChangeScalarOnPersistentCallsUpdatingUpdated() = x.Tests.ChangeScalarOnPersistentCallsUpdatingUpdated()
     
    [<Test>]
    member x.ChangeReferenceOnPersistentCallsUpdatingUpdated() = x.Tests.ChangeReferenceOnPersistentCallsUpdatingUpdated()
    
    [<Test>]
    member x.UpdatedDoesntCallPersistedAtOnce() = x.Tests.UpdatedDoesntCallPersistedAtOnce()
    
    [<Test>]
    member x.LoadObjectReturnSameObject() = x.Tests.LoadObjectReturnSameObject()
    
    [<Test>]
    member x.PersistentObjectHasContainerInjected() = x.Tests.PersistentObjectHasContainerInjected()
    
    [<Test>]
    member x.PersistentObjectHasServiceInjected() = x.Tests.PersistentObjectHasServiceInjected()
    
    [<Test>]
    member x.PersistentObjectHasLoadingLoadedCalled() = x.Tests.PersistentObjectHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CanAccessReferenceProperty() = x.Tests.CanAccessReferenceProperty()
    
    [<Test>]
    member x.ReferencePropertyHasLoadingLoadedCalled() = x.Tests.ReferencePropertyHasLoadingLoadedCalled()
    
    [<Test>]
    member x.ReferencePropertyObjectHasContainerInjected() = x.Tests.ReferencePropertyObjectHasContainerInjected()
    
    [<Test>]
    member x.ReferencePropertyObjectHasServiceInjected() = x.Tests.ReferencePropertyObjectHasServiceInjected()
    
    [<Test>]
    member x.ReferencePropertyObjectResolveStateIsPersistent() = x.Tests.ReferencePropertyObjectResolveStateIsPersistent()
    
    [<Test>]
    member x.ReferencePropertyObjectHasVersion() = x.Tests.ReferencePropertyObjectHasVersion()
    
    [<Test>]
    member x.CollectionPropertyHasLoadingLoadedCalled() = x.Tests.CollectionPropertyHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CollectionPropertyObjectHasContainerInjected() = x.Tests.CollectionPropertyObjectHasContainerInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasMenuServiceInjected() = x.Tests.CollectionPropertyObjectHasMenuServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasContributedServiceInjected() = x.Tests.CollectionPropertyObjectHasContributedServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectHasSystemServiceInjected() = x.Tests.CollectionPropertyObjectHasSystemServiceInjected()
    
    [<Test>]
    member x.CollectionPropertyObjectResolveStateIsPersistent() = x.Tests.CollectionPropertyObjectResolveStateIsPersistent()
    
    [<Test>]
    member x.CollectionPropertyObjectHasVersion() = x.Tests.CollectionPropertyObjectHasVersion()
    
    [<Test>]
    member x.CollectionPropertyCollectionResolveStateIsPersistent() = x.Tests.CollectionPropertyCollectionResolveStateIsPersistent()
    
    [<Test>]
    member x.AddToCollectionOnPersistent() = x.Tests.AddToCollectionOnPersistent()
    
    [<Test>]
    member x.AddToCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.AddToCollectionOnPersistentCallsUpdatingUpdated()
    
    [<Test>]
    member x.RemoveFromCollectionOnPersistent() = x.Tests.RemoveFromCollectionOnPersistent()
    
    [<Test>]
    member x.RemoveFromCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.RemoveFromCollectionOnPersistentCallsUpdatingUpdated()
      
    [<Test>]
    member x.ClearCollectionOnPersistent() = x.Tests.ClearCollectionOnPersistent()
    
    [<Test>]
    member x.ClearCollectionOnPersistentCallsUpdatingUpdated() = x.Tests.ClearCollectionOnPersistentCallsUpdatingUpdated()
      
    [<Test>]
    member x.NewObjectHasContainerInjected() = x.Tests.NewObjectHasContainerInjected()
    
    [<Test>]
    member x.NewObjectHasCreatedCalled() = x.Tests.NewObjectHasCreatedCalled()
    
    [<Test>]
    member x.NewObjectHasServiceInjected() = x.Tests.NewObjectHasServiceInjected()
    
    [<Test>]
    member x.NewObjectHasVersion() = x.Tests.NewObjectHasVersion()
    
    [<Test>]
    member x.NewObjectIsCreated() = x.Tests.NewObjectIsCreated()
    
    [<Test>]
    member x.NewObjectIsTransient() = x.Tests.NewObjectIsTransient()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersisted() = x.Tests.SaveNewObjectCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursively() = x.Tests.SaveNewObjectCallsPersistingPersistedRecursively()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursivelyFails() = x.Tests.SaveNewObjectCallsPersistingPersistedRecursivelyFails()
    
    [<Test>]
    member x.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax() = 
       
        let os = x.NakedObjectsFramework.ServiceProvider.GetService<IObjectStore>() :?> EFCoreObjectStore
        os.MaximumCommitCycles <- 1
        x.Tests.SaveNewObjectCallsPersistingPersistedRecursivelyExceedsMax()
        os.MaximumCommitCycles <- 10
    
    [<Test>]
    member x.SaveNewObjectTransientCollectionItemCallsPersistingPersisted() = x.Tests.SaveNewObjectTransientCollectionItemCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectTransientReferenceCallsPersistingPersisted() = x.Tests.SaveNewObjectTransientReferenceCallsPersistingPersisted()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentItemCollectionItem() = x.Tests.SaveNewObjectWithPersistentItemCollectionItem()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentReference() = x.Tests.SaveNewObjectWithPersistentReference()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() = x.Tests.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction()
    
    [<Test>]
    member x.SaveNewObjectWithPersistentReferenceInSeperateTransaction() = x.Tests.SaveNewObjectWithPersistentReferenceInSeperateTransaction()
    
    [<Test>]
    member x.SaveNewObjectWithScalars() = x.Tests.SaveNewObjectWithScalars()
    
    // cross validate is done from facade
    //[<Test>]
    //member x.SaveNewObjectWithValidate() = x.Tests.SaveNewObjectWithValidate()
    
    [<Test>]
    member x.ChangeObjectWithValidate() = x.Tests.ChangeObjectWithValidate()
    
    [<Test>]
    member x.SaveNewObjectWithTransientCollectionItem() = x.Tests.SaveNewObjectWithTransientCollectionItem()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReference() = x.Tests.SaveNewObjectWithTransientReference()
    
    [<Test>]
    member x.EmptyCollectionPropertyCollectionResolveStateIsPersistent() = x.Tests.EmptyCollectionPropertyCollectionResolveStateIsPersistent()
    
    [<Test>]
    member x.GetInlineInstance() = x.Tests.GetInlineInstance()
    
    [<Test>]
    member x.InlineObjectHasContainerInjected() = x.Tests.InlineObjectHasContainerInjected()
    
    [<Test>]
    member x.InlineObjectHasServiceInjected() = x.Tests.InlineObjectHasServiceInjected()
    
    [<Test>]
    member x.InlineObjectHasParentInjected() = x.Tests.InlineObjectHasParentInjected()
    
    [<Test>]
    member x.InlineObjectHasVersion() = x.Tests.InlineObjectHasVersion()
    
    [<Test>]
    member x.InlineObjectHasLoadingLoadedCalled() = x.Tests.InlineObjectHasLoadingLoadedCalled()
    
    [<Test>]
    member x.CreateTransientInlineInstance() = x.Tests.CreateTransientInlineInstance()
    
    [<Test>]
    member x.TransientInlineObjectHasContainerInjected() = x.Tests.TransientInlineObjectHasContainerInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasServiceInjected() = x.Tests.TransientInlineObjectHasServiceInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasParentInjected() = x.Tests.TransientInlineObjectHasParentInjected()
    
    [<Test>]
    member x.TransientInlineObjectHasVersion() = x.Tests.TrainsientInlineObjectHasVersion()
    
    [<Test>]
    member x.InlineObjectCallsCreated() = x.Tests.InlineObjectCallsCreated()
    
    [<Test>]
    member x.SaveInlineObjectCallsPersistingPersisted() = x.Tests.SaveInlineObjectCallsPersistingPersisted()
    
    [<Test>]
    member x.ChangeScalarOnInlineObjectCallsUpdatingUpdated() = x.Tests.ChangeScalarOnInlineObjectCallsUpdatingUpdated()
     
    [<Test>]
    member x.RefreshResetsObject() = x.Tests.RefreshResetsObject()
    
    [<Test>]
    member x.GetKeysReturnsKeys() = x.Tests.GetKeysReturnsKeys()
    
    [<Test>]
    member x.FindByKey() = x.Tests.FindByKey()
    
    [<Test>]
    member x.CreateAndDeleteNewObjectWithScalars() = x.Tests.CreateAndDeleteNewObjectWithScalars()
    
    [<Test>]
    member x.DeleteObjectCallsDeletingDeleted() = x.Tests.DeleteObjectCallsDeletingDeleted()
    
    [<Test>]
    member x.CountCollectionOnPersistent() = x.Tests.CountCollectionOnPersistent()
    
    [<Test>]
    member x.CountUnResolvedCollectionOnPersistent() = x.Tests.CountUnResolvedCollectionOnPersistent()
    
    [<Test>]
    member x.CountCollectionOnTransient() = x.Tests.CountCollectionOnTransient()
    
    [<Test>]
    member x.CountEmptyCollectionOnTransient() = x.Tests.CountEmptyCollectionOnTransient()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceInvalid() = x.Tests.SaveNewObjectWithTransientReferenceInvalid()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceObjectInvalid() = x.Tests.SaveNewObjectWithTransientReferenceObjectInvalid()
    
    [<Test>]
    member x.SaveNewObjectWithTransientReferenceValidateAssocInvalid() = x.Tests.SaveNewObjectWithTransientReferenceValidateAssocInvalid()