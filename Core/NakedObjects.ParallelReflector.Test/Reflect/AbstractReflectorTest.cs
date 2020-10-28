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
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Component;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.TypeFacetFactory;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.FacetFactory;

namespace NakedObjects.ParallelReflect.Test {
    public abstract class AbstractReflectorTest {
        private static readonly ILoggerFactory MockLoggerFactory = new Mock<ILoggerFactory>().Object;

        private readonly IFacetFactory[] facetFactories = {
            new FallbackFacetFactory(new FacetFactoryOrder<FallbackFacetFactory>(), MockLoggerFactory),
            new IteratorFilteringFacetFactory(new FacetFactoryOrder<IteratorFilteringFacetFactory>(), MockLoggerFactory),
            new SystemClassMethodFilteringFactory(new FacetFactoryOrder<SystemClassMethodFilteringFactory>(), MockLoggerFactory),
            new RemoveSuperclassMethodsFacetFactory(new FacetFactoryOrder<RemoveSuperclassMethodsFacetFactory>(), MockLoggerFactory),
            new RemoveDynamicProxyMethodsFacetFactory(new FacetFactoryOrder<RemoveDynamicProxyMethodsFacetFactory>(), MockLoggerFactory),
            new RemoveEventHandlerMethodsFacetFactory(new FacetFactoryOrder<RemoveEventHandlerMethodsFacetFactory>(), MockLoggerFactory),
            new TypeMarkerFacetFactory(new FacetFactoryOrder<TypeMarkerFacetFactory>(), MockLoggerFactory),
            new MandatoryDefaultFacetFactory(new FacetFactoryOrder<MandatoryDefaultFacetFactory>(), MockLoggerFactory),
            new PropertyValidateDefaultFacetFactory(new FacetFactoryOrder<PropertyValidateDefaultFacetFactory>(), MockLoggerFactory),
            new ComplementaryMethodsFilteringFacetFactory(new FacetFactoryOrder<ComplementaryMethodsFilteringFacetFactory>(), MockLoggerFactory),
            new ActionMethodsFacetFactory(new FacetFactoryOrder<ActionMethodsFacetFactory>(), MockLoggerFactory),
            new CollectionFieldMethodsFacetFactory(new FacetFactoryOrder<CollectionFieldMethodsFacetFactory>(), MockLoggerFactory),
            new PropertyMethodsFacetFactory(new FacetFactoryOrder<PropertyMethodsFacetFactory>(), MockLoggerFactory),
            new IconMethodFacetFactory(new FacetFactoryOrder<IconMethodFacetFactory>(), MockLoggerFactory),
            new CallbackMethodsFacetFactory(new FacetFactoryOrder<CallbackMethodsFacetFactory>(), MockLoggerFactory),
            new TitleMethodFacetFactory(new FacetFactoryOrder<TitleMethodFacetFactory>(), MockLoggerFactory),
            new ValidateObjectFacetFactory(new FacetFactoryOrder<ValidateObjectFacetFactory>(), MockLoggerFactory),
            new ComplexTypeAnnotationFacetFactory(new FacetFactoryOrder<ComplexTypeAnnotationFacetFactory>(), MockLoggerFactory),
            new ViewModelFacetFactory(new FacetFactoryOrder<ViewModelFacetFactory>(), MockLoggerFactory),
            new BoundedAnnotationFacetFactory(new FacetFactoryOrder<BoundedAnnotationFacetFactory>(), MockLoggerFactory),
            new EnumFacetFactory(new FacetFactoryOrder<EnumFacetFactory>(), MockLoggerFactory),
            new ActionDefaultAnnotationFacetFactory(new FacetFactoryOrder<ActionDefaultAnnotationFacetFactory>(), MockLoggerFactory),
            new PropertyDefaultAnnotationFacetFactory(new FacetFactoryOrder<PropertyDefaultAnnotationFacetFactory>(), MockLoggerFactory),
            new DescribedAsAnnotationFacetFactory(new FacetFactoryOrder<DescribedAsAnnotationFacetFactory>(), MockLoggerFactory),
            new DisabledAnnotationFacetFactory(new FacetFactoryOrder<DisabledAnnotationFacetFactory>(), MockLoggerFactory),
            new PasswordAnnotationFacetFactory(new FacetFactoryOrder<PasswordAnnotationFacetFactory>(), MockLoggerFactory),
            new ExecutedAnnotationFacetFactory(new FacetFactoryOrder<ExecutedAnnotationFacetFactory>(), MockLoggerFactory),
            new PotencyAnnotationFacetFactory(new FacetFactoryOrder<PotencyAnnotationFacetFactory>(), MockLoggerFactory),
            new PageSizeAnnotationFacetFactory(new FacetFactoryOrder<PageSizeAnnotationFacetFactory>(), MockLoggerFactory),
            new HiddenAnnotationFacetFactory(new FacetFactoryOrder<HiddenAnnotationFacetFactory>(), MockLoggerFactory),
            new HiddenDefaultMethodFacetFactory(new FacetFactoryOrder<HiddenDefaultMethodFacetFactory>(), MockLoggerFactory),
            new DisableDefaultMethodFacetFactory(new FacetFactoryOrder<DisableDefaultMethodFacetFactory>(), MockLoggerFactory),
            new AuthorizeAnnotationFacetFactory(new FacetFactoryOrder<AuthorizeAnnotationFacetFactory>(), MockLoggerFactory),
            new ValidateProgrammaticUpdatesAnnotationFacetFactory(new FacetFactoryOrder<ValidateProgrammaticUpdatesAnnotationFacetFactory>(), MockLoggerFactory),
            new ImmutableAnnotationFacetFactory(new FacetFactoryOrder<ImmutableAnnotationFacetFactory>(), MockLoggerFactory),
            new MaxLengthAnnotationFacetFactory(new FacetFactoryOrder<MaxLengthAnnotationFacetFactory>(), MockLoggerFactory),
            new RangeAnnotationFacetFactory(new FacetFactoryOrder<RangeAnnotationFacetFactory>(), MockLoggerFactory),
            new MemberOrderAnnotationFacetFactory(new FacetFactoryOrder<MemberOrderAnnotationFacetFactory>(), MockLoggerFactory),
            new MultiLineAnnotationFacetFactory(new FacetFactoryOrder<MultiLineAnnotationFacetFactory>(), MockLoggerFactory),
            new NamedAnnotationFacetFactory(new FacetFactoryOrder<NamedAnnotationFacetFactory>(), MockLoggerFactory),
            new NotPersistedAnnotationFacetFactory(new FacetFactoryOrder<NotPersistedAnnotationFacetFactory>(), MockLoggerFactory),
            new ProgramPersistableOnlyAnnotationFacetFactory(new FacetFactoryOrder<ProgramPersistableOnlyAnnotationFacetFactory>(), MockLoggerFactory),
            new OptionalAnnotationFacetFactory(new FacetFactoryOrder<OptionalAnnotationFacetFactory>(), MockLoggerFactory),
            new RequiredAnnotationFacetFactory(new FacetFactoryOrder<RequiredAnnotationFacetFactory>(), MockLoggerFactory),
            new PluralAnnotationFacetFactory(new FacetFactoryOrder<PluralAnnotationFacetFactory>(), MockLoggerFactory),
            new DefaultNamingFacetFactory(new FacetFactoryOrder<DefaultNamingFacetFactory>(), MockLoggerFactory),
            new ConcurrencyCheckAnnotationFacetFactory(new FacetFactoryOrder<ConcurrencyCheckAnnotationFacetFactory>(), MockLoggerFactory),
            new ContributedActionAnnotationFacetFactory(new FacetFactoryOrder<ContributedActionAnnotationFacetFactory>(), MockLoggerFactory),
            new FinderActionFacetFactory(new FacetFactoryOrder<FinderActionFacetFactory>(), MockLoggerFactory),
            new MaskAnnotationFacetFactory(new FacetFactoryOrder<MaskAnnotationFacetFactory>(), MockLoggerFactory),
            new RegExAnnotationFacetFactory(new FacetFactoryOrder<RegExAnnotationFacetFactory>(), MockLoggerFactory),
            new TypeOfAnnotationFacetFactory(new FacetFactoryOrder<TypeOfAnnotationFacetFactory>(), MockLoggerFactory),
            new TableViewAnnotationFacetFactory(new FacetFactoryOrder<TableViewAnnotationFacetFactory>(), MockLoggerFactory),
            new TypicalLengthDerivedFromTypeFacetFactory(new FacetFactoryOrder<TypicalLengthDerivedFromTypeFacetFactory>(), MockLoggerFactory),
            new TypicalLengthAnnotationFacetFactory(new FacetFactoryOrder<TypicalLengthAnnotationFacetFactory>(), MockLoggerFactory),
            new EagerlyAnnotationFacetFactory(new FacetFactoryOrder<EagerlyAnnotationFacetFactory>(), MockLoggerFactory),
            new PresentationHintAnnotationFacetFactory(new FacetFactoryOrder<PresentationHintAnnotationFacetFactory>(), MockLoggerFactory),
            new BooleanValueTypeFacetFactory(new FacetFactoryOrder<BooleanValueTypeFacetFactory>(), MockLoggerFactory),
            new ByteValueTypeFacetFactory(new FacetFactoryOrder<ByteValueTypeFacetFactory>(), MockLoggerFactory),
            new SbyteValueTypeFacetFactory(new FacetFactoryOrder<SbyteValueTypeFacetFactory>(), MockLoggerFactory),
            new ShortValueTypeFacetFactory(new FacetFactoryOrder<ShortValueTypeFacetFactory>(), MockLoggerFactory),
            new IntValueTypeFacetFactory(new FacetFactoryOrder<IntValueTypeFacetFactory>(), MockLoggerFactory),
            new LongValueTypeFacetFactory(new FacetFactoryOrder<LongValueTypeFacetFactory>(), MockLoggerFactory),
            new UShortValueTypeFacetFactory(new FacetFactoryOrder<UShortValueTypeFacetFactory>(), MockLoggerFactory),
            new UIntValueTypeFacetFactory(new FacetFactoryOrder<UIntValueTypeFacetFactory>(), MockLoggerFactory),
            new ULongValueTypeFacetFactory(new FacetFactoryOrder<ULongValueTypeFacetFactory>(), MockLoggerFactory),
            new FloatValueTypeFacetFactory(new FacetFactoryOrder<FloatValueTypeFacetFactory>(), MockLoggerFactory),
            new DoubleValueTypeFacetFactory(new FacetFactoryOrder<DoubleValueTypeFacetFactory>(), MockLoggerFactory),
            new DecimalValueTypeFacetFactory(new FacetFactoryOrder<DecimalValueTypeFacetFactory>(), MockLoggerFactory),
            new CharValueTypeFacetFactory(new FacetFactoryOrder<CharValueTypeFacetFactory>(), MockLoggerFactory),
            new DateTimeValueTypeFacetFactory(new FacetFactoryOrder<DateTimeValueTypeFacetFactory>(), MockLoggerFactory),
            new TimeValueTypeFacetFactory(new FacetFactoryOrder<TimeValueTypeFacetFactory>(), MockLoggerFactory),
            new StringValueTypeFacetFactory(new FacetFactoryOrder<StringValueTypeFacetFactory>(), MockLoggerFactory),
            new GuidValueTypeFacetFactory(new FacetFactoryOrder<GuidValueTypeFacetFactory>(), MockLoggerFactory),
            new EnumValueTypeFacetFactory(new FacetFactoryOrder<EnumValueTypeFacetFactory>(), MockLoggerFactory),
            new FileAttachmentValueTypeFacetFactory(new FacetFactoryOrder<FileAttachmentValueTypeFacetFactory>(), MockLoggerFactory),
            new ImageValueTypeFacetFactory(new FacetFactoryOrder<ImageValueTypeFacetFactory>(), MockLoggerFactory),
            new ArrayValueTypeFacetFactory<byte>(new FacetFactoryOrder<ArrayValueTypeFacetFactory<byte>>(), MockLoggerFactory),
            new CollectionFacetFactory(new FacetFactoryOrder<CollectionFacetFactory>(), MockLoggerFactory)
        };

