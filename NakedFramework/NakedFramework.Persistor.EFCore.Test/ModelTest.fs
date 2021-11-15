// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedFramework.ModelTest

open NakedObjects
open NUnit.Framework
open SimpleDatabase
open System
open NakedFramework.Persistor.EFCore.Configuration
open Microsoft.EntityFrameworkCore
open NakedFramework.Architecture.Component
open NakedObjects.TestTypes
open ModelTestCode
open TestCode

let efCorePersistor = 
    let c = new EFCorePersistorConfiguration()
    c.MaximumCommitCycles <- 10
    let f = Func<DbContext> (fun () -> 
        let c = new EFCoreSimpleDatabaseDbContext(csMF)
        c.Create()
        c :> DbContext)
    c.Contexts <- [| f |]
    let p = getEFCoreObjectStore c
    setupEFCorePersistorForTesting p

[<TestFixture>]
type ModelTests() = 
    class
        
        [<OneTimeSetUp>]
        member x.Setup() = ()
        
        member x.persistor = efCorePersistor :> IObjectStore

        [<Test>]
        member x.TestCreateEntityPersistor() = CanCreateEntityPersistor x.persistor
        
        [<Test>]
        member x.TestCanGetInstanceWithComplexType() = CanGetInstanceWithComplexType x.persistor
        
        [<Test>]
        member x.TestCanUpdateInstanceWithComplexType() = CanUpdateInstanceWithComplexType x.persistor
        
        [<Test>]
        [<Ignore("pending investigation")>]
        member x.TestCreateInstanceWithComplexType() = CanCreateInstanceWithComplexType x.persistor
        
        [<Test>]
        member x.TestCanGetContextForCollection() = ModelCanGetContextForCollection x.persistor
        
        [<Test>]
        member x.TestCanGetContextForNonGenericCollection() = ModelCanGetContextForNonGenericCollection x.persistor
        
        [<Test>]
        member x.TestCanGetContextForArray() = ModelCanGetContextForArray x.persistor
        
        [<Test>]
        member x.TestCanGetContextForType() = ModelCanGetContextForType x.persistor
        
        [<Test>]
        member x.TestCanGetContextForComplexType() = ModelCanGetContextForComplexType x.persistor
        
        [<Test>]
        member x.TestCanInjectContainerOnNewInstance() = CanInjectContainerOnNewInstance x.persistor
        
        [<Test>]
        member x.TestCanInjectServiceOnNewInstance() = CanInjectServiceOnNewInstance x.persistor
    end