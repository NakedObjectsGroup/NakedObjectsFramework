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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.ParallelReflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.Component {
    public sealed class ObjectReflector : AbstractParallelReflector {
        private readonly IObjectReflectorConfiguration objectReflectorConfiguration;

        public ObjectReflector(ObjectFacetFactorySet objectFacetFactorySet,
                               ObjectClassStrategy objectClassStrategy,
                               IMetamodelBuilder metamodel,
                               IObjectReflectorConfiguration objectReflectorConfiguration,
                               IEnumerable<IFacetDecorator> facetDecorators,
                               ILoggerFactory loggerFactory,
                               ILogger<AbstractParallelReflector> logger) : base(metamodel, facetDecorators, loggerFactory, logger) {
            this.objectReflectorConfiguration = objectReflectorConfiguration;
            FacetFactorySet = objectFacetFactorySet;
            ClassStrategy = objectClassStrategy;
            Order = 1;
        }

        public override bool ConcurrencyChecking => objectReflectorConfiguration.ConcurrencyChecking;
        public override string Name { get; } = "Naked Objects";
        public override bool IgnoreCase => objectReflectorConfiguration.IgnoreCase;

        protected override IIntrospector GetNewIntrospector() => new ObjectIntrospector(this, LoggerFactory.CreateLogger<ObjectIntrospector>());

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectObjectTypes(Type[] ooTypes, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var placeholders = GetPlaceholders(ooTypes, ClassStrategy);
            var pending = specDictionary.Where(i => i.Value.IsPendingIntrospection).Select(i => i.Value.Type);
            var toIntrospect = placeholders.Select(kvp => kvp.Value.Type).Union(pending).ToArray();
            return placeholders.Any()
                ? IntrospectTypes(toIntrospect, specDictionary.AddRange(placeholders))
                : specDictionary;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var ooTypes = objectReflectorConfiguration.ObjectTypes;
            specDictionary = IntrospectObjectTypes(ooTypes, specDictionary);
            return specDictionary;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}