// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.AMultiDatabaseTest

open CodeOnlyTestCode
open DomainTestCode
open MultiDatabaseTestCode
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NUnit.Framework
open TestCode
open TestTypes
open System

let multiDatabasePersistor = 
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    c.UsingContext((CodeFirstConfig csMD).DbContext) |> ignore
    let f = (fun () -> new AdventureWorksEntities(csAWMARS) :> Data.Entity.DbContext)
    c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    let p = getEntityObjectStore c
    setupPersistorForTesting p

// run this first
[<TestFixture>]
type AMultiDatabaseTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = MultiDatabaseSetup()
        
        [<OneTimeTearDown>]
        member x.TearDown() = multiDatabasePersistor.SetupContexts()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor multiDatabasePersistor
        
        [<Test>]
        member x.CanQueryEachConnection() = CanQueryEachConnectionMulti multiDatabasePersistor
        
        [<Test>]     
        member x.TestCanCreateEachConnection() = CanCreateEachConnection multiDatabasePersistor
        
        [<Test>]
        member x.TestCanQueryEachConnectionMultiTimes() = CanQueryEachConnectionMultiTimes multiDatabasePersistor
        
        [<Test>]
        member x.TestCanCreateEachConnectionMultiTimes() = CanCreateEachConnectionMultiTimes multiDatabasePersistor
        
        [<Test>]
        [<Ignore("https://github.com/dotnet/runtime/issues/715")>]
        member x.CrossContextTransactionOK() = CrossContextTransactionOK multiDatabasePersistor
        
        [<Test>]
        [<Ignore("https://github.com/dotnet/runtime/issues/715")>]
        member x.CrossContextTransactionRollback() = CrossContextTransactionRollback multiDatabasePersistor
        
        // tests from Domainfirst 
        [<Test>]
        member x.TestGetInstancesGeneric() = DomainTestCode.CanGetInstancesGeneric multiDatabasePersistor
        
        [<Test>]
        member x.TestGetInstancesByType() = DomainTestCode.CanGetInstancesByType multiDatabasePersistor
        
        [<Test>]
        member x.TestGetInstancesIsProxy() = DomainTestCode.CanGetInstancesIsProxy multiDatabasePersistor
        
        [<Test>]
        member x.TestGetManyToOneReference() = DomainTestCode.CanGetManyToOneReference multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectBySingleKey() = DomainTestCode.CanGetObjectBySingleKey multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByMultiKey() = DomainTestCode.CanGetObjectByMultiKey multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByStringKey() = DomainTestCode.CanGetObjectByStringKey multiDatabasePersistor
        
        [<Test>]
        member x.TestGetObjectByDateKey() = DomainTestCode.CanGetObjectByDateKey multiDatabasePersistor
        
        [<Test>]
        member x.TestCreateTransientObject() = DomainTestCode.CanCreateTransientObject multiDatabasePersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarProperties() = DomainTestCode.CanSaveTransientObjectWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithPersistentReferenceProperty() = 
            DomainTestCode.CanSaveTransientObjectWithPersistentReferenceProperty multiDatabasePersistor
        
        [<Test>]       
        member x.TestSaveTransientObjectWithTransientReferenceProperty() = 
            DomainTestCode.CanSaveTransientObjectWithTransientReferenceProperty multiDatabasePersistor
        
        [<Test>]    
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            DomainTestCode.CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarProperties() = DomainTestCode.CanUpdatePersistentObjectWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferenceProperties() = DomainTestCode.CanUpdatePersistentObjectWithReferenceProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesDoFixup() = 
            DomainTestCode.CanUpdatePersistentObjectWithReferencePropertiesDoFixup multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionProperties() = DomainTestCode.CanUpdatePersistentObjectWithCollectionProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesDoFixup() = 
            DomainTestCode.CanUpdatePersistentObjectWithCollectionPropertiesDoFixup multiDatabasePersistor
        
        [<Test>]
        member x.TestNavigateReferences() = DomainTestCode.CanNavigateReferences multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesErrorAndReattempt multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesIgnore() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesIgnore multiDatabasePersistor
        
        [<Test>]        
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = 
            DomainTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt multiDatabasePersistor
        
        [<Test>]        
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = 
            DomainTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore multiDatabasePersistor
        
        [<Test>]        
        member x.TestPersistingPersistedCalledForCreateInstance() = DomainTestCode.CanPersistingPersistedCalledForCreateInstance multiDatabasePersistor
        
        [<Test>]        
        member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = 
            DomainTestCode.CanPersistingPersistedCalledForCreateInstanceWithReference multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatingUpdatedCalledForChange() = DomainTestCode.CanUpdatingUpdatedCalledForChange multiDatabasePersistor
        
        [<Test>]
        member x.TestGetKeyForType() = DomainTestCode.CanGetKeyForType multiDatabasePersistor
        
        [<Test>]
        member x.TestGetKeysForType() = DomainTestCode.CanGetKeysForType multiDatabasePersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForNewInstance() = DomainTestCode.CanContainerInjectionCalledForNewInstance multiDatabasePersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForGetInstance() = DomainTestCode.CanContainerInjectionCalledForGetInstance(resetPersistor multiDatabasePersistor)
        
        [<Test>]                
        member x.TestCreateManyToMany() = DomainTestCode.CanCreateManyToMany multiDatabasePersistor
        
        [<Test>]
        member x.TestCanUpdatePersistentObjectWithScalarPropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithScalarPropertiesAbort multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithReferencePropertiesAbort multiDatabasePersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionPropertiesAbort() = 
            DomainTestCode.CanUpdatePersistentObjectWithCollectionPropertiesAbort multiDatabasePersistor
        
        [<Test>]
        member x.TestRemoteResolve() = DomainTestCode.CanRemoteResolve(resetPersistor multiDatabasePersistor)
        
        [<Test>]
        member x.TestCanGetContextForCollection() = DomainCanGetContextForCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = DomainCanGetContextForNonGenericCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = DomainCanGetContextForArray multiDatabasePersistor
        
        [<Test>]
        member x.TestCanGetContextForType() = DomainCanGetContextForType multiDatabasePersistor
        
        // tests from codeonly 
        [<Test>]
        member x.TestCodeOnlyCreateEntityPersistor() = CodeOnlyTestCode.CanCreateEntityPersistor multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesGeneric() = CodeOnlyTestCode.CanGetInstancesGeneric multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesByType() = CodeOnlyTestCode.CanGetInstancesByType multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInstancesIsProxy() = CodeOnlyTestCode.CanGetInstancesIsProxy multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetObjectBySingleKey() = CodeOnlyTestCode.CanGetObjectBySingleKey multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateTransientObject() = CodeOnlyTestCode.CanCreateTransientObject multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarProperties() = CodeOnlyTestCode.CanSaveTransientObjectWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarPropertiesErrorAndReattempt() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithScalarPropertiesErrorAndIgnore() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyNavigateReferences() = CodeOnlyTestCode.CanNavigateReferences multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithPersistentReferenceProperty() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithPersistentReferenceProperty multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithTransientReferenceProperty() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientReferenceProperty multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithScalarProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithReferenceProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithReferenceProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithCollectionProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithCollectionProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyPersistingPersistedCalledForCreateInstance() = 
            CodeOnlyTestCode.CanPersistingPersistedCalledForCreateInstance multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyPersistingPersistedCalledForCreateInstanceWithReference() = 
            CodeOnlyTestCode.CanPersistingPersistedCalledForCreateInstanceWithCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatingUpdatedCalledForChange() = CodeOnlyTestCode.CanUpdatingUpdatedCalledForChange multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetKeyForType() = CodeOnlyTestCode.CanGetKeyForType multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateDomesticSubclass() = CodeOnlyTestCode.CanCreateDomesticSubclass multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateInternationalSubclass() = CodeOnlyTestCode.CanCreateInternationalSubclass multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCreateBaseClass() = CodeOnlyTestCode.CanCreateBaseClass multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetBaseClassGeneric() = CodeOnlyTestCode.CanGetBaseClassGeneric multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetBaseClassByType() = CodeOnlyTestCode.CanGetBaseClassByType multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetDomesticSubclassClassGeneric() = CodeOnlyTestCode.CanGetDomesticSubclassClassGeneric multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInternationalSubclassClassGeneric() = CodeOnlyTestCode.CanGetInternationalSubclassClassGeneric multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetDomesticSubclassClassByType() = CodeOnlyTestCode.CanGetDomesticSubclassClassByType multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetInternationalSubclassClassByType() = CodeOnlyTestCode.CanGetInternationalSubclassClassByType multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyNavigateToSubclass() = CodeOnlyTestCode.CanNavigateToSubclass multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetClassWithNonPersistedBase() = CodeOnlyTestCode.CanGetClassWithNonPersistedBase multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyGetNonPersistedClass() = CodeOnlyTestCode.CanGetNonPersistedClass multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyContainerInjectionCalledForNewInstance() = CodeOnlyTestCode.CanContainerInjectionCalledForNewInstance multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyContainerInjectionCalledForGetInstance() = CodeOnlyTestCode.CanContainerInjectionCalledForGetInstance multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientDomesticSubclasstWithScalarProperties() = 
            CodeOnlyTestCode.CanSaveTransientDomesticSubclasstWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientIntlSubclassWithScalarProperties() = 
            CodeOnlyTestCode.CanSaveTransientIntlSubclassWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentSubclassWithScalarProperties() = 
            CodeOnlyTestCode.CanUpdatePersistentSubclassWithScalarProperties multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlySaveTransientObjectWithTransientCollection() = 
            CodeOnlyTestCode.CanSaveTransientObjectWithTransientCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithScalarPropertiesAbort() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithScalarPropertiesAbort multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyUpdatePersistentObjectWithReferencePropertiesAbort() = 
            CodeOnlyTestCode.CanUpdatePersistentObjectWithReferencePropertiesAbort multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForCollection() = CodeOnlyCanGetContextForCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForNonGenericCollection() = CodeOnlyCanGetContextForNonGenericCollection multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForArray() = CodeOnlyCanGetContextForArray multiDatabasePersistor
        
        [<Test>]
        member x.TestCodeOnlyCanGetContextForType() = CodeOnlyCanGetContextForType multiDatabasePersistor
    end
