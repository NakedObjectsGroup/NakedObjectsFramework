// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedObjects.Rest.Test.Helpers

open NakedObjects.Facade
open NakedObjects.Rest
open RestfulObjects.Test.Data

//#if APPVEYOR 

let csRTA = @"Data Source=.\SQL2017;Initial Catalog=RestTestA;Integrated Security=True;"
let csRTB = @"Data Source=.\SQL2017;Initial Catalog=RestTestB;Integrated Security=True;"
let csRTZ = @"Data Source=.\SQL2017;Initial Catalog=RestTestZ;Integrated Security=True;"
let csRTD = @"Data Source=.\SQL2017;Initial Catalog=RestTestD;Integrated Security=True;"
let csRTDT = @"Data Source=.\SQL2017;Initial Catalog=RestTestDT;Integrated Security=True;"

//#else

//let csRTA = @"Data Source=.\SQLEXPRESS;Initial Catalog=RestTestA;Integrated Security=True;"
//let csRTB = @"Data Source=.\SQLEXPRESS;Initial Catalog=RestTestB;Integrated Security=True;"
//let csRTZ = @"Data Source=.\SQLEXPRESS;Initial Catalog=RestTestZ;Integrated Security=True;"
//let csRTD = @"Data Source=.\SQLEXPRESS;Initial Catalog=RestTestD;Integrated Security=True;"
//let csRTDT = @"Data Source=.\SQLEXPRESS;Initial Catalog=RestTestDT;Integrated Security=True;"

//#endif


type CodeFirstInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<CodeFirstContextLocal>()

let CodeFirstSetup() = 
    System.Data.Entity.Database.SetInitializer(new CodeFirstInitializer())
    ()

let mapper = new TestTypeCodeMapper()
let keyMapper = new TestKeyCodeMapper()

type RestfulObjectsController(ff: IFrameworkFacade) = 
    class
        inherit RestfulObjectsControllerBase(ff)
 end