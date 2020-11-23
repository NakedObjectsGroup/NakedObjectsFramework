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
using NakedFramework.ModelBuilding.Component;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Reflect;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection.DependencyInjection;
using NakedObjects.DependencyInjection.FacetFactory;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Reflect;
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

        private IHostBuilder CreateHostBuilder(string[] args, ICoreConfiguration cc, IFunctionalReflectorConfiguration rc, IObjectReflectorConfiguration orc = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { RegisterTypes(services, cc, rc, orc); });

        protected IServiceProvider GetContainer(ICoreConfiguration cc, IFunctionalReflectorConfiguration rc, IObjectReflectorConfiguration orc = null) {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, cc, rc, orc).Build();
            return hostBuilder.Services;
        }

        private static void RegisterFacetFactory<T>(string name, IServiceCollection services) {
            ConfigHelpers.RegisterFacetFactory(typeof(T), services);
        }

        private static void RegisterFacetFactory(Type facetFactorType, IServiceCollection services) {
            ConfigHelpers.RegisterFacetFactory(facetFactorType, services);
        }

        protected virtual void RegisterFacetFactories(IServiceCollection services) {
            foreach (var facetFactory in ObjectFacetFactories.StandardFacetFactories()) {
                RegisterFacetFactory(facetFactory, services);
            }

            foreach (var facetFactory in FunctionalFacetFactories.StandardFacetFactories()) {
                RegisterFacetFactory(facetFactory, services);
            }
        }

        protected virtual void RegisterTypes(IServiceCollection services, ICoreConfiguration cc, IFunctionalReflectorConfiguration frc, IObjectReflectorConfiguration orc = null) {
            RegisterFacetFactories(services);


            services.AddSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
            services.AddSingleton<IClassStrategy, ObjectClassStrategy>();
            services.AddSingleton<IReflector, ObjectReflector>();
            services.AddSingleton<IReflector, FunctionalReflector>();
            services.AddSingleton<IMetamodel, Metamodel>();
            services.AddSingleton<IMetamodelBuilder, Metamodel>();
            services.AddSingleton<IMenuFactory, NullMenuFactory>();
            services.AddSingleton<IModelBuilder, ModelBuilder>();
            services.AddSingleton<IModelIntegrator, ModelIntegrator>();
            services.AddSingleton(typeof(IFacetFactoryOrder<>), typeof(FacetFactoryOrder<>));

            var dflt = new ObjectReflectorConfiguration(new Type[] { }, new Type[] { });
            dflt.SupportedSystemTypes.Clear();

            var rc = orc ?? dflt;

            services.AddSingleton(cc);
            services.AddSingleton(rc);
            services.AddSingleton(frc);

            TestHook(services);
        }

        public record Test(int a) { }

        private ITypeSpecBuilder[] AllObjectSpecImmutables(IServiceProvider provider) {
            var metaModel = provider.GetService<IMetamodel>();
            return metaModel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();
        }


        [TestMethod]
        public void ReflectNoTypes() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new Type[0], new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.IsFalse(specs.Any());
        }

        [TestMethod]
        public void ReflectSimpleType() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectSimpleFunction() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new[] {typeof(SimpleFunctions)});

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
        }

        [TestMethod]
        public void ReflectTupleFunction() {
            ObjectReflectorConfiguration.NoValidate = true;

            var orc = new ObjectReflectorConfiguration(new Type[] { }, new Type[] { });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new[] {typeof(TupleFunctions)});

            var container = GetContainer(new CoreConfiguration(), rc, orc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(TupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs);
        }

        [TestMethod]
        public void ReflectUnsupportedTuple() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(UnsupportedTupleFunctions)}, new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();

            try {
                builder.Build();
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
            ObjectReflectorConfiguration.NoValidate = true;

            var orc = new ObjectReflectorConfiguration(new Type[] { }, new Type[] { });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new[] {typeof(SimpleInjectedFunctions)});

            var container = GetContainer(new CoreConfiguration(), rc, orc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            //AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleInjectedFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);

            // Assert.AreEqual(1, specs[0].ObjectActions.Count);
        }


        [TestMethod]
        public void ReflectNavigableType() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(NavigableClass)}, new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
            //AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectBoundedType() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(BoundedClass)}, new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.BoundedClass");
            Assert.IsTrue(spec.IsBoundedSet());
        }

        [TestMethod]
        public void ReflectIgnoredProperty() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(IgnoredClass)}, new Type[0]);

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.IgnoredClass");
            Assert.AreEqual(0, spec.Fields.Count);
        }

        [TestMethod]
        public void ReflectDefaultValueParameter() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new[] {typeof(ParameterDefaultClass)});

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
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
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new[] {typeof(LifeCycleFunctions)});

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
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
        public void ReflectViewModelFunctions()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass), typeof(SimpleViewModel) }, new[] { typeof(ViewModelFunctions) });

            var container = GetContainer(new CoreConfiguration(), rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflector.Test.Component.SimpleViewModel");

            IFacet facet = spec.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);
        }

    }
}