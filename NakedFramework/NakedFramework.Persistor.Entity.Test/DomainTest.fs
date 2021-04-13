// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.DomainTest

open DomainTestCode
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NakedFramework.Persistor.EF6.Configuration
open NUnit.Framework
open System
open System.Data.Entity.Core.Objects
open TestCode
open TestTypes
open NakedFramework.Architecture.Component
open NakedFramework.Persistor.EF6.Component

let persistor = 
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    let f = (fun () -> new AdventureWorksEntities(csAWMARS) :> Data.Entity.DbContext)
    c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    let p = getEntityObjectStore c
    setupPersistorForTesting p

let overwritePersistor =
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    let f = (fun () -> new AdventureWorksEntities(csAWMARS) :> Data.Entity.DbContext)
    c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    c.DefaultMergeOption <- MergeOption.OverwriteChanges
    let p = getEntityObjectStore c
    setupPersistorForTesting p

[<TestFixture>]
type DomainTests() = 
    class
        member x.persistor = persistor :> IObjectStore   
        member x.overwritePersistor = overwritePersistor :> IObjectStore
        
        [<OneTimeSetUp>]
        member x.Setup() = 
            DomainSetup()
            ()
        
        [<OneTimeTearDown>]
        member x.TearDown() = 
            match x.persistor with 
            | :? EntityObjectStore as eos -> eos.SetupContexts()
            | _ -> ()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor x.persistor
        
        [<Test>]
        member x.TestGetInstancesGeneric() = CanGetInstancesGeneric x.persistor
        
        [<Test>]
        member x.TestGetInstancesByType() = CanGetInstancesByType x.persistor
        
        [<Test>]
        member x.TestGetInstancesIsProxy() = CanGetInstancesIsProxy x.persistor
        
        [<Test>]
        member x.TestGetManyToOneReference() = CanGetManyToOneReference x.persistor
        
        [<Test>]
        member x.TestGetObjectBySingleKey() = CanGetObjectBySingleKey x.persistor
        
        [<Test>]
        member x.TestGetObjectByMultiKey() = CanGetObjectByMultiKey x.persistor
        
        [<Test>]
        member x.TestGetObjectByStringKey() = CanGetObjectByStringKey x.persistor
        
        [<Test>]
        member x.TestGetObjectByDateKey() = CanGetObjectByDateKey x.persistor
        
        [<Test>]
        member x.TestCreateTransientObject() = DomainTestCode.CanCreateTransientObject x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarProperties() = CanSaveTransientObjectWithScalarProperties x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithPersistentReferenceProperty() = CanSaveTransientObjectWithPersistentReferenceProperty x.persistor
        
        [<Test>]
        member x.TestCanSaveTransientObjectWithPersistentReferencePropertyInSeperateTransaction() = 
            CanSaveTransientObjectWithPersistentReferencePropertyInSeperateTransaction x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferenceProperty() = CanSaveTransientObjectWithTransientReferenceProperty x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarProperties() = CanUpdatePersistentObjectWithScalarProperties x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferenceProperties() = CanUpdatePersistentObjectWithReferenceProperties x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesDoFixup() = CanUpdatePersistentObjectWithReferencePropertiesDoFixup x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionProperties() = CanUpdatePersistentObjectWithCollectionProperties x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesDoFixup() = CanUpdatePersistentObjectWithCollectionPropertiesDoFixup x.persistor
        
        [<Test>]
        member x.TestNavigateReferences() = CanNavigateReferences x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt() = CanUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesIgnore() = CanUpdatePersistentObjectWithScalarPropertiesIgnore x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt x.persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore x.persistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstance() = CanPersistingPersistedCalledForCreateInstance x.persistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = CanPersistingPersistedCalledForCreateInstanceWithReference x.persistor
        
        [<Test>]
        member x.TestUpdatingUpdatedCalledForChange() = CanUpdatingUpdatedCalledForChange x.persistor
        
        [<Test>]
        member x.TestGetKeyForType() = CanGetKeyForType x.persistor
        
        [<Test>]
        member x.TestGetKeysForType() = CanGetKeysForType x.persistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForNewInstance() = CanContainerInjectionCalledForNewInstance x.persistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForGetInstance() = CanContainerInjectionCalledForGetInstance(resetPersistor x.persistor)
        
        [<Test>]
        member x.TestCreateManyToMany() = CanCreateManyToMany x.persistor
        
        [<Test>]
        member x.TestCanUpdatePersistentObjectWithScalarPropertiesAbort() = CanUpdatePersistentObjectWithScalarPropertiesAbort x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = CanUpdatePersistentObjectWithReferencePropertiesAbort x.persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesAbort() = CanUpdatePersistentObjectWithCollectionPropertiesAbort x.persistor
        
        [<Test>]
        member x.TestRemoteResolve() = CanRemoteResolve(resetPersistor x.persistor)
        
        [<Test>]
        member x.TestCanGetContextForCollection() = DomainCanGetContextForCollection x.persistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = DomainCanGetContextForNonGenericCollection x.persistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = DomainCanGetContextForArray x.persistor
        
        [<Test>]
        member x.TestCanGetContextForType() = DomainCanGetContextForType x.persistor
        
        [<Test>]        
        member x.TestCanDetectConcurrency() = CanDetectConcurrency x.persistor
        
        [<Test>]
        member x.DataUpdateNoCustomOnPersistingError() = DataUpdateNoCustomOnPersistingError x.persistor
        
        [<Test>]
        member x.DataUpdateNoCustomOnUpdatingError() = DataUpdateNoCustomOnUpdatingError x.persistor
        
        [<Test>]        
        member x.ConcurrencyNoCustomOnUpdatingError() = ConcurrencyNoCustomOnUpdatingError x.persistor
        
        [<Test>]       
        member x.OverWriteChangesOptionRefreshesObject() = OverWriteChangesOptionRefreshesObject x.overwritePersistor
        
        [<Test>]
        member x.AppendOnlyOptionDoesNotRefreshObject() = AppendOnlyOptionDoesNotRefreshObject x.persistor
        
        [<Test>]        
        member x.OverWriteChangesOptionRefreshesObjectNonGenericGet() = OverWriteChangesOptionRefreshesObjectNonGenericGet x.overwritePersistor
        
        [<Test>]
        member x.AppendOnlyOptionDoesNotRefreshObjectNonGenericGet() = AppendOnlyOptionDoesNotRefreshObjectNonGenericGet x.persistor
        
        [<Test>]
        member x.ExplicitOverwriteChangesRefreshesObject() = ExplicitOverwriteChangesRefreshesObject x.persistor
        
        [<Test>]
        member x.GetKeysReturnsKey() = GetKeysReturnsKey x.persistor
    end