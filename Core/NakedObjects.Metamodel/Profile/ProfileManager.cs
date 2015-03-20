// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
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
        private static readonly IDictionary<ProfileEvent, Type> EventToFacetMap = new Dictionary<ProfileEvent, Type> {
            {ProfileEvent.ActionInvocation, typeof (IActionInvocationFacet)},
            {ProfileEvent.Persisted, typeof (IPersistedCallbackFacet)},
            {ProfileEvent.Updated, typeof (IUpdatedCallbackFacet)}
        };

        private static readonly IDictionary<Type, Func<IFacet, IProfileManager, IFacet>> FacetToConstructorMap = new Dictionary<Type, Func<IFacet, IProfileManager, IFacet>> {
            {typeof (IActionInvocationFacet), (f, pm) => new ProfileActionInvocationFacet((IActionInvocationFacet) f, pm)},
            {typeof (IPersistedCallbackFacet), (f, pm) => new ProfilePersistedFacet((IPersistedCallbackFacet) f, pm)},
            {typeof (IUpdatedCallbackFacet), (f, pm) => new ProfileUpdatedFacet((IUpdatedCallbackFacet) f, pm)},
        };

        private readonly Type[] forFacetTypes;
        private readonly Type profilerType;

        public ProfileManager(IProfileConfiguration config) {
            profilerType = config.Profiler;

            if (!typeof (IProfiler).IsAssignableFrom(profilerType)) {
                throw new InitialisationException(profilerType.FullName + " is not an IProfiler");
            }

            forFacetTypes = config.EventsToProfile.Select(e => EventToFacetMap[e]).ToArray();
        }

        #region IFacetDecorator Members

        public IFacet Decorate(IFacet facet, ISpecification holder) {
            return forFacetTypes.Contains(facet.FacetType) ? FacetToConstructorMap[facet.FacetType](facet, this) : facet;
        }

        public Type[] ForFacetTypes {
            get { return forFacetTypes; }
        }

        #endregion

        #region IProfileManager Members

        public void Begin(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) {
            GetProfiler(lifecycleManager).Begin(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);
        }

        public void End(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) {
            GetProfiler(lifecycleManager).End(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);
        }

        #endregion

        private IProfiler GetProfiler(ILifecycleManager lifecycleManager) {
            return lifecycleManager.CreateNonAdaptedInjectedObject(profilerType) as IProfiler;
        }
    }
}