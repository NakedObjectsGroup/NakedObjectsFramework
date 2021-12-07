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
using NakedFramework.DependencyInjection.Component;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Component;
using NakedFramework.ParallelReflector;
using NakedFramework.ParallelReflector.Component;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.TypeFacetFactory;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.Test.Reflect;

public abstract class AbstractReflectorTest {
    private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
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
            NewFacetFactory<EagerlyAnnotationFacetFactory>(),
            NewFacetFactory<PresentationHintAnnotationFacetFactory>(),
            NewFacetFactory<ValueTypeFacetFactory>(),
           
            NewFacetFactory<EnumValueTypeFacetFactory>(),
            NewFacetFactory<ArrayValueTypeFacetFactory<byte>>(),
            NewFacetFactory<CollectionFacetFactory>()
        };

    private IFacetFactory NewFacetFactory<T>() where T : IFacetFactory => (T)Activator.CreateInstance(typeof(T), new TestFacetFactoryOrder<T>(ObjectFacetFactories.StandardFacetFactories()), LoggerFactory);

    protected static void AssertIsInstanceOfType<T>(object o) {
        Assert.IsInstanceOfType(o, typeof(T));
    }

    protected virtual IReflector Reflector(MetamodelHolder metamodel, ILoggerFactory lf) {
        var config = new ObjectReflectorConfiguration(new[] { typeof(TestPoco), typeof(TestDomainObject), typeof(ArrayList) }, Array.Empty<Type>());
        var objectFactFactorySet = new ObjectFacetFactorySet(FacetFactories.OfType<IObjectFacetFactoryProcessor>().ToArray());

        ClassStrategy = new ObjectClassStrategy(config);
        var mockLogger1 = new Mock<ILogger<AbstractParallelReflector>>().Object;
        var order = new ObjectReflectorOrder<ObjectReflector>();
        return new ObjectReflector(objectFactFactorySet, (ObjectClassStrategy)ClassStrategy, config, Array.Empty<IFacetDecorator>(), order, lf, mockLogger1);
    }

    [TestInitialize]
    public virtual void SetUp() {
        var cache = new ImmutableInMemorySpecCache();
        ObjectReflectorConfiguration.NoValidate = true;

        var metamodel = new MetamodelHolder(cache, LoggerFactory.CreateLogger<MetamodelHolder>());

        var reflector = Reflector(metamodel, LoggerFactory);

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

    private static ITypeSpecBuilder GetSpec(Type type, ITypeSpecBuilder[] specs) {
        return specs.SingleOrDefault(s => s.FullName == type.FullName);
    }

    public static void AssertSpec(Type type, ITypeSpecBuilder[] specs) {
        AssertSpec(type, GetSpec(type, specs));
    }
}

// Copyright (c) Naked Objects Group Ltd.