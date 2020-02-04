// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.DomainTest

open NUnit.Framework
open DomainTestCode
open TestTypes
open TestCode
open System.Data.Entity.Core.Objects
open NakedObjects.Persistor.Entity.Configuration
open System
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly


let persistor = 
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    let f = (fun () -> new AdventureWorksEntities(csOne) :> Data.Entity.DbContext)
    c.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
    let p = getEntityObjectStore c
    setupPersistorForTesting p

let overwritePersistor =
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    let f = (fun () -> new AdventureWorksEntities(csTwo) :> Data.Entity.DbContext)
    c.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
    c.DefaultMergeOption <- MergeOption.OverwriteChanges
    let p = getEntityObjectStore c
    setupPersistorForTesting p

[<TestFixture>]
type DomainTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = 
            DomainSetup()
            ()
        
        [<OneTimeTearDown>]
        member x.TearDown() = persistor.SetupContexts()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor persistor
        
        [<Test>]
        member x.TestGetInstancesGeneric() = CanGetInstancesGeneric persistor
        
        [<Test>]
        member x.TestGetInstancesByType() = CanGetInstancesByType persistor
        
        [<Test>]
        member x.TestGetInstancesIsProxy() = CanGetInstancesIsProxy persistor
        
        [<Test>]
        member x.TestGetManyToOneReference() = CanGetManyToOneReference persistor
        
        [<Test>]
        member x.TestGetObjectBySingleKey() = CanGetObjectBySingleKey persistor
        
        [<Test>]
        member x.TestGetObjectByMultiKey() = CanGetObjectByMultiKey persistor
        
        [<Test>]
        member x.TestGetObjectByStringKey() = CanGetObjectByStringKey persistor
        
        [<Test>]
        member x.TestGetObjectByDateKey() = CanGetObjectByDateKey persistor
        
        [<Test>]
        member x.TestCreateTransientObject() = DomainTestCode.CanCreateTransientObject persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarProperties() = CanSaveTransientObjectWithScalarProperties persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithPersistentReferenceProperty() = CanSaveTransientObjectWithPersistentReferenceProperty persistor
        
        [<Test>]
        member x.TestCanSaveTransientObjectWithPersistentReferencePropertyInSeperateTransaction() = 
            CanSaveTransientObjectWithPersistentReferencePropertyInSeperateTransaction persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferenceProperty() = CanSaveTransientObjectWithTransientReferenceProperty persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarProperties() = CanUpdatePersistentObjectWithScalarProperties persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferenceProperties() = CanUpdatePersistentObjectWithReferenceProperties persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesDoFixup() = CanUpdatePersistentObjectWithReferencePropertiesDoFixup persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionProperties() = CanUpdatePersistentObjectWithCollectionProperties persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesDoFixup() = CanUpdatePersistentObjectWithCollectionPropertiesDoFixup persistor
        
        [<Test>]
        member x.TestNavigateReferences() = CanNavigateReferences persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt() = CanUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesIgnore() = CanUpdatePersistentObjectWithScalarPropertiesIgnore persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt persistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore persistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstance() = CanPersistingPersistedCalledForCreateInstance persistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = CanPersistingPersistedCalledForCreateInstanceWithReference persistor
        
        [<Test>]
        member x.TestUpdatingUpdatedCalledForChange() = CanUpdatingUpdatedCalledForChange persistor
        
        [<Test>]
        member x.TestGetKeyForType() = CanGetKeyForType persistor
        
        [<Test>]
        member x.TestGetKeysForType() = CanGetKeysForType persistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForNewInstance() = CanContainerInjectionCalledForNewInstance persistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForGetInstance() = CanContainerInjectionCalledForGetInstance(resetPersistor persistor)
        
        [<Test>]
        member x.TestCreateManyToMany() = CanCreateManyToMany persistor
        
        [<Test>]
        member x.TestCanUpdatePersistentObjectWithScalarPropertiesAbort() = CanUpdatePersistentObjectWithScalarPropertiesAbort persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = CanUpdatePersistentObjectWithReferencePropertiesAbort persistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesAbort() = CanUpdatePersistentObjectWithCollectionPropertiesAbort persistor
        
        [<Test>]
        member x.TestRemoteResolve() = CanRemoteResolve(resetPersistor persistor)
        
        [<Test>]
        member x.TestCanGetContextForCollection() = DomainCanGetContextForCollection persistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = DomainCanGetContextForNonGenericCollection persistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = DomainCanGetContextForArray persistor
        
        [<Test>]
        member x.TestCanGetContextForType() = DomainCanGetContextForType persistor
        
        [<Test>]        
        member x.TestCanDetectConcurrency() = CanDetectConcurrency persistor
        
        [<Test>]
        member x.DataUpdateNoCustomOnPersistingError() = DataUpdateNoCustomOnPersistingError persistor
        
        [<Test>]
        member x.DataUpdateNoCustomOnUpdatingError() = DataUpdateNoCustomOnUpdatingError persistor
        
        [<Test>]        
        member x.ConcurrencyNoCustomOnUpdatingError() = ConcurrencyNoCustomOnUpdatingError persistor
        
        [<Test>]       
        member x.OverWriteChangesOptionRefreshesObject() = OverWriteChangesOptionRefreshesObject overwritePersistor
        
        [<Test>]
        member x.AppendOnlyOptionDoesNotRefreshObject() = AppendOnlyOptionDoesNotRefreshObject persistor
        
        [<Test>]        
        member x.OverWriteChangesOptionRefreshesObjectNonGenericGet() = OverWriteChangesOptionRefreshesObjectNonGenericGet overwritePersistor
        
        [<Test>]
        member x.AppendOnlyOptionDoesNotRefreshObjectNonGenericGet() = AppendOnlyOptionDoesNotRefreshObjectNonGenericGet persistor
        
        [<Test>]
        member x.ExplicitOverwriteChangesRefreshesObject() = ExplicitOverwriteChangesRefreshesObject persistor
        
        [<Test>]
        member x.GetKeysReturnsKey() = GetKeysReturnsKey persistor
    end