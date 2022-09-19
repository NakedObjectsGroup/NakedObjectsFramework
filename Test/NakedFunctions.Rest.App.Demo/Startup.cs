// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using AW;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Newtonsoft.Json;

namespace NakedFunctions.Rest.App.Demo {
    public class Startup {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(builder => {
                builder.MainMenus = MenuHelper.GenerateMenus(AWModelConfig.MainMenuTypes());
                builder.AddNakedFunctions(options => {
                    options.DomainTypes = AWModelConfig.FunctionalTypes();
                    options.DomainFunctions = AWModelConfig.Functions();
                });
                builder.AddEFCorePersistor(options => { });
                builder.AddRestfulObjects(options => options.BlobsClobs = true);
            });
            services.AddScoped<IPrincipalProvider, MockPrincipalProvider>();
            services.AddDbContext<DbContext, AdventureWorksEFCoreContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("AdventureWorksContext"));
                options.UseLazyLoadingProxies();
                //options.LogTo(m => Debug.WriteLine(m), LogLevel.Trace);
            });
            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins, builder => {
                    builder
                        .WithOrigins("http://localhost:49998",
                                     "http://localhost:8080",
                                     "http://nakedfunctionstest.azurewebsites.net",
                                     "https://nakedfunctionstest.azurewebsites.net")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning", "ETag", "Set-Cookie")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModelBuilder builder, ILoggerFactory loggerFactory) {
            
            // for Demo use Log4Net. Configured in log4net.config  
            loggerFactory.AddLog4Net();
            
            builder.Build();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
            app.UseRestfulObjects();
        }
    }
}