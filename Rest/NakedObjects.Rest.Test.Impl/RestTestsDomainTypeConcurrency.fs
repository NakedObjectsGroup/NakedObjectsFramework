//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.D

//open NUnit.Framework
//open NakedObjects.Rest
//open NakedObjects.Rest.Media
//open System
//open NakedObjects.Rest.Snapshot.Utility
//open System.Web.Http
//open Microsoft.Practices.Unity
//open RestfulObjects.Test.Data
//open NakedObjects.Facade.Impl.Implementation
//open NakedObjects.Facade.Impl.Utility
//open MvcTestApp.Controllers
//open NakedObjects.Rest.Test.RestTestsHelpers
//open NakedObjects.Architecture.Configuration
//open NakedObjects.Core.Configuration
//open System.Data.Entity.Core.Objects.DataClasses
//open System.Collections.Generic
//open System.Data.Entity.Core.Objects
//open NakedObjects.Persistor.Entity.Configuration
//open NakedObjects.Persistor.Entity
//open NakedObjects.Facade
//open NakedObjects.Facade.Translation
//open NakedObjects.Facade.Impl
//open NakedObjects.Facade.Interface
//open NakedObjects.Architecture.Menu
//open NakedObjects.Menu


//[<TestFixture>]
//type DNof4TestsDomainTypeConcurrency() = 
//    class
//        inherit NakedObjects.Xat.AcceptanceTestCase()
        
//        override x.RegisterTypes(container) = 
//            base.RegisterTypes(container)
//            let config = new EntityObjectStoreConfiguration()
//            let f = (fun () -> new CodeFirstContextLocal("RestTestD") :> Data.Entity.DbContext)
//            config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
//            container.RegisterInstance
//                (typeof<IEntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager())) 
//            |> ignore
//            container.RegisterType
//                (typeof<IOidStrategy>, typeof<EntityOidStrategy>, null, (new PerResolveLifetimeManager())) |> ignore
//            container.RegisterType
//                (typeof<IStringHasher>, typeof<NullStringHasher>, null, (new PerResolveLifetimeManager())) |> ignore
//            container.RegisterType
//                (typeof<IFrameworkFacade>, typeof<FrameworkFacade>, null, (new PerResolveLifetimeManager())) |> ignore
//            container.RegisterType
//                (typeof<IOidTranslator>, typeof<OidTranslatorSlashSeparatedTypeAndIds>, null, 
//                 (new PerResolveLifetimeManager())) |> ignore
//            let types = 
//                [| typeof<Immutable>
//                   typeof<WithActionViewModel>
//                   typeof<WithCollectionViewModel>
//                   typeof<WithValueViewModel>
//                   typeof<WithNestedViewModel>
//                   typeof<RedirectedObject>
//                   typeof<WithScalars>
//                   typeof<VerySimple>
//                   typeof<VerySimpleEager>
//                   typeof<WithAction>
//                   typeof<WithActionObject>
//                   typeof<WithAttachments>
//                   typeof<WithCollection>
//                   typeof<WithDateTimeKey>
//                   typeof<WithError>
//                   typeof<WithGetError>
//                   typeof<WithNestedViewModel>
//                   typeof<WithReference>
//                   typeof<WithReferenceViewModel>
//                   typeof<WithValueViewModelEdit>
//                   typeof<WithNestedViewModelEdit>
//                   typeof<WithReferenceViewModelEdit>
//                   typeof<MostSimple>
//                   typeof<MostSimpleViewModel>
//                   typeof<WithValue>
//                   typeof<TestEnum>
//                   typeof<MostSimple []>
//                   typeof<FormViewModel>
//                   typeof<SetWrapper<MostSimple>> |]
            
//            let services = 
//                [| typeof<RestDataRepository>
//                   typeof<WithActionService>
//                   typeof<ContributorService>
//                   typeof<TestTypeCodeMapper>
//                   typeof<TestKeyCodeMapper> |]
            
//            let mm (factory : IMenuFactory) = 
//                let menu1 = factory.NewMenu<RestDataRepository>(true)
//                let menu2 = factory.NewMenu<WithActionService>(true)
//                let menu3 = factory.NewMenu<ContributorService>(true)
//                let menu4 = factory.NewMenu<TestTypeCodeMapper>(true)
//                let menu5 = factory.NewMenu<TestKeyCodeMapper>(true)
//                [| menu1; menu2; menu3; menu4; menu5 |]
            
//            let reflectorConfig = 
//                new ReflectorConfiguration(types, services, [| "RestfulObjects.Test.Data" |], 
//                                           Func<IMenuFactory, IMenu []> mm, true)
//            container.RegisterInstance
//                (typeof<IReflectorConfiguration>, null, reflectorConfig, (new ContainerControlledLifetimeManager())) 
//            |> ignore
//            ()
        
//        [<OneTimeSetUp>]
//        member x.FixtureSetup() = 
//            CodeFirstSetup()
//            NakedObjects.Xat.AcceptanceTestCase.InitializeNakedObjectsFramework(x)
//            RestTestFunctions.ctt <- fun code -> mapper.TypeStringFromCode(code)
//            RestTestFunctions.ttc <- fun typ -> mapper.CodeFromTypeString(typ)
//            RestTestFunctions.ctk <- fun code -> keyMapper.KeyStringFromCode(code)
//            RestTestFunctions.ktc <- fun key -> keyMapper.CodeFromKeyString(key)
        
//        [<SetUp>]
//        member x.Setup() = 
//            x.StartTest()
//            UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
//            RestfulObjectsControllerBase.IsReadOnly <- false
//            GlobalConfiguration.Configuration.Formatters.[0] <- new JsonNetFormatter(null)
        
//        [<TearDown>]
//        member x.TearDown() = 
            
//            RestfulObjectsControllerBase.CacheSettings <- (0, 3600, 86400)
        
//        [<OneTimeTearDown>]
//        member x.FixtureTearDown() = NakedObjects.Xat.AcceptanceTestCase.CleanupNakedObjectsFramework(x)
        
//        override x.Services = 
//            [| typeof<RestDataRepository>
//               typeof<WithActionService>
//               typeof<ContributorService>
//               typeof<TestTypeCodeMapper>
//               typeof<TestKeyCodeMapper> |]
        
//        member x.api = x.GetConfiguredContainer().Resolve<RestfulObjectsController>()
        
//        [<Test>]
//        member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail x.api
        
//        [<Test>]
//        member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch x.api
        
//        [<Test>]
//        member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail x.api
        
//        [<Test>]
//        member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch x.api
        
//        [<Test>]
//        member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = 
//            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = 
//            ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = 
//            ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = 
//            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = 
//            ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = 
//            ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
//        [<Test>]
//        member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = 
//            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail x.api
        
//        [<Test>]
//        member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = 
//            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch x.api
        
//        [<Test>]
//        member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = 
//            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail x.api
        
//        [<Test>]
//        member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = 
//            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch x.api
        
//        [<Test>]
//        member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = 
//            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch x.api
//    end

