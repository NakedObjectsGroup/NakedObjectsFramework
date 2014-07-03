module NakedObjects.Rest.Test.Nof4DomainType
open NUnit.Framework
open NakedObjects
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Boot
open NakedObjects.Architecture.Adapter
open NakedObjects.Core.Context
open NakedObjects.Persistor.Objectstore.Inmemory
open RestfulObjects.Test.Data
open RestfulObjects.Mvc
open RestfulObjects.Mvc.Media
open System
open RestfulObjects.Snapshot.Utility 
open RestfulObjects.Snapshot.Constants
open System.Threading
open System.Security.Principal
open System.Web.Http

let mapper = new TestTypeCodeMapper()
let keyMapper = new TestKeyCodeMapper()


[<TestFixture>]
type Nof4TestsDomainType() = class      
    inherit  NakedObjects.Xat.AcceptanceTestCase()    
            
    [<TestFixtureSetUp>]
    member x.Setup() =     
        x.InitializeNakedObjectsFramework()
        MemoryObjectStore.DiscardObjects()
    
    [<SetUp>]
    member x.StartTest() =           
        x.Fixtures.InstallFixtures(NakedObjectsContext.ObjectPersistor)
        UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
        RestfulObjectsControllerBase.IsReadOnly <- false  
        let p = new GenericPrincipal(new GenericIdentity("REST"), [||])
        Thread.CurrentPrincipal <- p;
        GlobalConfiguration.Configuration.Formatters.[0] <- new JsonNetFormatter(null);
        RestTestFunctions.ctt <- fun(code) -> mapper.TypeStringFromCode(code)
        RestTestFunctions.ttc <- fun(typ) -> mapper.CodeFromTypeString(typ)
        RestTestFunctions.ctk <- fun(code) -> keyMapper.KeyStringFromCode(code)
        RestTestFunctions.ktc <- fun(key) -> keyMapper.CodeFromKeyString(key)

    [<TearDown>]
    member x.EndTest() =    
        RestfulObjectsControllerBase.DomainModel <- RestControlFlags.DomainModelType.Selectable
        RestfulObjectsControllerBase.ConcurrencyChecking <- false
        RestfulObjectsControllerBase.CacheSettings <- (0, 3600, 86400)
        MemoryObjectStore.DiscardObjects()
     
    [<TestFixtureTearDown>]
    member x.TearDown() = 
        x.CleanupNakedObjectsFramework()

    override x.MenuServices 
        with get() : IServicesInstaller  =      
            box (new ServicesInstaller([| box (new RestDataRepository());box (new WithActionService()) |])) :?> IServicesInstaller

    override x.SystemServices 
        with get() : IServicesInstaller  =      
            box (new ServicesInstaller([| box (new TestTypeCodeMapper());box (new TestKeyCodeMapper()) |])) :?> IServicesInstaller

    override x.ContributedActions 
        with get() : IServicesInstaller  =      
            box (new ServicesInstaller([| box (new ContributorService()) |])) :?> IServicesInstaller

    override x.Persistor 
        with get() : IObjectPersistorInstaller = 
            let inst = new InMemoryObjectPersistorInstaller()
            inst.SimpleOidGeneratorStart <- new System.Nullable<int>(100)
            box (inst) :?> IObjectPersistorInstaller

    override x.Fixtures 
        with get() : IFixturesInstaller = 
            box (new FixturesInstaller([| box (new RestDataFixtureUnitTests()) |])) :?> IFixturesInstaller 

    member x.API = 
        let api = new RestfulObjectsControllerBase()
        api.Surface <-  new NakedObjects.Surface.Nof4.Implementation.NakedObjectsSurface(new NakedObjects.Surface.Nof4.Utility.ExternalOid())  
        api

    // HomePage5
    [<Test>] 
    member x.GetHomePage() = HomePage5.GetHomePage x.API
    [<Test>] 
    member x.GetHomePageSimple() = HomePage5.GetHomePageSimple x.API
    [<Test>] 
    member x.GetHomePageFormal() = HomePage5.GetHomePageFormal x.API
    [<Test>] 
    member x.GetHomePageWithMediaType() = HomePage5.GetHomePageWithMediaType x.API
    [<Test>] 
    member x.NotAcceptableGetHomePage() = HomePage5.NotAcceptableGetHomePage x.API
    [<Test>] 
    member x.InvalidDomainModelGetHomePage() = HomePage5.InvalidDomainModelGetHomePage x.API
    // User6
    [<Test>] 
    member x.GetUser() = User6.GetUser x.API
    [<Test>] 
    member x.GetUserWithMediaType() = User6.GetUserWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser x.API
    // DomainServices7
    [<Test>] 
    member x.GetDomainServices() = DomainServices7.GetDomainServices x.API 
    [<Test>] 
    member x.GetDomainServicesFormal() = DomainServices7.GetDomainServicesFormal x.API 
    [<Test>] 
    member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices x.API 
    // Version8
    [<Test>] 
    member x.GetVersion() = Version8.GetVersion x.API 
    [<Test>] 
    member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion x.API 
    //Objects9
    [<Test>]
    member x.GetMostSimpleTransientObject() = Objects9.GetMostSimpleTransientObject x.API  
    [<Test>]
    member x.GetMostSimpleTransientObjectSimpleOnly() = Objects9.GetMostSimpleTransientObjectSimpleOnly x.API  
    [<Test>]
    member x.GetMostSimpleTransientObjectFormalOnly() = Objects9.GetMostSimpleTransientObjectFormalOnly x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObject() = Objects9.PersistMostSimpleTransientObject x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectSimpleOnly() = Objects9.PersistMostSimpleTransientObjectSimpleOnly x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectFormalOnly() = Objects9.PersistMostSimpleTransientObjectFormalOnly x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectValidateOnly() = Objects9.PersistMostSimpleTransientObjectValidateOnly x.API  
    [<Test>]
    member x.GetWithValueTransientObject() = Objects9.GetWithValueTransientObject x.API  
    [<Test>]
    member x.GetWithReferenceTransientObject() = Objects9.GetWithReferenceTransientObject x.API  
    [<Test>]
    member x.GetWithCollectionTransientObject() = Objects9.GetWithCollectionTransientObject x.API  
    [<Test>]
    member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectFormalOnly() = Objects9.PersistWithValueTransientObjectFormalOnly x.API  
    [<Test>]
    member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject x.API  
    [<Test>]
    member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnly() = Objects9.PersistWithValueTransientObjectValidateOnly x.API  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnly() = Objects9.PersistWithReferenceTransientObjectValidateOnly x.API  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnly() = Objects9.PersistWithCollectionTransientObjectValidateOnly x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFail x.API  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnlyFail() = Objects9.PersistWithReferenceTransientObjectValidateOnlyFail x.API  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnlyFail() = Objects9.PersistWithCollectionTransientObjectValidateOnlyFail x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail x.API  
    [<Test>]
    member x.PersistWithValueTransientObjectFailInvalid() = Objects9.PersistWithValueTransientObjectFailInvalid x.API 
    [<Test>]
    member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail x.API  
    [<Test>]
    member x.PersistWithReferenceTransientObjectFailInvalid() = Objects9.PersistWithReferenceTransientObjectFailInvalid x.API  
    [<Test>]
    member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgs() = Objects9.PersistMostSimpleTransientObjectMissingArgs x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgsValidateOnly() = Objects9.PersistMostSimpleTransientObjectMissingArgsValidateOnly x.API  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingMemberArgs() = Objects9.PersistMostSimpleTransientObjectMissingMemberArgs x.API  
    [<Test>]    
    member x.PersistMostSimpleTransientObjectNullDomainType() = Objects9.PersistMostSimpleTransientObjectNullDomainType x.API  
    [<Test>]   
    member x.PersistMostSimpleTransientObjectEmptyDomainType() = Objects9.PersistMostSimpleTransientObjectEmptyDomainType x.API   
    [<Test>]
    member x.PersistMostSimpleTransientObjectMalformedMemberArgs() = Objects9.PersistMostSimpleTransientObjectMalformedMemberArgs x.API  
    [<Test>]
    member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject x.API
    [<Test>]
    member x.PersistNoKeyTransientObject() = Objects9.PersistNoKeyTransientObject x.API
    // Error10
    [<Test>]
    member x.Error() = Error10.Error x.API 
    [<Test>]
    member x.NotAcceptableError() = Error10.NotAcceptableError x.API 
    // DomainObject14
    [<Test>]
    member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject x.API  
    [<Test>]
    member x.GetWithAttachmentsObject() = DomainObject14.GetWithAttachmentsObject x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSelectable() = DomainObject14.GetMostSimpleObjectConfiguredSelectable x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredNone() = DomainObject14.GetMostSimpleObjectConfiguredNone x.API  
    [<Test>]
    member x.GetMostSimpleObjectFormalOnly() = DomainObject14.GetMostSimpleObjectFormalOnly x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredFormalOnly() = DomainObject14.GetMostSimpleObjectConfiguredFormalOnly x.API 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSimpleOnly() = DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly x.API 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredCaching() = DomainObject14.GetMostSimpleObjectConfiguredCaching x.API 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredOverrides() = DomainObject14.GetMostSimpleObjectConfiguredOverrides x.API 
    [<Test>]
    member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly x.API 
    [<Test>]
    member x.GetWithDateTimeKeyObject() = DomainObject14.GetWithDateTimeKeyObject x.API  
    [<Test>]
    member x.GetVerySimpleEagerObject() = DomainObject14.GetVerySimpleEagerObject x.API   
    [<Test>]
    member x.GetWithValueObject() = DomainObject14.GetWithValueObject x.API 
    [<Test>]
    member x.GetWithScalarsObject() = DomainObject14.GetWithScalarsObject x.API 
    [<Test>]
    member x.GetWithValueObjectUserAuth() = DomainObject14.GetWithValueObjectUserAuth x.API 
    [<Test>]
    member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType x.API 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeSimple() = DomainObject14.GetMostSimpleObjectWithDomainTypeSimple x.API 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeFormal() = DomainObject14.GetMostSimpleObjectWithDomainTypeFormal x.API 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileSimple() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileSimple x.API 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileFormal() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileFormal x.API 
    [<Test>]
    member x.GetRedirectedObject() = DomainObject14.GetRedirectedObject x.API 
    [<Test>]
    member x.PutWithValueObject() = DomainObject14.PutWithValueObject x.API 
    [<Test>]
    member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess x.API 
    [<Test>]
    member x.PutWithScalarsObject() = DomainObject14.PutWithScalarsObject x.API
    [<Test>]
    member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail x.API 
    [<Test>]
    member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch x.API 
    [<Test>]
    member x.PutWithReferenceObject() = DomainObject14.PutWithReferenceObject x.API 
    [<Test>]
    member x.PutWithReferenceObjectValidateOnly() = DomainObject14.PutWithReferenceObjectValidateOnly x.API 
    [<Test>]
    member x.GetWithActionObject() = DomainObject14.GetWithActionObject x.API  
    [<Test>]
    member x.GetWithActionObjectSimpleOnly() = DomainObject14.GetWithActionObjectSimpleOnly x.API   
    [<Test>]
    member x.GetWithActionObjectFormalOnly() = DomainObject14.GetWithActionObjectFormalOnly x.API        
    [<Test>]
    member x.GetWithReferenceObject() = DomainObject14.GetWithReferenceObject x.API 
    [<Test>]
    member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject x.API 
    [<Test>]
    member x.GetWithCollectionObjectFormalOnly() = DomainObject14.GetWithCollectionObjectFormalOnly x.API 
    [<Test>]
    member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly x.API 
    [<Test>]
    member x.InvalidGetObject() = DomainObject14.InvalidGetObject x.API 
    [<Test>]
    member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject x.API 
    [<Test>]    
    member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType x.API 
    [<Test>]    
    member x.GetObjectIgnoreWrongDomainType() = DomainObject14.GetObjectIgnoreWrongDomainType x.API 
    [<Test>]
    member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs x.API 
    [<Test>]
    member x.PutWithValueObjectMissingArgsValidateOnly() = DomainObject14.PutWithValueObjectMissingArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs x.API 
    [<Test>]
    member x.PutWithValueObjectMalformedDateTimeArgs() = DomainObject14.PutWithValueObjectMalformedDateTimeArgs x.API 
    [<Test>]
    member x.PutWithValueObjectMalformedArgsValidateOnly() = DomainObject14.PutWithValueObjectMalformedArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue x.API 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue x.API 

    [<Test>]
    member x.PutWithReferenceObjectNotFoundArgsValue() = DomainObject14.PutWithReferenceObjectNotFoundArgsValue x.API 

    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgs() = DomainObject14.PutWithReferenceObjectMalformedArgs x.API 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgsValidateOnly() = DomainObject14.PutWithReferenceObjectMalformedArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectDisabledValue() = DomainObject14.PutWithValueObjectDisabledValue x.API 
    [<Test>]
    member x.PutWithValueObjectDisabledValueValidateOnly() = DomainObject14.PutWithValueObjectDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValue() = DomainObject14.PutWithReferenceObjectDisabledValue x.API 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValueValidateOnly() = DomainObject14.PutWithReferenceObjectDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectInvisibleValue() = DomainObject14.PutWithValueObjectInvisibleValue x.API 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValue() = DomainObject14.PutWithReferenceObjectInvisibleValue x.API 
    [<Test>]
    member x.PutWithValueObjectInvisibleValueValidateOnly() = DomainObject14.PutWithValueObjectInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValueImmutableObject() = DomainObject14.PutWithValueImmutableObject x.API 
    [<Test>]
    member x.PutWithReferenceImmutableObject() = DomainObject14.PutWithReferenceImmutableObject x.API 
    [<Test>]
    member x.PutWithValueImmutableObjectValidateOnly() = DomainObject14.PutWithValueImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceImmutableObjectValidateOnly() = DomainObject14.PutWithReferenceImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsName() = DomainObject14.PutWithValueObjectInvalidArgsName x.API 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsNameValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsNameValidateOnly x.API 
    [<Test>]   
    member x.NotAcceptablePutObjectWrongMediaType() = DomainObject14.NotAcceptablePutObjectWrongMediaType x.API  
    [<Test>]
    member x.PutWithValueInternalError() = DomainObject14.PutWithValueInternalError x.API 
    [<Test>]
    member x.PutWithReferenceInternalError() = DomainObject14.PutWithReferenceInternalError x.API 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidation() = DomainObject14.PutWithValueObjectFailCrossValidation x.API 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidationValidateOnly() = DomainObject14.PutWithValueObjectFailCrossValidationValidateOnly x.API 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidation() = DomainObject14.PutWithReferenceObjectFailsCrossValidation x.API 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidationValidateOnly() = DomainObject14.PutWithReferenceObjectFailsCrossValidationValidateOnly x.API
    [<Test>]
    member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey x.API 
    [<Test>]
    member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType x.API 
    [<Test>] 
    [<Ignore>] // no longer fails no sure if an issue - seems no reason to make fail ? 
    member x.ObjectNotFoundAbstractType() = DomainObject14.ObjectNotFoundAbstractType x.API  
    // view models 
    [<Test>]
    member x.GetMostSimpleViewModel() = DomainObject14.GetMostSimpleViewModel x.API  
    [<Test>]
    member x.GetWithValueViewModel() = DomainObject14.GetWithValueViewModel x.API  
    [<Test>]
    member x.GetWithReferenceViewModel() = DomainObject14.GetWithReferenceViewModel x.API
    [<Test>]
    member x.GetWithNestedViewModel() = DomainObject14.GetWithNestedViewModel x.API
    [<Test>]
    member x.PutWithReferenceViewModel() = DomainObject14.PutWithReferenceViewModel x.API
    [<Test>]
    member x.PutWithNestedViewModel() = DomainObject14.PutWithNestedViewModel x.API
    [<Test>]
    member x.PutWithValueViewModel() = DomainObject14.PutWithValueViewModel x.API
    // DomainService15
    [<Test>] 
    member x.GetService() = DomainService15.GetService x.API 
    [<Test>] 
    member x.GetContributorService() = DomainService15.GetContributorService x.API 
    [<Test>] 
    member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly x.API 
    [<Test>] 
    member x.GetServiceFormalOnly() = DomainService15.GetServiceFormalOnly x.API 
    [<Test>] 
    member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType x.API 
    [<Test>]
    member x.GetWithActionService() = DomainService15.GetWithActionService x.API   
    [<Test>]
    member x.InvalidGetService() = DomainService15.InvalidGetService x.API 
    [<Test>]
    member x.NotFoundGetService() = DomainService15.NotFoundGetService x.API 
    [<Test>]   
    member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType x.API 
    // ObjectProperty16
    [<Test>]
    member x.GetValueProperty() = ObjectProperty16.GetValueProperty x.API 
    [<Test>]
    member x.GetFileAttachmentProperty() = ObjectProperty16.GetFileAttachmentProperty x.API
    [<Test>]
    member x.GetImageAttachmentProperty() = ObjectProperty16.GetImageAttachmentProperty x.API 
    [<Test>]
    member x.GetFileAttachmentValue() = ObjectProperty16.GetFileAttachmentValue x.API 
    [<Test>]
    member x.GetAttachmentValueWrongMediaType() = ObjectProperty16.GetAttachmentValueWrongMediaType x.API 
    [<Test>]
    member x.GetImageAttachmentValue() = ObjectProperty16.GetImageAttachmentValue x.API  
    [<Test>]
    member x.GetValuePropertyViewModel() = ObjectProperty16.GetValuePropertyViewModel x.API 
    [<Test>]
    member x.GetEnumValueProperty() = ObjectProperty16.GetEnumValueProperty x.API 
    [<Test>]
    member x.GetValuePropertyUserAuth() = ObjectProperty16.GetValuePropertyUserAuth x.API     
    [<Test>]
    member x.GetValuePropertyFormalOnly() = ObjectProperty16.GetValuePropertyFormalOnly x.API 
    [<Test>]
    member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly x.API 
    [<Test>]
    member x.GetStringValueProperty() = ObjectProperty16.GetStringValueProperty x.API 
    [<Test>]
    member x.GetBlobValueProperty() = ObjectProperty16.GetBlobValueProperty x.API 
    [<Test>]
    member x.GetClobValueProperty() = ObjectProperty16.GetClobValueProperty x.API 
    [<Test>]
    member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType x.API 
    [<Test>]
    member x.GetChoicesValueProperty() = ObjectProperty16.GetChoicesValueProperty x.API 
    [<Test>]
    member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty x.API 
    [<Test>]
    member x.GetUserDisabledValueProperty() = ObjectProperty16.GetUserDisabledValueProperty x.API 
    [<Test>]
    member x.GetUserDisabledValuePropertyAuthorised() = ObjectProperty16.GetUserDisabledValuePropertyAuthorised x.API 
    [<Test>]
    member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty x.API 
    [<Test>]
    member x.GetAutoCompleteProperty() = ObjectProperty16.GetAutoCompleteProperty x.API 
    [<Test>]
    member x.InvokeAutoComplete() = ObjectProperty16.InvokeAutoComplete x.API 

    [<Test>]
    member x.InvokeAutoCompleteErrorNoParm() = ObjectProperty16.InvokeAutoCompleteErrorNoParm x.API 
    [<Test>]
    member x.InvokeAutoCompleteErrorMalformedParm() = ObjectProperty16.InvokeAutoCompleteErrorMalformedParm x.API 
    [<Test>]
    member x.InvokeAutoCompleteErrorUnrecognisedParm() = ObjectProperty16.InvokeAutoCompleteErrorUnrecognisedParm x.API 


    [<Test>]
    member x.InvokeConditionalChoicesReference() = ObjectProperty16.InvokeConditionalChoicesReference x.API 

    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorMalformedParm x.API 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorNoParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorNoParm x.API 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorUnrecognisedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorUnrecognisedParm x.API 



    [<Test>]
    member x.InvokeConditionalChoicesValue() = ObjectProperty16.InvokeConditionalChoicesValue x.API 

    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMalformedParm x.API 
    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMissingParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMissingParm x.API 


    [<Test>]
    member x.GetReferencePropertyViewModel() = ObjectProperty16.GetReferencePropertyViewModel x.API 
    [<Test>]
    member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty x.API 
    [<Test>]
    member x.GetChoicesReferenceProperty() = ObjectProperty16.GetChoicesReferenceProperty x.API 
    [<Test>]
    member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty x.API 
    [<Test>]
    member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty x.API 
    [<Test>]
    member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty x.API
    [<Test>]
    member x.GetUserHiddenValueProperty() = ObjectProperty16.GetUserHiddenValueProperty x.API  
    [<Test>]
    member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty x.API 
    [<Test>]    
    member x.NotAcceptableGetPropertyWrongMediaType() = ObjectProperty16.NotAcceptableGetPropertyWrongMediaType x.API 
    [<Test>]
    member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty x.API 
    [<Test>]
    member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty x.API 
    [<Test>]
    member x.GetPropertyAsCollection() = ObjectProperty16.GetPropertyAsCollection x.API 
    [<Test>]
    member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess x.API 
    [<Test>]
    member x.PutDateTimeValuePropertySuccess() = ObjectProperty16.PutDateTimeValuePropertySuccess x.API 
    [<Test>]
    member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess x.API 
    [<Test>]
    member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail x.API 
    [<Test>]
    member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch x.API 
    [<Test>]
    member x.PutUserDisabledValuePropertySuccess() = ObjectProperty16.PutUserDisabledValuePropertySuccess x.API 
    [<Test>]
    member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.PutClobPropertyBadRequest() = ObjectProperty16.PutClobPropertyBadRequest x.API 
    [<Test>]
    member x.PutBlobPropertyBadRequest() = ObjectProperty16.PutBlobPropertyBadRequest x.API 
    [<Test>]
    member x.DeleteValuePropertySuccess() = ObjectProperty16.DeleteValuePropertySuccess x.API 
    [<Test>]
    member x.DeleteValuePropertySuccessValidateOnly() = ObjectProperty16.DeleteValuePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.PutNullValuePropertySuccess() = ObjectProperty16.PutNullValuePropertySuccess x.API 
    [<Test>]
    member x.PutNullValuePropertySuccessValidateOnly() = ObjectProperty16.PutNullValuePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.PutReferencePropertySuccess() = ObjectProperty16.PutReferencePropertySuccess x.API 
    [<Test>]
    member x.PutReferencePropertySuccessValidateOnly() = ObjectProperty16.PutReferencePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertySuccess() = ObjectProperty16.DeleteReferencePropertySuccess x.API 
    [<Test>]
    member x.DeleteReferencePropertySuccessValidateOnly() = ObjectProperty16.DeleteReferencePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.PutNullReferencePropertySuccess() = ObjectProperty16.PutNullReferencePropertySuccess x.API 
    [<Test>]
    member x.PutNullReferencePropertySuccessValidateOnly() = ObjectProperty16.PutNullReferencePropertySuccessValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyMissingArgs() = ObjectProperty16.PutWithValuePropertyMissingArgs x.API 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgs() = ObjectProperty16.PutWithValuePropertyMalformedArgs x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue x.API 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidation() = ObjectProperty16.PutWithReferencePropertyFailCrossValidation x.API 

    [<Test>]
    member x.PutWithReferencePropertyMalformedArgs() = ObjectProperty16.PutWithReferencePropertyMalformedArgs x.API 

    [<Test>]
    member x.PutWithValuePropertyFailCrossValidation() = ObjectProperty16.PutWithValuePropertyFailCrossValidation x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValue() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValue x.API 
    [<Test>]
    member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue x.API 
    [<Test>]
    member x.PutWithValuePropertyUserDisabledValue() = ObjectProperty16.PutWithValuePropertyUserDisabledValue x.API 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue x.API 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValue() = ObjectProperty16.PutWithReferencePropertyInvisibleValue x.API 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObject() = ObjectProperty16.PutWithValuePropertyOnImmutableObject x.API 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObject() = ObjectProperty16.PutWithReferencePropertyOnImmutableObject x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName x.API 
    [<Test>]    
    member x.NotAcceptablePutPropertyWrongMediaType() = ObjectProperty16.NotAcceptablePutPropertyWrongMediaType x.API 
    [<Test>]
    member x.PutWithValuePropertyMissingArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithReferencePropertyFailCrossValidationValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithValuePropertyFailCrossValidationValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithValuePropertyOnImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithReferencePropertyOnImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly x.API   
    [<Test>]
    member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError x.API 
    [<Test>]
    member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError x.API 
    [<Test>]
    member x.DeleteValuePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteValuePropertyOnImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteReferencePropertyOnImmutableObjectValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue x.API 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue x.API 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue x.API 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue x.API 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObject() = ObjectProperty16.DeleteValuePropertyOnImmutableObject x.API 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObject() = ObjectProperty16.DeleteReferencePropertyOnImmutableObject x.API 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName x.API 
    [<Test>]    
    member x.NotAcceptableDeletePropertyWrongMediaType() = ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType x.API 
    [<Test>]
    member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError x.API 
    [<Test>]
    member x.DeleteReferencePropertyInternalError() = ObjectProperty16.DeleteReferencePropertyInternalError x.API 
    [<Test>]
    member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound x.API 
    // ObjectCollection17
    [<Test>]
    member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty x.API 
    [<Test>]
    member x.GetCollectionPropertyViewModel() = ObjectCollection17.GetCollectionPropertyViewModel x.API 
    [<Test>]
    member x.GetCollectionPropertyFormalOnly() = ObjectCollection17.GetCollectionPropertyFormalOnly x.API 
    [<Test>]
    member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly x.API 
    [<Test>]
    member x.GetCollectionSetProperty() = ObjectCollection17.GetCollectionSetProperty x.API 
    [<Test>]
    member x.GetCollectionSetPropertyFormalOnly() = ObjectCollection17.GetCollectionSetPropertyFormalOnly x.API 
    [<Test>]
    member x.GetCollectionSetPropertySimpleOnly() = ObjectCollection17.GetCollectionSetPropertySimpleOnly x.API 
    [<Test>]
    member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType x.API 
    [<Test>]
    member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty x.API 
    [<Test>]
    member x.GetCollectionValue() = ObjectCollection17.GetCollectionValue x.API 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionProperty() = ObjectCollection17.AddToAndDeleteFromCollectionProperty x.API 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyViewModel() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyViewModel x.API 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionProperty() = ObjectCollection17.AddToAndDeleteFromSetCollectionProperty x.API 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyConcurrencySuccess x.API 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess x.API 
