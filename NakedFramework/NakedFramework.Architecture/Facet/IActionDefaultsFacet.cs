// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Architecture.Facet;

/// <summary>
///     Obtain defaults for each of the parameters of the action
/// </summary>
/// <para>
///     In the standard Naked Objects Programming Model, corresponds to
///     invoking the <c>DefaultsXxx(...)</c> support method for an action
/// </para>
public interface IActionDefaultsFacet : IFacet {
    (object value, TypeOfDefaultValue type) GetDefault(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework);
}