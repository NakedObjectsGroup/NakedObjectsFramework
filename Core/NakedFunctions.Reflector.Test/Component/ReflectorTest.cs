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
using NakedFunctions.Reflector.Extensions;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection.Extensions;
using NakedObjects.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.Test.Reflect;

namespace NakedFunctions.Reflector.Test.Component {
    public class NullMenuFactory : IMenuFactory {
        public IMenu NewMenu(string name) => null;

        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions, string name = null) => null;

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => null;

        public IMenu NewMenu(string name, string id) => null;

        public IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false) => null;

        #endregion
    }

    [Bounded]
    public record BoundedClass {
    }

    public record IgnoredClass {
    [NakedFunctionsIgnore] public virtual string IgnoredProperty { get; set; }
    }

    public static class ParameterDefaultClass {
        public static SimpleClass ParameterWithDefaultFunction(this SimpleClass target, [DefaultValue("a default")] string parameter) => target;
    }

    public record SimpleClass {
    public virtual SimpleClass SimpleProperty { get; set; }
    }

    [ViewModel]
    public record SimpleViewModel
    {
    public virtual SimpleClass SimpleProperty { get; set; }
    }

    public class NavigableClass {
        public SimpleClass SimpleProperty { get; set; }
    }

    public static class SimpleFunctions {
        public static SimpleClass SimpleFunction(this SimpleClass target) => target;

        public static IList<SimpleClass> SimpleFunction1(this SimpleClass target) {
            return new[] {target};
        }
    }

    public static class SimpleInjectedFunctions {
        public static SimpleClass SimpleInjectedFunction(IQueryable<SimpleClass> injected) => injected.First();
    }

    public static class TupleFunctions {
        public static (SimpleClass, SimpleClass) TupleFunction(IQueryable<SimpleClass> injected) => (injected.First(), injected.First());

        public static (IList<SimpleClass>, IList<SimpleClass>) TupleFunction1(IQueryable<SimpleClass> injected) => (injected.ToList(), injected.ToList());
    }

    public static class UnsupportedTupleFunctions {
        public static ValueTuple TupleFunction(IQueryable<SimpleClass> injected) => new ValueTuple();
    }

    public static class LifeCycleFunctions {
        public static SimpleClass Persisting(this SimpleClass target) => target;
        public static SimpleClass Persisted(this SimpleClass target) => target;
        public static SimpleClass Updating(this SimpleClass target) => target;
        public static SimpleClass Updated(this SimpleClass target) => target;
    }

    public static class ViewModelFunctions {
        public static string[] DeriveKeys(this SimpleViewModel target) => null;
        public static SimpleViewModel PopulateUsingKeys(this SimpleViewModel target, string[] keys) => target;
    }

    [TestClass]
    public class ReflectorTest {
        private Action<IServiceCollection> TestHook { get; } = services => { };

        private IHostBuilder CreateHostBuilder(string[] args, Action<NakedCoreOptions> setup) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { RegisterTypes(services, setup); });

        protected IServiceProvider GetContainer(Action<NakedCoreOptions> setup) {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, setup).Build();
            return hostBuilder.Services;
        }

        protected virtual void RegisterTypes(IServiceCollection services, Action<NakedCoreOptions> setup) {
            services.AddNakedFramework(setup);
            TestHook(services);
        }

        public record Test(int a) { }

        private ITypeSpecBuilder[] AllObjectSpecImmutables(IServiceProvider provider) {
            var metaModel = provider.GetService<IMetamodel>();
            return metaModel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();
        }

        private static void EmptyObjectSetup(NakedObjectsOptions options) {
            options.Types = Array.Empty<Type>();
            options.Services = Array.Empty<Type>();
            options.NoValidate = true;
        }

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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.IsFalse(specs.Any());
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(TupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs);
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

            var container = GetContainer(Setup);

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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleInjectedFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.BoundedClass");
            Assert.IsTrue(spec.IsBoundedSet());
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.IgnoredClass");
            Assert.AreEqual(0, spec.Fields.Count);
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.SimpleClass");
            var actionSpec = spec.ContributedActions.Single();
            var parmSpec = actionSpec.Parameters[1];
            var facet = parmSpec.GetFacet<IActionDefaultsFacet>();
            Assert.IsNotNull(facet);

            var (defaultValue, type) = facet.GetDefault(null, null, null);
            Assert.AreEqual("a default", defaultValue);
            Assert.AreEqual("Explicit", type.ToString());
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

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.SimpleClass");

            IFacet facet = spec.GetFacet<IPersistingCallbackFacet>();
            Assert.IsNotNull(facet);
            facet = spec.GetFacet<IPersistedCallbackFacet>();
            Assert.IsNotNull(facet);
            facet = spec.GetFacet<IUpdatingCallbackFacet>();
            Assert.IsNotNull(facet);
            facet = spec.GetFacet<IUpdatedCallbackFacet>();
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void ReflectViewModelFunctions() {
            static void Setup(NakedCoreOptions coreOptions) {
                coreOptions.AddNakedObjects(EmptyObjectSetup);
                coreOptions.AddNakedFunctions(options => {
                        options.FunctionalTypes = new[] {typeof(SimpleClass), typeof(SimpleViewModel)};
                        options.Functions = new[] {typeof(ViewModelFunctions)};
                    }
                );
            }

            var container = GetContainer(Setup);

            container.GetService<IModelBuilder>()?.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.SimpleViewModel");

            IFacet facet = spec.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);
        }
    }
}