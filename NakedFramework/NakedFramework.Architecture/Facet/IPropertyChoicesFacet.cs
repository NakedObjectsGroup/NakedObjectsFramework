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
///     Provides a set of choices for a property.
/// </summary>
/// <para>
///     Viewers would typically represent this as a drop-down list box for the property.
/// </para>
/// <para>
///     In the standard Naked Objects Programming Model, corresponds to
///     the <c>ChoicesXxx</c> supporting method for the property <c>Xxx</c>.
/// </para>
/// <para>
///     An alternative mechanism may be to use the <see cref="BoundedAttribute" /> annotation
///     against the referenced class.
/// </para>
public interface IPropertyChoicesFacet : IFacet {
    (string, Type)[] ParameterNamesAndTypes { get; }

    bool IsEnabled(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework);

    /// <summary>
    ///     Gets the available choices for this property
    /// </summary>
    object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework);
}