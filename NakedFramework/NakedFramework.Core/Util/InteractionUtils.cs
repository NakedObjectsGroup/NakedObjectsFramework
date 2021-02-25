// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Interactions;
using NakedFramework.Core.Reflect;

namespace NakedFramework.Core.Util {
    public static class InteractionUtils {
        public static bool IsVisible(ISpecification specification, IInteractionContext ic) {
            var buf = new InteractionBuffer();
            var facets = specification.GetFacets().Where(f => f is IHidingInteractionAdvisor).Cast<IHidingInteractionAdvisor>();
            foreach (var advisor in facets) {
                buf.Append(advisor.Hides(ic));
            }

            return IsVisible(buf);
        }

        public static bool IsVisibleWhenPersistent(ISpecification specification, IInteractionContext ic) {
            var buf = new InteractionBuffer();
            var facets = specification.GetFacets()
                                      .Where(f => f is IHidingInteractionAdvisor)
                                      .Cast<IHidingInteractionAdvisor>();
            foreach (var advisor in facets) {
                if (advisor is IHiddenFacet facet) {
                    buf.Append(facet.HidesForState(true));
                }
                else {
                    buf.Append(advisor.Hides(ic));
                }
            }

            return IsVisible(buf);
        }

        private static bool IsVisible(IInteractionBuffer buf) => buf.IsEmpty;

        public static IConsent IsUsable(ISpecification specification, IInteractionContext ic) {
            var buf = IsUsable(specification, ic, new InteractionBuffer());
            return IsUsable(buf);
        }

        private static IInteractionBuffer IsUsable(ISpecification specification, IInteractionContext ic, IInteractionBuffer buf) {
            var facets = specification.GetFacets().Where(f => f is IDisablingInteractionAdvisor).Cast<IDisablingInteractionAdvisor>();
            foreach (var advisor in facets) {
                buf.Append(advisor.Disables(ic));
            }

            return buf;
        }

        private static IConsent IsUsable(IInteractionBuffer buf) => GetConsent(buf.ToString());

        public static IInteractionBuffer IsValid(ISpecification specification, IInteractionContext ic, IInteractionBuffer buf) {
            var facets = specification.GetFacets().Where(f => f is IValidatingInteractionAdvisor).Cast<IValidatingInteractionAdvisor>();
            foreach (var advisor in facets) {
                buf.Append(advisor.Invalidates(ic));
            }

            return buf;
        }

        public static IConsent IsValid(IInteractionBuffer buf) => GetConsent(buf.ToString());

        private static IConsent GetConsent(string message) {
            if (string.IsNullOrEmpty(message)) {
                return Allow.Default;
            }

            return new Veto(message);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}