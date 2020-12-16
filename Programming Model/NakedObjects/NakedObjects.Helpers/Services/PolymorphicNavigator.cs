// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Util;

namespace NakedObjects.Services {
    /// <summary>
    ///     Service that provides helper methods for navigating polymorphic associations.
    ///     Will delegate responsibility for determining the string representation of a type to an
    ///     injected ITypeCodeMapper if one has been registered.  Otherwise uses the default of fully-qualified type name.
    /// </summary>
    public class PolymorphicNavigator {
        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns a queryable of those links' owners.
        /// </summary>
        public virtual IQueryable<TOwner> FindOwners<TLink, TRole, TOwner>(TRole value)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            ThrowExceptionIfIdIsZero(value);
            IQueryable<TLink> links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            return links.Select(x => x.Owner);
        }

        /// <summary>
        ///     Searches all polymorphic links of type TLink to find those associated with the value
        ///     then returns the one (if any) that has the specified owner.  If more than one link
        ///     associates the same value and owner an error is thrown.
        /// </summary>
        public virtual TLink FindPolymorphicLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            ThrowExceptionIfIdIsZero(value);
            int id = value.Id;
            string type = GetType(value);
            int ownerId = owner.Id;
            IQueryable<TLink> links = FindPolymorphicLinks<TLink, TRole, TOwner>(value);
            IQueryable<TLink> matches = links.Where(x => x.Owner.Id == ownerId);
            return matches.SingleOrDefault();
        }

        private IQueryable<TLink> FindPolymorphicLinks<TLink, TRole, TOwner>(TRole value)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            ThrowExceptionIfIdIsZero(value);
            int id = value.Id;
            string type = GetType(value);
            return Container.Instances<TLink>().Where(x => x.AssociatedRoleObjectId == id && x.AssociatedRoleObjectType == type);
        }

        private void ThrowExceptionIfIdIsZero(IHasIntegerId value) {
            if (value != null && value.Id == 0) {
                throw new DomainException("Id value has not yet been assigned to this object. Should be persisted first");
            }
        }

        public virtual TLink AddLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            ThrowExceptionIfIdIsZero(value);
            TLink link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link != null) return null; //item is already associated,  so don't duplicate
            link = NewTransientLink<TLink, TRole, TOwner>(value, owner);
            Container.Persist(ref link);
            return link;
        }

        public virtual void RemoveLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            ThrowExceptionIfIdIsZero(value);
            TLink link = FindPolymorphicLink<TLink, TRole, TOwner>(value, owner);
            if (link == null) return; //item is not associated
            Container.DisposeInstance(link);
        }

        public virtual TLink NewTransientLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            if (value == null) return null;
            ThrowExceptionIfIdIsZero(value);
            var link = Container.NewTransientInstance<TLink>();
            link.Owner = owner;
            link.AssociatedRoleObject = value;
            return link;
        }

        public virtual TLink UpdateAddOrDeleteLink<TLink, TRole, TOwner>(TRole value, TLink link, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>, new()
            where TOwner : class, IHasIntegerId {
            if (link != null) {
                if (value == null) {
                    Container.DisposeInstance(link);
                    return null;
                }

                link.AssociatedRoleObject = value;
                return link;
            }

            ThrowExceptionIfIdIsZero(value);
            if (Container.IsPersistent(owner) && value != null) {
                return AddLink<TLink, TRole, TOwner>(value, owner);
            }

            return null;
        }

        public virtual TRole RoleObjectFromLink<TLink, TRole, TOwner>(ref TRole role, TLink link, TOwner owner)
            where TRole : class, IHasIntegerId
            where TLink : class, IPolymorphicLink<TRole, TOwner>
            where TOwner : class, IHasIntegerId {
            if (role != null) return role;
            if (link == null) return null;
            role = link.AssociatedRoleObject;
            return role;
        }

        /// <summary>
        ///     Safe way to get the type name from a value.
        ///     delegates to the injected ITypeCodeMapper if one exists.  Otherwises returns default of fully-qualified type name.
        /// </summary>
        public virtual string GetType(object value) {
            return CodeFromType(value);
        }

        public virtual T FindObject<T>(string type, int id) where T : class, IHasIntegerId {
            Type sysType = TypeFromCode(type);
            return FindByKey<T>(sysType, id);
        }

        private T FindByKey<T>(Type sysType, object id) {
            if (sysType != null && id != null) {
                MethodInfo m = GetType().GetMethod("FindByKeyGeneric", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo gm = m.MakeGenericMethod(new[] {sysType});
                return (T) gm.Invoke(this, new[] {id});
            }

            return default(T);
        }

        protected virtual object FindByKeyGeneric<TActual>(object id) where TActual : class {
            return Container.FindByKey<TActual>(id);
        }

        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        public ITypeCodeMapper TypeCodeMapper { set; protected get; }

        #endregion

        #region Convert between Type and string representation (code) for Type

        private Type TypeFromCode(string code) {
            if (TypeCodeMapper != null) {
                return TypeCodeMapper.TypeFromCode(code);
            }

            return TypeUtils.GetType(code);
        }

        private string CodeFromType(object obj) {
            Type type = obj.GetType().GetProxiedType();
            if (TypeCodeMapper != null) {
                return TypeCodeMapper.CodeFromType(type);
            }

            return type.FullName;
        }

        #endregion
    }
}