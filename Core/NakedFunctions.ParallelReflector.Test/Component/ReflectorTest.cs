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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core;
using NakedObjects.Core.Configuration;
using NakedObjects.DependencyInjection;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
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

        private IHostBuilder CreateHostBuilder(string[] args, IFunctionalReflectorConfiguration rc, IReflectorConfiguration orc = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    RegisterTypes(services, rc, orc);
                });

        protected IServiceProvider GetContainer(IFunctionalReflectorConfiguration rc, IReflectorConfiguration orc = null)
        {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, rc, orc).Build();
            return hostBuilder.Services;
        }

        private static void RegisterFacetFactory<T>(string name, IServiceCollection services, int order)
        {
            ConfigHelpers.RegisterFacetFactory(typeof(T), services, order);
        }

        protected virtual void RegisterFacetFactories(IServiceCollection services)
        {
            var order = 0;
            RegisterFacetFactory<FallbackFacetFactory>("FallbackFacetFactory", services, order++);
            RegisterFacetFactory<IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", services, order++);
            RegisterFacetFactory<SystemClassMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", services, order++);
            RegisterFacetFactory<RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", services, order++);
            RegisterFacetFactory<RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", services, order++);
            RegisterFacetFactory<RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", services, order++);
            RegisterFacetFactory<TypeMarkerFacetFactory>("TypeMarkerFacetFactory", services, order++);
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFacetFactory<MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", services, order++);
            RegisterFacetFactory<PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", services, order++);
            RegisterFacetFactory<ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", services, order++);
            RegisterFacetFactory<ActionMethodsFacetFactory>("ActionMethodsFacetFactory", services, order++);
            RegisterFacetFactory<CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", services, order++);
            RegisterFacetFactory<PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", services, order++);
            RegisterFacetFactory<IconMethodFacetFactory>("IconMethodFacetFactory", services, order++);
            RegisterFacetFactory<CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", services, order++);
            RegisterFacetFactory<TitleMethodFacetFactory>("TitleMethodFacetFactory", services, order++);
            RegisterFacetFactory<ValidateObjectFacetFactory>("ValidateObjectFacetFactory", services, order++);
            RegisterFacetFactory<ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ViewModelFacetFactory>("ViewModelFacetFactory", services, order++);
            RegisterFacetFactory<BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<EnumFacetFactory>("EnumFacetFactory", services, order++);
            RegisterFacetFactory<ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", services, order++);
            RegisterFacetFactory<DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", services, order++);
            RegisterFacetFactory<AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<DefaultNamingFacetFactory>("DefaultNamingFacetFactory", services, order++); // must come after Named and Plural factories
            RegisterFacetFactory<ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<FinderActionFacetFactory>("FinderActionFacetFactory", services, order++);
            // must come after any facets that install titles
            RegisterFacetFactory<MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", services, order++);
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFacetFactory<RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", services, order++);
            RegisterFacetFactory<TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", services, order++);
            RegisterFacetFactory<BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<IntValueTypeFacetFactory>("IntValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<LongValueTypeFacetFactory>("LongValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<CharValueTypeFacetFactory>("CharValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<StringValueTypeFacetFactory>("StringValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", services, order++);
            RegisterFacetFactory<ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", services, order++);
            RegisterFacetFactory<CollectionFacetFactory>("CollectionFacetFactory", services, order); // written to not trample over TypeOf if already installed

            RegisterFacetFactory<FunctionsFacetFactory>("FunctionsFacetFactory", services, order++);

        }

        protected virtual void RegisterTypes(IServiceCollection services, IFunctionalReflectorConfiguration frc, IReflectorConfiguration orc = null) {
            RegisterFacetFactories(services);


            services.AddSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
            services.AddSingleton<IClassStrategy, DefaultClassStrategy>();
            services.AddSingleton<IReflector, NakedObjects.ParallelReflect.Component.ParallelReflector>();
            services.AddSingleton<IMetamodel, NakedObjects.Meta.Component.Metamodel>();
            services.AddSingleton<IMetamodelBuilder, NakedObjects.Meta.Component.Metamodel>();
            services.AddSingleton<IMenuFactory, NullMenuFactory>();

            var dflt = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { "NakedFunctions" });
            dflt.SupportedSystemTypes.Clear();

            var rc = orc ?? dflt;

            services.AddSingleton<IReflectorConfiguration>(rc);


            services.AddSingleton<IFunctionalReflectorConfiguration>(frc);

            TestHook(services);
        }

        public record Test(int a) { }


        [TestMethod]
        public void ReflectNoTypes() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new Type[0], new Type[0]);

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        [TestMethod]
        public void ReflectSimpleType()
        {
            ReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[0]);

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(4, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectSimpleFunction()
        {
            ReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(SimpleFunctions) });

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(6, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(MenuFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
        }

        [TestMethod]
        public void ReflectTupleFunction() {
            ReflectorConfiguration.NoValidate = true;

            var orc = new ReflectorConfiguration(new Type[]{}, new Type[] { }, new string[] { "NakedFunctions" });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(TupleFunctions) });

            var container = GetContainer(rc, orc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
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
            ReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(UnsupportedTupleFunctions) }, new Type[0]);

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();

            try {
                reflector.Reflect();
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
            ReflectorConfiguration.NoValidate = true;

            var orc = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { "NakedFunctions" });
            orc.SupportedSystemTypes.Clear();
            orc.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(SimpleInjectedFunctions) });

            var container = GetContainer(rc, orc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
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
            ReflectorConfiguration.NoValidate = true;

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(NavigableClass) }, new Type[0]);

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(5, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
            //AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

    }
}