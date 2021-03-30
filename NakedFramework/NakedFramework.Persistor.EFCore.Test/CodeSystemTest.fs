module NakedFramework.CodeSystemTest

open NakedObjects.CodeSystemTest
open NUnit.Framework
open System
open NakedFramework.DependencyInjection.Extensions
open Microsoft.Extensions.Configuration
open NakedFramework.Persistor.EFCore.Extensions
open Microsoft.EntityFrameworkCore
open TestCodeOnly

[<TestFixture>]
type EFCoreCodeSystemTests() = 
    inherit CodeSystemTests()

    override x.AddNakedFunctions = Action<NakedCoreOptions> (fun (builder) -> ())

    member x.ContextInstallers = [|Func<IConfiguration, DbContext> (fun (c) -> 
                  let context = new EFCoreCodeFirstContext(NakedObjects.TestTypes.csCS)
                  context.Create()
                  (context :> DbContext))|]
        
    member x.EFCorePersistorOptions = Action<EFCorePersistorOptions> (fun  (options) -> options.ContextInstallers <- x.ContextInstallers)


