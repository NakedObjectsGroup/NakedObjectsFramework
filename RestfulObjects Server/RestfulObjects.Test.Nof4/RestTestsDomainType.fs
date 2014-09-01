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

let api (fw : INakedObjectsFramework) = 
    let api = new RestfulObjectsControllerBase()
    api.Surface <- new NakedObjects.Surface.Nof4.Implementation.NakedObjectsSurface(new NakedObjects.Surface.Nof4.Utility.ExternalOid(fw), fw)
    api

[<TestFixture>]
type Nof4TestsDomainType() = class      
    inherit  NakedObjects.Xat.AcceptanceTestCase()    
            
    [<TestFixtureSetUp>]
    member x.Setup() =     
        x.InitializeNakedObjectsFramework()
        MemoryObjectStore.DiscardObjects()
    
    [<SetUp>]
    member x.StartTest() =           
        x.Fixtures.InstallFixtures(x.NakedObjectsContext.ObjectPersistor, null)
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

    // HomePage5
    [<Test>] 
    member x.GetHomePage() = HomePage5.GetHomePage (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetHomePageSimple() = HomePage5.GetHomePageSimple (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetHomePageFormal() = HomePage5.GetHomePageFormal (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetHomePageWithMediaType() = HomePage5.GetHomePageWithMediaType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableGetHomePage() = HomePage5.NotAcceptableGetHomePage (api x.NakedObjectsContext)
    [<Test>] 
    member x.InvalidDomainModelGetHomePage() = HomePage5.InvalidDomainModelGetHomePage (api x.NakedObjectsContext)
    // User6
    [<Test>] 
    member x.GetUser() = User6.GetUser (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetUserWithMediaType() = User6.GetUserWithMediaType (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser (api x.NakedObjectsContext)
    // DomainServices7
    [<Test>] 
    member x.GetDomainServices() = DomainServices7.GetDomainServices (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetDomainServicesFormal() = DomainServices7.GetDomainServicesFormal (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaType (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices (api x.NakedObjectsContext) 
    // Version8
    [<Test>] 
    member x.GetVersion() = Version8.GetVersion (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion (api x.NakedObjectsContext) 
    //Objects9
    [<Test>]
    member x.GetMostSimpleTransientObject() = Objects9.GetMostSimpleTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleTransientObjectSimpleOnly() = Objects9.GetMostSimpleTransientObjectSimpleOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleTransientObjectFormalOnly() = Objects9.GetMostSimpleTransientObjectFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObject() = Objects9.PersistMostSimpleTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectSimpleOnly() = Objects9.PersistMostSimpleTransientObjectSimpleOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectFormalOnly() = Objects9.PersistMostSimpleTransientObjectFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectValidateOnly() = Objects9.PersistMostSimpleTransientObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithValueTransientObject() = Objects9.GetWithValueTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithReferenceTransientObject() = Objects9.GetWithReferenceTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithCollectionTransientObject() = Objects9.GetWithCollectionTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectFormalOnly() = Objects9.PersistWithValueTransientObjectFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnly() = Objects9.PersistWithValueTransientObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnly() = Objects9.PersistWithReferenceTransientObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnly() = Objects9.PersistWithCollectionTransientObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnlyFail() = Objects9.PersistWithReferenceTransientObjectValidateOnlyFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnlyFail() = Objects9.PersistWithCollectionTransientObjectValidateOnlyFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithValueTransientObjectFailInvalid() = Objects9.PersistWithValueTransientObjectFailInvalid (api x.NakedObjectsContext) 
    [<Test>]
    member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectFailInvalid() = Objects9.PersistWithReferenceTransientObjectFailInvalid (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgs() = Objects9.PersistMostSimpleTransientObjectMissingArgs (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgsValidateOnly() = Objects9.PersistMostSimpleTransientObjectMissingArgsValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingMemberArgs() = Objects9.PersistMostSimpleTransientObjectMissingMemberArgs (api x.NakedObjectsContext)  
    [<Test>]    
    member x.PersistMostSimpleTransientObjectNullDomainType() = Objects9.PersistMostSimpleTransientObjectNullDomainType (api x.NakedObjectsContext)  
    [<Test>]   
    member x.PersistMostSimpleTransientObjectEmptyDomainType() = Objects9.PersistMostSimpleTransientObjectEmptyDomainType (api x.NakedObjectsContext)   
    [<Test>]
    member x.PersistMostSimpleTransientObjectMalformedMemberArgs() = Objects9.PersistMostSimpleTransientObjectMalformedMemberArgs (api x.NakedObjectsContext)  
    [<Test>]
    member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PersistNoKeyTransientObject() = Objects9.PersistNoKeyTransientObject (api x.NakedObjectsContext)
    // Error10
    [<Test>]
    member x.Error() = Error10.Error (api x.NakedObjectsContext) 
    [<Test>]
    member x.NotAcceptableError() = Error10.NotAcceptableError (api x.NakedObjectsContext) 
    // DomainObject14
    [<Test>]
    member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithAttachmentsObject() = DomainObject14.GetWithAttachmentsObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSelectable() = DomainObject14.GetMostSimpleObjectConfiguredSelectable (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredNone() = DomainObject14.GetMostSimpleObjectConfiguredNone (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleObjectFormalOnly() = DomainObject14.GetMostSimpleObjectFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredFormalOnly() = DomainObject14.GetMostSimpleObjectConfiguredFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSimpleOnly() = DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredCaching() = DomainObject14.GetMostSimpleObjectConfiguredCaching (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredOverrides() = DomainObject14.GetMostSimpleObjectConfiguredOverrides (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithDateTimeKeyObject() = DomainObject14.GetWithDateTimeKeyObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetVerySimpleEagerObject() = DomainObject14.GetVerySimpleEagerObject (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetWithValueObject() = DomainObject14.GetWithValueObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithScalarsObject() = DomainObject14.GetWithScalarsObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithValueObjectUserAuth() = DomainObject14.GetWithValueObjectUserAuth (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeSimple() = DomainObject14.GetMostSimpleObjectWithDomainTypeSimple (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeFormal() = DomainObject14.GetMostSimpleObjectWithDomainTypeFormal (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileSimple() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileSimple (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileFormal() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileFormal (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetRedirectedObject() = DomainObject14.GetRedirectedObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObject() = DomainObject14.PutWithValueObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithScalarsObject() = DomainObject14.PutWithScalarsObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObject() = DomainObject14.PutWithReferenceObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectValidateOnly() = DomainObject14.PutWithReferenceObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithActionObject() = DomainObject14.GetWithActionObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithActionObjectSimpleOnly() = DomainObject14.GetWithActionObjectSimpleOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetWithActionObjectFormalOnly() = DomainObject14.GetWithActionObjectFormalOnly (api x.NakedObjectsContext)        
    [<Test>]
    member x.GetWithReferenceObject() = DomainObject14.GetWithReferenceObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithCollectionObjectFormalOnly() = DomainObject14.GetWithCollectionObjectFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvalidGetObject() = DomainObject14.InvalidGetObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject (api x.NakedObjectsContext) 
    [<Test>]    
    member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]    
    member x.GetObjectIgnoreWrongDomainType() = DomainObject14.GetObjectIgnoreWrongDomainType (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMissingArgsValidateOnly() = DomainObject14.PutWithValueObjectMissingArgsValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMalformedDateTimeArgs() = DomainObject14.PutWithValueObjectMalformedDateTimeArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectMalformedArgsValidateOnly() = DomainObject14.PutWithValueObjectMalformedArgsValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue (api x.NakedObjectsContext) 

    [<Test>]
    member x.PutWithReferenceObjectNotFoundArgsValue() = DomainObject14.PutWithReferenceObjectNotFoundArgsValue (api x.NakedObjectsContext) 

    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgs() = DomainObject14.PutWithReferenceObjectMalformedArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgsValidateOnly() = DomainObject14.PutWithReferenceObjectMalformedArgsValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectDisabledValue() = DomainObject14.PutWithValueObjectDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectDisabledValueValidateOnly() = DomainObject14.PutWithValueObjectDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValue() = DomainObject14.PutWithReferenceObjectDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValueValidateOnly() = DomainObject14.PutWithReferenceObjectDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvisibleValue() = DomainObject14.PutWithValueObjectInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValue() = DomainObject14.PutWithReferenceObjectInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvisibleValueValidateOnly() = DomainObject14.PutWithValueObjectInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueImmutableObject() = DomainObject14.PutWithValueImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceImmutableObject() = DomainObject14.PutWithReferenceImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueImmutableObjectValidateOnly() = DomainObject14.PutWithValueImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceImmutableObjectValidateOnly() = DomainObject14.PutWithReferenceImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsName() = DomainObject14.PutWithValueObjectInvalidArgsName (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsNameValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsNameValidateOnly (api x.NakedObjectsContext) 
    [<Test>]   
    member x.NotAcceptablePutObjectWrongMediaType() = DomainObject14.NotAcceptablePutObjectWrongMediaType (api x.NakedObjectsContext)  
    [<Test>]
    member x.PutWithValueInternalError() = DomainObject14.PutWithValueInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceInternalError() = DomainObject14.PutWithReferenceInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidation() = DomainObject14.PutWithValueObjectFailCrossValidation (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidationValidateOnly() = DomainObject14.PutWithValueObjectFailCrossValidationValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidation() = DomainObject14.PutWithReferenceObjectFailsCrossValidation (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidationValidateOnly() = DomainObject14.PutWithReferenceObjectFailsCrossValidationValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey (api x.NakedObjectsContext) 
    [<Test>]
    member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType (api x.NakedObjectsContext) 
    [<Test>] 
    [<Ignore>] // no longer fails no sure if an issue - seems no reason to make fail ? 
    member x.ObjectNotFoundAbstractType() = DomainObject14.ObjectNotFoundAbstractType (api x.NakedObjectsContext)  
    // view models 
    [<Test>]
    member x.GetMostSimpleViewModel() = DomainObject14.GetMostSimpleViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithValueViewModel() = DomainObject14.GetWithValueViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetWithReferenceViewModel() = DomainObject14.GetWithReferenceViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.GetWithNestedViewModel() = DomainObject14.GetWithNestedViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.PutWithReferenceViewModel() = DomainObject14.PutWithReferenceViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.PutWithNestedViewModel() = DomainObject14.PutWithNestedViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.PutWithValueViewModel() = DomainObject14.PutWithValueViewModel (api x.NakedObjectsContext)
    // DomainService15
    [<Test>] 
    member x.GetService() = DomainService15.GetService (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetContributorService() = DomainService15.GetContributorService (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetServiceFormalOnly() = DomainService15.GetServiceFormalOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetWithActionService() = DomainService15.GetWithActionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvalidGetService() = DomainService15.InvalidGetService (api x.NakedObjectsContext) 
    [<Test>]
    member x.NotFoundGetService() = DomainService15.NotFoundGetService (api x.NakedObjectsContext) 
    [<Test>]   
    member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType (api x.NakedObjectsContext) 
    // ObjectProperty16
    [<Test>]
    member x.GetValueProperty() = ObjectProperty16.GetValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetFileAttachmentProperty() = ObjectProperty16.GetFileAttachmentProperty (api x.NakedObjectsContext)
    [<Test>]
    member x.GetImageAttachmentProperty() = ObjectProperty16.GetImageAttachmentProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetFileAttachmentValue() = ObjectProperty16.GetFileAttachmentValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetAttachmentValueWrongMediaType() = ObjectProperty16.GetAttachmentValueWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetImageAttachmentValue() = ObjectProperty16.GetImageAttachmentValue (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetValuePropertyViewModel() = ObjectProperty16.GetValuePropertyViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetEnumValueProperty() = ObjectProperty16.GetEnumValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetValuePropertyUserAuth() = ObjectProperty16.GetValuePropertyUserAuth (api x.NakedObjectsContext)     
    [<Test>]
    member x.GetValuePropertyFormalOnly() = ObjectProperty16.GetValuePropertyFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetStringValueProperty() = ObjectProperty16.GetStringValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetBlobValueProperty() = ObjectProperty16.GetBlobValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetClobValueProperty() = ObjectProperty16.GetClobValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetChoicesValueProperty() = ObjectProperty16.GetChoicesValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetUserDisabledValueProperty() = ObjectProperty16.GetUserDisabledValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetUserDisabledValuePropertyAuthorised() = ObjectProperty16.GetUserDisabledValuePropertyAuthorised (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetAutoCompleteProperty() = ObjectProperty16.GetAutoCompleteProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeAutoComplete() = ObjectProperty16.InvokeAutoComplete (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeAutoCompleteErrorNoParm() = ObjectProperty16.InvokeAutoCompleteErrorNoParm (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeAutoCompleteErrorMalformedParm() = ObjectProperty16.InvokeAutoCompleteErrorMalformedParm (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeAutoCompleteErrorUnrecognisedParm() = ObjectProperty16.InvokeAutoCompleteErrorUnrecognisedParm (api x.NakedObjectsContext) 


    [<Test>]
    member x.InvokeConditionalChoicesReference() = ObjectProperty16.InvokeConditionalChoicesReference (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorMalformedParm (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorNoParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorNoParm (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorUnrecognisedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorUnrecognisedParm (api x.NakedObjectsContext) 



    [<Test>]
    member x.InvokeConditionalChoicesValue() = ObjectProperty16.InvokeConditionalChoicesValue (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMalformedParm (api x.NakedObjectsContext) 
    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMissingParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMissingParm (api x.NakedObjectsContext) 


    [<Test>]
    member x.GetReferencePropertyViewModel() = ObjectProperty16.GetReferencePropertyViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetChoicesReferenceProperty() = ObjectProperty16.GetChoicesReferenceProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty (api x.NakedObjectsContext)
    [<Test>]
    member x.GetUserHiddenValueProperty() = ObjectProperty16.GetUserHiddenValueProperty (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty (api x.NakedObjectsContext) 
    [<Test>]    
    member x.NotAcceptableGetPropertyWrongMediaType() = ObjectProperty16.NotAcceptableGetPropertyWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetPropertyAsCollection() = ObjectProperty16.GetPropertyAsCollection (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutDateTimeValuePropertySuccess() = ObjectProperty16.PutDateTimeValuePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutUserDisabledValuePropertySuccess() = ObjectProperty16.PutUserDisabledValuePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutClobPropertyBadRequest() = ObjectProperty16.PutClobPropertyBadRequest (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutBlobPropertyBadRequest() = ObjectProperty16.PutBlobPropertyBadRequest (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertySuccess() = ObjectProperty16.DeleteValuePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertySuccessValidateOnly() = ObjectProperty16.DeleteValuePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutNullValuePropertySuccess() = ObjectProperty16.PutNullValuePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutNullValuePropertySuccessValidateOnly() = ObjectProperty16.PutNullValuePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutReferencePropertySuccess() = ObjectProperty16.PutReferencePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutReferencePropertySuccessValidateOnly() = ObjectProperty16.PutReferencePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertySuccess() = ObjectProperty16.DeleteReferencePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertySuccessValidateOnly() = ObjectProperty16.DeleteReferencePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutNullReferencePropertySuccess() = ObjectProperty16.PutNullReferencePropertySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutNullReferencePropertySuccessValidateOnly() = ObjectProperty16.PutNullReferencePropertySuccessValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyMissingArgs() = ObjectProperty16.PutWithValuePropertyMissingArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgs() = ObjectProperty16.PutWithValuePropertyMalformedArgs (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidation() = ObjectProperty16.PutWithReferencePropertyFailCrossValidation (api x.NakedObjectsContext) 

    [<Test>]
    member x.PutWithReferencePropertyMalformedArgs() = ObjectProperty16.PutWithReferencePropertyMalformedArgs (api x.NakedObjectsContext) 

    [<Test>]
    member x.PutWithValuePropertyFailCrossValidation() = ObjectProperty16.PutWithValuePropertyFailCrossValidation (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValue() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyUserDisabledValue() = ObjectProperty16.PutWithValuePropertyUserDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValue() = ObjectProperty16.PutWithReferencePropertyInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObject() = ObjectProperty16.PutWithValuePropertyOnImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObject() = ObjectProperty16.PutWithReferencePropertyOnImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName (api x.NakedObjectsContext) 
    [<Test>]    
    member x.NotAcceptablePutPropertyWrongMediaType() = ObjectProperty16.NotAcceptablePutPropertyWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyMissingArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithReferencePropertyFailCrossValidationValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithValuePropertyFailCrossValidationValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithValuePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithReferencePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteValuePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteReferencePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObject() = ObjectProperty16.DeleteValuePropertyOnImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObject() = ObjectProperty16.DeleteReferencePropertyOnImmutableObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName (api x.NakedObjectsContext) 
    [<Test>]    
    member x.NotAcceptableDeletePropertyWrongMediaType() = ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.DeleteReferencePropertyInternalError() = ObjectProperty16.DeleteReferencePropertyInternalError (api x.NakedObjectsContext) 
    [<Test>]
    member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound (api x.NakedObjectsContext) 
    // ObjectCollection17
    [<Test>]
    member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionPropertyViewModel() = ObjectCollection17.GetCollectionPropertyViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionPropertyFormalOnly() = ObjectCollection17.GetCollectionPropertyFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionSetProperty() = ObjectCollection17.GetCollectionSetProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionSetPropertyFormalOnly() = ObjectCollection17.GetCollectionSetPropertyFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionSetPropertySimpleOnly() = ObjectCollection17.GetCollectionSetPropertySimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionValue() = ObjectCollection17.GetCollectionValue (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionProperty() = ObjectCollection17.AddToAndDeleteFromCollectionProperty (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyViewModel() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyViewModel (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionProperty() = ObjectCollection17.AddToAndDeleteFromSetCollectionProperty (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyConcurrencySuccess (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionPropertyConcurrencyFail() = ObjectCollection17.AddToCollectionPropertyConcurrencyFail (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.AddToCollectionPropertyMissingIfMatchHeader (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyConcurrencyFail() = ObjectCollection17.DeleteFromCollectionPropertyConcurrencyFail (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.DeleteFromCollectionPropertyMissingIfMatchHeader (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionPropertyValidateOnly() = ObjectCollection17.AddToCollectionPropertyValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyValidateOnly() = ObjectCollection17.DeleteFromCollectionPropertyValidateOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection (api x.NakedObjectsContext)  
    [<Test>]    
    member x.NotAcceptableGetCollectionWrongMediaType() = ObjectCollection17.NotAcceptableGetCollectionWrongMediaType (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetErrorValueCollection() = ObjectCollection17.GetErrorValueCollection (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionAsProperty() = ObjectCollection17.GetCollectionAsProperty (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionMissingArgs() = ObjectCollection17.AddToCollectionMissingArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgs() = ObjectCollection17.AddToCollectionMalformedArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgs() = ObjectCollection17.AddToCollectionInvalidArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionDisabledValue() = ObjectCollection17.AddToCollectionDisabledValue (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValue() = ObjectCollection17.AddToCollectionInvisibleValue (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionImmutableObject() = ObjectCollection17.AddToCollectionImmutableObject (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsName() = ObjectCollection17.AddToCollectionInvalidArgsName (api x.NakedObjectsContext) 
//    [<Test>]        
//    member x.NotAcceptableAddCollectionWrongMediaType() = ObjectCollection17.NotAcceptableAddCollectionWrongMediaType (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionMissingArgsValidateOnly() = ObjectCollection17.AddToCollectionMissingArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgsValidateOnly() = ObjectCollection17.AddToCollectionMalformedArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionDisabledValueValidateOnly() = ObjectCollection17.AddToCollectionDisabledValueValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValueValidateOnly() = ObjectCollection17.AddToCollectionInvisibleValueValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionImmutableObjectValidateOnly() = ObjectCollection17.AddToCollectionImmutableObjectValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsNameValidateOnly (api x.NakedObjectsContext)   
//    [<Test>]    
//    member x.AddToCollectionInternalError() = ObjectCollection17.AddToCollectionInternalError (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgs() = ObjectCollection17.DeleteFromCollectionMissingArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgs() = ObjectCollection17.DeleteFromCollectionMalformedArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgs() = ObjectCollection17.DeleteFromCollectionInvalidArgs (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValue() = ObjectCollection17.DeleteFromCollectionDisabledValue (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValue() = ObjectCollection17.DeleteFromCollectionInvisibleValue (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObject() = ObjectCollection17.DeleteFromCollectionImmutableObject (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsName() = ObjectCollection17.DeleteFromCollectionInvalidArgsName (api x.NakedObjectsContext) 
//    [<Test>]        
//    member x.NotAcceptableDeleteFromCollectionWrongMediaType() = ObjectCollection17.NotAcceptableDeleteFromCollectionWrongMediaType (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMissingArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMalformedArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValueValidateOnly() = ObjectCollection17.DeleteFromCollectionDisabledValueValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValueValidateOnly() = ObjectCollection17.DeleteFromCollectionInvisibleValueValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObjectValidateOnly() = ObjectCollection17.DeleteFromCollectionImmutableObjectValidateOnly (api x.NakedObjectsContext) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsNameValidateOnly (api x.NakedObjectsContext)  
//    [<Test>]    
//    member x.DeleteFromCollectionInternalError() = ObjectCollection17.DeleteFromCollectionInternalError (api x.NakedObjectsContext) 
    // ObjectAction18
    [<Test>]
    member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionContributedService() = ObjectAction18.GetActionContributedService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyViewModel() = ObjectAction18.GetActionPropertyViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetOverloadedActionPropertyObject() = ObjectAction18.GetOverloadedActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetOverloadedActionPropertyService() = ObjectAction18.GetOverloadedActionPropertyService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetOverloadedActionPropertyViewModel() = ObjectAction18.GetOverloadedActionPropertyViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyDateTimeViewModel() = ObjectAction18.GetActionPropertyDateTimeViewModel (api x.NakedObjectsContext)

    [<Test>]
    member x.GetActionPropertyCollectionViewModel() = ObjectAction18.GetActionPropertyCollectionViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyCollectionService() = ObjectAction18.GetActionPropertyCollectionService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyCollectionObject() = ObjectAction18.GetActionPropertyCollectionObject (api x.NakedObjectsContext) 

    [<Test>]
    member x.GetActionPropertyDateTimeService() = ObjectAction18.GetActionPropertyDateTimeService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyDateTimeObject() = ObjectAction18.GetActionPropertyDateTimeObject (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetUserDisabledActionPropertyObject() = ObjectAction18.GetUserDisabledActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetUserDisabledActionPropertyService() = ObjectAction18.GetUserDisabledActionPropertyService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetUserDisabledActionPropertyViewModel() = ObjectAction18.GetUserDisabledActionPropertyViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionPropertyQueryOnlyObject() = ObjectAction18.GetActionPropertyQueryOnlyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyQueryOnlyService() = ObjectAction18.GetActionPropertyQueryOnlyService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionPropertyQueryOnlyViewModel() = ObjectAction18.GetActionPropertyQueryOnlyViewModel (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionPropertyIdempotentObject() = ObjectAction18.GetActionPropertyIdempotentObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyIdempotentService() = ObjectAction18.GetActionPropertyIdempotentService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionPropertyIdempotentViewModel() = ObjectAction18.GetActionPropertyIdempotentViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionPropertyWithOptObject() = ObjectAction18.GetActionPropertyWithOptObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyWithOptService() = ObjectAction18.GetActionPropertyWithOptService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyWithOptViewModel() = ObjectAction18.GetActionPropertyWithOptViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyWithOptObjectSimpleOnly() = ObjectAction18.GetActionPropertyWithOptObjectSimpleOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyWithOptServiceSimpleOnly() = ObjectAction18.GetActionPropertyWithOptServiceSimpleOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionPropertyWithOptViewModelSimpleOnly() = ObjectAction18.GetActionPropertyWithOptViewModelSimpleOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionPropertyWithOptObjectFormalOnly() = ObjectAction18.GetActionPropertyWithOptObjectFormalOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyWithOptServiceFormalOnly() = ObjectAction18.GetActionPropertyWithOptServiceFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionPropertyWithOptViewModelFormalOnly() = ObjectAction18.GetActionPropertyWithOptViewModelFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionPropertyViewModelWithMediaType() = ObjectAction18.GetActionPropertyViewModelWithMediaType (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetScalarActionService() = ObjectAction18.GetScalarActionService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetScalarActionViewModel() = ObjectAction18.GetScalarActionViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionWithValueParmObject() = ObjectAction18.GetActionWithValueParmObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithValueParmService() = ObjectAction18.GetActionWithValueParmService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionWithValueParmViewModel() = ObjectAction18.GetActionWithValueParmViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesObject() = ObjectAction18.GetActionWithValueParmWithChoicesObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithValueParmWithChoicesService() = ObjectAction18.GetActionWithValueParmWithChoicesService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesViewModel() = ObjectAction18.GetActionWithValueParmWithChoicesViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetActionWithValueParmWithDefaultObject() = ObjectAction18.GetActionWithValueParmWithDefaultObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithValueParmWithDefaultService() = ObjectAction18.GetActionWithValueParmWithDefaultService (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetActionWithValueParmWithDefaultViewModel() = ObjectAction18.GetActionWithValueParmWithDefaultViewModel (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithReferenceParmViewModel() = ObjectAction18.GetActionWithReferenceParmViewModel (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesService() = ObjectAction18.GetActionWithReferenceParmWithChoicesService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesViewModel() = ObjectAction18.GetActionWithReferenceParmWithChoicesViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteObject() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteService() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteViewModel() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.InvokeParmWithAutoCompleteObject() = ObjectAction18.InvokeParmWithAutoCompleteObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithAutoCompleteService() = ObjectAction18.InvokeParmWithAutoCompleteService (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModel() = ObjectAction18.InvokeParmWithAutoCompleteViewModel (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorNoParm (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorNoParm (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorNoParm (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorMalformedParm (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorMalformedParm (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorMalformedParm (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm (api x.NakedObjectsContext) 



    [<Test>]
    member x.InvokeParmWithConditionalChoicesObject() = ObjectAction18.InvokeParmWithConditionalChoicesObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithConditionalChoicesService() = ObjectAction18.InvokeParmWithConditionalChoicesService (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeParmWithConditionalChoicesViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.InvokeParmWithConditionalChoicesObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesObjectErrorMalformedParm (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeParmWithConditionalChoicesServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesServiceErrorMalformedParm (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObject() = ObjectAction18.InvokeValueParmWithConditionalChoicesObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesService() = ObjectAction18.InvokeValueParmWithConditionalChoicesService (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultService() = ObjectAction18.GetActionWithReferenceParmWithDefaultService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultViewModel() = ObjectAction18.GetActionWithReferenceParmWithDefaultViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithChoicesAndDefaultViewModel() = ObjectAction18.GetActionWithChoicesAndDefaultViewModel (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetContributedActionOnContributee() = ObjectAction18.GetContributedActionOnContributee (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributeeOnBaseClass() = ObjectAction18.GetContributedActionOnContributeeBaseClass (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributeeWithRef() = ObjectAction18.GetContributedActionOnContributeeWithRef (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributeeWithValue() = ObjectAction18.GetContributedActionOnContributeeWithValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributer() = ObjectAction18.GetContributedActionOnContributer (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributerOnBaseClass() = ObjectAction18.GetContributedActionOnContributerBaseClass (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributerWithRef() = ObjectAction18.GetContributedActionOnContributerWithRef (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetContributedActionOnContributerWithValue() = ObjectAction18.GetContributedActionOnContributerWithValue (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvalidActionPropertyViewModel() = ObjectAction18.GetInvalidActionPropertyViewModel (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetNotFoundActionPropertyViewModel() = ObjectAction18.GetNotFoundActionPropertyViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetUserDisabledActionObject() = ObjectAction18.GetUserDisabledActionObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetUserDisabledActionService() = ObjectAction18.GetUserDisabledActionService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetUserDisabledActionViewModel() = ObjectAction18.GetUserDisabledActionViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetHiddenActionPropertyViewModel() = ObjectAction18.GetHiddenActionPropertyViewModel (api x.NakedObjectsContext)   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject (api x.NakedObjectsContext)   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeService() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeService (api x.NakedObjectsContext) 
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeViewModel() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetQueryActionObject() = ObjectAction18.GetQueryActionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetQueryActionService() = ObjectAction18.GetQueryActionService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetQueryActionViewModel() = ObjectAction18.GetQueryActionViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetQueryActionWithParmsObject() = ObjectAction18.GetQueryActionWithParmsObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetQueryActionWithParmsService() = ObjectAction18.GetQueryActionWithParmsService (api x.NakedObjectsContext)
    [<Test>]
    member x.GetQueryActionWithParmsViewModel() = ObjectAction18.GetQueryActionWithParmsViewModel (api x.NakedObjectsContext)
    [<Test>]
    member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionViewModel() = ObjectAction18.GetCollectionActionViewModel (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModel() = ObjectAction18.GetCollectionActionWithParmsViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsObjectFormalOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelFormalOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelSimpleOnly (api x.NakedObjectsContext)

    [<Test>]
    member x.ActionNotFound() = ObjectAction18.ActionNotFound (api x.NakedObjectsContext)     
    // ObjectActionInvoke19
    [<Test>]  
    member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModel (api x.NakedObjectsContext)
   
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectService() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectViewModel (api x.NakedObjectsContext)

    [<Test>]
    member x.PostInvokeActionContributedService() = ObjectActionInvoke19.PostInvokeActionContributedService (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnViewModelObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnViewModelService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnViewModelViewModel (api x.NakedObjectsContext)

    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectService() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectViewModel (api x.NakedObjectsContext)


    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectService() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectViewModel (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeActionReturnNullViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.PostInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnObjectService() = ObjectActionInvoke19.PutInvokeActionReturnObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.PutInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PutInvokeActionReturnViewModelObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnViewModelService() = ObjectActionInvoke19.PutInvokeActionReturnViewModelService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PutInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PutInvokeActionReturnViewModelViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.PutInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PutInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectViewModel (api x.NakedObjectsContext)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsContext)   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsContext)   

    [<Test>]
    member x.GetInvokeActionReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnObjectService() = ObjectActionInvoke19.GetInvokeActionReturnObjectService (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.GetInvokeActionReturnViewModelObject() = ObjectActionInvoke19.GetInvokeActionReturnViewModelObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnViewModelService() = ObjectActionInvoke19.GetInvokeActionReturnViewModelService (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.GetInvokeActionReturnViewModelViewModel (api x.NakedObjectsContext)    



    [<Test>]
    member x.GetInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnNullObjectService() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeContribActionReturnObject() = ObjectActionInvoke19.PostInvokeContribActionReturnObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithRefParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithRefParm (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithValueParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithValueParm (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectBaseClass() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectBaseClass (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelWithMediaType (api x.NakedObjectsContext)  
       
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsContext) 

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch (api x.NakedObjectsContext) 
      
    [<Test>]
    member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectFormalOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceFormalOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelFormalOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelValidateOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarViewModel (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionReturnNullScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnNullScalarService() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnNullScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarViewModel (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectFormalOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceFormalOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelFormalOnly (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelValidateOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.GetInvokeActionReturnQueryObject() = ObjectActionInvoke19.GetInvokeActionReturnQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnQueryService() = ObjectActionInvoke19.GetInvokeActionReturnQueryService (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.GetInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelValidateOnly (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly (api x.NakedObjectsContext)     


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly (api x.NakedObjectsContext)       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly (api x.NakedObjectsContext)       


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)    


    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsContext)    
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsContext)    
    
      
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectViewModel (api x.NakedObjectsContext)  
    
     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsContext)     
    
      
    [<Test>]
    member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject (api x.NakedObjectsContext)
    [<Test>]
    member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService (api x.NakedObjectsContext)  
    [<Test>]
    member x.NotFoundActionInvokeViewModel() = ObjectActionInvoke19.NotFoundActionInvokeViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject (api x.NakedObjectsContext)
    [<Test>]
    member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService (api x.NakedObjectsContext)   
    [<Test>]
    member x.HiddenActionInvokeViewModel() = ObjectActionInvoke19.HiddenActionInvokeViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetActionWithSideEffectsViewModel() = ObjectActionInvoke19.GetActionWithSideEffectsViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetActionWithIdempotentObject() = ObjectActionInvoke19.GetActionWithIdempotentObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetActionWithIdempotentService() = ObjectActionInvoke19.GetActionWithIdempotentService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetActionWithIdempotentViewModel() = ObjectActionInvoke19.GetActionWithIdempotentViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.PutActionWithQueryOnlyObject() = ObjectActionInvoke19.PutActionWithQueryOnlyObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PutActionWithQueryOnlyService() = ObjectActionInvoke19.PutActionWithQueryOnlyService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PutActionWithQueryOnlyViewModel() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeObject (api x.NakedObjectsContext)
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeService (api x.NakedObjectsContext)   
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService (api x.NakedObjectsContext)   
    [<Test>]
    member x.MissingParmsOnPostViewModel() = ObjectActionInvoke19.MissingParmsOnPostViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService (api x.NakedObjectsContext) 
    [<Test>]
    member x.DisabledActionPostInvokeViewModel() = ObjectActionInvoke19.DisabledActionPostInvokeViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.UserDisabledActionPostInvokeObject() = ObjectActionInvoke19.UserDisabledActionPostInvokeObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.UserDisabledActionPostInvokeService() = ObjectActionInvoke19.UserDisabledActionPostInvokeService (api x.NakedObjectsContext) 
    [<Test>]
    member x.UserDisabledActionPostInvokeViewModel() = ObjectActionInvoke19.UserDisabledActionPostInvokeViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject (api x.NakedObjectsContext)
    [<Test>]
    member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService (api x.NakedObjectsContext)  
    [<Test>]
    member x.NotFoundActionPostInvokeViewModel() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject (api x.NakedObjectsContext)
    [<Test>]
    member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService (api x.NakedObjectsContext)   
    [<Test>]
    member x.HiddenActionPostInvokeViewModel() = ObjectActionInvoke19.HiddenActionPostInvokeViewModel (api x.NakedObjectsContext)   


    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject (api x.NakedObjectsContext)
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService (api x.NakedObjectsContext)   
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.PostQueryActionWithErrorObject() = ObjectActionInvoke19.PostQueryActionWithErrorObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostQueryActionWithErrorService() = ObjectActionInvoke19.PostQueryActionWithErrorService (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostQueryActionWithErrorViewModel() = ObjectActionInvoke19.PostQueryActionWithErrorViewModel (api x.NakedObjectsContext)    


    [<Test>]
    member x.GetQueryActionWithErrorObject() = ObjectActionInvoke19.GetQueryActionWithErrorObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetQueryActionWithErrorService() = ObjectActionInvoke19.GetQueryActionWithErrorService (api x.NakedObjectsContext) 
    [<Test>]
    member x.GetQueryActionWithErrorViewModel() = ObjectActionInvoke19.GetQueryActionWithErrorViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.MalformedFormalParmsOnPostQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService (api x.NakedObjectsContext) 
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModel (api x.NakedObjectsContext) 


    
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryObject() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryService() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryService (api x.NakedObjectsContext) 
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryViewModel (api x.NakedObjectsContext) 

     
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryObject() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryService() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryService (api x.NakedObjectsContext) 
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryViewModel (api x.NakedObjectsContext) 

    [<Test>]
    member x.InvalidFormalParmsOnPostQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.MissingParmsOnGetQueryObject() = ObjectActionInvoke19.MissingParmsOnGetQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MissingParmsOnGetQueryService() = ObjectActionInvoke19.MissingParmsOnGetQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.MissingParmsOnGetQueryViewModel() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryService() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.MalformedFormalParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryService() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryService (api x.NakedObjectsContext)    
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModel (api x.NakedObjectsContext)    


    [<Test>]
    member x.InvalidFormalParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryService (api x.NakedObjectsContext)  
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.DisabledActionInvokeQueryObject() = ObjectActionInvoke19.DisabledActionInvokeQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.DisabledActionInvokeQueryService() = ObjectActionInvoke19.DisabledActionInvokeQueryService (api x.NakedObjectsContext)    
    [<Test>]
    member x.DisabledActionInvokeQueryViewModel() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModel (api x.NakedObjectsContext)    


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly (api x.NakedObjectsContext)        
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly (api x.NakedObjectsContext)        
    
     
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly (api x.NakedObjectsContext)   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalService (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly (api x.NakedObjectsContext)  
    
          
    [<Test>]
    member x.PostInvokeActionReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionReturnQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnQueryService() = ObjectActionInvoke19.PostInvokeActionReturnQueryService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModel (api x.NakedObjectsContext)  


    [<Test>]
    member x.PostInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModelValidateOnly (api x.NakedObjectsContext)  


    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostInvokeOverloadedActionObject() = ObjectActionInvoke19.PostInvokeOverloadedActionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeOverloadedActionService() = ObjectActionInvoke19.PostInvokeOverloadedActionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeOverloadedActionViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionViewModel (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly (api x.NakedObjectsContext)  

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModel (api x.NakedObjectsContext)   
       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly (api x.NakedObjectsContext)     
            
    [<Test>]
    member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostCollectionActionWithErrorService() = ObjectActionInvoke19.PostCollectionActionWithErrorService (api x.NakedObjectsContext)  
    [<Test>]
    member x.PostCollectionActionWithErrorViewModel() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModel (api x.NakedObjectsContext)  

    [<Test>]
    member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService (api x.NakedObjectsContext)    
    [<Test>]
    member x.MissingParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionService() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService (api x.NakedObjectsContext)  
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModel (api x.NakedObjectsContext)  

    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionService() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService (api x.NakedObjectsContext)    
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.DisabledActionInvokeCollectionObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionObject (api x.NakedObjectsContext) 
    [<Test>]
    member x.DisabledActionInvokeCollectionService() = ObjectActionInvoke19.DisabledActionInvokeCollectionService (api x.NakedObjectsContext) 
    [<Test>]
    member x.DisabledActionInvokeCollectionViewModel() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModel (api x.NakedObjectsContext) 

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalService (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel (api x.NakedObjectsContext)    

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsContext)       
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsContext)       
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalService (api x.NakedObjectsContext)      
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel (api x.NakedObjectsContext)      
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectFormalOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceFormalOnly (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelFormalOnly (api x.NakedObjectsContext)    
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly (api x.NakedObjectsContext)    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly (api x.NakedObjectsContext)    
    
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService (api x.NakedObjectsContext)     
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel (api x.NakedObjectsContext)     
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly (api x.NakedObjectsContext)             
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly (api x.NakedObjectsContext)             
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly (api x.NakedObjectsContext)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly (api x.NakedObjectsContext)           
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly (api x.NakedObjectsContext)           
    
    [<Test>]
    member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.PostQueryActionWithValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithValidateFailObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostQueryActionWithValidateFailService() = ObjectActionInvoke19.PostQueryActionWithValidateFailService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostQueryActionWithValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel (api x.NakedObjectsContext)   

    [<Test>]
    member x.PostQueryActionWithCrossValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject (api x.NakedObjectsContext)
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailService() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService (api x.NakedObjectsContext)   
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel (api x.NakedObjectsContext) 


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService (api x.NakedObjectsContext)    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel (api x.NakedObjectsContext)    
    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject (api x.NakedObjectsContext)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService (api x.NakedObjectsContext)   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel (api x.NakedObjectsContext)   
    
    [<Test>]
    member x.MissingParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MissingParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>] 
    member x.MissingParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsContext)   
    
    [<Test>] 
    member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.MissingParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsContext)    

    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsContext)   
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsContext)         
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsContext)         
    
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsContext)      
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsContext)      
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsContext)     
    
    [<Test>] 
    member x.DisabledActionInvokeQueryObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.DisabledActionInvokeQueryServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.DisabledActionInvokeQueryViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.DisabledActionInvokeCollectionViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly (api x.NakedObjectsContext) 
    
    [<Test>] 
    member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly (api x.NakedObjectsContext)      
    [<Test>] 
    member x.NotFoundActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly (api x.NakedObjectsContext)      
    
    [<Test>] 
    member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.HiddenActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>] 
    member x.GetActionWithSideEffectsViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly (api x.NakedObjectsContext)   
    
    [<Test>] 
    member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.GetActionWithIdempotentViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.PutActionWithQueryOnlyViewModelValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.GetQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>] 
    member x.GetQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly (api x.NakedObjectsContext)     
    
    [<Test>] 
    member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.PostCollectionActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>] 
    member x.MissingParmsOnPostViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly (api x.NakedObjectsContext)     
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly (api x.NakedObjectsContext)     
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly (api x.NakedObjectsContext)       
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly (api x.NakedObjectsContext)       
    
    [<Test>] 
    member x.InvalidUrlOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.InvalidUrlOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly (api x.NakedObjectsContext)   
    [<Test>] 
    member x.InvalidUrlOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly (api x.NakedObjectsContext)   
    
    
    [<Test>] 
    member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly (api x.NakedObjectsContext)  
    [<Test>] 
    member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>] 
    member x.DisabledActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly (api x.NakedObjectsContext)  
    
    [<Test>] 
    member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.NotFoundActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.HiddenActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly (api x.NakedObjectsContext)     
    [<Test>] 
    member x.PostQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly (api x.NakedObjectsContext)     
    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly (api x.NakedObjectsContext)    
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly (api x.NakedObjectsContext)  
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsContext)  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsContext)  
    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsContext) 
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsContext)    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsContext)    
    

    // DomainType21
    [<Test>] 
    member x.GetMostSimpleObjectType() = DomainType21.GetMostSimpleObjectType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetWithActionObjectType() = DomainType21.GetWithActionObjectType (api x.NakedObjectsContext)
    [<Test>]
    member x.GetWithActionServiceType() = DomainType21.GetWithActionServiceType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetWithReferenceObjectType() = DomainType21.GetWithReferenceObjectType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetWithValueObjectType() = DomainType21.GetWithValueObjectType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetWithCollectionObjectType() = DomainType21.GetWithCollectionObjectType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetPredefinedDomainTypes() = DomainType21.GetPredefinedDomainTypes (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundPredefinedDomainTypes() = DomainType21.NotFoundPredefinedDomainTypes (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundGetMostSimpleObjectType() = DomainType21.NotFoundGetMostSimpleObjectType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableGetMostSimpleObjectType() = DomainType21.NotAcceptableGetMostSimpleObjectType (api x.NakedObjectsContext)
    // DomainProperty22
    [<Test>] 
    member x.GetValuePropertyType() = DomainProperty22.GetValuePropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetReferencePropertyType() = DomainProperty22.GetReferencePropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetValueStringPropertyType() = DomainProperty22.GetValueStringPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetValueDateTimePropertyType() = DomainProperty22.GetValueDateTimePropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundPropertyType() = DomainProperty22.NotFoundPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypePropertyType() = DomainProperty22.NotFoundTypePropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableGetValuePropertyType() = DomainProperty22.NotAcceptableGetValuePropertyType (api x.NakedObjectsContext)
    // DomainCollection23
    [<Test>] 
    member x.GetCollectionPropertyType() = DomainCollection23.GetCollectionPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetSetCollectionPropertyType() = DomainCollection23.GetSetCollectionPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetCollectionPropertyTypeWithDescription() = DomainCollection23.GetCollectionPropertyTypeWithDescription (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeCollectionPropertyType() = DomainCollection23.NotFoundTypeCollectionPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundCollectionPropertyType() = DomainCollection23.NotFoundCollectionPropertyType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableGetCollectionPropertyType() = DomainCollection23.NotAcceptableGetCollectionPropertyType (api x.NakedObjectsContext)
    // DomainAction24
    [<Test>] 
    member x.GetActionTypeObjectNoParmsScalar() = DomainAction24.GetActionTypeObjectNoParmsScalar (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsScalar() = DomainAction24.GetActionTypeServiceNoParmsScalar (api x.NakedObjectsContext)

    [<Test>] 
    member x.GetOverloadedActionTypeObjectNoParmsScalar() = DomainAction24.GetOverloadedActionTypeObjectNoParmsScalar (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetOverloadedActionTypeServiceNoParmsScalar() = DomainAction24.GetOverloadedActionTypeServiceNoParmsScalar (api x.NakedObjectsContext)


    [<Test>] 
    member x.GetActionTypeObjectNoParmsVoid() = DomainAction24.GetActionTypeObjectNoParmsVoid (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsVoid() = DomainAction24.GetActionTypeServiceNoParmsVoid (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectNoParmsCollection() = DomainAction24.GetActionTypeObjectNoParmsCollection (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsCollection() = DomainAction24.GetActionTypeServiceNoParmsCollection (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectParmsScalar() = DomainAction24.GetActionTypeObjectParmsScalar (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceParmsScalar() = DomainAction24.GetActionTypeServiceParmsScalar (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectParmsVoid() = DomainAction24.GetActionTypeObjectParmsVoid (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceParmsVoid() = DomainAction24.GetActionTypeServiceParmsVoid (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectParmsCollection() = DomainAction24.GetActionTypeObjectParmsCollection (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeServiceParmsCollection() = DomainAction24.GetActionTypeServiceParmsCollection (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributee() = DomainAction24.GetActionTypeObjectContributedOnContributee (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributer() = DomainAction24.GetActionTypeObjectContributedOnContributer (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeActionType() = DomainAction24.NotFoundTypeActionType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundActionTypeObject() = DomainAction24.NotFoundActionTypeObject (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundActionTypeService() = DomainAction24.NotFoundActionTypeService (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableActionType() = DomainAction24.NotAcceptableActionType (api x.NakedObjectsContext)
    // DomainActionParameter25
    [<Test>] 
    member x.GetActionParameterTypeInt() = DomainActionParameter25.GetActionParameterTypeInt (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionParameterTypeString() = DomainActionParameter25.GetActionParameterTypeString (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetOverloadedActionParameterTypeString() = DomainActionParameter25.GetOverloadedActionParameterTypeString (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionParameterTypeDateTime() = DomainActionParameter25.GetActionParameterTypeDateTime (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionParameterTypeReference() = DomainActionParameter25.GetActionParameterTypeReference (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetActionParameterTypeStringOptional() = DomainActionParameter25.GetActionParameterTypeStringOptional (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundType() = DomainActionParameter25.NotFoundType (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundAction() = DomainActionParameter25.NotFoundAction (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundParm() = DomainActionParameter25.NotFoundParm (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableActionParameterType() = DomainActionParameter25.NotAcceptableActionParameterType (api x.NakedObjectsContext)    
    // DomainTypeActionInvoke26
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms (api x.NakedObjectsContext)
    [<Test>] 
    member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf (api x.NakedObjectsContext)
    [<Test>] 
    member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf (api x.NakedObjectsContext)
    [<Test>] 
    member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf (api x.NakedObjectsContext)
    [<Test>] 
    member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf (api x.NakedObjectsContext)
end
