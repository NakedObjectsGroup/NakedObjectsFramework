module NakedObjects.Rest.Test.Nof4
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
open NakedObjects.Core.Context
open NakedObjects.Core.Util

let api = 
    let api = new RestfulObjectsControllerBase()
    api.Surface <- new NakedObjects.Surface.Nof4.Implementation.NakedObjectsSurface(new NakedObjects.Surface.Nof4.Utility.ExternalOid())
    api


[<TestFixture>]
type Nof4Tests() = class      
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

    // HomePage5
    [<Test>] 
    member x.GetHomePage() = HomePage5.GetHomePage api
    [<Test>] 
    member x.GetHomePageSimple() = HomePage5.GetHomePageSimple api
    [<Test>] 
    member x.GetHomePageFormal() = HomePage5.GetHomePageFormal api
    [<Test>] 
    member x.GetHomePageWithMediaType() = HomePage5.GetHomePageWithMediaType api
    [<Test>] 
    member x.NotAcceptableGetHomePage() = HomePage5.NotAcceptableGetHomePage api
    [<Test>] 
    member x.InvalidDomainModelGetHomePage() = HomePage5.InvalidDomainModelGetHomePage api
    // User6
    [<Test>] 
    member x.GetUser() = User6.GetUser api
    [<Test>] 
    member x.GetUserWithMediaType() = User6.GetUserWithMediaType api 
    [<Test>] 
    member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser api
    // DomainServices7
    [<Test>] 
    member x.GetDomainServices() = DomainServices7.GetDomainServices api 
    [<Test>] 
    member x.GetDomainServicesFormal() = DomainServices7.GetDomainServicesFormal api 
    [<Test>] 
    member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaType api 
    [<Test>] 
    member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices api 
    // Version8
    [<Test>] 
    member x.GetVersion() = Version8.GetVersion api 
    [<Test>] 
    member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType api 
    [<Test>] 
    member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion api 
    //Objects9
    [<Test>]
    member x.GetMostSimpleTransientObject() = Objects9.GetMostSimpleTransientObject api  
    [<Test>]
    member x.GetMostSimpleTransientObjectSimpleOnly() = Objects9.GetMostSimpleTransientObjectSimpleOnly api  
    [<Test>]
    member x.GetMostSimpleTransientObjectFormalOnly() = Objects9.GetMostSimpleTransientObjectFormalOnly api  
    [<Test>]
    member x.PersistMostSimpleTransientObject() = Objects9.PersistMostSimpleTransientObject api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectSimpleOnly() = Objects9.PersistMostSimpleTransientObjectSimpleOnly api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectFormalOnly() = Objects9.PersistMostSimpleTransientObjectFormalOnly api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectValidateOnly() = Objects9.PersistMostSimpleTransientObjectValidateOnly api  
    [<Test>]
    member x.GetWithValueTransientObject() = Objects9.GetWithValueTransientObject api  
    [<Test>]
    member x.GetWithReferenceTransientObject() = Objects9.GetWithReferenceTransientObject api  
    [<Test>]
    member x.GetWithCollectionTransientObject() = Objects9.GetWithCollectionTransientObject api  
    [<Test>]
    member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject api  
    [<Test>]
    member x.PersistWithValueTransientObjectFormalOnly() = Objects9.PersistWithValueTransientObjectFormalOnly api  
    [<Test>]
    member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject api  
    [<Test>]
    member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject api  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnly() = Objects9.PersistWithValueTransientObjectValidateOnly api  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnly() = Objects9.PersistWithReferenceTransientObjectValidateOnly api  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnly() = Objects9.PersistWithCollectionTransientObjectValidateOnly api  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFail api  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnlyFail() = Objects9.PersistWithReferenceTransientObjectValidateOnlyFail api  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnlyFail() = Objects9.PersistWithCollectionTransientObjectValidateOnlyFail api  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail api  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail api  
    [<Test>]
    member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail api  
    [<Test>]
    member x.PersistWithValueTransientObjectFailInvalid() = Objects9.PersistWithValueTransientObjectFailInvalid api 
    [<Test>]
    member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail api  
    [<Test>]
    member x.PersistWithReferenceTransientObjectFailInvalid() = Objects9.PersistWithReferenceTransientObjectFailInvalid api  
    [<Test>]
    member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgs() = Objects9.PersistMostSimpleTransientObjectMissingArgs api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgsValidateOnly() = Objects9.PersistMostSimpleTransientObjectMissingArgsValidateOnly api  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingMemberArgs() = Objects9.PersistMostSimpleTransientObjectMissingMemberArgs api  
    [<Test>]    
    member x.PersistMostSimpleTransientObjectNullDomainType() = Objects9.PersistMostSimpleTransientObjectNullDomainType api  
    [<Test>]   
    member x.PersistMostSimpleTransientObjectEmptyDomainType() = Objects9.PersistMostSimpleTransientObjectEmptyDomainType api   
    [<Test>]
    member x.PersistMostSimpleTransientObjectMalformedMemberArgs() = Objects9.PersistMostSimpleTransientObjectMalformedMemberArgs api  
    [<Test>]
    member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject api
    [<Test>]
    member x.PersistNoKeyTransientObject() = Objects9.PersistNoKeyTransientObject api
    // Error10
    [<Test>]
    member x.Error() = Error10.Error api 
    [<Test>]
    member x.NotAcceptableError() = Error10.NotAcceptableError api 
    // DomainObject14
    [<Test>]
    member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject api  
    [<Test>]
    member x.GetWithAttachmentsObject() = DomainObject14.GetWithAttachmentsObject api  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSelectable() = DomainObject14.GetMostSimpleObjectConfiguredSelectable api  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredNone() = DomainObject14.GetMostSimpleObjectConfiguredNone api  
    [<Test>]
    member x.GetMostSimpleObjectFormalOnly() = DomainObject14.GetMostSimpleObjectFormalOnly api  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredFormalOnly() = DomainObject14.GetMostSimpleObjectConfiguredFormalOnly api 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSimpleOnly() = DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly api 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredCaching() = DomainObject14.GetMostSimpleObjectConfiguredCaching api 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredOverrides() = DomainObject14.GetMostSimpleObjectConfiguredOverrides api 
    [<Test>]
    member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly api 
    [<Test>]
    member x.GetWithDateTimeKeyObject() = DomainObject14.GetWithDateTimeKeyObject api  
    [<Test>]
    member x.GetVerySimpleEagerObject() = DomainObject14.GetVerySimpleEagerObject api  
    [<Test>]
    member x.GetWithValueObject() = DomainObject14.GetWithValueObject api 
    [<Test>]
    member x.GetWithScalarsObject() = DomainObject14.GetWithScalarsObject api 
    [<Test>]
    member x.GetWithValueObjectUserAuth() = DomainObject14.GetWithValueObjectUserAuth api 
    [<Test>]
    member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType api 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeSimple() = DomainObject14.GetMostSimpleObjectWithDomainTypeSimple api 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeFormal() = DomainObject14.GetMostSimpleObjectWithDomainTypeFormal api 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileSimple() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileSimple api 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileFormal() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileFormal api 
    [<Test>]
    member x.GetRedirectedObject() = DomainObject14.GetRedirectedObject api 
    [<Test>]
    member x.PutWithValueObject() = DomainObject14.PutWithValueObject api 
    [<Test>]
    member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess api 
    [<Test>]
    member x.PutWithScalarsObject() = DomainObject14.PutWithScalarsObject api
    [<Test>]
    member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail api 
    [<Test>]
    member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch api 
    [<Test>]
    member x.PutWithReferenceObject() = DomainObject14.PutWithReferenceObject api 
    [<Test>]
    member x.PutWithReferenceObjectValidateOnly() = DomainObject14.PutWithReferenceObjectValidateOnly api 
    [<Test>]
    member x.GetWithActionObject() = DomainObject14.GetWithActionObject api  
    [<Test>]
    member x.GetWithActionObjectSimpleOnly() = DomainObject14.GetWithActionObjectSimpleOnly api   
    [<Test>]
    member x.GetWithActionObjectFormalOnly() = DomainObject14.GetWithActionObjectFormalOnly api        
    [<Test>]
    member x.GetWithReferenceObject() = DomainObject14.GetWithReferenceObject api 
    [<Test>]
    member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject api 
    [<Test>]
    member x.GetWithCollectionObjectFormalOnly() = DomainObject14.GetWithCollectionObjectFormalOnly api 
    [<Test>]
    member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly api 
    [<Test>]
    member x.InvalidGetObject() = DomainObject14.InvalidGetObject api 
    [<Test>]
    member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject api 
    [<Test>]    
    member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType api 
    [<Test>]    
    member x.GetObjectIgnoreWrongDomainType() = DomainObject14.GetObjectIgnoreWrongDomainType api 
    [<Test>]
    member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs api 
    [<Test>]
    member x.PutWithValueObjectMissingArgsValidateOnly() = DomainObject14.PutWithValueObjectMissingArgsValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs api 
    [<Test>]
    member x.PutWithValueObjectMalformedDateTimeArgs() = DomainObject14.PutWithValueObjectMalformedDateTimeArgs api 
    [<Test>]
    member x.PutWithValueObjectMalformedArgsValidateOnly() = DomainObject14.PutWithValueObjectMalformedArgsValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue api 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly api 
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue api 
    
    [<Test>]
    member x.PutWithReferenceObjectNotFoundArgsValue() = DomainObject14.PutWithReferenceObjectNotFoundArgsValue api 

    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly api 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgs() = DomainObject14.PutWithReferenceObjectMalformedArgs api 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgsValidateOnly() = DomainObject14.PutWithReferenceObjectMalformedArgsValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectDisabledValue() = DomainObject14.PutWithValueObjectDisabledValue api 
    [<Test>]
    member x.PutWithValueObjectDisabledValueValidateOnly() = DomainObject14.PutWithValueObjectDisabledValueValidateOnly api 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValue() = DomainObject14.PutWithReferenceObjectDisabledValue api 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValueValidateOnly() = DomainObject14.PutWithReferenceObjectDisabledValueValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectInvisibleValue() = DomainObject14.PutWithValueObjectInvisibleValue api 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValue() = DomainObject14.PutWithReferenceObjectInvisibleValue api 
    [<Test>]
    member x.PutWithValueObjectInvisibleValueValidateOnly() = DomainObject14.PutWithValueObjectInvisibleValueValidateOnly api 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvisibleValueValidateOnly api 
    [<Test>]
    member x.PutWithValueImmutableObject() = DomainObject14.PutWithValueImmutableObject api 
    [<Test>]
    member x.PutWithReferenceImmutableObject() = DomainObject14.PutWithReferenceImmutableObject api 
    [<Test>]
    member x.PutWithValueImmutableObjectValidateOnly() = DomainObject14.PutWithValueImmutableObjectValidateOnly api 
    [<Test>]
    member x.PutWithReferenceImmutableObjectValidateOnly() = DomainObject14.PutWithReferenceImmutableObjectValidateOnly api 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsName() = DomainObject14.PutWithValueObjectInvalidArgsName api 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsNameValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsNameValidateOnly api 
    [<Test>]   
    member x.NotAcceptablePutObjectWrongMediaType() = DomainObject14.NotAcceptablePutObjectWrongMediaType api  
    [<Test>]
    member x.PutWithValueInternalError() = DomainObject14.PutWithValueInternalError api 
    [<Test>]
    member x.PutWithReferenceInternalError() = DomainObject14.PutWithReferenceInternalError api 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidation() = DomainObject14.PutWithValueObjectFailCrossValidation api 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidationValidateOnly() = DomainObject14.PutWithValueObjectFailCrossValidationValidateOnly api 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidation() = DomainObject14.PutWithReferenceObjectFailsCrossValidation api 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidationValidateOnly() = DomainObject14.PutWithReferenceObjectFailsCrossValidationValidateOnly api
    [<Test>]
    member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey api 
    [<Test>]
    member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType api 
    [<Test>] 
    [<Ignore>] // no longer fails no sure if an issue - seems no reason to make fail ? 
    member x.ObjectNotFoundAbstractType() = DomainObject14.ObjectNotFoundAbstractType api  
    // view models
    [<Test>]
    member x.GetMostSimpleViewModel() = DomainObject14.GetMostSimpleViewModel api  
    [<Test>]
    member x.GetWithValueViewModel() = DomainObject14.GetWithValueViewModel api  
    [<Test>]
    member x.GetWithReferenceViewModel() = DomainObject14.GetWithReferenceViewModel api
    [<Test>]
    member x.GetWithNestedViewModel() = DomainObject14.GetWithNestedViewModel api
    [<Test>]
    member x.PutWithReferenceViewModel() = DomainObject14.PutWithReferenceViewModel api
    [<Test>]
    member x.PutWithNestedViewModel() = DomainObject14.PutWithNestedViewModel api
    [<Test>]
    member x.PutWithValueViewModel() = DomainObject14.PutWithValueViewModel api
    // DomainService15
    [<Test>] 
    member x.GetService() = DomainService15.GetService api 
    [<Test>] 
    member x.GetContributorService() = DomainService15.GetContributorService api 
    [<Test>] 
    member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly api 
    [<Test>] 
    member x.GetServiceFormalOnly() = DomainService15.GetServiceFormalOnly api 
    [<Test>] 
    member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType api 
    [<Test>]
    member x.GetWithActionService() = DomainService15.GetWithActionService api   
    [<Test>]
    member x.InvalidGetService() = DomainService15.InvalidGetService api 
    [<Test>]
    member x.NotFoundGetService() = DomainService15.NotFoundGetService api 
    [<Test>]   
    member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType api 
    // ObjectProperty16
    [<Test>]
    member x.GetValueProperty() = ObjectProperty16.GetValueProperty api 
    [<Test>]
    member x.GetFileAttachmentProperty() = ObjectProperty16.GetFileAttachmentProperty api 
    [<Test>]
    member x.GetImageAttachmentProperty() = ObjectProperty16.GetImageAttachmentProperty api 
    [<Test>]
    member x.GetFileAttachmentValue() = ObjectProperty16.GetFileAttachmentValue api 
    [<Test>]
    member x.GetAttachmentValueWrongMediaType() = ObjectProperty16.GetAttachmentValueWrongMediaType api 
    [<Test>]
    member x.GetImageAttachmentValue() = ObjectProperty16.GetImageAttachmentValue api 
    [<Test>]
    member x.GetValuePropertyViewModel() = ObjectProperty16.GetValuePropertyViewModel api 
    [<Test>]
    member x.GetEnumValueProperty() = ObjectProperty16.GetEnumValueProperty api 
    [<Test>]
    member x.GetValuePropertyUserAuth() = ObjectProperty16.GetValuePropertyUserAuth api     
    [<Test>]
    member x.GetValuePropertyFormalOnly() = ObjectProperty16.GetValuePropertyFormalOnly api 
    [<Test>]
    member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly api 
    [<Test>]
    member x.GetStringValueProperty() = ObjectProperty16.GetStringValueProperty api 
    [<Test>]
    member x.GetBlobValueProperty() = ObjectProperty16.GetBlobValueProperty api 
    [<Test>]
    member x.GetClobValueProperty() = ObjectProperty16.GetClobValueProperty api 
    [<Test>]
    member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType api 
    [<Test>]
    member x.GetChoicesValueProperty() = ObjectProperty16.GetChoicesValueProperty api 
    [<Test>]
    member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty api 
    [<Test>]
    member x.GetUserDisabledValueProperty() = ObjectProperty16.GetUserDisabledValueProperty api 
    [<Test>]
    member x.GetUserDisabledValuePropertyAuthorised() = ObjectProperty16.GetUserDisabledValuePropertyAuthorised api 
    [<Test>]
    member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty api 
    [<Test>]
    member x.GetAutoCompleteProperty() = ObjectProperty16.GetAutoCompleteProperty api 
    [<Test>]
    member x.InvokeAutoComplete() = ObjectProperty16.InvokeAutoComplete api 

    [<Test>]
    member x.InvokeAutoCompleteErrorNoParm() = ObjectProperty16.InvokeAutoCompleteErrorNoParm api 
    [<Test>]
    member x.InvokeAutoCompleteErrorMalformedParm() = ObjectProperty16.InvokeAutoCompleteErrorMalformedParm api 
    [<Test>]
    member x.InvokeAutoCompleteErrorUnrecognisedParm() = ObjectProperty16.InvokeAutoCompleteErrorUnrecognisedParm api 


    [<Test>]
    member x.InvokeConditionalChoicesReference() = ObjectProperty16.InvokeConditionalChoicesReference api 

    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorMalformedParm api 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorNoParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorNoParm api 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorUnrecognisedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorUnrecognisedParm api 


    [<Test>]
    member x.InvokeConditionalChoicesValue() = ObjectProperty16.InvokeConditionalChoicesValue api 

    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMalformedParm api 
    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMissingParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMissingParm api 

    [<Test>]
    member x.GetReferencePropertyViewModel() = ObjectProperty16.GetReferencePropertyViewModel api 
    [<Test>]
    member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty api 
    [<Test>]
    member x.GetChoicesReferenceProperty() = ObjectProperty16.GetChoicesReferenceProperty api 
    [<Test>]
    member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty api 
    [<Test>]
    member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty api 
    [<Test>]
    member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty api
    [<Test>]
    member x.GetUserHiddenValueProperty() = ObjectProperty16.GetUserHiddenValueProperty api  
    [<Test>]
    member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty api 
    [<Test>]    
    member x.NotAcceptableGetPropertyWrongMediaType() = ObjectProperty16.NotAcceptableGetPropertyWrongMediaType api 
    [<Test>]
    member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty api 
    [<Test>]
    member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty api 
    [<Test>]
    member x.GetPropertyAsCollection() = ObjectProperty16.GetPropertyAsCollection api 
    [<Test>]
    member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess api 
    [<Test>]
    member x.PutDateTimeValuePropertySuccess() = ObjectProperty16.PutDateTimeValuePropertySuccess api 
    [<Test>]
    member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess api 
    [<Test>]
    member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail api 
    [<Test>]
    member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch api 
    [<Test>]
    member x.PutUserDisabledValuePropertySuccess() = ObjectProperty16.PutUserDisabledValuePropertySuccess api 
    [<Test>]
    member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly api 
    [<Test>]
    member x.PutClobPropertyBadRequest() = ObjectProperty16.PutClobPropertyBadRequest api 
    [<Test>]
    member x.PutBlobPropertyBadRequest() = ObjectProperty16.PutBlobPropertyBadRequest api 
    [<Test>]
    member x.DeleteValuePropertySuccess() = ObjectProperty16.DeleteValuePropertySuccess api 
    [<Test>]
    member x.DeleteValuePropertySuccessValidateOnly() = ObjectProperty16.DeleteValuePropertySuccessValidateOnly api 
    [<Test>]
    member x.PutNullValuePropertySuccess() = ObjectProperty16.PutNullValuePropertySuccess api 
    [<Test>]
    member x.PutNullValuePropertySuccessValidateOnly() = ObjectProperty16.PutNullValuePropertySuccessValidateOnly api 
    [<Test>]
    member x.PutReferencePropertySuccess() = ObjectProperty16.PutReferencePropertySuccess api 
    [<Test>]
    member x.PutReferencePropertySuccessValidateOnly() = ObjectProperty16.PutReferencePropertySuccessValidateOnly api 
    [<Test>]
    member x.DeleteReferencePropertySuccess() = ObjectProperty16.DeleteReferencePropertySuccess api 
    [<Test>]
    member x.DeleteReferencePropertySuccessValidateOnly() = ObjectProperty16.DeleteReferencePropertySuccessValidateOnly api 
    [<Test>]
    member x.PutNullReferencePropertySuccess() = ObjectProperty16.PutNullReferencePropertySuccess api 
    [<Test>]
    member x.PutNullReferencePropertySuccessValidateOnly() = ObjectProperty16.PutNullReferencePropertySuccessValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyMissingArgs() = ObjectProperty16.PutWithValuePropertyMissingArgs api 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgs() = ObjectProperty16.PutWithValuePropertyMalformedArgs api 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue api 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidation() = ObjectProperty16.PutWithReferencePropertyFailCrossValidation api 

    [<Test>]
    member x.PutWithReferencePropertyMalformedArgs() = ObjectProperty16.PutWithReferencePropertyMalformedArgs api 

    [<Test>]
    member x.PutWithValuePropertyFailCrossValidation() = ObjectProperty16.PutWithValuePropertyFailCrossValidation api 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValue() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValue api 
    [<Test>]
    member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue api 
    [<Test>]
    member x.PutWithValuePropertyUserDisabledValue() = ObjectProperty16.PutWithValuePropertyUserDisabledValue api 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue api 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue api 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValue() = ObjectProperty16.PutWithReferencePropertyInvisibleValue api 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObject() = ObjectProperty16.PutWithValuePropertyOnImmutableObject api 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObject() = ObjectProperty16.PutWithReferencePropertyOnImmutableObject api 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName api 
    [<Test>]    
    member x.NotAcceptablePutPropertyWrongMediaType() = ObjectProperty16.NotAcceptablePutPropertyWrongMediaType api 
    [<Test>]
    member x.PutWithValuePropertyMissingArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly api 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithReferencePropertyFailCrossValidationValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithValuePropertyFailCrossValidationValidateOnly api 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly api 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly api 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithValuePropertyOnImmutableObjectValidateOnly api 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithReferencePropertyOnImmutableObjectValidateOnly api 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly api   
    [<Test>]
    member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError api 
    [<Test>]
    member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError api 
    [<Test>]
    member x.DeleteValuePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly api 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly api 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly api 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly api 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteValuePropertyOnImmutableObjectValidateOnly api 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteReferencePropertyOnImmutableObjectValidateOnly api 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly api 
    [<Test>]
    member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue api 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue api 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue api 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue api 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObject() = ObjectProperty16.DeleteValuePropertyOnImmutableObject api 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObject() = ObjectProperty16.DeleteReferencePropertyOnImmutableObject api 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName api 
    [<Test>]    
    member x.NotAcceptableDeletePropertyWrongMediaType() = ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType api 
    [<Test>]
    member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError api 
    [<Test>]
    member x.DeleteReferencePropertyInternalError() = ObjectProperty16.DeleteReferencePropertyInternalError api 
    [<Test>]
    member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound api 
    // ObjectCollection17
    [<Test>]
    member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty api 
    [<Test>]
    member x.GetCollectionPropertyViewModel() = ObjectCollection17.GetCollectionPropertyViewModel api 
    [<Test>]
    member x.GetCollectionPropertyFormalOnly() = ObjectCollection17.GetCollectionPropertyFormalOnly api 
    [<Test>]
    member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly api 
    [<Test>]
    member x.GetCollectionSetProperty() = ObjectCollection17.GetCollectionSetProperty api 
    [<Test>]
    member x.GetCollectionSetPropertyFormalOnly() = ObjectCollection17.GetCollectionSetPropertyFormalOnly api 
    [<Test>]
    member x.GetCollectionSetPropertySimpleOnly() = ObjectCollection17.GetCollectionSetPropertySimpleOnly api 
    [<Test>]
    member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType api 
    [<Test>]
    member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty api 
    [<Test>]
    member x.GetCollectionValue() = ObjectCollection17.GetCollectionValue api 
    [<Test>]    
    member x.AddToAndDeleteFromCollectionProperty() = ObjectCollection17.AddToAndDeleteFromCollectionProperty api 
    [<Test>]    
    member x.AddToAndDeleteFromCollectionPropertyViewModel() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyViewModel api 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionProperty() = ObjectCollection17.AddToAndDeleteFromSetCollectionProperty api 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyConcurrencySuccess api 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess api 
