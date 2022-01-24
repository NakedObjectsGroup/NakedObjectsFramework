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
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class HideForContextViaAboutMethodFacet : AbstractViaAboutMethodFacet, IHideForContextFacet {
    private readonly ILogger<HideForContextViaAboutMethodFacet> logger;

    public HideForContextViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutHelpers.AboutType aboutType, ILogger<HideForContextViaAboutMethodFacet> logger)
        : base(typeof(IHideForContextFacet), holder, method, aboutType) =>
        this.logger = logger;

    #region IHideForContextFacet Members

    public string Hides(IInteractionContext ic) => HiddenReason(ic.Target, ic.Framework);

    public Exception CreateExceptionFor(IInteractionContext ic) => new HiddenException(ic, Hides(ic));

    public string HiddenReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        var about = InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Visible, false, true);
        return about.Visible ? null : NakedObjects.Resources.NakedObjects.Hidden;
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.