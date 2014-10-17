// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Indicates a collection should not be counted in the summary view.
    ///     There is currently no attribute or programming convention in the
    ///     standard ProgrammingModel that makes use of this, but developers
    ///     may add their own, and this is known to be in use in one site at least.
    /// </summary>
    public interface INotCountedFacet : IMarkerFacet {}
}