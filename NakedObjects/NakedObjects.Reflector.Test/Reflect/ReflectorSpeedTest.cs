// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Extensions;

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
    private Action<IServiceCollection> TestHook { get; } = services => { };

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

            //Assert.IsTrue(time < 500, $"Elapsed time was {time} milliseconds");

            Assert.AreEqual(162, AllObjectSpecImmutables(container).Length);
        }
    }
}