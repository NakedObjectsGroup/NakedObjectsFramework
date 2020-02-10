// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.DependencyInjection;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.TypeFacetFactory;

namespace NakedObjects.ParallelReflect.Test {
    public class NullMenuFactory : IMenuFactory {
        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions, string name = null) {
            return null;
        }

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
            return null;
        }

        #endregion

        public IMenu NewMenu(string name) {
            return null;
        }
    }

    [TestClass]
    public class ReflectorTest {
        #region TestEnum enum

        public enum TestEnum {
            Value1,
            Value2
        }

        #endregion

        private IHostBuilder CreateHostBuilder(string[] args, IReflectorConfiguration rc) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    RegisterTypes(services, rc);
                });

        protected IServiceProvider GetContainer(IReflectorConfiguration rc) {
            ImmutableSpecFactory.ClearCache();
            var hostBuilder = CreateHostBuilder(new string[] { }, rc).Build();

            return hostBuilder.Services;
        }

        private void RegisterFacetFactory<T>(string name, IServiceCollection services, int order) {
            ConfigHelpers.RegisterFacetFactory(typeof(T), services, order);
        }

        protected virtual void RegisterFacetFactories(IServiceCollection services) {
            int order = 0;
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
        }

        protected virtual void RegisterTypes(IServiceCollection services, IReflectorConfiguration rc) {
            RegisterFacetFactories(services);

            services.AddSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
            services.AddSingleton<IClassStrategy, DefaultClassStrategy>();
            services.AddSingleton<IReflector, ParallelReflector>();
            services.AddSingleton<IMetamodel, Metamodel>();
            services.AddSingleton<IMetamodelBuilder, Metamodel>();
            services.AddSingleton<IMenuFactory, NullMenuFactory>();

            services.AddSingleton<IReflectorConfiguration>(rc);
        }

        [TestMethod]
        public void ReflectNoTypes() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        [TestMethod]
        public void ReflectObjectType() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(object)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(1, reflector.AllObjectSpecImmutables.Count());

            AbstractReflectorTest.AssertSpec(typeof(object), reflector.AllObjectSpecImmutables.First());
        }

        [TestMethod]
        public void ReflectListTypes() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(List<object>), typeof(List<int>), typeof(object), typeof(int)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(int), specs);
            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(List<>), specs);
        }

        [TestMethod]
        public void ReflectSetTypes() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(SetWrapper<>), typeof(object)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(SetWrapper<>), specs);
        }

        [TestMethod]
        public void ReflectQueryableTypes() {
            IQueryable<object> qo = new List<object>().AsQueryable();
            IQueryable<int> qi = new List<int>().AsQueryable();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {qo.GetType(), qi.GetType(), typeof(int), typeof(object)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(int), specs);
            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(EnumerableQuery<>), specs);
        }

        [TestMethod]
        public void ReflectWhereIterator() {
            IEnumerable<int> it = new List<int> {1, 2, 3}.Where(i => i == 2);
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {it.GetType().GetGenericTypeDefinition(), typeof(Object)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs);
        }

        [TestMethod]
        public void ReflectWhereSelectIterator() {
            IEnumerable<int> it = new List<int> {1, 2, 3}.Where(i => i == 2).Select(i => i);
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {it.GetType().GetGenericTypeDefinition(), typeof(Object)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs);
        }

        private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs) {
            return specs.Single(s => s.FullName == type.FullName);
        }

        [TestMethod]
        public void ReflectInt() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(int)}, new Type[] { }, new[] {"System"});
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();

            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(8, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(int), specs);
            AbstractReflectorTest.AssertSpec(typeof(IConvertible), specs);
            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(ValueType), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IFormattable), specs);
        }

        [TestMethod]
        public void ReflectByteArray() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(TestObjectWithByteArray)}, new Type[] { }, new[] {"System"});
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();

            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(31, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(IList), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<long>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(int), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<byte>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IConvertible), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<byte>), specs);
            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<bool>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<bool>), specs);
            AbstractReflectorTest.AssertSpec(typeof(byte[]), specs);
            AbstractReflectorTest.AssertSpec(typeof(Array), specs);
            AbstractReflectorTest.AssertSpec(typeof(ValueType), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<long>), specs);
            AbstractReflectorTest.AssertSpec(typeof(long), specs);
            AbstractReflectorTest.AssertSpec(typeof(IStructuralComparable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable), specs);
            AbstractReflectorTest.AssertSpec(typeof(ICollection), specs);
            AbstractReflectorTest.AssertSpec(typeof(bool), specs);
            AbstractReflectorTest.AssertSpec(typeof(ICloneable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(byte), specs);
            AbstractReflectorTest.AssertSpec(typeof(IFormattable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IReadOnlyList<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IReadOnlyCollection<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IStructuralEquatable), specs);
            AbstractReflectorTest.AssertSpec(typeof(ICollection<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(TestObjectWithByteArray), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable), specs);
        }

        [TestMethod]
        public void ReflectStringArray() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(TestObjectWithStringArray), typeof(string)}, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(TestObjectWithStringArray), specs);
            AbstractReflectorTest.AssertSpec(typeof(string), specs);
        }

        [TestMethod]
        public void ReflectWithScalars() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(WithScalars)}, new Type[] { }, new[] {"System"});
            rc.SupportedSystemTypes.Clear();
            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(74, specs.Length);

            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Decimal>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Int16), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IList), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(UInt32), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<String>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Int64>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Int32>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Decimal), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Int32), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Byte>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IConvertible), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Byte>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Object), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<DateTime>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Single>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Boolean>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Char>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Single>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Boolean>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Byte[]), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(DateTimeKind), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Array), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Char), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(ValueType), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<TimeSpan>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(DayOfWeek), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<UInt16>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Int64>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Int64), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(DateTime), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IStructuralComparable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<DateTime>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(UInt64), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Enum), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(SByte[]), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<SByte>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(WithScalars), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(ICollection), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Boolean), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Double>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Decimal>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<UInt16>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<UInt32>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(ICloneable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Int16>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(TimeSpan), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<String>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IList<>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Byte), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Char>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Char[]), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<UInt32>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(Single), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IFormattable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(ISerializable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Int32>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(SByte), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<SByte>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(String), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IReadOnlyList<>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IReadOnlyCollection<>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IStructuralEquatable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(ICollection<>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<UInt64>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<TimeSpan>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(UInt16), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<UInt64>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEnumerable<>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IComparable<Int16>), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IDeserializationCallback), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEnumerable), specs);
            AbstractReflectorTest.AssertSpecsContain(typeof(IEquatable<Double>), specs);
        }

        [TestMethod]
        public void ReflectSimpleDomainObject() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof(SimpleDomainObject)}, new Type[] { }, new[] {"System"});
            rc.SupportedSystemTypes.Clear();
            var container = GetContainer(rc);

            var reflector = container.GetService<IReflector>();
            reflector.Reflect();

            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(19, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(IComparable<string>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(int), specs);
            AbstractReflectorTest.AssertSpec(typeof(IConvertible), specs);
            AbstractReflectorTest.AssertSpec(typeof(object), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<char>), specs);
            AbstractReflectorTest.AssertSpec(typeof(char), specs);
            AbstractReflectorTest.AssertSpec(typeof(ValueType), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable), specs);
            AbstractReflectorTest.AssertSpec(typeof(ICloneable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<string>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<char>), specs);
            AbstractReflectorTest.AssertSpec(typeof(void), specs);
            AbstractReflectorTest.AssertSpec(typeof(IFormattable), specs);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<int>), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleDomainObject), specs);
            AbstractReflectorTest.AssertSpec(typeof(string), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable<>), specs);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable), specs);
        }

        #region Nested type: SetWrapper

        public class SetWrapper<T> : ISet<T> {
            private readonly ICollection<T> wrapped;

            public SetWrapper(ICollection<T> wrapped) {
                this.wrapped = wrapped;
            }

            #region ISet<T> Members

            public IEnumerator<T> GetEnumerator() {
                return wrapped.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public void UnionWith(IEnumerable<T> other) { }
            public void IntersectWith(IEnumerable<T> other) { }
            public void ExceptWith(IEnumerable<T> other) { }
            public void SymmetricExceptWith(IEnumerable<T> other) { }

            public bool IsSubsetOf(IEnumerable<T> other) {
                return false;
            }

            public bool IsSupersetOf(IEnumerable<T> other) {
                return false;
            }

            public bool IsProperSupersetOf(IEnumerable<T> other) {
                return false;
            }

            public bool IsProperSubsetOf(IEnumerable<T> other) {
                return false;
            }

            public bool Overlaps(IEnumerable<T> other) {
                return false;
            }

            public bool SetEquals(IEnumerable<T> other) {
                return false;
            }

            public bool Add(T item) {
                wrapped.Add(item);
                return true;
            }

            void ICollection<T>.Add(T item) {
                wrapped.Add(item);
            }

            public void Clear() {
                wrapped.Clear();
            }

            public bool Contains(T item) {
                return false;
            }

            public void CopyTo(T[] array, int arrayIndex) { }

            public bool Remove(T item) {
                return false;
            }

            public int Count => wrapped.Count;

            public bool IsReadOnly => wrapped.IsReadOnly;

            #endregion
        }

        #endregion

        #region Nested type: SimpleDomainObject

        public class SimpleDomainObject {
            [Key, Title, ConcurrencyCheck]
            public virtual int Id { get; set; }

            public virtual void Action() { }

            public virtual string HideAction() {
                return null;
            }
        }

        #endregion

        #region Nested type: TestObjectWithByteArray

        public class TestObjectWithByteArray {
            public byte[] ByteArray { get; set; }
        }

        #endregion

        #region Nested type: TestObjectWithStringArray

        public class TestObjectWithStringArray {
            public string[] StringArray { get; set; }
        }

        #endregion

        #region Nested type: WithScalars

        public class WithScalars {
            public WithScalars() {
                Init();
            }

            [Key, Title, ConcurrencyCheck]
            public virtual int Id { get; set; }

            [NotMapped]
            public virtual sbyte SByte { get; set; }

            public virtual byte Byte { get; set; }
            public virtual short Short { get; set; }

            [NotMapped]
            public virtual ushort UShort { get; set; }

            public virtual int Int { get; set; }

            [NotMapped]
            public virtual uint UInt { get; set; }

            public virtual long Long { get; set; }

            [NotMapped]
            public virtual ulong ULong { get; set; }

            public virtual char Char {
                get => '3';
                // ReSharper disable once ValueParameterNotUsed
                set { }
            }

            public virtual bool Bool { get; set; }
            public virtual string String { get; set; }
            public virtual float Float { get; set; }
            public virtual double Double { get; set; }
            public virtual decimal Decimal { get; set; }
            public virtual byte[] ByteArray { get; set; }
            public virtual sbyte[] SByteArray { get; set; }
            public virtual char[] CharArray { get; set; }

            public virtual DateTime DateTime { get; set; } = DateTime.Parse("2012-03-27T09:42:36");

            public virtual ICollection<WithScalars> List { get; set; } = new List<WithScalars>();

            [NotMapped]
            public virtual ICollection<WithScalars> Set { get; set; } = new HashSet<WithScalars>();

            [EnumDataType(typeof(TestEnum))]
            public virtual int EnumByAttributeChoices { get; set; }

            private void Init() {
                SByte = 10;
                UInt = 14;
                ULong = 15;
                UShort = 16;
            }
        }

        #endregion
    }
}