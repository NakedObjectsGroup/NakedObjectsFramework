// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    ///     An INakedObjectAdapter is an adapter to domain objects. The NOF alsways deals with domain objects via these
    ///     adapters. The adapter gives access to the MetamodelManager (INakedObjectSpecification) for the domain object type,
    ///     provides a unique identifier for the object (Oid), and its current 'lifecycle' state.
    /// </summary>
    public interface INakedObjectAdapter {
        /// <summary>
        ///     Returns the adapted domain object, the POCO, that this adapter represents with the NOF
        /// </summary>
        object Object { get; }

        /// <summary>
        ///     Returns the specification that details the structure of this object's adapted domain object.
        ///     Specifically, this specification provides the mechanism to access and manipulate the domain object.
        /// </summary>
        ITypeSpec Spec { get; }

        /// <summary>
        ///     The object's unique id. This id allows the object to added to, stored by, and retrieved from the object
        ///     store
        /// </summary>
        IOid Oid { get; }

        /// <summary>
        ///     Determines what 'lazy loaded' state the domain object is in
        /// </summary>
        IResolveStateMachine ResolveState { get; }

        /// <summary>
        ///     Returns the current version of the domain object
        /// </summary>
        IVersion Version { get; }

        /// <summary>
        ///     Sets the versions of the domain object
        /// </summary>
        IVersion OptimisticLock { set; }

        /// <summary>
        ///     Returns a list in priority order of names of icons to use if this object is to be displayed graphically
        /// </summary>
        /// <para>
        ///     Should always return at least one item
        /// </para>
        string IconName();

        /// <summary>
        ///     Returns the title to display this object with, which is usually got from the
        ///     wrapped <see cref="Object" /> domain object
        /// </summary>
        string TitleString();

        /// <summary>
        ///     Returns a local independent string for this object 
        /// </summary>
        string InvariantString();

        /// <summary>
        ///     Checks the version of this adapter to make sure that it does not differ from the specified
        ///     version
        /// </summary>
        void CheckLock(IVersion otherVersion);

        /// <summary>
        ///     Sometimes it is necessary to manage the replacement of the underlying domain object (by another
        ///     component such as an object store). This method allows the adapter to be kept while the domain object
        ///     is replaced
        /// </summary>
        void ReplacePoco(object poco);

        /// <summary>
        ///     Checks that a transient object is in a valid state to be persisted. Returns reason that it cannot be persisted, or null if it can be persisted.
        /// </summary>
        string ValidToPersist();

        /// <summary>
        ///     Sets the oid if the oid is currently null and new new oid is transient. Used to make the oid a place
        ///     holder for custom mmechanisms to recover the object.
        /// </summary>
        /// <param name="oid"></param>
        void SetATransientOid(IOid oid);

        void LoadAnyComplexTypes();
        void Created();
        void Deleting();
        void Deleted();
        void Loading();
        void Loaded();
        void Persisting();
        void Persisted();
        void Updating();
        void Updated();
    }

    // Copyright (c) Naked Objects Group Ltd.
}