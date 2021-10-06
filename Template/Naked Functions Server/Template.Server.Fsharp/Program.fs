// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFunctions.Rest.App.Demo

open System.Data.Common;
open System.Data.SqlClient;
open Microsoft.AspNetCore.Hosting;
open Microsoft.Extensions.Hosting;
open Microsoft.Extensions.Logging;

module Program = 
    let CreateHostBuilder(args : string[]) : IHostBuilder =
        // Clear default logging providers so we just log to log4net
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(fun logging -> logging.ClearProviders() |> ignore)
            .ConfigureWebHostDefaults(fun webBuilder -> webBuilder.UseStartup<Startup>() |> ignore );

    [<EntryPoint>]
    let main args =
        DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        CreateHostBuilder(args).Build().Run();
        0
