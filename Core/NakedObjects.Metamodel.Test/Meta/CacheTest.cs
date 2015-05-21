// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect.Component;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Value;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Meta.Test {
    public class AbstractTestWithByteArray {
        public virtual object Ba { get; set; }
    }

    public class TestWithByteArray : AbstractTestWithByteArray {}

    public class TestService {}

    public class TestSimpleDomainObject {
        private IList<TestSimpleDomainObject> testCollection = new List<TestSimpleDomainObject>();
        public virtual TestSimpleDomainObject TestProperty { get; set; }

        public virtual IList<TestSimpleDomainObject> TestCollection {
            get { return testCollection; }
            set { testCollection = value; }
        }

        public virtual TestSimpleDomainObject TestAction(TestSimpleDomainObject testParm) {
            return this;
        }
    }

    [Named("Test")]
    [IconName("aname")]
    public class TestAnnotatedDomainObject {
        private IList<TestAnnotatedDomainObject> testCollection = new List<TestAnnotatedDomainObject>();

        [Title]
        public virtual TestSimpleDomainObject TestProperty { get; set; }

        public virtual Image TestImage { get; set; }

        public virtual IList<TestAnnotatedDomainObject> TestCollection {
            get { return testCollection; }
            set { testCollection = value; }
        }

        [Disabled]
        [DisplayName("Discount")]
        [MemberOrder(30)]
        [Mask("C")]
        public virtual decimal TestDecimal { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual decimal TestHidden { get; set; }

        [Optionally]
        [TypicalLength(20)]
        [MaxLength(100)]
        [MultiLine]
        [RegEx(Validation = "a", Format = "a", Message = "a")]
        [DataType(DataType.Password)]
        public virtual string TestString { get; set; }

        public virtual int? TestInt { get; set; }
        public virtual DateTime TestDateTime { get; set; }
        public virtual TestEnum TestEnum { get; set; }

        [ConcurrencyCheck]
        public virtual string TestConcurrency { get; set; }

        public virtual string[] ChoicesTestString() {
            return new string[] {};
        }

        public virtual string DefaultTestString() {
            return "";
        }

        public virtual string ValidateTestString(string tovalidate) {
            return "";
        }

        public void Persisted() {}

        [PageSize(20)]
        public IQueryable<TestAnnotatedDomainObject> AutoCompleteTestProperty([MinLength(2)] string name) {
            return null;
        }

        public virtual TestAnnotatedDomainObject TestAction(
            [Named("test"), DefaultValue(null)] TestAnnotatedDomainObject testParm) {
            return this;
        }

        [PageSize(20)]
        public IQueryable<TestAnnotatedDomainObject> AutoComplete0TestAction([MinLength(2)] string name) {
            return null;
        }

        public string DisableTestString(string value) {
            return null;
        }
    }

    public enum TestEnum {
        Value1,
        Value2
    };

    [TestClass]
    public class CacheTest {
        private string testDir;

        [TestInitialize]
        public void Setup() {
            string curDir = Directory.GetCurrentDirectory();
            testDir = Path.Combine(curDir, "testmetadata");
            Directory.CreateDirectory(testDir);
            Directory.GetFiles(testDir).ForEach(File.Delete);
        }

        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<IFacetFactory, SystemClassMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(0));
            container.RegisterType<IMenuFactory, NullMenuFactory>();
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
        }

        public void BinarySerialize(ReflectorConfiguration rc, string file) {
            IUnityContainer container = GetContainer();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var cache = container.Resolve<ISpecificationCache>();

            IEnumerable<string> f1 =
                cache.AllSpecifications().SelectMany(s => s.Fields)
                    .Where(s => s != null)
                    .OfType<IOneToOneAssociationSpecImmutable>()
                    .SelectMany(s => s.GetFacets())
                    .Select(f => f.GetType().FullName)
                    .Distinct();

            // ReSharper disable once UnusedVariable
            string ss = f1.Aggregate("", (s, t) => s + "field facet" + t + "\r\n");
            //Console.WriteLine(ss);

            cache.Serialize(file);

            // and roundtrip 

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new PerResolveLifetimeManager(),
                new InjectionConstructor(file));
            var newCache = container.Resolve<ISpecificationCache>();
            CompareCaches(cache, newCache);
        }

        [TestMethod]
        public void BinarySerializeIntTypes() {
            var ss = new[] {typeof (int)};
            var ns = new[] {typeof (int).Namespace};
            ReflectorConfiguration.NoValidate = true;
            var rc = new ReflectorConfiguration(ss, ss, ns);
            string file = Path.Combine(testDir, "metadataint.bin");
            BinarySerialize(rc, file);
        }

        [TestMethod]
        public void BinarySerializeImageTypes() {
            var ss = new[] {typeof (Image)};
            var ns = new[] {typeof (TestService).Namespace};
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(ss, ss, ns);
            string file = Path.Combine(testDir, "metadataimg.bin");
            BinarySerialize(rc, file);
        }

        [TestMethod]
        public void BinarySerializeBaTypes() {
            var ss = new[] {typeof (AbstractTestWithByteArray)};

            var ns = new[] {typeof (AbstractTestWithByteArray).Namespace};
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (AbstractTestWithByteArray)}, ss, ns);
            string file = Path.Combine(testDir, "metadataba.bin");
            BinarySerialize(rc, file);
        }

        [TestMethod]
        public void BinarySerializeEnumTypes() {
            var ss = new[] {typeof (TestEnum)};
            var ns = new[] {typeof (TestEnum).Namespace};
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(ss, ss, ns);
            string file = Path.Combine(testDir, "metadataenum.bin");
            BinarySerialize(rc, file);
        }

        [TestMethod]
        public void BinarySerializeSimpleDomainObjectTypes() {
            var ns = new[] {typeof (TestSimpleDomainObject).Namespace};
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (TestSimpleDomainObject)}, new[] {typeof (TestService)}, ns);
            string file = Path.Combine(testDir, "metadatatsdo.bin");
            BinarySerialize(rc, file);
        }

        [TestMethod]
        public void BinarySerializeAnnotatedDomainObjectTypes() {
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (TestAnnotatedDomainObject)}, new[] {typeof (TestService)},
                new[] {typeof (TestService).Namespace});

            string file = Path.Combine(testDir, "metadatatado.bin");
            BinarySerialize(rc, file);
        }

        private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
            Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

            // checks for fields and Objects actions 

            string error = newCache.AllSpecifications().Where(s => s.Fields.Any() && s.Fields.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

            Assert.IsTrue(newCache.AllSpecifications().Select(s => s.Fields).All(fs => !fs.Any() || fs.All(f => f != null)), error);

            error = newCache.AllSpecifications().Where(s => s.ObjectActions.Any() && s.ObjectActions.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

            Assert.IsTrue(newCache.AllSpecifications().Select(s => s.ObjectActions).All(fs => !fs.Any() || fs.All(f => f != null)), error);

            var zipped = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new {a, b});

            foreach (var item in zipped) {
                Assert.AreEqual(item.a.FullName, item.b.FullName);

                Assert.AreEqual(item.a.GetFacets().Count(), item.b.GetFacets().Count());

                var zipfacets = item.a.GetFacets().Zip(item.b.GetFacets(), (x, y) => new {x, y});

                foreach (var zipfacet in zipfacets) {
                    Assert.AreEqual(zipfacet.x.FacetType, zipfacet.y.FacetType);
                }
            }
        }

        #region Nested type: NullMenuFactory

        public class NullMenuFactory : IMenuFactory {
            #region IMenuFactory Members

            public IMenu NewMenu(string name) {
                return null;
            }

            public IMenu NewMenu<T>(bool addAllActions, string name = null) {
                return null;
            }
            #endregion


            public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
                return null;
            }
        }

        #endregion

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

            public void UnionWith(IEnumerable<T> other) {}
            public void IntersectWith(IEnumerable<T> other) {}
            public void ExceptWith(IEnumerable<T> other) {}
            public void SymmetricExceptWith(IEnumerable<T> other) {}

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

            public void CopyTo(T[] array, int arrayIndex) {}

            public bool Remove(T item) {
                return false;
            }

            public int Count {
                get { return wrapped.Count; }
            }

            public bool IsReadOnly {
                get { return wrapped.IsReadOnly; }
            }

            #endregion
        }

        #endregion

        #region Nested type: TestObjectWithByteArray

        public class TestObjectWithByteArray {
            public byte[] ByteArray { get; set; }
        }

        #endregion
    }
}