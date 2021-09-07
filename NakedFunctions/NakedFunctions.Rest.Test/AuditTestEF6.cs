// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Audit;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test.TestCase;
using NakedFunctions.Audit;
using NakedFunctions.Reflector.Audit;
using NakedFunctions.Rest.Test.Data;
using NakedFunctions.Rest.Test.Data.Sub;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static NakedFunctions.Rest.Test.AuditHelpers;

namespace NakedFunctions.Rest.Test {


    public static class AuditHelpers {
        public static string FullName<T>() => typeof(T).FullName;

        public static void ResetDefaultAudit() {
            DefaultAuditor.ActionInvokedCount = 0;
        }

        public static void ResetMenuAudit() {
            MenuAuditor.ActionInvokedCount = 0;
        }

        public static void AssertDefaultAudit(int expectedVisible) {
            Assert.AreEqual(expectedVisible, DefaultAuditor.ActionInvokedCount);
        }

        public static void ResetBarAudit() {
            BarAuditor.ActionInvokedCount = 0;
        }

        public static void AssertBarAudit(int expectedVisible) {
            Assert.AreEqual(expectedVisible, BarAuditor.ActionInvokedCount);
        }

        public static void ResetQuxAudit() {
            QuxAuditor.ActionInvokedCount = 0;
        }

        public static void AssertQuxAudit(int expectedVisible) {
            Assert.AreEqual(expectedVisible, QuxAuditor.ActionInvokedCount);
        }

        public static void AssertMenuAudit(int expectedVisible) {
            Assert.AreEqual(expectedVisible, MenuAuditor.ActionInvokedCount);
        }
    }

    public class MenuAuditor : IMenuAuditor {
        public static int ActionInvokedCount = 0;

        public IContext ActionInvoked(string actionName, string menuName, bool queryOnly, object[] withParameters, IContext context) {
            Assert.IsNotNull(actionName);
            Assert.IsNotNull(menuName);
            Assert.IsNotNull(queryOnly);
            Assert.IsNotNull(withParameters);
            Assert.IsNotNull(context);
            ActionInvokedCount++;
            return context;
        }
    }

    public class DefaultAuditor : ITypeAuditor {
        public static int ActionInvokedCount = 0;

        public IContext ActionInvoked(string actionName, object onObject, bool queryOnly, object[] withParameters, IContext context) {
            Assert.IsNotNull(actionName);
            Assert.IsNotNull(onObject);
            Assert.IsNotNull(queryOnly);
            Assert.IsNotNull(withParameters);
            Assert.IsNotNull(context);
            ActionInvokedCount++;
            return context;
        }

        public IContext ObjectUpdated(object updatedObject, IContext context) => throw new NotImplementedException();

        public IContext ObjectPersisted(object updatedObject, IContext context) => throw new NotImplementedException();
    }

    public class BarAuditor : ITypeAuditor {
        public static int ActionInvokedCount = 0;

        public IContext ActionInvoked(string actionName, object onObject, bool queryOnly, object[] withParameters, IContext context) {
            Assert.IsNotNull(actionName);
            Assert.IsNotNull(onObject);
            Assert.IsNotNull(queryOnly);
            Assert.IsNotNull(withParameters);
            Assert.IsNotNull(context);
            ActionInvokedCount++;
            return context;
        }

        public IContext ObjectUpdated(object updatedObject, IContext context) => throw new NotImplementedException();

        public IContext ObjectPersisted(object updatedObject, IContext context) => throw new NotImplementedException();
    }

    public class QuxAuditor : ITypeAuditor {
        public static int ActionInvokedCount = 0;

        public IContext ActionInvoked(string actionName, object onObject, bool queryOnly, object[] withParameters, IContext context) {
            Assert.IsNotNull(actionName);
            Assert.IsNotNull(onObject);
            Assert.IsNotNull(queryOnly);
            Assert.IsNotNull(withParameters);
            Assert.IsNotNull(context);
            ActionInvokedCount++;
            return context;
        }

        public IContext ObjectUpdated(object updatedObject, IContext context) => throw new NotImplementedException();

        public IContext ObjectPersisted(object updatedObject, IContext context) => throw new NotImplementedException();
    }



