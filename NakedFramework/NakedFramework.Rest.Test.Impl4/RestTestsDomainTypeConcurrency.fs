// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.DomainTypeConcurrency

open NUnit.Framework
open NakedObjects.Rest
open NakedObjects.Rest.Test
open System
open RestfulObjects.Test.Data
open NakedObjects.Facade.Impl.Implementation
open NakedObjects.Facade.Impl.Utility
open NakedObjects.Rest.Test.Helpers
open NakedObjects.Rest.Test.Functions
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Facade
open NakedObjects.Facade.Translation
open NakedObjects.Facade.Impl
open NakedObjects.Facade.Interface
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open NakedObjects.Rest.Snapshot.Utility
open Microsoft.Extensions.Configuration
open NakedFramework
open NakedFramework.Xat.TestCase

type NullStringHasher() = 
    interface IStringHasher with
        member x.GetHash(toHash : string) : string = null;
    end

[<TestFixture>]
type Tests() = 
    class
        inherit AcceptanceTestCase()
        
        override x.ObjectTypes = 
            [| typeof<Immutable>
               typeof<WithActionViewModel>
               typeof<WithCollectionViewModel>
               typeof<WithValueViewModel>
               typeof<WithNestedViewModel>
               typeof<RedirectedObject>
               typeof<WithScalars>
               typeof<VerySimple>
               typeof<VerySimpleEager>
               typeof<WithAction>
               typeof<WithActionObject>
               typeof<WithAttachments>
               typeof<WithCollection>
               typeof<WithDateTimeKey>
               typeof<WithGuidKey>
               typeof<WithError>
               typeof<WithGetError>
               typeof<WithNestedViewModel>
               typeof<WithReference>
               typeof<WithReferenceViewModel>
               typeof<WithValueViewModelEdit>
               typeof<WithNestedViewModelEdit>
               typeof<WithReferenceViewModelEdit>
               typeof<MostSimple>
               typeof<MostSimpleViewModel>
               typeof<WithValue>
               typeof<WithValuePersist>
               typeof<MostSimplePersist>
               typeof<WithReferencePersist>
               typeof<WithCollectionPersist>
               typeof<TestEnum>
               typeof<FormViewModel>
               typeof<SetWrapper<MostSimple>> |]

        override x.Services = 
           [| typeof<RestDataRepository>
              typeof<WithActionService>
              typeof<ContributorService>
              typeof<TestTypeCodeMapper>
              typeof<TestKeyCodeMapper> |]

        override x.MainMenus (factory : IMenuFactory)  = 
               let menu1 = factory.NewMenu<RestDataRepository>(true)
               let menu2 = factory.NewMenu<WithActionService>(true)
               let menu3 = factory.NewMenu<ContributorService>(true)
               let menu4 = factory.NewMenu<TestTypeCodeMapper>(true)
               let menu5 = factory.NewMenu<TestKeyCodeMapper>(true)
               [| menu1; menu2; menu3; menu4; menu5 |]

        override x.EnforceProxies = false
        
        override x.ContextInstallers = 
            [|  Func<IConfiguration, Data.Entity.DbContext> (fun (c : IConfiguration) -> new CodeFirstContextLocal(csRTB) :> Data.Entity.DbContext) |]

          
        override x.RegisterTypes(services) =
          base.RegisterTypes(services)
          services.AddTransient<RestfulObjectsController, RestfulObjectsController>() |> ignore
          services.AddMvc(fun (options) -> options.EnableEndpointRouting <- false)
                  .AddNewtonsoftJson(fun (options) -> options.SerializerSettings.DateTimeZoneHandling <- DateTimeZoneHandling.Utc)
                  |> ignore
          ()
        
        [<OneTimeSetUp>]
        member x.FixtureSetup() =
           CodeFirstSetup()
           AcceptanceTestCase.InitializeNakedObjectsFramework(x)
           ctt <- fun code -> mapper.TypeStringFromCode(code)
           ttc <- fun typ -> mapper.CodeFromTypeString(typ)
           ctk <- fun code -> keyMapper.KeyStringFromCode(code)
           ktc <- fun key -> keyMapper.CodeFromKeyString(key)
        
        [<SetUp>]
        member x.Setup() = 
          x.StartTest()
        
        [<TearDown>]
        member x.TearDown() =        
          resetCache x.api
        
        [<OneTimeTearDown>]
        member x.FixtureTearDown() = AcceptanceTestCase.CleanupNakedObjectsFramework(x)
        
        member x.api =
          let sp = x.GetConfiguredContainer()
          let api = sp.GetService<RestfulObjectsController>()
          setDebugWarnings api false
          setMockContext api sp
        
        [<Test>]
        member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch x.api
        
        [<Test>]
        member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail x.api
        
        [<Test>]
        member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess x.api x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch x.api
    end
    // to avoid warning
    [<EntryPoint>]
    let main argv = 0