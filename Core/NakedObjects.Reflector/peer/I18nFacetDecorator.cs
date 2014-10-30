// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Spec;
using NakedObjects.Reflect.I18n;
using NakedObjects.Reflect.Spec;
using NakedObjects.Util;

namespace NakedObjects.Core.I18n {
    public class I18nFacetDecorator : IFacetDecorator {
        private readonly II18nManager i18nManager;
        private readonly bool staticFacets;

        public I18nFacetDecorator(II18nManager manager, bool staticFacets) {
            i18nManager = manager;
            this.staticFacets = staticFacets;
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
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

        private IFacet GetDescriptionFacet(ISpecification holder, IDescribedAsFacet facet, IIdentifier identifier) {
            string i18nDescription;
            string original = (facet).Value;
            if (holder is ActionParameterSpec) {
                int index = ((ActionParameterSpec) holder).Number;
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
            return i18nDescription == null ? null : new DescribedAsFacetWrapI18n(i18nDescription, facet.Specification);
        }

        private IFacet GetNamedFacet(ISpecification holder, INamedFacet facet, IIdentifier identifier) {
            string original = (facet).Value ?? NameUtils.NaturalName(identifier.MemberName);
            string i18nName;
            if (holder is ActionParameterSpec) {
                int index = ((ActionParameterSpec) holder).Number;
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
            return i18nName == null ? null : new NamedFacetWrapI18n(i18nName, facet.Specification);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}