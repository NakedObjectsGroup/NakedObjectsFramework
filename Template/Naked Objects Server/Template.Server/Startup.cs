// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using NakedFramework.Architecture.Component;
using Template.Model;
using NakedFramework.Menu;

namespace NakedObjects.Rest.App.Demo {
    public class Startup {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
                options.TokenValidationParameters.NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            });
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(frameworkOptions => {
                frameworkOptions.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus());
                // frameworkOptions.AddEF6Persistor(persistorOptions => { persistorOptions.ContextInstallers = new[] { ModelConfig. }; });
                frameworkOptions.AddEFCorePersistor(persistorOptions => { persistorOptions.ContextCreators = new[] { ModelConfig.EFCoreDbContextCreator }; });
                frameworkOptions.AddRestfulObjects(restOptions => {  });
                frameworkOptions.AddNakedObjects(appOptions => {
                    appOptions.DomainModelTypes = ModelConfig.DomainModelTypes();
                    appOptions.DomainModelServices = ModelConfig.DomainModelServices();
                });
            });
            services.AddCors(corsOptions => {
                corsOptions.AddPolicy(MyAllowSpecificOrigins, policyBuilder => {
                    policyBuilder
                        .WithOrigins("http://localhost:5001",
                            "http://localhost", "http://localhost:49998")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning","ETag", "Set-Cookie")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModelBuilder builder, ILoggerFactory loggerFactory)
        {
            // for Demo use Log4Net. Configured in log4net.config  
            loggerFactory.AddLog4Net();

            builder.Build();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
            app.UseRestfulObjects();
        }
    }
}