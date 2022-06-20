﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.Linq;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect.Component;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;

namespace NakedObjects.Reflect.Test {
    public class NullMenuFactory : IMenuFactory {

        public IMenu NewMenu(string name) {
            return null;
        }

        public IMenu NewMenu<T>(bool addAllActions, string name = null) {
            return null;
        }
        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
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

        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterFacetFactories(IUnityContainer container) {
            int order = 0;
            container.RegisterType<IFacetFactory, FallbackFacetFactory>("FallbackFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SystemClassMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeMarkerFacetFactory>("TypeMarkerFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            container.RegisterType<IFacetFactory, MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionMethodsFacetFactory>("ActionMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IconMethodFacetFactory>("IconMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TitleMethodFacetFactory>("TitleMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateObjectFacetFactory>("ValidateObjectFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ViewModelFacetFactory>("ViewModelFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumFacetFactory>("EnumFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DefaultNamingFacetFactory>("DefaultNamingFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // must come after Named and Plural factories
            container.RegisterType<IFacetFactory, ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FinderActionFacetFactory>("FinderActionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles
            container.RegisterType<IFacetFactory, MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            container.RegisterType<IFacetFactory, RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IntValueTypeFacetFactory>("IntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, LongValueTypeFacetFactory>("LongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CharValueTypeFacetFactory>("CharValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, StringValueTypeFacetFactory>("StringValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFacetFactory>("CollectionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order)); // written to not trample over TypeOf if already installed
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            RegisterFacetFactories(container);

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IMenuFactory, NullMenuFactory>();
        }

        [TestMethod]
        public void ReflectNoTypes() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        [TestMethod]
        public void ReflectObjectType() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (object)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.AreEqual(1, reflector.AllObjectSpecImmutables.Count());
        
            AbstractReflectorTest.AssertSpec(typeof(object), reflector.AllObjectSpecImmutables.First());
        }

        [TestMethod]
        public void ReflectListTypes() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (List<object>), typeof (List<int>), typeof (object), typeof (int)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(int), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(object), specs[1]);
            AbstractReflectorTest.AssertSpec(typeof(List<>), specs[2]);
        }

        [TestMethod]
        public void ReflectSetTypes() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (SetWrapper<>), typeof (object)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(SetWrapper<>), specs[1]);
        }

        [TestMethod]
        public void ReflectQueryableTypes() {
            IUnityContainer container = GetContainer();
            IQueryable<object> qo = new List<object>().AsQueryable();
            IQueryable<int> qi = new List<int>().AsQueryable();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {qo.GetType(), qi.GetType(), typeof (int), typeof (object)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(int), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(object), specs[1]);
            AbstractReflectorTest.AssertSpec(typeof(EnumerableQuery<>), specs[2]);
        }

        [TestMethod]
        public void ReflectWhereIterator() {
            IUnityContainer container = GetContainer();
            IEnumerable<int> it = new List<int> { 1, 2, 3 }.Where(i => i == 2);
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] { it.GetType().GetGenericTypeDefinition(), typeof(Object) }, new Type[] { }, new string[] { });
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs[0]);
            AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs[1]);
        }


        [TestMethod]
        public void ReflectWhereSelectIterator() {
            IUnityContainer container = GetContainer();
            IEnumerable<int> it = new List<int> {1, 2, 3}.Where(i => i == 2).Select(i => i);
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {it.GetType().GetGenericTypeDefinition(), typeof (Object)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(object), specs[0]);
            AbstractReflectorTest.AssertSpec(it.GetType().GetGenericTypeDefinition(), specs[1]);
        }

