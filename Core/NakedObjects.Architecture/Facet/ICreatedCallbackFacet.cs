// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Represents the mechanism to inform the object that it has just been created.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, this is represented
    ///     by a <c>Created</c> method.  The framework calls this once the object
    ///     has been created via <c>NewTransientInstance</c> or
    ///     <c>NewInstance</c>.  The method is <i>not</i> called when the object
    ///     is subsequently resolved having been persisted; for that see
    ///     <see cref="ILoadingCallbackFacet" /> and <see cref="ILoadedCallbackFacet" />
    /// </para>
    /// <seealso cref="ILoadingCallbackFacet" />
    /// <seealso cref="ILoadedCallbackFacet" />
    public interface ICreatedCallbackFacet : ICallbackFacet {}

    // Copyright (c) Naked Objects Group Ltd.
}