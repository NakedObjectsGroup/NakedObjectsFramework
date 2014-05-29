// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using NakedObjects.Services;

namespace NakedObjects {
    public class PolymorphicLink<TRole, TOwner> : IPolymorphicLink<TRole, TOwner>
        where TRole : class, IHasIntegerId
        where TOwner : class, IHasIntegerId {
        #region Injected Services

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        #endregion

        [Disabled, Hidden]
        public virtual int Id { get; set; }

        [Disabled]
        public virtual string AssociatedRoleObjectType { get; set; }


        [Disabled]
        public virtual int AssociatedRoleObjectId { get; set; }

        [NotPersisted, Disabled]
        public virtual TRole AssociatedRoleObject {
            get { return PolymorphicNavigator.FindObject<TRole>(AssociatedRoleObjectType, AssociatedRoleObjectId); }
            set {
                AssociatedRoleObjectType = PolymorphicNavigator.GetType(value);
                AssociatedRoleObjectId = value.Id;
            }
        }

        [Required]
        public virtual TOwner Owner { get; set; }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideAssociatedRoleObjectType() {
            return false;
        }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideAssociatedRoleObjectId() {
            return false;
        }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideOwner() {
            return false;
        }
    }
}