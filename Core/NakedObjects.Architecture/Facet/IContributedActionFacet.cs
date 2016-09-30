// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Architecture.Facet {
    public interface IContributedActionFacet : IFacet {
        bool IsContributedTo(IObjectSpecImmutable spec);
        bool IsContributedToCollectionOf(IObjectSpecImmutable spec);
        bool IsContributedToLocalCollectionOf(IObjectSpecImmutable spec, string id);
        //Returns null if the action is to be 'top-level'
        string SubMenuWhenContributedTo(IObjectSpecImmutable spec);
        //Id has been included for generating UI code that is backwards-compatible with NOF 6.
        string IdWhenContributedTo(IObjectSpecImmutable spec);
    }
}