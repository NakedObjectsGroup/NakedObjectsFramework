// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using NakedObjects.Services;

namespace NakedObjects {
    public class PolymorphicLink<TRole, TOwner> : IPolymorphicLink<TRole, TOwner>
        where TRole : class, IHasIntegerId
        where TOwner : class, IHasIntegerId {
        #region Injected Services

        public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        #endregion

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        #region IPolymorphicLink<TRole,TOwner> Members

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

        #endregion

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