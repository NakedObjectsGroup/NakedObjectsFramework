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
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class PropertyChoicesViaAboutMethodFacet : AbstractViaAboutMethodFacet, IPropertyChoicesFacet, IImperativeFacet {
    public PropertyChoicesViaAboutMethodFacet(MethodInfo optionsMethod, ISpecification holder, ILogger<PropertyChoicesViaAboutMethodFacet> logger)
        : base(typeof(IPropertyChoicesFacet), holder, optionsMethod, AboutHelpers.AboutType.Field) { }

    #region IPropertyChoicesFacet Members

    public (string, IObjectSpecImmutable)[] ParameterNamesAndTypes => Array.Empty<(string, IObjectSpecImmutable)>();

    public IAbout GetAbout(INakedObjectAdapter nakedObjectAdapter) => InvokeAboutMethod(nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Parameters, false, true);

    private object[] GetChoices(INakedObjectAdapter nakedObjectAdapter) {
        if (GetAbout(nakedObjectAdapter) is FieldAbout fa) {
            return fa.Options?.Any() == true ? fa.Options : Array.Empty<object>();
        }

        return Array.Empty<object>();
    }

    public bool IsEnabled(INakedObjectAdapter nakedObjectAdapter) => GetChoices(nakedObjectAdapter).Any();

    public object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) => GetChoices(inObjectAdapter);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.