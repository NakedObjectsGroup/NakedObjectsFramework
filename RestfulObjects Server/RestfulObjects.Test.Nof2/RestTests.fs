module NakedObjects.Rest.Test.Nof2
open NUnit.Framework
open System
open sdm.core.boot
open RestfulObjects.Snapshot.Utility
open RestfulObjects.Mvc
open sdm.systems.application.container
open RestfulObjects.Test.Data
open sdm.systems.application.services.api.security.authorisation
open sdm.systems.application.services.api.security.authentication
open sdm.systems.reflector.container
open RestfulObjects.Test.TestCase.Nof2
open Microsoft.Practices.Unity
open System
open MvcTestApp.Controllers
open NakedObjects.Facade

[<TestFixture>]
type Nof2Tests() = class      
    inherit  DotNetAcceptanceTestCase("test", "objects-Rest.xml")    

    override x.setUpFixtures() = 
        let y = x.AppContainer()
        y.injectDependencies(box(x))
        let s = AbstractBusinessObject.classActionOrder("")
        x.addFixture(new RestDataFixture()) 
        ()

    override x.setUpContainer() = ()

    member x.AppContainer() : IContainer  =
        box(x.Container) :?> IContainer
    
    member x.RegisterTypes (container : IUnityContainer) = 
          container.RegisterType(typeof<RestfulObjectsController>, typeof<RestfulObjectsController>, null, (new PerResolveLifetimeManager())) |> ignore
          container.RegisterType(typeof<IOidStrategy>, typeof<TestOidStrategy>, null, (new PerResolveLifetimeManager())) |> ignore
          container.RegisterType(typeof<IFrameworkFacade>, typeof<FrameworkFacade>, null, (new PerResolveLifetimeManager())) |> ignore
        
    member x.UnityContainer : IUnityContainer = 
        let c = new UnityContainer()
        x.RegisterTypes(c);
        box(c) :?> IUnityContainer
           
        
    [<SetUp>]
    member x.Setup() =
        x.SetUp()
        UriMtHelper.GetApplicationPath <- Func<string>(fun () -> "")
        RestfulObjectsControllerBase.IsReadOnly <- false  
       
    
    [<TearDown>]
    member x.TearDown() = 
        RestfulObjectsControllerBase.DomainModel <- RestControlFlags.DomainModelType.Selectable
        RestfulObjectsControllerBase.ConcurrencyChecking <- false
        x.tearDown()
        ()

    member x.API : RestfulObjectsController = x.UnityContainer.Resolve<RestfulObjectsController>()
   
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
    member x.GetUser() = User6.GetUser x.API
    [<Test>] 
    member x.NotAcceptableGetUser() = User6.NotAcceptableGetUser x.API
    [<Test>] 
    member x.GetUserWithMediaType() = User6.GetUserWithMediaType x.API
    [<Test>] 
    member x.GetDomainServices() = DomainServices7.GetDomainServices x.API 
    [<Test>] 
    member x.GetDomainServicesWithMediaType() = DomainServices7.GetDomainServicesWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetDomainServices() = DomainServices7.NotAcceptableGetDomainServices x.API 
    [<Test>] 
    member x.GetVersion() = Version8.GetVersion x.API 
    [<Test>] 
    member x.GetVersionWithMediaType() = Version8.GetVersionWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetVersion() = Version8.NotAcceptableGetVersion x.API 
    [<Test>]
    member x.Error() = Error10.Error x.API 
    [<Test>]
    member x.NotAcceptableError() = Error10.NotAcceptableError x.API 
  
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
    member x.PersistWithValueTransientObjectFail() = Objects9.PersistWithValueTransientObjectFail x.API  
    [<Test>]
    member x.PersistWithReferenceTransientObjectFail() = Objects9.PersistWithReferenceTransientObjectFail x.API  
    [<Test>]
    member x.PersistWithCollectionTransientObjectFail() = Objects9.PersistWithCollectionTransientObjectFail x.API  



    [<Test>]
    member x.PersistWithValueTransientObject() = Objects9.PersistWithValueTransientObject x.API  

    [<Test>]
    member x.PersistWithReferenceTransientObject() = Objects9.PersistWithReferenceTransientObject x.API  

    [<Test>]
    member x.PersistWithCollectionTransientObject() = Objects9.PersistWithCollectionTransientObject x.API  

    [<Test>]
    member x.PersistUnknownTypeTransientObject() = Objects9.PersistUnknownTypeTransientObject x.API

