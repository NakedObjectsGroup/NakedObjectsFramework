// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Reflect;

namespace NakedObjects.Core.Util {
    public static class InteractionUtils {
        public static bool IsVisible(ISpecification specification, IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            var buf = new InteractionBuffer();
            IEnumerable<IHidingInteractionAdvisor> facets = specification.GetFacets().Where(f => f is IHidingInteractionAdvisor).Cast<IHidingInteractionAdvisor>();
            foreach (IHidingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Hides(ic, lifecycleManager, manager));
            }
            return IsVisible(buf);
        }

        public static bool IsVisibleWhenPersistent(ISpecification specification, IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            var buf = new InteractionBuffer();
            IEnumerable<IHidingInteractionAdvisor> facets = specification.GetFacets().Where(f => f is IHidingInteractionAdvisor).Cast<IHidingInteractionAdvisor>();
            foreach (IHidingInteractionAdvisor advisor in facets) {
                var facet = advisor as IHiddenFacet;
                if (facet?.Value != WhenTo.UntilPersisted) {
                    buf.Append(advisor.Hides(ic, lifecycleManager, manager));
                }
            }
            return IsVisible(buf);
        }

        private static bool IsVisible(IInteractionBuffer buf) {
            return buf.IsEmpty;
        }

        public static IConsent IsUsable(ISpecification specification, IInteractionContext ic) {
            IInteractionBuffer buf = IsUsable(specification, ic, new InteractionBuffer());
            return IsUsable(buf);
        }

        private static IInteractionBuffer IsUsable(ISpecification specification, IInteractionContext ic, IInteractionBuffer buf) {
            IEnumerable<IDisablingInteractionAdvisor> facets = specification.GetFacets().Where(f => f is IDisablingInteractionAdvisor).Cast<IDisablingInteractionAdvisor>();
            foreach (IDisablingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Disables(ic));
            }
            return buf;
        }

        private static IConsent IsUsable(IInteractionBuffer buf) {
            return GetConsent(buf.ToString());
        }

        public static IConsent IsValid(ISpecification specification, IInteractionContext ic) {
            IInteractionBuffer buf = IsValid(specification, ic, new InteractionBuffer());
            return IsValid(buf);
        }

        public static IInteractionBuffer IsValid(ISpecification specification, IInteractionContext ic, IInteractionBuffer buf) {
            IEnumerable<IValidatingInteractionAdvisor> facets = specification.GetFacets().Where(f => f is IValidatingInteractionAdvisor).Cast<IValidatingInteractionAdvisor>();
            foreach (IValidatingInteractionAdvisor advisor in facets) {
                buf.Append(advisor.Invalidates(ic));
            }
            return buf;
        }

        public static IConsent IsValid(IInteractionBuffer buf) {
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