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
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.Entity.Extensions;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;

namespace NakedObjects.Rest.Test.App {
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
                builder.MainMenus = null;
                builder.AddEF6Persistor(options => {
                    options.ContextInstallers = new[] { NakedObjectsRunSettings.DbContextInstaller };
                });
                builder.AddRestfulObjects(options => {
                    options.AcceptHeaderStrict = true;
                    options.DebugWarnings = true;
                    options.DefaultPageSize = 20;
                    options.InlineDetailsInActionMemberRepresentations = true;
                    options.InlineDetailsInCollectionMemberRepresentations = true;
                    options.InlineDetailsInPropertyMemberRepresentations = true;
                });
                builder.AddNakedObjects(options => {
                    options.Types = NakedObjectsRunSettings.Types;
                    options.Services = NakedObjectsRunSettings.Services;
                    options.ConcurrencyCheck = false;
                    options.NoValidate = true;
                });
            });
            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins, builder => {
                    builder
                        .WithOrigins("http://localhost:49998",
                                     "http://localhost:8080",
                                     "http://nakedobjectstest.azurewebsites.net",
                                     "http://nakedobjectstest2.azurewebsites.net",
                                     "https://nakedobjectstest.azurewebsites.net",
                                     "https://nakedobjectstest2.azurewebsites.net",
                                     "http://localhost")
                        .AllowAnyHeader()
                        .WithExposedHeaders("Warning", "ETag")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModelBuilder modelBuilder) {
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