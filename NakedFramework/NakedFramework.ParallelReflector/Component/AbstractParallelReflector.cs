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
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.ParallelReflector.Reflect;

namespace NakedFramework.ParallelReflector.Component; 

public abstract class AbstractParallelReflector : IReflector {
    private readonly FacetDecoratorSet facetDecoratorSet;
    private readonly ILogger<AbstractParallelReflector> logger;
    protected readonly ILoggerFactory LoggerFactory;

    protected AbstractParallelReflector(IEnumerable<IFacetDecorator> facetDecorators,
                                        IReflectorOrder reflectorOrder,
                                        ILoggerFactory loggerFactory,
                                        ILogger<AbstractParallelReflector> logger) {
        LoggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
        this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
        facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
        Order = reflectorOrder.Order;
    }

    public IClassStrategy ClassStrategy { get; protected init; }
    public IFacetFactorySet FacetFactorySet { get; protected init; }

    protected abstract IIntrospector GetNewIntrospector();

    public (ITypeSpecBuilder typeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) IntrospectSpecification(Type actualType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (actualType == null) {
            throw new ReflectionException("cannot introspect null");
        }

        var typeKey = TypeKeyUtils.GetKeyForType(actualType);

        return string.IsNullOrEmpty(metamodel[typeKey].FullName)
            ? LoadSpecificationAndCache(actualType, metamodel)
            : (metamodel[typeKey], metamodel);
    }

    protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectTypes(Type[] toIntrospect, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var introspectedDictionary = toIntrospect.Any()
            ? toIntrospect.AsParallel().SelectMany(type => IntrospectSpecification(type, specDictionary).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary()
            : specDictionary;

        if (introspectedDictionary.Any(i => i.Value.IsPlaceHolder)) {
            var placeholders = introspectedDictionary.Where(i => i.Value.IsPlaceHolder).Select(p => p.Key).Aggregate("", (a, k) => $"{a}, {k}");
            throw new ReflectionException($"Unexpected placeholder(s): {placeholders}");
        }

        return introspectedDictionary;
    }

    private ITypeSpecBuilder GetPlaceholder(Type type) => CreateSpecification(type);

    private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var specification = CreateSpecification(type);
        metamodel = metamodel.Add(TypeKeyUtils.GetKeyForType(type), specification);
        return (specification, metamodel);
    }

    protected IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types) =>
        types.Select(TypeKeyUtils.FilterNullableAndProxies)
             .Distinct(new TypeKeyComparer())
             .ToDictionary(TypeKeyUtils.GetKeyForType, GetPlaceholder).ToImmutableDictionary();

    private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var specification = metamodel[TypeKeyUtils.GetKeyForType(type)] ?? ThrowNullSpecificationError(type);
        metamodel = specification.Introspect(facetDecoratorSet, GetNewIntrospector(), metamodel);
        return (specification, metamodel);
    }

    private ITypeSpecBuilder ThrowNullSpecificationError(Type type) {
        throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
    }

    private ITypeSpecBuilder CreateSpecification(Type type) {
        TypeUtils.GetType(type.FullName); // This should ensure type is cached
        var spec = ImmutableSpecFactory.CreateTypeSpecImmutable(type, ClassStrategy.IsService(type), ClassStrategy.IsTypeRecognized(type));
        return spec ?? ThrowNullSpecificationError(type);
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
    public abstract string Name { get; }
    public abstract ReflectorType ReflectorType { get; }
    public int Order { get; }
    public abstract bool IgnoreCase { get; }

    public virtual (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (type == null) {
            throw new NakedObjectSystemException("cannot load specification for null");
        }

        var actualType = TypeKeyUtils.FilterNullableAndProxies(type);
        var typeKey = TypeKeyUtils.GetKeyForType(actualType);
        return metamodel.ContainsKey(typeKey) ? (metamodel[typeKey], metamodel) : LoadPlaceholder(actualType, metamodel);
    }

    public virtual bool FindSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (type == null) {
            throw new NakedObjectSystemException("cannot find specification for null");
        }

        var actualType = TypeKeyUtils.FilterNullableAndProxies(type);
        var typeKey = TypeKeyUtils.GetKeyForType(actualType);
        return metamodel.ContainsKey(typeKey);
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