// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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

        [NotPersisted, Disabled]
        public virtual TRole AssociatedRoleObject {
            get { return ObjectFinder.FindObject<TRole>(RoleObjectOid); }
            set { RoleObjectOid = ObjectFinder.GetCompoundKey(value); }
        }

        #region IPolymorphicLinkWithOid<TRole,TOwner> Members

        [Disabled]
        public virtual string RoleObjectOid { get; set; }

        [Required]
        public virtual TOwner Owner { get; set; }

        #endregion

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideRoleObjectOid() {
            return false;
        }

        /// <summary>
        ///     To allow sub-classes to render property visible/invisible
        /// </summary>
        public virtual bool HideAssociatedRoleObject() {
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