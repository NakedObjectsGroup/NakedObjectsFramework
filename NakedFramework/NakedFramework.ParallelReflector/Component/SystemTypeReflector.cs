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
using NakedFramework.ParallelReflector.Reflect;

namespace NakedFramework.ParallelReflector.Component {
    public sealed class SystemTypeReflector : AbstractParallelReflector {
        public SystemTypeReflector(SystemTypeFacetFactorySet systemTypeFacetFactorySet,
                                   SystemTypeClassStrategy systemTypeClassStrategy,
                                   ICoreConfiguration coreConfiguration,
                                   IEnumerable<IFacetDecorator> facetDecorators,
                                   IReflectorOrder<SystemTypeReflector> reflectorOrder,
                                   ILoggerFactory loggerFactory,
                                   ILogger<AbstractParallelReflector> logger) : base(facetDecorators, reflectorOrder, loggerFactory, logger) {
            CoreConfiguration = coreConfiguration;
            FacetFactorySet = systemTypeFacetFactorySet;
            ClassStrategy = systemTypeClassStrategy;
        }

        private ICoreConfiguration CoreConfiguration { get; }

        public override bool ConcurrencyChecking => false;
        public override string Name => "Naked Framework";
        public override ReflectorType ReflectorType => ReflectorType.System;
        public override bool IgnoreCase => false;


        protected override IIntrospector GetNewIntrospector() => new SystemTypeIntrospector(this, LoggerFactory.CreateLogger<SystemTypeIntrospector>());

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectSystemTypes(Type[] systemTypes, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var placeholders = GetPlaceholders(systemTypes);
            var pending = specDictionary.Where(i => i.Value.IsPendingIntrospection).Select(i => i.Value.Type);
            var toIntrospect = placeholders.Select(kvp => kvp.Value.Type).Union(pending).ToArray();
            return placeholders.Any()
                ? IntrospectTypes(toIntrospect, specDictionary.AddRange(placeholders))
                : specDictionary;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Reflect(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var systemTypes = CoreConfiguration.SupportedSystemTypes.ToArray();
            specDictionary = IntrospectSystemTypes(systemTypes, specDictionary);
            return specDictionary;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}