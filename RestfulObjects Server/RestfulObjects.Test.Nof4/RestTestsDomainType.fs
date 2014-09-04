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
        x.Fixtures.InstallFixtures(x.NakedObjectsFramework.ObjectPersistor, null)
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
    member x.GetHomePage() = HomePage5.GetHomePage (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetHomePageSimple() = HomePage5.GetHomePageSimple (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetHomePageFormal() = HomePage5.GetHomePageFormal (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetHomePageWithMediaType() = HomePage5.GetHomePageWithMediaType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableGetHomePage() = HomePage5.NotAcceptableGetHomePage (api x.NakedObjectsFramework)
    [<Test>] 
    member x.InvalidDomainModelGetHomePage() = HomePage5.InvalidDomainModelGetHomePage (api x.NakedObjectsFramework)
    // User6
    [<Test>] 
    member x.GetUser() = User6.GetUser (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetUserWithMediaType() = User6.GetUserWithMediaType (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser (api x.NakedObjectsFramework)
    // DomainServices7
    [<Test>] 
    member x.GetDomainServices() = DomainServices7.GetDomainServices (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetDomainServicesFormal() = DomainServices7.GetDomainServicesFormal (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaType (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices (api x.NakedObjectsFramework) 
    // Version8
    [<Test>] 
    member x.GetVersion() = Version8.GetVersion (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion (api x.NakedObjectsFramework) 
    //Objects9
    [<Test>]
    member x.GetMostSimpleTransientObject() = Objects9.GetMostSimpleTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleTransientObjectSimpleOnly() = Objects9.GetMostSimpleTransientObjectSimpleOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleTransientObjectFormalOnly() = Objects9.GetMostSimpleTransientObjectFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObject() = Objects9.PersistMostSimpleTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectSimpleOnly() = Objects9.PersistMostSimpleTransientObjectSimpleOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectFormalOnly() = Objects9.PersistMostSimpleTransientObjectFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectValidateOnly() = Objects9.PersistMostSimpleTransientObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithValueTransientObject() = Objects9.GetWithValueTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithReferenceTransientObject() = Objects9.GetWithReferenceTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithCollectionTransientObject() = Objects9.GetWithCollectionTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectFormalOnly() = Objects9.PersistWithValueTransientObjectFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnly() = Objects9.PersistWithValueTransientObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnly() = Objects9.PersistWithReferenceTransientObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnly() = Objects9.PersistWithCollectionTransientObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectValidateOnlyFail() = Objects9.PersistWithReferenceTransientObjectValidateOnlyFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectValidateOnlyFail() = Objects9.PersistWithCollectionTransientObjectValidateOnlyFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlySimpleOnlyFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail() = Objects9.PersistWithValueTransientObjectValidateOnlyFormalOnlyFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithValueTransientObjectFailInvalid() = Objects9.PersistWithValueTransientObjectFailInvalid (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithReferenceTransientObjectFailInvalid() = Objects9.PersistWithReferenceTransientObjectFailInvalid (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgs() = Objects9.PersistMostSimpleTransientObjectMissingArgs (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingArgsValidateOnly() = Objects9.PersistMostSimpleTransientObjectMissingArgsValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistMostSimpleTransientObjectMissingMemberArgs() = Objects9.PersistMostSimpleTransientObjectMissingMemberArgs (api x.NakedObjectsFramework)  
    [<Test>]    
    member x.PersistMostSimpleTransientObjectNullDomainType() = Objects9.PersistMostSimpleTransientObjectNullDomainType (api x.NakedObjectsFramework)  
    [<Test>]   
    member x.PersistMostSimpleTransientObjectEmptyDomainType() = Objects9.PersistMostSimpleTransientObjectEmptyDomainType (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PersistMostSimpleTransientObjectMalformedMemberArgs() = Objects9.PersistMostSimpleTransientObjectMalformedMemberArgs (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PersistNoKeyTransientObject() = Objects9.PersistNoKeyTransientObject (api x.NakedObjectsFramework)
    // Error10
    [<Test>]
    member x.Error() = Error10.Error (api x.NakedObjectsFramework) 
    [<Test>]
    member x.NotAcceptableError() = Error10.NotAcceptableError (api x.NakedObjectsFramework) 
    // DomainObject14
    [<Test>]
    member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithAttachmentsObject() = DomainObject14.GetWithAttachmentsObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSelectable() = DomainObject14.GetMostSimpleObjectConfiguredSelectable (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredNone() = DomainObject14.GetMostSimpleObjectConfiguredNone (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleObjectFormalOnly() = DomainObject14.GetMostSimpleObjectFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredFormalOnly() = DomainObject14.GetMostSimpleObjectConfiguredFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSimpleOnly() = DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredCaching() = DomainObject14.GetMostSimpleObjectConfiguredCaching (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredOverrides() = DomainObject14.GetMostSimpleObjectConfiguredOverrides (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithDateTimeKeyObject() = DomainObject14.GetWithDateTimeKeyObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetVerySimpleEagerObject() = DomainObject14.GetVerySimpleEagerObject (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetWithValueObject() = DomainObject14.GetWithValueObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithScalarsObject() = DomainObject14.GetWithScalarsObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithValueObjectUserAuth() = DomainObject14.GetWithValueObjectUserAuth (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeSimple() = DomainObject14.GetMostSimpleObjectWithDomainTypeSimple (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetMostSimpleObjectWithDomainTypeFormal() = DomainObject14.GetMostSimpleObjectWithDomainTypeFormal (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileSimple() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileSimple (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithValueObjectWithDomainTypeNoProfileFormal() = DomainObject14.GetWithValueObjectWithDomainTypeNoProfileFormal (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetRedirectedObject() = DomainObject14.GetRedirectedObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObject() = DomainObject14.PutWithValueObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithScalarsObject() = DomainObject14.PutWithScalarsObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutWithValueObjectConcurrencyFail() = DomainObject14.PutWithValueObjectConcurrencyFail (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMissingIfMatch() = DomainObject14.PutWithValueObjectMissingIfMatch (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObject() = DomainObject14.PutWithReferenceObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectValidateOnly() = DomainObject14.PutWithReferenceObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithActionObject() = DomainObject14.GetWithActionObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithActionObjectSimpleOnly() = DomainObject14.GetWithActionObjectSimpleOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetWithActionObjectFormalOnly() = DomainObject14.GetWithActionObjectFormalOnly (api x.NakedObjectsFramework)        
    [<Test>]
    member x.GetWithReferenceObject() = DomainObject14.GetWithReferenceObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithCollectionObjectFormalOnly() = DomainObject14.GetWithCollectionObjectFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvalidGetObject() = DomainObject14.InvalidGetObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject (api x.NakedObjectsFramework) 
    [<Test>]    
    member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]    
    member x.GetObjectIgnoreWrongDomainType() = DomainObject14.GetObjectIgnoreWrongDomainType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMissingArgsValidateOnly() = DomainObject14.PutWithValueObjectMissingArgsValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMalformedDateTimeArgs() = DomainObject14.PutWithValueObjectMalformedDateTimeArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectMalformedArgsValidateOnly() = DomainObject14.PutWithValueObjectMalformedArgsValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue (api x.NakedObjectsFramework) 

    [<Test>]
    member x.PutWithReferenceObjectNotFoundArgsValue() = DomainObject14.PutWithReferenceObjectNotFoundArgsValue (api x.NakedObjectsFramework) 

    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgs() = DomainObject14.PutWithReferenceObjectMalformedArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectMalformedArgsValidateOnly() = DomainObject14.PutWithReferenceObjectMalformedArgsValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectDisabledValue() = DomainObject14.PutWithValueObjectDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectDisabledValueValidateOnly() = DomainObject14.PutWithValueObjectDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValue() = DomainObject14.PutWithReferenceObjectDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectDisabledValueValidateOnly() = DomainObject14.PutWithReferenceObjectDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvisibleValue() = DomainObject14.PutWithValueObjectInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValue() = DomainObject14.PutWithReferenceObjectInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvisibleValueValidateOnly() = DomainObject14.PutWithValueObjectInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectInvisibleValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueImmutableObject() = DomainObject14.PutWithValueImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceImmutableObject() = DomainObject14.PutWithReferenceImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueImmutableObjectValidateOnly() = DomainObject14.PutWithValueImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceImmutableObjectValidateOnly() = DomainObject14.PutWithReferenceImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsName() = DomainObject14.PutWithValueObjectInvalidArgsName (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectInvalidArgsNameValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsNameValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]   
    member x.NotAcceptablePutObjectWrongMediaType() = DomainObject14.NotAcceptablePutObjectWrongMediaType (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PutWithValueInternalError() = DomainObject14.PutWithValueInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceInternalError() = DomainObject14.PutWithReferenceInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidation() = DomainObject14.PutWithValueObjectFailCrossValidation (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValueObjectFailCrossValidationValidateOnly() = DomainObject14.PutWithValueObjectFailCrossValidationValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidation() = DomainObject14.PutWithReferenceObjectFailsCrossValidation (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferenceObjectFailsCrossValidationValidateOnly() = DomainObject14.PutWithReferenceObjectFailsCrossValidationValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey (api x.NakedObjectsFramework) 
    [<Test>]
    member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType (api x.NakedObjectsFramework) 
    [<Test>] 
    [<Ignore>] // no longer fails no sure if an issue - seems no reason to make fail ? 
    member x.ObjectNotFoundAbstractType() = DomainObject14.ObjectNotFoundAbstractType (api x.NakedObjectsFramework)  
    // view models 
    [<Test>]
    member x.GetMostSimpleViewModel() = DomainObject14.GetMostSimpleViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithValueViewModel() = DomainObject14.GetWithValueViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetWithReferenceViewModel() = DomainObject14.GetWithReferenceViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetWithNestedViewModel() = DomainObject14.GetWithNestedViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutWithReferenceViewModel() = DomainObject14.PutWithReferenceViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutWithNestedViewModel() = DomainObject14.PutWithNestedViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutWithValueViewModel() = DomainObject14.PutWithValueViewModel (api x.NakedObjectsFramework)
    // DomainService15
    [<Test>] 
    member x.GetService() = DomainService15.GetService (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetContributorService() = DomainService15.GetContributorService (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetServiceFormalOnly() = DomainService15.GetServiceFormalOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetWithActionService() = DomainService15.GetWithActionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvalidGetService() = DomainService15.InvalidGetService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.NotFoundGetService() = DomainService15.NotFoundGetService (api x.NakedObjectsFramework) 
    [<Test>]   
    member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType (api x.NakedObjectsFramework) 
    // ObjectProperty16
    [<Test>]
    member x.GetValueProperty() = ObjectProperty16.GetValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetFileAttachmentProperty() = ObjectProperty16.GetFileAttachmentProperty (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetImageAttachmentProperty() = ObjectProperty16.GetImageAttachmentProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetFileAttachmentValue() = ObjectProperty16.GetFileAttachmentValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetAttachmentValueWrongMediaType() = ObjectProperty16.GetAttachmentValueWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetImageAttachmentValue() = ObjectProperty16.GetImageAttachmentValue (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetValuePropertyViewModel() = ObjectProperty16.GetValuePropertyViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetEnumValueProperty() = ObjectProperty16.GetEnumValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetValuePropertyUserAuth() = ObjectProperty16.GetValuePropertyUserAuth (api x.NakedObjectsFramework)     
    [<Test>]
    member x.GetValuePropertyFormalOnly() = ObjectProperty16.GetValuePropertyFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetStringValueProperty() = ObjectProperty16.GetStringValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetBlobValueProperty() = ObjectProperty16.GetBlobValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetClobValueProperty() = ObjectProperty16.GetClobValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetChoicesValueProperty() = ObjectProperty16.GetChoicesValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetUserDisabledValueProperty() = ObjectProperty16.GetUserDisabledValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetUserDisabledValuePropertyAuthorised() = ObjectProperty16.GetUserDisabledValuePropertyAuthorised (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetAutoCompleteProperty() = ObjectProperty16.GetAutoCompleteProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeAutoComplete() = ObjectProperty16.InvokeAutoComplete (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeAutoCompleteErrorNoParm() = ObjectProperty16.InvokeAutoCompleteErrorNoParm (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeAutoCompleteErrorMalformedParm() = ObjectProperty16.InvokeAutoCompleteErrorMalformedParm (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeAutoCompleteErrorUnrecognisedParm() = ObjectProperty16.InvokeAutoCompleteErrorUnrecognisedParm (api x.NakedObjectsFramework) 


    [<Test>]
    member x.InvokeConditionalChoicesReference() = ObjectProperty16.InvokeConditionalChoicesReference (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorMalformedParm (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorNoParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorNoParm (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeConditionalChoicesReferenceErrorUnrecognisedParm() = ObjectProperty16.InvokeConditionalChoicesReferenceErrorUnrecognisedParm (api x.NakedObjectsFramework) 



    [<Test>]
    member x.InvokeConditionalChoicesValue() = ObjectProperty16.InvokeConditionalChoicesValue (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMalformedParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMalformedParm (api x.NakedObjectsFramework) 
    [<Test>]
    member x.InvokeConditionalChoicesValueErrorMissingParm() = ObjectProperty16.InvokeConditionalChoicesValueErrorMissingParm (api x.NakedObjectsFramework) 


    [<Test>]
    member x.GetReferencePropertyViewModel() = ObjectProperty16.GetReferencePropertyViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetChoicesReferenceProperty() = ObjectProperty16.GetChoicesReferenceProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetUserHiddenValueProperty() = ObjectProperty16.GetUserHiddenValueProperty (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty (api x.NakedObjectsFramework) 
    [<Test>]    
    member x.NotAcceptableGetPropertyWrongMediaType() = ObjectProperty16.NotAcceptableGetPropertyWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetPropertyAsCollection() = ObjectProperty16.GetPropertyAsCollection (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutDateTimeValuePropertySuccess() = ObjectProperty16.PutDateTimeValuePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutUserDisabledValuePropertySuccess() = ObjectProperty16.PutUserDisabledValuePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutClobPropertyBadRequest() = ObjectProperty16.PutClobPropertyBadRequest (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutBlobPropertyBadRequest() = ObjectProperty16.PutBlobPropertyBadRequest (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertySuccess() = ObjectProperty16.DeleteValuePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertySuccessValidateOnly() = ObjectProperty16.DeleteValuePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutNullValuePropertySuccess() = ObjectProperty16.PutNullValuePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutNullValuePropertySuccessValidateOnly() = ObjectProperty16.PutNullValuePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutReferencePropertySuccess() = ObjectProperty16.PutReferencePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutReferencePropertySuccessValidateOnly() = ObjectProperty16.PutReferencePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertySuccess() = ObjectProperty16.DeleteReferencePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertySuccessValidateOnly() = ObjectProperty16.DeleteReferencePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutNullReferencePropertySuccess() = ObjectProperty16.PutNullReferencePropertySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutNullReferencePropertySuccessValidateOnly() = ObjectProperty16.PutNullReferencePropertySuccessValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyMissingArgs() = ObjectProperty16.PutWithValuePropertyMissingArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgs() = ObjectProperty16.PutWithValuePropertyMalformedArgs (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidation() = ObjectProperty16.PutWithReferencePropertyFailCrossValidation (api x.NakedObjectsFramework) 

    [<Test>]
    member x.PutWithReferencePropertyMalformedArgs() = ObjectProperty16.PutWithReferencePropertyMalformedArgs (api x.NakedObjectsFramework) 

    [<Test>]
    member x.PutWithValuePropertyFailCrossValidation() = ObjectProperty16.PutWithValuePropertyFailCrossValidation (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValue() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyUserDisabledValue() = ObjectProperty16.PutWithValuePropertyUserDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValue() = ObjectProperty16.PutWithReferencePropertyInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObject() = ObjectProperty16.PutWithValuePropertyOnImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObject() = ObjectProperty16.PutWithReferencePropertyOnImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName (api x.NakedObjectsFramework) 
    [<Test>]    
    member x.NotAcceptablePutPropertyWrongMediaType() = ObjectProperty16.NotAcceptablePutPropertyWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyMissingArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithReferencePropertyFailCrossValidationValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyFailCrossValidationValidateOnly() = ObjectProperty16.PutWithValuePropertyFailCrossValidationValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithValuePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.PutWithReferencePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteValuePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObjectValidateOnly() = ObjectProperty16.DeleteReferencePropertyOnImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyOnImmutableObject() = ObjectProperty16.DeleteValuePropertyOnImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyOnImmutableObject() = ObjectProperty16.DeleteReferencePropertyOnImmutableObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName (api x.NakedObjectsFramework) 
    [<Test>]    
    member x.NotAcceptableDeletePropertyWrongMediaType() = ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DeleteReferencePropertyInternalError() = ObjectProperty16.DeleteReferencePropertyInternalError (api x.NakedObjectsFramework) 
    [<Test>]
    member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound (api x.NakedObjectsFramework) 
    // ObjectCollection17
    [<Test>]
    member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionPropertyViewModel() = ObjectCollection17.GetCollectionPropertyViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionPropertyFormalOnly() = ObjectCollection17.GetCollectionPropertyFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionSetProperty() = ObjectCollection17.GetCollectionSetProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionSetPropertyFormalOnly() = ObjectCollection17.GetCollectionSetPropertyFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionSetPropertySimpleOnly() = ObjectCollection17.GetCollectionSetPropertySimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionValue() = ObjectCollection17.GetCollectionValue (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionProperty() = ObjectCollection17.AddToAndDeleteFromCollectionProperty (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyViewModel() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyViewModel (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionProperty() = ObjectCollection17.AddToAndDeleteFromSetCollectionProperty (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToAndDeleteFromCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyConcurrencySuccess (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionPropertyConcurrencyFail() = ObjectCollection17.AddToCollectionPropertyConcurrencyFail (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.AddToCollectionPropertyMissingIfMatchHeader (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyConcurrencyFail() = ObjectCollection17.DeleteFromCollectionPropertyConcurrencyFail (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.DeleteFromCollectionPropertyMissingIfMatchHeader (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionPropertyValidateOnly() = ObjectCollection17.AddToCollectionPropertyValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionPropertyValidateOnly() = ObjectCollection17.DeleteFromCollectionPropertyValidateOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection (api x.NakedObjectsFramework)  
    [<Test>]    
    member x.NotAcceptableGetCollectionWrongMediaType() = ObjectCollection17.NotAcceptableGetCollectionWrongMediaType (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetErrorValueCollection() = ObjectCollection17.GetErrorValueCollection (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionAsProperty() = ObjectCollection17.GetCollectionAsProperty (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionMissingArgs() = ObjectCollection17.AddToCollectionMissingArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgs() = ObjectCollection17.AddToCollectionMalformedArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgs() = ObjectCollection17.AddToCollectionInvalidArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionDisabledValue() = ObjectCollection17.AddToCollectionDisabledValue (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValue() = ObjectCollection17.AddToCollectionInvisibleValue (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionImmutableObject() = ObjectCollection17.AddToCollectionImmutableObject (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsName() = ObjectCollection17.AddToCollectionInvalidArgsName (api x.NakedObjectsFramework) 
//    [<Test>]        
//    member x.NotAcceptableAddCollectionWrongMediaType() = ObjectCollection17.NotAcceptableAddCollectionWrongMediaType (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionMissingArgsValidateOnly() = ObjectCollection17.AddToCollectionMissingArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionMalformedArgsValidateOnly() = ObjectCollection17.AddToCollectionMalformedArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionDisabledValueValidateOnly() = ObjectCollection17.AddToCollectionDisabledValueValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvisibleValueValidateOnly() = ObjectCollection17.AddToCollectionInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionImmutableObjectValidateOnly() = ObjectCollection17.AddToCollectionImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.AddToCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsNameValidateOnly (api x.NakedObjectsFramework)   
//    [<Test>]    
//    member x.AddToCollectionInternalError() = ObjectCollection17.AddToCollectionInternalError (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgs() = ObjectCollection17.DeleteFromCollectionMissingArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgs() = ObjectCollection17.DeleteFromCollectionMalformedArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgs() = ObjectCollection17.DeleteFromCollectionInvalidArgs (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValue() = ObjectCollection17.DeleteFromCollectionDisabledValue (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValue() = ObjectCollection17.DeleteFromCollectionInvisibleValue (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObject() = ObjectCollection17.DeleteFromCollectionImmutableObject (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsName() = ObjectCollection17.DeleteFromCollectionInvalidArgsName (api x.NakedObjectsFramework) 
//    [<Test>]        
//    member x.NotAcceptableDeleteFromCollectionWrongMediaType() = ObjectCollection17.NotAcceptableDeleteFromCollectionWrongMediaType (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionMissingArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMissingArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionMalformedArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMalformedArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionDisabledValueValidateOnly() = ObjectCollection17.DeleteFromCollectionDisabledValueValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvisibleValueValidateOnly() = ObjectCollection17.DeleteFromCollectionInvisibleValueValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionImmutableObjectValidateOnly() = ObjectCollection17.DeleteFromCollectionImmutableObjectValidateOnly (api x.NakedObjectsFramework) 
//    [<Test>]    
//    member x.DeleteFromCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsNameValidateOnly (api x.NakedObjectsFramework)  
//    [<Test>]    
//    member x.DeleteFromCollectionInternalError() = ObjectCollection17.DeleteFromCollectionInternalError (api x.NakedObjectsFramework) 
    // ObjectAction18
    [<Test>]
    member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionContributedService() = ObjectAction18.GetActionContributedService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyViewModel() = ObjectAction18.GetActionPropertyViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetOverloadedActionPropertyObject() = ObjectAction18.GetOverloadedActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetOverloadedActionPropertyService() = ObjectAction18.GetOverloadedActionPropertyService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetOverloadedActionPropertyViewModel() = ObjectAction18.GetOverloadedActionPropertyViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyDateTimeViewModel() = ObjectAction18.GetActionPropertyDateTimeViewModel (api x.NakedObjectsFramework)

    [<Test>]
    member x.GetActionPropertyCollectionViewModel() = ObjectAction18.GetActionPropertyCollectionViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyCollectionService() = ObjectAction18.GetActionPropertyCollectionService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyCollectionObject() = ObjectAction18.GetActionPropertyCollectionObject (api x.NakedObjectsFramework) 

    [<Test>]
    member x.GetActionPropertyDateTimeService() = ObjectAction18.GetActionPropertyDateTimeService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyDateTimeObject() = ObjectAction18.GetActionPropertyDateTimeObject (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetUserDisabledActionPropertyObject() = ObjectAction18.GetUserDisabledActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetUserDisabledActionPropertyService() = ObjectAction18.GetUserDisabledActionPropertyService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetUserDisabledActionPropertyViewModel() = ObjectAction18.GetUserDisabledActionPropertyViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionPropertyQueryOnlyObject() = ObjectAction18.GetActionPropertyQueryOnlyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyQueryOnlyService() = ObjectAction18.GetActionPropertyQueryOnlyService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionPropertyQueryOnlyViewModel() = ObjectAction18.GetActionPropertyQueryOnlyViewModel (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionPropertyIdempotentObject() = ObjectAction18.GetActionPropertyIdempotentObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyIdempotentService() = ObjectAction18.GetActionPropertyIdempotentService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionPropertyIdempotentViewModel() = ObjectAction18.GetActionPropertyIdempotentViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionPropertyWithOptObject() = ObjectAction18.GetActionPropertyWithOptObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyWithOptService() = ObjectAction18.GetActionPropertyWithOptService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyWithOptViewModel() = ObjectAction18.GetActionPropertyWithOptViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyWithOptObjectSimpleOnly() = ObjectAction18.GetActionPropertyWithOptObjectSimpleOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyWithOptServiceSimpleOnly() = ObjectAction18.GetActionPropertyWithOptServiceSimpleOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionPropertyWithOptViewModelSimpleOnly() = ObjectAction18.GetActionPropertyWithOptViewModelSimpleOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionPropertyWithOptObjectFormalOnly() = ObjectAction18.GetActionPropertyWithOptObjectFormalOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyWithOptServiceFormalOnly() = ObjectAction18.GetActionPropertyWithOptServiceFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionPropertyWithOptViewModelFormalOnly() = ObjectAction18.GetActionPropertyWithOptViewModelFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionPropertyViewModelWithMediaType() = ObjectAction18.GetActionPropertyViewModelWithMediaType (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetScalarActionService() = ObjectAction18.GetScalarActionService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetScalarActionViewModel() = ObjectAction18.GetScalarActionViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionWithValueParmObject() = ObjectAction18.GetActionWithValueParmObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithValueParmService() = ObjectAction18.GetActionWithValueParmService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionWithValueParmViewModel() = ObjectAction18.GetActionWithValueParmViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesObject() = ObjectAction18.GetActionWithValueParmWithChoicesObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithValueParmWithChoicesService() = ObjectAction18.GetActionWithValueParmWithChoicesService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionWithValueParmWithChoicesViewModel() = ObjectAction18.GetActionWithValueParmWithChoicesViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetActionWithValueParmWithDefaultObject() = ObjectAction18.GetActionWithValueParmWithDefaultObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithValueParmWithDefaultService() = ObjectAction18.GetActionWithValueParmWithDefaultService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetActionWithValueParmWithDefaultViewModel() = ObjectAction18.GetActionWithValueParmWithDefaultViewModel (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithReferenceParmViewModel() = ObjectAction18.GetActionWithReferenceParmViewModel (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesService() = ObjectAction18.GetActionWithReferenceParmWithChoicesService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesViewModel() = ObjectAction18.GetActionWithReferenceParmWithChoicesViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteObject() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteService() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithReferenceParmsWithAutoCompleteViewModel() = ObjectAction18.GetActionWithReferenceParmsWithAutoCompleteViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.InvokeParmWithAutoCompleteObject() = ObjectAction18.InvokeParmWithAutoCompleteObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithAutoCompleteService() = ObjectAction18.InvokeParmWithAutoCompleteService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModel() = ObjectAction18.InvokeParmWithAutoCompleteViewModel (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorNoParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorNoParm (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorNoParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorNoParm (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorMalformedParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorMalformedParm (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorMalformedParm (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm() = ObjectAction18.InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm (api x.NakedObjectsFramework) 



    [<Test>]
    member x.InvokeParmWithConditionalChoicesObject() = ObjectAction18.InvokeParmWithConditionalChoicesObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithConditionalChoicesService() = ObjectAction18.InvokeParmWithConditionalChoicesService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeParmWithConditionalChoicesViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.InvokeParmWithConditionalChoicesObjectErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesObjectErrorMalformedParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeParmWithConditionalChoicesServiceErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesServiceErrorMalformedParm (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm() = ObjectAction18.InvokeParmWithConditionalChoicesViewModelErrorMalformedParm (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesObjectErrorMissingParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesServiceErrorMissingParm (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesObject() = ObjectAction18.InvokeValueParmWithConditionalChoicesObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesService() = ObjectAction18.InvokeValueParmWithConditionalChoicesService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvokeValueParmWithConditionalChoicesViewModel() = ObjectAction18.InvokeValueParmWithConditionalChoicesViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultService() = ObjectAction18.GetActionWithReferenceParmWithDefaultService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultViewModel() = ObjectAction18.GetActionWithReferenceParmWithDefaultViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithChoicesAndDefaultViewModel() = ObjectAction18.GetActionWithChoicesAndDefaultViewModel (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetContributedActionOnContributee() = ObjectAction18.GetContributedActionOnContributee (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributeeOnBaseClass() = ObjectAction18.GetContributedActionOnContributeeBaseClass (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributeeWithRef() = ObjectAction18.GetContributedActionOnContributeeWithRef (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributeeWithValue() = ObjectAction18.GetContributedActionOnContributeeWithValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributer() = ObjectAction18.GetContributedActionOnContributer (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributerOnBaseClass() = ObjectAction18.GetContributedActionOnContributerBaseClass (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributerWithRef() = ObjectAction18.GetContributedActionOnContributerWithRef (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetContributedActionOnContributerWithValue() = ObjectAction18.GetContributedActionOnContributerWithValue (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvalidActionPropertyViewModel() = ObjectAction18.GetInvalidActionPropertyViewModel (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetNotFoundActionPropertyViewModel() = ObjectAction18.GetNotFoundActionPropertyViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetUserDisabledActionObject() = ObjectAction18.GetUserDisabledActionObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetUserDisabledActionService() = ObjectAction18.GetUserDisabledActionService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetUserDisabledActionViewModel() = ObjectAction18.GetUserDisabledActionViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetHiddenActionPropertyViewModel() = ObjectAction18.GetHiddenActionPropertyViewModel (api x.NakedObjectsFramework)   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject (api x.NakedObjectsFramework)   
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeService() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeService (api x.NakedObjectsFramework) 
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeViewModel() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetQueryActionObject() = ObjectAction18.GetQueryActionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetQueryActionService() = ObjectAction18.GetQueryActionService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetQueryActionViewModel() = ObjectAction18.GetQueryActionViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetQueryActionWithParmsObject() = ObjectAction18.GetQueryActionWithParmsObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetQueryActionWithParmsService() = ObjectAction18.GetQueryActionWithParmsService (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetQueryActionWithParmsViewModel() = ObjectAction18.GetQueryActionWithParmsViewModel (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionViewModel() = ObjectAction18.GetCollectionActionViewModel (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModel() = ObjectAction18.GetCollectionActionWithParmsViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsObjectFormalOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelFormalOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetCollectionActionWithParmsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetCollectionActionWithParmsServiceSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetCollectionActionWithParmsViewModelSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsViewModelSimpleOnly (api x.NakedObjectsFramework)

    [<Test>]
    member x.ActionNotFound() = ObjectAction18.ActionNotFound (api x.NakedObjectsFramework)     
    // ObjectActionInvoke19
    [<Test>]  
    member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModel (api x.NakedObjectsFramework)
   
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectService() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeOverloadedActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionReturnObjectViewModel (api x.NakedObjectsFramework)

    [<Test>]
    member x.PostInvokeActionContributedService() = ObjectActionInvoke19.PostInvokeActionContributedService (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnViewModelObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnViewModelService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnViewModelViewModel (api x.NakedObjectsFramework)

    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectService() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnRedirectedObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnRedirectedObjectViewModel (api x.NakedObjectsFramework)


    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectService() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeUserDisabledActionReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeUserDisabledActionReturnObjectViewModel (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeActionReturnNullViewModelObject() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelService() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnNullViewModelViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullViewModelViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.PostInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnObjectService() = ObjectActionInvoke19.PutInvokeActionReturnObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PutInvokeActionReturnViewModelObject() = ObjectActionInvoke19.PutInvokeActionReturnViewModelObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnViewModelService() = ObjectActionInvoke19.PutInvokeActionReturnViewModelService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PutInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.PutInvokeActionReturnViewModelViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.PutInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PutInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.PutInvokeActionReturnNullObjectViewModel (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PutInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PutInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsFramework)   

    [<Test>]
    member x.GetInvokeActionReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnObjectService() = ObjectActionInvoke19.GetInvokeActionReturnObjectService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.GetInvokeActionReturnViewModelObject() = ObjectActionInvoke19.GetInvokeActionReturnViewModelObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnViewModelService() = ObjectActionInvoke19.GetInvokeActionReturnViewModelService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetInvokeActionReturnViewModelViewModel() = ObjectActionInvoke19.GetInvokeActionReturnViewModelViewModel (api x.NakedObjectsFramework)    



    [<Test>]
    member x.GetInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnNullObjectService() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionReturnNullObjectViewModel() = ObjectActionInvoke19.GetInvokeActionReturnNullObjectViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeContribActionReturnObject() = ObjectActionInvoke19.PostInvokeContribActionReturnObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithRefParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithRefParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectWithValueParm() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectWithValueParm (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeContribActionReturnObjectBaseClass() = ObjectActionInvoke19.PostInvokeContribActionReturnObjectBaseClass (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelWithMediaType (api x.NakedObjectsFramework)  
       
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectConcurrencyFail (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PutInvokeActionReturnObjectObjectMissingIfMatch (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencySuccess (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencySuccess (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencySuccess() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencySuccess (api x.NakedObjectsFramework) 

    [<Test>]
    member x.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch() = ObjectActionInvoke19.GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch (api x.NakedObjectsFramework) 
      
    [<Test>]
    member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectFormalOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceFormalOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelFormalOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarViewModelValidateOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarViewModel (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionReturnNullScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnNullScalarService() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnNullScalarViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarViewModel (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectFormalOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceFormalOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelFormalOnly (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidViewModelValidateOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.GetInvokeActionReturnQueryObject() = ObjectActionInvoke19.GetInvokeActionReturnQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnQueryService() = ObjectActionInvoke19.GetInvokeActionReturnQueryService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.GetInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnQueryViewModelValidateOnly (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly (api x.NakedObjectsFramework)     


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly (api x.NakedObjectsFramework)       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly (api x.NakedObjectsFramework)       


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)    


    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsFramework)    
    
      
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectObject() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectService() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetInvokeActionWithParmReturnObjectViewModel() = ObjectActionInvoke19.GetInvokeActionWithParmReturnObjectViewModel (api x.NakedObjectsFramework)  
    
     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly (api x.NakedObjectsFramework)     
    
      
    [<Test>]
    member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.NotFoundActionInvokeViewModel() = ObjectActionInvoke19.NotFoundActionInvokeViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.HiddenActionInvokeViewModel() = ObjectActionInvoke19.HiddenActionInvokeViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetActionWithSideEffectsViewModel() = ObjectActionInvoke19.GetActionWithSideEffectsViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetActionWithIdempotentObject() = ObjectActionInvoke19.GetActionWithIdempotentObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetActionWithIdempotentService() = ObjectActionInvoke19.GetActionWithIdempotentService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetActionWithIdempotentViewModel() = ObjectActionInvoke19.GetActionWithIdempotentViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.PutActionWithQueryOnlyObject() = ObjectActionInvoke19.PutActionWithQueryOnlyObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PutActionWithQueryOnlyService() = ObjectActionInvoke19.PutActionWithQueryOnlyService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PutActionWithQueryOnlyViewModel() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.NotAcceptableGetInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptableGetInvokeWrongMediaTypeViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.MissingParmsOnPostViewModel() = ObjectActionInvoke19.MissingParmsOnPostViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DisabledActionPostInvokeViewModel() = ObjectActionInvoke19.DisabledActionPostInvokeViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.UserDisabledActionPostInvokeObject() = ObjectActionInvoke19.UserDisabledActionPostInvokeObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.UserDisabledActionPostInvokeService() = ObjectActionInvoke19.UserDisabledActionPostInvokeService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.UserDisabledActionPostInvokeViewModel() = ObjectActionInvoke19.UserDisabledActionPostInvokeViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.NotFoundActionPostInvokeViewModel() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.HiddenActionPostInvokeViewModel() = ObjectActionInvoke19.HiddenActionPostInvokeViewModel (api x.NakedObjectsFramework)   


    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject (api x.NakedObjectsFramework)
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService (api x.NakedObjectsFramework)   
    [<Test>]    
    member x.NotAcceptablePostInvokeWrongMediaTypeViewModel() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.PostQueryActionWithErrorObject() = ObjectActionInvoke19.PostQueryActionWithErrorObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostQueryActionWithErrorService() = ObjectActionInvoke19.PostQueryActionWithErrorService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostQueryActionWithErrorViewModel() = ObjectActionInvoke19.PostQueryActionWithErrorViewModel (api x.NakedObjectsFramework)    


    [<Test>]
    member x.GetQueryActionWithErrorObject() = ObjectActionInvoke19.GetQueryActionWithErrorObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetQueryActionWithErrorService() = ObjectActionInvoke19.GetQueryActionWithErrorService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.GetQueryActionWithErrorViewModel() = ObjectActionInvoke19.GetQueryActionWithErrorViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.MalformedFormalParmsOnPostQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModel (api x.NakedObjectsFramework) 


    
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryObject() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryService() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.WrongTypeFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.WrongTypeFormalParmsOnPostQueryViewModel (api x.NakedObjectsFramework) 

     
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryObject() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryService() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.EmptyFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.EmptyFormalParmsOnPostQueryViewModel (api x.NakedObjectsFramework) 

    [<Test>]
    member x.InvalidFormalParmsOnPostQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.InvalidFormalParmsOnPostQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.MissingParmsOnGetQueryObject() = ObjectActionInvoke19.MissingParmsOnGetQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MissingParmsOnGetQueryService() = ObjectActionInvoke19.MissingParmsOnGetQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.MissingParmsOnGetQueryViewModel() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryService() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.MalformedSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.MalformedFormalParmsOnGetQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.MalformedFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryService() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.InvalidSimpleParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModel (api x.NakedObjectsFramework)    


    [<Test>]
    member x.InvalidFormalParmsOnGetQueryObject() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryService() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.InvalidFormalParmsOnGetQueryViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.DisabledActionInvokeQueryObject() = ObjectActionInvoke19.DisabledActionInvokeQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.DisabledActionInvokeQueryService() = ObjectActionInvoke19.DisabledActionInvokeQueryService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.DisabledActionInvokeQueryViewModel() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModel (api x.NakedObjectsFramework)    


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsSimpleViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarMissingParmsFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarMissingParmsFormalViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly (api x.NakedObjectsFramework)        
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly (api x.NakedObjectsFramework)        
    
     
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModel (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly (api x.NakedObjectsFramework)   


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly (api x.NakedObjectsFramework)  
    
          
    [<Test>]
    member x.PostInvokeActionReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionReturnQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnQueryService() = ObjectActionInvoke19.PostInvokeActionReturnQueryService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModel (api x.NakedObjectsFramework)  


    [<Test>]
    member x.PostInvokeActionReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnQueryViewModelValidateOnly (api x.NakedObjectsFramework)  


    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostInvokeOverloadedActionObject() = ObjectActionInvoke19.PostInvokeOverloadedActionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeOverloadedActionService() = ObjectActionInvoke19.PostInvokeOverloadedActionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeOverloadedActionViewModel() = ObjectActionInvoke19.PostInvokeOverloadedActionViewModel (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly (api x.NakedObjectsFramework)  

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModel (api x.NakedObjectsFramework)   
       
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly (api x.NakedObjectsFramework)     
            
    [<Test>]
    member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostCollectionActionWithErrorService() = ObjectActionInvoke19.PostCollectionActionWithErrorService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.PostCollectionActionWithErrorViewModel() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModel (api x.NakedObjectsFramework)  

    [<Test>]
    member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.MissingParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionService() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService (api x.NakedObjectsFramework)  
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModel (api x.NakedObjectsFramework)  

    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionService() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionViewModel() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.DisabledActionInvokeCollectionObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionObject (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DisabledActionInvokeCollectionService() = ObjectActionInvoke19.DisabledActionInvokeCollectionService (api x.NakedObjectsFramework) 
    [<Test>]
    member x.DisabledActionInvokeCollectionViewModel() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModel (api x.NakedObjectsFramework) 

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel (api x.NakedObjectsFramework)    

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsFramework)       
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsFramework)       
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalService (api x.NakedObjectsFramework)      
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel (api x.NakedObjectsFramework)      
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectFormalOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceFormalOnly (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelFormalOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelFormalOnly (api x.NakedObjectsFramework)    
    
    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly (api x.NakedObjectsFramework)    
    [<Test>]
    member x.PostInvokeActionReturnCollectionViewModelVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionViewModelVerifyOnly (api x.NakedObjectsFramework)    
    
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService (api x.NakedObjectsFramework)     
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModel (api x.NakedObjectsFramework)     
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly (api x.NakedObjectsFramework)             
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly (api x.NakedObjectsFramework)             
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModel() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly (api x.NakedObjectsFramework)           
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly (api x.NakedObjectsFramework)           
    
    [<Test>]
    member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionReturnCollectionViewModel() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.PostQueryActionWithValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithValidateFailObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostQueryActionWithValidateFailService() = ObjectActionInvoke19.PostQueryActionWithValidateFailService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostQueryActionWithValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithValidateFailViewModel (api x.NakedObjectsFramework)   

    [<Test>]
    member x.PostQueryActionWithCrossValidateFailObject() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailService() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.PostQueryActionWithCrossValidateFailViewModel() = ObjectActionInvoke19.PostQueryActionWithCrossValidateFailViewModel (api x.NakedObjectsFramework) 


    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService (api x.NakedObjectsFramework)    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel (api x.NakedObjectsFramework)    
    
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService (api x.NakedObjectsFramework)   
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel (api x.NakedObjectsFramework)   
    
    [<Test>]
    member x.MissingParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MissingParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>] 
    member x.MissingParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsFramework)   
    
    [<Test>] 
    member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.MissingParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsFramework)    

    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.MalformedSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedSimpleParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>] 
    member x.MalformedFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsFramework)   
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsFramework)         
    [<Test>] 
    member x.InvalidSimpleParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidSimpleParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsFramework)         
    
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryServiceValidateOnly (api x.NakedObjectsFramework)      
    [<Test>] 
    member x.InvalidFormalParmsOnGetQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnGetQueryViewModelValidateOnly (api x.NakedObjectsFramework)      
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionViewModelValidateOnly (api x.NakedObjectsFramework)     
    
    [<Test>] 
    member x.DisabledActionInvokeQueryObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.DisabledActionInvokeQueryServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.DisabledActionInvokeQueryViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeQueryViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.DisabledActionInvokeCollectionViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionViewModelValidateOnly (api x.NakedObjectsFramework) 
    
    [<Test>] 
    member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly (api x.NakedObjectsFramework)      
    [<Test>] 
    member x.NotFoundActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeViewModelValidateOnly (api x.NakedObjectsFramework)      
    
    [<Test>] 
    member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.HiddenActionInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>] 
    member x.GetActionWithSideEffectsViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsViewModelValidateOnly (api x.NakedObjectsFramework)   
    
    [<Test>] 
    member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.GetActionWithIdempotentViewModelValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.PutActionWithQueryOnlyViewModelValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.GetQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>] 
    member x.GetQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.GetQueryActionWithErrorViewModelValidateOnly (api x.NakedObjectsFramework)     
    
    [<Test>] 
    member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.PostCollectionActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>] 
    member x.MissingParmsOnPostViewModelValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostViewModelValidateOnly (api x.NakedObjectsFramework)     
    
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryViewModelValidateOnly (api x.NakedObjectsFramework)     
    
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryServiceValidateOnly (api x.NakedObjectsFramework)       
    [<Test>] 
    member x.InvalidFormalParmsOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostQueryViewModelValidateOnly (api x.NakedObjectsFramework)       
    
    [<Test>] 
    member x.InvalidUrlOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.InvalidUrlOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryServiceValidateOnly (api x.NakedObjectsFramework)   
    [<Test>] 
    member x.InvalidUrlOnPostQueryViewModelValidateOnly() = ObjectActionInvoke19.InvalidUrlOnPostQueryViewModelValidateOnly (api x.NakedObjectsFramework)   
    
    
    [<Test>] 
    member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly (api x.NakedObjectsFramework)  
    [<Test>] 
    member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>] 
    member x.DisabledActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeViewModelValidateOnly (api x.NakedObjectsFramework)  
    
    [<Test>] 
    member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.NotFoundActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.HiddenActionPostInvokeViewModelValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly (api x.NakedObjectsFramework)     
    [<Test>] 
    member x.PostQueryActionWithErrorViewModelValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorViewModelValidateOnly (api x.NakedObjectsFramework)     
    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.GetInvokeActionReturnCollectionViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionViewModelValidateOnly (api x.NakedObjectsFramework)    
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly (api x.NakedObjectsFramework)  
    
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsFramework)  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsFramework)  
    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly (api x.NakedObjectsFramework) 
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly (api x.NakedObjectsFramework)    
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly (api x.NakedObjectsFramework)    
    

    // DomainType21
    [<Test>] 
    member x.GetMostSimpleObjectType() = DomainType21.GetMostSimpleObjectType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetWithActionObjectType() = DomainType21.GetWithActionObjectType (api x.NakedObjectsFramework)
    [<Test>]
    member x.GetWithActionServiceType() = DomainType21.GetWithActionServiceType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetWithReferenceObjectType() = DomainType21.GetWithReferenceObjectType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetWithValueObjectType() = DomainType21.GetWithValueObjectType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetWithCollectionObjectType() = DomainType21.GetWithCollectionObjectType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetPredefinedDomainTypes() = DomainType21.GetPredefinedDomainTypes (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundPredefinedDomainTypes() = DomainType21.NotFoundPredefinedDomainTypes (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundGetMostSimpleObjectType() = DomainType21.NotFoundGetMostSimpleObjectType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableGetMostSimpleObjectType() = DomainType21.NotAcceptableGetMostSimpleObjectType (api x.NakedObjectsFramework)
    // DomainProperty22
    [<Test>] 
    member x.GetValuePropertyType() = DomainProperty22.GetValuePropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetReferencePropertyType() = DomainProperty22.GetReferencePropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetValueStringPropertyType() = DomainProperty22.GetValueStringPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetValueDateTimePropertyType() = DomainProperty22.GetValueDateTimePropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundPropertyType() = DomainProperty22.NotFoundPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypePropertyType() = DomainProperty22.NotFoundTypePropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableGetValuePropertyType() = DomainProperty22.NotAcceptableGetValuePropertyType (api x.NakedObjectsFramework)
    // DomainCollection23
    [<Test>] 
    member x.GetCollectionPropertyType() = DomainCollection23.GetCollectionPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetSetCollectionPropertyType() = DomainCollection23.GetSetCollectionPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetCollectionPropertyTypeWithDescription() = DomainCollection23.GetCollectionPropertyTypeWithDescription (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeCollectionPropertyType() = DomainCollection23.NotFoundTypeCollectionPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundCollectionPropertyType() = DomainCollection23.NotFoundCollectionPropertyType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableGetCollectionPropertyType() = DomainCollection23.NotAcceptableGetCollectionPropertyType (api x.NakedObjectsFramework)
    // DomainAction24
    [<Test>] 
    member x.GetActionTypeObjectNoParmsScalar() = DomainAction24.GetActionTypeObjectNoParmsScalar (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsScalar() = DomainAction24.GetActionTypeServiceNoParmsScalar (api x.NakedObjectsFramework)

    [<Test>] 
    member x.GetOverloadedActionTypeObjectNoParmsScalar() = DomainAction24.GetOverloadedActionTypeObjectNoParmsScalar (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetOverloadedActionTypeServiceNoParmsScalar() = DomainAction24.GetOverloadedActionTypeServiceNoParmsScalar (api x.NakedObjectsFramework)


    [<Test>] 
    member x.GetActionTypeObjectNoParmsVoid() = DomainAction24.GetActionTypeObjectNoParmsVoid (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsVoid() = DomainAction24.GetActionTypeServiceNoParmsVoid (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectNoParmsCollection() = DomainAction24.GetActionTypeObjectNoParmsCollection (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceNoParmsCollection() = DomainAction24.GetActionTypeServiceNoParmsCollection (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectParmsScalar() = DomainAction24.GetActionTypeObjectParmsScalar (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceParmsScalar() = DomainAction24.GetActionTypeServiceParmsScalar (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectParmsVoid() = DomainAction24.GetActionTypeObjectParmsVoid (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceParmsVoid() = DomainAction24.GetActionTypeServiceParmsVoid (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectParmsCollection() = DomainAction24.GetActionTypeObjectParmsCollection (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeServiceParmsCollection() = DomainAction24.GetActionTypeServiceParmsCollection (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributee() = DomainAction24.GetActionTypeObjectContributedOnContributee (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionTypeObjectContributedOnContributer() = DomainAction24.GetActionTypeObjectContributedOnContributer (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeActionType() = DomainAction24.NotFoundTypeActionType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundActionTypeObject() = DomainAction24.NotFoundActionTypeObject (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundActionTypeService() = DomainAction24.NotFoundActionTypeService (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableActionType() = DomainAction24.NotAcceptableActionType (api x.NakedObjectsFramework)
    // DomainActionParameter25
    [<Test>] 
    member x.GetActionParameterTypeInt() = DomainActionParameter25.GetActionParameterTypeInt (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionParameterTypeString() = DomainActionParameter25.GetActionParameterTypeString (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetOverloadedActionParameterTypeString() = DomainActionParameter25.GetOverloadedActionParameterTypeString (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionParameterTypeDateTime() = DomainActionParameter25.GetActionParameterTypeDateTime (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionParameterTypeReference() = DomainActionParameter25.GetActionParameterTypeReference (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetActionParameterTypeStringOptional() = DomainActionParameter25.GetActionParameterTypeStringOptional (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundType() = DomainActionParameter25.NotFoundType (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundAction() = DomainActionParameter25.NotFoundAction (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundParm() = DomainActionParameter25.NotFoundParm (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableActionParameterType() = DomainActionParameter25.NotAcceptableActionParameterType (api x.NakedObjectsFramework)    
    // DomainTypeActionInvoke26
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueSimpleParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSubTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnFalseFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnFalseFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnFalseFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSubTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSubTypeOfReturnTrueFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.GetIsSuperTypeOfReturnTrueFormalParms() = DomainTypeActionInvoke26.GetIsSuperTypeOfReturnTrueFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSubTypeOfFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundTypeIsSuperTypeOfFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundActionSimpleParms() = DomainTypeActionInvoke26.NotFoundActionSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundActionFormalParms() = DomainTypeActionInvoke26.NotFoundActionFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfSimpleParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfSimpleParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundSuperTypeIsSubTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSuperTypeIsSubTypeOfFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotFoundSubTypeIsSuperTypeOfFormalParms() = DomainTypeActionInvoke26.NotFoundSubTypeIsSuperTypeOfFormalParms (api x.NakedObjectsFramework)
    [<Test>] 
    member x.MissingParmsIsSubTypeOf() = DomainTypeActionInvoke26.MissingParmsIsSubTypeOf (api x.NakedObjectsFramework)
    [<Test>] 
    member x.MalformedSimpleParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedSimpleParmsIsSubTypeOf (api x.NakedObjectsFramework)
    [<Test>] 
    member x.MalformedFormalParmsIsSubTypeOf() = DomainTypeActionInvoke26.MalformedFormalParmsIsSubTypeOf (api x.NakedObjectsFramework)
    [<Test>] 
    member x.NotAcceptableIsSubTypeOf() = DomainTypeActionInvoke26.NotAcceptableIsSubTypeOf (api x.NakedObjectsFramework)
end
