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
open NakedObjects.Core.Security
open System.Security.Principal
open NakedObjects.Reflector.DotNet
open Moq
open NakedObjects.Architecture.Reflect

let persistor =
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    let r = (new Mock<INakedObjectReflector>()).Object
    let m = (new Mock<IMetadata>()).Object

    c.UsingEdmxContext "Model1Container" |> ignore
    //c.ContextConfiguration <- [|(box ModelConfig :?> EntityContextConfiguration)|]
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(m), r, i)
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