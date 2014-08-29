module NakedObjects.Rest.Test.Nof4Types
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

let api (fw : INakedObjectsFramework) = 
    let api = new RestfulObjectsControllerBase()
    api.Surface <- new NakedObjects.Surface.Nof4.Implementation.NakedObjectsSurface(new NakedObjects.Surface.Nof4.Utility.ExternalOid(fw), fw)
    api

[<TestFixture>]
type Nof4TestsTypes() = class      
    inherit  NakedObjects.Xat.AcceptanceTestCase()    
            
    [<TestFixtureSetUp>]
    member x.Setup() =     
        x.InitializeNakedObjectsFramework()
        MemoryObjectStore.DiscardObjects()
    
    [<SetUp>]
    member x.StartTest() =           
        x.Fixtures.InstallFixtures(x.NakedObjectsContext.ObjectPersistor)
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

  


    // DomainTypes20
    [<Test>] 
    member x.GetDomainTypes() = DomainTypes20.GetDomainTypes (api x.NakedObjectsContext)
    [<Test>] 
    member x.GetDomainTypesWithMediaType() = DomainTypes20.GetDomainTypesWithMediaType (api x.NakedObjectsContext) 
    [<Test>] 
    member x.NotAcceptableGetDomainTypes() = DomainTypes20.NotAcceptableGetDomainTypes (api x.NakedObjectsContext)
    
end
