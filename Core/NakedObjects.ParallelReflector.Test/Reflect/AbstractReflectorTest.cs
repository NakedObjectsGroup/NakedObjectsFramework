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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Component;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.TypeFacetFactory;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.ParallelReflect.Test {
    public abstract class AbstractReflectorTest {
        private readonly IFacetFactory[] facetFactories = {
            new FallbackFacetFactory(0),
            new IteratorFilteringFacetFactory(1),
            new SystemClassMethodFilteringFactory(2),
            new RemoveSuperclassMethodsFacetFactory(3),
            new RemoveDynamicProxyMethodsFacetFactory(5),
            new RemoveEventHandlerMethodsFacetFactory(6),
            new TypeMarkerFacetFactory(7),
            new MandatoryDefaultFacetFactory(8),
            new PropertyValidateDefaultFacetFactory(9),
            new ComplementaryMethodsFilteringFacetFactory(10),
            new ActionMethodsFacetFactory(11),
            new CollectionFieldMethodsFacetFactory(12),
            new PropertyMethodsFacetFactory(13),
            new IconMethodFacetFactory(14),
            new CallbackMethodsFacetFactory(15),
            new TitleMethodFacetFactory(16),
            new ValidateObjectFacetFactory(17),
            new ComplexTypeAnnotationFacetFactory(19),
            new ViewModelFacetFactory(20),
            new BoundedAnnotationFacetFactory(21),
            new EnumFacetFactory(22),
            new ActionDefaultAnnotationFacetFactory(23),
            new PropertyDefaultAnnotationFacetFactory(24),
            new DescribedAsAnnotationFacetFactory(25),
            new DisabledAnnotationFacetFactory(26),
            new PasswordAnnotationFacetFactory(27),
            new ExecutedAnnotationFacetFactory(28),
            new PotencyAnnotationFacetFactory(29),
            new PageSizeAnnotationFacetFactory(30),
            new HiddenAnnotationFacetFactory(32),
            new HiddenDefaultMethodFacetFactory(33),
            new DisableDefaultMethodFacetFactory(34),
            new AuthorizeAnnotationFacetFactory(35),
            new ValidateProgrammaticUpdatesAnnotationFacetFactory(36),
            new ImmutableAnnotationFacetFactory(37),
            new MaxLengthAnnotationFacetFactory(38),
            new RangeAnnotationFacetFactory(39),
            new MemberOrderAnnotationFacetFactory(40),
            new MultiLineAnnotationFacetFactory(41),
            new NamedAnnotationFacetFactory(42),
            new NotPersistedAnnotationFacetFactory(43),
            new ProgramPersistableOnlyAnnotationFacetFactory(44),
            new OptionalAnnotationFacetFactory(45),
            new RequiredAnnotationFacetFactory(46),
            new PluralAnnotationFacetFactory(47),
            new DefaultNamingFacetFactory(48),
            new ConcurrencyCheckAnnotationFacetFactory(50),
            new ContributedActionAnnotationFacetFactory(51),
            new FinderActionFacetFactory(52),
            new MaskAnnotationFacetFactory(53),
            new RegExAnnotationFacetFactory(54),
            new TypeOfAnnotationFacetFactory(55),
            new TableViewAnnotationFacetFactory(56),
            new TypicalLengthDerivedFromTypeFacetFactory(57),
            new TypicalLengthAnnotationFacetFactory(58),
            new EagerlyAnnotationFacetFactory(59),
            new PresentationHintAnnotationFacetFactory(60),
            new BooleanValueTypeFacetFactory(61),
            new ByteValueTypeFacetFactory(62),
            new SbyteValueTypeFacetFactory(63),
            new ShortValueTypeFacetFactory(64),
            new IntValueTypeFacetFactory(65),
            new LongValueTypeFacetFactory(66),
            new UShortValueTypeFacetFactory(67),
            new UIntValueTypeFacetFactory(68),
            new ULongValueTypeFacetFactory(69),
            new FloatValueTypeFacetFactory(70),
            new DoubleValueTypeFacetFactory(71),
            new DecimalValueTypeFacetFactory(72),
            new CharValueTypeFacetFactory(73),
            new DateTimeValueTypeFacetFactory(74),
            new TimeValueTypeFacetFactory(75),
            new StringValueTypeFacetFactory(76),
            new GuidValueTypeFacetFactory(77),
            new EnumValueTypeFacetFactory(78),
            new FileAttachmentValueTypeFacetFactory(79),
            new ImageValueTypeFacetFactory(80),
            new ArrayValueTypeFacetFactory<byte>(81),
            new CollectionFacetFactory(82)
        };

        protected IImmutableDictionary<string, ITypeSpecBuilder> Metamodel;
        protected IObjectSpecImmutable Specification;

        protected void AssertIsInstanceOfType<T>(object o) {
            Assert.IsInstanceOfType(o, typeof(T));
        }

        [TestInitialize]
        public virtual void SetUp() {
            var cache = new ImmutableInMemorySpecCache();
            ReflectorConfiguration.NoValidate = true;
            var config = new ReflectorConfiguration(new[] {typeof(List<TestPoco>), typeof(ArrayList)}, new Type[] { }, new[] {typeof(TestPoco).Namespace});
            var menuFactory = new NullMenuFactory();
            var classStrategy = new DefaultClassStrategy(config);
            var metamodel = new Metamodel(classStrategy, cache);
            var reflector = new ParallelReflector(classStrategy, metamodel, config, menuFactory, new IFacetDecorator[] { }, facetFactories);

            var result = LoadSpecification(reflector);
            Specification = result.Item1 as IObjectSpecImmutable;
            Metamodel = result.Item2;
        }

        protected abstract Tuple<ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>> LoadSpecification(ParallelReflector reflector);

        public static void AssertSpec(Type type, ITypeSpecBuilder spec) {
            Assert.IsNotNull(spec, type.FullName);
            Assert.AreEqual(new IdentifierImpl(type.FullName), spec.Identifier);
            Assert.AreEqual(type.FullName, spec.FullName);
            Assert.AreEqual(TypeNameUtils.GetShortName(type.Name), spec.ShortName);
            Assert.AreEqual(type, spec.Type);
        }

        public static void AssertSpecsContain(Type type, ITypeSpecBuilder[] specs) {

            foreach (var spec in specs) {
                if (type.FullName == spec.FullName) {
                    AssertSpec(type, spec);
                    return;
                }
            }

            Assert.Fail("Spec missing: " + type.FullName);
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