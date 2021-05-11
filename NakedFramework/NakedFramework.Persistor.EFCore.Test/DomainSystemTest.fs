// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedFramework.DomainSystemTest

open NUnit.Framework
open System
open NakedFramework.DependencyInjection.Extensions
open Microsoft.Extensions.Configuration
open NakedFramework.Persistor.EFCore.Extensions
open Microsoft.EntityFrameworkCore
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NakedObjects.DomainSystemTest

[<TestFixture>]
type EFCoreDomainSystemTests() = 
    inherit DomainSystemTests()

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    member x.ContextInstallers = [|Func<IConfiguration, DbContext> (fun (c) -> 
                  let context = new EFCoreAdventureWorksEntities(NakedObjects.TestTypes.csAWMARS) 
                  context.Create()
                  (context :> DbContext))|]
        
    member x.EFCorePersistorOptions = Action<EFCorePersistorOptions> (fun  (options) -> options.ContextInstallers <- x.ContextInstallers)


