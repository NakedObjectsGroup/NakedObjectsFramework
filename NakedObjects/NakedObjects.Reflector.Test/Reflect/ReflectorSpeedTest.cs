// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using model1000;
using model500;
using model5000;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Configuration;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.Facet;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.Reflect;

public static class NakedObjectsRunSettings {
    // Unintrospected specs: AdventureWorksModel.SalesOrderHeader+SalesReasonCategories,AdventureWorksModel.Sales.QuickOrderForm,

    private static Type[] AllAdventureWorksTypes =>
        Assembly.GetAssembly(typeof(AssemblyHook)).GetTypes().Where(t => t.IsPublic).Where(t => t.Namespace == "AdventureWorksModel").Append(typeof(SalesOrderHeader.SalesReasonCategories)).ToArray();

    public static Type[] Types => AllAdventureWorksTypes;

    public static Type[] Services =>
        new[] {
            typeof(CustomerRepository),
            typeof(OrderRepository),
            typeof(ProductRepository),
            typeof(EmployeeRepository),
            typeof(SalesRepository),
            typeof(SpecialOfferRepository),
            typeof(PersonRepository),
            typeof(VendorRepository),
            typeof(PurchaseOrderRepository),
            typeof(WorkOrderRepository),
            typeof(OrderContributedActions),
            typeof(CustomerContributedActions),
            typeof(SpecialOfferContributedActions),
            typeof(ServiceWithNoVisibleActions)
        };

    /// <summary>
    ///     Return an array of IMenus (obtained via the factory, then configured) to
    ///     specify the Main Menus for the application. If none are returned then
    ///     the Main Menus will be derived automatically from the Services.
    /// </summary>
    public static IMenu[] MainMenus(IMenuFactory factory) {
        var customerMenu = factory.NewMenu<CustomerRepository>();
        CustomerRepository.Menu(customerMenu);
        var salesMenu = factory.NewMenu<SalesRepository>();
        SalesRepository.Menu(salesMenu);
        return new[] {
            customerMenu,
            factory.NewMenu<OrderRepository>(true),
            factory.NewMenu<ProductRepository>(true),
            factory.NewMenu<EmployeeRepository>(true),
            salesMenu,
            factory.NewMenu<SpecialOfferRepository>(true),
            factory.NewMenu<PersonRepository>(true),
            factory.NewMenu<VendorRepository>(true),
            factory.NewMenu<PurchaseOrderRepository>(true),
            factory.NewMenu<WorkOrderRepository>(true),
            factory.NewMenu<ServiceWithNoVisibleActions>(true, "Empty")
        };
    }
}

[TestClass]
public class ReflectorSpeedTest {
    private static readonly Dictionary<string, long> Stats = new();
    private Action<IServiceCollection> TestHook { get; } = services => { };

    private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
        Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

        // checks for fields and Objects actions 

        var error = newCache.AllSpecifications().Where(s => s.OrderedFields.Any() && s.OrderedFields.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

        Assert.IsTrue(newCache.AllSpecifications().Select(s => s.OrderedFields).All(fs => !fs.Any() || fs.All(f => f != null)), error);

        error = newCache.AllSpecifications().Where(s => s.OrderedObjectActions.Any() && s.OrderedObjectActions.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

        Assert.IsTrue(newCache.AllSpecifications().Select(s => s.OrderedObjectActions).All(fs => !fs.Any() || fs.All(f => f != null)), error);

        var zippedSpecs = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new { a, b });

        foreach (var item in zippedSpecs) {
            AssertSpecification(item.a, item.b);
        }

        var zippedMenus = cache.MainMenus().Zip(newCache.MainMenus(), (a, b) => new { a, b });

        foreach (var item in zippedMenus) {
            AssertMenu(item.a, item.b);
        }
    }

    private IHostBuilder CreateHostBuilder(string[] args, Action<NakedFrameworkOptions> setup) {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureServices((hostContext, services) => { RegisterTypes(services, setup); });
    }

    protected (IServiceProvider, IHost) GetContainer(Action<NakedFrameworkOptions> setup) {
        ImmutableSpecFactory.ClearCache();
        var hostBuilder = CreateHostBuilder(Array.Empty<string>(), setup).Build();
        return (hostBuilder.Services, hostBuilder);
    }

    protected virtual void RegisterTypes(IServiceCollection services, Action<NakedFrameworkOptions> setup) {
        services.AddNakedFramework(setup);
        TestHook(services);
    }

    private ITypeSpecBuilder[] AllObjectSpecImmutables(IServiceProvider provider) {
        var metaModel = provider.GetService<IMetamodel>();
        return metaModel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();
    }

