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
using NakedFunctions.Reflector.Extensions;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection.Extensions;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.Test.Reflect;

namespace NakedFunctions.Reflector.Test.Component {
    [TestClass]
    public class ReflectorTest {
        private Action<IServiceCollection> TestHook { get; } = services => { };

        private IHostBuilder CreateHostBuilder(string[] args, Action<NakedCoreOptions> setup) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { RegisterTypes(services, setup); });

        protected (IServiceProvider, IHost) GetContainer(Action<NakedCoreOptions> setup) {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, setup).Build();
            return (hostBuilder.Services, hostBuilder);
        }

        protected virtual void RegisterTypes(IServiceCollection services, Action<NakedCoreOptions> setup) {
            services.AddNakedFramework(setup);
            TestHook(services);
        }

        private ITypeSpecBuilder[] AllObjectSpecImmutables(IServiceProvider provider) {
            var metaModel = provider.GetService<IMetamodel>();
            return metaModel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();
        }

        private static void EmptyObjectSetup(NakedObjectsOptions options) {
            options.Types = Array.Empty<Type>();
            options.Services = Array.Empty<Type>();
            options.NoValidate = true;
        }

        private static string FullName<T>() => $"{typeof(T).FullName}";

        [TestMethod]
        public void ReflectNoTypes() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => Array.Empty<Type>();
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = Array.Empty<Type>();
                        options.Functions = Array.Empty<Type>();
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = Array.Empty<Type>();
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);

                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());
                var propertySpec = spec.Fields.First();
                var facet = propertySpec.GetFacet<IDisabledFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(WhenTo.Always, facet.Value);
            }
        }

        [TestMethod]
        public void ReflectSimpleFunction() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(SimpleFunctions)};
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] {typeof(IQueryable<>), typeof(IList<>)};
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(TupleFunctions)};
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(UnsupportedTupleFunctions)};
                        options.Functions = Array.Empty<Type>();
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] {typeof(IQueryable<>)};
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(SimpleInjectedFunctions)};
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(NavigableClass), typeof(SimpleClass)};
                        options.Functions = Array.Empty<Type>();
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(BoundedClass)};
                        options.Functions = Array.Empty<Type>();
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(CreateNewFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var actionSpec = spec.ContributedActions.First();
                var facet = actionSpec.GetFacet<ICreateNewFacet>();
                Assert.IsNotNull(facet);
            }
        }

        [TestMethod]
        public void ReflectDisplayAsPropertyAction() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(DisplayAsPropertyFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var propertySpec = spec.Fields[0];
                var facet = propertySpec.GetFacet<IDisplayAsPropertyFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(false, propertySpec.ReturnSpec.IsCollection);

                propertySpec = spec.Fields[1];
                facet = propertySpec.GetFacet<IDisplayAsPropertyFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(true, propertySpec.ReturnSpec.IsCollection);
            }
        }

        [TestMethod]
        public void ReflectPluralClass() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(PluralClass)};
                        options.Functions = Array.Empty<Type>();
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(IgnoredClass)};
                        options.Functions = Array.Empty<Type>();
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<IgnoredClass>());
                Assert.AreEqual(0, spec.Fields.Count);
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(ParameterDefaultClass)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                AssertParm(spec.ContributedActions[0], true);
                AssertParm(spec.ContributedActions[1], (byte) 66);
                AssertParm(spec.ContributedActions[2], 'g');
                AssertParm(spec.ContributedActions[3], DateTime.UtcNow.AddDays(35));
                AssertParm(spec.ContributedActions[4], 56.23);
                AssertParm(spec.ContributedActions[5], (float) 22.82);
                AssertParm(spec.ContributedActions[6], 72);
                AssertParm(spec.ContributedActions[7], (long) 91);
                AssertParm(spec.ContributedActions[8], (short) 30);
                AssertParm(spec.ContributedActions[9], "a default");
            }
        }

        [TestMethod]
        public void ReflectRangeParameter() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(RangeClass)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var parmSpec = spec.ContributedActions.First().Parameters[1];
                var facet = parmSpec.GetFacet<IRangeFacet>();
                Assert.IsNotNull(facet);

                Assert.AreEqual(1, facet.Min);
                Assert.AreEqual(56, facet.Max);
            }
        }

        [TestMethod]
        public void ReflectDescribedAs() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(DescribedAsClass)};
                        options.Functions = new[] {typeof(DescribedAsFunctions)};
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

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<IDescribedAsFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Property Description", facet.Value);

                var actionSpec = spec.ContributedActions.First();

                facet = actionSpec.GetFacet<IDescribedAsFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Function Description", facet.Value);
            }
        }

        [TestMethod]
        public void ReflectRenderEagerly() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(RenderEagerlyClass)};
                        options.Functions = new[] {typeof(RenderEagerlyFunctions)};
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

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<IEagerlyFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(Do.Rendering, facet.What);

                var actionSpec = spec.ContributedActions.First();

                facet = actionSpec.GetFacet<IEagerlyFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(Do.Rendering, facet.What);
            }
        }

        [TestMethod]
        public void ReflectTableView() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(TableViewClass)};
                        options.Functions = new[] {typeof(TableViewFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<TableViewClass>());

                var propertySpec = spec.Fields.First();

                var facet = propertySpec.GetFacet<ITableViewFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(true, facet.Title);

                var actionSpec1 = spec.ContributedActions.First();
                var actionSpec2 = spec.ContributedActions[1];

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(MaskClass)};
                        options.Functions = new[] {typeof(MaskFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<MaskClass>());

                var facet = spec.GetFacet<IMaskFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Class Mask", facet.Value);

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<IMaskFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Property Mask", facet.Value);

                var actionSpec = spec.ContributedActions.First();

                facet = actionSpec.GetFacet<IMaskFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Function Mask", facet.Value);

                var parmSpec = actionSpec.Parameters[1];

                facet = parmSpec.GetFacet<IMaskFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Parameter Mask", facet.Value);
            }
        }

        [TestMethod]
        public void ReflectOptionally() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(OptionallyClass)};
                        options.Functions = new[] {typeof(OptionallyFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<OptionallyClass>());

                var propertySpec1 = spec.Fields.First();
                var propertySpec2 = spec.Fields[1];

                var facet = propertySpec1.GetFacet<IMandatoryFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(true, facet.IsMandatory);
                Assert.AreEqual(false, facet.IsOptional);

                facet = propertySpec2.GetFacet<IMandatoryFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(false, facet.IsMandatory);
                Assert.AreEqual(true, facet.IsOptional);

                var actionSpec = spec.ContributedActions.First();

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(NamedClass)};
                        options.Functions = new[] {typeof(NamedFunctions)};
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

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<INamedFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Property Name", facet.Value);

                var actionSpec = spec.ContributedActions.First();

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(RegexClass)};
                        options.Functions = new[] {typeof(RegexFunctions)};
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

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<IRegExFacet>();
                Assert.IsNull(facet);

                var actionSpec = spec.ContributedActions.First();

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(HintClass)};
                        options.Functions = new[] {typeof(HintFunctions)};
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

                var propertySpec = spec.Fields.First();

                facet = propertySpec.GetFacet<IPresentationHintFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Property Hint", facet.Value);

                var actionSpec = spec.ContributedActions.First();

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(PageSizeFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var actionSpec = spec.ContributedActions.First();

                var facet = actionSpec.GetFacet<IPageSizeFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(66, facet.Value);
            }
        }

        [TestMethod]
        public void ReflectPassword() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(PasswordFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var actionSpec = spec.ContributedActions.First();

                var parmSpec = actionSpec.Parameters[1];

                var facet = parmSpec.GetFacet<IPasswordFacet>();
                Assert.IsNotNull(facet);
            }
        }

        [TestMethod]
        public void ReflectMultiline() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(MultilineClass)};
                        options.Functions = new[] {typeof(MultiLineFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<MultilineClass>());

                var propertySpec = spec.Fields.First();

                var facet = propertySpec.GetFacet<IMultiLineFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(1, facet.NumberOfLines);
                Assert.AreEqual(2, facet.Width);

                var actionSpec = spec.ContributedActions.First();

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(OrderClass)};
                        options.Functions = new[] {typeof(OrderFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<OrderClass>());

                var propertySpec = spec.Fields.First();
                var collectionSpec = spec.Fields[1];

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

                var actionSpec = spec.ContributedActions.First();

                facet = actionSpec.GetFacet<IMemberOrderFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual("Function Order", facet.Name);
                Assert.AreEqual("Function Order", facet.Grouping);
                Assert.AreEqual("2", facet.Sequence);
            }
        }

        [TestMethod]
        public void ReflectHidden() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(HiddenClass)};
                        options.Functions = new Type[] { };
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<HiddenClass>());

                var facet = spec.Fields.Single(f => f.Name == "Hidden Property").GetFacet<IHiddenFacet>();
                Assert.IsNotNull(facet);
                Assert.AreEqual(WhenTo.Always, facet.Value);
            }
        }

        [TestMethod]
        public void ReflectHiddenViaFunction() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(HiddenClass)};
                        options.Functions = new[] {typeof(HideFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<HiddenClass>());
                var facet = spec.Fields.Single(f => f.Name == "Hidden Property Via Function").GetFacet<IHideForContextFacet>();
                Assert.IsNotNull(facet);
                //Assert.AreEqual(WhenTo.Always, facet.Value);
            }
        }

        [TestMethod]
        public void ReflectVersioned() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(VersionedClass)};
                        options.Functions = new Type[] { };
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<VersionedClass>());

                var facet = spec.Fields.First().GetFacet<IConcurrencyCheckFacet>();
                Assert.IsNotNull(facet);
            }
        }

        [TestMethod]
        public void ReflectLifeCycleFunctions() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(LifeCycleFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                IFacet facet = spec.GetFacet<IPersistingCallbackFacet>();
                Assert.IsNotNull(facet);
                facet = spec.GetFacet<IPersistedCallbackFacet>();
                Assert.IsNotNull(facet);
                facet = spec.GetFacet<IUpdatingCallbackFacet>();
                Assert.IsNotNull(facet);
                facet = spec.GetFacet<IUpdatedCallbackFacet>();
                Assert.IsNotNull(facet);
            }
        }

        [TestMethod]
        public void ReflectViewModelFunctions() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass), typeof(ViewModel)};
                        options.Functions = new[] {typeof(ViewModelFunctions)};
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(PotentFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var actionSpecs = spec.ContributedActions;

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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(ContributedCollectionFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<SimpleClass>());

                var actionSpecs = spec.CollectionContributedActions;
                var facet = actionSpecs[0].GetFacet<IContributedFunctionFacet>();

                Assert.IsNotNull(facet);
                Assert.IsTrue(facet.IsContributedToCollectionOf(spec));
            }
        }

        [TestMethod]
        public void ReflectEditAnnotation() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(EditClass), typeof(SimpleClass)};
                        options.Functions = new[] {typeof(EditClassFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == FullName<EditClass>());

                var actionSpecs = spec.ContributedActions;
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
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(DuplicateFunctions)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                try {
                    container.GetService<IModelBuilder>()?.Build();
                }
                catch (ReflectionException e) {
                    Assert.AreEqual("Name clash between user actions defined on NakedFunctions.Reflector.Test.Component.DuplicateFunctions.Function and NakedFunctions.Reflector.Test.Component.DuplicateFunctions.Function", e.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ReflectionException), "string")]
        public void ReflectDuplicateFunctionsDifferentType() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass)};
                        options.Functions = new[] {typeof(DuplicateFunctions1), typeof(DuplicateFunctions2)};
                    }
                );
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                try {
                    container.GetService<IModelBuilder>()?.Build();
                }
                catch (ReflectionException e) {
                    Assert.AreEqual("Name clash between user actions defined on NakedFunctions.Reflector.Test.Component.DuplicateFunctions1.Function and NakedFunctions.Reflector.Test.Component.DuplicateFunctions2.Function", e.Message);
                    throw;
                }
            }
        }
    }
}