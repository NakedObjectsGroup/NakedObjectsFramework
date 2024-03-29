// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;

namespace NakedFramework.Architecture.Facet;

/// <summary>
///     Obtain choices for each of the parameters of the action
/// </summary>
/// <para>
///     In the standard Naked Objects Programming Model, corresponds to
///     invoking the <c>ChoicesXxx</c> support method for an
///     action
/// </para>
public interface IActionChoicesFacet : IFacet {
    (string, Type)[] ParameterNamesAndTypes { get; }
    bool IsMultiple { get; }

    bool IsEnabled(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework);

    object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework);
}