    private static Type[] CreateTypes() {
        var types = new List<Type>();

        var ett = new[] { typeof(Ordering), typeof(EmailStatus), typeof(EmailPromotion), typeof(ProductLineEnum), typeof(ProductClassEnum), typeof(SalesOrderHeader.SalesReasonCategories), typeof(OrderStatus) };

        var gtt = new[] {
            typeof(DefaultedFacetUsingDefaultsProvider<>),
            typeof(ParseableFacetUsingParser<>),
            typeof(TitleFacetUsingParser<>),
            typeof(ValueFacetFromSemanticProvider<>),
            typeof(EnumValueSemanticsProvider<>),
            typeof(ArrayValueSemanticsProvider<>)
        };

        foreach (var et in ett) {
            foreach (var gt in gtt) {
                var t = gt.MakeGenericType(et);
                types.Add(t);
            }
        }

        return types.ToArray();
    }

    private static Type[] AdditionalKnownTypes() {
        var a = Assembly.GetAssembly(typeof(LoadingCallbackFacetNull));
        var tt = a.GetTypes().Where(t => t is { IsSerializable: true, IsPublic: true }).ToArray();

        tt = tt.Union(CreateTypes()).ToArray();

        return tt;
    }

    [TestCleanup]
    public void CleanUp() {
        ReflectorDefaults.ParallelDegree = 0;
        ImmutableSpecFactory.ClearCache();
        OutputStats();
    }

    public void OutputStats() {
        var testCount = GetType().GetMethods().Count(m => m.GetCustomAttribute(typeof(TestMethodAttribute)) is not null);
        if (Stats.Count == testCount) {
            Console.WriteLine("Stats:");
            var longest = Stats.Max(kvp => kvp.Key.Length);

            foreach (var kvp in Stats.OrderByDescending(kvp => kvp.Key)) {
                var t = $"Test: {kvp.Key.PadRight(longest)} \tElapsedTime: {kvp.Value}ms\r\n";

                Console.WriteLine(t);
                File.AppendAllText(@"..\..\..\..\..\benchmarks.txt", t);
            }
        }
    }

    private static void Record(string test, long elapsed) => Stats[test] = elapsed;

