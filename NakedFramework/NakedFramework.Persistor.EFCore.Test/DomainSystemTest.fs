module NakedFramework.DomainSystemTest

open NakedObjects.CodeSystemTest
open NUnit.Framework
open System
open NakedFramework.DependencyInjection.Extensions
open Microsoft.Extensions.Configuration
open NakedFramework.Persistor.EFCore.Extensions
open Microsoft.EntityFrameworkCore
open NakedObjects.ModelSystemTest
open SimpleDatabase
open NakedObjects.DomainSystemTest
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly

[<TestFixture>]
type EFCoreDomainSystemTests() = 
    inherit DomainSystemTests()

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    member x.ContextInstaller = Func<IConfiguration, DbContext> (fun (c) -> 
                  let context = new EFCoreAdventureWorksEntities(NakedObjects.TestTypes.csAWMARS) 
                  context.Create()
                  (context :> DbContext))
        
    member x.EFCorePersistorOptions = Action<EFCorePersistorOptions> (fun  (options) -> options.ContextInstaller <- x.ContextInstaller)


