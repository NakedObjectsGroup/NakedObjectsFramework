// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;

namespace NakedObjects.Services {
    public interface IPolymorphicNavigatorWithOid {
        TLink AddLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, IHasIntegerId;

        IQueryable<TOwner> FindOwners<TLink, TRole, TOwner>(TRole value)
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, IHasIntegerId;

        TLink NewTransientLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, IHasIntegerId;

        void RemoveLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, IHasIntegerId;

        TLink UpdateAddOrDeleteLink<TLink, TRole, TOwner>(TRole value, TLink link, TOwner owner)
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, IHasIntegerId;
    }
}