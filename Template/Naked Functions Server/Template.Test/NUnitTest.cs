// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Template.Test.Data;
using Template.Test.Helpers;
using Template.Test.TestCase;

namespace Template.Test {
    public class NUnitTest : AbstractRestTest {
        private static void CleanUpDatabase() => ObjectDbContext.Delete();

        protected static void ConfigureServices(IServiceCollection services) {
            ConfigureServicesBase(services);
            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddNakedFramework(builder => {
                builder.MainMenus = DataSetup.MainMenus;
                builder.AddEF6Persistor(options => { options.ContextInstallers = DataSetup.ContextInstallers; });
                builder.AddNakedFunctions(options => {
                    options.FunctionalTypes = DataSetup.Records;
                    options.Functions = DataSetup.Functions;
                });
                builder.AddRestfulObjects(options => options.BlobsClobs = true);
            });
        }

        private static IDictionary<string, string> Configuration() {
            var config = ConfigurationBase();
            config["ConnectionStrings:Spike"] = @"Server=(localdb)\MSSQLLocalDB;Initial Catalog=Spike;Integrated Security=True;";
            return config;
        }

        [SetUp]
        public void SetUp() { }

        [TearDown]
        public void TearDown() { }

        [OneTimeSetUp]
        public void FixtureSetUp() {
            InitializeNakedFramework(ConfigureServices, Configuration);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedFramework();
            CleanUpDatabase();
        }

        // actual tests are identical

        [Test]
        public void TestGetObject() {
            var foo = this.GetObject(new Key<Foo>("1"));

            Assert.AreEqual("Foo", foo.GetExtension("friendlyName"));
            Assert.AreEqual("Fred", foo.GetTitle());
        }
    }
}