// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Container;

[TestFixture]
public class TestContainer : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
        NakedFramework = ServiceScope.ServiceProvider.GetService<INakedFramework>();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.AddEF6Persistor(options => { options.ContextCreators = ContextCreators; });
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = ObjectTypes;
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
    }

    protected INakedFramework NakedFramework { get; set; }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new ContainerDbContext() };

    protected Type[] ObjectTypes => new[] { typeof(Object1), typeof(Object2), typeof(ViewModel2), typeof(TestEnum) };

    protected Type[] Services => new[] { typeof(SimpleRepository<Object1>) };

   

    private Object1 TestObject => NakedFramework.LifecycleManager.CreateInstance((IObjectSpec)NakedFramework.MetamodelManager.GetSpecification(typeof(Object1))).Object as Object1;

    [Test]
    public void DefaultsTransient() {
        var testObject = TestObject;
        ClassicAssert.IsNotNull(testObject?.Container);

        var o2 = testObject.Container.NewTransientInstance<Object2>();

        ClassicAssert.AreEqual(new DateTime(), o2.TestDateTime);
        ClassicAssert.IsNull(o2.TestNullableDateTime);
        ClassicAssert.AreEqual(0, o2.TestInt);
        ClassicAssert.IsNull(o2.TestNullableInt);

        ClassicAssert.AreEqual(TestEnum.Value1, o2.TestEnum);
        ClassicAssert.IsNull(o2.TestNullableEnum);

        ClassicAssert.AreEqual(0, o2.TestEnumDt);
        ClassicAssert.IsNull(o2.TestNullableEnumDt);
    }

    [Test]
    public void DefaultsViewModel() {
        var testObject = TestObject;
        ClassicAssert.IsNotNull(testObject.Container);

        var vm = testObject.NewViewModel();

        ClassicAssert.AreEqual(new DateTime(), vm.TestDateTime);
        ClassicAssert.IsNull(vm.TestNullableDateTime);
        ClassicAssert.AreEqual(0, vm.TestInt);
        ClassicAssert.IsNull(vm.TestNullableInt);

        ClassicAssert.AreEqual(TestEnum.Value1, vm.TestEnum);
        ClassicAssert.IsNull(vm.TestNullableEnum);

        ClassicAssert.AreEqual(0, vm.TestEnumDt);
        ClassicAssert.IsNull(vm.TestNullableEnumDt);
    }
}

#region Domain classes used by tests

public class ContainerDbContext : DbContext {
    public const string DatabaseName = "TestContainer";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public ContainerDbContext() : base(Cs) { }
    public DbSet<Object1> Object1 { get; set; }
    public static void Delete() => Database.Delete(Cs);
}

public class Object1 {
    [NakedObjectsIgnore] // as container is public for test
    public IDomainObjectContainer Container { get; set; }

    public virtual int Id { get; set; }

    public ViewModel2 NewViewModel() => Container.NewViewModel<ViewModel2>();
}

public class Object2 {
    public virtual int Id { get; set; }

    public DateTime TestDateTime { get; set; }

    public DateTime? TestNullableDateTime { get; set; }

    public int TestInt { get; set; }

    public int? TestNullableInt { get; set; }

    public TestEnum TestEnum { get; set; }

    public TestEnum? TestNullableEnum { get; set; }

    [EnumDataType(typeof(TestEnum))]
    public int TestEnumDt { get; set; }

    [EnumDataType(typeof(TestEnum))]
    public int? TestNullableEnumDt { get; set; }
}

public enum TestEnum {
    Value1,
    Value2
}

public class ViewModel2 : IViewModel {
    public virtual int Id { get; set; }

    public DateTime TestDateTime { get; set; }

    public DateTime? TestNullableDateTime { get; set; }

    public int TestInt { get; set; }

    public int? TestNullableInt { get; set; }

    public TestEnum TestEnum { get; set; }

    public TestEnum? TestNullableEnum { get; set; }

    [EnumDataType(typeof(TestEnum))]
    public int TestEnumDt { get; set; }

    [EnumDataType(typeof(TestEnum))]
    public int? TestNullableEnumDt { get; set; }

    #region IViewModel Members

    public string[] DeriveKeys() => Array.Empty<string>();

    public void PopulateUsingKeys(string[] keys) { }

    #endregion
}

#endregion