// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
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
public sealed class ActionValidateViaAboutMethodFacet : AbstractViaAboutMethodFacet, IActionValidationFacet {
    public ActionValidateViaAboutMethodFacet(MethodInfo method, AboutHelpers.AboutType aboutType, ILogger<ActionValidateViaAboutMethodFacet> logger)
        : base(typeof(IPropertyValidateFacet), method, aboutType, logger) { }

    public override Type FacetType => typeof(IActionValidationFacet);
    public string Invalidates(IInteractionContext ic) => InvalidReason(ic.Target, ic.Framework, ic.ProposedArguments);

    public Exception CreateExceptionFor(IInteractionContext ic) => new InvalidException(ic, Invalidates(ic));

    public string InvalidReason(INakedObjectAdapter target, INakedFramework framework, INakedObjectAdapter[] proposedArgument) {
        if (InvokeAboutMethod(framework, target.GetDomainObject(), AboutTypeCodes.Valid, true, true, proposedArgument.Select(no => no?.Object).ToArray()) is ActionAbout actionAbout) {
            return actionAbout.Usable ? null : string.IsNullOrWhiteSpace(actionAbout.UnusableReason) ? "Invalid Parameter" : actionAbout.UnusableReason;
        }

        return null;
    }
}

// Copyright (c) Naked Objects Group Ltd.