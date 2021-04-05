// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedFramework.AMultiDatabaseTest

open NakedObjects
open CodeOnlyTestCode
open DomainTestCode
open MultiDatabaseTestCode
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NUnit.Framework
open TestCode
open TestTypes
open System
open NakedFramework.Architecture.Component
open NakedFramework.Persistor.EFCore.Configuration
open Microsoft.EntityFrameworkCore
open TestCodeOnly

let multiDatabasePersistor = 
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f1 = Func<DbContext> (fun () -> 
        let c = new EFCoreCodeFirstContext(csMD)
        c.Create()
        c :> DbContext)
    let f2 = Func<DbContext> (fun () -> 
        let c = new EFCoreAdventureWorksEntities(csAWMARS)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f1; f2 |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

// run this first
[<TestFixture>]
type AMultiDatabaseTests() = 
    class

        member x.multiDatabasePersistor = multiDatabasePersistor :> IObjectStore
        
        [<OneTimeSetUp>]
        member x.Setup() =
            let c1 = new EFCoreCodeFirstContext(csMD)
            c1.Delete()

        [<OneTimeTearDown>]
        member x.TearDown() =
           let c1 = new EFCoreCodeFirstContext(csMD)
           c1.Delete()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor x.multiDatabasePersistor
        
        [<Test>]
        member x.CanQueryEachConnection() = CanQueryEachConnectionMulti x.multiDatabasePersistor
        
        [<Test>]     
        member x.TestCanCreateEachConnection() = CanCreateEachConnection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanQueryEachConnectionMultiTimes() = CanQueryEachConnectionMultiTimes x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanCreateEachConnectionMultiTimes() = CanCreateEachConnectionMultiTimes x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("https://github.com/dotnet/runtime/issues/715")>]
        member x.CrossContextTransactionOK() = CrossContextTransactionOK x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("https://github.com/dotnet/runtime/issues/715")>]
        member x.CrossContextTransactionRollback() = CrossContextTransactionRollback x.multiDatabasePersistor
        
        // tests from Domainfirst 
        [<Test>]
        member x.TestGetInstancesGeneric() = DomainTestCode.CanGetInstancesGeneric x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetInstancesByType() = DomainTestCode.CanGetInstancesByType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetInstancesIsProxy() = DomainTestCode.CanGetInstancesIsProxy x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetManyToOneReference() = DomainTestCode.CanGetManyToOneReference x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectBySingleKey() = DomainTestCode.CanGetObjectBySingleKey x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByMultiKey() = DomainTestCode.CanGetObjectByMultiKey x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByStringKey() = DomainTestCode.CanGetObjectByStringKey x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByDateKey() = DomainTestCode.CanGetObjectByDateKey x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCreateTransientObject() = DomainTestCode.CanCreateTransientObject x.multiDatabasePersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarProperties() = DomainTestCode.CanSaveTransientObjectWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithPersistentReferenceProperty() = 
            DomainTestCode.CanSaveTransientObjectWithPersistentReferenceProperty x.multiDatabasePersistor
        
        [<Test>]       
        member x.TestSaveTransientObjectWithTransientReferenceProperty() = 
            DomainTestCode.CanSaveTransientObjectWithTransientReferenceProperty x.multiDatabasePersistor
        
        [<Test>] 
        [<Ignore("FIX")>]
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            DomainTestCode.CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarProperties() = DomainTestCode.CanUpdatePersistentObjectWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferenceProperties() = DomainTestCode.CanUpdatePersistentObjectWithReferenceProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesDoFixup() = 
            DomainTestCode.CanUpdatePersistentObjectWithReferencePropertiesDoFixup x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestUpdatePersistentObjectWithCollectionProperties() = DomainTestCode.CanUpdatePersistentObjectWithCollectionProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesDoFixup() = 
            DomainTestCode.CanUpdatePersistentObjectWithCollectionPropertiesDoFixup x.multiDatabasePersistor
        
        [<Test>]
        member x.TestNavigateReferences() = DomainTestCode.CanNavigateReferences x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesIgnore() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesIgnore x.multiDatabasePersistor
        
        [<Test>]        
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = 
            DomainTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt x.multiDatabasePersistor
        
        [<Test>]        
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = 
            DomainTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore x.multiDatabasePersistor
        
        [<Test>]        
        member x.TestPersistingPersistedCalledForCreateInstance() = DomainTestCode.CanPersistingPersistedCalledForCreateInstance x.multiDatabasePersistor
        
        [<Test>]        
        member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = 
            DomainTestCode.CanPersistingPersistedCalledForCreateInstanceWithReference x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatingUpdatedCalledForChange() = DomainTestCode.CanUpdatingUpdatedCalledForChange x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetKeyForType() = DomainTestCode.CanGetKeyForType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestGetKeysForType() = DomainTestCode.CanGetKeysForType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForNewInstance() = DomainTestCode.CanContainerInjectionCalledForNewInstance x.multiDatabasePersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForGetInstance() = DomainTestCode.CanContainerInjectionCalledForGetInstance(resetPersistor x.multiDatabasePersistor)
        
        [<Test>]                
        member x.TestCreateManyToMany() = DomainTestCode.CanCreateManyToMany x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanUpdatePersistentObjectWithScalarPropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesAbort x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithReferencePropertiesAbort x.multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithCollectionPropertiesAbort x.multiDatabasePersistor
        
        [<Test>]
        member x.TestRemoteResolve() = DomainTestCode.CanRemoteResolve(resetPersistor x.multiDatabasePersistor)
        
        [<Test>]
        member x.TestCanGetContextForCollection() = DomainCanGetContextForCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = DomainCanGetContextForNonGenericCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = DomainCanGetContextForArray x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForType() = DomainCanGetContextForType x.multiDatabasePersistor
        
        // tests from codeonly 
        [<Test>]
        member x.TestCodeOnlyCreateEntityPersistor() = CodeOnlyTestCode.CanCreateEntityPersistor x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesGeneric() = CodeOnlyTestCode.CanGetInstancesGeneric x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesByType() = CodeOnlyTestCode.CanGetInstancesByType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesIsProxy() = CodeOnlyTestCode.CanGetInstancesIsProxy x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetObjectBySingleKey() = CodeOnlyTestCode.CanGetObjectBySingleKey x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateTransientObject() = CodeOnlyTestCode.CanCreateTransientObject x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarProperties() = CodeOnlyTestCode.CanSaveTransientObjectWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarPropertiesErrorAndReattempt() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarPropertiesErrorAndIgnore() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyNavigateReferences() = CodeOnlyTestCode.CanNavigateReferences x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithPersistentReferenceProperty() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithPersistentReferenceProperty x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithTransientReferenceProperty() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientReferenceProperty x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithScalarProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithReferenceProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithReferenceProperties x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestCodeOnlyUpdatePersistentObjectWithCollectionProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithCollectionProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyPersistingPersistedCalledForCreateInstance() = 
            CodeOnlyTestCode.CanPersistingPersistedCalledForCreateInstance x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyPersistingPersistedCalledForCreateInstanceWithReference() = 
            CodeOnlyTestCode.CanPersistingPersistedCalledForCreateInstanceWithCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatingUpdatedCalledForChange() = CodeOnlyTestCode.CanUpdatingUpdatedCalledForChange x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetKeyForType() = CodeOnlyTestCode.CanGetKeyForType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateDomesticSubclass() = CodeOnlyTestCode.CanCreateDomesticSubclass x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateInternationalSubclass() = CodeOnlyTestCode.CanCreateInternationalSubclass x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateBaseClass() = CodeOnlyTestCode.CanCreateBaseClass x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetBaseClassGeneric() = CodeOnlyTestCode.CanGetBaseClassGeneric x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetBaseClassByType() = CodeOnlyTestCode.CanGetBaseClassByType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetDomesticSubclassClassGeneric() = CodeOnlyTestCode.CanGetDomesticSubclassClassGeneric x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInternationalSubclassClassGeneric() = CodeOnlyTestCode.CanGetInternationalSubclassClassGeneric x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetDomesticSubclassClassByType() = CodeOnlyTestCode.CanGetDomesticSubclassClassByType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInternationalSubclassClassByType() = CodeOnlyTestCode.CanGetInternationalSubclassClassByType x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyNavigateToSubclass() = CodeOnlyTestCode.CanNavigateToSubclass x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetClassWithNonPersistedBase() = CodeOnlyTestCode.CanGetClassWithNonPersistedBase x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetNonPersistedClass() = CodeOnlyTestCode.CanGetNonPersistedClass x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyContainerInjectionCalledForNewInstance() = CodeOnlyTestCode.CanContainerInjectionCalledForNewInstance x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyContainerInjectionCalledForGetInstance() = CodeOnlyTestCode.CanContainerInjectionCalledForGetInstance x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientDomesticSubclasstWithScalarProperties() = 
            CodeOnlyTestCode.CanSaveTransientDomesticSubclasstWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientIntlSubclassWithScalarProperties() = 
            CodeOnlyTestCode.CanSaveTransientIntlSubclassWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentSubclassWithScalarProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentSubclassWithScalarProperties x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestCodeOnlySaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithTransientCollection() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithScalarPropertiesAbort() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithScalarPropertiesAbort x.multiDatabasePersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestCodeOnlyUpdatePersistentObjectWithReferencePropertiesAbort() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithReferencePropertiesAbort x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForCollection() = CodeOnlyCanGetContextForCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForNonGenericCollection() = CodeOnlyCanGetContextForNonGenericCollection x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForArray() = CodeOnlyCanGetContextForArray x.multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForType() = CodeOnlyCanGetContextForType x.multiDatabasePersistor
    end
