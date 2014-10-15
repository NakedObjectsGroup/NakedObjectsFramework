// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedObjects.ModelTest
open NUnit.Framework
open TestTypes
open NakedObjects.EntityObjectStore
open ModelTestCode
open NakedObjects.Core.Context
open NakedObjects.Core.Security
open System.Security.Principal
open NakedObjects.Reflector.DotNet
open Moq
open NakedObjects.Architecture.Reflect
open NakedObjects.Architecture.Persist

let persistor =
    let c = new EntityObjectStoreConfiguration()
    let s = new SimpleSession(new GenericPrincipal(new GenericIdentity(""), [||]))
    let u = new SimpleUpdateNotifier()
    let i = new DotNetDomainObjectContainerInjector()
    let r = (new Mock<INakedObjectReflector>()).Object
    let m = mockMetamodelManager.Object
    let nom = (new Mock<INakedObjectManager>()).Object

    c.UsingEdmxContext "Model1Container" |> ignore
    let p = new EntityObjectStore(s, u, c, new EntityOidGenerator(m), m, i, nom)
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