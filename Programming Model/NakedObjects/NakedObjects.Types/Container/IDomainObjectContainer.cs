// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;

namespace NakedObjects {
    /// <summary>
    ///     In a typical Naked Objects application this provides the sole point of contact between the domain model
    ///     and the framework. An implementation of this context may be injected into any domain object or service.
    /// </summary>
    public interface IDomainObjectContainer {
        /// <summary>
        ///     Get details about the current user
        /// </summary>
        IPrincipal Principal { get; }

        /// <summary>
        ///     Refresh the object from the  underlying data store
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This forces a refresh of the object from the underlying data store and does an ObjectChanged to refresh the
        ///         view
        ///     </para>
        /// </remarks>
        void Refresh(object obj);

        /// <summary>
        ///     Ensure that the specified object is completely loaded into memory
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This forces the lazy loading mechanism to load the object if it is not already loaded
        ///     </para>
        /// </remarks>
        void Resolve(object obj);

        /// <summary>
        ///     Ensure that the specified object is completely loaded into memory, though only if the supplied field
        ///     reference is <c>null</c>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This forces the lazy loading mechanism to load the object if it is not already loaded
        ///     </para>
        /// </remarks>
        void Resolve(object obj, object field);

        /// <summary>
        ///     Flags that the specified object's state has changed and its changes need to be saved
        /// </summary>
        void ObjectChanged(object obj);

        /// <summary>
        ///     Create a new instance of T, but do not persist it
        /// </summary>
        T NewTransientInstance<T>() where T : new();

        /// <summary>
        ///     Create a new instance of T, wher T is a View Model
        /// </summary>
        T NewViewModel<T>() where T : IViewModel, new();

        /// <summary>
        ///     Create a new instance of type, where type is a View Model
        /// </summary>
        IViewModel NewViewModel(Type type);

        /// <summary>
        ///     Create a new instance of type, but do not persist it
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Use when Type is not known at compile time
        ///     </para>
        /// </remarks>
        object NewTransientInstance(Type type);

        /// <summary>
        ///     Determines if the specified object is persistent; that is is stored permanently outside of the virtual machine.
        /// </summary>
        bool IsPersistent(object obj);

        /// <summary>
        ///     Make the specified transient object persistent. Throws an exception if object is already persistent.
        /// </summary>
        void Persist<T>(ref T transientObject);

        /// <summary>
        ///     Delete the object from the persistent object store
        /// </summary>
        void DisposeInstance(object persistentObject);

        /// <summary>
        ///     Make the specified message available to the user. This will probably be displayed in transitory
        ///     fashion, so is only suitable for useful but optional information..
        /// </summary>
        /// <seealso cref="WarnUser" />
        /// <seealso cref="RaiseError" />
        void InformUser(string message);

        /// <summary>
        ///     Warn the user about a situation with the specified message. The context should guarantee to display
        ///     this warning to the user.
        /// </summary>
        /// <seealso cref="InformUser" />
        /// <seealso cref="RaiseError" />
        void WarnUser(string message);

        /// <summary>
        ///     Notify the user of an application error with the specified message. This will probably be
        ///     displayed in an alarming fashion, so is only suitable for errors
        /// </summary>
        /// <seealso cref="InformUser" />
        /// <seealso cref="WarnUser" />
        void RaiseError(string message);

        /// <summary>
        ///     Instances of T as starting point for LINQ query
        /// </summary>
        /// <seealso cref="Instances" />
        IQueryable<T> Instances<T>() where T : class;

        /// <summary>
        ///     Instances of Type as starting point for LINQ query. Use when Type is not known at compile time
        /// </summary>
        /// <seealso cref="Instances{T}" />
        IQueryable Instances(Type type);

        /// <summary>
        ///     Abort the current transaction
        /// </summary>
        void AbortCurrentTransaction();

        /// <summary>
        /// Get a service by its type. Return null if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>();

        #region Titles

        /// <summary>
        /// Convenience method to get hold of the title of any domain object,
        /// without needing to know whether that is defined by a Title() method,
        /// ToString() method, or a [Title] attribute on a property.
        /// </summary>
        string TitleOf(object obj, string format = null);

        /// <summary>
        /// Returns a framework-generated implementation of ITitleBuilder, which
        /// </summary>
        ITitleBuilder NewTitleBuilder();

        /// <summary>
        /// Creates a new ITitleBuilder object, 
        /// containing the Title of the specified object with an optional default
        /// title if the object has no specified title.
        /// </summary>
        ITitleBuilder NewTitleBuilder(object obj, string defaultTitle = null);

        /// <summary>
        ///     Creates a new ITitleBuilder object, containing the specified text
        /// </summary>
        ITitleBuilder NewTitleBuilder(string text);

        #endregion
    }
}