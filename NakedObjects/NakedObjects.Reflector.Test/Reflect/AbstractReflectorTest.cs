// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.TypeFacetFactory;
using NakedObjects.DependencyInjection.FacetFactory;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Component;
using NakedObjects.ParallelReflector.Component;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Reflect;
using BooleanValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.BooleanValueTypeFacetFactory;
using ByteValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.ByteValueTypeFacetFactory;
using CharValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.CharValueTypeFacetFactory;
using DateTimeValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.DateTimeValueTypeFacetFactory;
using DecimalValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.DecimalValueTypeFacetFactory;
using DoubleValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.DoubleValueTypeFacetFactory;
using EnumValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.EnumValueTypeFacetFactory;
using FileAttachmentValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.FileAttachmentValueTypeFacetFactory;
using FloatValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.FloatValueTypeFacetFactory;
using GuidValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.GuidValueTypeFacetFactory;
using ImageValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.ImageValueTypeFacetFactory;
using IntValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.IntValueTypeFacetFactory;
using LongValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.LongValueTypeFacetFactory;
using SbyteValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.SbyteValueTypeFacetFactory;
using ShortValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.ShortValueTypeFacetFactory;
using StringValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.StringValueTypeFacetFactory;
using TimeValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.TimeValueTypeFacetFactory;
using UIntValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.UIntValueTypeFacetFactory;
using ULongValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.ULongValueTypeFacetFactory;
using UShortValueTypeFacetFactory = NakedObjects.Reflector.TypeFacetFactory.UShortValueTypeFacetFactory;

namespace NakedObjects.Reflector.Test.Reflect {
    public abstract class AbstractReflectorTest {
        private static readonly ILoggerFactory MockLoggerFactory = new Mock<ILoggerFactory>().Object;
        protected IClassStrategy ClassStrategy;
        protected IImmutableDictionary<string, ITypeSpecBuilder> Metamodel;
        protected IObjectSpecImmutable Specification;

        protected IFacetFactory[] FacetFactories =>
            new[] {
                NewFacetFactory<FallbackFacetFactory>(),
                NewFacetFactory<IteratorFilteringFacetFactory>(),
                NewFacetFactory<SystemClassMethodFilteringFactory>(),
                NewFacetFactory<RemoveSuperclassMethodsFacetFactory>(),
                NewFacetFactory<RemoveDynamicProxyMethodsFacetFactory>(),
                NewFacetFactory<RemoveEventHandlerMethodsFacetFactory>(),
                NewFacetFactory<TypeMarkerFacetFactory>(),
                NewFacetFactory<MandatoryDefaultFacetFactory>(),
                NewFacetFactory<PropertyValidateDefaultFacetFactory>(),
                NewFacetFactory<ComplementaryMethodsFilteringFacetFactory>(),
                NewFacetFactory<ActionMethodsFacetFactory>(),
                NewFacetFactory<CollectionFieldMethodsFacetFactory>(),
                NewFacetFactory<PropertyMethodsFacetFactory>(),
                NewFacetFactory<IconMethodFacetFactory>(),
                NewFacetFactory<CallbackMethodsFacetFactory>(),
                NewFacetFactory<TitleMethodFacetFactory>(),
                NewFacetFactory<ValidateObjectFacetFactory>(),
                NewFacetFactory<ComplexTypeAnnotationFacetFactory>(),
                NewFacetFactory<ViewModelFacetFactory>(),
                NewFacetFactory<BoundedAnnotationFacetFactory>(),
                NewFacetFactory<EnumFacetFactory>(),
                NewFacetFactory<ActionDefaultAnnotationFacetFactory>(),
                NewFacetFactory<PropertyDefaultAnnotationFacetFactory>(),
                NewFacetFactory<DescribedAsAnnotationFacetFactory>(),
                NewFacetFactory<DisabledAnnotationFacetFactory>(),
                NewFacetFactory<PasswordAnnotationFacetFactory>(),
                NewFacetFactory<ExecutedAnnotationFacetFactory>(),
                NewFacetFactory<PotencyAnnotationFacetFactory>(),
                NewFacetFactory<PageSizeAnnotationFacetFactory>(),
                NewFacetFactory<HiddenAnnotationFacetFactory>(),
                NewFacetFactory<HiddenDefaultMethodFacetFactory>(),
                NewFacetFactory<DisableDefaultMethodFacetFactory>(),
                NewFacetFactory<AuthorizeAnnotationFacetFactory>(),
                NewFacetFactory<ValidateProgrammaticUpdatesAnnotationFacetFactory>(),
                NewFacetFactory<ImmutableAnnotationFacetFactory>(),
                NewFacetFactory<MaxLengthAnnotationFacetFactory>(),
                NewFacetFactory<RangeAnnotationFacetFactory>(),
                NewFacetFactory<MemberOrderAnnotationFacetFactory>(),
                NewFacetFactory<MultiLineAnnotationFacetFactory>(),
                NewFacetFactory<NamedAnnotationFacetFactory>(),
                NewFacetFactory<NotPersistedAnnotationFacetFactory>(),
                NewFacetFactory<ProgramPersistableOnlyAnnotationFacetFactory>(),
                NewFacetFactory<OptionalAnnotationFacetFactory>(),
                NewFacetFactory<RequiredAnnotationFacetFactory>(),
                NewFacetFactory<PluralAnnotationFacetFactory>(),
                NewFacetFactory<DefaultNamingFacetFactory>(),
                NewFacetFactory<ConcurrencyCheckAnnotationFacetFactory>(),
                NewFacetFactory<ContributedActionAnnotationFacetFactory>(),
                NewFacetFactory<FinderActionFacetFactory>(),
                NewFacetFactory<MaskAnnotationFacetFactory>(),
                NewFacetFactory<RegExAnnotationFacetFactory>(),
                NewFacetFactory<TypeOfAnnotationFacetFactory>(),
                NewFacetFactory<TableViewAnnotationFacetFactory>(),
                NewFacetFactory<TypicalLengthDerivedFromTypeFacetFactory>(),
                NewFacetFactory<TypicalLengthAnnotationFacetFactory>(),
                NewFacetFactory<EagerlyAnnotationFacetFactory>(),
                NewFacetFactory<PresentationHintAnnotationFacetFactory>(),
                NewFacetFactory<BooleanValueTypeFacetFactory>(),
                NewFacetFactory<ByteValueTypeFacetFactory>(),
                NewFacetFactory<SbyteValueTypeFacetFactory>(),
                NewFacetFactory<ShortValueTypeFacetFactory>(),
                NewFacetFactory<IntValueTypeFacetFactory>(),
                NewFacetFactory<LongValueTypeFacetFactory>(),
                NewFacetFactory<UShortValueTypeFacetFactory>(),
                NewFacetFactory<UIntValueTypeFacetFactory>(),
                NewFacetFactory<ULongValueTypeFacetFactory>(),
                NewFacetFactory<FloatValueTypeFacetFactory>(),
                NewFacetFactory<DoubleValueTypeFacetFactory>(),
                NewFacetFactory<DecimalValueTypeFacetFactory>(),
                NewFacetFactory<CharValueTypeFacetFactory>(),
                NewFacetFactory<DateTimeValueTypeFacetFactory>(),
                NewFacetFactory<TimeValueTypeFacetFactory>(),
                NewFacetFactory<StringValueTypeFacetFactory>(),
                NewFacetFactory<GuidValueTypeFacetFactory>(),
                NewFacetFactory<EnumValueTypeFacetFactory>(),
                NewFacetFactory<FileAttachmentValueTypeFacetFactory>(),
                NewFacetFactory<ImageValueTypeFacetFactory>(),
                NewFacetFactory<TypeFacetFactory.ArrayValueTypeFacetFactory<byte>>(),
                NewFacetFactory<CollectionFacetFactory>()
            };