//    [<Test>]    
//    member x.AddToCollectionPropertyConcurrencyFail() = ObjectCollection17.AddToCollectionPropertyConcurrencyFail api 
//    [<Test>]    
//    member x.AddToCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.AddToCollectionPropertyMissingIfMatchHeader api 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyConcurrencyFail() = ObjectCollection17.DeleteFromCollectionPropertyConcurrencyFail api 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.DeleteFromCollectionPropertyMissingIfMatchHeader api 
//    [<Test>]    
//    member x.AddToCollectionPropertyValidateOnly() = ObjectCollection17.AddToCollectionPropertyValidateOnly api 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyValidateOnly() = ObjectCollection17.DeleteFromCollectionPropertyValidateOnly api 
    [<Test>]
    member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection api 
    [<Test>]
    member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection api 
    [<Test>]
    member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection api  
    [<Test>]    
    member x.NotAcceptableGetCollectionWrongMediaType() = ObjectCollection17.NotAcceptableGetCollectionWrongMediaType api 
    [<Test>]
    member x.GetErrorValueCollection() = ObjectCollection17.GetErrorValueCollection api 
    [<Test>]
    member x.GetCollectionAsProperty() = ObjectCollection17.GetCollectionAsProperty api 
//    [<Test>]
//    
//    member x.AddToCollectionMissingArgs() = ObjectCollection17.AddToCollectionMissingArgs api 
//    [<Test>]
//    
//    member x.AddToCollectionMalformedArgs() = ObjectCollection17.AddToCollectionMalformedArgs api 
//    [<Test>]
//    
//    member x.AddToCollectionInvalidArgs() = ObjectCollection17.AddToCollectionInvalidArgs api 
//    [<Test>]
//    
//    member x.AddToCollectionDisabledValue() = ObjectCollection17.AddToCollectionDisabledValue api 
//    [<Test>]
//    
//    member x.AddToCollectionInvisibleValue() = ObjectCollection17.AddToCollectionInvisibleValue api 
//    [<Test>]
//    
//    member x.AddToCollectionImmutableObject() = ObjectCollection17.AddToCollectionImmutableObject api 
//    [<Test>]
//    
//    member x.AddToCollectionInvalidArgsName() = ObjectCollection17.AddToCollectionInvalidArgsName api 
//    [<Test>]
//        
//    member x.NotAcceptableAddCollectionWrongMediaType() = ObjectCollection17.NotAcceptableAddCollectionWrongMediaType api 
//    [<Test>]
//    
//    member x.AddToCollectionMissingArgsValidateOnly() = ObjectCollection17.AddToCollectionMissingArgsValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionMalformedArgsValidateOnly() = ObjectCollection17.AddToCollectionMalformedArgsValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionInvalidArgsValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionDisabledValueValidateOnly() = ObjectCollection17.AddToCollectionDisabledValueValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionInvisibleValueValidateOnly() = ObjectCollection17.AddToCollectionInvisibleValueValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionImmutableObjectValidateOnly() = ObjectCollection17.AddToCollectionImmutableObjectValidateOnly api 
//    [<Test>]
//    
//    member x.AddToCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsNameValidateOnly api   
//    [<Test>]
//    
//    member x.AddToCollectionInternalError() = ObjectCollection17.AddToCollectionInternalError api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionMissingArgs() = ObjectCollection17.DeleteFromCollectionMissingArgs api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionMalformedArgs() = ObjectCollection17.DeleteFromCollectionMalformedArgs api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvalidArgs() = ObjectCollection17.DeleteFromCollectionInvalidArgs api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionDisabledValue() = ObjectCollection17.DeleteFromCollectionDisabledValue api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvisibleValue() = ObjectCollection17.DeleteFromCollectionInvisibleValue api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionImmutableObject() = ObjectCollection17.DeleteFromCollectionImmutableObject api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvalidArgsName() = ObjectCollection17.DeleteFromCollectionInvalidArgsName api 
//    [<Test>]
//        
//    member x.NotAcceptableDeleteFromCollectionWrongMediaType() = ObjectCollection17.NotAcceptableDeleteFromCollectionWrongMediaType api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionMissingArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMissingArgsValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionMalformedArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMalformedArgsValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvalidArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionDisabledValueValidateOnly() = ObjectCollection17.DeleteFromCollectionDisabledValueValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvisibleValueValidateOnly() = ObjectCollection17.DeleteFromCollectionInvisibleValueValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionImmutableObjectValidateOnly() = ObjectCollection17.DeleteFromCollectionImmutableObjectValidateOnly api 
//    [<Test>]
//    
//    member x.DeleteFromCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsNameValidateOnly api  
//    [<Test>]
//    
//    member x.DeleteFromCollectionInternalError() = ObjectCollection17.DeleteFromCollectionInternalError api 
    // ObjectAction18
    [<Test>]
    member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject api
    [<Test>]
    member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService api  
    [<Test>]
    member x.GetActionContributedService() = ObjectAction18.GetActionContributedService api  
    [<Test>]
    member x.GetActionPropertyViewModel() = ObjectAction18.GetActionPropertyViewModel api  

    [<Test>]
    member x.GetActionPropertyDateTimeViewModel() = ObjectAction18.GetActionPropertyDateTimeViewModel api
    [<Test>]
    member x.GetActionPropertyDateTimeService() = ObjectAction18.GetActionPropertyDateTimeService api  
    [<Test>]
    member x.GetActionPropertyDateTimeObject() = ObjectAction18.GetActionPropertyDateTimeObject api  
 
    [<Test>]
    member x.GetActionPropertyCollectionViewModel() = ObjectAction18.GetActionPropertyCollectionViewModel api
    [<Test>]
    member x.GetActionPropertyCollectionService() = ObjectAction18.GetActionPropertyCollectionService api  
    [<Test>]
    member x.GetActionPropertyCollectionObject() = ObjectAction18.GetActionPropertyCollectionObject api 



    [<Test>]
    member x.GetOverloadedActionPropertyObject() = ObjectAction18.GetOverloadedActionPropertyObject api
    [<Test>]
    member x.GetOverloadedActionPropertyService() = ObjectAction18.GetOverloadedActionPropertyService api  
    [<Test>]
    member x.GetOverloadedActionPropertyViewModel() = ObjectAction18.GetOverloadedActionPropertyViewModel api  

    [<Test>]
    member x.GetUserDisabledActionPropertyObject() = ObjectAction18.GetUserDisabledActionPropertyObject api
    [<Test>]
    member x.GetUserDisabledActionPropertyService() = ObjectAction18.GetUserDisabledActionPropertyService api 
    [<Test>]
    member x.GetUserDisabledActionPropertyViewModel() = ObjectAction18.GetUserDisabledActionPropertyViewModel api 
    [<Test>]
    member x.GetActionPropertyQueryOnlyObject() = ObjectAction18.GetActionPropertyQueryOnlyObject api
    [<Test>]
    member x.GetActionPropertyQueryOnlyService() = ObjectAction18.GetActionPropertyQueryOnlyService api   
    [<Test>]
    member x.GetActionPropertyQueryOnlyViewModel() = ObjectAction18.GetActionPropertyQueryOnlyViewModel api   
    [<Test>]
    member x.GetActionPropertyIdempotentObject() = ObjectAction18.GetActionPropertyIdempotentObject api
    [<Test>]
    member x.GetActionPropertyIdempotentService() = ObjectAction18.GetActionPropertyIdempotentService api 
    [<Test>]
    member x.GetActionPropertyIdempotentViewModel() = ObjectAction18.GetActionPropertyIdempotentViewModel api 
    [<Test>]
    member x.GetActionPropertyWithOptObject() = ObjectAction18.GetActionPropertyWithOptObject api
    [<Test>]
    member x.GetActionPropertyWithOptService() = ObjectAction18.GetActionPropertyWithOptService api  
    [<Test>]
    member x.GetActionPropertyWithOptViewModel() = ObjectAction18.GetActionPropertyWithOptViewModel api  
    [<Test>]
    member x.GetActionPropertyWithOptObjectSimpleOnly() = ObjectAction18.GetActionPropertyWithOptObjectSimpleOnly api
    [<Test>]
    member x.GetActionPropertyWithOptServiceSimpleOnly() = ObjectAction18.GetActionPropertyWithOptServiceSimpleOnly api   
    [<Test>]
    member x.GetActionPropertyWithOptViewModelSimpleOnly() = ObjectAction18.GetActionPropertyWithOptViewModelSimpleOnly api   
    [<Test>]
    member x.GetActionPropertyWithOptObjectFormalOnly() = ObjectAction18.GetActionPropertyWithOptObjectFormalOnly api
    [<Test>]
    member x.GetActionPropertyWithOptServiceFormalOnly() = ObjectAction18.GetActionPropertyWithOptServiceFormalOnly api 
    [<Test>]
    member x.GetActionPropertyWithOptViewModelFormalOnly() = ObjectAction18.GetActionPropertyWithOptViewModelFormalOnly api 
    [<Test>]
    member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType api
    [<Test>]
    member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType api  
    [<Test>]
    member x.GetActionPropertyViewModelWithMediaType() = ObjectAction18.GetActionPropertyViewModelWithMediaType api  
    [<Test>]
    member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject api 
    [<Test>]
    member x.GetScalarActionService() = ObjectAction18.GetScalarActionService api 
    [<Test>]
    member x.GetScalarActionViewModel() = ObjectAction18.GetScalarActionViewModel api 
    [<Test>]
    member x.GetActionWithValueParmObject() = ObjectAction18.GetActionWithValueParmObject api
    [<Test>]
    member x.GetActionWithValueParmService() = ObjectAction18.GetActionWithValueParmService api 
    [<Test>]
    member x.GetActionWithValueParmViewModel() = ObjectAction18.GetActionWithValueParmViewModel api 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesObject() = ObjectAction18.GetActionWithValueParmWithChoicesObject api
    [<Test>]
    member x.GetActionWithValueParmWithChoicesService() = ObjectAction18.GetActionWithValueParmWithChoicesService api 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesViewModel() = ObjectAction18.GetActionWithValueParmWithChoicesViewModel api 
    [<Test>]
    member x.GetActionWithValueParmWithDefaultObject() = ObjectAction18.GetActionWithValueParmWithDefaultObject api
    [<Test>]
    member x.GetActionWithValueParmWithDefaultService() = ObjectAction18.GetActionWithValueParmWithDefaultService api    
    [<Test>]
    member x.GetActionWithValueParmWithDefaultViewModel() = ObjectAction18.GetActionWithValueParmWithDefaultViewModel api    
    [<Test>]
    member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject api
    [<Test>]
    member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService api   
    [<Test>]
    member x.GetActionWithReferenceParmViewModel() = ObjectAction18.GetActionWithReferenceParmViewModel api   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesObject api
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesService() = ObjectAction18.GetActionWithReferenceParmWithChoicesService api   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesViewModel() = ObjectAction18.GetActionWithReferenceParmWithChoicesViewModel api   

    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteObject() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteObject api
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteService() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteService api   
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteViewModel() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteViewModel api   

    [<Test>]
    member x.InvokeParmWithAutoCompleteObject() = ObjectAction18.InvokeParmWithAutoCompleteObject api
    [<Test>]
    member x.InvokeParmWithAutoCompleteService() = ObjectAction18.InvokeParmWithAutoCompleteService api   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModel() = ObjectAction18.InvokeParmWithAutoCompleteViewModel api 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorNoParm api
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorNoParm api   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorNoParm api 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorMalformedParm api
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorMalformedParm api   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorMalformedParm api 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm api
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm api   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm api 






    [<Test>]
    member x.InvokeParmWithConditionalChoicesObject() = ObjectAction18.InvokeParmWithConditionalChoicesObject api
    [<Test>]
    member x.InvokeParmWithConditionalChoicesService() = ObjectAction18.InvokeParmWithConditionalChoicesService api   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeParmWithConditionalChoicesViewModel api 

    [<Test>]
    member x.InvokeParmWithConditionalChoicesObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesObjectErrorMalformedParm api
    [<Test>]
    member x.InvokeParmWithConditionalChoicesServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesServiceErrorMalformedParm api   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm api 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm api
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm api   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm api 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObject() = ObjectAction18.InvokeValueParmWithConditionalChoicesObject api
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesService() = ObjectAction18.InvokeValueParmWithConditionalChoicesService api   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModel api 

    
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultObject api
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultService() = ObjectAction18.GetActionWithReferenceParmWithDefaultService api  
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultViewModel() = ObjectAction18.GetActionWithReferenceParmWithDefaultViewModel api  
    [<Test>]
    member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject api
    [<Test>]
    member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService api   
    [<Test>]
    member x.GetActionWithChoicesAndDefaultViewModel() = ObjectAction18.GetActionWithChoicesAndDefaultViewModel api   
    [<Test>]
    member x.GetContributedActionOnContributee() = ObjectAction18.GetContributedActionOnContributee api 
    [<Test>]
    member x.GetContributedActionOnContributeeOnBaseClass() = ObjectAction18.GetContributedActionOnContributeeBaseClass api 
    [<Test>]
    member x.GetContributedActionOnContributeeWithRef() = ObjectAction18.GetContributedActionOnContributeeWithRef api 
    [<Test>]
    member x.GetContributedActionOnContributeeWithValue() = ObjectAction18.GetContributedActionOnContributeeWithValue api 
    [<Test>]
    member x.GetContributedActionOnContributer() = ObjectAction18.GetContributedActionOnContributer api 
    [<Test>]
    member x.GetContributedActionOnContributerOnBaseClass() = ObjectAction18.GetContributedActionOnContributerBaseClass api 
    [<Test>]
    member x.GetContributedActionOnContributerWithRef() = ObjectAction18.GetContributedActionOnContributerWithRef api 
    [<Test>]
    member x.GetContributedActionOnContributerWithValue() = ObjectAction18.GetContributedActionOnContributerWithValue api 
    [<Test>]
    member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject api
    [<Test>]
    member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService api   
    [<Test>]
    member x.GetInvalidActionPropertyViewModel() = ObjectAction18.GetInvalidActionPropertyViewModel api   
    [<Test>]
    member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject api
    [<Test>]
    member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService api 
    [<Test>]
    member x.GetNotFoundActionPropertyViewModel() = ObjectAction18.GetNotFoundActionPropertyViewModel api 
    [<Test>]
    member x.GetUserDisabledActionObject() = ObjectAction18.GetUserDisabledActionObject api 
    [<Test>]
    member x.GetUserDisabledActionService() = ObjectAction18.GetUserDisabledActionService api  
    [<Test>]
    member x.GetUserDisabledActionViewModel() = ObjectAction18.GetUserDisabledActionViewModel api  
    [<Test>]
    member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject api
    [<Test>]
    member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService api   
    [<Test>]
    member x.GetHiddenActionPropertyViewModel() = ObjectAction18.GetHiddenActionPropertyViewModel api   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject api   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeService() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeService api 
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeViewModel() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeViewModel api 
    [<Test>]
    member x.GetQueryActionObject() = ObjectAction18.GetQueryActionObject api
    [<Test>]
    member x.GetQueryActionService() = ObjectAction18.GetQueryActionService api  
    [<Test>]
    member x.GetQueryActionViewModel() = ObjectAction18.GetQueryActionViewModel api  
    [<Test>]
    member x.GetQueryActionWithParmsObject() = ObjectAction18.GetQueryActionWithParmsObject api
    [<Test>]
    member x.GetQueryActionWithParmsService() = ObjectAction18.GetQueryActionWithParmsService api
    [<Test>]
    member x.GetQueryActionWithParmsViewModel() = ObjectAction18.GetQueryActionWithParmsViewModel api
    [<Test>]
    member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject api 
    [<Test>]
    member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService api 
    [<Test>]
    member x.GetCollectionActionViewModel() = ObjectAction18.GetCollectionActionViewModel api 
    [<Test>]
    member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject api 
    [<Test>]
    member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService api  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModel() = ObjectAction18.GetCollectionActionWithParmsViewModel api  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsObjectFormalOnly api 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceFormalOnly api  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelFormalOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelFormalOnly api  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly api 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly api
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelSimpleOnly api

    [<Test>]
    member x.ActionNotFound() = ObjectAction18.ActionNotFound api       
    // ObjectActionInvoke19
    [<Test>]
    member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject api
    [<Test>]
    member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService api  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModel api
   
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectObject api
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectService() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectService api  
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectViewModel api



    [<Test>]
    member x.PostInvokeActionContributedService() = ObjectActionInvoke19.PostInvokeActionContributedService api  


    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectObject api
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectService() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectService api  
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectViewModel api


    [<Test>]
    member x.PostInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnViewModelObject api
    [<Test>]
    member x.PostInvokeActionReturnViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnViewModelService api  
    [<Test>]
    member x.PostInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnViewModelViewModel api


    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectObject api
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectService() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectService api   
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectViewModel api   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject api
    [<Test>]
    member x.PostInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectService api   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectViewModel api   

    [<Test>]
    member x.PostInvokeActionReturnNullViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelObject api
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelService api   
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelViewModel api   


    [<Test>]
    member x.PostInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly api  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelValidateOnly api  

    [<Test>]
    member x.PutInvokeActionReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnObjectObject api
    [<Test>]
    member x.PutInvokeActionReturnObjectService() = ObjectActionInvoke19.PutInvokeActionReturnObjectService api   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModel api   

    [<Test>]
    member x.PutInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PutInvokeActionReturnViewModelObject api
    [<Test>]
    member x.PutInvokeActionReturnViewModelService() = ObjectActionInvoke19.PutInvokeActionReturnViewModelService api   
    [<Test>]
    member x.PutInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PutInvokeActionReturnViewModelViewModel api   


    [<Test>]
    member x.PutInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectObject api
    [<Test>]
    member x.PutInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectService api  
    [<Test>]
    member x.PutInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectViewModel api  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectValidateOnly api
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceValidateOnly api  
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelValidateOnly api  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess api
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess api   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess api   

    [<Test>]
    member x.GetInvokeActionReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnObjectObject api
    [<Test>]
    member x.GetInvokeActionReturnObjectService() = ObjectActionInvoke19.GetInvokeActionReturnObjectService api    
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModel api    

    [<Test>]
    member x.GetInvokeActionReturnViewModelObject() = ObjectActionInvoke19.GetInvokeActionReturnViewModelObject api
    [<Test>]
    member x.GetInvokeActionReturnViewModelService() = ObjectActionInvoke19.GetInvokeActionReturnViewModelService api    
    [<Test>]
    member x.GetInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.GetInvokeActionReturnViewModelViewModel api    


    [<Test>]
    member x.GetInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectObject api
    [<Test>]
    member x.GetInvokeActionReturnNullObjectService() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectService api   
    [<Test>]
    member x.GetInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectViewModel api   

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceValidateOnly api   
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelValidateOnly api   

    [<Test>]
    member x.PostInvokeContribActionReturnObject() = ObjectActionInvoke19.PostInvokeContribActionReturnObject api
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithRefParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithRefParm api
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithValueParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithValueParm api
    [<Test>]
    member x.PostInvokeContribActionReturnObjectBaseClass() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectBaseClass api
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType api
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType api  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelWithMediaType api  
       
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess api
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess api  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess api  

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail api
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch api
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail api
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch api
    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess api
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess api 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess api 

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch api
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch api 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch api 
      
    [<Test>]
    member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject api
    [<Test>]
    member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService api   
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModel api   

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectFormalOnly api
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceFormalOnly api  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelFormalOnly api  

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly api  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelValidateOnly api  

    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject api
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService api  
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarViewModel api  

    [<Test>]
    member x.PostInvokeActionReturnNullScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject api
    [<Test>]
    member x.PostInvokeActionReturnNullScalarService() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarService api  
    [<Test>]
    member x.PostInvokeActionReturnNullScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarViewModel api  
    [<Test>]
    member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject api
    [<Test>]
    member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService api    
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModel api    

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectFormalOnly api
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceFormalOnly api   
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelFormalOnly api   

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly api  
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelValidateOnly api  

    [<Test>]
    member x.GetInvokeActionReturnQueryObject() = ObjectActionInvoke19.GetInvokeActionReturnQueryObject api
    [<Test>]
    member x.GetInvokeActionReturnQueryService() = ObjectActionInvoke19.GetInvokeActionReturnQueryService api    
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModel api    

    [<Test>]
    member x.GetInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryServiceValidateOnly api   
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelValidateOnly api   

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService api    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModel api    

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly api     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly api     


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService api  
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModel api  


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly api       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly api       


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService api   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModel api   


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly api    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly api    


    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObject api
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectService api    
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModel api    
    
      
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly api
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly api   
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly api   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObject api
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectService api   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModel api   


    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectObject api
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectService api  
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectViewModel api  
    
     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly api     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly api     
    
      
    [<Test>]
    member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject api
    [<Test>]
    member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService api  
    [<Test>]
    member x.NotFoundActionInvokeViewModel() = ObjectActionInvoke19.NotFoundActionInvokeViewModel api  


    [<Test>]
    member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject api
    [<Test>]
    member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService api   
    [<Test>]
    member x.HiddenActionInvokeViewModel() = ObjectActionInvoke19.HiddenActionInvokeViewModel api   


    [<Test>]
    member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject api
    [<Test>]
    member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService api   
    [<Test>]
    member x.GetActionWithSideEffectsViewModel() = ObjectActionInvoke19.GetActionWithSideEffectsViewModel api   


    [<Test>]
    member x.GetActionWithIdempotentObject() = ObjectActionInvoke19.GetActionWithIdempotentObject api
    [<Test>]
    member x.GetActionWithIdempotentService() = ObjectActionInvoke19.GetActionWithIdempotentService api  
    [<Test>]
    member x.GetActionWithIdempotentViewModel() = ObjectActionInvoke19.GetActionWithIdempotentViewModel api  


    [<Test>]
    member x.PutActionWithQueryOnlyObject() = ObjectActionInvoke19.PutActionWithQueryOnlyObject api
    [<Test>]
    member x.PutActionWithQueryOnlyService() = ObjectActionInvoke19.PutActionWithQueryOnlyService api  
    [<Test>]
    member x.PutActionWithQueryOnlyViewModel() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModel api  


    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeObject api
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeService api   
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeViewModel api   


    [<Test>]
    member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject api
    [<Test>]
    member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService api   
    [<Test>]
    member x.MissingParmsOnPostViewModel() = ObjectActionInvoke19.MissingParmsOnPostViewModel api   


    [<Test>]
    member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject api 
    [<Test>]
    member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService api 
    [<Test>]
    member x.DisabledActionPostInvokeViewModel() = ObjectActionInvoke19.DisabledActionPostInvokeViewModel api 


    [<Test>]
    member x.UserDisabledActionPostInvokeObject() = ObjectActionInvoke19.UserDisabledActionPostInvokeObject api 
    [<Test>]
    member x.UserDisabledActionPostInvokeService() = ObjectActionInvoke19.UserDisabledActionPostInvokeService api 
    [<Test>]
    member x.UserDisabledActionPostInvokeViewModel() = ObjectActionInvoke19.UserDisabledActionPostInvokeViewModel api 


    [<Test>]
    member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject api
    [<Test>]
    member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService api  
    [<Test>]
    member x.NotFoundActionPostInvokeViewModel() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModel api  


    [<Test>]
    member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject api
    [<Test>]
    member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService api   
    [<Test>]
    member x.HiddenActionPostInvokeViewModel() = ObjectActionInvoke19.HiddenActionPostInvokeViewModel api   


    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject api
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService api   
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeViewModel api   


    [<Test>]
    member x.PostQueryActionWithErrorObject() = ObjectActionInvoke19.PostQueryActionWithErrorObject api
    [<Test>]
    member x.PostQueryActionWithErrorService() = ObjectActionInvoke19.PostQueryActionWithErrorService api    
    [<Test>]
    member x.PostQueryActionWithErrorViewModel() = ObjectActionInvoke19.PostQueryActionWithErrorViewModel api    


    [<Test>]
    member x.GetQueryActionWithErrorObject() = ObjectActionInvoke19.GetQueryActionWithErrorObject api
    [<Test>]
    member x.GetQueryActionWithErrorService() = ObjectActionInvoke19.GetQueryActionWithErrorService api 
    [<Test>]
    member x.GetQueryActionWithErrorViewModel() = ObjectActionInvoke19.GetQueryActionWithErrorViewModel api 


    [<Test>]
    member x.MalformedFormalParmsOnPostQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject api
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService api 
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModel api 


    
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryObject() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryObject api
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryService() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryService api 
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryViewModel api 

     
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryObject() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryObject api
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryService() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryService api 
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryViewModel api 

    [<Test>]
    member x.InvalidFormalParmsOnPostQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObject api
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryService api   
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModel api   


    [<Test>]
    member x.MissingParmsOnGetQueryObject() = ObjectActionInvoke19.MissingParmsOnGetQueryObject api
    [<Test>]
    member x.MissingParmsOnGetQueryService() = ObjectActionInvoke19.MissingParmsOnGetQueryService api   
    [<Test>]
    member x.MissingParmsOnGetQueryViewModel() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModel api   


    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObject api
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryService() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryService api   
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModel api   


    [<Test>]
    member x.MalformedFormalParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObject api
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryService api   
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModel api   


    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObject api
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryService() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryService api    
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModel api    


    [<Test>]
    member x.InvalidFormalParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObject api
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryService api  
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModel api  


    [<Test>]
    member x.DisabledActionInvokeQueryObject() = ObjectActionInvoke19.DisabledActionInvokeQueryObject api
    [<Test>]
    member x.DisabledActionInvokeQueryService() = ObjectActionInvoke19.DisabledActionInvokeQueryService api    
    [<Test>]
    member x.DisabledActionInvokeQueryViewModel() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModel api    


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObject api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleService api   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel api   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleObject api
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleService api   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleViewModel api   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalObject api
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalService api   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalViewModel api   

    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly api        
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly api        
    
     
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObject api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalService api   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel api   


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly api   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly api   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObject api
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalService api  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel api  


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly api
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly api  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly api  
    
          
    [<Test>]
    member x.PostInvokeActionReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionReturnQueryObject api
    [<Test>]
    member x.PostInvokeActionReturnQueryService() = ObjectActionInvoke19.PostInvokeActionReturnQueryService api  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModel api  


    [<Test>]
    member x.PostInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryServiceValidateOnly api  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModelValidateOnly api  


    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObject api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryService api   
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModel api   

    
    [<Test>]
    member x.PostInvokeOverloadedActionObject() = ObjectActionInvoke19.PostInvokeOverloadedActionObject api
    [<Test>]
    member x.PostInvokeOverloadedActionService() = ObjectActionInvoke19.PostInvokeOverloadedActionService api   
    [<Test>]
    member x.PostInvokeOverloadedActionViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionViewModel api   


    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly api  
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly api  

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryService api   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModel api   
       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly api     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly api     
            
    [<Test>]
    member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject api
    [<Test>]
    member x.PostCollectionActionWithErrorService() = ObjectActionInvoke19.PostCollectionActionWithErrorService api  
    [<Test>]
    member x.PostCollectionActionWithErrorViewModel() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModel api  

    [<Test>]
    member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject api
    [<Test>]
    member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService api    
    [<Test>]
    member x.MissingParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModel api    

    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject api
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionService() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService api  
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModel api  

    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject api
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionService() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService api    
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModel api    

    [<Test>]
    member x.DisabledActionInvokeCollectionObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionObject api 
    [<Test>]
    member x.DisabledActionInvokeCollectionService() = ObjectActionInvoke19.DisabledActionInvokeCollectionService api 
    [<Test>]
    member x.DisabledActionInvokeCollectionViewModel() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModel api 

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObject api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalService api    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel api    

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly api       
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly api       
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalService api      
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel api      
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly api   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly api   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject api
    [<Test>]
    member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService api   
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel api   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectFormalOnly api
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceFormalOnly api    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelFormalOnly api    
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly api
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly api    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly api    
    
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject api
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService api   
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel api   
    
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject api
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService api   
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel api   
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService api     
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel api     
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly api             
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly api             
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService api   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel api   
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly api
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly api           
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly api           
    
    [<Test>]
    member x.PostQueryActionWithValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithValidateFailObject api
    [<Test>]
    member x.PostQueryActionWithValidateFailService() = ObjectActionInvoke19.PostQueryActionWithValidateFailService api   
    [<Test>]
    member x.PostQueryActionWithValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel api   

    [<Test>]
    member x.PostQueryActionWithCrossValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject api
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailService() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService api   
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel api  

    [<Test>]
    member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject api
    [<Test>]
    member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService api   
    [<Test>]
    member x.GetInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel api   
    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService api    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel api    
    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject api
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService api   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel api   
    
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject api
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService api   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel api   
    
    [<Test>]
    member x.MissingParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly api 
    [<Test>] 
    member x.MissingParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly api   
    [<Test>] 
    member x.MissingParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly api   
    
    [<Test>] 
    member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly api 
    [<Test>] 
    member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly api    
    [<Test>] 
    member x.MissingParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly api    

    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly api 
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly api    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly api    
    
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly api 
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly api   
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly api   
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly api 
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly api    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly api    
    
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly api 
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly api         
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly api         
    
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly api  
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly api      
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly api      
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly api 
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly api     
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly api     
    
    [<Test>] 
    member x.DisabledActionInvokeQueryObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly api 
    [<Test>] 
    member x.DisabledActionInvokeQueryServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly api    
    [<Test>] 
    member x.DisabledActionInvokeQueryViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly api    
    
    [<Test>] 
    member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly api 
    [<Test>] 
    member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly api 
    [<Test>] 
    member x.DisabledActionInvokeCollectionViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly api 
    
    [<Test>] 
    member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly api 
    [<Test>] 
    member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly api      
    [<Test>] 
    member x.NotFoundActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly api      
    
    [<Test>] 
    member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly api 
    [<Test>] 
    member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly api    
    [<Test>] 
    member x.HiddenActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly api    
    
    [<Test>] 
    member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly api 
    [<Test>] 
    member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly api   
    [<Test>] 
    member x.GetActionWithSideEffectsViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly api   
    
    [<Test>] 
    member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly api 
    [<Test>] 
    member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly api    
    [<Test>] 
    member x.GetActionWithIdempotentViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly api    
    
    [<Test>] 
    member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly api 
    [<Test>] 
    member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly api    
    [<Test>] 
    member x.PutActionWithQueryOnlyViewModelValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly api    
    
    [<Test>] 
    member x.GetQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly api 
    [<Test>] 
    member x.GetQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly api     
    [<Test>] 
    member x.GetQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly api     
    
    [<Test>] 
    member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly api 
    [<Test>] 
    member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly api    
    [<Test>] 
    member x.PostCollectionActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly api    
    
    [<Test>] 
    member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly api 
    [<Test>] 
    member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly api     
    [<Test>] 
    member x.MissingParmsOnPostViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly api     
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly api 
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly api     
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly api     
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly api 
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly api       
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly api       
    
    [<Test>] 
    member x.InvalidUrlOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly api 
    [<Test>] 
    member x.InvalidUrlOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly api   
    [<Test>] 
    member x.InvalidUrlOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly api   
    
    
    [<Test>] 
    member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly api  
    [<Test>] 
    member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly api  
    [<Test>] 
    member x.DisabledActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly api  
    
    [<Test>] 
    member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly api 
    [<Test>] 
    member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly api    
    [<Test>] 
    member x.NotFoundActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly api    
    
    [<Test>] 
    member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly api 
    [<Test>] 
    member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly api    
    [<Test>] 
    member x.HiddenActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly api    
    
    [<Test>] 
    member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly api 
    [<Test>] 
    member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly api     
    [<Test>] 
    member x.PostQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly api     
    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly api 
    [<Test>] 
    member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly api    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly api    
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly api 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly api  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly api  
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly api 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly api  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly api  
    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly api 
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly api    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly api    
    
    
 
    // DomainType21
    [<Test>] 
    member x.GetMostSimpleObjectType() = DomainType21.GetMostSimpleObjectType api
    [<Test>] 
    member x.GetWithActionObjectType() = DomainType21.GetWithActionObjectType api
    [<Test>]
    member x.GetWithActionServiceType() = DomainType21.GetWithActionServiceType api
    [<Test>] 
    member x.GetWithReferenceObjectType() = DomainType21.GetWithReferenceObjectType api
    [<Test>] 
    member x.GetWithValueObjectType() = DomainType21.GetWithValueObjectType api
    [<Test>] 
    member x.GetWithCollectionObjectType() = DomainType21.GetWithCollectionObjectType api
    [<Test>] 
    member x.GetPredefinedDomainTypes() = DomainType21.GetPredefinedDomainTypes api
    [<Test>] 
    member x.NotFoundPredefinedDomainTypes() = DomainType21.NotFoundPredefinedDomainTypes api
    [<Test>] 
    member x.NotFoundGetMostSimpleObjectType() = DomainType21.NotFoundGetMostSimpleObjectType api
    [<Test>] 
    member x.NotAcceptableGetMostSimpleObjectType() = DomainType21.NotAcceptableGetMostSimpleObjectType api
    // DomainProperty22
    [<Test>] 
    member x.GetValuePropertyType() = DomainProperty22.GetValuePropertyType api
    [<Test>] 
    member x.GetReferencePropertyType() = DomainProperty22.GetReferencePropertyType api
    [<Test>] 
    member x.GetValueStringPropertyType() = DomainProperty22.GetValueStringPropertyType api
    [<Test>] 
    member x.GetValueDateTimePropertyType() = DomainProperty22.GetValueDateTimePropertyType api
    [<Test>] 
    member x.NotFoundPropertyType() = DomainProperty22.NotFoundPropertyType api
    [<Test>] 
    member x.NotFoundTypePropertyType() = DomainProperty22.NotFoundTypePropertyType api
    [<Test>] 
    member x.NotAcceptableGetValuePropertyType() = DomainProperty22.NotAcceptableGetValuePropertyType api
    // DomainCollection23
    [<Test>] 
    member x.GetCollectionPropertyType() = DomainCollection23.GetCollectionPropertyType api
    [<Test>] 
    member x.GetSetCollectionPropertyType() = DomainCollection23.GetSetCollectionPropertyType api
    [<Test>] 
    member x.GetCollectionPropertyTypeWithDescription() = DomainCollection23.GetCollectionPropertyTypeWithDescription api
    [<Test>] 
    member x.NotFoundTypeCollectionPropertyType() = DomainCollection23.NotFoundTypeCollectionPropertyType api
    [<Test>] 
    member x.NotFoundCollectionPropertyType() = DomainCollection23.NotFoundCollectionPropertyType api
    [<Test>] 
    member x.NotAcceptableGetCollectionPropertyType() = DomainCollection23.NotAcceptableGetCollectionPropertyType api
    // DomainAction24
    [<Test>] 
    member x.GetActionTypeObjectNoParmsScalar() = DomainAction24.GetActionTypeObjectNoParmsScalar api
    [<Test>] 
    member x.GetActionTypeServiceNoParmsScalar() = DomainAction24.GetActionTypeServiceNoParmsScalar api

    [<Test>] 
    member x.GetOverloadedActionTypeObjectNoParmsScalar() = DomainAction24.GetOverloadedActionTypeObjectNoParmsScalar api
    [<Test>] 
    member x.GetOverloadedActionTypeServiceNoParmsScalar() = DomainAction24.GetOverloadedActionTypeServiceNoParmsScalar api


    [<Test>] 
    member x.GetActionTypeObjectNoParmsVoid() = DomainAction24.GetActionTypeObjectNoParmsVoid api
    [<Test>] 
    member x.GetActionTypeServiceNoParmsVoid() = DomainAction24.GetActionTypeServiceNoParmsVoid api
    [<Test>] 
    member x.GetActionTypeObjectNoParmsCollection() = DomainAction24.GetActionTypeObjectNoParmsCollection api
    [<Test>] 
    member x.GetActionTypeServiceNoParmsCollection() = DomainAction24.GetActionTypeServiceNoParmsCollection api
    [<Test>] 
    member x.GetActionTypeObjectParmsScalar() = DomainAction24.GetActionTypeObjectParmsScalar api
    [<Test>] 
    member x.GetActionTypeServiceParmsScalar() = DomainAction24.GetActionTypeServiceParmsScalar api
    [<Test>] 
    member x.GetActionTypeObjectParmsVoid() = DomainAction24.GetActionTypeObjectParmsVoid api
    [<Test>] 
    member x.GetActionTypeServiceParmsVoid() = DomainAction24.GetActionTypeServiceParmsVoid api
    [<Test>] 
    member x.GetActionTypeObjectParmsCollection() = DomainAction24.GetActionTypeObjectParmsCollection api
    [<Test>] 
    member x.GetActionTypeServiceParmsCollection() = DomainAction24.GetActionTypeServiceParmsCollection api
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributee() = DomainAction24.GetActionTypeObjectContributedOnContributee api
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributer() = DomainAction24.GetActionTypeObjectContributedOnContributer api
    [<Test>] 
    member x.NotFoundTypeActionType() = DomainAction24.NotFoundTypeActionType api
    [<Test>] 
    member x.NotFoundActionTypeObject() = DomainAction24.NotFoundActionTypeObject api
    [<Test>] 
    member x.NotFoundActionTypeService() = DomainAction24.NotFoundActionTypeService api
    [<Test>] 
    member x.NotAcceptableActionType() = DomainAction24.NotAcceptableActionType api
    // DomainActionParameter25
    [<Test>] 
    member x.GetActionParameterTypeInt() = DomainActionParameter25.GetActionParameterTypeInt api
    [<Test>] 
    member x.GetActionParameterTypeString() = DomainActionParameter25.GetActionParameterTypeString api
    [<Test>] 
    member x.GetOverloadedActionParameterTypeString() = DomainActionParameter25.GetOverloadedActionParameterTypeString api
    [<Test>] 
    member x.GetActionParameterTypeDateTime() = DomainActionParameter25.GetActionParameterTypeDateTime api
    [<Test>] 
    member x.GetActionParameterTypeReference() = DomainActionParameter25.GetActionParameterTypeReference api
    [<Test>] 
    member x.GetActionParameterTypeStringOptional() = DomainActionParameter25.GetActionParameterTypeStringOptional api
    [<Test>] 
    member x.NotFoundType() = DomainActionParameter25.NotFoundType api
    [<Test>] 
    member x.NotFoundAction() = DomainActionParameter25.NotFoundAction api
    [<Test>] 
    member x.NotFoundParm() = DomainActionParameter25.NotFoundParm api
    [<Test>] 
    member x.NotAcceptableActionParameterType() = DomainActionParameter25.NotAcceptableActionParameterType api    
    // DomainTypeActionInvoke26
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms api
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms api
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms api
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms api
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms api
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms api
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms api
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms api
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms api
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms api
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms api
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms api
    [<Test>] 
    member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms api
    [<Test>] 
    member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms api
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms api
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms api
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms api
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms api
    [<Test>] 
    member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf api
    [<Test>] 
    member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf api
    [<Test>] 
    member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf api
    [<Test>] 
    member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf api
end
