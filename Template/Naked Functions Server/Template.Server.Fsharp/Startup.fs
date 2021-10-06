// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFunctions.Rest.App.Demo

open System;
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open NakedFramework.Architecture.Component
open NakedFramework.DependencyInjection.Extensions
open NakedFramework.Menu
open NakedFramework.Persistor.EFCore.Extensions
open NakedFramework.Rest.Extensions
open NakedFunctions.Reflector.Extensions
open Newtonsoft.Json
open Template.Model.Fsharp

type Startup private () = 
   
        member this.MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        member this.Startup(configuration : IConfiguration ) = 
            this.Configuration <- configuration;
        
        member val Configuration : IConfiguration = null with get, set

        // This method gets called by the runtime. Use this method to add services to the container.
        member this.ConfigureServices (services : IServiceCollection) =
        
            services.AddAuthentication(fun options -> 
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme |> ignore
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme |> ignore
            ).AddJwtBearer(fun options ->
                let domain = "Auth0:Domain"
                options.Authority = $"https://{this.Configuration.[domain]}/" |> ignore
                options.Audience = this.Configuration.["Auth0:Audience"] |> ignore
                options.TokenValidationParameters.NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" |> ignore
            ) |> ignore

            services.AddControllers()
                .AddNewtonsoftJson(fun options -> options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc |> ignore) |> ignore
            services.AddMvc(fun options -> options.EnableEndpointRouting = false |> ignore) |> ignore
            services.AddHttpContextAccessor() |> ignore
            services.AddNakedFramework(fun frameworkOptions -> 
                frameworkOptions.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus()) |> ignore
                frameworkOptions.AddEFCorePersistor(fun peristorOptions -> 
                    peristorOptions.ContextCreators = [| Func<IConfiguration, DbContext> ModelConfig.EFCoreDbContextCreator |] |> ignore)
                frameworkOptions.AddNakedFunctions(fun appOptions -> 
                    appOptions.DomainTypes = ModelConfig.DomainTypes() |> ignore
                    appOptions.DomainFunctions = ModelConfig.TypesDefiningDomainFunctions() |> ignore
                );
                frameworkOptions.AddRestfulObjects(fun _ -> ())
            )
            services.AddCors(fun corsOptions ->
                corsOptions.AddPolicy(this.MyAllowSpecificOrigins, fun policyBuilder ->
                    policyBuilder
                        .WithOrigins("http://localhost:5001")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning", "ETag", "Set-Cookie")
                        .AllowAnyMethod()
                        .AllowCredentials() |> ignore
                )
            )
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        member this.Configure (app : IApplicationBuilder) (env : IWebHostEnvironment)  (builder: IModelBuilder)  (loggerFactory : ILoggerFactory) =
            // for Demo use Log4Net. Configured in log4net.config  
            loggerFactory.AddLog4Net() |> ignore

            builder.Build();

            if (env.IsDevelopment()) then app.UseDeveloperExceptionPage() |> ignore
            
            app.UseAuthentication() |> ignore

            app.UseCors(this.MyAllowSpecificOrigins)|> ignore
            app.UseRouting() |> ignore
            app.UseRestfulObjects() |> ignore
        