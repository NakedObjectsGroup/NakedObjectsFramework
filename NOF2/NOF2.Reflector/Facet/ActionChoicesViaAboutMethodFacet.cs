// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Util;
using NOF2.About;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class ActionChoicesViaAboutMethodFacet : AbstractViaAboutMethodFacet, IActionChoicesFacet {
    private readonly int index;

    public ActionChoicesViaAboutMethodFacet(MethodInfo aboutmethod, int index, ILogger<ActionChoicesViaAboutMethodFacet> logger)
        : base(aboutmethod, AboutHelpers.AboutType.Action, logger) =>
        this.index = index;

    public override Type FacetType => typeof(IActionChoicesFacet);

    public (string, Type)[] ParameterNamesAndTypes => Array.Empty<(string, Type)>();

    public bool IsMultiple => false;

    public bool IsEnabled(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetChoices(nakedObjectAdapter, framework).Any();

    public object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework) => GetChoices(nakedObjectAdapter, framework);

    private object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (GetAbout(nakedObjectAdapter, framework) is ActionAbout actionAbout) {
            var parameterOptions = actionAbout.ParamOptions?.Any() == true ? actionAbout.ParamOptions[index] : Array.Empty<object>();
            if (parameterOptions.Any()) {
                return parameterOptions;
            }
        }

        return Array.Empty<object>();
    }

    public IAbout GetAbout(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Parameters, false, true);
}

// Copyright (c) Naked Objects Group Ltd.