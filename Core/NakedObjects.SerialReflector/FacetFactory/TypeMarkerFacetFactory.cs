// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class TypeMarkerFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypeMarkerFacetFactory(IFacetFactoryOrder<TypeMarkerFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var facets = new List<IFacet> {
                new TypeIsAbstractFacet(specification, IsAbstract(type)),
                new TypeIsInterfaceFacet(specification, IsInterface(type)),
                new TypeIsSealedFacet(specification, IsSealed(type)),
                new TypeIsVoidFacet(specification, IsVoid(type))
            };

            FacetUtils.AddFacets(facets);
        }

        private static bool IsVoid(Type type) => type == typeof(void);

        private static bool IsSealed(Type type) => type.IsSealed;

        private static bool IsInterface(Type type) => type.IsInterface;

        private static bool IsAbstract(Type type) => type.IsAbstract;
    }
}