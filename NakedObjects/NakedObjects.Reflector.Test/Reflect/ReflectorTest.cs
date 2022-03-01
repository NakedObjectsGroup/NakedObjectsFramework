// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.DependencyInjection.Utils;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SpecImmutable;
using NakedObjects;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.FacetFactory;
using TestData;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace TestData {
    public class WithDuplicates {
        [Key]
        [Title]
        public virtual int Id { get; set; }

        public virtual void Duplicate() { }

        public virtual void Duplicate(int parm) { }
    }

    public class WithIgnored {
        [Key]
        [Title]
        public virtual int Id { get; set; }

        public virtual void TestIsIgnored(Dictionary<string, string> ignored) { }

        public virtual void TestIsIgnored1(IDictionary<string, string> ignored) { }
    }

    public class WithDuplicatesService {
        public virtual WithDuplicates Duplicate([ContributedAction] WithDuplicates parm) => parm;
    }
}

namespace NakedObjects.Reflector.Test.Reflect {
    public class NullMenuFactory : IMenuFactory {
        public IMenu NewMenu(string name) => null;

        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions, string name = null) => null;

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => null;

        public IMenu NewMenu(string name, string id) => null;

        public IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false) => null;

        #endregion
    }

    [TestClass]
    public class ReflectorTest {
        #region TestEnum enum

        public enum TestEnum {
            Value1,
            Value2
        }

        #endregion

        private Action<IServiceCollection> TestHook { get; set; } = services => { };

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
        public void ReflectNoTypes() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => Array.Empty<Type>();
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.IsFalse(specs.Any());
            }
        }

        [TestMethod]
        public void ReflectObjectType() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(object) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(1, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(object), specs.First());
            }
        }

        [TestMethod]
        public void ReflectListTypes() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(List<object>), typeof(List<int>), typeof(object), typeof(int) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(3, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(int), specs);
                AbstractReflectorTest.AssertSpec(typeof(object), specs);
                AbstractReflectorTest.AssertSpec(typeof(List<>), specs);
            }
        }

        [TestMethod]
        public void ReflectSetTypes() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(object), typeof(SetWrapper<>) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(2, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(object), specs);
                AbstractReflectorTest.AssertSpec(typeof(SetWrapper<>), specs);
            }
        }

        [TestMethod]
        public void ReflectQueryableTypes() {
            var qo = new List<object>().AsQueryable();
            var qi = new List<int>().AsQueryable();

            void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { qo.GetType(), qi.GetType(), typeof(int), typeof(object) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(3, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(int), specs);
                AbstractReflectorTest.AssertSpec(typeof(object), specs);
                AbstractReflectorTest.AssertSpec(typeof(EnumerableQuery<>), specs);
            }
        }

        [TestMethod]
        public void ReflectWhereIterator() {
            var it = new List<int> { 1, 2, 3 }.Where(i => i == 2);

            void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { it.GetType().GetGenericTypeDefinition(), typeof(object) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(2, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(object), specs);
                AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs);
            }
        }

        [TestMethod]
        public void ReflectWhereSelectIterator() {
            var it = new List<int> { 1, 2, 3 }.Where(i => i == 2).Select(i => i);

            void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { it.GetType().GetGenericTypeDefinition(), typeof(object) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(2, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(object), specs);
                AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs);
            }
        }

        private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs) {
            return specs.Single(s => s.FullName == type.FullName);
        }

        [TestMethod]
        public void ReflectInt() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(int) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = Array.Empty<Type>();
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(1, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(int), specs);
            }
        }

        [TestMethod]
        public void ReflectByteArray() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(byte), typeof(byte[]) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(TestObjectWithByteArray) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(3, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(byte[]), specs);
                AbstractReflectorTest.AssertSpec(typeof(byte), specs);
                AbstractReflectorTest.AssertSpec(typeof(TestObjectWithByteArray), specs);
            }
        }

        [TestMethod]
        public void ReflectStringArray() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(string) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(TestObjectWithStringArray) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(2, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(TestObjectWithStringArray), specs);
                AbstractReflectorTest.AssertSpec(typeof(string), specs);
            }
        }

        [TestMethod]
        public void ReflectWithScalars() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(ICollection<>), typeof(object) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(WithScalars) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(3, specs.Length);
                AbstractReflectorTest.AssertSpecsContain(typeof(WithScalars), specs);
            }
        }

        [TestMethod]
        public void ReflectSimpleDomainObject() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(void) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(SimpleDomainObject) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(2, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(SimpleDomainObject), specs);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ReflectionException), "string")]
        public void ReflectDuplicateMethods() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(WithDuplicates) };
                    options.DomainModelServices = new[] { typeof(WithDuplicatesService) };
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                try {
                    container.GetService<IModelBuilder>()?.Build();
                }
                catch (ReflectionException e) {
                    Assert.AreEqual("Name clash between user actions defined on TestData.WithDuplicates.Duplicate and TestData.WithDuplicates.Duplicate and TestData.WithDuplicatesService.Duplicate: actions on and/or contributed to a menu or object must have unique names.", e.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        //[ExpectedException(typeof(ReflectionException), "string")]
        public void ReflectIgnoredMethods() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(WithIgnored) };
                    options.DomainModelServices = new[] { typeof(WithDuplicatesService) };
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                //Assert.AreEqual(1, specs.Length);
                AbstractReflectorTest.AssertSpec(typeof(WithIgnored), specs);
            }
        }

        [TestMethod]
        public void ReplaceFacetFactory() {
            TestHook = services => services.RegisterReplacementFacetFactory<IDomainObjectFacetFactoryProcessor, ReplacementBoundedAnnotationFacetFactory, BoundedAnnotationFacetFactory>();

            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => Array.Empty<Type>();
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(SimpleBoundedObject) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(1, specs.Length);
                var spec = specs.First();
                Assert.IsFalse(spec.ContainsFacet<IBoundedFacet>());
            }
        }

        [TestMethod]
        public void ReplaceDelegatingFacetFactory() {
            TestHook = services => ConfigHelpers.RegisterReplacementFacetFactoryDelegatingToOriginal<IDomainObjectFacetFactoryProcessor, ReplacementDelegatingBoundedAnnotationFacetFactory, BoundedAnnotationFacetFactory>(services);

            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => Array.Empty<Type>();
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(SimpleBoundedObject) };
                    options.DomainModelServices = Array.Empty<Type>();
                    options.NoValidate = true;
                });
            }

            var (container, host) = GetContainer(Setup);

            using (host) {
                container.GetService<IModelBuilder>()?.Build();
                var specs = AllObjectSpecImmutables(container);
                Assert.AreEqual(1, specs.Length);
                var spec = specs.First();
                Assert.IsFalse(spec.ContainsFacet<IBoundedFacet>());
            }
        }

        [TestMethod]
        public void ReflectDisplayAsProperty() {
            static void Setup(NakedFrameworkOptions coreOptions) {
                coreOptions.SupportedSystemTypes = t => new[] { typeof(void) };
                coreOptions.AddNakedObjects(options => {
                    options.DomainModelTypes = new[] { typeof(DisplayAsPropertyObject) };
                    options.DomainModelServices = new[] { typeof(DisplayAsPropertyService) };
                    options.NoValidate = true;
                });
            }

            void Build(int run) {
                var (container, host) = GetContainer(Setup);

                using (host) {
                    container.GetService<IModelBuilder>()?.Build();
                    var specs = AllObjectSpecImmutables(container);

                    var testSpec = AllObjectSpecImmutables(container).Single(s => s.ShortName == nameof(DisplayAsPropertyObject)) as IObjectSpecImmutable;
                    var fields = testSpec.OrderedFields;
                    var fCount = fields.Count;

                    var actions = testSpec.OrderedObjectActions;
                    var aCount = actions.Count;

                    var testService = AllObjectSpecImmutables(container).Single(s => s.ShortName == nameof(DisplayAsPropertyService)) as ITypeSpecImmutable;

                    var actions1 = testService.OrderedObjectActions;
                    var cCount = actions1.Count;

                    var iCount = ((IntegrationFacet)testSpec.GetFacet<IIntegrationFacet>()).ActionCount;

                    Assert.AreEqual((2, 0, 0, 2), (fCount, aCount, cCount, iCount), $"Failed on run: {run}");
                }
            }

            AbstractIntegrationFacet.AllowRemove = false;

            try {
                // repeat to flush out race conditions 
                for (var i = 0; i < 100; i++) {
                    Build(i);
                }
            }
            finally {
                AbstractIntegrationFacet.AllowRemove = true;
            }
        }

        #region Nested type: ReplacementBoundedAnnotationFacetFactory

        public sealed class ReplacementBoundedAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
            public ReplacementBoundedAnnotationFacetFactory(IFacetFactoryOrder<BoundedAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
                : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

            public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;
        }

        #endregion

        #region Nested type: ReplacementDelegatingBoundedAnnotationFacetFactory

        public sealed class ReplacementDelegatingBoundedAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
            private readonly BoundedAnnotationFacetFactory originalFactory;

            public ReplacementDelegatingBoundedAnnotationFacetFactory(IFacetFactoryOrder<BoundedAnnotationFacetFactory> order, BoundedAnnotationFacetFactory originalFactory, ILoggerFactory loggerFactory)
                : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
                this.originalFactory = originalFactory;

            public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
                Assert.IsNotNull(originalFactory);
                return metamodel;
            }
        }

        #endregion

        #region Nested type: SetWrapper

        public class SetWrapper<T> : ISet<T> {
            private readonly ICollection<T> wrapped;

            public SetWrapper(ICollection<T> wrapped) => this.wrapped = wrapped;

            #region ISet<T> Members

            public IEnumerator<T> GetEnumerator() => wrapped.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void UnionWith(IEnumerable<T> other) { }
            public void IntersectWith(IEnumerable<T> other) { }
            public void ExceptWith(IEnumerable<T> other) { }
            public void SymmetricExceptWith(IEnumerable<T> other) { }

            public bool IsSubsetOf(IEnumerable<T> other) => false;

            public bool IsSupersetOf(IEnumerable<T> other) => false;

            public bool IsProperSupersetOf(IEnumerable<T> other) => false;

            public bool IsProperSubsetOf(IEnumerable<T> other) => false;

            public bool Overlaps(IEnumerable<T> other) => false;

            public bool SetEquals(IEnumerable<T> other) => false;

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

            public bool Contains(T item) => false;

            public void CopyTo(T[] array, int arrayIndex) { }

            public bool Remove(T item) => false;

            public int Count => wrapped.Count;

            public bool IsReadOnly => wrapped.IsReadOnly;

            #endregion
        }

        #endregion

        #region Nested type: SimpleBoundedObject

        [Bounded]
        public class SimpleBoundedObject {
            [Key] [Title] [ConcurrencyCheck] public virtual int Id { get; set; }
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

            [Key] [Title] [ConcurrencyCheck] public virtual int Id { get; set; }

            [NotMapped] public virtual sbyte SByte { get; set; }

            public virtual byte Byte { get; set; }
            public virtual short Short { get; set; }

            [NotMapped] public virtual ushort UShort { get; set; }

            public virtual int Int { get; set; }

            [NotMapped] public virtual uint UInt { get; set; }

            public virtual long Long { get; set; }

            [NotMapped] public virtual ulong ULong { get; set; }

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

            [NotMapped] public virtual ICollection<WithScalars> Set { get; set; } = new HashSet<WithScalars>();

            [EnumDataType(typeof(TestEnum))] public virtual int EnumByAttributeChoices { get; set; }

            private void Init() {
                SByte = 10;
                UInt = 14;
                ULong = 15;
                UShort = 16;
            }
        }

        #endregion

        #region Nested type: SimpleDomainObject

        public class SimpleDomainObject {
            [Key] [Title] [ConcurrencyCheck] public virtual int Id { get; set; }

            public virtual void Action() { }

            public virtual string HideAction() => null;
        }

        public class DisplayAsPropertyObject {
            [Key] [Title] [ConcurrencyCheck] public virtual int Id { get; set; }

            [DisplayAsProperty]
            public virtual DisplayAsPropertyObject ADisplayAsPropertyAction() => null;
        }

        public class DisplayAsPropertyService {
            [DisplayAsProperty]
            public virtual DisplayAsPropertyObject ADisplayAsPropertyContributedAction([ContributedAction] DisplayAsPropertyObject obj) => null;
        }

        #endregion
    }
}