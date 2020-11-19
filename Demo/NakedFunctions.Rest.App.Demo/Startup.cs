// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using AdventureWorksModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.DependencyInjection.Extensions;
using NakedObjects.Menu;
using Newtonsoft.Json;

namespace NakedFunctions.Rest.App.Demo {
    public class Startup {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedCore(options => { 
                options.ContextInstallers = new[] {ModelConfig_NakedFunctionsPM.DbContextInstaller};
                options.MainMenus = CombinedNOandNFMenus;
            });
            services.AddNakedObjects(options => {
                options.ModelNamespaces = ModelConfig_NakedObjectsPM.ModelNamespaces();
                options.Types = new[] {
                    typeof(Vendor),
                    typeof(Person),
                    typeof(Customer),
                    typeof(Shift),
                    typeof(Store),
                    typeof(BusinessEntity),
                    typeof(EmployeeDepartmentHistory),
                    typeof(Department),
                    typeof(EmployeePayHistory),
                    typeof(SpecialOfferProduct),
                    typeof(CustomerDashboard),
                    typeof(IEmployee)
                };
                options.Services = ModelConfig_NakedObjectsPM.Services().ToArray();
                options.NoValidate = true;
            });
            services.AddNakedFunctions(options => {
                options.FunctionalTypes = ModelConfig_NakedFunctionsPM.DomainTypes();
                options.Functions = ModelConfig_NakedFunctionsPM.ContributedFunctions();
            });
            services.AddRestfulObjects();

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

        private static IMenu[] CombinedNOandNFMenus(IMenuFactory mf)
        {
            var menus = ModelConfig_NakedFunctionsPM.MainMenus(mf).ToList();
            menus.AddRange(ModelConfig_NakedObjectsPM.MainMenus(mf));
            return menus.ToArray();
        }
    }
}