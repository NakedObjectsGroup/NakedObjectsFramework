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
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class ActionChoicesViaAboutMethodFacet : AbstractViaAboutMethodFacet, IActionChoicesFacet, IImperativeFacet {
    private readonly int index;
    private readonly ILogger<ActionChoicesViaAboutMethodFacet> logger;

    public ActionChoicesViaAboutMethodFacet(MethodInfo aboutmethod, ISpecification holder, int index, ILogger<ActionChoicesViaAboutMethodFacet> logger)
        : base(typeof(IActionChoicesFacet), holder, aboutmethod, AboutHelpers.AboutType.Action) {
        this.index = index;
        this.logger = logger;
    }

    public (string, IObjectSpecImmutable)[] ParameterNamesAndTypes => Array.Empty<(string, IObjectSpecImmutable)>();

    public bool IsMultiple => false;

    public bool IsEnabled(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetChoices(nakedObjectAdapter, framework).Any();

    public object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework) => GetChoices(nakedObjectAdapter, framework);

    private object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (GetAbout(nakedObjectAdapter, framework) is IActionAbout actionAbout) {
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