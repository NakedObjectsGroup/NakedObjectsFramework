// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect.Component;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;

namespace NakedObjects.Reflect.Test {
    public abstract class AbstractReflectorTest {
        private static readonly ILoggerFactory MockLoggerFactory = new Mock<ILoggerFactory>().Object;

        private readonly IFacetFactory[] facetFactories = {
            new FallbackFacetFactory(0, MockLoggerFactory),
            new IteratorFilteringFacetFactory(1, MockLoggerFactory),
            new SystemClassMethodFilteringFactory(2, MockLoggerFactory),
            new RemoveSuperclassMethodsFacetFactory(3, MockLoggerFactory),
            new RemoveDynamicProxyMethodsFacetFactory(5, MockLoggerFactory),
            new RemoveEventHandlerMethodsFacetFactory(6, MockLoggerFactory),
            new TypeMarkerFacetFactory(7, MockLoggerFactory),
            new MandatoryDefaultFacetFactory(8, MockLoggerFactory),
            new PropertyValidateDefaultFacetFactory(9, MockLoggerFactory),
            new ComplementaryMethodsFilteringFacetFactory(10, MockLoggerFactory),
            new ActionMethodsFacetFactory(11, MockLoggerFactory),
            new CollectionFieldMethodsFacetFactory(12, MockLoggerFactory),
            new PropertyMethodsFacetFactory(13, MockLoggerFactory),
            new IconMethodFacetFactory(14, MockLoggerFactory),
            new CallbackMethodsFacetFactory(15, MockLoggerFactory),
            new TitleMethodFacetFactory(16, MockLoggerFactory),
            new ValidateObjectFacetFactory(17, MockLoggerFactory),
            new ComplexTypeAnnotationFacetFactory(19, MockLoggerFactory),
            new ViewModelFacetFactory(20, MockLoggerFactory),
            new BoundedAnnotationFacetFactory(21, MockLoggerFactory),
            new EnumFacetFactory(22, MockLoggerFactory),
            new ActionDefaultAnnotationFacetFactory(23, MockLoggerFactory),
            new PropertyDefaultAnnotationFacetFactory(24, MockLoggerFactory),
            new DescribedAsAnnotationFacetFactory(25, MockLoggerFactory),
            new DisabledAnnotationFacetFactory(26, MockLoggerFactory),
            new PasswordAnnotationFacetFactory(27, MockLoggerFactory),
            new ExecutedAnnotationFacetFactory(28, MockLoggerFactory),
            new PotencyAnnotationFacetFactory(29, MockLoggerFactory),
            new PageSizeAnnotationFacetFactory(30, MockLoggerFactory),
            new HiddenAnnotationFacetFactory(32, MockLoggerFactory),
            new HiddenDefaultMethodFacetFactory(33, MockLoggerFactory),
            new DisableDefaultMethodFacetFactory(34, MockLoggerFactory),
            new AuthorizeAnnotationFacetFactory(35, MockLoggerFactory),
            new ValidateProgrammaticUpdatesAnnotationFacetFactory(36, MockLoggerFactory),
            new ImmutableAnnotationFacetFactory(37, MockLoggerFactory),
            new MaxLengthAnnotationFacetFactory(38, MockLoggerFactory),
            new RangeAnnotationFacetFactory(39, MockLoggerFactory),
            new MemberOrderAnnotationFacetFactory(40, MockLoggerFactory),
            new MultiLineAnnotationFacetFactory(41, MockLoggerFactory),
            new NamedAnnotationFacetFactory(42, MockLoggerFactory),
            new NotPersistedAnnotationFacetFactory(43, MockLoggerFactory),
            new ProgramPersistableOnlyAnnotationFacetFactory(44, MockLoggerFactory),
            new OptionalAnnotationFacetFactory(45, MockLoggerFactory),
            new RequiredAnnotationFacetFactory(46, MockLoggerFactory),
            new PluralAnnotationFacetFactory(47, MockLoggerFactory),
            new DefaultNamingFacetFactory(48, MockLoggerFactory),
            new ConcurrencyCheckAnnotationFacetFactory(50, MockLoggerFactory),
            new ContributedActionAnnotationFacetFactory(51, MockLoggerFactory),
            new FinderActionFacetFactory(52, MockLoggerFactory),
            new MaskAnnotationFacetFactory(53, MockLoggerFactory),
            new RegExAnnotationFacetFactory(54, MockLoggerFactory),
            new TypeOfAnnotationFacetFactory(55, MockLoggerFactory),
            new TableViewAnnotationFacetFactory(56, MockLoggerFactory),
            new TypicalLengthDerivedFromTypeFacetFactory(57, MockLoggerFactory),
            new TypicalLengthAnnotationFacetFactory(58, MockLoggerFactory),
            new EagerlyAnnotationFacetFactory(59, MockLoggerFactory),
            new PresentationHintAnnotationFacetFactory(60, MockLoggerFactory),
            new BooleanValueTypeFacetFactory(61, MockLoggerFactory),
            new ByteValueTypeFacetFactory(62, MockLoggerFactory),
            new SbyteValueTypeFacetFactory(63, MockLoggerFactory),
            new ShortValueTypeFacetFactory(64, MockLoggerFactory),
            new IntValueTypeFacetFactory(65, MockLoggerFactory),
            new LongValueTypeFacetFactory(66, MockLoggerFactory),
            new UShortValueTypeFacetFactory(67, MockLoggerFactory),
            new UIntValueTypeFacetFactory(68, MockLoggerFactory),
            new ULongValueTypeFacetFactory(69, MockLoggerFactory),
            new FloatValueTypeFacetFactory(70, MockLoggerFactory),
            new DoubleValueTypeFacetFactory(71, MockLoggerFactory),
            new DecimalValueTypeFacetFactory(72, MockLoggerFactory),
            new CharValueTypeFacetFactory(73, MockLoggerFactory),
            new DateTimeValueTypeFacetFactory(74, MockLoggerFactory),
            new TimeValueTypeFacetFactory(75, MockLoggerFactory),
            new StringValueTypeFacetFactory(76, MockLoggerFactory),
            new GuidValueTypeFacetFactory(77, MockLoggerFactory),
            new EnumValueTypeFacetFactory(78, MockLoggerFactory),
            new FileAttachmentValueTypeFacetFactory(79, MockLoggerFactory),
            new ImageValueTypeFacetFactory(80, MockLoggerFactory),
            new ArrayValueTypeFacetFactory<byte>(81, MockLoggerFactory),
            new CollectionFacetFactory(82, MockLoggerFactory)
        };

