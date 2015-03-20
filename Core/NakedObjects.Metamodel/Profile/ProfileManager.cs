// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Profile {
    [Serializable]
    public sealed class ProfileManager : IFacetDecorator, IProfileManager {
        private readonly Type defaultProfiler;
        private readonly Type[] forFacetTypes = {typeof (IActionInvocationFacet), typeof (IUpdatedCallbackFacet), typeof (IPersistedCallbackFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceProfilers;

        public ProfileManager(IProfileConfiguration config) {
            defaultProfiler = config.DefaultProfiler;
            namespaceProfilers = config.NamespaceProfilers.OrderByDescending(x => x.Key.Length).ToImmutableDictionary();
            Validate();
        }

        #region IFacetDecorator Members

        public IFacet Decorate(IFacet facet, ISpecification holder) {
            if (facet.FacetType == typeof (IActionInvocationFacet)) {
                return new ProfileActionInvocationFacet((IActionInvocationFacet) facet, this);
            }

            if (facet.FacetType == typeof (IUpdatedCallbackFacet)) {
                return new ProfileUpdatedFacet((IUpdatedCallbackFacet) facet, this);
            }

            if (facet.FacetType == typeof (IPersistedCallbackFacet)) {
                return new ProfilePersistedFacet((IPersistedCallbackFacet) facet, this);
            }

            return facet;
        }

        public Type[] ForFacetTypes {
            get { return forFacetTypes; }
        }

        #endregion

        #region IProfileManager Members

        public void Begin(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager) {
            IProfiler profiler = GetProfiler(nakedObjectAdapter, lifecycleManager);
            profiler.Begin();
        }

        public void End(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager) {
            IProfiler profiler = GetProfiler(nakedObjectAdapter, lifecycleManager);
            profiler.End();
        }

        #endregion

        private void ValidateType(Type toValidate) {
            if (!typeof (IProfiler).IsAssignableFrom(toValidate)) {
                throw new InitialisationException(toValidate.FullName + " is not an IProfiler");
            }
        }

        private void Validate() {
            ValidateType(defaultProfiler);
            if (namespaceProfilers.Any()) {
                namespaceProfilers.ForEach(kvp => ValidateType(kvp.Value));
            }
        }

        private IProfiler GetProfiler(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) {
            return GetNamespaceProfilerFor(nakedObjectAdapter, lifecycleManager) ?? GetDefaultProfiler(lifecycleManager);
        }

        private IProfiler GetNamespaceProfilerFor(INakedObjectAdapter target, ILifecycleManager lifecycleManager) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Spec.FullName;

            // already ordered OrderByDescending(x => x.Key.Length).
            Type profiler = namespaceProfilers.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).
                Select(x => x.Value).
                FirstOrDefault();

            return profiler != null ? CreateProfiler(profiler, lifecycleManager) : null;
        }

        private IProfiler CreateProfiler(Type profiler, ILifecycleManager lifecycleManager) {
            return lifecycleManager.CreateNonAdaptedInjectedObject(profiler) as IProfiler;
        }

        private IProfiler GetDefaultProfiler(ILifecycleManager lifecycleManager) {
            return CreateProfiler(defaultProfiler, lifecycleManager);
        }
    }
}