// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.Component {
    public abstract class ParallelReflector : IReflector {
        protected readonly FacetDecoratorSet facetDecoratorSet;
        protected readonly IMetamodelBuilder initialMetamodel;
        protected readonly ILogger<ParallelReflector> logger;
        protected readonly ILoggerFactory loggerFactory;
        protected readonly CompositeReflectorConfiguration reflectorConfiguration;

        protected ParallelReflector(IMetamodelBuilder metamodel,
                                    IObjectReflectorConfiguration objectReflectorConfiguration,
                                    IFunctionalReflectorConfiguration functionalReflectorConfiguration,
                                    IEnumerable<IFacetDecorator> facetDecorators,
                                    IEnumerable<IFacetFactory> facetFactories,
                                    ILoggerFactory loggerFactory,
                                    ILogger<ParallelReflector> logger) {
            
            FunctionalClassStrategy = new FunctionClassStrategy(functionalReflectorConfiguration);
            initialMetamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            reflectorConfiguration = new CompositeReflectorConfiguration(objectReflectorConfiguration, functionalReflectorConfiguration);
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
           
            FunctionalFacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.Functional)).ToArray());
        }

        public FacetFactorySet FunctionalFacetFactorySet { get; }

        public (ITypeSpecBuilder typeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) IntrospectSpecification(Type actualType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            if (actualType == null) {
                throw new ReflectionException("cannot introspect null");
            }

            var typeKey = TypeKeyUtils.GetKeyForType(actualType);

            return string.IsNullOrEmpty(metamodel[typeKey].FullName)
                ? LoadSpecificationAndCache(actualType, metamodel, getIntrospector)
                : (metamodel[typeKey], metamodel);
        }


        protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var ph = metamodel.Where(i => string.IsNullOrEmpty(i.Value.FullName)).Select(i => i.Value.Type);
            var mm = ph.AsParallel().SelectMany(type => IntrospectSpecification(type, metamodel, getIntrospector).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary();

            return mm.Any(i => string.IsNullOrEmpty(i.Value.FullName))
                ? IntrospectPlaceholders(mm, getIntrospector)
                : mm;
        }


        private ITypeSpecBuilder GetPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            return specification;
        }

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = metamodel.Add(TypeKeyUtils.GetKeyForType(type), specification);

            return (specification, metamodel);
        }

        protected IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types, IClassStrategy classStrategy) =>
            types.Select(t => classStrategy.GetType(t))
                 .Where(t => t != null)
                 .Distinct(new TypeKeyComparer())
                 .ToDictionary(t => TypeKeyUtils.GetKeyForType(t), t => GetPlaceholder(t, null)).ToImmutableDictionary();

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var specification = metamodel[TypeKeyUtils.GetKeyForType(type)];

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = specification.Introspect(facetDecoratorSet, getIntrospector(), metamodel);

            return (specification, metamodel);
        }

        private ITypeSpecBuilder CreateSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached

            return IsService(type) ? (ITypeSpecBuilder) ImmutableSpecFactory.CreateServiceSpecImmutable(type, metamodel) : ImmutableSpecFactory.CreateObjectSpecImmutable(type, metamodel);
        }

        private bool IsService(Type type) => reflectorConfiguration.ServiceTypeSet.Contains(type);

        #region Nested type: TypeKeyComparer

        private class TypeKeyComparer : IEqualityComparer<Type> {
            #region IEqualityComparer<Type> Members

            public bool Equals(Type x, Type y) => TypeKeyUtils.GetKeyForType(x).Equals(TypeKeyUtils.GetKeyForType(y), StringComparison.Ordinal);

            public int GetHashCode(Type obj) => TypeKeyUtils.GetKeyForType(obj).GetHashCode();

            #endregion
        }

        #endregion

        #region Nested type: TypeSpecKeyComparer

        private class TypeSpecKeyComparer : IEqualityComparer<KeyValuePair<string, ITypeSpecBuilder>> {
            #region IEqualityComparer<KeyValuePair<string,ITypeSpecBuilder>> Members

            public bool Equals(KeyValuePair<string, ITypeSpecBuilder> x, KeyValuePair<string, ITypeSpecBuilder> y) => x.Key.Equals(y.Key, StringComparison.Ordinal);

            public int GetHashCode(KeyValuePair<string, ITypeSpecBuilder> obj) => obj.Key.GetHashCode();

            #endregion
        }

        #endregion

        #region IReflector Members

        public bool ConcurrencyChecking => reflectorConfiguration.ConcurrencyChecking;

        public bool IgnoreCase => reflectorConfiguration.IgnoreCase;

       // public IClassStrategy ObjectClassStrategy { get; }

        public IClassStrategy FunctionalClassStrategy { get; }

        public IMetamodel Metamodel => null;

        public ITypeSpecBuilder LoadSpecification(Type type) => throw new NotImplementedException();

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable => throw new NotImplementedException();

        public ITypeSpecBuilder[] AllObjectSpecImmutables => initialMetamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IClassStrategy classStrategy, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (type == null) {
                throw new NakedObjectSystemException("cannot load specification for null");
            }

            var actualType = classStrategy.GetType(type) ?? type;
            var typeKey = TypeKeyUtils.GetKeyForType(actualType);
            return !metamodel.ContainsKey(typeKey) ? LoadPlaceholder(actualType, metamodel) : (metamodel[typeKey], metamodel);
        }

        public (T, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification<T>(Type type, IClassStrategy classStrategy, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) where T : class, ITypeSpecImmutable {
            ITypeSpecBuilder spec;
            (spec, metamodel) = LoadSpecification(type, classStrategy, metamodel);
            return (spec as T, metamodel);
        }


        public abstract IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}