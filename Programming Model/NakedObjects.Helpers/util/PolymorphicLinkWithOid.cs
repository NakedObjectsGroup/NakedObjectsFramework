// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using NakedObjects.Services;

namespace NakedObjects {
    public class PolymorphicLinkWithOid<TRole, TOwner> : IPolymorphicLinkWithOid<TRole, TOwner>
        where TRole : class
        where TOwner : class, IHasIntegerId {
        #region Injected Services

        public IObjectFinder ObjectFinder { set; protected get; }

        #endregion

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [Disabled]
        public virtual string RoleObjectOid { get; set; }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideRoleObjectOid()
        {
            return false;
        }

        [NotPersisted, Disabled]
        public virtual TRole AssociatedRoleObject {
            get { return ObjectFinder.FindObject<TRole>(RoleObjectOid); }
            set {
                RoleObjectOid = ObjectFinder.GetCompoundKey(value);
            }
        }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideAssociatedRoleObject()
        {
            return false;
        }

        [Required]
        public virtual TOwner Owner { get; set; }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideOwner() {
            return false;
        }
    }
}