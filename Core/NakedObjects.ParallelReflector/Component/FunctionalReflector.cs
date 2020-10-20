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
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.ParallelReflect.Component {
    public sealed class FunctionalReflector : ParallelReflector {
        public FunctionalReflector(IMetamodelBuilder metamodel,
                                   IObjectReflectorConfiguration objectReflectorConfiguration,
                                   IFunctionalReflectorConfiguration functionalReflectorConfiguration,
                                   IEnumerable<IFacetDecorator> facetDecorators,
                                   IEnumerable<IFacetFactory> facetFactories,
                                   ILoggerFactory loggerFactory,
                                   ILogger<ParallelReflector> logger) : base(metamodel, objectReflectorConfiguration, functionalReflectorConfiguration, facetDecorators, facetFactories, loggerFactory, logger) { }


        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectFunctionalTypes(Type[] records, Type[] functions, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var allFunctionalTypes = records.Union(functions).ToArray();

            var placeholders = GetPlaceholders(allFunctionalTypes, FunctionalClassStrategy);
            return placeholders.Any()
                ? IntrospectPlaceholders(specDictionary.AddRange(placeholders), () => new FunctionalIntrospector(this, FunctionalFacetFactorySet, FunctionalClassStrategy, functions))
                : specDictionary;
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var records = reflectorConfiguration.RecordTypes;
            var functions = reflectorConfiguration.FunctionTypes;
            return IntrospectFunctionalTypes(records, functions, specDictionary);
            //specDictionary.ForEach(i => initialMetamodel.Add(i.Value.Type, i.Value));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}