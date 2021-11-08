// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Legacy.Reflector.Extensions;
using Legacy.Rest.App.Demo.CustomReflectorExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;

namespace Legacy.Rest.App.Demo {
    public class Startup {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration) { }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(builder => {
                builder.MainMenus = ModelConfig.MainMenus;
                //builder.AddEF6Persistor(options => { options.ContextCreators = new[] {NakedObjectsRunSettings.DbContextCreator}; });
                builder.AddEFCorePersistor(options => { options.ContextCreators = new[] { ModelConfig.EFDbContextCreator }; });
                builder.AddRestfulObjects(options => {
                    options.AcceptHeaderStrict = true;
                    options.DebugWarnings = true;
                    options.DefaultPageSize = 20;
                    options.InlineDetailsInActionMemberRepresentations = false;
                    options.InlineDetailsInCollectionMemberRepresentations = false;
                    options.InlineDetailsInPropertyMemberRepresentations = false;
                });
                builder.AddNakedObjects(options => {
                    options.DomainModelTypes = ModelConfig.NOTypes;
                    options.DomainModelServices = ModelConfig.NOServices;
                    options.RegisterCustomTypes = services => {
                        services.AddSingleton(typeof(AppendFacetFactoryOrder<>), typeof(AppendFacetFactoryOrder<>));
                        services.AddSingleton(typeof(IObjectFacetFactoryProcessor), typeof(AWNotCountedAnnotationFacetFactoryParallel));
                        services.AddSingleton(typeof(IObjectFacetFactoryProcessor), typeof(AWNotNavigableFacetFactoryParallel));
                    };
                });
                builder.AddLegacy(options => {
                    options.NoValidate = true;
                    options.DomainModelTypes = ModelConfig.LegacyTypes();
                    options.DomainModelServices = ModelConfig.LegacyServices;
                });
            });
            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins, builder => {
                    builder
                        .WithOrigins("http://localhost:5001",
                                     "http://localhost:49998",
                                     "http://localhost:8080",
                                     "http://nakedobjectstest2.azurewebsites.net",
                                     "https://nakedobjectstest2.azurewebsites.net")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning", "ETag", "Set-Cookie")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModelBuilder modelBuilder, ILoggerFactory loggerFactory) {
            // for Demo use Log4Net. Configured in log4net.config  
            loggerFactory.AddLog4Net();

            modelBuilder.Build();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
            app.UseRestfulObjects();
        }
    }
}