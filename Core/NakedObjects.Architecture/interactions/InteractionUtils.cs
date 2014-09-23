// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Interactions {
    public static class InteractionUtils {
        public static bool IsVisible(IFacetHolder facetHolder, InteractionContext ic, ILifecycleManager persistor) {
            var buf = new InteractionBuffer();
            IFacet[] facets = facetHolder.GetFacets(FacetFilters.IsA(typeof (IHidingInteractionAdvisor)));
            foreach (IHidingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Hides(ic, persistor));
            }
            return IsVisible(buf);
        }

        public static bool IsVisibleWhenPersistent(IFacetHolder facetHolder, InteractionContext ic, ILifecycleManager persistor) {
            var buf = new InteractionBuffer();
            IFacet[] facets = facetHolder.GetFacets(FacetFilters.IsA(typeof (IHidingInteractionAdvisor)));
            foreach (IHidingInteractionAdvisor advisor in facets) {
                if (advisor is IHiddenFacet) {
                    if (((IHiddenFacet) advisor).Value == WhenTo.OncePersisted) {
                        continue;
                    }
                }
                buf.Append(advisor.Hides(ic, persistor));
            }
            return IsVisible(buf);
        }

        private static bool IsVisible(InteractionBuffer buf) {
            return buf.IsEmpty;
        }

        public static IConsent IsUsable(IFacetHolder facetHolder, InteractionContext ic) {
            InteractionBuffer buf = IsUsable(facetHolder, ic, new InteractionBuffer());
            return IsUsable(buf);
        }

        private static InteractionBuffer IsUsable(IFacetHolder facetHolder, InteractionContext ic, InteractionBuffer buf) {
            IFacet[] facets = facetHolder.GetFacets(FacetFilters.IsA(typeof (IDisablingInteractionAdvisor)));
            foreach (IDisablingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Disables(ic));
            }
            return buf;
        }

        /// <summary>
        ///     To decode an <see cref="InteractionBuffer" /> returned by
        ///     <see
        ///         cref="IsUsable(IFacetHolder,InteractionContext,InteractionBuffer)" />
        /// </summary>
        private static IConsent IsUsable(InteractionBuffer buf) {
            return GetConsent(buf.ToString());
        }

        public static IConsent IsValid(IFacetHolder facetHolder, InteractionContext ic) {
            InteractionBuffer buf = IsValid(facetHolder, ic, new InteractionBuffer());
            return IsValid(buf);
        }

        public static InteractionBuffer IsValid(IFacetHolder facetHolder, InteractionContext ic, InteractionBuffer buf) {
            IFacet[] facets = facetHolder.GetFacets(FacetFilters.IsA(typeof (IValidatingInteractionAdvisor)));
            foreach (IValidatingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Invalidates(ic));
            }
            return buf;
        }

        /// <summary>
        ///     To decode an <see cref="InteractionBuffer" /> returned by
        ///     <see
        ///         cref="IsValid(IFacetHolder,InteractionContext,InteractionBuffer)" />
        /// </summary>
        public static IConsent IsValid(InteractionBuffer buf) {
            return GetConsent(buf.ToString());
        }


        private static IConsent GetConsent(string message) {
            if (string.IsNullOrEmpty(message)) {
                return Allow.Default;
            }
            return new Veto(message);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}