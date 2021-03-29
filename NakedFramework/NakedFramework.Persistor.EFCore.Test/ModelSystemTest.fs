module NakedFramework.ModelSystemTest

open NakedObjects.CodeSystemTest
open NUnit.Framework
open System
open NakedFramework.DependencyInjection.Extensions
open Microsoft.Extensions.Configuration
open NakedFramework.Persistor.EFCore.Extensions
open Microsoft.EntityFrameworkCore
open NakedObjects.ModelSystemTest
open SimpleDatabase

[<TestFixture>]
type EFCoreModelSystemTests() = 
    inherit ModelSystemTests()

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    member x.ContextInstaller = Func<IConfiguration, DbContext> (fun (c) -> 
                  let context = new EFCoreSimpleDatabaseDbContext(NakedObjects.TestTypes.csMF)
                  context.Create()
                  (context :> DbContext))
        
    member x.EFCorePersistorOptions = Action<EFCorePersistorOptions> (fun  (options) -> options.ContextInstaller <- x.ContextInstaller)


