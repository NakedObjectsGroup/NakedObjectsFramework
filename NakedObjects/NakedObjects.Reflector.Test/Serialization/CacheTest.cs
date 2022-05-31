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
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Value;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Reflector.Facet;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedObjects.Reflector.Test.Serialization;

public class AbstractTestWithByteArray {
    public virtual object Ba { get; set; }
}

public class TestWithByteArray : AbstractTestWithByteArray { }

public class TestService { }

public class TestSimpleDomainObject {
    public virtual TestSimpleDomainObject TestProperty { get; set; }

    public virtual IList<TestSimpleDomainObject> TestCollection { get; set; } = new List<TestSimpleDomainObject>();

    public virtual TestSimpleDomainObject TestAction(TestSimpleDomainObject testParm) => this;
}

public class TestSimpleDomainObjectWithMenu {
    public virtual TestSimpleDomainObject TestProperty { get; set; }

    public virtual IList<TestSimpleDomainObject> TestCollection { get; set; } = new List<TestSimpleDomainObject>();

    public virtual TestSimpleDomainObjectWithMenu TestAction(TestSimpleDomainObjectWithMenu testParm) => this;

    public static void Menu(IMenu menu) {
        menu.AddAction("TestAction");
    }
}

[Named("Test")]
public class TestAnnotatedDomainObject {
    [Title]
    public virtual TestSimpleDomainObject TestProperty { get; set; }

    public virtual Image TestImage { get; set; }

    public virtual IList<TestAnnotatedDomainObject> TestCollection { get; set; } = new List<TestAnnotatedDomainObject>();

    [Disabled]
    [DisplayName("Discount")]
    [MemberOrder(30)]
    [Mask("C")]
    public virtual decimal TestDecimal { get; set; }

    [Hidden(WhenTo.Always)]
    public virtual decimal TestHidden { get; set; }

    [Optionally]
    [MaxLength(100)]
    [MultiLine]
    [RegEx(Validation = "a", Format = "a", Message = "a")]
    [DataType(DataType.Password)]
    public virtual string TestString { get; set; }

    public virtual int? TestInt { get; set; }
    public virtual DateTime TestDateTime { get; set; }
    public virtual NakedFramework.Metamodel.Test.SemanticsProvider.TestEnum TestEnum { get; set; }

    [ConcurrencyCheck]
    public virtual string TestConcurrency { get; set; }

    public virtual string[] ChoicesTestString() {
        return new string[] { };
    }

    public virtual string DefaultTestString() => "";

    public virtual string ValidateTestString(string tovalidate) => "";

    public void Persisted() { }

    [PageSize(20)]
    public IQueryable<TestAnnotatedDomainObject> AutoCompleteTestProperty([MinLength(2)] string name) => null;

    public virtual TestAnnotatedDomainObject TestAction(
        [Named("test")] [DefaultValue(null)] TestAnnotatedDomainObject testParm) =>
        this;

    [PageSize(20)]
    public IQueryable<TestAnnotatedDomainObject> AutoComplete0TestAction([MinLength(2)] string name) => null;

    public string DisableTestString(string value) => null;
}

public enum TestEnum {
    Value1,
    Value2
}

[TestClass]
public class CacheTest {
    private string testDir;

    private Action<IServiceCollection> TestHook { get; } = services => { };

    [TestInitialize]
    public void Initialize() {
        var curDir = Directory.GetCurrentDirectory();
        testDir = Path.Combine(curDir, "testmetadata");
        Directory.CreateDirectory(testDir);
        Directory.GetFiles(testDir).ForEach(File.Delete);
    }

    protected (IServiceProvider, IHost) GetContainer(Action<NakedFrameworkOptions> setup) {
        ImmutableSpecFactory.ClearCache();
        var hostBuilder = CreateHostBuilder(Array.Empty<string>(), setup).Build();
        return (hostBuilder.Services, hostBuilder);
    }

    private IHostBuilder CreateHostBuilder(string[] args, Action<NakedFrameworkOptions> setup) {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureServices((hostContext, services) => { RegisterTypes(services, setup); });
    }

    protected virtual void RegisterTypes(IServiceCollection services, Action<NakedFrameworkOptions> setup) {
        services.AddNakedFramework(setup);
        TestHook(services);
    }

    private Type[] AdditionalKnownTypes()
    {
        var a = Assembly.GetAssembly(typeof(LoadingCallbackFacetNull));
        var tt = a.GetTypes().Where(t => t is { IsSerializable: true, IsPublic: true }).ToArray();

        tt = tt.Append(typeof(EnumValueSemanticsProvider<TestEnum>)).ToArray();
        tt = tt.Append(typeof(ParseableFacetUsingParser<TestEnum>)).ToArray();
        tt = tt.Append(typeof(ValueFacetFromSemanticProvider<TestEnum>)).ToArray();
        tt = tt.Append(typeof(DefaultedFacetUsingDefaultsProvider<TestEnum>)).ToArray();
        tt = tt.Append(typeof(TitleFacetUsingParser<TestEnum>)).ToArray();

        return tt;
    }





    public void SerializeIntTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => new[] { typeof(int) };
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = Array.Empty<Type>();
                options.DomainModelServices = Array.Empty<Type>();
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeIntTypes()
    {
        SerializeIntTypes("metadataint.bin");
    }

    [TestMethod]
    public void XmlSerializeIntTypes()
    {
        SerializeIntTypes("metadataint.xml");
    }