//    [<Test>]
//    member x.PersistTransientObjectNoDomainModel() = Objects9.PersistTransientObjectNoDomainModel x.API


    [<Test>]
    member x.GetMostSimpleObject() = DomainObject14.GetMostSimpleObject x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSelectable() = DomainObject14.GetMostSimpleObjectConfiguredSelectable x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredNone() = DomainObject14.GetMostSimpleObjectConfiguredNone x.API  
    [<Test>]
    member x.GetMostSimpleObjectFormalOnly() = DomainObject14.GetMostSimpleObjectFormalOnly x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredFormalOnly() = DomainObject14.GetMostSimpleObjectConfiguredFormalOnly x.API 
    [<Test>]
    member x.GetMostSimpleObjectSimpleOnly() = DomainObject14.GetMostSimpleObjectSimpleOnly x.API  
    [<Test>]
    member x.GetMostSimpleObjectConfiguredSimpleOnly() = DomainObject14.GetMostSimpleObjectConfiguredSimpleOnly x.API 
    [<Test>]
    member x.GetMostSimpleObjectConfiguredOverrides() = DomainObject14.GetMostSimpleObjectConfiguredOverrides x.API 
    [<Test>]
    member x.GetWithValueObject() = DomainObject14.GetWithValueObject x.API 
    [<Test>]
    member x.GetWithValueObjectWithMediaType() = DomainObject14.GetWithValueObjectWithMediaType x.API 
    [<Test>]
    member x.PutWithValueObject() = DomainObject14.PutWithValueObject x.API 
    [<Test>]
    member x.PutWithValueObjectValidateOnly() = DomainObject14.PutWithValueObjectValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectConcurrencySuccess() = DomainObject14.PutWithValueObjectConcurrencySuccess x.API 

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
    member x.GetWithCollectionObject() = DomainObject14.GetWithCollectionObject x.API // todo "Cannot be modified"

    [<Test>]
    member x.GetWithCollectionObjectSimpleOnly() = DomainObject14.GetWithCollectionObjectSimpleOnly x.API // todo "Cannot be modified"
    [<Test>]
    member x.GetWithCollectionObjectFormalOnly() = DomainObject14.GetWithCollectionObjectFormalOnly x.API // todo "Cannot be modified"

    [<Test>]
    member x.InvalidGetObject() = DomainObject14.InvalidGetObject x.API 
    [<Test>]
    member x.NotFoundGetObject() = DomainObject14.NotFoundGetObject x.API 
    [<Test>]    
    member x.NotAcceptableGetObjectWrongMediaType() = DomainObject14.NotAcceptableGetObjectWrongMediaType x.API 
    [<Test>]
    member x.PutWithValueObjectMissingArgs() = DomainObject14.PutWithValueObjectMissingArgs x.API 
    [<Test>]
    member x.PutWithValueObjectMissingArgsValidateOnly() = DomainObject14.PutWithValueObjectMissingArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValueObjectMalformedArgs() = DomainObject14.PutWithValueObjectMalformedArgs x.API 
    [<Test>]
    member x.PutWithValueObjectMalformedArgsValidateOnly() = DomainObject14.PutWithValueObjectMalformedArgsValidateOnly x.API
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValue() = DomainObject14.PutWithValueObjectInvalidArgsValue x.API // todo "invalid value is not a valid WholeNumber"
    [<Test>]
    member x.PutWithValueObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithValueObjectInvalidArgsValueValidateOnly x.API // todo "invalid value is not a valid WholeNumber"
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValue() = DomainObject14.PutWithReferenceObjectInvalidArgsValue x.API // todo "Invalid type: field must be set with a NoMemberSpecification [class=MostSimple]"
    [<Test>]
    member x.PutWithReferenceObjectInvalidArgsValueValidateOnly() = DomainObject14.PutWithReferenceObjectInvalidArgsValueValidateOnly x.API // todo "Invalid type: field must be set with a NoMemberSpecification [class=MostSimple]"
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
    member x.GetService() = DomainService15.GetService x.API 

    [<Test>] 
    member x.GetServiceSimpleOnly() = DomainService15.GetServiceSimpleOnly x.API 
    [<Test>] 
    member x.GetServiceFormalOnly() = DomainService15.GetServiceFormalOnly x.API 

    [<Test>] 
    member x.GetServiceWithMediaType() = DomainService15.GetServiceWithMediaType x.API 
//    [<Test>] 
//    member x.GetServiceAsObject() = DomainService15.GetServiceAsObject x.API 
    [<Test>]
    member x.GetWithActionService() = DomainService15.GetWithActionService x.API   
    [<Test>]
    member x.InvalidGetService() = DomainService15.InvalidGetService x.API 
    [<Test>]
    member x.NotFoundGetService() = DomainService15.NotFoundGetService x.API 
    [<Test>]    
    member x.NotAcceptableGetServiceWrongMediaType() = DomainService15.NotAcceptableGetServiceWrongMediaType x.API 
    [<Test>]
    member x.GetValueProperty() = ObjectProperty16.GetValueProperty x.API 
    [<Test>]
    member x.GetValuePropertyFormalOnly() = ObjectProperty16.GetValuePropertyFormalOnly x.API 
    [<Test>]
    member x.GetValuePropertySimpleOnly() = ObjectProperty16.GetValuePropertySimpleOnly x.API 

    [<Test>]
    member x.GetValuePropertyWithMediaType() = ObjectProperty16.GetValuePropertyWithMediaType x.API 
//    [<Test>]
//    member x.GetChoicesValueProperty() = RestTests.GetChoicesValueProperty x.API 
    [<Test>]
    member x.GetDisabledValueProperty() = ObjectProperty16.GetDisabledValueProperty x.API 
    [<Test>]
    member x.GetReferenceProperty() = ObjectProperty16.GetReferenceProperty x.API 
    [<Test>]
    member x.GetDisabledReferenceProperty() = ObjectProperty16.GetDisabledReferenceProperty x.API 
