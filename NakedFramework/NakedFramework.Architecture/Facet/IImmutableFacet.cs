// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Interactions;

namespace NakedFramework.Architecture.Facet;

/// <summary>
///     Indicates that the instances of this class are immutable and so may not be modified
///     either through the viewer or indeed programmatically
/// </summary>
/// <para>
///     In the standard Naked Objects Programming Model, typically corresponds to applying the
///     <see cref="ImmutableAttribute" /> annotation at the class level
/// </para>
public interface IImmutableFacet : ISingleWhenValueFacet, IDisablingInteractionAdvisor { }