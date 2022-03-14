// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Facet;

namespace NakedFramework.Metamodel.Facet;

/// <summary>
///     This is only used at by the custom 'SdmNotCountedAttribute'
/// </summary>
[Serializable]
public sealed class NotCountedFacet : FacetAbstract, INotCountedFacet, IMarkerFacet {
    private NotCountedFacet() { }

    private static NotCountedFacet instance;

    public static NotCountedFacet Instance => instance ??= new NotCountedFacet();

    public override Type FacetType => typeof(INotCountedFacet);
}

// Copyright (c) Naked Objects Group Ltd.