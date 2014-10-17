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
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Actions.PageSize;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Facets.Properties.Validate;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets.Naming.Named;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;
using NakedObjects.Reflector.DotNet.Reflect.Properties;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Central point for providing some kind of default for any  <see cref="IFacet" />s required by the Naked Objects Framework itself.
    /// </summary>
    public class FallbackFacetFactory : FacetFactoryAbstract {
        public FallbackFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.Everything) {}


        public bool Recognizes(MethodInfo method) {
            return false;
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
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

            // TODO this should really only be applied to objects marked as ParseableEntryFacet or ones that are externally processed as such
            if (holder is IParseableFacet || holder is OneToOneAssociationSpecImmutable) {
                var association = (OneToOneAssociationSpecImmutable) holder;
                facets.Add(new MaxLengthFacetZero(holder));
                DefaultTypicalLength(facets, association.Specification, holder);
                facets.Add(new MultiLineFacetNone(holder));
            }

            if (holder is ActionSpecImmutable) {
                facets.Add(new ExecutedFacetAtDefault(holder));
                facets.Add(new ActionDefaultsFacetNone(holder));
                facets.Add(new ActionChoicesFacetNone(holder));
                facets.Add(new PageSizeFacetDefault(holder));
            }

            return FacetUtils.AddFacets(facets);
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return Process(specification);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            var facets = new List<IFacet>();

            if (holder is ActionParameterSpecImmutable) {
                var param = (ActionParameterSpecImmutable) holder;

                INamedFacet namedFacet;
                string name = method.GetParameters()[paramNum].Name;

                if (name == null) {
                    namedFacet = new NamedFacetNone(holder);
                }
                else {
                    namedFacet = new NamedFacetInferred(NameUtils.NaturalName(name), holder);
                }

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