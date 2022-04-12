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
using NakedFramework.Profile;

namespace NakedFramework.Metamodel.Profile;

[Serializable]
public sealed class ProfileDecorator : IFacetDecorator {
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

    private static readonly IDictionary<Type, Func<IFacet, IIdentifier, IFacet>> FacetToConstructorMap = new Dictionary<Type, Func<IFacet, IIdentifier, IFacet>> {
        { typeof(IActionInvocationFacet), (f, i) => new ProfileActionInvocationFacet((IActionInvocationFacet)f, i) },
        { typeof(IPropertySetterFacet), (f, _) => new ProfilePropertySetterFacet((IPropertySetterFacet)f) },
        { typeof(ICreatedCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Created, (ICallbackFacet)f) },
        { typeof(IDeletedCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Deleted, (ICallbackFacet)f) },
        { typeof(IDeletingCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Deleting, (ICallbackFacet)f) },
        { typeof(ILoadedCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Loaded, (ICallbackFacet)f) },
        { typeof(ILoadingCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Loading, (ICallbackFacet)f) },
        { typeof(IPersistedCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Persisted, (ICallbackFacet)f) },
        { typeof(IPersistingCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Persisting, (ICallbackFacet)f) },
        { typeof(IUpdatedCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Updated, (ICallbackFacet)f) },
        { typeof(IUpdatingCallbackFacet), (f, _) => new ProfileCallbackFacet(ProfileEvent.Updating, (ICallbackFacet)f) }
    };

    public ProfileDecorator(IProfileConfiguration config) => ForFacetTypes = config.EventsToProfile.Select(e => EventToFacetMap[e]).ToArray();

    #region IFacetDecorator Members

    public IFacet Decorate(IFacet facet, ISpecification holder) => ForFacetTypes.Contains(facet.FacetType) ? FacetToConstructorMap[facet.FacetType](facet, holder.Identifier) : facet;

    public Type[] ForFacetTypes { get; }

    #endregion
}