        protected IMetamodel Metamodel;
        protected IObjectSpecImmutable Specification;

        protected static void AssertIsInstanceOfType<T>(object o) {
            Assert.IsInstanceOfType(o, typeof(T));
        }

        [TestInitialize]
        public virtual void SetUp() {
            var cache = new ImmutableInMemorySpecCache();
            ReflectorConfiguration.NoValidate = true;
            var config = new ReflectorConfiguration(new[] {typeof(List<TestPoco>), typeof(ArrayList)}, new Type[] { }, new[] {typeof(TestPoco).Namespace});
            var menuFactory = new NullMenuFactory();
            var classStrategy = new DefaultClassStrategy(config);
            var mockLogger = new Mock<ILogger<Metamodel>>().Object;
            var metamodel = new Metamodel(classStrategy, cache, mockLogger);
            var mockLogger1 = new Mock<ILogger<Reflector>>().Object;
            var mockLoggerFactory = new Mock<ILoggerFactory>().Object;

            var reflector = new Reflector(classStrategy, metamodel, config, menuFactory, new IFacetDecorator[] { }, facetFactories, mockLoggerFactory, mockLogger1);

            Specification = LoadSpecification(reflector);
            Metamodel = metamodel;
        }

        protected abstract IObjectSpecImmutable LoadSpecification(Reflector reflector);

        public static void AssertSpec(Type type, ITypeSpecBuilder spec) {
            Assert.AreEqual(new IdentifierImpl(type.FullName), spec.Identifier);
            Assert.AreEqual(type.FullName, spec.FullName);
            Assert.AreEqual(TypeNameUtils.GetShortName(type.Name), spec.ShortName);
            Assert.AreEqual(type, spec.Type);
        }

        private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs) {
            return specs.SingleOrDefault(s => s.FullName == type.FullName);
        }

        public static void AssertSpec(Type type, ITypeSpecBuilder[] specs) {
            AssertSpec(type, GetSpec(type, specs));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}