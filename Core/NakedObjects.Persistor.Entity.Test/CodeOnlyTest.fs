// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.CodeOnlyTest
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
open Moq
open NakedObjects.Architecture.Reflect
open NakedObjects.Architecture.Persist

let codeOnlyPersistor =
    let r = mockMetamodelManager.Object
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    let nom = (new Mock<INakedObjectManager>()).Object

    c.UsingCodeFirstContext ((CodeFirstConfig "CodeOnlyTests").DbContext) |> ignore
    //c.ContextConfiguration <- [|(box (CodeFirstConfig "CodeOnlyTests") :?> EntityContextConfiguration)|]
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(r),r,i, nom)
    setupPersistorForTesting p

[<TestFixture>]
type CodeOnlyTests() = class                      
    [<TestFixtureSetUp>] member x.Setup() = CodeFirstSetup()                  
    [<Test>] member x.TestCreateEntityPersistor() = CanCreateEntityPersistor codeOnlyPersistor             
    [<Test>] member x.TestGetInstancesGeneric() = CanGetInstancesGeneric codeOnlyPersistor           
    [<Test>] member x.TestGetInstancesByType() = CanGetInstancesByType codeOnlyPersistor                 
    [<Test>] member x.TestGetInstancesIsProxy() = CanGetInstancesIsProxy codeOnlyPersistor  
    [<Test>] member x.TestGetObjectBySingleKey() = CanGetObjectBySingleKey codeOnlyPersistor    
    [<Test>] member x.TestCreateTransientObject() = CodeOnlyTestCode.CanCreateTransientObject codeOnlyPersistor            
    [<Test>] member x.TestSaveTransientObjectWithScalarProperties() = CanSaveTransientObjectWithScalarProperties codeOnlyPersistor       
    [<Test>] member x.TestSaveTransientObjectWithScalarPropertiesErrorAndReattempt() = CanSaveTransientObjectWithScalarPropertiesErrorAndReattempt codeOnlyPersistor                   
    [<Test>] member x.TestSaveTransientObjectWithScalarPropertiesErrorAndIgnore() = CanSaveTransientObjectWithScalarPropertiesErrorAndIgnore codeOnlyPersistor
    [<Test>] member x.TestNavigateReferences() = CanNavigateReferences codeOnlyPersistor        
    [<Test>] member x.TestSaveTransientObjectWithPersistentReferenceProperty() = CanSaveTransientObjectWithPersistentReferenceProperty codeOnlyPersistor      
    [<Test>] member x.TestSaveTransientObjectWithTransientReferenceProperty() = CanSaveTransientObjectWithTransientReferenceProperty codeOnlyPersistor
    [<Test>] member x.TestUpdatePersistentObjectWithScalarProperties() = CanUpdatePersistentObjectWithScalarProperties codeOnlyPersistor          
    [<Test>] member x.TestUpdatePersistentObjectWithReferenceProperties() = CanUpdatePersistentObjectWithReferenceProperties codeOnlyPersistor                        
    [<Test>] member x.TestUpdatePersistentObjectWithCollectionProperties() = CanUpdatePersistentObjectWithCollectionProperties codeOnlyPersistor                    
    [<Test>] member x.TestPersistingPersistedCalledForCreateInstance() = CanPersistingPersistedCalledForCreateInstance codeOnlyPersistor 
    [<Test>] member x.TestPersistingPersistedCalledForCreateInstanceWithReference() = CanPersistingPersistedCalledForCreateInstanceWithCollection codeOnlyPersistor
    [<Test>] member x.TestUpdatingUpdatedCalledForChange() = CanUpdatingUpdatedCalledForChange codeOnlyPersistor           
    [<Test>] member x.TestGetKeyForType() = CanGetKeyForType codeOnlyPersistor         
    [<Test>] member x.TestCreateDomesticSubclass() = CanCreateDomesticSubclass codeOnlyPersistor      
    [<Test>] member x.TestCreateInternationalSubclass() = CanCreateInternationalSubclass codeOnlyPersistor        
    [<Test>] member x.TestCreateBaseClass() = CanCreateBaseClass codeOnlyPersistor    
    [<Test>] member x.TestGetBaseClassGeneric() = CanGetBaseClassGeneric codeOnlyPersistor  
    [<Test>] member x.TestGetBaseClassByType() = CanGetBaseClassByType codeOnlyPersistor
    [<Test>] member x.TestGetDomesticSubclassClassGeneric() = CanGetDomesticSubclassClassGeneric codeOnlyPersistor      
    [<Test>] member x.TestGetInternationalSubclassClassGeneric() = CanGetInternationalSubclassClassGeneric codeOnlyPersistor         
    [<Test>] member x.TestGetDomesticSubclassClassByType() = CanGetDomesticSubclassClassByType codeOnlyPersistor
    [<Test>] member x.TestGetInternationalSubclassClassByType() = CanGetInternationalSubclassClassByType codeOnlyPersistor     
    [<Test>] member x.TestNavigateToSubclass() = CanNavigateToSubclass codeOnlyPersistor
    [<Test>] member x.TestGetClassWithNonPersistedBase() = CanGetClassWithNonPersistedBase codeOnlyPersistor            
    [<Test>] member x.TestGetNonPersistedClass() = CanGetNonPersistedClass codeOnlyPersistor
    [<Test>] member x.TestContainerInjectionCalledForNewInstance() = CanContainerInjectionCalledForNewInstance codeOnlyPersistor
    [<Test>] member x.TestContainerInjectionCalledForGetInstance() = CanContainerInjectionCalledForGetInstance codeOnlyPersistor
    [<Test>] member x.TestSaveTransientDomesticSubclasstWithScalarProperties() = CanSaveTransientDomesticSubclasstWithScalarProperties codeOnlyPersistor  
    [<Test>] member x.TestSaveTransientIntlSubclassWithScalarProperties() = CanSaveTransientIntlSubclassWithScalarProperties codeOnlyPersistor
    [<Test>] member x.TestUpdatePersistentSubclassWithScalarProperties() = CanUpdatePersistentSubclassWithScalarProperties codeOnlyPersistor
    [<Test>] member x.TestSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies() = CanSaveTransientObjectWithTransientReferencePropertyAndConfirmProxies codeOnlyPersistor                        
    [<Test>] member x.TestSaveTransientObjectWithTransientCollection() = CanSaveTransientObjectWithTransientCollection codeOnlyPersistor
    [<Test>] member x.TestUpdatePersistentObjectWithScalarPropertiesAbort() =  CanUpdatePersistentObjectWithScalarPropertiesAbort codeOnlyPersistor
    [<Test>] member x.TestUpdatePersistentObjectWithReferencePropertiesAbort() = CanUpdatePersistentObjectWithReferencePropertiesAbort codeOnlyPersistor
    [<Test>] member x.TestCanGetContextForCollection() = CodeOnlyCanGetContextForCollection  codeOnlyPersistor
    [<Test>] member x.TestCanGetContextForNonGenericCollection() = CodeOnlyCanGetContextForNonGenericCollection  codeOnlyPersistor
    [<Test>] member x.TestCanGetContextForArray() = CodeOnlyCanGetContextForArray  codeOnlyPersistor
    [<Test>] member x.TestCanGetContextForType() = CodeOnlyCanGetContextForType  codeOnlyPersistor
    [<Test>] member x.GetKeysReturnsKey() = GetKeysReturnsKey codeOnlyPersistor
 end
