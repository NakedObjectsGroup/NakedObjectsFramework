// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.C

open NUnit.Framework
open NakedObjects.Rest
open NakedObjects.Rest.Media
open System
open NakedObjects.Rest.Snapshot.Utility
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
open NakedObjects.Facade.Interface
open NakedObjects.Architecture.Menu
open NakedObjects.Menu

[<TestFixture>]
type CNof4TestsDomainType() = 
    class
        inherit NakedObjects.Xat.AcceptanceTestCase()
        
        override x.RegisterTypes(container) = 
            base.RegisterTypes(container)
            let config = new EntityObjectStoreConfiguration()
            let f = (fun () -> new CodeFirstContextLocal("RestTestB") :> Data.Entity.DbContext)
            config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) |> ignore
            container.RegisterInstance
                (typeof<IEntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager())) 
            |> ignore
            container.RegisterType
                (typeof<IOidStrategy>, typeof<EntityOidStrategy>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType
                (typeof<IStringHasher>, typeof<NullStringHasher>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType
                (typeof<IFrameworkFacade>, typeof<FrameworkFacade>, null, (new PerResolveLifetimeManager())) |> ignore
            container.RegisterType
                (typeof<IOidTranslator>, typeof<OidTranslatorSlashSeparatedTypeAndIds>, null, 
                 (new PerResolveLifetimeManager())) |> ignore
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
                   typeof<WithGuidKey>
                   typeof<WithError>
                   typeof<WithGetError>
                   typeof<WithNestedViewModel>
                   typeof<WithReference>
                   typeof<WithReferenceViewModel>
                   typeof<MostSimple>
                   typeof<MostSimpleViewModel>
                   typeof<WithValue>
                   typeof<TestEnum>
                   typeof<MostSimple []>
                   typeof<FormViewModel>
                   typeof<SetWrapper<MostSimple>> |]
            
            let services = 
                [| typeof<RestDataRepository>
                   typeof<WithActionService>
                   typeof<ContributorService>
                   typeof<TestTypeCodeMapper>
                   typeof<TestKeyCodeMapper> |]
            
            let mm (factory : IMenuFactory) = 
                let menu1 = factory.NewMenu<RestDataRepository>(true)
                let menu2 = factory.NewMenu<WithActionService>(true)
                let menu3 = factory.NewMenu<ContributorService>(true)
                let menu4 = factory.NewMenu<TestTypeCodeMapper>(true)
                let menu5 = factory.NewMenu<TestKeyCodeMapper>(true)
                [| menu1; menu2; menu3; menu4; menu5 |]
            
            let reflectorConfig = 
                new ReflectorConfiguration(types, services, [| "RestfulObjects.Test.Data" |], 
                                           Func<IMenuFactory, IMenu []> mm, false)
            container.RegisterInstance
                (typeof<IReflectorConfiguration>, null, reflectorConfig, (new ContainerControlledLifetimeManager())) 
            |> ignore
            ()
        
        [<TestFixtureSetUp>]
        member x.FixtureSetup() = 
            CodeFirstSetup()
            NakedObjects.Xat.AcceptanceTestCase.InitializeNakedObjectsFramework(x)
            RestTestFunctions.ctt <- fun code -> mapper.TypeStringFromCode(code)
            RestTestFunctions.ttc <- fun typ -> mapper.CodeFromTypeString(typ)
            RestTestFunctions.ctk <- fun code -> keyMapper.KeyStringFromCode(code)
            RestTestFunctions.ktc <- fun key -> keyMapper.CodeFromKeyString(key)
        
        [<SetUp>]
        member x.Setup() = 
            x.StartTest()
            UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
            RestfulObjectsControllerBase.IsReadOnly <- false
            GlobalConfiguration.Configuration.Formatters.[0] <- new JsonNetFormatter(null)
        
        [<TearDown>]
        member x.TearDown() = 
           
            
            RestfulObjectsControllerBase.CacheSettings <- (0, 3600, 86400)
        
        [<TestFixtureTearDown>]
        member x.FixtureTearDown() = NakedObjects.Xat.AcceptanceTestCase.CleanupNakedObjectsFramework(x)
        
        override x.Services = 
            [| typeof<RestDataRepository>
               typeof<WithActionService>
               typeof<ContributorService>
               typeof<TestTypeCodeMapper>
               typeof<TestKeyCodeMapper> |]
        
        member x.api = x.GetConfiguredContainer().Resolve<RestfulObjectsController>()
        
        [<Test>]
        member x.GetHomePage() = HomePage5.GetHomePage x.api
        
        [<Test>]
        member x.GetHomePageWithMediaType() = HomePage5.GetHomePageWithMediaType x.api
        
        [<Test>]
        member x.NotAcceptableGetHomePage() = HomePage5.NotAcceptableGetHomePage x.api
        
        [<Test>]
        member x.InvalidDomainModelGetHomePage() = HomePage5.InvalidDomainModelGetHomePage x.api
        
        [<Test>]
        member x.GetUser() = User6.GetUser x.api
        
        [<Test>]
        member x.GetUserWithMediaType() = User6.GetUserWithMediaType x.api
        
        [<Test>]
        member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser x.api
        
        [<Test>]
        member x.GetDomainServices() = DomainServices7.GetDomainServicesWithTTC x.api
        
        [<Test>]
        member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaTypeWithTTC x.api
        
        [<Test>]
        member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices x.api
        
        [<Test>]
        member x.GetMenus() = Menus7.GetMenusWithTTC x.api
        
        [<Test>]
        member x.GetMenusWithMediaType() = Menus7.GetMenusWithMediaTypeWithTTC x.api
        
        [<Test>]
        member x.NotAcceptableGetMenus() = Menus7.NotAcceptableGetMenus x.api
        
        [<Test>]
        member x.GetVersion() = Version8.GetVersion x.api
        
        [<Test>]
        member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType x.api
        
        [<Test>]
        member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion x.api
        
        [<Test>]
        member x.GetMostSimpleTransientObject() = Objects9.GetMostSimpleTransientObject x.api
        
        [<Test>]
        member x.GetMostSimpleTransientObjectSimpleOnly() = Objects9.GetMostSimpleTransientObjectSimpleOnly x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObject() = Objects9.PersistMostSimpleTransientObject x.api
        
        [<Test>]
        [<Ignore>]
         // temp ignore untill fix persist id
         member x.PersistMostSimpleTransientObjectSimpleOnly() = 
            Objects9.PersistMostSimpleTransientObjectSimpleOnly x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectValidateOnly() = 
            Objects9.PersistMostSimpleTransientObjectValidateOnly x.api
        
        [<Test>]
        [<Ignore>]
        member x.GetWithValueTransientObject() = Objects9.GetWithValueTransientObject x.api
        
        [<Test>]
        member x.GetWithReferenceTransientObject() = Objects9.GetWithReferenceTransientObject x.api
        
        [<Test>]
        member x.GetWithCollectionTransientObject() = Objects9.GetWithCollectionTransientObject x.api
        
        // remove these just because the ids get changed. The tests have already been run once if uncomment and run 
        // on their own should pass - only fail because previous run changes ids.
        // [<Test>]
        // member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject x.api
        // 
        // [<Test>]
        // member x.PersistWithValueTransientObjectFormalOnly() = Objects9.PersistWithValueTransientObjectFormalOnly x.api
        [<Test>]        
        member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject x.api
        
        [<Test>]
        member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject x.api
        
        [<Test>]        
        member x.PersistWithValueTransientObjectValidateOnly() = 
            Objects9.PersistWithValueTransientObjectValidateOnly x.api
        
        [<Test>]
        member x.PersistWithReferenceTransientObjectValidateOnly() = 
            Objects9.PersistWithReferenceTransientObjectValidateOnly x.api
        
        [<Test>]
        member x.PersistWithCollectionTransientObjectValidateOnly() = 
            Objects9.PersistWithCollectionTransientObjectValidateOnly x.api
        
        [<Test>]
        member x.PersistWithValueTransientObjectValidateOnlyFail() = 
            Objects9.PersistWithValueTransientObjectValidateOnlyFail x.api
        
        [<Test>]
        member x.PersistWithReferenceTransientObjectValidateOnlyFail() = 
            Objects9.PersistWithReferenceTransientObjectValidateOnlyFail x.api
        
        [<Test>]
        member x.PersistWithCollectionTransientObjectValidateOnlyFail() = 
            Objects9.PersistWithCollectionTransientObjectValidateOnlyFail x.api
        
        [<Test>]
        member x.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail() = 
            Objects9.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail x.api
        
        [<Test>]
        member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail x.api
        
        [<Test>]
        member x.PersistWithValueTransientObjectFailInvalid() = 
            Objects9.PersistWithValueTransientObjectFailInvalid x.api
        
        [<Test>]
        member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail x.api
        
        [<Test>]
        member x.PersistWithReferenceTransientObjectFailInvalid() = 
            Objects9.PersistWithReferenceTransientObjectFailInvalid x.api
        
        [<Test>]
        member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectMissingArgs() = 
            Objects9.PersistMostSimpleTransientObjectMissingArgs x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectMissingArgsValidateOnly() = 
            Objects9.PersistMostSimpleTransientObjectMissingArgsValidateOnly x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectMissingMemberArgs() = 
            Objects9.PersistMostSimpleTransientObjectMissingMemberArgs x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectNullDomainType() = 
            Objects9.PersistMostSimpleTransientObjectNullDomainType x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectEmptyDomainType() = 
            Objects9.PersistMostSimpleTransientObjectEmptyDomainType x.api
        
        [<Test>]
        member x.PersistMostSimpleTransientObjectMalformedMemberArgs() = 
            Objects9.PersistMostSimpleTransientObjectMalformedMemberArgs x.api
        
        [<Test>]
        member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject x.api
        
        [<Test>]
        member x.PersistNoKeyTransientObject() = Objects9.PersistNoKeyTransientObject x.api
        
        [<Test>]
        member x.PersistWithValueTransientObjectFailCrossValidation() = 
            Objects9.PersistWithValueTransientObjectFailCrossValidation x.api
        
        [<Test>]
        member x.Error() = Error10.Error x.api
        
        [<Test>]
        member x.NotAcceptableError() = Error10.NotAcceptableError x.api
        
        [<Test>]
        member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject x.api
        
        [<Test>]
        member x.GetMostSimpleObjectWithDetailsFlag() = DomainObject14.GetMostSimpleObjectWithDetailsFlag x.api

        [<Test>]
        member x.GetWithAttachmentsObject() = DomainObject14.GetWithAttachmentsObject x.api
        
        [<Test>]
        member x.GetMostSimpleObjectConfiguredSelectable() = 
            DomainObject14.GetMostSimpleObjectConfiguredSelectable x.api
        
        [<Test>]
        member x.GetMostSimpleObjectConfiguredSimpleOnly() = 
            DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly x.api
        
        [<Test>]
        member x.GetMostSimpleObjectConfiguredCaching() = DomainObject14.GetMostSimpleObjectConfiguredCaching x.api
        
        [<Test>]
        member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly x.api
        
        [<Test>]
        member x.GetWithDateTimeKeyObject() = DomainObject14.GetWithDateTimeKeyObject x.api
        
        [<Test>]
        member x.GetWithGuidKeyObject() = DomainObject14.GetWithGuidKeyObject x.api

        [<Test>]
        member x.GetVerySimpleEagerObject() = DomainObject14.GetVerySimpleEagerObject x.api
        
        [<Test>]
        member x.GetWithValueObject() = DomainObject14.GetWithValueObject x.api
        
        [<Test>]
        member x.GetWithScalarsObject() = DomainObject14.GetWithScalarsObject x.api
        
        [<Test>]
        member x.GetWithValueObjectUserAuth() = 
            x.SetUser("viewUser")
            DomainObject14.GetWithValueObjectUserAuth x.api
            x.SetUser("Test")
        
        [<Test>]
        member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType x.api
        
        [<Test>]
        member x.GetMostSimpleObjectWithDomainTypeSimple() = 
            DomainObject14.GetMostSimpleObjectWithDomainTypeSimple x.api
        
        [<Test>]
        member x.GetWithValueObjectWithDomainTypeNoProfileSimple() = 
            DomainObject14.GetWithValueObjectWithDomainTypeNoProfileSimple x.api
        
        [<Test>]
        member x.GetRedirectedObject() = DomainObject14.GetRedirectedObject x.api
        
        [<Test>]
        member x.PutWithValueObject() = DomainObject14.PutWithValueObject x.api
        
        [<Test>]
        member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly x.api
        
        [<Test>]
        member x.PutWithScalarsObject() = DomainObject14.PutWithScalarsObject x.api
        
        [<Test>]
        member x.PutWithReferenceObject() = DomainObject14.PutWithReferenceObject x.api
        
        [<Test>]
        member x.PutWithReferenceObjectValidateOnly() = DomainObject14.PutWithReferenceObjectValidateOnly x.api
        
        [<Test>]
        member x.GetWithActionObject() = DomainObject14.GetWithActionObject x.api
        
        [<Test>]
        member x.GetWithActionObjectSimpleOnly() = DomainObject14.GetWithActionObjectSimpleOnly x.api
        
        [<Test>]
        member x.GetWithReferenceObject() = DomainObject14.GetWithReferenceObject x.api
        
        [<Test>]
        member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject x.api
        
        [<Test>]
        member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly x.api
        
        [<Test>]
        member x.InvalidGetObject() = DomainObject14.InvalidGetObject x.api
        
        [<Test>]
        member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject x.api
        
        [<Test>]
        member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType x.api
        
        [<Test>]
        member x.GetObjectIgnoreWrongDomainType() = DomainObject14.GetObjectIgnoreWrongDomainType x.api
        
        [<Test>]
        member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs x.api
        
        [<Test>]
        member x.PutWithValueObjectMissingArgsValidateOnly() = 
            DomainObject14.PutWithValueObjectMissingArgsValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs x.api
        
        [<Test>]
        member x.PutWithValueObjectMalformedDateTimeArgs() = 
            DomainObject14.PutWithValueObjectMalformedDateTimeArgs x.api

        [<Test>]
        member x.PutWithValueObjectMalformedTimeArgs() = 
            DomainObject14.PutWithValueObjectMalformedTimeArgs x.api
        
        [<Test>]
        member x.PutWithValueObjectMalformedArgsValidateOnly() = 
            DomainObject14.PutWithValueObjectMalformedArgsValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue x.api
        
        [<Test>]
        member x.PutWithValueObjectInvalidArgsValueValidateOnly() = 
            DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue x.api
        
        [<Test>]
        member x.PutWithReferenceObjectNotFoundArgsValue() = 
            DomainObject14.PutWithReferenceObjectNotFoundArgsValue x.api
        
        [<Test>]
        member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = 
            DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceObjectMalformedArgs() = DomainObject14.PutWithReferenceObjectMalformedArgs x.api
        
        [<Test>]
        member x.PutWithReferenceObjectMalformedArgsValidateOnly() = 
            DomainObject14.PutWithReferenceObjectMalformedArgsValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueObjectDisabledValue() = DomainObject14.PutWithValueObjectDisabledValue x.api
        
        [<Test>]
        member x.PutWithValueObjectDisabledValueValidateOnly() = 
            DomainObject14.PutWithValueObjectDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceObjectDisabledValue() = DomainObject14.PutWithReferenceObjectDisabledValue x.api
        
        [<Test>]
        member x.PutWithReferenceObjectDisabledValueValidateOnly() = 
            DomainObject14.PutWithReferenceObjectDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueObjectInvisibleValue() = DomainObject14.PutWithValueObjectInvisibleValue x.api
        
        [<Test>]
        member x.PutWithReferenceObjectInvisibleValue() = DomainObject14.PutWithReferenceObjectInvisibleValue x.api
        
        [<Test>]
        member x.PutWithValueObjectInvisibleValueValidateOnly() = 
            DomainObject14.PutWithValueObjectInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceObjectInvisibleValueValidateOnly() = 
            DomainObject14.PutWithReferenceObjectInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueImmutableObject() = DomainObject14.PutWithValueImmutableObject x.api
        
        [<Test>]
        member x.PutWithReferenceImmutableObject() = DomainObject14.PutWithReferenceImmutableObject x.api
        
        [<Test>]
        member x.PutWithValueImmutableObjectValidateOnly() = 
            DomainObject14.PutWithValueImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceImmutableObjectValidateOnly() = 
            DomainObject14.PutWithReferenceImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.PutWithValueObjectInvalidArgsName() = DomainObject14.PutWithValueObjectInvalidArgsName x.api
        
        [<Test>]
        member x.PutWithValueObjectInvalidArgsNameValidateOnly() = 
            DomainObject14.PutWithValueObjectInvalidArgsNameValidateOnly x.api
        
        [<Test>]
        member x.NotAcceptablePutObjectWrongMediaType() = DomainObject14.NotAcceptablePutObjectWrongMediaType x.api
        
        [<Test>]
        member x.PutWithValueInternalError() = DomainObject14.PutWithValueInternalError x.api
        
        [<Test>]
        member x.PutWithReferenceInternalError() = DomainObject14.PutWithReferenceInternalError x.api
        
        [<Test>]
        member x.PutWithValueObjectFailCrossValidation() = DomainObject14.PutWithValueObjectFailCrossValidation x.api
        
        [<Test>]
        member x.PutWithValueObjectFailCrossValidationValidateOnly() = 
            DomainObject14.PutWithValueObjectFailCrossValidationValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferenceObjectFailsCrossValidation() = 
            DomainObject14.PutWithReferenceObjectFailsCrossValidation x.api
        
        [<Test>]
        member x.PutWithReferenceObjectFailsCrossValidationValidateOnly() = 
            DomainObject14.PutWithReferenceObjectFailsCrossValidationValidateOnly x.api
        
        [<Test>]
        member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey x.api
        
        [<Test>]
        member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType x.api
        
        [<Test>]
        member x.ObjectNotFoundAbstractType() = DomainObject14.ObjectNotFoundAbstractType x.api
        
        [<Test>]
        member x.GetMostSimpleViewModel() = DomainObject14.GetMostSimpleViewModel x.api
        
        [<Test>]
        member x.GetFormViewModel() = DomainObject14.GetFormViewModel x.api

        [<Test>]
        member x.GetWithValueViewModel() = DomainObject14.GetWithValueViewModel x.api
        
        [<Test>]
        member x.GetWithReferenceViewModel() = DomainObject14.GetWithReferenceViewModel x.api
        
        [<Test>]
        member x.GetWithNestedViewModel() = DomainObject14.GetWithNestedViewModel x.api
        
        [<Test>]
        [<Ignore>]
        member x.PutWithReferenceViewModel() = DomainObject14.PutWithReferenceViewModel x.api
        
        [<Test>]
        [<Ignore>]
        member x.PutWithNestedViewModel() = DomainObject14.PutWithNestedViewModel x.api
        
        [<Test>]
        [<Ignore>]
        member x.PutWithValueViewModel() = DomainObject14.PutWithValueViewModel x.api
        
        [<Test>]
        member x.GetService() = DomainService15.GetService x.api
        
        [<Test>]
        member x.GetContributorService() = DomainService15.GetContributorService x.api
        
        [<Test>]
        member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly x.api
        
        [<Test>]
        member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType x.api
        
        [<Test>]
        member x.GetWithActionService() = DomainService15.GetWithActionService x.api
        
        [<Test>]
        member x.InvalidGetService() = DomainService15.InvalidGetService x.api
        
        [<Test>]
        member x.NotFoundGetService() = DomainService15.NotFoundGetService x.api
        
        [<Test>]
        member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType x.api
        
        //
        [<Test>]
        member x.GetMenu() = DomainMenu15.GetMenu x.api
        
        [<Test>]
        member x.GetContributorMenu() = DomainMenu15.GetContributorMenu x.api
        
        [<Test>]
        member x.GetMenuSimpleOnly() = DomainMenu15.GetMenuSimpleOnly x.api
        
        [<Test>]
        member x.GetMenuWithMediaType() = DomainMenu15.GetMenuWithMediaType x.api
        
        [<Test>]
        //[<Ignore>]
        member x.GetWithActionMenu() = DomainMenu15.GetWithActionMenu x.api
        
        [<Test>]
        member x.InvalidGetMenu() = DomainMenu15.InvalidGetMenu x.api
        
        [<Test>]
        member x.NotFoundGetMenu() = DomainMenu15.NotFoundGetMenu x.api
        
        [<Test>]
        member x.NotAcceptableGetMenuWrongMediaType() = DomainMenu15.NotAcceptableGetMenuWrongMediaType x.api
        
        //f
        [<Test>]
        member x.GetValueProperty() = ObjectProperty16.GetValueProperty x.api

      
        
        [<Test>]
        member x.GetFileAttachmentProperty() = ObjectProperty16.GetFileAttachmentProperty x.api
        
        [<Test>]
        member x.GetImageAttachmentProperty() = ObjectProperty16.GetImageAttachmentProperty x.api
        
        [<Test>]
        member x.GetFileAttachmentValue() = ObjectProperty16.GetFileAttachmentValue x.api
        
        [<Test>]
        member x.GetAttachmentValueWrongMediaType() = ObjectProperty16.GetAttachmentValueWrongMediaType x.api
        
        [<Test>]
        member x.GetImageAttachmentValue() = ObjectProperty16.GetImageAttachmentValue x.api
        
        [<Test>]
        member x.GetImageAttachmentValueWithDefault() = ObjectProperty16.GetImageAttachmentValueWithDefault x.api
        
        [<Test>]
        member x.GetValuePropertyViewModel() = ObjectProperty16.GetValuePropertyViewModel x.api
        
        [<Test>]
        member x.GetEnumValueProperty() = ObjectProperty16.GetEnumValueProperty x.api
        
        [<Test>]
        member x.GetValuePropertyUserAuth() = 
            x.SetUser("viewUser")
            ObjectProperty16.GetValuePropertyUserAuth x.api
            x.SetUser("Test")
        
        [<Test>]
        member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly x.api
        
        [<Test>]
        member x.GetStringValueProperty() = ObjectProperty16.GetStringValueProperty x.api
        
        [<Test>]
        member x.GetBlobValueProperty() = ObjectProperty16.GetBlobValueProperty x.api
        
        [<Test>]
        member x.GetClobValueProperty() = ObjectProperty16.GetClobValueProperty x.api
        
        [<Test>]
        member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType x.api
        
        [<Test>]
        member x.GetChoicesValueProperty() = ObjectProperty16.GetChoicesValueProperty x.api
        
        [<Test>]
        member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty x.api
        
        [<Test>]
        member x.GetUserDisabledValueProperty() = ObjectProperty16.GetUserDisabledValueProperty x.api
        
        [<Test>]
        member x.GetUserDisabledValuePropertyAuthorised() = 
            x.SetUser("editUser")
            ObjectProperty16.GetUserDisabledValuePropertyAuthorised x.api
            x.SetUser("Test")
        
        [<Test>]
        member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty x.api
        
        [<Test>]
        member x.GetAutoCompleteProperty() = ObjectProperty16.GetAutoCompleteProperty x.api
        
        [<Test>]
        member x.InvokeAutoComplete() = ObjectProperty16.InvokeAutoComplete x.api
        
        [<Test>]
        member x.InvokeAutoCompleteErrorNoParm() = ObjectProperty16.InvokeAutoCompleteErrorNoParm x.api
        
        [<Test>]
        member x.InvokeAutoCompleteErrorMalformedParm() = ObjectProperty16.InvokeAutoCompleteErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeAutoCompleteErrorUnrecognisedParm() = 
            ObjectProperty16.InvokeAutoCompleteErrorUnrecognisedParm x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesReference() = ObjectProperty16.InvokeConditionalChoicesReference x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesReferenceErrorMalformedParm() = 
            ObjectProperty16.InvokeConditionalChoicesReferenceErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesReferenceErrorNoParm() = 
            ObjectProperty16.InvokeConditionalChoicesReferenceErrorNoParm x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesReferenceErrorUnrecognisedParm() = 
            ObjectProperty16.InvokeConditionalChoicesReferenceErrorUnrecognisedParm x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesValue() = ObjectProperty16.InvokeConditionalChoicesValue x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesValueErrorMalformedParm() = 
            ObjectProperty16.InvokeConditionalChoicesValueErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeConditionalChoicesValueErrorMissingParm() = 
            ObjectProperty16.InvokeConditionalChoicesValueErrorMissingParm x.api
        
        [<Test>]
        member x.GetReferencePropertyViewModel() = ObjectProperty16.GetReferencePropertyViewModel x.api
        
        [<Test>]
        member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty x.api
        
        [<Test>]
        member x.GetChoicesReferenceProperty() = ObjectProperty16.GetChoicesReferenceProperty x.api
        
        [<Test>]
        member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty x.api
        
        [<Test>]
        member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty x.api
        
        [<Test>]
        member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty x.api
        
        [<Test>]
        member x.GetUserHiddenValueProperty() = ObjectProperty16.GetUserHiddenValueProperty x.api
        
        [<Test>]
        member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty x.api
        
        [<Test>]
        member x.NotAcceptableGetPropertyWrongMediaType() = 
            ObjectProperty16.NotAcceptableGetPropertyWrongMediaType x.api
        
        [<Test>]
        member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty x.api
        
        [<Test>]
        member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty x.api
        
        [<Test>]
        member x.GetPropertyAsCollection() = ObjectProperty16.GetPropertyAsCollection x.api
        
        [<Test>]
        member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess x.api
        
        [<Test>]
        member x.PutDateTimeValuePropertySuccess() = 
            ObjectProperty16.PutDateTimeValuePropertySuccess x.api
            x.NakedObjectsFramework.TransactionManager.StartTransaction()
            let o = x.NakedObjectsFramework.Persistor.Instances<WithValue>() |> Seq.head
            o.ADateTimeValue <- new DateTime(2012, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc)
            x.NakedObjectsFramework.TransactionManager.EndTransaction()
        
        [<Test>]
        member x.PutUserDisabledValuePropertySuccess() = 
            x.SetUser("editUser")
            ObjectProperty16.PutUserDisabledValuePropertySuccess x.api
            x.NakedObjectsFramework.TransactionManager.StartTransaction()
            let o = x.NakedObjectsFramework.Persistor.Instances<WithValue>() |> Seq.head
            o.AUserDisabledValue <- 0
            x.NakedObjectsFramework.TransactionManager.EndTransaction()
            x.SetUser("Test")
        
        [<Test>]
        member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly x.api
        
        [<Test>]
        member x.PutClobPropertyBadRequest() = ObjectProperty16.PutClobPropertyBadRequest x.api
        
        [<Test>]
        member x.PutBlobPropertyBadRequest() = ObjectProperty16.PutBlobPropertyBadRequest x.api
        
        [<Test>]
        member x.DeleteValuePropertySuccess() = 
            ObjectProperty16.DeleteValuePropertySuccess x.api
            x.NakedObjectsFramework.TransactionManager.StartTransaction()
            let o = x.NakedObjectsFramework.Persistor.Instances<WithValue>() |> Seq.head
            o.AValue <- 100
            x.NakedObjectsFramework.TransactionManager.EndTransaction()
        
        [<Test>]
        member x.DeleteValuePropertySuccessValidateOnly() = 
            ObjectProperty16.DeleteValuePropertySuccessValidateOnly x.api
            x.NakedObjectsFramework.TransactionManager.StartTransaction()
            let o = x.NakedObjectsFramework.Persistor.Instances<WithValue>() |> Seq.head
            o.AValue <- 100
            x.NakedObjectsFramework.TransactionManager.EndTransaction()
        
        [<Test>]
        member x.PutNullValuePropertySuccess() = ObjectProperty16.PutNullValuePropertySuccess x.api
        
        [<Test>]
        member x.PutNullValuePropertySuccessValidateOnly() = 
            ObjectProperty16.PutNullValuePropertySuccessValidateOnly x.api
        
        [<Test>]
        member x.PutReferencePropertySuccess() = ObjectProperty16.PutReferencePropertySuccess x.api
        
        [<Test>]
        member x.PutReferencePropertySuccessValidateOnly() = 
            ObjectProperty16.PutReferencePropertySuccessValidateOnly x.api
        
        [<Test>]
        member x.DeleteReferencePropertySuccess() = ObjectProperty16.DeleteReferencePropertySuccess x.api
        
        [<Test>]
        member x.DeleteReferencePropertySuccessValidateOnly() = 
            ObjectProperty16.DeleteReferencePropertySuccessValidateOnly x.api
        
        [<Test>]
        member x.PutNullReferencePropertySuccess() = ObjectProperty16.PutNullReferencePropertySuccess x.api
        
        [<Test>]
        member x.PutNullReferencePropertySuccessValidateOnly() = 
            ObjectProperty16.PutNullReferencePropertySuccessValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyMissingArgs() = ObjectProperty16.PutWithValuePropertyMissingArgs x.api
        
        [<Test>]
        member x.PutWithValuePropertyMalformedArgs() = ObjectProperty16.PutWithValuePropertyMalformedArgs x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue x.api
        
        [<Test>]
        member x.PutWithReferencePropertyFailCrossValidation() = 
            ObjectProperty16.PutWithReferencePropertyFailCrossValidation x.api
        
        [<Test>]
        member x.PutWithReferencePropertyMalformedArgs() = ObjectProperty16.PutWithReferencePropertyMalformedArgs x.api
        
        [<Test>]
        member x.PutWithValuePropertyFailCrossValidation() = 
            ObjectProperty16.PutWithValuePropertyFailCrossValidation x.api
        
        [<Test>]
        member x.PutWithReferencePropertyInvalidArgsValue() = 
            ObjectProperty16.PutWithReferencePropertyInvalidArgsValue x.api
        
        [<Test>]
        member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue x.api
        
        [<Test>]
        member x.PutWithValuePropertyUserDisabledValue() = ObjectProperty16.PutWithValuePropertyUserDisabledValue x.api
        
        [<Test>]
        member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue x.api
        
        [<Test>]
        member x.PutWithReferencePropertyInvisibleValue() = 
            ObjectProperty16.PutWithReferencePropertyInvisibleValue x.api
        
        [<Test>]
        member x.PutWithValuePropertyOnImmutableObject() = ObjectProperty16.PutWithValuePropertyOnImmutableObject x.api
        
        [<Test>]
        member x.PutWithReferencePropertyOnImmutableObject() = 
            ObjectProperty16.PutWithReferencePropertyOnImmutableObject x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName x.api
        
        [<Test>]
        member x.NotAcceptablePutPropertyWrongMediaType() = 
            ObjectProperty16.NotAcceptablePutPropertyWrongMediaType x.api
        
        [<Test>]
        member x.PutWithValuePropertyMissingArgsValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyMalformedArgsValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferencePropertyFailCrossValidationValidateOnly() = 
            ObjectProperty16.PutWithReferencePropertyFailCrossValidationValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyFailCrossValidationValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyFailCrossValidationValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = 
            ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyDisabledValueValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferencePropertyDisabledValueValidateOnly() = 
            ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvisibleValueValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferencePropertyInvisibleValueValidateOnly() = 
            ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyOnImmutableObjectValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyOnImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.PutWithReferencePropertyOnImmutableObjectValidateOnly() = 
            ObjectProperty16.PutWithReferencePropertyOnImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = 
            ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly x.api
        
        [<Test>]
        member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError x.api
        
        [<Test>]
        member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError x.api
        
        [<Test>]
        member x.DeleteValuePropertyDisabledValueValidateOnly() = 
            ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.DeleteReferencePropertyDisabledValueValidateOnly() = 
            ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly x.api
        
        [<Test>]
        member x.DeleteValuePropertyInvisibleValueValidateOnly() = 
            ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.DeleteReferencePropertyInvisibleValueValidateOnly() = 
            ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly x.api
        
        [<Test>]
        member x.DeleteValuePropertyOnImmutableObjectValidateOnly() = 
            ObjectProperty16.DeleteValuePropertyOnImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.DeleteReferencePropertyOnImmutableObjectValidateOnly() = 
            ObjectProperty16.DeleteReferencePropertyOnImmutableObjectValidateOnly x.api
        
        [<Test>]
        member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = 
            ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly x.api
        
        [<Test>]
        member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue x.api
        
        [<Test>]
        member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue x.api
        
        [<Test>]
        member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue x.api
        
        [<Test>]
        member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue x.api
        
        [<Test>]
        member x.DeleteValuePropertyOnImmutableObject() = ObjectProperty16.DeleteValuePropertyOnImmutableObject x.api
        
        [<Test>]
        member x.DeleteReferencePropertyOnImmutableObject() = 
            ObjectProperty16.DeleteReferencePropertyOnImmutableObject x.api
        
        [<Test>]
        member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName x.api
        
        [<Test>]
        member x.NotAcceptableDeletePropertyWrongMediaType() = 
            ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType x.api
        
        [<Test>]
        member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError x.api
        
        [<Test>]
        member x.DeleteReferencePropertyInternalError() = ObjectProperty16.DeleteReferencePropertyInternalError x.api
        
        [<Test>]
        member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound x.api
        
        [<Test>]
        member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty x.api

        [<Test>]
        member x.GetCollectionPropertyWithInlineFlag() = ObjectCollection17.GetCollectionPropertyWithInlineFlag x.api
        
        [<Test>]
        member x.GetCollectionPropertyViewModel() = ObjectCollection17.GetCollectionPropertyViewModel x.api
        
        [<Test>]
        member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly x.api
        
        [<Test>]
        member x.GetCollectionSetProperty() = ObjectCollection17.GetCollectionSetProperty x.api
        
        [<Test>]
        member x.GetCollectionSetPropertySimpleOnly() = ObjectCollection17.GetCollectionSetPropertySimpleOnly x.api
        
        [<Test>]
        member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType x.api
        
        [<Test>]
        member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty x.api
        
        [<Test>]
        member x.GetCollectionValue() = ObjectCollection17.GetCollectionValue x.api
        
        [<Test>]
        member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection x.api
        
        [<Test>]
        member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection x.api
        
        [<Test>]
        member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection x.api
        
        [<Test>]
        member x.GetNakedObjectsIgnoredCollection() = ObjectCollection17.GetNakedObjectsIgnoredCollection x.api
        
        [<Test>]
        member x.NotAcceptableGetCollectionWrongMediaType() = 
            ObjectCollection17.NotAcceptableGetCollectionWrongMediaType x.api
        
        [<Test>]
        member x.GetErrorValueCollection() = ObjectCollection17.GetErrorValueCollection x.api
        
        [<Test>]
        member x.GetCollectionAsProperty() = ObjectCollection17.GetCollectionAsProperty x.api
        
        [<Test>]
        member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject x.api
        
        [<Test>]
        member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService x.api
        
        [<Test>]
        member x.GetActionContributedService() = ObjectAction18.GetActionContributedService x.api
        
        [<Test>]
        member x.GetActionPropertyViewModel() = ObjectAction18.GetActionPropertyViewModel x.api
        
        [<Test>]
        member x.GetOverloadedActionPropertyObject() = ObjectAction18.GetOverloadedActionPropertyObject x.api
        
        [<Test>]
        member x.GetOverloadedActionPropertyService() = ObjectAction18.GetOverloadedActionPropertyService x.api
        
        [<Test>]
        member x.GetOverloadedActionPropertyViewModel() = ObjectAction18.GetOverloadedActionPropertyViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyDateTimeViewModel() = ObjectAction18.GetActionPropertyDateTimeViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyCollectionViewModel() = ObjectAction18.GetActionPropertyCollectionViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyCollectionService() = ObjectAction18.GetActionPropertyCollectionService x.api
        
        [<Test>]
        member x.GetActionPropertyCollectionObject() = ObjectAction18.GetActionPropertyCollectionObject x.api
        
        [<Test>]
        member x.GetActionPropertyDateTimeService() = ObjectAction18.GetActionPropertyDateTimeService x.api
        
        [<Test>]
        member x.GetActionPropertyDateTimeObject() = ObjectAction18.GetActionPropertyDateTimeObject x.api
        
        [<Test>]
        member x.GetUserDisabledActionPropertyObject() = ObjectAction18.GetUserDisabledActionPropertyObject x.api
        
        [<Test>]
        member x.GetUserDisabledActionPropertyService() = ObjectAction18.GetUserDisabledActionPropertyService x.api
        
        [<Test>]
        member x.GetUserDisabledActionPropertyViewModel() = ObjectAction18.GetUserDisabledActionPropertyViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyQueryOnlyObject() = ObjectAction18.GetActionPropertyQueryOnlyObject x.api
        
        [<Test>]
        member x.GetActionPropertyQueryOnlyService() = ObjectAction18.GetActionPropertyQueryOnlyService x.api
        
        [<Test>]
        member x.GetActionPropertyQueryOnlyViewModel() = ObjectAction18.GetActionPropertyQueryOnlyViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyIdempotentObject() = ObjectAction18.GetActionPropertyIdempotentObject x.api
        
        [<Test>]
        member x.GetActionPropertyIdempotentService() = ObjectAction18.GetActionPropertyIdempotentService x.api
        
        [<Test>]
        member x.GetActionPropertyIdempotentViewModel() = ObjectAction18.GetActionPropertyIdempotentViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptObject() = ObjectAction18.GetActionPropertyWithOptObject x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptService() = ObjectAction18.GetActionPropertyWithOptService x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptViewModel() = ObjectAction18.GetActionPropertyWithOptViewModel x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptObjectSimpleOnly() = 
            ObjectAction18.GetActionPropertyWithOptObjectSimpleOnly x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptServiceSimpleOnly() = 
            ObjectAction18.GetActionPropertyWithOptServiceSimpleOnly x.api
        
        [<Test>]
        member x.GetActionPropertyWithOptViewModelSimpleOnly() = 
            ObjectAction18.GetActionPropertyWithOptViewModelSimpleOnly x.api
        
        [<Test>]
        member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType x.api
        
        [<Test>]
        member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType x.api
        
        [<Test>]
        member x.GetActionPropertyViewModelWithMediaType() = 
            ObjectAction18.GetActionPropertyViewModelWithMediaType x.api
        
        [<Test>]
        member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject x.api
        
        [<Test>]
        member x.GetScalarActionService() = ObjectAction18.GetScalarActionService x.api
        
        [<Test>]
        member x.GetScalarActionViewModel() = ObjectAction18.GetScalarActionViewModel x.api
        
        [<Test>]
        member x.GetActionWithValueParmObject() = ObjectAction18.GetActionWithValueParmObject x.api
        
        [<Test>]
        member x.GetActionWithValueParmService() = ObjectAction18.GetActionWithValueParmService x.api
        
        [<Test>]
        member x.GetActionWithValueParmViewModel() = ObjectAction18.GetActionWithValueParmViewModel x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithChoicesObject() = 
            ObjectAction18.GetActionWithValueParmWithChoicesObject x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithChoicesService() = 
            ObjectAction18.GetActionWithValueParmWithChoicesService x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithChoicesViewModel() = 
            ObjectAction18.GetActionWithValueParmWithChoicesViewModel x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithDefaultObject() = 
            ObjectAction18.GetActionWithValueParmWithDefaultObject x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithDefaultService() = 
            ObjectAction18.GetActionWithValueParmWithDefaultService x.api
        
        [<Test>]
        member x.GetActionWithValueParmWithDefaultViewModel() = 
            ObjectAction18.GetActionWithValueParmWithDefaultViewModel x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmViewModel() = ObjectAction18.GetActionWithReferenceParmViewModel x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithChoicesObject() = 
            ObjectAction18.GetActionWithReferenceParmWithChoicesObject x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithChoicesService() = 
            ObjectAction18.GetActionWithReferenceParmWithChoicesService x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithChoicesViewModel() = 
            ObjectAction18.GetActionWithReferenceParmWithChoicesViewModel x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmsWithAutoCompleteObject() = 
            ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteObject x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmsWithAutoCompleteService() = 
            ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteService x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmsWithAutoCompleteViewModel() = 
            ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteViewModel x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteObject() = ObjectAction18.InvokeParmWithAutoCompleteObject x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteService() = ObjectAction18.InvokeParmWithAutoCompleteService x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteViewModel() = ObjectAction18.InvokeParmWithAutoCompleteViewModel x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteObjectErrorNoParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteObjectErrorNoParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteServiceErrorNoParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteServiceErrorNoParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteViewModelErrorNoParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorNoParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteObjectErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteObjectErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteServiceErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteServiceErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteViewModelErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm x.api
        
        [<Test>]
        member x.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm() = 
            ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesObject() = ObjectAction18.InvokeParmWithConditionalChoicesObject x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesService() = 
            ObjectAction18.InvokeParmWithConditionalChoicesService x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesViewModel() = 
            ObjectAction18.InvokeParmWithConditionalChoicesViewModel x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesObjectErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithConditionalChoicesObjectErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesServiceErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithConditionalChoicesServiceErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm() = 
            ObjectAction18.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesObject() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesObject x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesService() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesService x.api
        
        [<Test>]
        member x.InvokeValueParmWithConditionalChoicesViewModel() = 
            ObjectAction18.InvokeValueParmWithConditionalChoicesViewModel x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithDefaultObject() = 
            ObjectAction18.GetActionWithReferenceParmWithDefaultObject x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithDefaultService() = 
            ObjectAction18.GetActionWithReferenceParmWithDefaultService x.api
        
        [<Test>]
        member x.GetActionWithReferenceParmWithDefaultViewModel() = 
            ObjectAction18.GetActionWithReferenceParmWithDefaultViewModel x.api
        
        [<Test>]
        member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject x.api
        
        [<Test>]
        member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService x.api
        
        [<Test>]
        member x.GetActionWithChoicesAndDefaultViewModel() = 
            ObjectAction18.GetActionWithChoicesAndDefaultViewModel x.api
        
        [<Test>]
        member x.GetContributedActionOnContributee() = ObjectAction18.GetContributedActionOnContributee x.api
        
        [<Test>]
        member x.GetContributedActionOnContributeeOnBaseClass() = 
            ObjectAction18.GetContributedActionOnContributeeBaseClass x.api
        
        [<Test>]
        member x.GetContributedActionOnContributeeWithRef() = 
            ObjectAction18.GetContributedActionOnContributeeWithRef x.api
        
        [<Test>]
        member x.GetContributedActionOnContributeeWithValue() = 
            ObjectAction18.GetContributedActionOnContributeeWithValue x.api
        
        [<Test>]
        member x.GetContributedActionOnContributer() = ObjectAction18.GetContributedActionOnContributer x.api
        
        [<Test>]
        member x.GetContributedActionOnContributerOnBaseClass() = 
            ObjectAction18.GetContributedActionOnContributerBaseClass x.api
        
        [<Test>]
        member x.GetContributedActionOnContributerWithRef() = 
            ObjectAction18.GetContributedActionOnContributerWithRef x.api
        
        [<Test>]
        member x.GetContributedActionOnContributerWithValue() = 
            ObjectAction18.GetContributedActionOnContributerWithValue x.api
        
        [<Test>]
        member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject x.api
        
        [<Test>]
        member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService x.api
        
        [<Test>]
        member x.GetInvalidActionPropertyViewModel() = ObjectAction18.GetInvalidActionPropertyViewModel x.api
        
        [<Test>]
        member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject x.api
        
        [<Test>]
        member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService x.api
        
        [<Test>]
        member x.GetNotFoundActionPropertyViewModel() = ObjectAction18.GetNotFoundActionPropertyViewModel x.api
        
        [<Test>]
        member x.GetUserDisabledActionObject() = ObjectAction18.GetUserDisabledActionObject x.api
        
        [<Test>]
        member x.GetUserDisabledActionService() = ObjectAction18.GetUserDisabledActionService x.api
        
        [<Test>]
        member x.GetUserDisabledActionViewModel() = ObjectAction18.GetUserDisabledActionViewModel x.api
        
        [<Test>]
        member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject x.api
        
        [<Test>]
        member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService x.api
        
        [<Test>]
        member x.GetHiddenActionPropertyViewModel() = ObjectAction18.GetHiddenActionPropertyViewModel x.api
        
        [<Test>]
        member x.NotAcceptableGetActionWrongMediaTypeObject() = 
            ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject x.api
        
        [<Test>]
        member x.NotAcceptableGetActionWrongMediaTypeService() = 
            ObjectAction18.NotAcceptableGetActionWrongMediaTypeService x.api
        
        [<Test>]
        member x.NotAcceptableGetActionWrongMediaTypeViewModel() = 
            ObjectAction18.NotAcceptableGetActionWrongMediaTypeViewModel x.api
        
        [<Test>]
        member x.GetQueryActionObject() = ObjectAction18.GetQueryActionObject x.api
        
        [<Test>]
        member x.GetQueryActionService() = ObjectAction18.GetQueryActionService x.api
        
        [<Test>]
        member x.GetQueryActionViewModel() = ObjectAction18.GetQueryActionViewModel x.api
        
        [<Test>]
        member x.GetQueryActionWithParmsObject() = ObjectAction18.GetQueryActionWithParmsObject x.api
        
        [<Test>]
        member x.GetQueryActionWithParmsService() = ObjectAction18.GetQueryActionWithParmsService x.api
        
        [<Test>]
        member x.GetQueryActionWithParmsViewModel() = ObjectAction18.GetQueryActionWithParmsViewModel x.api
        
        [<Test>]
        member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject x.api
        
        [<Test>]
        member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService x.api
        
        [<Test>]
        member x.GetCollectionActionViewModel() = ObjectAction18.GetCollectionActionViewModel x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsViewModel() = ObjectAction18.GetCollectionActionWithParmsViewModel x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsObjectSimpleOnly() = 
            ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsServiceSimpleOnly() = 
            ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly x.api
        
        [<Test>]
        member x.GetCollectionActionWithParmsViewModelSimpleOnly() = 
            ObjectAction18.GetCollectionActionWithParmsViewModelSimpleOnly x.api
        
        [<Test>]
        member x.ActionNotFound() = ObjectAction18.ActionNotFound x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionReturnObjectObject() = 
            ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectObject x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionReturnObjectService() = 
            ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectService x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionReturnObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionContributedService() = ObjectActionInvoke19.PostInvokeActionContributedService x.api
        
        [<Test>]
        member x.PostInvokeCollectionContributedActionContributedService() = 
            ObjectActionInvoke19.PostInvokeCollectionContributedActionContributedService x.api
        
        [<Test>]
        member x.PostInvokeCollectionContributedActionContributedServiceMissingParm() = 
            ObjectActionInvoke19.PostInvokeCollectionContributedActionContributedServiceMissingParm x.api
        
        [<Test>]
        member x.PostInvokeActionReturnViewModelObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnViewModelObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnViewModelService() = 
            ObjectActionInvoke19.PostInvokeActionReturnViewModelService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnViewModelViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnViewModelViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnRedirectedObjectObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnRedirectedObjectService() = 
            ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnRedirectedObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeUserDisabledActionReturnObjectObject() = 
            ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectObject x.api
        
        [<Test>]
        member x.PostInvokeUserDisabledActionReturnObjectService() = 
            ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectService x.api
        
        [<Test>]
        member x.PostInvokeUserDisabledActionReturnObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullObjectObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullObjectService() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullObjectService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullViewModelObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullViewModelObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullViewModelService() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullViewModelService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullViewModelViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullViewModelViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnObjectObject x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectService() = ObjectActionInvoke19.PutInvokeActionReturnObjectService x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectViewModel() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectViewModel x.api
        
        [<Test>]
        member x.PutInvokeActionReturnViewModelObject() = 
            ObjectActionInvoke19.PutInvokeActionReturnViewModelObject x.api
        
        [<Test>]
        member x.PutInvokeActionReturnViewModelService() = 
            ObjectActionInvoke19.PutInvokeActionReturnViewModelService x.api
        
        [<Test>]
        member x.PutInvokeActionReturnViewModelViewModel() = 
            ObjectActionInvoke19.PutInvokeActionReturnViewModelViewModel x.api
        
        [<Test>]
        member x.PutInvokeActionReturnNullObjectObject() = 
            ObjectActionInvoke19.PutInvokeActionReturnNullObjectObject x.api
        
        [<Test>]
        member x.PutInvokeActionReturnNullObjectService() = 
            ObjectActionInvoke19.PutInvokeActionReturnNullObjectService x.api
        
        [<Test>]
        member x.PutInvokeActionReturnNullObjectViewModel() = 
            ObjectActionInvoke19.PutInvokeActionReturnNullObjectViewModel x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnObjectObject x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectService() = ObjectActionInvoke19.GetInvokeActionReturnObjectService x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModel() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionReturnViewModelObject() = 
            ObjectActionInvoke19.GetInvokeActionReturnViewModelObject x.api
        
        [<Test>]
        member x.GetInvokeActionReturnViewModelService() = 
            ObjectActionInvoke19.GetInvokeActionReturnViewModelService x.api
        
        [<Test>]
        member x.GetInvokeActionReturnViewModelViewModel() = 
            ObjectActionInvoke19.GetInvokeActionReturnViewModelViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionReturnNullObjectObject() = 
            ObjectActionInvoke19.GetInvokeActionReturnNullObjectObject x.api
        
        [<Test>]
        member x.GetInvokeActionReturnNullObjectService() = 
            ObjectActionInvoke19.GetInvokeActionReturnNullObjectService x.api
        
        [<Test>]
        member x.GetInvokeActionReturnNullObjectViewModel() = 
            ObjectActionInvoke19.GetInvokeActionReturnNullObjectViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeContribActionReturnObject() = ObjectActionInvoke19.PostInvokeContribActionReturnObject x.api
        
        [<Test>]
        member x.PostInvokeContribActionReturnObjectWithRefParm() = 
            ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithRefParm x.api
        
        [<Test>]
        member x.PostInvokeContribActionReturnObjectWithValueParm() = 
            ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithValueParm x.api
        
        [<Test>]
        member x.PostInvokeContribActionReturnObjectBaseClass() = 
            ObjectActionInvoke19.PostInvokeContribActionReturnObjectBaseClass x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectObjectWithMediaType() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectServiceWithMediaType() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType x.api
        
        [<Test>]
        member x.PostInvokeActionReturnObjectViewModelWithMediaType() = 
            ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelWithMediaType x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnScalarViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnScalarViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyScalarObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyScalarService() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyScalarViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullScalarObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullScalarService() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullScalarService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullScalarViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullScalarViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnVoidViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryObject() = ObjectActionInvoke19.GetInvokeActionReturnQueryObject x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryService() = ObjectActionInvoke19.GetInvokeActionReturnQueryService x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryObjectWithPaging() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryObjectWithPaging x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryServiceWithPaging() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryServiceWithPaging x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryViewModelWithPaging() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelWithPaging x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarService() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidService() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectService() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectObject() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObject x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectService() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectService x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectViewModel() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModel x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectObject() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectService() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectService x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModel x.api
        
        [<Test>]
        member x.VerifyPostInvokeActionWithReferenceParmsReturnObjectOnForm() = 
            ObjectActionInvoke19.VerifyPostInvokeActionWithReferenceParmsReturnObjectOnForm x.api

        [<Test>]
        member x.VerifyPostInvokeActionMissingParmOnForm() = 
            ObjectActionInvoke19.VerifyPostInvokeActionMissingParmOnForm x.api

        [<Test>]
        member x.GetInvokeActionWithParmReturnObjectObject() = 
            ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithParmReturnObjectService() = 
            ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectService x.api
        
        [<Test>]
        member x.GetInvokeActionWithParmReturnObjectViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject x.api
        
        [<Test>]
        member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService x.api
        
        [<Test>]
        member x.NotFoundActionInvokeViewModel() = ObjectActionInvoke19.NotFoundActionInvokeViewModel x.api
        
        [<Test>]
        member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject x.api
        
        [<Test>]
        member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService x.api
        
        [<Test>]
        member x.HiddenActionInvokeViewModel() = ObjectActionInvoke19.HiddenActionInvokeViewModel x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsViewModel() = ObjectActionInvoke19.GetActionWithSideEffectsViewModel x.api
        
        [<Test>]
        member x.GetActionWithIdempotentObject() = ObjectActionInvoke19.GetActionWithIdempotentObject x.api
        
        [<Test>]
        member x.GetActionWithIdempotentService() = ObjectActionInvoke19.GetActionWithIdempotentService x.api
        
        [<Test>]
        member x.GetActionWithIdempotentViewModel() = ObjectActionInvoke19.GetActionWithIdempotentViewModel x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyObject() = ObjectActionInvoke19.PutActionWithQueryOnlyObject x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyService() = ObjectActionInvoke19.PutActionWithQueryOnlyService x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyViewModel() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModel x.api
        
        [<Test>]
        member x.NotAcceptableGetInvokeWrongMediaTypeObject() = 
            ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeObject x.api
        
        [<Test>]
        member x.NotAcceptableGetInvokeWrongMediaTypeService() = 
            ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeService x.api
        
        [<Test>]
        member x.NotAcceptableGetInvokeWrongMediaTypeViewModel() = 
            ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeViewModel x.api
        
        [<Test>]
        member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject x.api
        
        [<Test>]
        member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService x.api
        
        [<Test>]
        member x.MissingParmsOnPostViewModel() = ObjectActionInvoke19.MissingParmsOnPostViewModel x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeViewModel() = ObjectActionInvoke19.DisabledActionPostInvokeViewModel x.api
        
        [<Test>]
        member x.UserDisabledActionPostInvokeObject() = ObjectActionInvoke19.UserDisabledActionPostInvokeObject x.api
        
        [<Test>]
        member x.UserDisabledActionPostInvokeService() = ObjectActionInvoke19.UserDisabledActionPostInvokeService x.api
        
        [<Test>]
        member x.UserDisabledActionPostInvokeViewModel() = 
            ObjectActionInvoke19.UserDisabledActionPostInvokeViewModel x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeViewModel() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModel x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeViewModel() = ObjectActionInvoke19.HiddenActionPostInvokeViewModel x.api
        
        [<Test>]
        member x.NotAcceptablePostInvokeWrongMediaTypeObject() = 
            ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject x.api
        
        [<Test>]
        member x.NotAcceptablePostInvokeWrongMediaTypeService() = 
            ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService x.api
        
        [<Test>]
        member x.NotAcceptablePostInvokeWrongMediaTypeViewModel() = 
            ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeViewModel x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorObject() = ObjectActionInvoke19.PostQueryActionWithErrorObject x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorService() = ObjectActionInvoke19.PostQueryActionWithErrorService x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorViewModel() = ObjectActionInvoke19.PostQueryActionWithErrorViewModel x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorObject() = ObjectActionInvoke19.GetQueryActionWithErrorObject x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorService() = ObjectActionInvoke19.GetQueryActionWithErrorService x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorViewModel() = ObjectActionInvoke19.GetQueryActionWithErrorViewModel x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryObject() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryService() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryViewModel() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModel x.api
        
        [<Test>]
        member x.WrongTypeFormalParmsOnPostQueryObject() = 
            ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryObject x.api
        
        [<Test>]
        member x.WrongTypeFormalParmsOnPostQueryService() = 
            ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryService x.api
        
        [<Test>]
        member x.WrongTypeFormalParmsOnPostQueryViewModel() = 
            ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryViewModel x.api
        
        [<Test>]
        member x.EmptyFormalParmsOnPostQueryObject() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryObject x.api
        
        [<Test>]
        member x.EmptyFormalParmsOnPostQueryService() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryService x.api
        
        [<Test>]
        member x.EmptyFormalParmsOnPostQueryViewModel() = 
            ObjectActionInvoke19.EmptyFormalParmsOnPostQueryViewModel x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObject x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryService() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostQueryService x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryViewModel() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModel x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryObject() = ObjectActionInvoke19.MissingParmsOnGetQueryObject x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryService() = ObjectActionInvoke19.MissingParmsOnGetQueryService x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryViewModel() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModel x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryObject() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObject x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryService() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryService x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryViewModel() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModel x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryObject() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObject x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryService() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryService x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryViewModel() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModel x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObject x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryService() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryService x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryViewModel() = 
            ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModel x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObject x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryService x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryViewModel() = 
            ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModel x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryObject() = ObjectActionInvoke19.DisabledActionInvokeQueryObject x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryService() = ObjectActionInvoke19.DisabledActionInvokeQueryService x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryViewModel() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObject() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleService() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleService x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarMissingParmsSimpleObject() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarMissingParmsSimpleService() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleService x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarMissingParmsSimpleViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionReturnQueryObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryService() = ObjectActionInvoke19.PostInvokeActionReturnQueryService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnQueryViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryObject() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryService() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryService x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModel x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionObject() = ObjectActionInvoke19.PostInvokeOverloadedActionObject x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionService() = ObjectActionInvoke19.PostInvokeOverloadedActionService x.api
        
        [<Test>]
        member x.PostInvokeOverloadedActionViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryObject() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryService() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryService x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorService() = 
            ObjectActionInvoke19.PostCollectionActionWithErrorService x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorViewModel() = 
            ObjectActionInvoke19.PostCollectionActionWithErrorViewModel x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionViewModel() = 
            ObjectActionInvoke19.MissingParmsOnPostCollectionViewModel x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionObject() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionService() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionViewModel() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModel x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionObject() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionService() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionViewModel() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModel x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionObject() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionObject x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionService() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionService x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionViewModel() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionService() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = 
            ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyCollectionObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyCollectionService() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnEmptyCollectionViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullCollectionObject() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullCollectionService() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService x.api
        
        [<Test>]
        member x.PostInvokeActionReturnNullCollectionViewModel() = 
            ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionService() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionObject() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionObject x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionService() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionService x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionViewModel() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel x.api
        
        [<Test>]
        member x.PostQueryActionWithValidateFailObject() = 
            ObjectActionInvoke19.PostQueryActionWithValidateFailObject x.api
        
        [<Test>]
        member x.PostQueryActionWithValidateFailService() = 
            ObjectActionInvoke19.PostQueryActionWithValidateFailService x.api
        
        [<Test>]
        member x.PostQueryActionWithValidateFailViewModel() = 
            ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel x.api
        
        [<Test>]
        member x.PostQueryActionWithCrossValidateFailObject() = 
            ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject x.api
        
        [<Test>]
        member x.PostQueryActionWithCrossValidateFailService() = 
            ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService x.api
        
        [<Test>]
        member x.PostQueryActionWithCrossValidateFailViewModel() = 
            ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryObjectValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryServiceValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnGetQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = 
            ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = 
            ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryObjectValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryServiceValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionInvokeCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionInvokeObjectValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionInvokeServiceValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionInvokeViewModelValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionInvokeObjectValidateOnly() = 
            ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionInvokeServiceValidateOnly() = 
            ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionInvokeViewModelValidateOnly() = 
            ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsObjectValidateOnly() = 
            ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsServiceValidateOnly() = 
            ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithSideEffectsViewModelValidateOnly() = 
            ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithIdempotentObjectValidateOnly() = 
            ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithIdempotentServiceValidateOnly() = 
            ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly x.api
        
        [<Test>]
        member x.GetActionWithIdempotentViewModelValidateOnly() = 
            ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyObjectValidateOnly() = 
            ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyServiceValidateOnly() = 
            ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly x.api
        
        [<Test>]
        member x.PutActionWithQueryOnlyViewModelValidateOnly() = 
            ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorObjectValidateOnly() = 
            ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorServiceValidateOnly() = 
            ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly x.api
        
        [<Test>]
        member x.GetQueryActionWithErrorViewModelValidateOnly() = 
            ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorObjectValidateOnly() = 
            ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorServiceValidateOnly() = 
            ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly x.api
        
        [<Test>]
        member x.PostCollectionActionWithErrorViewModelValidateOnly() = 
            ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostObjectValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostServiceValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly x.api
        
        [<Test>]
        member x.MissingParmsOnPostViewModelValidateOnly() = 
            ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.InvalidUrlOnPostQueryObjectValidateOnly() = 
            ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly x.api
        
        [<Test>]
        member x.InvalidUrlOnPostQueryServiceValidateOnly() = 
            ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly x.api
        
        [<Test>]
        member x.InvalidUrlOnPostQueryViewModelValidateOnly() = 
            ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeObjectValidateOnly() = 
            ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeServiceValidateOnly() = 
            ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly x.api
        
        [<Test>]
        member x.DisabledActionPostInvokeViewModelValidateOnly() = 
            ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeObjectValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeServiceValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly x.api
        
        [<Test>]
        member x.NotFoundActionPostInvokeViewModelValidateOnly() = 
            ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeObjectValidateOnly() = 
            ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeServiceValidateOnly() = 
            ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly x.api
        
        [<Test>]
        member x.HiddenActionPostInvokeViewModelValidateOnly() = 
            ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorObjectValidateOnly() = 
            ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorServiceValidateOnly() = 
            ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly x.api
        
        [<Test>]
        member x.PostQueryActionWithErrorViewModelValidateOnly() = 
            ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.api
        
        [<Test>]
        member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = 
            ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly x.api
        
        [<Test>]
        member x.GetIsSubTypeOfReturnFalseSimpleParms() = 
            DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms x.api
        
        [<Test>]
        member x.GetIsSuperTypeOfReturnFalseSimpleParms() = 
            DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms x.api
        
        [<Test>]
        member x.GetIsSubTypeOfReturnTrueSimpleParms() = 
            DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms x.api
        
        [<Test>]
        member x.GetIsSuperTypeOfReturnTrueSimpleParms() = 
            DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms x.api
        
        [<Test>]
        member x.GetIsSubTypeOfReturnFalseFormalParms() = 
            DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms x.api
        
        [<Test>]
        member x.GetIsSuperTypeOfReturnFalseFormalParms() = 
            DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms x.api
        
        [<Test>]
        member x.GetIsSubTypeOfReturnTrueFormalParms() = 
            DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms x.api
        
        [<Test>]
        member x.GetIsSuperTypeOfReturnTrueFormalParms() = 
            DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms x.api
        
        [<Test>]
        member x.NotFoundTypeIsSubTypeOfSimpleParms() = 
            DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms x.api
        
        [<Test>]
        member x.NotFoundTypeIsSuperTypeOfSimpleParms() = 
            DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms x.api
        
        [<Test>]
        member x.NotFoundTypeIsSubTypeOfFormalParms() = 
            DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms x.api
        
        [<Test>]
        member x.NotFoundTypeIsSuperTypeOfFormalParms() = 
            DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms x.api
        
        [<Test>]
        member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms x.api
        
        [<Test>]
        member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms x.api
        
        [<Test>]
        member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = 
            DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms x.api
        
        [<Test>]
        member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = 
            DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms x.api
        
        [<Test>]
        member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = 
            DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms x.api
        
        [<Test>]
        member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = 
            DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms x.api
        
        [<Test>]
        member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf x.api
        
        [<Test>]
        member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf x.api
        
        [<Test>]
        member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf x.api
        
        [<Test>]
        member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf x.api
    end

