// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Metamodel.Facet;

/// <summary>
///     Whether a property or a parameter is mandatory (not optional).
/// </summary>
/// <para>
///     For a mandatory property, the object cannot be saved/updated without
///     the value being provided.  For a mandatory parameter, the action cannot
///     be invoked without the value being provided.
/// </para>
/// <para>
///     In the standard Naked Objects Programming Model, specify mandatory by
///     <i>omitting</i> the <see cref="OptionallyAttribute" /> annotation.
/// </para>
[Serializable]
public sealed class MandatoryFacetDefault : MandatoryFacetAbstract {
    public override bool IsMandatory => true;

    public override bool CanAlwaysReplace => false;
}