    [TestMethod]
    public void ReflectAWTypesBenchMark() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = NakedObjectsRunSettings.Types;
                options.DomainModelServices = NakedObjectsRunSettings.Services;
                options.NoValidate = true;
            });
            coreOptions.MainMenus = NakedObjectsRunSettings.MainMenus;
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            var stopWatch = new Stopwatch();
            var mb = container.GetService<IModelBuilder>();
            stopWatch.Start();
            mb.Build();
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was {time} milliseconds");
            Record(MethodBase.GetCurrentMethod().Name, time);

            //Assert.IsTrue(time < 500, $"Elapsed time was {time} milliseconds");

            Assert.AreEqual(163, AllObjectSpecImmutables(container).Length);
        }
    }

    public void ReflectTestModelTypesBenchMark(Type[] testModelTypes, string test) {
        void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = testModelTypes;
                //options.DomainModelServices = NakedObjectsRunSettings.Services;
                options.NoValidate = true;
            });
            //coreOptions.MainMenus = NakedObjectsRunSettings.MainMenus;
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            var stopWatch = new Stopwatch();
            var mb = container.GetService<IModelBuilder>();
            GC.Collect(4, GCCollectionMode.Forced, true);
            stopWatch.Start();
            mb.Build();
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was {time} milliseconds");
            Record(test, time);

            //Assert.IsTrue(time < 500, $"Elapsed time was {time} milliseconds");

            //Assert.AreEqual(1055, AllObjectSpecImmutables(container).Length);
        }
    }

    [TestMethod]
    public void ReflectTestModel5000TypesBenchMark() {
        ReflectTestModelTypesBenchMark(model5000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel1000TypesBenchMarkParallelDefault() {
        ReflectTestModelTypesBenchMark(model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel1000TypesBenchMarkParallel1() {
        ReflectorDefaults.ParallelDegree = 1;
        ReflectTestModelTypesBenchMark(model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel1000TypesBenchMarkParallel2() {
        ReflectorDefaults.ParallelDegree = 2;
        ReflectTestModelTypesBenchMark(model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel1000TypesBenchMarkParallel3() {
        ReflectorDefaults.ParallelDegree = 3;
        ReflectTestModelTypesBenchMark(model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel1000TypesBenchMarkParallel4() {
        ReflectorDefaults.ParallelDegree = 3;
        ReflectTestModelTypesBenchMark(model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void ReflectTestModel500TypesBenchMark() {
        ReflectTestModelTypesBenchMark(model500_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    public void SerializeAWTypesBenchMark(string fileName, string test) {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = NakedObjectsRunSettings.Types;
                options.DomainModelServices = NakedObjectsRunSettings.Services;
                options.NoValidate = true;
            });
            coreOptions.MainMenus = NakedObjectsRunSettings.MainMenus;
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            var curDir = Directory.GetCurrentDirectory();
            var testDir = Path.Combine(curDir, "testserialize");
            Directory.CreateDirectory(testDir);
            Directory.GetFiles(testDir).ForEach(File.Delete);
            var file = Path.Combine(testDir, fileName);

            var metamodelBuilder = container.GetService<IMetamodelBuilder>();
            var mb = container.GetService<IModelBuilder>();

            mb.Build(file, AdditionalKnownTypes());
            var cache1 = metamodelBuilder?.Cache;

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            mb.RestoreFromFile(file, AdditionalKnownTypes());
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;
            var cache2 = metamodelBuilder?.Cache;

            Console.WriteLine($"Elapsed time was {time} milliseconds");
            Record(test, time);

            Assert.AreEqual(163, AllObjectSpecImmutables(container).Length);
            Assert.IsNotNull(cache1);
            Assert.IsNotNull(cache2);
            Assert.AreNotEqual(cache1, cache2);
            CompareCaches(cache1, cache2);
        }
    }

    [TestMethod]
    public void BinarySerializeAWTypesBenchMark() {
        SerializeAWTypesBenchMark("metadata.bin", MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void BinarySerializeAWTypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeAWTypesBenchMark("metadata.bin", MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }

    public void SerializeTestModelTypesBenchMark(string fileName, Type[] testModelTypes, string test) {
        void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = testModelTypes;
                //options.DomainModelServices = NakedObjectsRunSettings.Services;
                options.NoValidate = true;
            });
            //coreOptions.MainMenus = NakedObjectsRunSettings.MainMenus;
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            var curDir = Directory.GetCurrentDirectory();
            var testDir = Path.Combine(curDir, "testserialize");
            Directory.CreateDirectory(testDir);
            Directory.GetFiles(testDir).ForEach(File.Delete);
            var file = Path.Combine(testDir, fileName);

            var metamodelBuilder = container.GetService<IMetamodelBuilder>();
            var mb = container.GetService<IModelBuilder>();

            mb.Build(file, AdditionalKnownTypes());
            var cache1 = metamodelBuilder?.Cache;

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            mb.RestoreFromFile(file, AdditionalKnownTypes());
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;
            var cache2 = metamodelBuilder?.Cache;

            Console.WriteLine($"Elapsed time was {time} milliseconds");
            Record(test, time);

            Assert.IsNotNull(cache1);
            Assert.IsNotNull(cache2);
            Assert.AreNotEqual(cache1, cache2);
            CompareCaches(cache1, cache2);
        }
    }

    [TestMethod]
    public void BinarySerializeTestModel1000TypesBenchMark() {
        SerializeTestModelTypesBenchMark("metadata.bin", model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void BinarySerializeTestModel1000TypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeTestModelTypesBenchMark("metadata.bin", model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }

    [TestMethod]
    public void BinarySerializeTestModel5000TypesBenchMark() {
        SerializeTestModelTypesBenchMark("metadata.bin", model5000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void BinarySerializeTestModel5000TypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeTestModelTypesBenchMark("metadata.bin", model5000_Config.Types(), MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }

    [TestMethod]
    public void BinarySerializeTestModel500TypesBenchMark() {
        SerializeTestModelTypesBenchMark("metadata.bin", model500_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void BinarySerializeTestModel500TypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeTestModelTypesBenchMark("metadata.bin", model500_Config.Types(), MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }

    [TestMethod]
    public void XmlSerializeTestModel100TypesBenchMark() {
        SerializeTestModelTypesBenchMark("metadata.xml", model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void XmlSerializeTestModel100TypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeTestModelTypesBenchMark("metadata.xml", model1000_Config.Types(), MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }

    [TestMethod]
    public void XmlSerializeAWTypesBenchMark() {
        SerializeAWTypesBenchMark("metadata.xml", MethodBase.GetCurrentMethod().Name);
    }

    [TestMethod]
    public void XmlSerializeAWTypesBenchMarkWithJit() {
        ReflectorDefaults.JitSerialization = true;
        try {
            SerializeAWTypesBenchMark("metadata.xml", MethodBase.GetCurrentMethod().Name);
        }
        finally {
            ReflectorDefaults.JitSerialization = false;
        }
    }
}