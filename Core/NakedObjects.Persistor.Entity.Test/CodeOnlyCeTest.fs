// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

module NakedObjects.CodeOnlyCeTest
open NUnit.Framework
open CodeOnlyTestCode
open System
open NakedObjects.EntityObjectStore
open TestCode
open TestTypes
open NakedObjects.Core.Context
open NakedObjects.Core.Security
open System.Security.Principal
open NakedObjects.Reflector.DotNet

let codeOnlyCePersistor =
    let r = new MockReflector()  
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    c.ContextConfiguration <- [|(box (CodeFirstCeConfig "CodeOnlyCeTests") :?> EntityContextConfiguration)|]
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(r), r, i)
    setupPersistorForTesting p

[<TestFixture>]
type CodeOnlyCeTests() = class                      
    [<TestFixtureSetUp>] member x.Setup() = CodeFirstCeSetup()                  
    [<Test>] member x.TestCreateEntityPersistor() = CanCreateEntityPersistor codeOnlyCePersistor             
    [<Test>] member x.TestGetInstancesGeneric() = CanGetInstancesGeneric codeOnlyCePersistor           
    [<Test>] member x.TestGetInstancesByType() = CanGetInstancesByType codeOnlyCePersistor                 
    [<Test>] member x.TestGetInstancesIsProxy() = CanGetInstancesIsProxy codeOnlyCePersistor  
    [<Test>] member x.TestGetObjectBySingleKey() = CanGetObjectBySingleKey codeOnlyCePersistor    
    [<Test>] member x.TestCreateTransientObject() = CodeOnlyTestCode.CanCreateTransientObject codeOnlyCePersistor            
    [<Test>] member x.TestSaveTransientObjectWithScalarProperties() = CanSaveTransientObjectWithScalarProperties codeOnlyCePersistor       
    [<Test>] member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt codeOnlyCePersistor                   
    [<Test>] member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore codeOnlyCePersistor
    [<Test>] member x.TestNavigateReferences() = CanNavigateReferences codeOnlyCePersistor        
    [<Test>] member x.TestSaveTransientObjectWithPersistentReferenceProperty() = CanSaveTransientObjectWithPersistentReferenceProperty codeOnlyCePersistor      
    [<Test>] member x.TestSaveTransientObjectWithTransientReferenceProperty() = CanSaveTransientObjectWithTransientReferenceProperty codeOnlyCePersistor
    [<Test>] member x.TestUpdatePersistentObjectWithScalarProperties() = CanUpdatePersistentObjectWithScalarProperties codeOnlyCePersistor          
    [<Test>] member x.TestUpdatePersistentObjectWithReferenceProperties() = CanUpdatePersistentObjectWithReferenceProperties codeOnlyCePersistor                        
    [<Test>] member x.TestUpdatePersistentObjectWithCollectionProperties() = CanUpdatePersistentObjectWithCollectionProperties codeOnlyCePersistor                    
    [<Test>] member x.TestPersistingPersistedCalledForCreateInstance() = CanPersistingPersistedCalledForCreateInstance codeOnlyCePersistor 
    [<Test>] member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = CanPersistingPersistedCalledForCreateInstanceWithCollection codeOnlyCePersistor
    [<Test>] member x.TestUpdatingUpdatedCalledForChange() = CanUpdatingUpdatedCalledForChange codeOnlyCePersistor           
    [<Test>] member x.TestGetKeyForType() = CanGetKeyForType codeOnlyCePersistor         
    [<Test>] member x.TestCreateDomesticSubclass() = CanCreateDomesticSubclass codeOnlyCePersistor      
    [<Test>] member x.TestCreateInternationalSubclass() = CanCreateInternationalSubclass codeOnlyCePersistor        
    [<Test>] member x.TestCreateBaseClass() = CanCreateBaseClass codeOnlyCePersistor    
    [<Test>] member x.TestGetBaseClassGeneric() = CanGetBaseClassGeneric codeOnlyCePersistor  
    [<Test>] member x.TestGetBaseClassByType() = CanGetBaseClassByType codeOnlyCePersistor
    [<Test>] member x.TestGetDomesticSubclassClassGeneric() = CanGetDomesticSubclassClassGeneric codeOnlyCePersistor      
    [<Test>] member x.TestGetInternationalSubclassClassGeneric() = CanGetInternationalSubclassClassGeneric codeOnlyCePersistor         
    [<Test>] member x.TestGetDomesticSubclassClassByType() = CanGetDomesticSubclassClassByType codeOnlyCePersistor
    [<Test>] member x.TestGetInternationalSubclassClassByType() = CanGetInternationalSubclassClassByType codeOnlyCePersistor     
    [<Test>] member x.TestNavigateToSubclass() = CanNavigateToSubclass codeOnlyCePersistor
    [<Test>] member x.TestGetClassWithNonPersistedBase() = CanGetClassWithNonPersistedBase codeOnlyCePersistor            
    [<Test>] member x.TestGetNonPersistedClass() = CanGetNonPersistedClass codeOnlyCePersistor
    [<Test>] member x.TestContainerInjectionCalledForNewInstance() = CanContainerInjectionCalledForNewInstance codeOnlyCePersistor
    [<Test>] member x.TestContainerInjectionCalledForGetInstance() = CanContainerInjectionCalledForGetInstance codeOnlyCePersistor
    [<Test>] member x.TestSaveTransientDomesticSubclasstWithScalarProperties() = CanSaveTransientDomesticSubclasstWithScalarProperties codeOnlyCePersistor  
    [<Test>] member x.TestSaveTransientIntlSubclassWithScalarProperties() = CanSaveTransientIntlSubclassWithScalarProperties codeOnlyCePersistor
    [<Test>] member x.TestUpdatePersistentSubclassWithScalarProperties() = CanUpdatePersistentSubclassWithScalarProperties codeOnlyCePersistor
    [<Test>] member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies codeOnlyCePersistor                        
    [<Test>] member x.TestSaveTransientObjectWithTransientCollection() = CanSaveTransientObjectWithTransientCollection codeOnlyCePersistor
    [<Test>] member x.TestUpdatePersistentObjectWithScalarPropertiesAbort() =  CanUpdatePersistentObjectWithScalarPropertiesAbort codeOnlyCePersistor
    [<Test>] member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = CanUpdatePersistentObjectWithReferencePropertiesAbort codeOnlyCePersistor
    [<Test>] member x.TestCanGetContextForCollection() = CodeOnlyCanGetContextForCollection  codeOnlyCePersistor
    [<Test>] member x.TestCanGetContextForNonGenericCollection() = CodeOnlyCanGetContextForNonGenericCollection  codeOnlyCePersistor
    [<Test>] member x.TestCanGetContextForArray() = CodeOnlyCanGetContextForArray  codeOnlyCePersistor
    [<Test>] member x.TestCanGetContextForType() = CodeOnlyCanGetContextForType  codeOnlyCePersistor
    [<Test>] member x.GetKeysReturnsKey() = GetKeysReturnsKey codeOnlyCePersistor
 end

