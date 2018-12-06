// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using AdventureWorksModel;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace AWCustom {
    public sealed class AWNotNavigableFacetFactory : FacetFactoryAbstract {
        public AWNotNavigableFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Properties) { }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(AddressType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(Culture))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(SalesReason))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(UnitMeasure))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(ScrapReason))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(ProductSubcategory))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
            if (property.PropertyType.IsAssignableFrom(typeof(ProductCategory))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }
        }


    }

    public sealed class AWNotNavigableFacetFactoryParallel : NakedObjects.ParallelReflect.FacetFactory.FacetFactoryAbstract {
        public AWNotNavigableFacetFactoryParallel(int numericOrder)
            : base(numericOrder, FeatureType.Properties) { }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {

            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(AddressType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ContactType))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(Culture))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(SalesReason))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(UnitMeasure))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ScrapReason))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ProductSubcategory))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            if (property.PropertyType.IsAssignableFrom(typeof(ProductCategory))) {
                FacetUtils.AddFacet(new NotNavigableFacet(specification));
            }

            return metamodel;
        }
    }
}