//    [<Test>]
//    member x.GetChoicesReferenceProperty() = RestTests.GetChoicesReferenceProperty x.API 
    [<Test>]
    member x.GetInvalidProperty() = ObjectProperty16.GetInvalidProperty x.API 
    [<Test>]
    member x.GetNotFoundProperty() = ObjectProperty16.GetNotFoundProperty x.API 
    [<Test>]
    member x.GetHiddenValueProperty() = ObjectProperty16.GetHiddenValueProperty x.API 
    [<Test>]
    member x.GetHiddenReferenceProperty() = ObjectProperty16.GetHiddenReferenceProperty x.API 
    [<Test>]
    
    member x.NotAcceptableGetPropertyWrongMediaType() = ObjectProperty16.NotAcceptableGetPropertyWrongMediaType x.API 
    [<Test>]
    member x.GetErrorValueProperty() = ObjectProperty16.GetErrorValueProperty x.API 
    [<Test>]
    member x.GetErrorReferenceProperty() = ObjectProperty16.GetErrorReferenceProperty x.API 
    [<Test>]
    member x.PutValuePropertySuccess() = ObjectProperty16.PutValuePropertySuccess x.API 

    [<Test>]
    member x.PutValuePropertyConcurrencySuccess() = ObjectProperty16.PutValuePropertyConcurrencySuccess x.API 

    [<Test>]
    member x.PutValuePropertyConcurrencyFail() = ObjectProperty16.PutValuePropertyConcurrencyFail x.API 
    [<Test>]
    member x.PutValuePropertyMissingIfMatch() = ObjectProperty16.PutValuePropertyMissingIfMatch x.API 

    [<Test>]
    member x.PutValuePropertySuccessValidateOnly() = ObjectProperty16.PutValuePropertySuccessValidateOnly x.API 
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
    member x.PutWithValuePropertyInvalidArgsValue() = ObjectProperty16.PutWithValuePropertyInvalidArgsValue x.API // todo "invalid value is not a valid WholeNumber"
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValue() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValue x.API // todo "Invalid type: field must be set with a NoMemberSpecification [class=MostSimple]"
    [<Test>]
    member x.PutWithValuePropertyDisabledValue() = ObjectProperty16.PutWithValuePropertyDisabledValue x.API 

  

    [<Test>]
    member x.PutWithReferencePropertyDisabledValue() = ObjectProperty16.PutWithReferencePropertyDisabledValue x.API 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValue() = ObjectProperty16.PutWithValuePropertyInvisibleValue x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValue() = ObjectProperty16.PutWithReferencePropertyInvisibleValue x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsName() = ObjectProperty16.PutWithValuePropertyInvalidArgsName x.API 
    [<Test>]
    member x.NotAcceptablePutPropertyWrongMediaType() = ObjectProperty16.NotAcceptablePutPropertyWrongMediaType x.API 

    [<Test>]
    member x.PutWithValuePropertyMissingArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMissingArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyMalformedArgsValidateOnly() = ObjectProperty16.PutWithValuePropertyMalformedArgsValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsValueValidateOnly x.API // todo "invalid value is not a valid WholeNumber"
    [<Test>]
    member x.PutWithReferencePropertyInvalidArgsValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvalidArgsValueValidateOnly x.API // todo "Invalid type: field must be set with a NoMemberSpecification [class=MostSimple]"
    [<Test>]
    member x.PutWithValuePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithValuePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithValuePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.PutWithReferencePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.PutWithValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.PutWithValuePropertyInvalidArgsNameValidateOnly x.API 
 
    [<Test>]
    member x.PutWithValuePropertyInternalError() = ObjectProperty16.PutWithValuePropertyInternalError x.API 
    [<Test>]
    member x.PutWithReferencePropertyInternalError() = ObjectProperty16.PutWithReferencePropertyInternalError x.API 

    [<Test>]
    member x.DeleteValuePropertyDisabledValue() = ObjectProperty16.DeleteValuePropertyDisabledValue x.API 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValue() = ObjectProperty16.DeleteReferencePropertyDisabledValue x.API 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValue() = ObjectProperty16.DeleteValuePropertyInvisibleValue x.API 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValue() = ObjectProperty16.DeleteReferencePropertyInvisibleValue x.API 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsName() = ObjectProperty16.DeleteValuePropertyInvalidArgsName x.API 
    [<Test>]
    member x.NotAcceptableDeletePropertyWrongMediaType() = ObjectProperty16.NotAcceptableDeletePropertyWrongMediaType x.API 

    [<Test>]
    member x.DeleteValuePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteValuePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertyDisabledValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyDisabledValueValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteValuePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.DeleteReferencePropertyInvisibleValueValidateOnly() = ObjectProperty16.DeleteReferencePropertyInvisibleValueValidateOnly x.API 
    [<Test>]
    member x.DeleteValuePropertyInvalidArgsNameValidateOnly() = ObjectProperty16.DeleteValuePropertyInvalidArgsNameValidateOnly x.API 
  

    [<Test>]
    member x.DeleteValuePropertyInternalError() = ObjectProperty16.DeleteValuePropertyInternalError x.API 
//    [<Test>]
//    member x.DeleteReferencePropertyInternalError() = RestTests.DeleteReferencePropertyInternalError x.API 
    [<Test>]
    member x.GetCollectionProperty() = ObjectCollection17.GetCollectionProperty x.API // todo "sdm.systems.application.collections.InternalCollection"

    [<Test>]
    member x.GetCollectionPropertyFormalOnly() = ObjectCollection17.GetCollectionPropertyFormalOnly x.API // todo "sdm.systems.application.collections.InternalCollection"
    [<Test>]
    member x.GetCollectionPropertySimpleOnly() = ObjectCollection17.GetCollectionPropertySimpleOnly x.API // todo "sdm.systems.application.collections.InternalCollection"

    [<Test>]
    member x.GetCollectionPropertyWithMediaType() = ObjectCollection17.GetCollectionPropertyWithMediaType x.API // todo "sdm.systems.application.collections.InternalCollection"
    [<Test>]
    member x.GetDisabledCollectionProperty() = ObjectCollection17.GetDisabledCollectionProperty x.API // todo "sdm.systems.application.collections.InternalCollection" "Cannot be modified"
    [<Test>]
    member x.AddToAndDeleteFromCollectionProperty() = ObjectCollection17.AddToAndDeleteFromCollectionProperty x.API // todo "" 

//    [<Test>]
//    member x.AddToAndDeleteFromCollectionPropertyConcurrencySuccess() = ObjectCollection17.AddToAndDeleteFromCollectionPropertyConcurrencySuccess x.API // todo "" 
//
//    [<Test>]
//    member x.AddToCollectionPropertyConcurrencyFail() = ObjectCollection17.AddToCollectionPropertyConcurrencyFail x.API 
//
//    [<Test>]
//    member x.AddToCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.AddToCollectionPropertyMissingIfMatchHeader x.API 
//
//    [<Test>]
//    member x.DeleteFromCollectionPropertyConcurrencyFail() = ObjectCollection17.DeleteFromCollectionPropertyConcurrencyFail x.API 
//
//    [<Test>]
//    member x.DeleteFromCollectionPropertyMissingIfMatchHeader() = ObjectCollection17.DeleteFromCollectionPropertyMissingIfMatchHeader x.API
//
//
//    [<Test>]
//    member x.AddToCollectionPropertyValidateOnly() = ObjectCollection17.AddToCollectionPropertyValidateOnly x.API 
//    [<Test>]
//    member x.DeleteFromCollectionPropertyValidateOnly() = ObjectCollection17.DeleteFromCollectionPropertyValidateOnly x.API ""

    [<Test>]
    member x.GetInvalidCollection() = ObjectCollection17.GetInvalidCollection x.API 
    [<Test>]
    member x.GetNotFoundCollection() = ObjectCollection17.GetNotFoundCollection x.API 
    [<Test>]
    member x.GetHiddenValueCollection() = ObjectCollection17.GetHiddenValueCollection x.API 
    [<Test>]
    
    member x.NotAcceptableGetCollectionWrongMediaType() = ObjectCollection17.NotAcceptableGetCollectionWrongMediaType x.API 
