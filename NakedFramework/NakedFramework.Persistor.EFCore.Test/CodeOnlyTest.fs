// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedFramework.CodeOnlyTest

open NakedObjects
open CodeOnlyTestCode
open NakedFramework.Persistor.Entity.Configuration
open NUnit.Framework
open TestCode
open TestTypes
open NakedFramework.Persistor.EFCore.Configuration
open System
open Microsoft.EntityFrameworkCore
open TestCodeOnly
open System.Data.Common
open Microsoft.Data.SqlClient

let codeOnlyPersistor = 
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f = Func<DbContext> (fun () -> 
        let c = new EFCoreCodeFirstContext(csEFCO)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

[<TestFixture>]
type CodeOnlyTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() =
            let c = new EFCoreCodeFirstContext(csEFCO)
            c.Delete()

        [<OneTimeTearDown>]
        member x.TearDown() =
            let c = new EFCoreCodeFirstContext(csEFCO)
            c.Delete()
                
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
        [<Ignore("FIX")>]
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
        [<Ignore("FIX")>]
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
        [<Ignore("FIX")>]
        member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = 
            CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies codeOnlyPersistor
        
        [<Test>]
        member x.TestSaveTransientObjectWithTransientCollection() = CanSaveTransientObjectWithTransientCollection codeOnlyPersistor
        
        [<Test>]
        [<Ignore("FIX")>]
        member x.TestUpdatePersistentObjectWithScalarPropertiesAbort() = CanUpdatePersistentObjectWithScalarPropertiesAbort codeOnlyPersistor
        
        [<Test>]
         [<Ignore("FIX")>]
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

