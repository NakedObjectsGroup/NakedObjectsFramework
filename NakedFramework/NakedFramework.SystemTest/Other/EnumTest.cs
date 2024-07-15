// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Other;

[TestFixture]
public class EnumTest : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
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
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new EnumDbContext() };

    protected Type[] ObjectTypes => new[] { typeof(Foo), typeof(Sexes), typeof(HairColours) };

    protected Type[] Services => new[] { typeof(SimpleRepository<Foo>) };

    [Test]
    public virtual void EnumParameter() {
        var foo = NewTestObject<Foo>();
        var act1 = foo.GetAction("Action1");
        var values = act1.Parameters[0].GetChoices();
        Assert.AreEqual(4, values.Length);
        Assert.AreEqual("Female", values[0].Title);
        Assert.AreEqual("Male", values[1].Title);
        Assert.AreEqual("Not Specified", values[2].Title);
        Assert.AreEqual("Unknown", values[3].Title);
    }

    [Test]
    public virtual void EnumParameterWithDefault() {
        var foo = NewTestObject<Foo>();
        var act1 = foo.GetAction("Action1");
        Assert.AreEqual(null, act1.Parameters[0].GetDefault());

        var act2 = foo.GetAction("Action2");
        Assert.AreEqual("Unknown", act2.Parameters[0].GetDefault().Title);
    }

    [Test]
    public virtual void EnumPropertyBasic() {
        var foo = NewTestObject<Foo>();

        var sex1 = foo.GetPropertyByName("Sex1");
        var values = sex1.GetChoices();
        Assert.AreEqual(4, values.Length);
        Assert.AreEqual("Female", values[0].Title);
        Assert.AreEqual("Male", values[1].Title);
        Assert.AreEqual("Not Specified", values[2].Title);
        Assert.AreEqual("Unknown", values[3].Title);

        sex1.AssertFieldEntryIsValid("Male");
        sex1.AssertFieldEntryInvalid("Man");
    }

    [Test]
    public virtual void EnumPropertyByteEnum() {
        var foo = NewTestObject<Foo>();

        var sex1 = foo.GetPropertyByName("Hair Colour1");
        var values = sex1.GetChoices();
        Assert.AreEqual(5, values.Length);
        Assert.AreEqual("Black", values[0].Title);
        Assert.AreEqual("White", values[4].Title);

        sex1.AssertFieldEntryIsValid("Brunette");
        sex1.AssertFieldEntryInvalid("Fair");
    }

    [Test]
    public virtual void EnumPropertyWithChoices() {
        var foo = NewTestObject<Foo>();

        var sex1 = foo.GetPropertyByName("Sex3").AssertValueIsEqual("Male");
        var values = sex1.GetChoices();
        Assert.AreEqual(2, values.Length);
        Assert.AreEqual("Male", values[0].Title);
        Assert.AreEqual("Female", values[1].Title);
    }

    [Test]
    public virtual void EnumPropertyWithChoicesAndDefault() {
        var foo = NewTestObject<Foo>();

        var sex1 = foo.GetPropertyByName("Sex4").AssertValueIsEqual("Male");
        var values = sex1.GetChoices();
        Assert.AreEqual(2, values.Length);
    }

    [Test]
    public virtual void EnumPropertyWithDefault() {
        var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
        //Property with no default
        foo.GetPropertyByName("Sex1").AssertValueIsEqual("");
        //Property with default
        foo.GetPropertyByName("Sex2").AssertValueIsEqual("Unknown");
    }

    [Test]
    public virtual void IntPropertyAsEnum() {
        var foo = NewTestObject<Foo>();

        var sex1 = foo.GetPropertyByName("Sex5");
        var values = sex1.GetChoices();
        Assert.AreEqual(4, values.Length);
        Assert.AreEqual("Female", values[0].Title);
        Assert.AreEqual("Male", values[1].Title);
        Assert.AreEqual("Not Specified", values[2].Title);
        Assert.AreEqual("Unknown", values[3].Title);
    }
}

#region Classes used in tests

public class EnumDbContext : DbContext {
    public const string DatabaseName = "TestEnums";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public EnumDbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new EnumDatabaseInitializer());

    public class EnumDatabaseInitializer : DropCreateDatabaseAlways<EnumDbContext> {
        protected override void Seed(EnumDbContext context) {
            context.Foos.Add(new Foo { Id = 1, Sex1 = Sexes.Male, Sex2 = Sexes.Male, Sex3 = Sexes.Male, Sex4 = Sexes.Male, Sex5 = 1, HairColour1 = HairColours.Black });
        }
    }
}

public class Foo {
    public virtual int Id { get; set; }

    #region Sex1

    public virtual Sexes Sex1 { get; set; }

    #endregion

    #region Sex5

    [EnumDataType(typeof(Sexes))]
    public virtual int Sex5 { get; set; }

    #endregion

    #region HairColour1

    public virtual HairColours HairColour1 { get; set; }

    #endregion

    #region Action1

    public void Action1(Sexes sex) { }

    #endregion

    #region Sex2

    public virtual Sexes Sex2 { get; set; }

    public Sexes DefaultSex2() => Sexes.Unknown;

    #endregion

    #region Sex3

    public virtual Sexes Sex3 { get; set; }

    public Sexes[] ChoicesSex3() {
        return new[] { Sexes.Male, Sexes.Female };
    }

    #endregion

    #region Sex4

    public virtual Sexes Sex4 { get; set; }

    public Sexes[] ChoicesSex4() {
        return new[] { Sexes.Male, Sexes.Female };
    }

    public Sexes DefaultSex4() => Sexes.Male;

    #endregion

    #region Action2

    public void Action2(Sexes sex) { }

    public Sexes Default0Action2() => Sexes.Unknown;

    #endregion
}

public enum Sexes {
    Male = 1,
    Female = 2,
    Unknown = 3,
    NotSpecified = 4
}

public enum HairColours : byte {
    Black = 1,
    Blond = 2,
    Brunette = 3,
    Grey = 4,
    White = 5
}

#endregion