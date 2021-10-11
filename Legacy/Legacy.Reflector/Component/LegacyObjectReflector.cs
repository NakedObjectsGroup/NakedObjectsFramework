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
using Legacy.Reflector.Configuration;
using Legacy.Reflector.Reflect;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.ParallelReflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.Reflect;

namespace Legacy.Reflector.Component {
    public sealed class LegacyObjectReflector : AbstractParallelReflector {
        private readonly ILegacyObjectReflectorConfiguration legacyObjectReflectorConfiguration;

        public LegacyObjectReflector(LegacyObjectFacetFactorySet legacyObjectFacetFactorySet,
                               LegacyObjectClassStrategy legacyObjectClassStrategy,
                               ILegacyObjectReflectorConfiguration legacyObjectReflectorConfiguration,
                               IEnumerable<IFacetDecorator> facetDecorators,
                               IReflectorOrder<LegacyObjectReflector> reflectorOrder,
                               ILoggerFactory loggerFactory,
                               ILogger<AbstractParallelReflector> logger) : base(facetDecorators, reflectorOrder,  loggerFactory, logger) {
            this.legacyObjectReflectorConfiguration = legacyObjectReflectorConfiguration;
            FacetFactorySet = legacyObjectFacetFactorySet;
            ClassStrategy = legacyObjectClassStrategy;
        }

        public override bool ConcurrencyChecking => legacyObjectReflectorConfiguration.ConcurrencyChecking;
        public override string Name => "Naked Objects";
        public override ReflectorType ReflectorType => ReflectorType.Object;
        public override bool IgnoreCase => legacyObjectReflectorConfiguration.IgnoreCase;

        protected override IIntrospector GetNewIntrospector() => new LegacyObjectIntrospector(this, LoggerFactory.CreateLogger<LegacyObjectIntrospector>());

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectObjectTypes(Type[] ooTypes, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var placeholders = GetPlaceholders(ooTypes);
            var pending = specDictionary.Where(i => i.Value.IsPendingIntrospection).Select(i => i.Value.Type);
            var toIntrospect = placeholders.Select(kvp => kvp.Value.Type).Union(pending).ToArray();
            return placeholders.Any()
                ? IntrospectTypes(toIntrospect, specDictionary.AddRange(placeholders))
                : specDictionary;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var ooTypes = legacyObjectReflectorConfiguration.ObjectTypes;
            specDictionary = IntrospectObjectTypes(ooTypes, specDictionary);
            return specDictionary;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}