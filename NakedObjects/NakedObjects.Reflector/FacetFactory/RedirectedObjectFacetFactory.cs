// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedObjects.Redirect;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class RedirectedObjectFacetFactory : DomainObjectFacetFactoryProcessor {
    public RedirectedObjectFacetFactory(IFacetFactoryOrder<RedirectedObjectFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory, FeatureType.Objects) { }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (typeof(IRedirectedObject).IsAssignableFrom(type)) {
            var serverName = type.GetProperty(nameof(IRedirectedObject.ServerName));
            var oid = type.GetProperty(nameof(IRedirectedObject.Oid));
            FacetUtils.AddFacet(new RedirectedFacet(serverName, oid, Logger<RedirectedFacet>()), specification);
        }

        return metamodel;
    }
}