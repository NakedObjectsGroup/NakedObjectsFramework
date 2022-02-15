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
using NakedFramework.ParallelReflector.Component;
using NOF2.Reflector.Configuration;
using NOF2.Reflector.Reflect;

namespace NOF2.Reflector.Component;

public sealed class NOF2Reflector : AbstractParallelReflector {
    private readonly INOF2ReflectorConfiguration nof2ReflectorConfiguration;

    public NOF2Reflector(NOF2FacetFactorySet nof2FacetFactorySet,
                         NOF2ObjectClassStrategy nof2ObjectClassStrategy,
                         INOF2ReflectorConfiguration nof2ReflectorConfiguration,
                         IEnumerable<IFacetDecorator> facetDecorators,
                         IReflectorOrder<NOF2Reflector> reflectorOrder,
                         ILoggerFactory loggerFactory,
                         ILogger<AbstractParallelReflector> logger) : base(facetDecorators, reflectorOrder, loggerFactory, logger) {
        this.nof2ReflectorConfiguration = nof2ReflectorConfiguration;
        FacetFactorySet = nof2FacetFactorySet;
        ClassStrategy = nof2ObjectClassStrategy;
    }

    public override bool ConcurrencyChecking => nof2ReflectorConfiguration.ConcurrencyChecking;
    public override string Name => "NOF2";
    public override ReflectorType ReflectorType => ReflectorType.Object;
    public override bool IgnoreCase => nof2ReflectorConfiguration.IgnoreCase;

    protected override IIntrospector GetNewIntrospector() => new NOF2Introspector(this, LoggerFactory.CreateLogger<NOF2Introspector>());

    private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectObjectTypes(Type[] ooTypes, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var placeholders = GetPlaceholders(ooTypes);
        var pending = specDictionary.Where(i => i.Value.IsPendingIntrospection).Select(i => i.Value.Type);
        var toIntrospect = placeholders.Select(kvp => kvp.Value.Type).Union(pending).ToArray();
        return placeholders.Any()
            ? IntrospectTypes(toIntrospect, specDictionary.AddRange(placeholders))
            : specDictionary;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var ooTypes = nof2ReflectorConfiguration.ObjectTypes;
        specDictionary = IntrospectObjectTypes(ooTypes, specDictionary);
        return specDictionary;
    }
}

// Copyright (c) Naked Objects Group Ltd.