//    [<Test>]    
//    member x.AddToCollectionPropertyConcurrencyFail() = ObjectCollection17.AddToCollectionPropertyConcurrencyFail x.API 
//    [<Test>]    
//    member x.AddToCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.AddToCollectionPropertyMissingIfMatchHeader x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyConcurrencyFail() = ObjectCollection17.DeleteFromCollectionPropertyConcurrencyFail x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.DeleteFromCollectionPropertyMissingIfMatchHeader x.API 
//    [<Test>]    
//    member x.AddToCollectionPropertyValidateOnly() = ObjectCollection17.AddToCollectionPropertyValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyValidateOnly() = ObjectCollection17.DeleteFromCollectionPropertyValidateOnly x.API 
    [<Test>]
    member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection x.API 
    [<Test>]
    member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection x.API 
    [<Test>]
    member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection x.API  
    [<Test>]    
    member x.NotAcceptableGetCollectionWrongMediaType() = ObjectCollection17.NotAcceptableGetCollectionWrongMediaType x.API 
    [<Test>]
    member x.GetErrorValueCollection() = ObjectCollection17.GetErrorValueCollection x.API 
    [<Test>]
    member x.GetCollectionAsProperty() = ObjectCollection17.GetCollectionAsProperty x.API 
//    [<Test>]    
//    member x.AddToCollectionMissingArgs() = ObjectCollection17.AddToCollectionMissingArgs x.API 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgs() = ObjectCollection17.AddToCollectionMalformedArgs x.API 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgs() = ObjectCollection17.AddToCollectionInvalidArgs x.API 
//    [<Test>]    
//    member x.AddToCollectionDisabledValue() = ObjectCollection17.AddToCollectionDisabledValue x.API 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValue() = ObjectCollection17.AddToCollectionInvisibleValue x.API 
//    [<Test>]    
//    member x.AddToCollectionImmutableObject() = ObjectCollection17.AddToCollectionImmutableObject x.API 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsName() = ObjectCollection17.AddToCollectionInvalidArgsName x.API 
//    [<Test>]        
//    member x.NotAcceptableAddCollectionWrongMediaType() = ObjectCollection17.NotAcceptableAddCollectionWrongMediaType x.API 
//    [<Test>]    
//    member x.AddToCollectionMissingArgsValidateOnly() = ObjectCollection17.AddToCollectionMissingArgsValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgsValidateOnly() = ObjectCollection17.AddToCollectionMalformedArgsValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionDisabledValueValidateOnly() = ObjectCollection17.AddToCollectionDisabledValueValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValueValidateOnly() = ObjectCollection17.AddToCollectionInvisibleValueValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionImmutableObjectValidateOnly() = ObjectCollection17.AddToCollectionImmutableObjectValidateOnly x.API 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsNameValidateOnly x.API   
//    [<Test>]    
//    member x.AddToCollectionInternalError() = ObjectCollection17.AddToCollectionInternalError x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgs() = ObjectCollection17.DeleteFromCollectionMissingArgs x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgs() = ObjectCollection17.DeleteFromCollectionMalformedArgs x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgs() = ObjectCollection17.DeleteFromCollectionInvalidArgs x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValue() = ObjectCollection17.DeleteFromCollectionDisabledValue x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValue() = ObjectCollection17.DeleteFromCollectionInvisibleValue x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObject() = ObjectCollection17.DeleteFromCollectionImmutableObject x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsName() = ObjectCollection17.DeleteFromCollectionInvalidArgsName x.API 
//    [<Test>]        
//    member x.NotAcceptableDeleteFromCollectionWrongMediaType() = ObjectCollection17.NotAcceptableDeleteFromCollectionWrongMediaType x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMissingArgsValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMalformedArgsValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValueValidateOnly() = ObjectCollection17.DeleteFromCollectionDisabledValueValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValueValidateOnly() = ObjectCollection17.DeleteFromCollectionInvisibleValueValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObjectValidateOnly() = ObjectCollection17.DeleteFromCollectionImmutableObjectValidateOnly x.API 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsNameValidateOnly x.API  
//    [<Test>]    
//    member x.DeleteFromCollectionInternalError() = ObjectCollection17.DeleteFromCollectionInternalError x.API 
    // ObjectAction18
    [<Test>]
    member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject x.API
    [<Test>]
    member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService x.API  
    [<Test>]
    member x.GetActionContributedService() = ObjectAction18.GetActionContributedService x.API  
    [<Test>]
    member x.GetActionPropertyViewModel() = ObjectAction18.GetActionPropertyViewModel x.API  
    [<Test>]
    member x.GetOverloadedActionPropertyObject() = ObjectAction18.GetOverloadedActionPropertyObject x.API
    [<Test>]
    member x.GetOverloadedActionPropertyService() = ObjectAction18.GetOverloadedActionPropertyService x.API  
    [<Test>]
    member x.GetOverloadedActionPropertyViewModel() = ObjectAction18.GetOverloadedActionPropertyViewModel x.API  
    [<Test>]
    member x.GetActionPropertyDateTimeViewModel() = ObjectAction18.GetActionPropertyDateTimeViewModel x.API

    [<Test>]
    member x.GetActionPropertyCollectionViewModel() = ObjectAction18.GetActionPropertyCollectionViewModel x.API
    [<Test>]
    member x.GetActionPropertyCollectionService() = ObjectAction18.GetActionPropertyCollectionService x.API  
    [<Test>]
    member x.GetActionPropertyCollectionObject() = ObjectAction18.GetActionPropertyCollectionObject x.API 

    [<Test>]
    member x.GetActionPropertyDateTimeService() = ObjectAction18.GetActionPropertyDateTimeService x.API  
    [<Test>]
    member x.GetActionPropertyDateTimeObject() = ObjectAction18.GetActionPropertyDateTimeObject x.API  
    [<Test>]
    member x.GetUserDisabledActionPropertyObject() = ObjectAction18.GetUserDisabledActionPropertyObject x.API
    [<Test>]
    member x.GetUserDisabledActionPropertyService() = ObjectAction18.GetUserDisabledActionPropertyService x.API 
    [<Test>]
    member x.GetUserDisabledActionPropertyViewModel() = ObjectAction18.GetUserDisabledActionPropertyViewModel x.API 
    [<Test>]
    member x.GetActionPropertyQueryOnlyObject() = ObjectAction18.GetActionPropertyQueryOnlyObject x.API
    [<Test>]
    member x.GetActionPropertyQueryOnlyService() = ObjectAction18.GetActionPropertyQueryOnlyService x.API   
    [<Test>]
    member x.GetActionPropertyQueryOnlyViewModel() = ObjectAction18.GetActionPropertyQueryOnlyViewModel x.API   
    [<Test>]
    member x.GetActionPropertyIdempotentObject() = ObjectAction18.GetActionPropertyIdempotentObject x.API
    [<Test>]
    member x.GetActionPropertyIdempotentService() = ObjectAction18.GetActionPropertyIdempotentService x.API 
    [<Test>]
    member x.GetActionPropertyIdempotentViewModel() = ObjectAction18.GetActionPropertyIdempotentViewModel x.API 
    [<Test>]
    member x.GetActionPropertyWithOptObject() = ObjectAction18.GetActionPropertyWithOptObject x.API
    [<Test>]
    member x.GetActionPropertyWithOptService() = ObjectAction18.GetActionPropertyWithOptService x.API  
    [<Test>]
    member x.GetActionPropertyWithOptViewModel() = ObjectAction18.GetActionPropertyWithOptViewModel x.API  
    [<Test>]
    member x.GetActionPropertyWithOptObjectSimpleOnly() = ObjectAction18.GetActionPropertyWithOptObjectSimpleOnly x.API
    [<Test>]
    member x.GetActionPropertyWithOptServiceSimpleOnly() = ObjectAction18.GetActionPropertyWithOptServiceSimpleOnly x.API   
    [<Test>]
    member x.GetActionPropertyWithOptViewModelSimpleOnly() = ObjectAction18.GetActionPropertyWithOptViewModelSimpleOnly x.API   
    [<Test>]
    member x.GetActionPropertyWithOptObjectFormalOnly() = ObjectAction18.GetActionPropertyWithOptObjectFormalOnly x.API
    [<Test>]
    member x.GetActionPropertyWithOptServiceFormalOnly() = ObjectAction18.GetActionPropertyWithOptServiceFormalOnly x.API 
    [<Test>]
    member x.GetActionPropertyWithOptViewModelFormalOnly() = ObjectAction18.GetActionPropertyWithOptViewModelFormalOnly x.API 
    [<Test>]
    member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType x.API
    [<Test>]
    member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType x.API  
    [<Test>]
    member x.GetActionPropertyViewModelWithMediaType() = ObjectAction18.GetActionPropertyViewModelWithMediaType x.API  
    [<Test>]
    member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject x.API 
    [<Test>]
    member x.GetScalarActionService() = ObjectAction18.GetScalarActionService x.API 
    [<Test>]
    member x.GetScalarActionViewModel() = ObjectAction18.GetScalarActionViewModel x.API 
    [<Test>]
    member x.GetActionWithValueParmObject() = ObjectAction18.GetActionWithValueParmObject x.API
    [<Test>]
    member x.GetActionWithValueParmService() = ObjectAction18.GetActionWithValueParmService x.API 
    [<Test>]
    member x.GetActionWithValueParmViewModel() = ObjectAction18.GetActionWithValueParmViewModel x.API 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesObject() = ObjectAction18.GetActionWithValueParmWithChoicesObject x.API
    [<Test>]
    member x.GetActionWithValueParmWithChoicesService() = ObjectAction18.GetActionWithValueParmWithChoicesService x.API 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesViewModel() = ObjectAction18.GetActionWithValueParmWithChoicesViewModel x.API 
    [<Test>]
    member x.GetActionWithValueParmWithDefaultObject() = ObjectAction18.GetActionWithValueParmWithDefaultObject x.API
    [<Test>]
    member x.GetActionWithValueParmWithDefaultService() = ObjectAction18.GetActionWithValueParmWithDefaultService x.API    
    [<Test>]
    member x.GetActionWithValueParmWithDefaultViewModel() = ObjectAction18.GetActionWithValueParmWithDefaultViewModel x.API    
    [<Test>]
    member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService x.API   
    [<Test>]
    member x.GetActionWithReferenceParmViewModel() = ObjectAction18.GetActionWithReferenceParmViewModel x.API   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesService() = ObjectAction18.GetActionWithReferenceParmWithChoicesService x.API   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesViewModel() = ObjectAction18.GetActionWithReferenceParmWithChoicesViewModel x.API   

    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteObject() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteService() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteService x.API   
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteViewModel() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteViewModel x.API   


    [<Test>]
    member x.InvokeParmWithAutoCompleteObject() = ObjectAction18.InvokeParmWithAutoCompleteObject x.API
    [<Test>]
    member x.InvokeParmWithAutoCompleteService() = ObjectAction18.InvokeParmWithAutoCompleteService x.API   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModel() = ObjectAction18.InvokeParmWithAutoCompleteViewModel x.API 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorNoParm x.API
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorNoParm x.API   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorNoParm x.API 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorMalformedParm x.API
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorMalformedParm x.API   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorMalformedParm x.API 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm x.API
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm x.API   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm x.API 



    [<Test>]
    member x.InvokeParmWithConditionalChoicesObject() = ObjectAction18.InvokeParmWithConditionalChoicesObject x.API
    [<Test>]
    member x.InvokeParmWithConditionalChoicesService() = ObjectAction18.InvokeParmWithConditionalChoicesService x.API   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeParmWithConditionalChoicesViewModel x.API 


    [<Test>]
    member x.InvokeParmWithConditionalChoicesObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesObjectErrorMalformedParm x.API
    [<Test>]
    member x.InvokeParmWithConditionalChoicesServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesServiceErrorMalformedParm x.API   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm x.API 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm x.API
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm x.API   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm x.API 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObject() = ObjectAction18.InvokeValueParmWithConditionalChoicesObject x.API
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesService() = ObjectAction18.InvokeValueParmWithConditionalChoicesService x.API   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModel x.API 


    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultService() = ObjectAction18.GetActionWithReferenceParmWithDefaultService x.API  
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultViewModel() = ObjectAction18.GetActionWithReferenceParmWithDefaultViewModel x.API  
    [<Test>]
    member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject x.API
    [<Test>]
    member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService x.API   
    [<Test>]
    member x.GetActionWithChoicesAndDefaultViewModel() = ObjectAction18.GetActionWithChoicesAndDefaultViewModel x.API   
    [<Test>]
    member x.GetContributedActionOnContributee() = ObjectAction18.GetContributedActionOnContributee x.API 
    [<Test>]
    member x.GetContributedActionOnContributeeOnBaseClass() = ObjectAction18.GetContributedActionOnContributeeBaseClass x.API 
    [<Test>]
    member x.GetContributedActionOnContributeeWithRef() = ObjectAction18.GetContributedActionOnContributeeWithRef x.API 
    [<Test>]
    member x.GetContributedActionOnContributeeWithValue() = ObjectAction18.GetContributedActionOnContributeeWithValue x.API 
    [<Test>]
    member x.GetContributedActionOnContributer() = ObjectAction18.GetContributedActionOnContributer x.API 
    [<Test>]
    member x.GetContributedActionOnContributerOnBaseClass() = ObjectAction18.GetContributedActionOnContributerBaseClass x.API 
    [<Test>]
    member x.GetContributedActionOnContributerWithRef() = ObjectAction18.GetContributedActionOnContributerWithRef x.API 
    [<Test>]
    member x.GetContributedActionOnContributerWithValue() = ObjectAction18.GetContributedActionOnContributerWithValue x.API 
    [<Test>]
    member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject x.API
    [<Test>]
    member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService x.API   
    [<Test>]
    member x.GetInvalidActionPropertyViewModel() = ObjectAction18.GetInvalidActionPropertyViewModel x.API   
    [<Test>]
    member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject x.API
    [<Test>]
    member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService x.API 
    [<Test>]
    member x.GetNotFoundActionPropertyViewModel() = ObjectAction18.GetNotFoundActionPropertyViewModel x.API 
    [<Test>]
    member x.GetUserDisabledActionObject() = ObjectAction18.GetUserDisabledActionObject x.API 
    [<Test>]
    member x.GetUserDisabledActionService() = ObjectAction18.GetUserDisabledActionService x.API  
    [<Test>]
    member x.GetUserDisabledActionViewModel() = ObjectAction18.GetUserDisabledActionViewModel x.API  
    [<Test>]
    member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject x.API
    [<Test>]
    member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService x.API   
    [<Test>]
    member x.GetHiddenActionPropertyViewModel() = ObjectAction18.GetHiddenActionPropertyViewModel x.API   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject x.API   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeService() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeService x.API 
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeViewModel() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeViewModel x.API 
    [<Test>]
    member x.GetQueryActionObject() = ObjectAction18.GetQueryActionObject x.API
    [<Test>]
    member x.GetQueryActionService() = ObjectAction18.GetQueryActionService x.API  
    [<Test>]
    member x.GetQueryActionViewModel() = ObjectAction18.GetQueryActionViewModel x.API  
    [<Test>]
    member x.GetQueryActionWithParmsObject() = ObjectAction18.GetQueryActionWithParmsObject x.API
    [<Test>]
    member x.GetQueryActionWithParmsService() = ObjectAction18.GetQueryActionWithParmsService x.API
    [<Test>]
    member x.GetQueryActionWithParmsViewModel() = ObjectAction18.GetQueryActionWithParmsViewModel x.API
    [<Test>]
    member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject x.API 
    [<Test>]
    member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService x.API 
    [<Test>]
    member x.GetCollectionActionViewModel() = ObjectAction18.GetCollectionActionViewModel x.API 
    [<Test>]
    member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject x.API 
    [<Test>]
    member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService x.API  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModel() = ObjectAction18.GetCollectionActionWithParmsViewModel x.API  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsObjectFormalOnly x.API 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceFormalOnly x.API  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelFormalOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelFormalOnly x.API  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly x.API 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly x.API
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelSimpleOnly x.API

    [<Test>]
    member x.ActionNotFound() = ObjectAction18.ActionNotFound x.API     
    // ObjectActionInvoke19
    [<Test>]  
    member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService x.API  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModel x.API
   
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectService() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectService x.API  
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectViewModel x.API

    [<Test>]
    member x.PostInvokeActionContributedService() = ObjectActionInvoke19.PostInvokeActionContributedService x.API  

    [<Test>]
    member x.PostInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnViewModelObject x.API
    [<Test>]
    member x.PostInvokeActionReturnViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnViewModelService x.API  
    [<Test>]
    member x.PostInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnViewModelViewModel x.API

    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectObject x.API
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectService() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectService x.API  
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectViewModel x.API


    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectService() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectService x.API   
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectViewModel x.API   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectService x.API   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectViewModel x.API   

    [<Test>]
    member x.PostInvokeActionReturnNullViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelService x.API   
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelViewModel x.API   


    [<Test>]
    member x.PostInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly x.API  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelValidateOnly x.API  

    [<Test>]
    member x.PutInvokeActionReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnObjectObject x.API
    [<Test>]
    member x.PutInvokeActionReturnObjectService() = ObjectActionInvoke19.PutInvokeActionReturnObjectService x.API   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModel x.API   

    [<Test>]
    member x.PutInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PutInvokeActionReturnViewModelObject x.API
    [<Test>]
    member x.PutInvokeActionReturnViewModelService() = ObjectActionInvoke19.PutInvokeActionReturnViewModelService x.API   
    [<Test>]
    member x.PutInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PutInvokeActionReturnViewModelViewModel x.API   


    [<Test>]
    member x.PutInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectObject x.API
    [<Test>]
    member x.PutInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectService x.API  
    [<Test>]
    member x.PutInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectViewModel x.API  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceValidateOnly x.API  
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelValidateOnly x.API  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess x.API
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess x.API   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess x.API   

    [<Test>]
    member x.GetInvokeActionReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnObjectObject x.API
    [<Test>]
    member x.GetInvokeActionReturnObjectService() = ObjectActionInvoke19.GetInvokeActionReturnObjectService x.API    
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModel x.API    

    [<Test>]
    member x.GetInvokeActionReturnViewModelObject() = ObjectActionInvoke19.GetInvokeActionReturnViewModelObject x.API
    [<Test>]
    member x.GetInvokeActionReturnViewModelService() = ObjectActionInvoke19.GetInvokeActionReturnViewModelService x.API    
    [<Test>]
    member x.GetInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.GetInvokeActionReturnViewModelViewModel x.API    



    [<Test>]
    member x.GetInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectObject x.API
    [<Test>]
    member x.GetInvokeActionReturnNullObjectService() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectService x.API   
    [<Test>]
    member x.GetInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectViewModel x.API   

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceValidateOnly x.API   
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelValidateOnly x.API   

    [<Test>]
    member x.PostInvokeContribActionReturnObject() = ObjectActionInvoke19.PostInvokeContribActionReturnObject x.API
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithRefParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithRefParm x.API
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithValueParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithValueParm x.API
    [<Test>]
    member x.PostInvokeContribActionReturnObjectBaseClass() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectBaseClass x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType x.API  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelWithMediaType x.API  
       
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess x.API  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess x.API  

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch x.API
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail x.API
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch x.API
    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess x.API
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess x.API 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess x.API 

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch x.API
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch x.API 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch x.API 
      
    [<Test>]
    member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService x.API   
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModel x.API   

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectFormalOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceFormalOnly x.API  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelFormalOnly x.API  

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly x.API  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelValidateOnly x.API  

    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService x.API  
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarViewModel x.API  

    [<Test>]
    member x.PostInvokeActionReturnNullScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullScalarService() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarService x.API  
    [<Test>]
    member x.PostInvokeActionReturnNullScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarViewModel x.API  
    [<Test>]
    member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject x.API
    [<Test>]
    member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService x.API    
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModel x.API    

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectFormalOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceFormalOnly x.API   
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelFormalOnly x.API   

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly x.API  
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelValidateOnly x.API  

    [<Test>]
    member x.GetInvokeActionReturnQueryObject() = ObjectActionInvoke19.GetInvokeActionReturnQueryObject x.API
    [<Test>]
    member x.GetInvokeActionReturnQueryService() = ObjectActionInvoke19.GetInvokeActionReturnQueryService x.API    
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModel x.API    

    [<Test>]
    member x.GetInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryServiceValidateOnly x.API   
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelValidateOnly x.API   

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService x.API    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModel x.API    

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly x.API     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly x.API     


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService x.API  
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModel x.API  


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly x.API       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly x.API       


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService x.API   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModel x.API   


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.API    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.API    


    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObject x.API
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectService x.API    
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModel x.API    
    
      
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.API   
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.API   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObject x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectService x.API   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModel x.API   


    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectObject x.API
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectService x.API  
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectViewModel x.API  
    
     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.API     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly x.API     
    
      
    [<Test>]
    member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject x.API
    [<Test>]
    member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService x.API  
    [<Test>]
    member x.NotFoundActionInvokeViewModel() = ObjectActionInvoke19.NotFoundActionInvokeViewModel x.API  


    [<Test>]
    member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject x.API
    [<Test>]
    member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService x.API   
    [<Test>]
    member x.HiddenActionInvokeViewModel() = ObjectActionInvoke19.HiddenActionInvokeViewModel x.API   


    [<Test>]
    member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject x.API
    [<Test>]
    member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService x.API   
    [<Test>]
    member x.GetActionWithSideEffectsViewModel() = ObjectActionInvoke19.GetActionWithSideEffectsViewModel x.API   


    [<Test>]
    member x.GetActionWithIdempotentObject() = ObjectActionInvoke19.GetActionWithIdempotentObject x.API
    [<Test>]
    member x.GetActionWithIdempotentService() = ObjectActionInvoke19.GetActionWithIdempotentService x.API  
    [<Test>]
    member x.GetActionWithIdempotentViewModel() = ObjectActionInvoke19.GetActionWithIdempotentViewModel x.API  


    [<Test>]
    member x.PutActionWithQueryOnlyObject() = ObjectActionInvoke19.PutActionWithQueryOnlyObject x.API
    [<Test>]
    member x.PutActionWithQueryOnlyService() = ObjectActionInvoke19.PutActionWithQueryOnlyService x.API  
    [<Test>]
    member x.PutActionWithQueryOnlyViewModel() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModel x.API  


    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeObject x.API
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeService x.API   
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeViewModel x.API   


    [<Test>]
    member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject x.API
    [<Test>]
    member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService x.API   
    [<Test>]
    member x.MissingParmsOnPostViewModel() = ObjectActionInvoke19.MissingParmsOnPostViewModel x.API   


    [<Test>]
    member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject x.API 
    [<Test>]
    member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService x.API 
    [<Test>]
    member x.DisabledActionPostInvokeViewModel() = ObjectActionInvoke19.DisabledActionPostInvokeViewModel x.API 


    [<Test>]
    member x.UserDisabledActionPostInvokeObject() = ObjectActionInvoke19.UserDisabledActionPostInvokeObject x.API 
    [<Test>]
    member x.UserDisabledActionPostInvokeService() = ObjectActionInvoke19.UserDisabledActionPostInvokeService x.API 
    [<Test>]
    member x.UserDisabledActionPostInvokeViewModel() = ObjectActionInvoke19.UserDisabledActionPostInvokeViewModel x.API 


    [<Test>]
    member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject x.API
    [<Test>]
    member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService x.API  
    [<Test>]
    member x.NotFoundActionPostInvokeViewModel() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModel x.API  


    [<Test>]
    member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject x.API
    [<Test>]
    member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService x.API   
    [<Test>]
    member x.HiddenActionPostInvokeViewModel() = ObjectActionInvoke19.HiddenActionPostInvokeViewModel x.API   


    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject x.API
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService x.API   
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeViewModel x.API   


    [<Test>]
    member x.PostQueryActionWithErrorObject() = ObjectActionInvoke19.PostQueryActionWithErrorObject x.API
    [<Test>]
    member x.PostQueryActionWithErrorService() = ObjectActionInvoke19.PostQueryActionWithErrorService x.API    
    [<Test>]
    member x.PostQueryActionWithErrorViewModel() = ObjectActionInvoke19.PostQueryActionWithErrorViewModel x.API    


    [<Test>]
    member x.GetQueryActionWithErrorObject() = ObjectActionInvoke19.GetQueryActionWithErrorObject x.API
    [<Test>]
    member x.GetQueryActionWithErrorService() = ObjectActionInvoke19.GetQueryActionWithErrorService x.API 
    [<Test>]
    member x.GetQueryActionWithErrorViewModel() = ObjectActionInvoke19.GetQueryActionWithErrorViewModel x.API 


    [<Test>]
    member x.MalformedFormalParmsOnPostQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject x.API
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService x.API 
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModel x.API 


    
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryObject() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryObject x.API
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryService() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryService x.API 
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryViewModel x.API 

     
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryObject() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryObject x.API
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryService() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryService x.API 
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryViewModel x.API 

    [<Test>]
    member x.InvalidFormalParmsOnPostQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObject x.API
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryService x.API   
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModel x.API   


    [<Test>]
    member x.MissingParmsOnGetQueryObject() = ObjectActionInvoke19.MissingParmsOnGetQueryObject x.API
    [<Test>]
    member x.MissingParmsOnGetQueryService() = ObjectActionInvoke19.MissingParmsOnGetQueryService x.API   
    [<Test>]
    member x.MissingParmsOnGetQueryViewModel() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModel x.API   


    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObject x.API
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryService() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryService x.API   
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModel x.API   


    [<Test>]
    member x.MalformedFormalParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObject x.API
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryService x.API   
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModel x.API   


    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObject x.API
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryService() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryService x.API    
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModel x.API    


    [<Test>]
    member x.InvalidFormalParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObject x.API
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryService x.API  
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModel x.API  


    [<Test>]
    member x.DisabledActionInvokeQueryObject() = ObjectActionInvoke19.DisabledActionInvokeQueryObject x.API
    [<Test>]
    member x.DisabledActionInvokeQueryService() = ObjectActionInvoke19.DisabledActionInvokeQueryService x.API    
    [<Test>]
    member x.DisabledActionInvokeQueryViewModel() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModel x.API    


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleService x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel x.API   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleService x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleViewModel x.API   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalService x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalViewModel x.API   

    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly x.API        
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly x.API        
    
     
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalService x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel x.API   


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly x.API   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalService x.API  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel x.API  


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly x.API  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly x.API  
    
          
    [<Test>]
    member x.PostInvokeActionReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionReturnQueryObject x.API
    [<Test>]
    member x.PostInvokeActionReturnQueryService() = ObjectActionInvoke19.PostInvokeActionReturnQueryService x.API  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModel x.API  


    [<Test>]
    member x.PostInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryServiceValidateOnly x.API  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModelValidateOnly x.API  


    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObject x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryService x.API   
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModel x.API   

    [<Test>]
    member x.PostInvokeOverloadedActionObject() = ObjectActionInvoke19.PostInvokeOverloadedActionObject x.API
    [<Test>]
    member x.PostInvokeOverloadedActionService() = ObjectActionInvoke19.PostInvokeOverloadedActionService x.API   
    [<Test>]
    member x.PostInvokeOverloadedActionViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionViewModel x.API  

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly x.API  
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly x.API  

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryService x.API   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModel x.API   
       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly x.API     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly x.API     
            
    [<Test>]
    member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject x.API
    [<Test>]
    member x.PostCollectionActionWithErrorService() = ObjectActionInvoke19.PostCollectionActionWithErrorService x.API  
    [<Test>]
    member x.PostCollectionActionWithErrorViewModel() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModel x.API  

    [<Test>]
    member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject x.API
    [<Test>]
    member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService x.API    
    [<Test>]
    member x.MissingParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModel x.API    

    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject x.API
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionService() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService x.API  
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModel x.API  

    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject x.API
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionService() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService x.API    
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModel x.API    

    [<Test>]
    member x.DisabledActionInvokeCollectionObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionObject x.API 
    [<Test>]
    member x.DisabledActionInvokeCollectionService() = ObjectActionInvoke19.DisabledActionInvokeCollectionService x.API 
    [<Test>]
    member x.DisabledActionInvokeCollectionViewModel() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModel x.API 

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalService x.API    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel x.API    

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.API       
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly x.API       
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalService x.API      
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel x.API      
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.API   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly x.API   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService x.API   
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel x.API   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectFormalOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceFormalOnly x.API    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelFormalOnly x.API    
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly x.API    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly x.API    
    
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService x.API   
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel x.API   
    
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService x.API   
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel x.API   
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService x.API     
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel x.API     
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly x.API             
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly x.API             
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService x.API   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel x.API   
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly x.API           
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly x.API           
    
    [<Test>]
    member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject x.API
    [<Test>]
    member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService x.API   
    [<Test>]
    member x.GetInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel x.API   
    
    [<Test>]
    member x.PostQueryActionWithValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithValidateFailObject x.API
    [<Test>]
    member x.PostQueryActionWithValidateFailService() = ObjectActionInvoke19.PostQueryActionWithValidateFailService x.API   
    [<Test>]
    member x.PostQueryActionWithValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel x.API   

    [<Test>]
    member x.PostQueryActionWithCrossValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject x.API
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailService() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService x.API   
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel x.API 


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService x.API    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel x.API    
    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService x.API   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel x.API   
    
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService x.API   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel x.API   
    
    [<Test>]
    member x.MissingParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.MissingParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly x.API   
    [<Test>] 
    member x.MissingParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly x.API   
    
    [<Test>] 
    member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly x.API    
    [<Test>] 
    member x.MissingParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly x.API    

    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly x.API    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly x.API   
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly x.API   
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly x.API    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly x.API         
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly x.API         
    
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly x.API  
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly x.API      
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly x.API      
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly x.API     
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly x.API     
    
    [<Test>] 
    member x.DisabledActionInvokeQueryObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.DisabledActionInvokeQueryServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly x.API    
    [<Test>] 
    member x.DisabledActionInvokeQueryViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly x.API 
    [<Test>] 
    member x.DisabledActionInvokeCollectionViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly x.API 
    
    [<Test>] 
    member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly x.API      
    [<Test>] 
    member x.NotFoundActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly x.API      
    
    [<Test>] 
    member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly x.API    
    [<Test>] 
    member x.HiddenActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly x.API 
    [<Test>] 
    member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly x.API   
    [<Test>] 
    member x.GetActionWithSideEffectsViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly x.API   
    
    [<Test>] 
    member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly x.API 
    [<Test>] 
    member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly x.API    
    [<Test>] 
    member x.GetActionWithIdempotentViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly x.API 
    [<Test>] 
    member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly x.API    
    [<Test>] 
    member x.PutActionWithQueryOnlyViewModelValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.GetQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly x.API 
    [<Test>] 
    member x.GetQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly x.API     
    [<Test>] 
    member x.GetQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly x.API     
    
    [<Test>] 
    member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly x.API 
    [<Test>] 
    member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly x.API    
    [<Test>] 
    member x.PostCollectionActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly x.API 
    [<Test>] 
    member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly x.API     
    [<Test>] 
    member x.MissingParmsOnPostViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly x.API     
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly x.API     
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly x.API     
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly x.API       
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly x.API       
    
    [<Test>] 
    member x.InvalidUrlOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.InvalidUrlOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly x.API   
    [<Test>] 
    member x.InvalidUrlOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly x.API   
    
    
    [<Test>] 
    member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly x.API  
    [<Test>] 
    member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly x.API  
    [<Test>] 
    member x.DisabledActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly x.API  
    
    [<Test>] 
    member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly x.API    
    [<Test>] 
    member x.NotFoundActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly x.API    
    [<Test>] 
    member x.HiddenActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly x.API 
    [<Test>] 
    member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly x.API     
    [<Test>] 
    member x.PostQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly x.API     
    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly x.API    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly x.API    
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly x.API  
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly x.API  
    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.API    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly x.API    
    

    // DomainType21
    [<Test>] 
    member x.GetMostSimpleObjectType() = DomainType21.GetMostSimpleObjectType x.API
    [<Test>] 
    member x.GetWithActionObjectType() = DomainType21.GetWithActionObjectType x.API
    [<Test>]
    member x.GetWithActionServiceType() = DomainType21.GetWithActionServiceType x.API
    [<Test>] 
    member x.GetWithReferenceObjectType() = DomainType21.GetWithReferenceObjectType x.API
    [<Test>] 
    member x.GetWithValueObjectType() = DomainType21.GetWithValueObjectType x.API
    [<Test>] 
    member x.GetWithCollectionObjectType() = DomainType21.GetWithCollectionObjectType x.API
    [<Test>] 
    member x.GetPredefinedDomainTypes() = DomainType21.GetPredefinedDomainTypes x.API
    [<Test>] 
    member x.NotFoundPredefinedDomainTypes() = DomainType21.NotFoundPredefinedDomainTypes x.API
    [<Test>] 
    member x.NotFoundGetMostSimpleObjectType() = DomainType21.NotFoundGetMostSimpleObjectType x.API
    [<Test>] 
    member x.NotAcceptableGetMostSimpleObjectType() = DomainType21.NotAcceptableGetMostSimpleObjectType x.API
    // DomainProperty22
    [<Test>] 
    member x.GetValuePropertyType() = DomainProperty22.GetValuePropertyType x.API
    [<Test>] 
    member x.GetReferencePropertyType() = DomainProperty22.GetReferencePropertyType x.API
    [<Test>] 
    member x.GetValueStringPropertyType() = DomainProperty22.GetValueStringPropertyType x.API
    [<Test>] 
    member x.GetValueDateTimePropertyType() = DomainProperty22.GetValueDateTimePropertyType x.API
    [<Test>] 
    member x.NotFoundPropertyType() = DomainProperty22.NotFoundPropertyType x.API
    [<Test>] 
    member x.NotFoundTypePropertyType() = DomainProperty22.NotFoundTypePropertyType x.API
    [<Test>] 
    member x.NotAcceptableGetValuePropertyType() = DomainProperty22.NotAcceptableGetValuePropertyType x.API
    // DomainCollection23
    [<Test>] 
    member x.GetCollectionPropertyType() = DomainCollection23.GetCollectionPropertyType x.API
    [<Test>] 
    member x.GetSetCollectionPropertyType() = DomainCollection23.GetSetCollectionPropertyType x.API
    [<Test>] 
    member x.GetCollectionPropertyTypeWithDescription() = DomainCollection23.GetCollectionPropertyTypeWithDescription x.API
    [<Test>] 
    member x.NotFoundTypeCollectionPropertyType() = DomainCollection23.NotFoundTypeCollectionPropertyType x.API
    [<Test>] 
    member x.NotFoundCollectionPropertyType() = DomainCollection23.NotFoundCollectionPropertyType x.API
    [<Test>] 
    member x.NotAcceptableGetCollectionPropertyType() = DomainCollection23.NotAcceptableGetCollectionPropertyType x.API
    // DomainAction24
    [<Test>] 
    member x.GetActionTypeObjectNoParmsScalar() = DomainAction24.GetActionTypeObjectNoParmsScalar x.API
    [<Test>] 
    member x.GetActionTypeServiceNoParmsScalar() = DomainAction24.GetActionTypeServiceNoParmsScalar x.API

    [<Test>] 
    member x.GetOverloadedActionTypeObjectNoParmsScalar() = DomainAction24.GetOverloadedActionTypeObjectNoParmsScalar x.API
    [<Test>] 
    member x.GetOverloadedActionTypeServiceNoParmsScalar() = DomainAction24.GetOverloadedActionTypeServiceNoParmsScalar x.API


    [<Test>] 
    member x.GetActionTypeObjectNoParmsVoid() = DomainAction24.GetActionTypeObjectNoParmsVoid x.API
    [<Test>] 
    member x.GetActionTypeServiceNoParmsVoid() = DomainAction24.GetActionTypeServiceNoParmsVoid x.API
    [<Test>] 
    member x.GetActionTypeObjectNoParmsCollection() = DomainAction24.GetActionTypeObjectNoParmsCollection x.API
    [<Test>] 
    member x.GetActionTypeServiceNoParmsCollection() = DomainAction24.GetActionTypeServiceNoParmsCollection x.API
    [<Test>] 
    member x.GetActionTypeObjectParmsScalar() = DomainAction24.GetActionTypeObjectParmsScalar x.API
    [<Test>] 
    member x.GetActionTypeServiceParmsScalar() = DomainAction24.GetActionTypeServiceParmsScalar x.API
    [<Test>] 
    member x.GetActionTypeObjectParmsVoid() = DomainAction24.GetActionTypeObjectParmsVoid x.API
    [<Test>] 
    member x.GetActionTypeServiceParmsVoid() = DomainAction24.GetActionTypeServiceParmsVoid x.API
    [<Test>] 
    member x.GetActionTypeObjectParmsCollection() = DomainAction24.GetActionTypeObjectParmsCollection x.API
    [<Test>] 
    member x.GetActionTypeServiceParmsCollection() = DomainAction24.GetActionTypeServiceParmsCollection x.API
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributee() = DomainAction24.GetActionTypeObjectContributedOnContributee x.API
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributer() = DomainAction24.GetActionTypeObjectContributedOnContributer x.API
    [<Test>] 
    member x.NotFoundTypeActionType() = DomainAction24.NotFoundTypeActionType x.API
    [<Test>] 
    member x.NotFoundActionTypeObject() = DomainAction24.NotFoundActionTypeObject x.API
    [<Test>] 
    member x.NotFoundActionTypeService() = DomainAction24.NotFoundActionTypeService x.API
    [<Test>] 
    member x.NotAcceptableActionType() = DomainAction24.NotAcceptableActionType x.API
    // DomainActionParameter25
    [<Test>] 
    member x.GetActionParameterTypeInt() = DomainActionParameter25.GetActionParameterTypeInt x.API
    [<Test>] 
    member x.GetActionParameterTypeString() = DomainActionParameter25.GetActionParameterTypeString x.API
    [<Test>] 
    member x.GetOverloadedActionParameterTypeString() = DomainActionParameter25.GetOverloadedActionParameterTypeString x.API
    [<Test>] 
    member x.GetActionParameterTypeDateTime() = DomainActionParameter25.GetActionParameterTypeDateTime x.API
    [<Test>] 
    member x.GetActionParameterTypeReference() = DomainActionParameter25.GetActionParameterTypeReference x.API
    [<Test>] 
    member x.GetActionParameterTypeStringOptional() = DomainActionParameter25.GetActionParameterTypeStringOptional x.API
    [<Test>] 
    member x.NotFoundType() = DomainActionParameter25.NotFoundType x.API
    [<Test>] 
    member x.NotFoundAction() = DomainActionParameter25.NotFoundAction x.API
    [<Test>] 
    member x.NotFoundParm() = DomainActionParameter25.NotFoundParm x.API
    [<Test>] 
    member x.NotAcceptableActionParameterType() = DomainActionParameter25.NotAcceptableActionParameterType x.API    
    // DomainTypeActionInvoke26
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms x.API
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms x.API
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms x.API
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms x.API
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms x.API
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms x.API
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms x.API
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms x.API
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms x.API
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms x.API
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms x.API
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms x.API
    [<Test>] 
    member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms x.API
    [<Test>] 
    member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms x.API
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms x.API
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms x.API
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms x.API
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms x.API
    [<Test>] 
    member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf x.API
    [<Test>] 
    member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf x.API
    [<Test>] 
    member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf x.API
    [<Test>] 
    member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf x.API
end
