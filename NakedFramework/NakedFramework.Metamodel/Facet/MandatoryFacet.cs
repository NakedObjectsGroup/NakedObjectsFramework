// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet;

/// <summary>
///     Derived by presence of a <see cref="RequiredAttribute" /> annotation.
/// </summary>
/// <para>
///     This implementation indicates that the <see cref="ISpecification" /> is mandatory.
/// </para>
[Serializable]
public sealed class MandatoryFacet : MandatoryFacetAbstract {
    private MandatoryFacet() { }

    private static MandatoryFacet instance;

    public static MandatoryFacet Instance => instance ??= new MandatoryFacet();


    /// <summary>
    ///     Always returns <c>true</c>, indicating that the facet holder is in fact mandatory.
    /// </summary>
    public override bool IsMandatory => true;
}

// Copyright (c) Naked Objects Group Ltd.