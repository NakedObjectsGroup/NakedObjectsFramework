// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.ModelTest
open NUnit.Framework
open TestTypes
open NakedObjects.EntityObjectStore
open TestCode
open ModelTestCode
open NakedObjects.Core.Context

let persistor =
    let p = new EntityObjectStore([|(box ModelConfig :?> EntityContextConfiguration)|], new EntityOidGenerator(NakedObjectsContext.Reflector), NakedObjectsContext.Reflector)
    setupPersistorForInjectorTesting p


[<TestFixture>]
type ModelTests() = class              
    [<TestFixtureSetUp>] member x.Setup() = ModelSetup()
    [<Test>] member x.TestCreateEntityPersistor() = CanCreateEntityPersistor persistor     
    [<Test>] member x.TestCanGetInstanceWithComplexType() = CanGetInstanceWithComplexType persistor
    [<Test>] member x.TestCanUpdateInstanceWithComplexType() = CanUpdateInstanceWithComplexType persistor
    [<Test>] member x.TestCreateInstanceWithComplexType() = CanCreateInstanceWithComplexType persistor
    [<Test>] member x.TestCanGetContextForCollection() = ModelCanGetContextForCollection  persistor
    [<Test>] member x.TestCanGetContextForNonGenericCollection() = ModelCanGetContextForNonGenericCollection  persistor
    [<Test>] member x.TestCanGetContextForArray() = ModelCanGetContextForArray  persistor
    [<Test>] member x.TestCanGetContextForType() = ModelCanGetContextForType  persistor
    [<Test>] member x.TestCanGetContextForComplexType() = ModelCanGetContextForComplexType  persistor      
    [<Test>] member x.TestCanInjectContainerOnNewInstance() = CanInjectContainerOnNewInstance persistor   
    [<Test>] member x.TestCanInjectServiceOnNewInstance()= CanInjectServiceOnNewInstance persistor
end