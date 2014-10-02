// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Actions.PageSize;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Facets.Properties.Validate;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
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
        public FallbackFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.Everything) { }


        public bool Recognizes(MethodInfo method) {
            return false;
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacets(
                new IFacet[] {
                    new DescribedAsFacetNone(holder),  
                    new ImmutableFacetNever(holder),
                    new TitleFacetNone(holder),
                });
        }

        private static bool Process(IFacetHolder holder) {
            var facets = new List<IFacet>();

            if (holder is DotNetNakedObjectMemberPeer) {
                facets.Add(new NamedFacetNone(holder));
                facets.Add(new DescribedAsFacetNone(holder));                
            }

            if (holder is DotNetNakedObjectAssociationPeer) {
                facets.Add(new ImmutableFacetNever(holder));
                facets.Add(new PropertyDefaultFacetNone(holder));
                facets.Add(new PropertyValidateFacetNone(holder));
            }

            // TODO this should really only be applied to objects marked as ParseableEntryFacet or ones that are externally processed as such
            if (holder is IParseableFacet || holder is DotNetOneToOneAssociationPeer) {
                var association = (DotNetOneToOneAssociationPeer) holder;
                facets.Add(new MaxLengthFacetZero(holder));
                DefaultTypicalLength(facets, association.Specification, holder);
                facets.Add(new MultiLineFacetNone(holder));
            }

            if (holder is DotNetNakedObjectActionPeer) {
                facets.Add(new ExecutedFacetAtDefault(holder));
                facets.Add(new ActionDefaultsFacetNone(holder));
                facets.Add(new ActionChoicesFacetNone(holder));
                facets.Add(new PageSizeFacetDefault(holder));
            }

            return FacetUtils.AddFacets(facets);
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(holder);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            var facets = new List<IFacet>();

            if (holder is DotNetNakedObjectActionParamPeer) {
                var param = (DotNetNakedObjectActionParamPeer) holder;

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

        private static void DefaultTypicalLength(ICollection<IFacet> facets, IFacetHolder specification, IFacetHolder holder) {
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