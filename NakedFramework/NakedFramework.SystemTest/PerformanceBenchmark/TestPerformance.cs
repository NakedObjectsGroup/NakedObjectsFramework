// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedObjects.SystemTest.Performance;

[TestFixture]
public class TestPerformance : AbstractSystemTest<PerformanceDbContext> {
    [SetUp]
    public void SetUp() => StartTest();

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        PerformanceDbContext.Delete();
        var context = Activator.CreateInstance<PerformanceDbContext>();

        context.Database.Create();
        MyDbInitialiser.Seed(context);
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        PerformanceDbContext.Delete();
    }

    protected override Type[] ObjectTypes =>
        new[] {
            typeof(Qux)
        };

    protected override Type[] Services => new[] {
        typeof(SimpleRepository<Qux>)
    };

    private void GetSingleRandomQux() {
        var q = GetTestService("Quxes").GetAction("Get Random").InvokeReturnObject();
        Assert.IsNotNull(q);
    }

    [Test]
    public virtual void GetRandomBenchMark() {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        for (var i = 0; i < 1000; i++) {
            StartTest();
            GetSingleRandomQux();
            EndTest();
        }

        stopWatch.Stop();
        var time = stopWatch.ElapsedMilliseconds;
        // with dynamic 2929 ms
        // without dynamic 2918 ms
        var t = $"Test: {MethodBase.GetCurrentMethod().Name} \tElapsedTime: {time}ms\r\n";

        File.AppendAllText(@"..\..\..\..\..\benchmarks.txt", t);

        Assert.IsTrue(time < 3000, $"Elapsed time was {time} milliseconds");
    }
}

#region Classes used by tests

public class PerformanceDbContext : DbContext {
    public const string DatabaseName = "TestPerformance";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public PerformanceDbContext() : base(Cs) { }

    public DbSet<Qux> Quxes { get; set; }

    public static void Delete() => Database.Delete(Cs);
}

public class MyDbInitialiser {
    public static void Seed(PerformanceDbContext context) {
        for (var i = 0; i < 100; i++) {
            NewQux($"Qux {i}", context);
        }

        context.SaveChanges();
    }

    private static Qux NewQux(string name, PerformanceDbContext context) {
        var q = new Qux { Name = name };
        context.Quxes.Add(q);
        return q;
    }
}

public class Qux {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Name { get; set; }
}

#endregion