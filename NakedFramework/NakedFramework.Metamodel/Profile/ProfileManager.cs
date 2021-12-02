// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Profile;

namespace NakedFramework.Metamodel.Profile;

[Serializable]
public sealed class ProfileManager : IFacetDecorator, IProfileManager {
    private static readonly IDictionary<ProfileEvent, Type> EventToFacetMap = new Dictionary<ProfileEvent, Type> {
        { ProfileEvent.ActionInvocation, typeof(IActionInvocationFacet) },
        { ProfileEvent.PropertySet, typeof(IPropertySetterFacet) },
        { ProfileEvent.Created, typeof(ICreatedCallbackFacet) },
        { ProfileEvent.Deleted, typeof(IDeletedCallbackFacet) },
        { ProfileEvent.Deleting, typeof(IDeletingCallbackFacet) },
        { ProfileEvent.Loaded, typeof(ILoadedCallbackFacet) },
        { ProfileEvent.Loading, typeof(ILoadingCallbackFacet) },
        { ProfileEvent.Persisted, typeof(IPersistedCallbackFacet) },
        { ProfileEvent.Persisting, typeof(IPersistingCallbackFacet) },
        { ProfileEvent.Updated, typeof(IUpdatedCallbackFacet) },
        { ProfileEvent.Updating, typeof(IUpdatingCallbackFacet) }
    };

    private static readonly IDictionary<Type, Func<IFacet, IProfileManager, IFacet>> FacetToConstructorMap = new Dictionary<Type, Func<IFacet, IProfileManager, IFacet>> {
        { typeof(IActionInvocationFacet), (f, pm) => new ProfileActionInvocationFacet((IActionInvocationFacet)f, pm) },
        { typeof(IPropertySetterFacet), (f, pm) => new ProfilePropertySetterFacet((IPropertySetterFacet)f, pm) },
        { typeof(ICreatedCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Created, (ICallbackFacet)f, pm) },
        { typeof(IDeletedCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Deleted, (ICallbackFacet)f, pm) },
        { typeof(IDeletingCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Deleting, (ICallbackFacet)f, pm) },
        { typeof(ILoadedCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Loaded, (ICallbackFacet)f, pm) },
        { typeof(ILoadingCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Loading, (ICallbackFacet)f, pm) },
        { typeof(IPersistedCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Persisted, (ICallbackFacet)f, pm) },
        { typeof(IPersistingCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Persisting, (ICallbackFacet)f, pm) },
        { typeof(IUpdatedCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Updated, (ICallbackFacet)f, pm) },
        { typeof(IUpdatingCallbackFacet), (f, pm) => new ProfileCallbackFacet(ProfileEvent.Updating, (ICallbackFacet)f, pm) }
    };

    private readonly Type profilerType;

    public ProfileManager(IProfileConfiguration config) {
        profilerType = config.Profiler;

        if (!typeof(IProfiler).IsAssignableFrom(profilerType)) {
            throw new InitialisationException($"{profilerType.FullName} is not an IProfiler");
        }

        ForFacetTypes = config.EventsToProfile.Select(e => EventToFacetMap[e]).ToArray();
    }

    private IProfiler GetProfiler(ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedInjectedObject(profilerType) as IProfiler;

    #region IFacetDecorator Members

    public IFacet Decorate(IFacet facet, ISpecification holder) => ForFacetTypes.Contains(facet.FacetType) ? FacetToConstructorMap[facet.FacetType](facet, this) : facet;

    public Type[] ForFacetTypes { get; }

    #endregion

    #region IProfileManager Members

    public void Begin(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) => GetProfiler(lifecycleManager).Begin(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);

    public void End(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) => GetProfiler(lifecycleManager).End(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);

    #endregion
}