        [TestMethod]
        public void ReflectByteArray() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (TestObjectWithByteArray)}, new Type[] {}, new[] {"System"});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(31, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(System.Collections.IList), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<long>), specs[1]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<int>), specs[2]);
            AbstractReflectorTest.AssertSpec(typeof(int), specs[3]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<byte>), specs[4]);
            AbstractReflectorTest.AssertSpec(typeof(IConvertible), specs[5]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<byte>), specs[6]);
            AbstractReflectorTest.AssertSpec(typeof(object), specs[7]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<bool>), specs[8]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<bool>), specs[9]);
            AbstractReflectorTest.AssertSpec(typeof(byte[]), specs[10]);
            AbstractReflectorTest.AssertSpec(typeof(Array), specs[11]);
            AbstractReflectorTest.AssertSpec(typeof(ValueType), specs[12]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<long>), specs[13]);
            AbstractReflectorTest.AssertSpec(typeof(long), specs[14]);
            AbstractReflectorTest.AssertSpec(typeof(IStructuralComparable), specs[15]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable), specs[16]);
            AbstractReflectorTest.AssertSpec(typeof(ICollection), specs[17]);
            AbstractReflectorTest.AssertSpec(typeof(bool), specs[18]);
            AbstractReflectorTest.AssertSpec(typeof(ICloneable), specs[19]);
            AbstractReflectorTest.AssertSpec(typeof(IList<>), specs[20]);
            AbstractReflectorTest.AssertSpec(typeof(byte), specs[21]);
            AbstractReflectorTest.AssertSpec(typeof(IFormattable), specs[22]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<int>), specs[23]);
            AbstractReflectorTest.AssertSpec(typeof(IReadOnlyList<>), specs[24]);
            AbstractReflectorTest.AssertSpec(typeof(IReadOnlyCollection<>), specs[25]);
            AbstractReflectorTest.AssertSpec(typeof(IStructuralEquatable), specs[26]);
            AbstractReflectorTest.AssertSpec(typeof(ICollection<>), specs[27]);
            AbstractReflectorTest.AssertSpec(typeof(TestObjectWithByteArray), specs[28]);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable<>), specs[29]);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable), specs[30]);
        }


        [TestMethod]
        public void ReflectStringArray() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (TestObjectWithStringArray), typeof (string)}, new Type[] {}, new string[] {});
            rc.SupportedSystemTypes.Clear();

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(TestObjectWithStringArray), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(string), specs[1]);
        }

        [TestMethod]
        public void ReflectWithScalars() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (WithScalars)}, new Type[] {}, new[] {"System"});
            rc.SupportedSystemTypes.Clear();
            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(74, specs.Length);

        }

        [TestMethod]
        public void ReflectSimpleDomainObject() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = new ReflectorConfiguration(new[] {typeof (SimpleDomainObject)}, new Type[] {}, new[] {"System"});
            rc.SupportedSystemTypes.Clear();
            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(19, specs.Length);

            AbstractReflectorTest.AssertSpec(typeof(IComparable<string>), specs[0]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<int>), specs[1]);
            AbstractReflectorTest.AssertSpec(typeof(int), specs[2]);
            AbstractReflectorTest.AssertSpec(typeof(IConvertible), specs[3]);
            AbstractReflectorTest.AssertSpec(typeof(object), specs[4]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<char>), specs[5]);
            AbstractReflectorTest.AssertSpec(typeof(char), specs[6]);
            AbstractReflectorTest.AssertSpec(typeof(ValueType), specs[7]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable), specs[8]);
            AbstractReflectorTest.AssertSpec(typeof(ICloneable), specs[9]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<string>), specs[10]);
            AbstractReflectorTest.AssertSpec(typeof(IEquatable<char>), specs[11]);
            AbstractReflectorTest.AssertSpec(typeof(void), specs[12]);
            AbstractReflectorTest.AssertSpec(typeof(IFormattable), specs[13]);
            AbstractReflectorTest.AssertSpec(typeof(IComparable<int>), specs[14]);
            AbstractReflectorTest.AssertSpec(typeof(SimpleDomainObject), specs[15]);
            AbstractReflectorTest.AssertSpec(typeof(string), specs[16]);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable<>), specs[17]);
            AbstractReflectorTest.AssertSpec(typeof(IEnumerable), specs[18]);
        }


        private static Type[] Types
        {
            get
            {
                return new[] {
                    typeof (EnumerableQuery<object>),
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    //typeof (ActionResultModelQ<object>),
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof (QuickOrderForm),
                    //typeof (ActionResultModelQ<>),
                    //typeof (ActionResultModel<>)
                };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                    typeof (ContactRepository),
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository),
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions)
                };
            }
        }

        
        public static ReflectorConfiguration ReflectorConfig()
        {
            return new ReflectorConfiguration(Types, Services, Types.Select(t => t.Namespace).Distinct().ToArray(), MainMenus);
        }

     

       
        public static IMenu[] MainMenus(IMenuFactory factory)
        {
            return new IMenu[] { };
        }



        [TestMethod]
        public void ReflectAWBenchmark()
        {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;

            var rc = ReflectorConfig();
          
            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var sw = new Stopwatch();

            sw.Start();
            reflector.Reflect();
            sw.Stop();

            Console.WriteLine("AW reflect time= " + sw.ElapsedMilliseconds.ToString() + "ms");

            var specs = reflector.AllObjectSpecImmutables;
            
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

        #region Nested type: TestObjectWithStringArray

        public class TestObjectWithStringArray {
            public string[] StringArray { get; set; }
        }

        #endregion

        public class WithScalars {
            private DateTime dateTime = DateTime.Parse("2012-03-27T09:42:36");
            private ICollection<WithScalars> list = new List<WithScalars>();
            private ICollection<WithScalars> set = new HashSet<WithScalars>();

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

            private void Init() {
                SByte = 10;
                UInt = 14;
                ULong = 15;
                UShort = 16;
            }

            public virtual char Char {
                get { return '3'; }
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

            public virtual DateTime DateTime {
                get { return dateTime; }
                set { dateTime = value; }
            }

            public virtual ICollection<WithScalars> List {
                get { return list; }
                set { list = value; }
            }

            [NotMapped]
            public virtual ICollection<WithScalars> Set {
                get { return set; }
                set { set = value; }
            }

            [EnumDataType(typeof (TestEnum))]
            public virtual int EnumByAttributeChoices { get; set; }
        }

        public class SimpleDomainObject {
            [Key, Title, ConcurrencyCheck]
            public virtual int Id { get; set; }

            public virtual void Action() {}

            public virtual string HideAction() {
                return null;
            }
        }
    }
}