        private IFacetFactory NewFacetFactory<T>() where T : IFacetFactory
        {
            return (T)Activator.CreateInstance(typeof(T), new TestFacetFactoryOrder<T>(ObjectFacetFactories.StandardFacetFactories()), MockLoggerFactory);
        }

        protected static void AssertIsInstanceOfType<T>(object o)
        {
            Assert.IsInstanceOfType(o, typeof(T));
        }

        protected virtual IReflector Reflector(Metamodel metamodel, ILoggerFactory lf) {
            var config = new ObjectReflectorConfiguration(new[] {typeof(TestPoco), typeof(TestDomainObject), typeof(ArrayList)}, Array.Empty<Type>());
            var objectFactFactorySet = new ObjectFacetFactorySet(FacetFactories.OfType<IObjectFacetFactoryProcessor>().ToArray());

            ClassStrategy = new ObjectClassStrategy(config);
            var mockLogger1 = new Mock<ILogger<AbstractParallelReflector>>().Object;
            return new ObjectReflector(objectFactFactorySet, (ObjectClassStrategy) ClassStrategy, metamodel, config, Array.Empty<IFacetDecorator>(), lf, mockLogger1);
        }

        [TestInitialize]
        public virtual void SetUp() {
            var cache = new ImmutableInMemorySpecCache();
            ObjectReflectorConfiguration.NoValidate = true;

            var mockLogger = new Mock<ILogger<Metamodel>>().Object;

            var metamodel = new Metamodel(cache, mockLogger);
            var mockLoggerFactory = new Mock<ILoggerFactory>().Object;

            var reflector = Reflector(metamodel, mockLoggerFactory);

            ITypeSpecBuilder spec;
            (spec, Metamodel) = LoadSpecification(reflector);
            Specification = spec as IObjectSpecImmutable;
        }

        protected abstract (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(IReflector reflector);

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

        private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs)
        {
            return specs.SingleOrDefault(s => s.FullName == type.FullName);
        }

        public static void AssertSpec(Type type, ITypeSpecBuilder[] specs)
        {
            AssertSpec(type, GetSpec(type, specs));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}