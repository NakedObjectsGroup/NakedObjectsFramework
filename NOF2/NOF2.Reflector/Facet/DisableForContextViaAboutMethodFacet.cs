// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NOF2.About;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class DisableForContextViaAboutMethodFacet : AbstractViaAboutMethodFacet, IDisableForContextFacet {
    public DisableForContextViaAboutMethodFacet(MethodInfo method, AboutHelpers.AboutType aboutType, ILogger<DisableForContextViaAboutMethodFacet> logger)
        : base(typeof(IDisableForContextFacet), method, aboutType, logger) { }

    public override Type FacetType => typeof(IDisableForContextFacet);
    public string Disables(IInteractionContext ic) => DisabledReason(ic.Target, ic.Framework);

    public Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

    public string DisabledReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        var about = InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Usable, false, true);

        return about.Usable ? null : string.IsNullOrWhiteSpace(about.UnusableReason) ? NakedObjects.Resources.NakedObjects.Disabled : about.UnusableReason;
    }
}

// Copyright (c) Naked Objects Group Ltd.