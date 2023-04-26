﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using NakedObjects.Services;

namespace NakedObjects; 

public class PolymorphicLink<TRole, TOwner> : IPolymorphicLink<TRole, TOwner>
    where TRole : class, IHasIntegerId
    where TOwner : class, IHasIntegerId {
    #region Injected Services

    public PolymorphicNavigator PolymorphicNavigator { set; protected get; }

    #endregion

    [NakedObjectsIgnore]
    public virtual int Id { get; set; }

    /// <summary>
    ///     To allow sub-classes to render property visible/invisible
    /// </summary>
    public virtual bool HideAssociatedRoleObjectType() => false;

    /// <summary>
    ///     To allow sub-classes to render property visible/invisible
    /// </summary>
    public virtual bool HideAssociatedRoleObjectId() => false;

    /// <summary>
    ///     To allow sub-classes to render property visible/invisible
    /// </summary>
    public virtual bool HideOwner() => false;

    #region IPolymorphicLink<TRole,TOwner> Members

    [Disabled]
    public virtual string AssociatedRoleObjectType { get; set; }

    [Disabled]
    public virtual int AssociatedRoleObjectId { get; set; }

#pragma warning disable CS0618
    [NotPersisted]
#pragma warning restore CS0618
    [Disabled]
    public virtual TRole AssociatedRoleObject {
        get => PolymorphicNavigator.FindObject<TRole>(AssociatedRoleObjectType, AssociatedRoleObjectId);
        set {
            AssociatedRoleObjectType = PolymorphicNavigator.GetType(value);
            AssociatedRoleObjectId = value.Id;
        }
    }

    [Required]
    public virtual TOwner Owner { get; set; }

    #endregion
}