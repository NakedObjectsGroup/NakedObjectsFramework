// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Represents the mechanism to inform the object that it is about to be persisted to the object store for the first
    ///     time
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, this is represented
    ///     by a <c>Persisting</c> method. Called only if the object is known to be in a valid state.
    /// </para>
    /// <seealso cref="IPersistedCallbackFacet" />
    public interface IPersistingCallbackFacet : ICallbackFacet { }

    // Copyright (c) Naked Objects Group Ltd.
}