//    [<Test>]
//    member x.GetErrorValueCollection() = RestTests.GetErrorValueCollection x.API 

//    [<Test>]
//    member x.AddToCollectionMissingArgs() = ObjectCollection17.AddToCollectionMissingArgs x.API 
//    [<Test>]
//    member x.AddToCollectionMalformedArgs() = ObjectCollection17.AddToCollectionMalformedArgs x.API 
//    [<Test>]
//    member x.AddToCollectionInvalidArgs() = ObjectCollection17.AddToCollectionInvalidArgs x.API 
//    [<Test>]
//    member x.AddToCollectionDisabledValue() = ObjectCollection17.AddToCollectionDisabledValue x.API "Cannot be modified"
//    [<Test>]
//    member x.AddToCollectionInvisibleValue() = ObjectCollection17.AddToCollectionInvisibleValue x.API 
//    [<Test>]
//    member x.AddToCollectionInvalidArgsName() = ObjectCollection17.AddToCollectionInvalidArgsName x.API 
//    [<Test>]
//    member x.NotAcceptableAddCollectionWrongMediaType() = ObjectCollection17.NotAcceptableAddCollectionWrongMediaType x.API 
//
//    [<Test>]
//    member x.AddToCollectionMissingArgsValidateOnly() = ObjectCollection17.AddToCollectionMissingArgsValidateOnly x.API 
//    [<Test>]
//    member x.AddToCollectionMalformedArgsValidateOnly() = ObjectCollection17.AddToCollectionMalformedArgsValidateOnly x.API 
//    [<Test>]
//    member x.AddToCollectionInvalidArgsValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsValidateOnly x.API 
//    [<Test>]
//    member x.AddToCollectionDisabledValueValidateOnly() = ObjectCollection17.AddToCollectionDisabledValueValidateOnly x.API "Cannot be modified"
//    [<Test>]
//    member x.AddToCollectionInvisibleValueValidateOnly() = ObjectCollection17.AddToCollectionInvisibleValueValidateOnly x.API 
//    [<Test>]
//    member x.AddToCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.AddToCollectionInvalidArgsNameValidateOnly x.API 
  

//    [<Test>]
//    member x.AddToCollectionInternalError() = RestTests.AddToCollectionInternalError x.API 

//    [<Test>]
//    member x.DeleteFromCollectionMissingArgs() = ObjectCollection17.DeleteFromCollectionMissingArgs x.API 
//    [<Test>]
//    member x.DeleteFromCollectionMalformedArgs() = ObjectCollection17.DeleteFromCollectionMalformedArgs x.API 
//    [<Test>]
//    member x.DeleteFromCollectionInvalidArgs() = ObjectCollection17.DeleteFromCollectionInvalidArgs x.API 
//    [<Test>]
//    member x.DeleteFromCollectionDisabledValue() = ObjectCollection17.DeleteFromCollectionDisabledValue x.API "Cannot be modified"
//    [<Test>]
//    member x.DeleteFromCollectionInvisibleValue() = ObjectCollection17.DeleteFromCollectionInvisibleValue x.API 
//    [<Test>]
//    member x.DeleteFromCollectionInvalidArgsName() = ObjectCollection17.DeleteFromCollectionInvalidArgsName x.API 
//    [<Test>]   
//    member x.NotAcceptableDeleteFromCollectionWrongMediaType() = ObjectCollection17.NotAcceptableDeleteFromCollectionWrongMediaType x.API 
//
//    [<Test>]
//    member x.DeleteFromCollectionMissingArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMissingArgsValidateOnly x.API 
//    [<Test>]
//    member x.DeleteFromCollectionMalformedArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionMalformedArgsValidateOnly x.API 
//    [<Test>]
//    member x.DeleteFromCollectionInvalidArgsValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsValidateOnly x.API 
//    [<Test>]
//    member x.DeleteFromCollectionDisabledValueValidateOnly() = ObjectCollection17.DeleteFromCollectionDisabledValueValidateOnly x.API "Cannot be modified"
//    [<Test>]
//    member x.DeleteFromCollectionInvisibleValueValidateOnly() = ObjectCollection17.DeleteFromCollectionInvisibleValueValidateOnly x.API 
//    [<Test>]
//    member x.DeleteFromCollectionInvalidArgsNameValidateOnly() = ObjectCollection17.DeleteFromCollectionInvalidArgsNameValidateOnly x.API 
//  

//    [<Test>]
//    member x.DeleteFromCollectionInternalError() = RestTests.DeleteFromCollectionInternalError x.API 
    [<Test>]
    member x.GetActionPropertyObject() = ObjectAction18.GetActionPropertyObject x.API
    [<Test>]
    member x.GetActionPropertyService() = ObjectAction18.GetActionPropertyService x.API 
//    [<Test>]
//    member x.GetActionPropertyServiceAsObject() = ObjectAction18.GetActionPropertyServiceAsObject x.API 
    [<Test>]
    member x.GetActionPropertyObjectWithMediaType() = ObjectAction18.GetActionPropertyObjectWithMediaType x.API
    [<Test>]
    member x.GetActionPropertyServiceWithMediaType() = ObjectAction18.GetActionPropertyServiceWithMediaType x.API 