    public class AuditTestEF6 : AcceptanceTestCase {
        protected override Type[] Functions { get; } = {
            typeof(BarFunctions),
            typeof(QuxFunctions),
            typeof(FooFunctions),
            typeof(FooSubFunctions),
            typeof(FooMenuFunctions),
            typeof(QuxMenuFunctions)
        };

        protected override Type[] Records { get; } = {
            typeof(Foo),
            typeof(Bar),
            typeof(Qux),
            typeof(FooSub)
        };

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextCreators =>
            new Func<IConfiguration, DbContext>[] { config => new AuthDbContext() };

        protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };


        protected virtual void CleanUpDatabase() {
            ObjectDbContext.Delete();
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) => new[] {typeof(FooMenuFunctions), typeof(QuxMenuFunctions)}.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        protected override IAuditConfiguration AuditConfiguration {
            get {
                var config = new AuditConfiguration<DefaultAuditor, MenuAuditor>();
                config.AddNamespaceAuditor<BarAuditor>(typeof(Bar).FullName);
                config.AddNamespaceAuditor<QuxAuditor>(typeof(Qux).FullName);
                return config;
            }
        }

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ObjectReflectorConfiguration.NoValidate = true;
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            CleanUpDatabase();
        }

        protected RestfulObjectsControllerBase Api() {
            var sp = GetConfiguredContainer();
            var api = sp.GetService<RestfulObjectsController>();
            return Helpers.SetMockContext(api, sp);
        }

        private JObject GetObject(string type, string id) {
            var api = Api().AsGet();
            var result = api.GetObject(type, id);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }

        [Test]
        public void DefaultAuditorCalledForNonSpecificType() {
            ResetDefaultAudit();
            ResetBarAudit();
            ResetQuxAudit();
            ResetMenuAudit();

            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

            var result = api.GetInvoke(FullName<Foo>(), "1", nameof(FooFunctions.Act1), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);

            AssertDefaultAudit(1);
            AssertBarAudit(0);
            AssertQuxAudit(0);
            AssertMenuAudit(0);
        }

        [Test]
        public void NamespaceAuditorCalledForSpecificType() {
            ResetDefaultAudit();
            ResetBarAudit();
            ResetQuxAudit();
            ResetMenuAudit();

            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

            var result = api.GetInvoke(FullName<Bar>(), "1", nameof(BarFunctions.Act1), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);

            AssertDefaultAudit(0);
            AssertBarAudit(1);
            AssertQuxAudit(0);
            AssertMenuAudit(0);
        }

        [Test]
        public void MenuAuditorCalledForNonSpecificMenu() {
            ResetDefaultAudit();
            ResetBarAudit();
            ResetQuxAudit();
            ResetMenuAudit();

            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

            var result = api.GetInvokeOnMenu(nameof(FooMenuFunctions), nameof(FooMenuFunctions.Act1), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);

            AssertDefaultAudit(0);
            AssertBarAudit(0);
            AssertQuxAudit(0);
            AssertMenuAudit(1);
        }
    }

    public class MenuAuditTestEF6 : AcceptanceTestCase {
        

        protected override Type[] Functions { get; } = {
            typeof(BarFunctions),
            typeof(QuxFunctions),
            typeof(FooFunctions),
            typeof(FooSubFunctions),
            typeof(FooMenuFunctions),
        };

        protected override Type[] Records { get; } = {
            typeof(Foo),
            typeof(Bar),
            typeof(Qux),
            typeof(FooSub)
        };

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextCreators =>
            new Func<IConfiguration, DbContext>[] { config => new AuthDbContext() };

        protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };



        protected virtual void CleanUpDatabase() {
            ObjectDbContext.Delete();
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) => new[] { typeof(FooMenuFunctions) }.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ObjectReflectorConfiguration.NoValidate = true;
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            CleanUpDatabase();
        }

        protected RestfulObjectsControllerBase Api() {
            var sp = GetConfiguredContainer();
            var api = sp.GetService<RestfulObjectsController>();
            return Helpers.SetMockContext(api, sp);
        }

        private JObject GetObject(string type, string id) {
            var api = Api().AsGet();
            var result = api.GetObject(type, id);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }
    }

}