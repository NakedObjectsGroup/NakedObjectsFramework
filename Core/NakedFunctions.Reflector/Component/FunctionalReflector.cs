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
using NakedFunctions.Reflector.Reflect;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.ParallelReflect;
using NakedObjects.ParallelReflect.Component;

namespace NakedFunctions.Reflector.Component {
    public sealed class FunctionalReflector : ParallelReflector {
        private readonly IFunctionalReflectorConfiguration functionalReflectorConfiguration;

        public FunctionalReflector(IMetamodelBuilder metamodel,
                                   IFunctionalReflectorConfiguration functionalReflectorConfiguration,
                                   IEnumerable<IFacetDecorator> facetDecorators,
                                   IEnumerable<IFacetFactory> facetFactories,
                                   ILoggerFactory loggerFactory,
                                   ILogger<ParallelReflector> logger) : base(metamodel, facetDecorators, loggerFactory, logger) {
            this.functionalReflectorConfiguration = functionalReflectorConfiguration;
            ClassStrategy = new FunctionClassStrategy(functionalReflectorConfiguration);
            FacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.Functional)).ToArray());
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectFunctionalTypes(Type[] records, Type[] functions, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var allFunctionalTypes = records.Union(functions).ToArray();

            var placeholders = GetPlaceholders(allFunctionalTypes, ClassStrategy);
            return placeholders.Any()
                ? IntrospectPlaceholders(specDictionary.AddRange(placeholders), () => new FunctionalIntrospector(this, functions, LoggerFactory.CreateLogger<FunctionalIntrospector>()))
                : specDictionary;
        }

        public override bool IgnoreCase => functionalReflectorConfiguration.IgnoreCase;
        public override bool ConcurrencyChecking => functionalReflectorConfiguration.ConcurrencyChecking;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var records = functionalReflectorConfiguration.Types;
            var functions = functionalReflectorConfiguration.Functions;
            return IntrospectFunctionalTypes(records, functions, specDictionary);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}