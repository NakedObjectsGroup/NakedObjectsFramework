module NakedObjects.Rest.Test.Nof4TypesDomainType
open NUnit.Framework
open NakedObjects
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Core.Adapter.Map
open NakedObjects.Boot
open NakedObjects.Architecture.Adapter
open NakedObjects.Architecture.Persist
open NakedObjects.Architecture.Reflect
open NakedObjects.Core.Context
open NakedObjects.Core.Persist
open NakedObjects.Persistor
open NakedObjects.Persistor.Objectstore
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
open Microsoft.Practices.Unity
open NakedObjects.EntityObjectStore
open RestfulObjects.Test.Data
open NakedObjects.Surface.Nof4.Implementation
open NakedObjects.Surface
open MvcTestApp.Controllers

let mapper = new TestTypeCodeMapper()
let keyMapper = new TestKeyCodeMapper()

//let api (fw : INakedObjectsFramework) = 
//    let api = new RestfulObjectsControllerBase()
//    api.Surface <- new NakedObjects.Surface.Nof4.Implementation.NakedObjectsSurface(new NakedObjects.Surface.Nof4.Utility.ExternalOid(fw), fw)
//    api

[<TestFixture>]
type Nof4TestsTypeDomainType() = class      
    inherit  NakedObjects.Xat.AcceptanceTestCase()    

    override x.RegisterTypes(container) = 
        base.RegisterTypes(container)
        let config = new EntityObjectStoreConfiguration()
        let f = (fun () -> new CodeFirstContext("RestTest") :> Data.Entity.DbContext)
        let ignore = config.UsingCodeFirstContext(Func<Data.Entity.DbContext>(f)) 
        let ignore = container.RegisterInstance(typeof<EntityObjectStoreConfiguration>, null, config, (new ContainerControlledLifetimeManager()))
        let ignore = container.RegisterType(typeof<INakedObjectsSurface>, typeof<NakedObjectsSurface>, null, (new ContainerControlledLifetimeManager()))
        ()         
            
    [<TestFixtureSetUp>]
    member x.Setup() =     
        x.InitializeNakedObjectsFramework()
        //MemoryObjectStore.DiscardObjects()
    
    [<SetUp>]
    member x.StartTest() =           
        x.Fixtures.InstallFixtures(x.NakedObjectsFramework.ObjectPersistor, x.NakedObjectsFramework.Injector)
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
        //MemoryObjectStore.DiscardObjects()
     
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

//    override x.Fixtures 
//        with get() : IFixturesInstaller = 
//            box (new FixturesInstaller([| box (new RestDataFixtureUnitTests()) |])) :?> IFixturesInstaller 
 
 
    member x.api = x.GetConfiguredContainer().Resolve<RestfulObjectsController>()

    // DomainTypes20
    [<Test>] 
    member x.GetDomainTypes() = DomainTypes20.GetDomainTypesDomainType x.api 
    [<Test>] 
    member x.GetDomainTypesWithMediaType() = DomainTypes20.GetDomainTypesWithMediaTypeDomainType x.api  
    [<Test>] 
    member x.NotAcceptableGetDomainTypes() = DomainTypes20.NotAcceptableGetDomainTypes x.api 
   
end
