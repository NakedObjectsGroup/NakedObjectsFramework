// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.ModelTest

open ModelTestCode
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.TestTypes
open NUnit.Framework
open SimpleDatabase
open System
open TestCode

let persistor = 
    let c = new EntityObjectStoreConfiguration()
    let f = (fun () -> new SimpleDatabaseDbContext(csMF) :> Data.Entity.DbContext)
    c.UsingContext(Func<Data.Entity.DbContext>(f)) |> ignore
    let p = getEntityObjectStore c
    setupPersistorForInjectorTesting p

[<TestFixture>]
type ModelTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = ()
        
        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor persistor
        
        [<Test>]
        member x.TestCanGetInstanceWithComplexType() = CanGetInstanceWithComplexType persistor
        
        [<Test>]
        member x.TestCanUpdateInstanceWithComplexType() = CanUpdateInstanceWithComplexType persistor
        
        [<Test>]
        member x.TestCreateInstanceWithComplexType() = CanCreateInstanceWithComplexType persistor
        
        [<Test>]
        member x.TestCanGetContextForCollection() = ModelCanGetContextForCollection persistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = ModelCanGetContextForNonGenericCollection persistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = ModelCanGetContextForArray persistor
        
        [<Test>]
        member x.TestCanGetContextForType() = ModelCanGetContextForType persistor
        
        [<Test>]
        member x.TestCanGetContextForComplexType() = ModelCanGetContextForComplexType persistor
        
        [<Test>]
        member x.TestCanInjectContainerOnNewInstance() = CanInjectContainerOnNewInstance persistor
        
        [<Test>]
        member x.TestCanInjectServiceOnNewInstance() = CanInjectServiceOnNewInstance persistor
    end