// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;

namespace NakedFramework.Architecture.Component; 

public interface IFacetFactory : IComparable<IFacetFactory> {
    /// <summary>
    ///     To order the factory
    /// </summary>
    int NumericOrder { get; }

    /// <summary>
    ///     The <see cref="FeatureType" />s that this facet factory can create <see cref="IFacet" />s for.
    /// </summary>
    /// <para>
    ///     Used by the <see cref="IFacetFactorySet" /> to reduce the number of <see cref="IFacetFactory" />s that are
    ///     queried when building up the meta-model.
    /// </para>
    FeatureType FeatureTypes { get; }
}