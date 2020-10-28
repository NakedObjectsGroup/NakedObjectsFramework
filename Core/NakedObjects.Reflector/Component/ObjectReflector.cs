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

namespace NakedObjects.ParallelReflect.Component {
    public sealed class ObjectReflector : ParallelReflector {
        private readonly IObjectReflectorConfiguration objectReflectorConfiguration;

        public ObjectReflector(IMetamodelBuilder metamodel,
                               IObjectReflectorConfiguration objectReflectorConfiguration,
                               IEnumerable<IFacetDecorator> facetDecorators,
                               IEnumerable<IFacetFactory> facetFactories,
                               ILoggerFactory loggerFactory,
                               ILogger<ParallelReflector> logger) : base(metamodel, facetDecorators, loggerFactory, logger) {
            this.objectReflectorConfiguration = objectReflectorConfiguration;
            FacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.ObjectOriented)).ToArray());
            ClassStrategy = new ObjectClassStrategy(objectReflectorConfiguration);
        }

        public override bool ConcurrencyChecking => objectReflectorConfiguration.ConcurrencyChecking;
        public override bool IgnoreCase => objectReflectorConfiguration.IgnoreCase;

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectObjectTypes(Type[] ooTypes) {
            var placeholders = GetPlaceholders(ooTypes, ClassStrategy);
            return placeholders.Any()
                ? IntrospectPlaceholders(placeholders, () => new ObjectIntrospector(this, LoggerFactory.CreateLogger<ObjectIntrospector>()))
                : placeholders;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var ooTypes = objectReflectorConfiguration.ObjectTypes;
            return IntrospectObjectTypes(ooTypes);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}