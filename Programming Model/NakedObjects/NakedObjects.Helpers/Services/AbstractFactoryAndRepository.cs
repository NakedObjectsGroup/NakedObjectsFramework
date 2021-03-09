// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedFramework;
using NakedFramework.Resources;

namespace NakedObjects.Services {
    /// <summary>
    ///     Convenience super class for factories and repositories that wish to interact with the container
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Subclassing is NOT mandatory; the methods in this superclass can be pushed down into domain objects and
    ///         another superclass used if required
    ///     </para>
    /// </remarks>
    /// <see cref="IDomainObjectContainer" />
    public abstract class AbstractFactoryAndRepository {
        /// <summary>
        ///     Unique identifier for this service
        /// </summary>
        [Hidden(WhenTo.Always)]
        public virtual string Id => GetType().Name;

        /// <summary>
        ///     Fully qualified type name for this service
        /// </summary>
        protected string ClassName => GetType().FullName;

        /// <summary>
        ///     Returns the first item from an IQueryable, but warns user
        ///     'No matching object found' if the IQueryable has a Count of 0.
        /// </summary>
        protected T SingleObjectWarnIfNoMatch<T>(IQueryable<T> query) {
            if (!query.Any()) {
                WarnUser(ProgrammingModel.NoMatchSingular);
                return default;
            }

            return query.First();
        }

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T
        /// </summary>
        protected T Random<T>() where T : class {
            var query = Instances<T>();
            var random = new Random().Next(query.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return query.OrderBy(n => "").Skip(random).FirstOrDefault();
        }

        protected IQueryable FindByTitleAndType(Type type, string partialTitleString) {
            var m = GetType().GetMethod("FindByTitle", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type);
            return (IQueryable) m.Invoke(this, new object[] {partialTitleString});
        }

        #region Container Helper Methods

        // This field is not persisted, nor displayed to the user.
        public IDomainObjectContainer Container { protected get; set; }

        /// <summary>
        ///     Get details about the current user
        /// </summary>
        protected IPrincipal Principal => Container.Principal;

        /// <summary>
        ///     Ensure that the this object is completely loaded into memory
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This forces the lazy loading mechanism to load this object if it is not already loaded
        ///         obj is a hint that this object needs to be loaded (eg if it is null).
        ///     </para>
        /// </remarks>
        protected void Resolve(object obj) {
            Container.Resolve(this, obj);
        }

        /// <summary>
        ///     Flags that this object's state has changed and its changes need to be saved
        /// </summary>
        protected void ObjectChanged() {
            Container.ObjectChanged(this);
        }

        /// <summary>
        ///     Create a new instance of T, but do not persist it
        /// </summary>
        /// <seealso cref="Persist{T}" />
        protected T NewTransientInstance<T>() where T : new() => Container.NewTransientInstance<T>();

        /// <summary>
        ///     Create a new instance of type, but do not persist it
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Use when Type is not known at compile time
        ///     </para>
        /// </remarks>
        /// <seealso cref="Persist{T}" />
        protected object NewTransientInstance(Type type) => Container.NewTransientInstance(type);

        /// <summary>
        ///     Determines if the specified object is persistent; that is is stored permanently outside of the virtual machine.
        /// </summary>
        protected bool IsPersistent(object obj) => Container.IsPersistent(obj);

        /// <summary>
        ///     Make the specified transient object persistent. Throws an exception if object is already persistent.
        /// </summary>
        protected void Persist<T>(ref T transientObject) {
            Container.Persist(ref transientObject);
        }

        /// <summary>
        ///     Delete the object from the persistent object store
        /// </summary>
        protected void DisposeInstance(object persistentObject) {
            Container.DisposeInstance(persistentObject);
        }

        /// <summary>
        ///     Make the specified message available to the user. This will probably be displayed in transitory
        ///     fashion, so is only suitable for useful but optional information..
        /// </summary>
        /// <seealso cref="WarnUser" />
        /// <seealso cref="RaiseError" />
        protected void InformUser(string message) {
            Container.InformUser(message);
        }

        /// <summary>
        ///     Warn the user about a situation with the specified message. The container should guarantee to display
        ///     this warning to the user.
        /// </summary>
        /// <seealso cref="InformUser" />
        /// <seealso cref="RaiseError" />
        protected void WarnUser(string message) {
            Container.WarnUser(message);
        }

        /// <summary>
        ///     Notify the user of an application error with the specified message. This will probably be
        ///     displayed in an alarming fashion, so is only suitable for errors
        /// </summary>
        /// <seealso cref="InformUser" />
        /// <seealso cref="WarnUser" />
        protected void RaiseError(string message) {
            Container.RaiseError(message);
        }

        /// <summary>
        ///     Instances of T as starting point for LINQ query
        /// </summary>
        /// <seealso cref="Instances" />
        protected IQueryable<T> Instances<T>() where T : class => Container.Instances<T>();

        /// <summary>
        ///     Instances of Type as starting point for LINQ query. Use when Type is not known at compile time
        /// </summary>
        /// <seealso cref="Instances{T}" />
        protected IQueryable Instances(Type type) => Container.Instances(type);

        #endregion
    }
}