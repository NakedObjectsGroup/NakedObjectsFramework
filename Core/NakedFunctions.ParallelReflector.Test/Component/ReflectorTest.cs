// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.ModelBuilding.Component;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.FunctionalFacetFactory;
using NakedObjects.ParallelReflect.Test;
using NakedObjects.ParallelReflect.TypeFacetFactory;

namespace NakedFunctions.Reflect.Test {

    public class NullMenuFactory : IMenuFactory
    {
        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions, string name = null) => null;

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => null;

        #endregion

        public IMenu NewMenu(string name) => null;
    }

    [Bounded]
    public record BoundedClass { }

   
    public record IgnoredClass {
        [NakedFunctionsIgnore]
        public virtual string IgnoredProperty { get; set; }
    }

    public static class ParameterDefaultClass {
        public static SimpleClass DefaultParameterFunction(this SimpleClass target, [NakedFunctions.DefaultValue("a default")] string parameter) => target;
    }

    public record SimpleClass {
        public virtual SimpleClass SimpleProperty { get; set; }
    }

    public class NavigableClass {
        public SimpleClass SimpleProperty { get; set; }
    }

    public static class SimpleFunctions {
        public static SimpleClass SimpleFunction(this SimpleClass target) {
            return target;
        }

        public static IList<SimpleClass> SimpleFunction1(this SimpleClass target)
        {
            return new []{target};
        }
    }

    public static class SimpleInjectedFunctions {
        public static SimpleClass SimpleInjectedFunction(IQueryable<SimpleClass> injected) {
            return injected.First();
        }
    }


    public static class TupleFunctions
    {
        public static (SimpleClass, SimpleClass) TupleFunction(IQueryable<SimpleClass> injected)
        {
            return (injected.First(), injected.First());
        }

        public static (IList<SimpleClass>, IList<SimpleClass>) TupleFunction1(IQueryable<SimpleClass> injected)
        {
            return (injected.ToList(), injected.ToList());
        }
    }

    public static class UnsupportedTupleFunctions
    {
        public static ValueTuple TupleFunction(IQueryable<SimpleClass> injected)
        {
            return new ValueTuple();
        }

    }


    [TestClass]
    public class ReflectorTest {

        private Action<IServiceCollection> TestHook { get; set; } = services => { };

        private IHostBuilder CreateHostBuilder(string[] args, IFunctionalReflectorConfiguration rc, IObjectReflectorConfiguration orc = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    RegisterTypes(services, rc, orc);
                });

        protected IServiceProvider GetContainer(IFunctionalReflectorConfiguration rc, IObjectReflectorConfiguration orc = null)
        {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, rc, orc).Build();
            return hostBuilder.Services;
        }

        private static void RegisterFacetFactory<T>(string name, IServiceCollection services)
        {
            ConfigHelpers.RegisterFacetFactory(typeof(T), services);
        }

        protected virtual void RegisterFacetFactories(IServiceCollection services)
        {
            RegisterFacetFactory<FallbackFacetFactory>("FallbackFacetFactory", services);
            RegisterFacetFactory<IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", services);
            RegisterFacetFactory<SystemClassMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", services);
            RegisterFacetFactory<RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", services);
            RegisterFacetFactory<RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", services);
            RegisterFacetFactory<RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", services);
            RegisterFacetFactory<TypeMarkerFacetFactory>("TypeMarkerFacetFactory", services);
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFacetFactory<MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", services);
            RegisterFacetFactory<PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", services);
            RegisterFacetFactory<ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", services);
            RegisterFacetFactory<ActionMethodsFacetFactory>("ActionMethodsFacetFactory", services);
            RegisterFacetFactory<CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", services);
            RegisterFacetFactory<PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", services);
            RegisterFacetFactory<IconMethodFacetFactory>("IconMethodFacetFactory", services);
            RegisterFacetFactory<CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", services);
            RegisterFacetFactory<TitleMethodFacetFactory>("TitleMethodFacetFactory", services);
            RegisterFacetFactory<ValidateObjectFacetFactory>("ValidateObjectFacetFactory", services);
            RegisterFacetFactory<ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", services);
            RegisterFacetFactory<ViewModelFacetFactory>("ViewModelFacetFactory", services);
            RegisterFacetFactory<BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", services);
            RegisterFacetFactory<EnumFacetFactory>("EnumFacetFactory", services);
            RegisterFacetFactory<ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", services);
            RegisterFacetFactory<PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", services);
            RegisterFacetFactory<DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", services);
            RegisterFacetFactory<DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", services);
            RegisterFacetFactory<PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", services);
            RegisterFacetFactory<ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", services);
            RegisterFacetFactory<PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", services);
            RegisterFacetFactory<PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", services);
            RegisterFacetFactory<HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", services);
            RegisterFacetFactory<HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", services);
            RegisterFacetFactory<DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", services);
            RegisterFacetFactory<AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", services);
            RegisterFacetFactory<ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", services);
            RegisterFacetFactory<ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", services);
            RegisterFacetFactory<MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", services);
            RegisterFacetFactory<RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", services);
            RegisterFacetFactory<MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", services);
            RegisterFacetFactory<MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", services);
            RegisterFacetFactory<NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", services);
            RegisterFacetFactory<NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", services);
            RegisterFacetFactory<ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", services);
            RegisterFacetFactory<OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", services);
            RegisterFacetFactory<RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", services);
            RegisterFacetFactory<PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", services);
            RegisterFacetFactory<DefaultNamingFacetFactory>("DefaultNamingFacetFactory", services); // must come after Named and Plural factories
            RegisterFacetFactory<ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", services);
            RegisterFacetFactory<ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", services);
            RegisterFacetFactory<FinderActionFacetFactory>("FinderActionFacetFactory", services);
            // must come after any facets that install titles
            RegisterFacetFactory<MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", services);
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFacetFactory<RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", services);
            RegisterFacetFactory<TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", services);
            RegisterFacetFactory<TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", services);
            RegisterFacetFactory<TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", services);
            RegisterFacetFactory<TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", services);
            RegisterFacetFactory<EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", services);
            RegisterFacetFactory<PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", services);
            RegisterFacetFactory<BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", services);
            RegisterFacetFactory<ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", services);
            RegisterFacetFactory<SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", services);
            RegisterFacetFactory<ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", services);
            RegisterFacetFactory<IntValueTypeFacetFactory>("IntValueTypeFacetFactory", services);
            RegisterFacetFactory<LongValueTypeFacetFactory>("LongValueTypeFacetFactory", services);
            RegisterFacetFactory<UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", services);
            RegisterFacetFactory<UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", services);
            RegisterFacetFactory<ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", services);
            RegisterFacetFactory<FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", services);
            RegisterFacetFactory<DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", services);
            RegisterFacetFactory<DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", services);
            RegisterFacetFactory<CharValueTypeFacetFactory>("CharValueTypeFacetFactory", services);
            RegisterFacetFactory<DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", services);
            RegisterFacetFactory<TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", services);
            RegisterFacetFactory<StringValueTypeFacetFactory>("StringValueTypeFacetFactory", services);
            RegisterFacetFactory<GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", services);
            RegisterFacetFactory<EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", services);
            RegisterFacetFactory<FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", services);
            RegisterFacetFactory<ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", services);
            RegisterFacetFactory<ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", services);
            RegisterFacetFactory<CollectionFacetFactory>("CollectionFacetFactory", services); // written to not trample over TypeOf if already installed

            RegisterFacetFactory<FunctionsFacetFactory>(nameof(FunctionsFacetFactory), services);
            RegisterFacetFactory<ContributedFunctionFacetFactory>(nameof(ContributedFunctionFacetFactory), services);

        }

        protected virtual void RegisterTypes(IServiceCollection services, IFunctionalReflectorConfiguration frc, IObjectReflectorConfiguration orc = null) {
            RegisterFacetFactories(services);


            services.AddSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
            services.AddSingleton<IClassStrategy, ObjectClassStrategy>();
            services.AddSingleton<IReflector,ObjectReflector>();
            services.AddSingleton<IReflector, FunctionalReflector>();
            services.AddSingleton<IMetamodel, NakedObjects.Meta.Component.Metamodel>();
            services.AddSingleton<IMetamodelBuilder, NakedObjects.Meta.Component.Metamodel>();
            services.AddSingleton<IMenuFactory, NullMenuFactory>();
            services.AddSingleton<IModelBuilder, ModelBuilder>();
            services.AddSingleton<IModelIntegrator, ModelIntegrator>();
            services.AddSingleton(typeof(IFacetFactoryOrder<>), typeof(FacetFactoryOrder<>));

            var dflt = new ObjectReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { "NakedFunctions" });
            dflt.SupportedSystemTypes.Clear();

            var rc = orc ?? dflt;

            services.AddSingleton<IObjectReflectorConfiguration>(rc);


            services.AddSingleton<IFunctionalReflectorConfiguration>(frc);

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

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.IsFalse(specs.Any());
        }

        [TestMethod]
        public void ReflectSimpleType()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[0]);

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.AreEqual(3, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectSimpleFunction()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(SimpleFunctions) });

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.AreEqual(5, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
        }

        [TestMethod]
        public void ReflectTupleFunction() {
            ObjectReflectorConfiguration.NoValidate = true;

            var orc = new ObjectReflectorConfiguration(new Type[]{}, new Type[] { }, new string[] { "NakedFunctions" });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(TupleFunctions) });

            var container = GetContainer(rc, orc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.AreEqual(7, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(TupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs);
        }

        [TestMethod]
        public void ReflectUnsupportedTuple()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(UnsupportedTupleFunctions) }, new Type[0]);

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();

            try {
                builder.Build();
                Assert.Fail("exception expected");
            }
            catch (AggregateException ae) {
                var re = ae.InnerExceptions.FirstOrDefault();
                Assert.IsInstanceOfType(re, typeof(ReflectionException));
                Assert.AreEqual("Cannot reflect empty tuple on NakedFunctions.Reflect.Test.UnsupportedTupleFunctions.TupleFunction", re.Message);
            }
        }

        [TestMethod]
        public void ReflectSimpleInjectedFunction()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var orc = new ObjectReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { "NakedFunctions" });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(SimpleInjectedFunctions) });

            var container = GetContainer(rc, orc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.AreEqual(6, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleInjectedFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);

           // Assert.AreEqual(1, specs[0].ObjectActions.Count);
        }


        [TestMethod]
        public void ReflectNavigableType()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(NavigableClass) }, new Type[0]);

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            Assert.AreEqual(3, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
            //AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectBoundedType() {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(BoundedClass)}, new Type[0]);

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflect.Test.BoundedClass");
            Assert.IsTrue(spec.IsBoundedSet());
        }

        [TestMethod]
        public void ReflectIgnoredProperty()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(IgnoredClass) }, new Type[0]);

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflect.Test.IgnoredClass");
            Assert.AreEqual(0, spec.Fields.Count);
        }


        [TestMethod] 
        public void ReflectDefaultValueParameter()
        {
            ObjectReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new Type[] { typeof(SimpleClass) }, new Type[] { typeof(ParameterDefaultClass) });

            var container = GetContainer(rc);

            var builder = container.GetService<IModelBuilder>();
            builder.Build();
            var specs = AllObjectSpecImmutables(container);
            var spec = specs.OfType<ObjectSpecImmutable>().Single(s => s.FullName == "NakedFunctions.Reflect.Test.SimpleClass");
            var actionSpec = spec.ContributedActions.Single();
            var parmSpec = actionSpec.Parameters[1];
            var facet = parmSpec.GetFacet<IActionDefaultsFacet>();
            Assert.IsNotNull(facet);

            var (defaultValue, type) = facet.GetDefault(null, null, null);
            Assert.AreEqual("a default", defaultValue);
            Assert.AreEqual("Explicit", type.ToString());
        }
    }
}