// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.Helpers

open RestfulObjects.Test.Data
open RestfulObjects.Test.Data.Context
open Microsoft.Extensions.Logging
open NakedFramework.Rest.Configuration
open NakedFramework.Rest.API
open NakedFramework.Facade.Interface

let appveyorServer = @"Data Source=(local)\SQL2017;"
let localServer = @"Data Source=(localdb)\MSSQLLocalDB;"

#if APPVEYOR 
let server = appveyorServer
#else
let server = localServer
#endif

let csRTA = server + @"Initial Catalog=RestTestA;Integrated Security=True;"
let csRTB = server + @"Initial Catalog=RestTestB;Integrated Security=True;"
let csRTZ = server + @"Initial Catalog=RestTestZ;Integrated Security=True;"
let csRTD = server + @"Initial Catalog=RestTestD;Integrated Security=True;"
let csRTDT = server + @"Initial Catalog=RestTestDT;Integrated Security=True;"

type CodeFirstInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<CodeFirstContextLocal>()

let CodeFirstSetup() = 
    System.Data.Entity.Database.SetInitializer(new CodeFirstInitializer())
    ()

let mapper = new TestTypeCodeMapper()
let keyMapper = new TestKeyCodeMapper()

type RestfulObjectsController(ff: IFrameworkFacade, l : ILogger<RestfulObjectsControllerBase>, lf : ILoggerFactory, c : IRestfulObjectsConfiguration) = 
    class
        inherit RestfulObjectsControllerBase(ff, l, lf, c)
 end