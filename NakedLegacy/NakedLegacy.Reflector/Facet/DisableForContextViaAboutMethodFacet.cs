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
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class DisableForContextViaAboutMethodFacet : AbstractViaAboutMethodFacet, IDisableForContextFacet {
    private readonly ILogger<DisableForContextViaAboutMethodFacet> logger;

    public DisableForContextViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutHelpers.AboutType aboutType, ILogger<DisableForContextViaAboutMethodFacet> logger)
        : base(typeof(IDisableForContextFacet), holder, method, aboutType) =>
        this.logger = logger;

    public string Disables(IInteractionContext ic) => DisabledReason(ic.Target, ic.Framework);

    public Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

    public string DisabledReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (nakedObjectAdapter == null) {
            return null;
        }

        var about = InvokeAboutMethod(nakedObjectAdapter.Object, AboutTypeCodes.Usable);

        return about.Usable ? null :  string.IsNullOrWhiteSpace(about.UnusableReason) ?  NakedObjects.Resources.NakedObjects.Disabled : about.UnusableReason;
    }
}

// Copyright (c) Naked Objects Group Ltd.