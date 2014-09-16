// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedObjects.Rest.Test.Nof4TypesDomainType

open NUnit.Framework
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open RestfulObjects.Mvc
open RestfulObjects.Mvc.Media
open System
open RestfulObjects.Snapshot.Utility
open System.Web.Http
open Microsoft.Practices.Unity
open NakedObjects.EntityObjectStore
open RestfulObjects.Test.Data
open NakedObjects.Surface.Nof4.Implementation
open NakedObjects.Surface.Nof4.Utility
open NakedObjects.Surface
open MvcTestApp.Controllers
open NakedObjects.Rest.Test.RestTestsHelpers

[<TestFixture>]
type Nof4TestsTypeDomainType() = 
    class
        inherit NakedObjects.Xat.AcceptanceTestCase()
        
        override x.RegisterTypes(container) = 
            base.RegisterTypes(container)
            let config = new EntityObjectStoreConfiguration()
            let f = (fun () -> new CodeFirstContext("RestTest") :> Data.Entity.DbContext)
            config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
            
            container.RegisterInstance(typeof<EntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager())) |> ignore
            container.RegisterType(typeof<IOidStrategy>, typeof<ExternalOid>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType(typeof<INakedObjectsSurface>, typeof<NakedObjectsSurface>, null, (new PerResolveLifetimeManager())) |> ignore
            ()
        
        [<TestFixtureSetUp>]
        member x.FixtureSetup() = 
            CodeFirstSetup()
            NakedObjects.Xat.AcceptanceTestCase.InitializeNakedObjectsFramework(x)

        [<TestFixtureTearDown>]
        member x.FixtureTearDown() = NakedObjects.Xat.AcceptanceTestCase.CleanupNakedObjectsFramework(x)
        
        [<SetUp>]
        member x.Setup() = 
            x.StartTest()
            UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
            RestfulObjectsControllerBase.IsReadOnly <- false
            GlobalConfiguration.Configuration.Formatters.[0] <- new JsonNetFormatter(null)
            RestTestFunctions.ctt <- fun code -> mapper.TypeStringFromCode(code)
            RestTestFunctions.ttc <- fun typ -> mapper.CodeFromTypeString(typ)
            RestTestFunctions.ctk <- fun code -> keyMapper.KeyStringFromCode(code)
            RestTestFunctions.ktc <- fun key -> keyMapper.CodeFromKeyString(key)
        
        override x.MenuServices : IServicesInstaller = 
            box (new ServicesInstaller([| box (new RestDataRepository())
                                          box (new WithActionService()) |])) :?> IServicesInstaller
        
        override x.SystemServices : IServicesInstaller = 
            box (new ServicesInstaller([| box (new TestTypeCodeMapper())
                                          box (new TestKeyCodeMapper()) |])) :?> IServicesInstaller
        
        override x.ContributedActions : IServicesInstaller = box (new ServicesInstaller([| box (new ContributorService()) |])) :?> IServicesInstaller
        member x.api = x.GetConfiguredContainer().Resolve<RestfulObjectsController>()
        
        [<Test>]
        member x.GetDomainTypes() = DomainTypes20.GetDomainTypesDomainType x.api
        
        [<Test>]
        member x.GetDomainTypesWithMediaType() = DomainTypes20.GetDomainTypesWithMediaTypeDomainType x.api
        
        [<Test>]
        member x.NotAcceptableGetDomainTypes() = DomainTypes20.NotAcceptableGetDomainTypes x.api
    end