//    [<Test>]
//    member x.GetActionPropertyServiceAsObjectWithMediaType() = ObjectAction18.GetActionPropertyServiceAsObjectWithMediaType x.API 
    [<Test>]
    member x.GetScalarActionObject() = ObjectAction18.GetScalarActionObject x.API // todo "string"
    [<Test>]
    member x.GetScalarActionService() = ObjectAction18.GetScalarActionService x.API // todo "string"
//    [<Test>]
//    member x.GetScalarActionServiceAsObject() = ObjectAction18.GetScalarActionServiceAsObject x.API "string"
    [<Test>]
    member x.GetActionWithReferenceParmObject() = ObjectAction18.GetActionWithReferenceParmObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmService() = ObjectAction18.GetActionWithReferenceParmService x.API 
//    [<Test>]
//    member x.GetActionWithReferenceParmServiceAsObject() = ObjectAction18.GetActionWithReferenceParmServiceAsObject x.API 
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmWithChoicesService() = ObjectAction18.GetActionWithReferenceParmWithChoicesService x.API 
//    [<Test>]
//    member x.GetActionWithReferenceParmWithChoicesServiceAsObject() = ObjectAction18.GetActionWithReferenceParmWithChoicesServiceAsObject x.API 
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultObject x.API
    [<Test>]
    member x.GetActionWithReferenceParmWithDefaultService() = ObjectAction18.GetActionWithReferenceParmWithDefaultService x.API 
//    [<Test>]
//    member x.GetActionWithReferenceParmWithDefaultServiceAsObject() = ObjectAction18.GetActionWithReferenceParmWithDefaultServiceAsObject x.API 
    [<Test>]
    member x.GetActionWithChoicesAndDefaultObject() = ObjectAction18.GetActionWithChoicesAndDefaultObject x.API
    [<Test>]
    member x.GetActionWithChoicesAndDefaultService() = ObjectAction18.GetActionWithChoicesAndDefaultService x.API 
//    [<Test>]
//    member x.GetActionWithChoicesAndDefaultServiceAsObject() = ObjectAction18.GetActionWithChoicesAndDefaultServiceAsObject x.API  
    [<Test>]
    member x.GetInvalidActionPropertyObject() = ObjectAction18.GetInvalidActionPropertyObject x.API
    [<Test>]
    member x.GetInvalidActionPropertyService() = ObjectAction18.GetInvalidActionPropertyService x.API 
//    [<Test>]
//    member x.GetInvalidActionPropertyServiceAsObject() = ObjectAction18.GetInvalidActionPropertyServiceAsObject x.API 
    [<Test>]
    member x.GetNotFoundActionPropertyObject() = ObjectAction18.GetNotFoundActionPropertyObject x.API
    [<Test>]
    member x.GetNotFoundActionPropertyService() = ObjectAction18.GetNotFoundActionPropertyService x.API 
//    [<Test>]
//    member x.GetNotFoundActionPropertyServiceAsObject() = ObjectAction18.GetNotFoundActionPropertyServiceAsObject x.API 
    [<Test>]
    member x.GetHiddenActionPropertyObject() = ObjectAction18.GetHiddenActionPropertyObject x.API
    [<Test>]
    member x.GetHiddenActionPropertyService() = ObjectAction18.GetHiddenActionPropertyService x.API 
//    [<Test>]
//    member x.GetHiddenActionPropertyServiceAsObject() = ObjectAction18.GetHiddenActionPropertyServiceAsObject x.API  
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeObject x.API
//    [<Test>]   
//    member x.NotAcceptableGetActionWrongMediaTypeServiceAsObject() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeServiceAsObject x.API 
    [<Test>]   
    member x.NotAcceptableGetActionWrongMediaTypeService() = ObjectAction18.NotAcceptableGetActionWrongMediaTypeService x.API 
    [<Test>]
    member x.PostInvokeActionReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectService() = ObjectActionInvoke19.PostInvokeActionReturnObjectService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnObjectServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencySuccess x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceConcurrencySuccess x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnObjectServiceAsObjectConcurrencySuccess() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceAsObjectConcurrencySuccess x.API 

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectConcurrencyFail() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectConcurrencyFail x.API
  
    [<Test>]
    member x.PostInvokeActionReturnObjectObjectMissingIfMatch() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectMissingIfMatch x.API

    [<Test>]
    member x.PostInvokeActionReturnNullObjectObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullObjectService() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnNullObjectServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnNullObjectServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnObjectServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceAsObjectValidateOnly x.API 



    [<Test>]
    member x.PostInvokeActionReturnObjectObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectObjectWithMediaType x.API
    [<Test>]
    member x.PostInvokeActionReturnObjectServiceWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceWithMediaType x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnObjectServiceAsObjectWithMediaType() = ObjectActionInvoke19.PostInvokeActionReturnObjectServiceAsObjectWithMediaType x.API 
   
    [<Test>]
    member x.PostInvokeActionReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnScalarService() = ObjectActionInvoke19.PostInvokeActionReturnScalarService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnScalarServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnScalarServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnScalarServiceAsObjectValidateOnly x.API 

    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnEmptyScalarService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnEmptyScalarServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyScalarServiceAsObject x.API

    [<Test>]
    member x.PostInvokeActionReturnNullScalarObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullScalarService() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnNullScalarServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnNullScalarServiceAsObject x.API

    [<Test>]
    member x.PostInvokeActionReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidObject x.API
    [<Test>]
    member x.PostInvokeActionReturnVoidService() = ObjectActionInvoke19.PostInvokeActionReturnVoidService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnVoidServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnVoidServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionReturnVoidServiceAsObjectValidateOnly x.API 

    [<Test>]
    member x.GetInvokeActionReturnCollectionObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObject x.API
    [<Test>]
    member x.GetInvokeActionReturnCollectionService() = ObjectActionInvoke19.GetInvokeActionReturnCollectionService x.API 
