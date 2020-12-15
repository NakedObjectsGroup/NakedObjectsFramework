﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.CodeOnlyTest

open CodeOnlyTestCode
open NakedObjects.Persistor.Entity.Configuration
open NUnit.Framework
open TestCode
open TestTypes

let codeOnlyPersistor = 
    EntityObjectStoreConfiguration.NoValidate <- true
    let c = new EntityObjectStoreConfiguration()
    c.UsingContext((CodeFirstConfig csCO).DbContext) |> ignore
    let p = getEntityObjectStore c
    setupPersistorForTesting p

[<TestFixture>]
type CodeOnlyTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = CodeFirstSetup()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor codeOnlyPersistor
        
        [<Test>]
        member x.TestGetInstancesGeneric() = CanGetInstancesGeneric codeOnlyPersistor
        
        [<Test>]
        member x.TestGetInstancesByType() = CanGetInstancesByType codeOnlyPersistor
        
        [<Test>]
        member x.TestGetInstancesIsProxy() = CanGetInstancesIsProxy codeOnlyPersistor
        
        [<Test>]
        member x.TestGetObjectBySingleKey() = CanGetObjectBySingleKey codeOnlyPersistor
        
        [<Test>]
        member x.TestCreateTransientObject() = CodeOnlyTestCode.CanCreateTransientObject codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarProperties() = CanSaveTransientObjectWithScalarProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore codeOnlyPersistor
        
        [<Test>]
        member x.TestNavigateReferences() = CanNavigateReferences codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithPersistentReferenceProperty() = CanSaveTransientObjectWithPersistentReferenceProperty codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferenceProperty() = CanSaveTransientObjectWithTransientReferenceProperty codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarProperties() = CanUpdatePersistentObjectWithScalarProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferenceProperties() = CanUpdatePersistentObjectWithReferenceProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithCollectionProperties() = CanUpdatePersistentObjectWithCollectionProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstance() = CanPersistingPersistedCalledForCreateInstance codeOnlyPersistor
        
        [<Test>]
        member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = CanPersistingPersistedCalledForCreateInstanceWithCollection codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatingUpdatedCalledForChange() = CanUpdatingUpdatedCalledForChange codeOnlyPersistor
        
        [<Test>]
        member x.TestGetKeyForType() = CanGetKeyForType codeOnlyPersistor
        
        [<Test>]
        member x.TestCreateDomesticSubclass() = CanCreateDomesticSubclass codeOnlyPersistor
        
        [<Test>]
        member x.TestCreateInternationalSubclass() = CanCreateInternationalSubclass codeOnlyPersistor
        
        [<Test>]
        member x.TestCreateBaseClass() = CanCreateBaseClass codeOnlyPersistor
        
        [<Test>]
        member x.TestGetBaseClassGeneric() = CanGetBaseClassGeneric codeOnlyPersistor
        
        [<Test>]
        member x.TestGetBaseClassByType() = CanGetBaseClassByType codeOnlyPersistor
        
        [<Test>]
        member x.TestGetDomesticSubclassClassGeneric() = CanGetDomesticSubclassClassGeneric codeOnlyPersistor
        
        [<Test>]
        member x.TestGetInternationalSubclassClassGeneric() = CanGetInternationalSubclassClassGeneric codeOnlyPersistor
        
        [<Test>]
        member x.TestGetDomesticSubclassClassByType() = CanGetDomesticSubclassClassByType codeOnlyPersistor
        
        [<Test>]
        member x.TestGetInternationalSubclassClassByType() = CanGetInternationalSubclassClassByType codeOnlyPersistor
        
        [<Test>]
        member x.TestNavigateToSubclass() = CanNavigateToSubclass codeOnlyPersistor
        
        [<Test>]
        member x.TestGetClassWithNonPersistedBase() = CanGetClassWithNonPersistedBase codeOnlyPersistor
        
        [<Test>]
        member x.TestGetNonPersistedClass() = CanGetNonPersistedClass codeOnlyPersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForNewInstance() = CanContainerInjectionCalledForNewInstance codeOnlyPersistor
        
        [<Test>]
        member x.TestContainerInjectionCalledForGetInstance() = CanContainerInjectionCalledForGetInstance codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientDomesticSubclasstWithScalarProperties() = CanSaveTransientDomesticSubclasstWithScalarProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientIntlSubclassWithScalarProperties() = CanSaveTransientIntlSubclassWithScalarProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentSubclassWithScalarProperties() = CanUpdatePersistentSubclassWithScalarProperties codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientCollection() = CanSaveTransientObjectWithTransientCollection codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesAbort() = CanUpdatePersistentObjectWithScalarPropertiesAbort codeOnlyPersistor
        
        [<Test>]
        member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = CanUpdatePersistentObjectWithReferencePropertiesAbort codeOnlyPersistor
        
        [<Test>]
        member x.TestCanGetContextForCollection() = CodeOnlyCanGetContextForCollection codeOnlyPersistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = CodeOnlyCanGetContextForNonGenericCollection codeOnlyPersistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = CodeOnlyCanGetContextForArray codeOnlyPersistor
        
        [<Test>]
        member x.TestCanGetContextForType() = CodeOnlyCanGetContextForType codeOnlyPersistor
        
        [<Test>]
        member x.GetKeysReturnsKey() = GetKeysReturnsKey codeOnlyPersistor
    end

