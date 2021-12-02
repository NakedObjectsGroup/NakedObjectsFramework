// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFunctions.Reflector.Extensions;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.Test.Reflect;

namespace NakedFunctions.Reflector.Test.Component;

[TestClass]
public class ReflectorTest {
    private Action<IServiceCollection> TestHook { get; } = services => { };

    private IHostBuilder CreateHostBuilder(string[] args, Action<NakedFrameworkOptions> setup) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => { RegisterTypes(services, setup); });

    protected (IServiceProvider, IHost) GetContainer(Action<NakedFrameworkOptions> setup) {
        ImmutableSpecFactory.ClearCache();
        var hostBuilder = CreateHostBuilder(new string[] { }, setup).Build();
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

    private static void EmptyObjectSetup(NakedObjectsOptions options) {
        options.DomainModelTypes = Array.Empty<Type>();
        options.DomainModelServices = Array.Empty<Type>();
        options.NoValidate = true;
    }

    private static string FullName<T>() => $"{typeof(T).FullName}";

    [TestMethod]
    public void ReflectNoTypes() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.SupportedSystemTypes = t => Array.Empty<Type>();

            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = Array.Empty<Type>();
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.IsFalse(specs.Any());
        }
    }

    [TestMethod]
    public void ReflectSimpleType() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            //
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);

            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());
            var propertySpec = spec.OrderedFields.First();
            var facet = propertySpec.GetFacet<IDisabledFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(WhenTo.Always, facet.Value);
        }
    }

    [TestMethod]
    public void ReflectSimpleFunction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(SimpleFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
        }
    }

    [TestMethod]
    public void ReflectTupleFunction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.SupportedSystemTypes = t => new[] { typeof(IQueryable<>), typeof(IList<>) };

            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(TupleFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(TupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs);
        }
    }

    [TestMethod]
    public void ReflectUnsupportedTuple() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(UnsupportedTupleFunctions) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            try {
                container.GetService<IModelBuilder>()?.Build();
                Assert.Fail("exception expected");
            }
            catch (AggregateException ae) {
                var re = ae.InnerExceptions.FirstOrDefault();
                Assert.IsInstanceOfType(re, typeof(ReflectionException));
                Assert.AreEqual("Cannot reflect empty tuple on NakedFunctions.Reflector.Test.Component.UnsupportedTupleFunctions.TupleFunction", re.Message);
            }
        }
    }

    [TestMethod]
    public void ReflectSimpleInjectedFunction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.SupportedSystemTypes = t => new[] { typeof(IQueryable<>) };

            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(SimpleInjectedFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleInjectedFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
        }
    }

    [TestMethod]
    public void ReflectNavigableType() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(NavigableClass), typeof(SimpleClass) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
        }
    }

    [TestMethod]
    public void ReflectBoundedType() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(BoundedClass) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<BoundedClass>());
            Assert.IsTrue(spec.IsBoundedSet());
        }
    }

    [TestMethod]
    public void ReflectCreateNewAction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(CreateNewFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpec = spec.OrderedContributedActions.First();
            var facet = actionSpec.GetFacet<ICreateNewFacet>();
            Assert.IsNotNull(facet);
        }
    }

    [TestMethod]
    public void ReflectDisplayAsPropertyAction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(DisplayAsPropertyFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var propertySpec = spec.OrderedFields[0];
            var facet = propertySpec.GetFacet<IDisplayAsPropertyFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(false, propertySpec.ReturnSpec.IsCollection);
            Assert.AreEqual("Always disabled", propertySpec.GetFacet<IDisabledFacet>().DisabledReason(null));

            propertySpec = spec.OrderedFields[1];
            facet = propertySpec.GetFacet<IDisplayAsPropertyFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, propertySpec.ReturnSpec.IsCollection);
            Assert.AreEqual("Always disabled", propertySpec.GetFacet<IDisabledFacet>().DisabledReason(null));
        }
    }

    [TestMethod]
    public void ReflectPluralClass() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(PluralClass) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<PluralClass>());
            var facet = spec.GetFacet<IPluralFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Class Plural", facet.Value);
        }
    }

    [TestMethod]
    public void ReflectIgnoredProperty() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(IgnoredClass) };
                    options.DomainFunctions = Array.Empty<Type>();
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<IgnoredClass>());
            Assert.AreEqual(0, spec.OrderedFields.Count);
        }
    }

    private static void AssertParm(IActionSpecImmutable actionSpec, object value) {
        var parmSpec = actionSpec.Parameters[1];
        var facet = parmSpec.GetFacet<IActionDefaultsFacet>();
        Assert.IsNotNull(facet);

        var (defaultValue, type) = facet.GetDefault(null, null);
        if (value is DateTime dt) {
            Assert.AreEqual(dt.ToString(), defaultValue.ToString());
        }
        else {
            Assert.AreEqual(value, defaultValue);
        }

        Assert.AreEqual("Explicit", type.ToString());
    }

    [TestMethod]
    public void ReflectDefaultValueParameter() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(ParameterDefaultClass) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            AssertParm(spec.OrderedContributedActions[0], true);
            AssertParm(spec.OrderedContributedActions[1], (byte)66);
            AssertParm(spec.OrderedContributedActions[2], 'g');
            AssertParm(spec.OrderedContributedActions[3], DateTime.UtcNow.AddDays(35));
            AssertParm(spec.OrderedContributedActions[4], 56.23);
            AssertParm(spec.OrderedContributedActions[5], (float)22.82);
            AssertParm(spec.OrderedContributedActions[6], 72);
            AssertParm(spec.OrderedContributedActions[7], (long)91);
            AssertParm(spec.OrderedContributedActions[8], (short)30);
            AssertParm(spec.OrderedContributedActions[9], "a default");
        }
    }

    [TestMethod]
    public void ReflectRangeParameter() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(RangeClass) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var parmSpec = spec.OrderedContributedActions.First().Parameters[1];
            var facet = parmSpec.GetFacet<IRangeFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual(1, facet.Min);
            Assert.AreEqual(56, facet.Max);
        }
    }

    [TestMethod]
    public void ReflectDescribedAs() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(DescribedAsClass) };
                    options.DomainFunctions = new[] { typeof(DescribedAsFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<DescribedAsClass>());

            var facet = spec.GetFacet<IDescribedAsFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Class Description", facet.Value);

            var propertySpec = spec.OrderedFields.First();

            facet = propertySpec.GetFacet<IDescribedAsFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Property Description", facet.Value);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IDescribedAsFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Function Description", facet.Value);
        }
    }

    [TestMethod]
    public void ReflectRenderEagerly() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(RenderEagerlyClass) };
                    options.DomainFunctions = new[] { typeof(RenderEagerlyFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<RenderEagerlyClass>());

            var facet = spec.GetFacet<IEagerlyFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(Do.Rendering, facet.What);

            var propertySpec = spec.OrderedFields.First();

            facet = propertySpec.GetFacet<IEagerlyFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(Do.Rendering, facet.What);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IEagerlyFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(Do.Rendering, facet.What);
        }
    }

    [TestMethod]
    public void ReflectTableView() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(TableViewClass) };
                    options.DomainFunctions = new[] { typeof(TableViewFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<TableViewClass>());

            var propertySpec = spec.OrderedFields.First();

            var facet = propertySpec.GetFacet<ITableViewFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, facet.Title);

            var actionSpec1 = spec.OrderedContributedActions.First();
            var actionSpec2 = spec.OrderedContributedActions[1];

            facet = actionSpec1.GetFacet<ITableViewFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, facet.Title);

            facet = actionSpec2.GetFacet<ITableViewFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, facet.Title);
        }
    }

    [TestMethod]
    public void ReflectMask() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(MaskClass) };
                    options.DomainFunctions = new[] { typeof(MaskFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<MaskClass>());

            var propertySpec = spec.OrderedFields.First();
            var facet = propertySpec.GetFacet<IMaskFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Property Mask", facet.Value);
        }
    }

    [TestMethod]
    public void ReflectOptionally() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(OptionallyClass) };
                    options.DomainFunctions = new[] { typeof(OptionallyFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<OptionallyClass>());

            var propertySpec1 = spec.OrderedFields.First();
            var propertySpec2 = spec.OrderedFields[1];

            var facet = propertySpec1.GetFacet<IMandatoryFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, facet.IsMandatory);
            Assert.AreEqual(false, facet.IsOptional);

            var actionSpec = spec.OrderedContributedActions.First();

            var parmSpec1 = actionSpec.Parameters[1];
            var parmSpec2 = actionSpec.Parameters[2];

            facet = parmSpec1.GetFacet<IMandatoryFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(false, facet.IsMandatory);
            Assert.AreEqual(true, facet.IsOptional);

            facet = parmSpec2.GetFacet<IMandatoryFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(true, facet.IsMandatory);
            Assert.AreEqual(false, facet.IsOptional);
        }
    }

    [TestMethod]
    public void ReflectNamed() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(NamedClass) };
                    options.DomainFunctions = new[] { typeof(NamedFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<NamedClass>());

            var facet = spec.GetFacet<INamedFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Class Name", facet.Value);

            var propertySpec = spec.OrderedFields.First();

            facet = propertySpec.GetFacet<INamedFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Property Name", facet.Value);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<INamedFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Function Name", facet.Value);

            var parmSpec = actionSpec.Parameters[1];

            facet = parmSpec.GetFacet<INamedFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Parameter Name", facet.Value);
        }
    }

    [TestMethod]
    public void ReflectRegex() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(RegexClass) };
                    options.DomainFunctions = new[] { typeof(RegexFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<RegexClass>());

            var facet = spec.GetFacet<IRegExFacet>();
            Assert.IsNull(facet);

            var propertySpec = spec.OrderedFields.First();

            facet = propertySpec.GetFacet<IRegExFacet>();
            Assert.IsNull(facet);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IRegExFacet>();
            Assert.IsNull(facet);

            var parmSpec = actionSpec.Parameters[1];

            facet = parmSpec.GetFacet<IRegExFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Parameter Regex", facet.Pattern.ToString());
        }
    }

    [TestMethod]
    public void ReflectPresentationHint() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(HintClass) };
                    options.DomainFunctions = new[] { typeof(HintFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<HintClass>());

            var facet = spec.GetFacet<IPresentationHintFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Class Hint", facet.Value);

            var propertySpec = spec.OrderedFields.First();

            facet = propertySpec.GetFacet<IPresentationHintFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Property Hint", facet.Value);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IPresentationHintFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Function Hint", facet.Value);

            var parmSpec = actionSpec.Parameters[1];

            facet = parmSpec.GetFacet<IPresentationHintFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Parameter Hint", facet.Value);
        }
    }

    [TestMethod]
    public void ReflectPageSize() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(PageSizeFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpec = spec.OrderedContributedActions.First();

            var facet = actionSpec.GetFacet<IPageSizeFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(66, facet.Value);
        }
    }

    [TestMethod]
    public void ReflectPassword() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(PasswordFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpec = spec.OrderedContributedActions.First();

            var parmSpec = actionSpec.Parameters[1];

            var facet = parmSpec.GetFacet<IPasswordFacet>();
            Assert.IsNotNull(facet);
        }
    }

    [TestMethod]
    public void ReflectMultiline() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(MultilineClass) };
                    options.DomainFunctions = new[] { typeof(MultiLineFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<MultilineClass>());

            var propertySpec = spec.OrderedFields.First();

            var facet = propertySpec.GetFacet<IMultiLineFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(1, facet.NumberOfLines);
            Assert.AreEqual(2, facet.Width);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IMultiLineFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(3, facet.NumberOfLines);
            Assert.AreEqual(4, facet.Width);

            var parmSpec = actionSpec.Parameters[1];

            facet = parmSpec.GetFacet<IMultiLineFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(5, facet.NumberOfLines);
            Assert.AreEqual(6, facet.Width);
        }
    }

    [TestMethod]
    public void ReflectOrder() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(OrderClass) };
                    options.DomainFunctions = new[] { typeof(OrderFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<OrderClass>());

            var propertySpec = spec.OrderedFields.First();
            var collectionSpec = spec.OrderedFields[1];

            var facet = propertySpec.GetFacet<IMemberOrderFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Property Order", facet.Name);
            Assert.AreEqual("Property Order", facet.Grouping);
            Assert.AreEqual("0", facet.Sequence);

            facet = collectionSpec.GetFacet<IMemberOrderFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Collection Order", facet.Name);
            Assert.AreEqual("Collection Order", facet.Grouping);
            Assert.AreEqual("1", facet.Sequence);

            var actionSpec = spec.OrderedContributedActions.First();

            facet = actionSpec.GetFacet<IMemberOrderFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual("Function Order", facet.Name);
            Assert.AreEqual("Function Order", facet.Grouping);
            Assert.AreEqual("2", facet.Sequence);
        }
    }

    [TestMethod]
    public void ReflectHidden() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(HiddenClass) };
                    options.DomainFunctions = new Type[] { };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<HiddenClass>());

            var facet = spec.OrderedFields.Single(f => f.Name == "Hidden Property").GetFacet<IHiddenFacet>();
            Assert.IsNotNull(facet);
            Assert.AreEqual(WhenTo.Always, facet.Value);
        }
    }

    [TestMethod]
    public void ReflectHiddenViaFunction() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(HiddenClass) };
                    options.DomainFunctions = new[] { typeof(HideFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<HiddenClass>());
            var facet = spec.OrderedFields.Single(f => f.Name == "Hidden Property Via Function").GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
        }
    }

    [TestMethod]
    public void ReflectVersioned() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(VersionedClass) };
                    options.DomainFunctions = new Type[] { };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<VersionedClass>());

            var facet = spec.OrderedFields.First().GetFacet<IConcurrencyCheckFacet>();
            Assert.IsNotNull(facet);
        }
    }

    [TestMethod]
    public void ReflectViewModelFunctions() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass), typeof(ViewModel) };
                    options.DomainFunctions = new[] { typeof(ViewModelFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<ViewModel>());

            IFacet facet = spec.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);
        }
    }

    [TestMethod]
    public void ReflectPotentFunctions() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(PotentFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpecs = spec.OrderedContributedActions;

            Assert.IsNull(actionSpecs[0].GetFacet<IQueryOnlyFacet>());
            Assert.IsNull(actionSpecs[1].GetFacet<IQueryOnlyFacet>());
            Assert.IsNotNull(actionSpecs[2].GetFacet<IQueryOnlyFacet>());
            Assert.IsNull(actionSpecs[0].GetFacet<IIdempotentFacet>());
            Assert.IsNull(actionSpecs[1].GetFacet<IIdempotentFacet>());
            Assert.IsNull(actionSpecs[2].GetFacet<IIdempotentFacet>());
        }
    }

    [TestMethod]
    public void ReflectContributedCollections() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(ContributedCollectionFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpecs = spec.OrderedCollectionContributedActions;
            var facet = actionSpecs[0].GetFacet<IContributedFunctionFacet>();

            Assert.IsNotNull(facet);
            Assert.IsTrue(facet.IsContributedToCollectionOf(spec));
        }
    }

    [TestMethod]
    public void ReflectEditAnnotation() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(EditClass), typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(EditClassFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<EditClass>());

            var actionSpecs = spec.OrderedContributedActions;
            var actionSpec = actionSpecs[0];
            var facet = actionSpec.GetFacet<IEditPropertiesFacet>();

            Assert.IsNotNull(facet);
            var matched = facet.Properties;

            Assert.AreEqual(3, matched.Length);

            Assert.AreEqual("SimpleProperty", matched[0]);
            Assert.AreEqual("IntProperty", matched[1]);
            Assert.AreEqual("StringProperty", matched[2]);

            var parameterSpecs = actionSpec.Parameters;

            var eFacet = parameterSpecs[0].GetFacet<IActionDefaultsFacet>();
            Assert.IsNull(eFacet);

            eFacet = parameterSpecs[1].GetFacet<IActionDefaultsFacet>();
            Assert.IsNotNull(eFacet);

            eFacet = parameterSpecs[3].GetFacet<IActionDefaultsFacet>();
            Assert.IsNotNull(eFacet);

            eFacet = parameterSpecs[3].GetFacet<IActionDefaultsFacet>();
            Assert.IsNotNull(eFacet);

            eFacet = parameterSpecs[4].GetFacet<IActionDefaultsFacet>();
            Assert.IsNull(eFacet);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ReflectionException), "string")]
    public void ReflectDuplicateFunctionsSameType() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(DuplicateFunctions) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            try {
                container.GetService<IModelBuilder>()?.Build();
            }
            catch (ReflectionException e) {
                Assert.AreEqual("Name clash between user actions defined on NakedFunctions.Reflector.Test.Component.DuplicateFunctions.Function and NakedFunctions.Reflector.Test.Component.DuplicateFunctions.Function: actions on and/or contributed to a menu or object must have unique names.", e.Message);
                throw;
            }
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ReflectionException), "string")]
    public void ReflectDuplicateFunctionsDifferentType() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(DuplicateFunctions1), typeof(DuplicateFunctions2) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            try {
                container.GetService<IModelBuilder>()?.Build();
            }
            catch (ReflectionException e) {
                Assert.AreEqual("Name clash between user actions defined on NakedFunctions.Reflector.Test.Component.DuplicateFunctions1.Function and NakedFunctions.Reflector.Test.Component.DuplicateFunctions2.Function: actions on and/or contributed to a menu or object must have unique names.", e.Message);
                throw;
            }
        }
    }

    [TestMethod]
    public void ReflectParameterChoices() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(ChoicesClass) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpecs = spec.OrderedContributedActions;
            var actionSpec = actionSpecs.SingleOrDefault(s => s.Identifier.MemberName == nameof(ChoicesClass.ActionWithChoices));

            var parameterSpecs = actionSpec.Parameters;

            var p1Spec = parameterSpecs[1];
            var p2Spec = parameterSpecs[2];

            var choices1Facet = p1Spec.GetFacet<IActionChoicesFacet>();
            var choices2Facet = p1Spec.GetFacet<IActionChoicesFacet>();

            Assert.IsNotNull(choices1Facet);
            Assert.IsNotNull(choices2Facet);
        }
    }

    [TestMethod]
    public void ReflectParameterMismatchedChoices() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(ChoicesClass) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpecs = spec.OrderedContributedActions;
            var actionSpec = actionSpecs.SingleOrDefault(s => s.Identifier.MemberName == nameof(ChoicesClass.ActionWithMismatchedChoices));

            var parameterSpecs = actionSpec.Parameters;

            var p1Spec = parameterSpecs[1];
            var p2Spec = parameterSpecs[2];

            var choices1Facet = p1Spec.GetFacet<IActionChoicesFacet>();
            var choices2Facet = p1Spec.GetFacet<IActionChoicesFacet>();

            Assert.IsNull(choices1Facet);
            Assert.IsNull(choices2Facet);
        }
    }

    [TestMethod]
    public void ReflectTargetMismatchedChoices() {
        static void Setup(NakedFrameworkOptions coreOptions) {
            coreOptions.AddNakedFunctions(options => {
                    options.DomainTypes = new[] { typeof(SimpleClass) };
                    options.DomainFunctions = new[] { typeof(MismatchedTargetClass) };
                }
            );
        }

        var (container, host) = GetContainer(Setup);

        using (host) {
            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

            var actionSpecs = spec.OrderedContributedActions;
            var actionSpec = actionSpecs.SingleOrDefault(s => s.Identifier.MemberName == nameof(MismatchedTargetClass.ActionWithChoices));

            var parameterSpecs = actionSpec.Parameters;

            var p1Spec = parameterSpecs[1];
            var p2Spec = parameterSpecs[2];

            var choices1Facet = p1Spec.GetFacet<IActionChoicesFacet>();
            var choices2Facet = p1Spec.GetFacet<IActionChoicesFacet>();

            Assert.IsNull(choices1Facet);
            Assert.IsNull(choices2Facet);
        }
    }
}