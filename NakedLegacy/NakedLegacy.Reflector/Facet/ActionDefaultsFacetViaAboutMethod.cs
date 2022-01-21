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
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class ActionDefaultsViaAboutMethodFacet : AbstractViaAboutMethodFacet, IActionDefaultsFacet, IImperativeFacet {
    private readonly int index;
    private readonly ILogger<ActionDefaultsViaAboutMethodFacet> logger;
    
    public ActionDefaultsViaAboutMethodFacet(MethodInfo method, ISpecification holder, int index, ILogger<ActionDefaultsViaAboutMethodFacet> logger)
        : base(typeof(IActionDefaultsFacet), holder, method, AboutHelpers.AboutType.Action) {
        this.index = index;
        this.logger = logger;
    }

    public (object, TypeOfDefaultValue) GetDefault(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
      
        if (GetAbout(nakedObjectAdapter) is ActionAbout actionAbout) {
            var parameterDefaults = actionAbout.ParamDefaultValues ?? Array.Empty<object>();
            var aboutDefault = parameterDefaults.Length > index ? parameterDefaults[index] : null;
            if (aboutDefault is not null) {
                return (aboutDefault, TypeOfDefaultValue.Explicit);
            }
        }

        return (null, TypeOfDefaultValue.Implicit);
    }

    public IAbout GetAbout(INakedObjectAdapter nakedObjectAdapter) => InvokeAboutMethod(nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Parameters, false, true);
}

// Copyright (c) Naked Objects Group Ltd.