//    [<Test>]
//    member x.GetInvokeActionReturnCollectionServiceAsObject() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceAsObject x.API 
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleService x.API 
//    [<Test>]
//    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceAsObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceAsObject x.API 
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalService x.API 
//    [<Test>]
//    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObject() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObject x.API 
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalService x.API 
//    [<Test>]
//    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObject() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObject x.API 
   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceAsObject x.API 
//    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnScalarServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnScalarServiceAsObjectValidateOnly x.API 

    
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceAsObject x.API 
//   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnVoidServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnVoidServiceAsObjectValidateOnly x.API 
//    
   
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceAsObject x.API 
//    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnObjectServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnObjectServiceAsObjectValidateOnly x.API 

    
    [<Test>]
    member x.NotFoundActionInvokeObject() = ObjectActionInvoke19.NotFoundActionInvokeObject x.API
    [<Test>]
    member x.NotFoundActionInvokeService() = ObjectActionInvoke19.NotFoundActionInvokeService x.API 
//    [<Test>]
//    member x.NotFoundActionInvokeServiceAsObject() = ObjectActionInvoke19.NotFoundActionInvokeServiceAsObject x.API 
    [<Test>]
    member x.HiddenActionInvokeObject() = ObjectActionInvoke19.HiddenActionInvokeObject x.API
    [<Test>]
    member x.HiddenActionInvokeService() = ObjectActionInvoke19.HiddenActionInvokeService x.API 
//    [<Test>]
//    member x.HiddenActionInvokeServiceAsObject() = ObjectActionInvoke19.HiddenActionInvokeServiceAsObject x.API 
    [<Test>]
    member x.GetActionWithSideEffectsObject() = ObjectActionInvoke19.GetActionWithSideEffectsObject x.API
    [<Test>]
    member x.GetActionWithSideEffectsService() = ObjectActionInvoke19.GetActionWithSideEffectsService x.API 
//    [<Test>]
//    member x.GetActionWithSideEffectsServiceAsObject() = ObjectActionInvoke19.GetActionWithSideEffectsServiceAsObject x.API 
    [<Test>]
    member x.MissingParmsOnPostObject() = ObjectActionInvoke19.MissingParmsOnPostObject x.API
    [<Test>]
    member x.MissingParmsOnPostService() = ObjectActionInvoke19.MissingParmsOnPostService x.API 
//    [<Test>]
//    member x.MissingParmsOnPostServiceAsObject() = ObjectActionInvoke19.MissingParmsOnPostServiceAsObject x.API 
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObject x.API
    [<Test>]
    member x.MalformedFormalParmsOnPostQueryService() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryService x.API 
//    [<Test>]
//    member x.MalformedFormalParmsOnPostQueryServiceAsObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceAsObject x.API 
    [<Test>]
    member x.DisabledActionPostInvokeObject() = ObjectActionInvoke19.DisabledActionPostInvokeObject x.API // todo "Cannot be invoked"
    [<Test>]
    member x.DisabledActionPostInvokeService() = ObjectActionInvoke19.DisabledActionPostInvokeService x.API // todo "Cannot be invoked"
//    [<Test>]
//    member x.DisabledActionPostInvokeServiceAsObject() = ObjectActionInvoke19.DisabledActionPostInvokeServiceAsObject x.API "Cannot be invoked"
    [<Test>]
    member x.NotFoundActionPostInvokeObject() = ObjectActionInvoke19.NotFoundActionPostInvokeObject x.API
    [<Test>]
    member x.NotFoundActionPostInvokeService() = ObjectActionInvoke19.NotFoundActionPostInvokeService x.API 
//    [<Test>]
//    member x.NotFoundActionPostInvokeServiceAsObject() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceAsObject x.API 
    [<Test>]
    member x.HiddenActionPostInvokeObject() = ObjectActionInvoke19.HiddenActionPostInvokeObject x.API
    [<Test>]
    member x.HiddenActionPostInvokeService() = ObjectActionInvoke19.HiddenActionPostInvokeService x.API 
//    [<Test>]
//    member x.HiddenActionPostInvokeServiceAsObject() = ObjectActionInvoke19.HiddenActionPostInvokeServiceAsObject x.API 
    [<Test>]
    
    member x.NotAcceptablePostInvokeWrongMediaTypeObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeObject x.API
    [<Test>]
    
    member x.NotAcceptablePostInvokeWrongMediaTypeService() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeService x.API 
//    [<Test>]
//    
//    member x.NotAcceptablePostInvokeWrongMediaTypeServiceAsObject() = ObjectActionInvoke19.NotAcceptablePostInvokeWrongMediaTypeServiceAsObject x.API 
    [<Test>]
    member x.ObjectNotFoundWrongKey() = DomainObject14.ObjectNotFoundWrongKey x.API 
    [<Test>]
    member x.ObjectNotFoundWrongType() = DomainObject14.ObjectNotFoundWrongType x.API 
    [<Test>]
    member x.PropertyNotFound() = ObjectProperty16.PropertyNotFound x.API 
    [<Test>]
    member x.ActionNotFound() = ObjectAction18.ActionNotFound x.API  
    [<Test>]
    member x.PostCollectionActionWithErrorObject() = ObjectActionInvoke19.PostCollectionActionWithErrorObject x.API
    [<Test>]
    member x.PostCollectionActionWithErrorService() = ObjectActionInvoke19.PostCollectionActionWithErrorService x.API 
//    [<Test>]
//    member x.PostCollectionActionWithErrorServiceAsObject() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceAsObject x.API 
    [<Test>]
    member x.MissingParmsOnPostCollectionObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionObject x.API
    [<Test>]
    member x.MissingParmsOnPostCollectionService() = ObjectActionInvoke19.MissingParmsOnPostCollectionService x.API 
