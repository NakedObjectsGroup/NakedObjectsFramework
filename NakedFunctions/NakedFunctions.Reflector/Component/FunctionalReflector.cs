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
using NakedFramework.Architecture.Configuration;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.ParallelReflector.Component;
using NakedFunctions.Reflector.Reflect;

namespace NakedFunctions.Reflector.Component; 

public sealed class FunctionalReflector : AbstractParallelReflector {
    private readonly IFunctionalReflectorConfiguration functionalReflectorConfiguration;

    public FunctionalReflector(FunctionalFacetFactorySet functionalFacetFactorySet,
                               FunctionClassStrategy functionClassStrategy,
                               IFunctionalReflectorConfiguration functionalReflectorConfiguration,
                               IEnumerable<IFacetDecorator> facetDecorators,
                               IReflectorOrder<FunctionalReflector> reflectorOrder,
                               ILoggerFactory loggerFactory,
                               ILogger<AbstractParallelReflector> logger) : base(facetDecorators, reflectorOrder,  loggerFactory, logger) {
        this.functionalReflectorConfiguration = functionalReflectorConfiguration;
        ClassStrategy = functionClassStrategy;
        FacetFactorySet = functionalFacetFactorySet;
    }

    public override bool IgnoreCase => functionalReflectorConfiguration.IgnoreCase;

    public override bool ConcurrencyChecking => functionalReflectorConfiguration.ConcurrencyChecking;
    public override string Name => "Naked Functions";
    public override ReflectorType ReflectorType => ReflectorType.Functional;

    private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectFunctionalTypes(Type[] records, Type[] functions, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var allFunctionalTypes = records.Union(functions).ToArray();

        var placeholders = GetPlaceholders(allFunctionalTypes);
        var pending = specDictionary.Where(i => i.Value.IsPendingIntrospection).Select(i => i.Value.Type);
        var toIntrospect = placeholders.Select(kvp => kvp.Value.Type).Union(pending).ToArray();
        return placeholders.Any()
            ? IntrospectTypes(toIntrospect, specDictionary.AddRange(placeholders))
            : specDictionary;
    }

    protected override IIntrospector GetNewIntrospector() => new FunctionalIntrospector(this, LoggerFactory.CreateLogger<FunctionalIntrospector>());

    public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var records = functionalReflectorConfiguration.Types;
        var functions = functionalReflectorConfiguration.Functions;
        specDictionary = IntrospectFunctionalTypes(records, functions, specDictionary);
        return specDictionary;
    }
}

// Copyright (c) Naked Objects Group Ltd.