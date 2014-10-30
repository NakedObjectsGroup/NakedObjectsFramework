// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;
using NakedObjects.Meta.SpecImmutable;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Central point for providing some kind of default for any  <see cref="IFacet" />s required by the Naked Objects Framework itself.
    /// </summary>
    public class FallbackFacetFactory : FacetFactoryAbstract {
        public FallbackFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.Everything) {}


        public bool Recognizes(MethodInfo method) {
            return false;
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return FacetUtils.AddFacets(
                new IFacet[] {
                    new DescribedAsFacetNone(specification),
                    new ImmutableFacetNever(specification),
                    new TitleFacetNone(specification),
                });
        }

        private static bool Process(ISpecification holder) {
            var facets = new List<IFacet>();

            if (holder is MemberSpecImmutable) {
                facets.Add(new NamedFacetNone(holder));
                facets.Add(new DescribedAsFacetNone(holder));
            }

            if (holder is AssociationSpecImmutable) {
                facets.Add(new ImmutableFacetNever(holder));
                facets.Add(new PropertyDefaultFacetNone(holder));
                facets.Add(new PropertyValidateFacetNone(holder));
            }

            if (holder is OneToOneAssociationSpecImmutable) {
                var association = (OneToOneAssociationSpecImmutable) holder;
                facets.Add(new MaxLengthFacetZero(holder));
                DefaultTypicalLength(facets, association.Specification, holder);
                facets.Add(new MultiLineFacetNone(holder));
            }

            if (holder is ActionSpecImmutable) {
                facets.Add(new ExecutedFacetDefault(holder));
                facets.Add(new ActionDefaultsFacetNone(holder));
                facets.Add(new ActionChoicesFacetNone(holder));
                facets.Add(new PageSizeFacetDefault(holder));
            }

            return FacetUtils.AddFacets(facets);
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return Process(specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return Process(specification);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var facets = new List<IFacet>();

            if (holder is ActionParameterSpecImmutable) {
                var param = (ActionParameterSpecImmutable) holder;
                string name = method.GetParameters()[paramNum].Name;
                var namedFacet = name == null ? (INamedFacet) new NamedFacetNone(holder) : new NamedFacetInferred(NameUtils.NaturalName(name), holder);
                facets.Add(namedFacet);
                facets.Add(new DescribedAsFacetNone(holder));
                facets.Add(new MultiLineFacetNone(holder));
                facets.Add(new MaxLengthFacetZero(holder));
                facets.Add(new TypicalLengthFacetZero(holder));
                DefaultTypicalLength(facets, param.Specification, holder);
            }

            return FacetUtils.AddFacets(facets);
        }

        private static void DefaultTypicalLength(ICollection<IFacet> facets, ISpecification specification, ISpecification holder) {
            var typicalLengthFacet = specification.GetFacet<ITypicalLengthFacet>();
            if (typicalLengthFacet == null) {
                typicalLengthFacet = new TypicalLengthFacetZero(holder);
            }
            else {
                typicalLengthFacet = new TypicalLengthFacetInferred(typicalLengthFacet.Value, holder);
            }
            facets.Add(typicalLengthFacet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}