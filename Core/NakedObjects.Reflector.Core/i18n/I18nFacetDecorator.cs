// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.I18n {
    public class I18nFacetDecorator : IFacetDecorator {
        private readonly II18nManager i18nManager;
        private readonly bool staticFacets;

        public I18nFacetDecorator(II18nManager manager, bool staticFacets) {
            i18nManager = manager;
            this.staticFacets = staticFacets;
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, IFacetHolder holder) {
            IIdentifier identifier = holder.Identifier;
            Type facetType = facet.FacetType;

            if (facetType == typeof (INamedFacet)) {
                return GetNamedFacet(holder, facet as INamedFacet, identifier);
            }
            if (facetType == typeof (IDescribedAsFacet)) {
                return GetDescriptionFacet(holder, facet as IDescribedAsFacet, identifier);
            }

            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return new[] {typeof (INamedFacet), typeof (IDescribedAsFacet)}; }
        }

        #endregion

       

    
        private IFacet GetDescriptionFacet(IFacetHolder holder, IDescribedAsFacet facet, IIdentifier identifier) {
            string i18nDescription;
            string original = (facet).Value;
            if (holder is NakedObjectActionParameterAbstract) {
                int index = ((NakedObjectActionParameterAbstract) holder).Number;
                i18nDescription = i18nManager.GetParameterDescription(identifier, index, original);

                if (!staticFacets) {
                    return new DescribedAsFacetDynamicWrapI18n(i18nManager, holder, identifier, facet, index);
                }
            }
            else {
                i18nDescription = i18nManager.GetDescription(identifier, original);

                if (!staticFacets) {
                    return new DescribedAsFacetDynamicWrapI18n(i18nManager, holder, identifier, facet);
                }
            }
            return i18nDescription == null ? null : new DescribedAsFacetWrapI18n(i18nDescription, facet.FacetHolder);
        }

        private IFacet GetNamedFacet(IFacetHolder holder, INamedFacet facet, IIdentifier identifier) {
            string original = (facet).Value ?? NameUtils.NaturalName(identifier.MemberName);
            string i18nName;
            if (holder is NakedObjectActionParameterAbstract) {
                int index = ((NakedObjectActionParameterAbstract) holder).Number;
                i18nName = i18nManager.GetParameterName(identifier, index, original);

                if (!staticFacets) {
                    return new NamedFacetDynamicWrapI18n(i18nManager, holder, identifier, facet, index);
                }
            }
            else {
                i18nName = i18nManager.GetName(identifier, original);

                if (!staticFacets) {
                    return new NamedFacetDynamicWrapI18n(i18nManager, holder, identifier, facet);
                }
            }
            return i18nName == null ? null : new NamedFacetWrapI18n(i18nName, facet.FacetHolder);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}