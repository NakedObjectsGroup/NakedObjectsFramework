// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFunctions.Reflector.Extensions;
using Newtonsoft.Json;
using RestTestFramework;
using Template.Model;
using Template.Model.Functions;
using Template.Model.Types;

namespace Template.RestTest
{
    [TestClass]
    public class ExampleMSTest : AbstractRestTest {


        private static void CleanUpDatabase() => Database.Delete(cs);

        public static readonly string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True;MultipleActiveResultSets=True";

        protected static void ConfigureServices(IServiceCollection services) {
            ConfigureServicesBase(services);
            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);

            services.AddNakedFramework(builder => {
                builder.MainMenus = MenuHelper.GenerateMenus(ModelConfig.MainMenus());
                builder.AddEFCorePersistor(options => { options.ContextInstallers = new[] { ModelConfig.EFCoreDbContextCreator }; });
                builder.AddNakedFunctions(options => {
                    options.FunctionalTypes = ModelConfig.Types();
                    options.Functions = ModelConfig.Functions();
                });
                builder.AddRestfulObjects(options => options.BlobsClobs = true);
            });
        }

        private static IDictionary<string, string> Configuration() {
            var config = ConfigurationBase();
            config["ConnectionStrings:ExampleCS"] = cs;
            return config;
        }

        [TestInitialize]
        public void SetUp() { }

        [TestCleanup]
        public void TearDown() { }

        [ClassInitialize]
        public static void FixtureSetUp(TestContext tf) {
            InitializeNakedFramework(ConfigureServices, Configuration);
        }

        [ClassCleanup]
        public static void FixtureTearDown() {
            CleanupNakedFramework();
            CleanUpDatabase();
        }

        [TestMethod]
        public void TestGetObject()
        {
            var foo = this.GetObject(new Key<Student>("1"));
            Assert.AreEqual("Student", foo.GetExtension("friendlyName"));
            Assert.AreEqual("Alie Algol", foo.GetTitle());
        }


        [TestMethod]
        public void TestGetMenu()
        {
            var menu = this.GetMenu(nameof(Student_MenuFunctions));
            Assert.AreEqual("Students", menu.GetTitle());
        }

        [TestMethod]
        public void TestInvokeMenuAction()
        {
            var parameters = ActionHelpers.CreateParameters(("id", "3"));
            var stu = this.InvokeAction(nameof(Student_MenuFunctions), nameof(Student_MenuFunctions.FindStudentById), parameters, Methods.Get);
            stu.AssertHasTitle("James Java");
            var fullName = stu.GetMember(nameof(Student.FullName));
            fullName.AssertHasFieldName("Full Name").AssertHasValue("James Java").AssertNotEditable();
        }


       // "disabledReason": "Field not editable"


        //[TestMethod]
        //public void TestInvokeAction() {
        //    var foo = this.InvokeAction(new Key<Foo>("2"), nameof(FooFunctions.ResetName), Methods.Post);
        //    Assert.AreEqual("New Name", foo.GetMember(nameof(Foo.Name)).GetValue());
        //}

        //[TestMethod]
        //public void TestInvokeActionWithParameters() {
        //    var parameters = ActionHelpers.CreateParameters(("name", "Updated Name"));
        //    var foo = this.InvokeAction(new Key<Foo>("2"), nameof(FooFunctions.UpdateName), parameters, Methods.Post);
        //    Assert.AreEqual("Updated Name", foo.GetMember(nameof(Foo.Name)).GetValue());
        //}

        //[TestMethod]
        //public void TestCopyName() {
        //    // to show multiple server calls
        //    var foo1 = this.GetObject(new Key<Foo>("1"));
        //    var name = foo1.GetMember(nameof(Foo.Name)).GetValue();
        //    var parameters = ActionHelpers.CreateParameters(("name", name));
        //    var foo2 = this.InvokeAction(new Key<Foo>("2"), nameof(FooFunctions.UpdateName), parameters, Methods.Post);
        //    Assert.AreEqual(name, foo2.GetMember(nameof(Foo.Name)).GetValue());
        //}


        //[TestMethod]
        //public void TestCopyNameFrom()
        //{
        //    // to show reference parameter
        //    var foo1 = this.GetObject(new Key<Foo>("1"));
        //    var name = foo1.GetMember(nameof(Foo.Name)).GetValue();
        //    var parameters = ActionHelpers.CreateParameters(("from", foo1));
        //    var foo2 = this.InvokeAction(new Key<Foo>("2"), nameof(FooFunctions.UpdateNameFrom), parameters, Methods.Post);
        //    Assert.AreEqual(name, foo2.GetMember(nameof(Foo.Name)).GetValue());
        //}
    }
}