//    [<Test>]
//    member x.MissingParmsOnPostCollectionServiceAsObject() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceAsObject x.API 
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObject x.API
    [<Test>]
    member x.MalformedFormalParmsOnPostCollectionService() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionService x.API 
//    [<Test>]
//    member x.MalformedFormalParmsOnPostCollectionServiceAsObject() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceAsObject x.API 
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObject x.API
    [<Test>]
    member x.InvalidFormalParmsOnPostCollectionService() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionService x.API 
//    [<Test>]
//    member x.InvalidFormalParmsOnPostCollectionServiceAsObject() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceAsObject x.API 
    [<Test>]
    member x.DisabledActionInvokeCollectionObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionObject x.API // todo "Cannot be invoked"
    [<Test>]
    member x.DisabledActionInvokeCollectionService() = ObjectActionInvoke19.DisabledActionInvokeCollectionService x.API // todo "Cannot be invoked"
//    [<Test>]
//    member x.DisabledActionInvokeCollectionServiceAsObject() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceAsObject x.API "Cannot be invoked"
    [<Test>]
    member x.GetCollectionActionObject() = ObjectAction18.GetCollectionActionObject x.API // todo "java.lang.Object"
    [<Test>]
    member x.GetCollectionActionService() = ObjectAction18.GetCollectionActionService x.API // todo "java.lang.Object"
//    [<Test>]
//    member x.GetCollectionActionServiceAsObject() = ObjectAction18.GetCollectionActionServiceAsObject x.API "java.lang.Object"
//   
    [<Test>]
    member x.GetCollectionActionWithParmsObject() = ObjectAction18.GetCollectionActionWithParmsObject x.API // todo "java.lang.Object"
    [<Test>]
    member x.GetCollectionActionWithParmsService() = ObjectAction18.GetCollectionActionWithParmsService x.API // todo "java.lang.Object"
//    [<Test>]
//    member x.GetCollectionActionWithParmsServiceAsObject() = ObjectAction18.GetCollectionActionWithParmsServiceAsObject x.API  "java.lang.Object"
    
    [<Test>]
    member x.GetCollectionActionWithParmsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsObjectSimpleOnly x.API // todo "java.lang.Object"
    [<Test>]
    member x.GetCollectionActionWithParmsServiceSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceSimpleOnly x.API // todo "java.lang.Object"
//    [<Test>]
//    member x.GetCollectionActionWithParmsServiceAsObjectSimpleOnly() = ObjectAction18.GetCollectionActionWithParmsServiceAsObjectSimpleOnly x.API  "java.lang.Object"

    [<Test>]
    member x.GetCollectionActionWithParmsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsObjectFormalOnly x.API // todo "java.lang.Object"
    [<Test>]
    member x.GetCollectionActionWithParmsServiceFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceFormalOnly x.API // todo "java.lang.Object"
//    [<Test>]
//    member x.GetCollectionActionWithParmsServiceAsObjectFormalOnly() = ObjectAction18.GetCollectionActionWithParmsServiceAsObjectFormalOnly x.API  "java.lang.Object"
//    
    
    
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObject x.API 
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObjectValidateOnly x.API 

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObject x.API 
  
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObjectValidateOnly x.API 


    [<Test>]
    member x.PostInvokeActionReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnCollectionService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnCollectionServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnCollectionObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionObjectVerifyOnly x.API
    [<Test>]
    member x.PostInvokeActionReturnCollectionServiceVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceVerifyOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnCollectionServiceAsObjectVerifyOnly() = ObjectActionInvoke19.PostInvokeActionReturnCollectionServiceAsObjectVerifyOnly x.API 

    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnEmptyCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnEmptyCollectionServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnEmptyCollectionServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionReturnNullCollectionObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionReturnNullCollectionService() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionService x.API 
//    [<Test>]
//    member x.PostInvokeActionReturnNullCollectionServiceAsObject() = ObjectActionInvoke19.PostInvokeActionReturnNullCollectionServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceAsObject x.API 
    
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithScalarParmsReturnCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithScalarParmsReturnCollectionServiceAsObjectValidateOnly x.API 

    
    
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObject x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionService() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionService x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceAsObject() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceAsObject x.API 

    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly x.API
    [<Test>]
    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly x.API 
//    [<Test>]
//    member x.PostInvokeActionWithReferenceParmsReturnCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostInvokeActionWithReferenceParmsReturnCollectionServiceAsObjectValidateOnly x.API 
//   
   
    [<Test>] 
    member x.MissingParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.MissingParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceValidateOnly x.API  
//    [<Test>] 
//    member x.MissingParmsOnPostCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostCollectionServiceAsObjectValidateOnly x.API  
  
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceValidateOnly x.API   
//    [<Test>] 
//    member x.MalformedFormalParmsOnPostCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostCollectionServiceAsObjectValidateOnly x.API   
  
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.InvalidFormalParmsOnPostCollectionServiceValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceValidateOnly x.API  
//    [<Test>] 
//    member x.InvalidFormalParmsOnPostCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.InvalidFormalParmsOnPostCollectionServiceAsObjectValidateOnly x.API  
//  
    [<Test>] 
    member x.DisabledActionInvokeCollectionObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionObjectValidateOnly x.API // todo "Cannot be invoked"
    [<Test>] 
    member x.DisabledActionInvokeCollectionServiceValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceValidateOnly x.API // todo "Cannot be invoked"
//    [<Test>] 
//    member x.DisabledActionInvokeCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.DisabledActionInvokeCollectionServiceAsObjectValidateOnly x.API "Cannot be invoked"
    [<Test>] 
    member x.NotFoundActionInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.NotFoundActionInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceValidateOnly x.API  
