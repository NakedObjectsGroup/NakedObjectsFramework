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
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.ParallelReflector.Reflect;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflector.Component {
    public abstract class AbstractParallelReflector : IReflector {
        protected readonly FacetDecoratorSet FacetDecoratorSet;
        protected readonly IMetamodelBuilder InitialMetamodel;
        protected readonly ILogger<AbstractParallelReflector> Logger;
        protected readonly ILoggerFactory LoggerFactory;

        protected AbstractParallelReflector(IMetamodelBuilder metamodel,
                                    IEnumerable<IFacetDecorator> facetDecorators,
                                    ILoggerFactory loggerFactory,
                                    ILogger<AbstractParallelReflector> logger) {
            InitialMetamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            LoggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            Logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            FacetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
        }

        protected abstract IIntrospector GetNewIntrospector();

        public IClassStrategy ClassStrategy { get; init; }
        public IFacetFactorySet FacetFactorySet { get; init; }

        public (ITypeSpecBuilder typeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) IntrospectSpecification(Type actualType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (actualType == null) {
                throw new ReflectionException("cannot introspect null");
            }

            var typeKey = TypeKeyUtils.GetKeyForType(actualType);

            return string.IsNullOrEmpty(metamodel[typeKey].FullName)
                ? LoadSpecificationAndCache(actualType, metamodel)
                : (metamodel[typeKey], metamodel);
        }


        protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectTypes(Type[] toIntrospect,  IImmutableDictionary<string, ITypeSpecBuilder> specDictionary)
        {
            specDictionary = toIntrospect.Any()
                ? toIntrospect.AsParallel().SelectMany(type => IntrospectSpecification(type, specDictionary).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary()
                : specDictionary;

            // todo is this now necessary ?
            return specDictionary.Any(i => i.Value.IsPlaceHolder)
                ? IntrospectPlaceholders(specDictionary)
                : specDictionary;
        }


        protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var ph = specDictionary.Where(i => i.Value.IsPlaceHolder).Select(i => i.Value.Type).ToArray();
            return IntrospectTypes(ph, specDictionary);
        }


        private ITypeSpecBuilder GetPlaceholder(Type type) {
            var specification = CreateSpecification(type);

            if (specification == null) {
                throw new ReflectionException(Logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            return specification;
        }

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type);

            if (specification == null) {
                throw new ReflectionException(Logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = metamodel.Add(TypeKeyUtils.GetKeyForType(type), specification);

            return (specification, metamodel);
        }

        protected IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types, IClassStrategy classStrategy) =>
            types.Select(TypeKeyUtils.FilterNullableAndProxies)
                 .Distinct(new TypeKeyComparer())
                 .ToDictionary(TypeKeyUtils.GetKeyForType, GetPlaceholder).ToImmutableDictionary();

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = metamodel[TypeKeyUtils.GetKeyForType(type)];

            if (specification == null) {
                throw new ReflectionException(Logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = specification.Introspect(FacetDecoratorSet, GetNewIntrospector(), metamodel);

            return (specification, metamodel);
        }

        private ITypeSpecBuilder CreateSpecification(Type type) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached
            return ImmutableSpecFactory.CreateTypeSpecImmutable(type, ClassStrategy.IsService(type), ClassStrategy.IsTypeRecognized(type));
        }

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

        public abstract bool ConcurrencyChecking { get; }
        public abstract bool IgnoreCase { get; }

        public virtual (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (type == null) {
                throw new NakedObjectSystemException("cannot load specification for null");
            }

            var actualType = TypeKeyUtils.FilterNullableAndProxies(type);
            var typeKey = TypeKeyUtils.GetKeyForType(actualType);
            return metamodel.ContainsKey(typeKey) ? (metamodel[typeKey], metamodel) : LoadPlaceholder(actualType, metamodel);
        }

        public virtual (T, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification<T>(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) where T : class, ITypeSpecImmutable {
            ITypeSpecBuilder spec;
            (spec, metamodel) = LoadSpecification(type, metamodel);
            return (spec as T, metamodel);
        }


        public abstract IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}