    public void SerializeImageTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => new[] { typeof(Image) };
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = Array.Empty<Type>();
                options.DomainModelServices = Array.Empty<Type>();
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeImageTypes()
    {
        SerializeImageTypes("metadataimg.bin");
    }

    [TestMethod]
    public void XmlSerializeImageTypes()
    {
        SerializeImageTypes("metadataimg.xml");
    }

    public void SerializeBaTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => t;
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = new[] { typeof(AbstractTestWithByteArray) };
                options.DomainModelServices = Array.Empty<Type>();
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeBaTypes()
    {
        SerializeBaTypes("metadataba.bin");
    }

    [TestMethod]
    public void XmlSerializeBaTypes()
    {
        SerializeBaTypes("metadataba.xml");
    }

    public void SerializeEnumTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => t;
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = new[] { typeof(TestEnum) };
                options.DomainModelServices = Array.Empty<Type>();
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeEnumTypes()
    {
        SerializeEnumTypes("metadataenum.bin");
    }

    [TestMethod]
    public void XmlSerializeEnumTypes()
    {
        SerializeEnumTypes("metadataenum.xml");
    }

    public void SerializeSimpleDomainObjectTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => t;
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = new[] { typeof(TestSimpleDomainObject) };
                options.DomainModelServices = new[] { typeof(TestService) };
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeSimpleDomainObjectTypes()
    {
        SerializeSimpleDomainObjectTypes("metadatatsdo.bin");
    }

    [TestMethod]
    public void XmlSerializeSimpleDomainObjectTypes()
    {
        SerializeSimpleDomainObjectTypes("metadatatsdo.xml");
    }

    public void SerializeAnnotatedDomainObjectTypes(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => t;
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = new[] { typeof(TestAnnotatedDomainObject) };
                options.DomainModelServices = new[] { typeof(TestService) };
                options.NoValidate = true;
            });
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeAnnotatedDomainObjectTypes()
    {
        SerializeAnnotatedDomainObjectTypes("metadatatado.bin");
    }

    [TestMethod]
    public void XmlSerializeAnnotatedDomainObjectTypes()
    {
        SerializeAnnotatedDomainObjectTypes("metadatatado.xml");
    }

    public void SerializeMenus(string fileName)
    {
        RecurseCheck = new HashSet<ISpecification>();
        var file = Path.Combine(testDir, fileName);

        static void Setup(NakedFrameworkOptions coreOptions)
        {
            coreOptions.SupportedSystemTypes = t => t;
            coreOptions.AddNakedObjects(options => {
                options.DomainModelTypes = new[] { typeof(TestSimpleDomainObjectWithMenu) };
                options.DomainModelServices = new[] { typeof(TestService) };
                options.NoValidate = true;
            });
            coreOptions.MainMenus = f => new[] { f.NewMenu(typeof(TestService)) };
        }

        var (container, host) = GetContainer(Setup);

        using (host)
        {
            CompareCaches(container, file, AdditionalKnownTypes());
        }
    }

    [TestMethod]
    public void BinarySerializeMenus()
    {
        SerializeMenus("metadatamenu.bin");
    }

    [TestMethod]
    public void XmlSerializeMenus()
    {
        SerializeMenus("metadatamenu.xml");
    }

    private static void CompareCaches(IServiceProvider container, string file, Type[] additionalKnownTypes = null) {
        var metamodelBuilder = container.GetService<IMetamodelBuilder>();
        var modelBuilder = container.GetService<IModelBuilder>();
        modelBuilder?.Build(file, additionalKnownTypes ?? Array.Empty<Type>());
        var cache1 = metamodelBuilder?.Cache;
        modelBuilder?.RestoreFromFile(file, additionalKnownTypes ?? Array.Empty<Type>());
        var cache2 = metamodelBuilder?.Cache;

        Assert.IsNotNull(cache1);
        Assert.IsNotNull(cache2);
        Assert.AreNotEqual(cache1, cache2);
        CompareCaches(cache1, cache2);
    }

    private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
        Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

        // checks for fields and Objects actions 

        var error = newCache.AllSpecifications().Where(s => s.OrderedFields.Any() && s.OrderedFields.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

        Assert.IsTrue(newCache.AllSpecifications().Select(s => s.OrderedFields).All(fs => !fs.Any() || fs.All(f => f != null)), error);

        error = newCache.AllSpecifications().Where(s => s.OrderedObjectActions.Any() && s.OrderedObjectActions.Any(f => f == null)).Select(s => s.FullName).Aggregate("", (s, t) => s + " " + t);

        Assert.IsTrue(newCache.AllSpecifications().Select(s => s.OrderedObjectActions).All(fs => !fs.Any() || fs.All(f => f != null)), error);

        var zippedSpecs = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new { a, b });

        foreach (var item in zippedSpecs) {
            AssertSpecification(item.a, item.b);
        }

        var zippedMenus = cache.MainMenus().Zip(newCache.MainMenus(), (a, b) => new { a, b });

        foreach (var item in zippedMenus) {
            AssertMenu(item.a, item.b);
        }
    }

    #region Nested type: NullMenuFactory

    public class NullMenuFactory : IMenuFactory {
        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => null;
        public IMenu NewMenu(string name, string id) => throw new NotImplementedException();

        public IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false) => throw new NotImplementedException();

        #region IMenuFactory Members

        public IMenu NewMenu(string name) => null;

        public IMenu NewMenu<T>(bool addAllActions, string name = null) => null;

        #endregion
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

    #region Nested type: TestObjectWithByteArray

    public class TestObjectWithByteArray {
        public byte[] ByteArray { get; set; }
    }

    #endregion
}