//    [<Test>] 
//    member x.NotFoundActionInvokeServiceAsObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionInvokeServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.HiddenActionInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.HiddenActionInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceValidateOnly x.API  
//    [<Test>] 
//    member x.HiddenActionInvokeServiceAsObjectValidateOnly() = ObjectActionInvoke19.HiddenActionInvokeServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetActionWithSideEffectsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsObjectValidateOnly x.API 
    [<Test>] 
    member x.GetActionWithSideEffectsServiceValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceValidateOnly x.API  
//    [<Test>] 
//    member x.GetActionWithSideEffectsServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithSideEffectsServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetActionWithIdempotentObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentObjectValidateOnly x.API 
    [<Test>] 
    member x.GetActionWithIdempotentServiceValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceValidateOnly x.API  
//    [<Test>] 
//    member x.GetActionWithIdempotentServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetActionWithIdempotentServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.PutActionWithQueryOnlyObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyObjectValidateOnly x.API 
    [<Test>] 
    member x.PutActionWithQueryOnlyServiceValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceValidateOnly x.API  
//    [<Test>] 
//    member x.PutActionWithQueryOnlyServiceAsObjectValidateOnly() = ObjectActionInvoke19.PutActionWithQueryOnlyServiceAsObjectValidateOnly x.API  
   
    [<Test>] 
    member x.PostCollectionActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorObjectValidateOnly x.API 
    [<Test>] 
    member x.PostCollectionActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceValidateOnly x.API  
//    [<Test>] 
//    member x.PostCollectionActionWithErrorServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostCollectionActionWithErrorServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.MissingParmsOnPostObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostObjectValidateOnly x.API 
    [<Test>] 
    member x.MissingParmsOnPostServiceValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceValidateOnly x.API  
//    [<Test>] 
//    member x.MissingParmsOnPostServiceAsObjectValidateOnly() = ObjectActionInvoke19.MissingParmsOnPostServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryObjectValidateOnly x.API 
    [<Test>] 
    member x.MalformedFormalParmsOnPostQueryServiceValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceValidateOnly x.API  
//    [<Test>] 
//    member x.MalformedFormalParmsOnPostQueryServiceAsObjectValidateOnly() = ObjectActionInvoke19.MalformedFormalParmsOnPostQueryServiceAsObjectValidateOnly x.API    
    [<Test>] 
    member x.DisabledActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeObjectValidateOnly x.API  // todo "Cannot be invoked"
    [<Test>] 
    member x.DisabledActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceValidateOnly x.API // todo  "Cannot be invoked"
//    [<Test>] 
//    member x.DisabledActionPostInvokeServiceAsObjectValidateOnly() = ObjectActionInvoke19.DisabledActionPostInvokeServiceAsObjectValidateOnly x.API  "Cannot be invoked"
    [<Test>] 
    member x.NotFoundActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.NotFoundActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceValidateOnly x.API  
//    [<Test>] 
//    member x.NotFoundActionPostInvokeServiceAsObjectValidateOnly() = ObjectActionInvoke19.NotFoundActionPostInvokeServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.HiddenActionPostInvokeObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeObjectValidateOnly x.API 
    [<Test>] 
    member x.HiddenActionPostInvokeServiceValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceValidateOnly x.API  
//    [<Test>] 
//    member x.HiddenActionPostInvokeServiceAsObjectValidateOnly() = ObjectActionInvoke19.HiddenActionPostInvokeServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.PostQueryActionWithErrorObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorObjectValidateOnly x.API 
    [<Test>] 
    member x.PostQueryActionWithErrorServiceValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceValidateOnly x.API  
//    [<Test>] 
//    member x.PostQueryActionWithErrorServiceAsObjectValidateOnly() = ObjectActionInvoke19.PostQueryActionWithErrorServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionReturnCollectionObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionReturnCollectionServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceValidateOnly x.API  
//    [<Test>] 
//    member x.GetInvokeActionReturnCollectionServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionReturnCollectionServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly x.API  
//    [<Test>]
//    member x.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly x.API  
//    [<Test>] 
//    member x.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithScalarParmsReturnCollectionFormalServiceAsObjectValidateOnly x.API  
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly x.API 
    [<Test>] 
    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly x.API  
//    [<Test>] 
//    member x.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObjectValidateOnly() = ObjectActionInvoke19.GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceAsObjectValidateOnly x.API  

    [<Test>] 
    [<Ignore>] // domain types not supported on nof2 
    member x.GetDomainTypes() = DomainTypes20.GetDomainTypes x.API
    [<Test>] 
    [<Ignore>] // domain types not supported on nof2 
    member x.GetDomainTypesWithMediaType() = DomainTypes20.GetDomainTypesWithMediaType x.API 
    [<Test>] 
    member x.NotAcceptableGetDomainTypes() = DomainTypes20.NotAcceptableGetDomainTypes x.API

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

    [<Test>] 
    member x.GetValuePropertyType() = DomainProperty22.GetValuePropertyType x.API

    [<Test>]
    [<Ignore>] // domain types not supported on nof2  
    member x.GetCollectionPropertyType() = DomainCollection23.GetCollectionPropertyType x.API

    [<Test>] 
    member x.GetActionTypeObjectNoParmsScalar() = DomainAction24.GetActionTypeObjectNoParmsScalar x.API
    [<Test>] 
    member x.GetActionTypeServiceNoParmsScalar() = DomainAction24.GetActionTypeServiceNoParmsScalar x.API
    [<Test>] 
    member x.GetActionTypeObjectNoParmsVoid() = DomainAction24.GetActionTypeObjectNoParmsVoid x.API
    [<Test>] 
    member x.GetActionTypeServiceNoParmsVoid() = DomainAction24.GetActionTypeServiceNoParmsVoid x.API


    [<Test>] 
    member x.GetActionParameterTypeInt() = DomainActionParameter25.GetActionParameterTypeInt x.API
end
