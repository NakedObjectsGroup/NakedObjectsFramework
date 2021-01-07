// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;

namespace NakedObjects.Services {
    /// <summary>
    ///     Service that provides helper methods for navigating polymorphic associations that make
    ///     use of Link objects defined by IPolymorphicLinkWithOid.
    ///     Delegates to an injected implementation of IObjectFinder.
    /// </summary>
    public class PolymorphicNavigatorWithOid : IPolymorphicNavigatorWithOid {
        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns the one (if any) that has the specified owner.  If more than one link
        ///     associates the same value and owner an error is thrown.
        /// </summary>
        public virtual TLink FindPolymorphicLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            var ownerId = owner.Id;
            var links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            var matches = links.Where(x => x.Owner.Id == ownerId);
            return matches.SingleOrDefault();
        }

        private IQueryable<TLink> FindPolymorphicLinks<TLink, TRole, TOwner>(TRole value)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            var roleOid = ObjectFinder.GetCompoundKey(value);
            return Container.Instances<TLink>().Where(x => x.RoleObjectOid == roleOid);
        }

        #region IPolymorphicNavigatorWithOid Members

        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns a queryable of those links' owners.
        /// </summary>
        public virtual IQueryable<TOwner> FindOwners<TLink, TRole, TOwner>(TRole value)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            var links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            return links.Select(x => x.Owner);
        }

        public virtual TLink AddLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            var link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link != null) {
                return null; //item is already associated,  so don't duplicate
            }

            link = NewTransientLink<TLink, TRole, TOwner>(value, owner);
            Container.Persist(ref link);
            return link;
        }

        public virtual void RemoveLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            var link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link == null) {
                return; //item is not associated
            }

            Container.DisposeInstance(link);
        }

        public virtual TLink NewTransientLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            if (value == null) {
                return null;
            }

            var link = Container.NewTransientInstance<TLink>();
            link.Owner = owner;
            link.RoleObjectOid = ObjectFinder.GetCompoundKey(value);
            return link;
        }

        public virtual TLink UpdateAddOrDeleteLink<TLink, TRole, TOwner>(TRole value, TLink link, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            if (link != null) {
                if (value == null) {
                    Container.DisposeInstance(link);
                    return null;
                }

                link.RoleObjectOid = ObjectFinder.GetCompoundKey(value);
                return link;
            }

            if (Container.IsPersistent(this) && value != null) {
                return AddLink<TLink, TRole, TOwner>(value, owner);
            }

            return null;
        }

        #endregion

        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        public IObjectFinder ObjectFinder { set; protected get; }

        #endregion
    }
}