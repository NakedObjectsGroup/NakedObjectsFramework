// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Profile;

namespace NakedFramework.Metamodel.Profile;

[Serializable]
public sealed class ProfileManager : IProfileManager {
    private readonly Type profilerType;

    public ProfileManager(IProfileConfiguration config) {
        profilerType = config.Profiler;

        if (!typeof(IProfiler).IsAssignableFrom(profilerType)) {
            throw new InitialisationException($"{profilerType.FullName} is not an IProfiler");
        }
    }

    private IProfiler GetProfiler(ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedInjectedObject(profilerType) as IProfiler;

    #region IProfileManager Members

    public void Begin(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) => GetProfiler(lifecycleManager).Begin(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);

    public void End(ISession session, ProfileEvent profileEvent, string member, INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) => GetProfiler(lifecycleManager).End(session.Principal, profileEvent, nakedObjectAdapter.GetDomainObject().GetType(), member);

    #endregion
}