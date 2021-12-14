// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Error;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.ParallelReflector.Component;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Extensions;
using NakedObjects.Reflector.Component;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.SystemTest.MultiReflector;

internal class MultiReflectorOrder<T> : IReflectorOrder<T> {
    public int Order => typeof(T) switch {
        { } t when t.IsAssignableTo(typeof(SystemTypeReflector)) => 0,
        { } t when t.IsAssignableTo(typeof(ObjectReflector)) => 1,
        { } t when t.IsAssignableTo(typeof(FunctionalReflector)) => 1,
        _ => throw new InitialisationException($"Unexpected reflector type {typeof(T)}")
    };
}

[TestFixture]
public class MultiReflectorTest : AbstractSystemTest<FooContext> {
    [SetUp]
    public void Initialize() {
        StartTest();
    }

    [TearDown]
    public void Cleanup() {
        EndTest();
    }

    [OneTimeSetUp]
    public void SetupTestFixture() {
        FooContext.Delete();
        var context = Activator.CreateInstance<FooContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void TearDownTest() {
        CleanupNakedObjectsFramework(this);
        FooContext.Delete();
    }

    protected override Action<NakedFrameworkOptions> AddCoreOptions => builder => {
        base.AddCoreOptions(builder);
        builder.Services.AddSingleton(typeof(IReflectorOrder<>), typeof(MultiReflectorOrder<>));
    };

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => builder => builder.AddNakedFunctions(NakedFunctionsOptions);

    protected override Type[] ObjectTypes => new[] { typeof(Foo) };

    protected override Type[] Services => new[] { typeof(SimpleRepository<Foo>) };

    protected override Type[] Records => new[] { typeof(Bar) };

    protected override Func<Type[], Type[]> SupportedSystemTypes => t => new[] { typeof(string), typeof(int) };

    [Test]

    [Ignore("Needs reworking after reflector work")]
    public virtual void AllSpecs() {
        var allSpecs = NakedFramework.MetamodelManager.AllSpecs;
        Assert.AreEqual(10, allSpecs.Length);
    }
}

public class FooContext : DbContext {
    public const string DatabaseName = "MultiReflectorTest";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public FooContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public static void Delete() => Database.Delete(Cs);
}

public class Foo {
    [Title]
    public virtual string Name { get; set; }

    [Key]
    public virtual int Id { get; set; }

    public virtual Bar BarProperty { get; set; }
}

public record Bar {
    public virtual string Name { get; set; }

    [Key]
    public virtual int Id { get; set; }

    public override string ToString() => Name;
}