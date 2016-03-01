// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.C

open NUnit.Framework
open RestfulObjects.Mvc
open RestfulObjects.Mvc.Media
open System
open RestfulObjects.Snapshot.Utility
open System.Web.Http
open Microsoft.Practices.Unity
open RestfulObjects.Test.Data
open NakedObjects.Facade.Impl.Implementation
open NakedObjects.Facade.Impl.Utility
open MvcTestApp.Controllers
open NakedObjects.Rest.Test.RestTestsHelpers
open NakedObjects.Architecture.Configuration
open NakedObjects.Core.Configuration
open System.Data.Entity.Core.Objects.DataClasses
open System.Collections.Generic
open System.Data.Entity.Core.Objects
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Persistor.Entity
open NakedObjects.Facade
open NakedObjects.Facade.Translation
open NakedObjects.Facade.Impl
open NakedObjects.Architecture.Menu
open NakedObjects.Menu

[<TestFixture>]
type CNof4TestsConcurrency() = 
    class
        inherit NakedObjects.Xat.AcceptanceTestCase()
        
        override x.RegisterTypes(container) = 
            base.RegisterTypes(container)
            let config = new EntityObjectStoreConfiguration()
            let f = (fun () -> new CodeFirstContextLocal("RestTestC") :> Data.Entity.DbContext)
            config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
            container.RegisterInstance(typeof<IEntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager())) |> ignore
            container.RegisterType(typeof<IOidStrategy>, typeof<EntityOidStrategy>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType(typeof<IFrameworkFacade>, typeof<FrameworkFacade>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType(typeof<IOidTranslator>, typeof<OidTranslatorSlashSeparatedTypeAndIds>, null, (new PerResolveLifetimeManager())) |> ignore

            let types = 
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
                   typeof<WithError>
                   typeof<WithGetError>
                   typeof<WithNestedViewModel>
                   typeof<WithReference>
                   typeof<WithReferenceViewModel>
                   typeof<MostSimple>
                   typeof<MostSimpleViewModel>
                   typeof<WithValue>
                   typeof<TestEnum>                   
                   typeof<MostSimple[]>                 
                   typeof<SetWrapper<MostSimple>> |]

            let mm(factory : IMenuFactory) = 
                let menu1 = factory.NewMenu<RestDataRepository>(true);
                let menu2 = factory.NewMenu<WithActionService>(true);
                let menu3 = factory.NewMenu<ContributorService>(true);
           
                [| menu1; menu2;  menu3 |];


            let services = [| typeof<RestDataRepository>;  typeof<WithActionService>; typeof<ContributorService> |]
            let reflectorConfig = new ReflectorConfiguration(types, services,[|"RestfulObjects.Test.Data"|], Func<IMenuFactory, IMenu[]>  mm, true)
            container.RegisterInstance(typeof<IReflectorConfiguration>, null, reflectorConfig, (new ContainerControlledLifetimeManager())) |> ignore
            ()
        
        [<TestFixtureSetUp>]
        member x.FixtureSetup() = 
            CodeFirstSetup()
            NakedObjects.Xat.AcceptanceTestCase.InitializeNakedObjectsFramework(x)
        
        [<SetUp>]
        member x.Setup() = 
            x.StartTest()
            UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
            RestfulObjectsControllerBase.IsReadOnly <- false
            GlobalConfiguration.Configuration.Formatters.[0] <- new JsonNetFormatter(null)
        
        [<TearDown>]
        member x.TearDown() = 
            RestfulObjectsControllerBase.DomainModel <- RestControlFlags.DomainModelType.Selectable
            //RestfulObjectsControllerBase.ConcurrencyChecking <- false
            RestfulObjectsControllerBase.CacheSettings <- (0, 3600, 86400)
        
        [<TestFixtureTearDown>]
        member x.FixtureTearDown() = NakedObjects.Xat.AcceptanceTestCase.CleanupNakedObjectsFramework(x)
        
        override x.Services = 
           [| typeof<RestDataRepository>
              typeof<WithActionService>
              typeof<ContributorService> |]

        override x.MainMenus(factory) = 
            let menu1 = factory.NewMenu<RestDataRepository>(true);
            let menu2 = factory.NewMenu<WithActionService>(true);
            let menu3 = factory.NewMenu<ContributorService>(true);
           
            [| menu1; menu2;  menu3 |];
            
        member x.api = x.GetConfiguredContainer().Resolve<RestfulObjectsController>()

     
     
        
        [<Test>]
        member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess x.api
        

        [<Test>]
        member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch x.api


        
        [<Test>]
        member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess x.api
        
        [<Test>]
        member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail x.api
        
        [<Test>]
        member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch x.api
        

        [<Test>]
        member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        

        [<Test>]
        member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch x.api




//        
//       
//        
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel x.api
//        
//            
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnEmptyCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService x.api
//        
//        [<Test>]
//        member x.PostInvokeActionReturnNullCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = 
//            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = 
//            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithValidateFailObject x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithValidateFailService() = ObjectActionInvoke19.PostQueryActionWithValidateFailService x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithCrossValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithCrossValidateFailService() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithCrossValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel x.api
//        
//        [<Test>]
//        member x.MissingParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = 
//            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeQueryObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeQueryServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeQueryViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionInvokeCollectionViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithSideEffectsViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetActionWithIdempotentViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.PutActionWithQueryOnlyViewModelValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.PostCollectionActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MissingParmsOnPostViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidUrlOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidUrlOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.InvalidUrlOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.DisabledActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.NotFoundActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.HiddenActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.PostQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.api
//        
//        [<Test>]
//        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = 
//            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly x.api
//       
//        [<Test>]
//        member x.GetIsSubTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms x.api
//        
//        [<Test>]
//        member x.GetIsSuperTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms x.api
//        
//        [<Test>]
//        member x.GetIsSubTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms x.api
//        
//        [<Test>]
//        member x.GetIsSuperTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms x.api
//        
//        [<Test>]
//        member x.GetIsSubTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms x.api
//        
//        [<Test>]
//        member x.GetIsSuperTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms x.api
//        
//        [<Test>]
//        member x.GetIsSubTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms x.api
//        
//        [<Test>]
//        member x.GetIsSuperTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms x.api
//        
//        [<Test>]
//        member x.NotFoundTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms x.api
//        
//        [<Test>]
//        member x.NotFoundTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms x.api
//        
//        [<Test>]
//        member x.NotFoundTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms x.api
//        
//        [<Test>]
//        member x.NotFoundTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms x.api
//        
//        [<Test>]
//        member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms x.api
//        
//        [<Test>]
//        member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms x.api
//        
//        [<Test>]
//        member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms x.api
//        
//        [<Test>]
//        member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms x.api
//        
//        [<Test>]
//        member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms x.api
//        
//        [<Test>]
//        member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms x.api
//        
//        [<Test>]
//        member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf x.api
//        
//        [<Test>]
//        member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf x.api
//        
//        [<Test>]
//        member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf x.api
//        
//        [<Test>]
//        member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf x.api

              
    end