        protected IImmutableDictionary<string, ITypeSpecBuilder> Metamodel;
        protected IObjectSpecImmutable Specification;
        protected IClassStrategy ClassStrategy;

        protected static void AssertIsInstanceOfType<T>(object o) => Assert.IsInstanceOfType(o, typeof(T));

        [TestInitialize]
        public virtual void SetUp() {
            var cache = new ImmutableInMemorySpecCache();
            ObjectReflectorConfiguration.NoValidate = true;
            var config = new ObjectReflectorConfiguration(new[] {typeof(List<TestPoco>), typeof(ArrayList)}, new Type[] { }, new[] {typeof(TestPoco).Namespace});
            var functionalReflectorConfiguration = new FunctionalReflectorConfiguration(new Type[] { }, new Type[] { });

            var menuFactory = new NullMenuFactory();
            ClassStrategy = new ObjectClassStrategy(config);
            var mockLogger = new Mock<ILogger<Metamodel>>().Object;

            var metamodel = new Metamodel(cache, mockLogger);
            var mockLogger1 = new Mock<ILogger<ParallelReflector>>().Object;
            var mockLoggerFactory = new Mock<ILoggerFactory>().Object;

            var reflector = new ObjectReflector(metamodel, config, new IFacetDecorator[] { }, facetFactories, mockLoggerFactory, mockLogger1);

            ITypeSpecBuilder spec;
            (spec, Metamodel) = LoadSpecification(reflector);
            Specification = spec as IObjectSpecImmutable;
        }

        protected abstract (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(ParallelReflector reflector);

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

        private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs) => specs.SingleOrDefault(s => s.FullName == type.FullName);

        public static void AssertSpec(Type type, ITypeSpecBuilder[] specs) => AssertSpec(type, GetSpec(type, specs));
    }

    // Copyright (c) Naked Objects Group Ltd.
}