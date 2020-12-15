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
using NakedObjects.Meta.Facet;
using NakedObjects.Profile;

namespace NakedObjects.Meta.Profile {
    [Serializable]
    public sealed class ProfileCallbackFacet : CallbackFacetAbstract,
        ICreatedCallbackFacet,
        IDeletedCallbackFacet,
        IDeletingCallbackFacet,
        ILoadedCallbackFacet,
        ILoadingCallbackFacet,
        IPersistedCallbackFacet,
        IPersistingCallbackFacet,
        IUpdatedCallbackFacet,
        IUpdatingCallbackFacet {
        private readonly ProfileEvent associatedEvent;
        private readonly IProfileManager profileManager;
        private readonly ICallbackFacet underlyingFacet;

        public ProfileCallbackFacet(ProfileEvent associatedEvent, ICallbackFacet underlyingFacet, IProfileManager profileManager) : base(underlyingFacet.FacetType, underlyingFacet.Specification) {
            this.associatedEvent = associatedEvent;
            this.underlyingFacet = underlyingFacet;
            this.profileManager = profileManager;
        }

        #region ICreatedCallbackFacet Members

        public override void Invoke(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager metamodelManager) {
            profileManager.Begin(session, associatedEvent, "", nakedObjectAdapter, lifecycleManager);
            try {
                underlyingFacet.Invoke(nakedObjectAdapter, session, lifecycleManager, metamodelManager);
            }
            finally {
                profileManager.End(session, associatedEvent, "", nakedObjectAdapter, lifecycleManager);
            }
        }

        #endregion
    }
}