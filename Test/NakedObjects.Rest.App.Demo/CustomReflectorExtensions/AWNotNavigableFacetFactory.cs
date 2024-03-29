﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using AdventureWorksModel;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.DependencyInjection.Component;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedObjects.Reflector.FacetFactory;

namespace NakedObjects.Rest.App.Demo.AWCustom {
    public sealed class AWNotNavigableFacetFactoryParallel : DomainObjectFacetFactoryProcessor {
        public AWNotNavigableFacetFactoryParallel(AppendFacetFactoryOrder<AWNotNavigableFacetFactoryParallel> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Properties) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(AddressType))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(Culture))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(SalesReason))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(UnitMeasure))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ScrapReason))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ProductSubcategory))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ProductCategory))) {
                FacetUtils.AddFacet(NotNavigableFacet.Instance, specification);
            }

            return metamodel;
        }
    }
}