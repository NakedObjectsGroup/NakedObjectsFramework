// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Util;

namespace NakedObjects.Services {
    /// <summary>
    ///     Service that provides helper methods for navigating polymorphic associations that make
    ///     use of Link objects defined by IPolymorphicLinkWithOid.
    ///     Delegates to an injected implementation of IObjectFinder.
    /// </summary>
    public class PolymorphicNavigatorWithOid : IPolymorphicNavigatorWithOid {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public IObjectFinder ObjectFinder { set; protected get; }
        #endregion

        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns a queryable of those links' owners.
        /// </summary>
        public virtual IQueryable<TOwner> FindOwners<TLink, TRole, TOwner>(TRole value)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            
            IQueryable<TLink> links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            return links.Select(x => x.Owner);
        }

        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns the one (if any) that has the specified owner.  If more than one link
        ///     associates the same value and owner an error is thrown.
        /// </summary>
        public virtual TLink FindPolymorphicLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            
            int ownerId = owner.Id;
            IQueryable<TLink> links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            IQueryable<TLink> matches = links.Where(x => x.Owner.Id == ownerId);
            return matches.SingleOrDefault();
        }

        private IQueryable<TLink> FindPolymorphicLinks<TLink, TRole, TOwner>(TRole value)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>
            where TOwner : class, IHasIntegerId {

                string roleOid = ObjectFinder.GetCompoundKey(value);
            return Container.Instances<TLink>().Where(x => x.RoleObjectOid == roleOid);
        }

        public virtual TLink AddLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            
            TLink link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link != null) return null; //item is already associated,  so don't duplicate
            link = NewTransientLink<TLink, TRole, TOwner>(value, owner);
            Container.Persist(ref link);
            return link;
        }

        public virtual void RemoveLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            
            TLink link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link == null) return; //item is not associated
            Container.DisposeInstance(link);
        }

        public virtual TLink NewTransientLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class
            where TLink : class, IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            if (value == null) return null;
            
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
    }
}