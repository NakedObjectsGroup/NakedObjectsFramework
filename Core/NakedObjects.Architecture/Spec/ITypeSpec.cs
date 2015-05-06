// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    /// Gives access to the full specification (metadata) for a specific domain object type 
    /// (defined by the FullName property). This is the 'runtime specification', which can provide
    /// services in relation to a specific object; where possible its responsibilities are delegated 
    /// to the static version of the specification: IObjectSpecImmutable.
    /// </summary>
    public interface ITypeSpec : ISpecification {
        /// <summary>
        ///     Determines if objects of this type can be set up from a text entry string.
        /// </summary>
        /// <para>
        ///     In effect, means this has got a <see cref="IParseableFacet" />
        /// </para>
        /// >
        bool IsParseable { get; }

        /// <summary>
        ///     Determines if objects of this type can be converted to a data-stream
        /// </summary>
        /// <para>
        ///     In effect, means has got <see cref="IEncodeableFacet" />
        /// </para>
        bool IsEncodeable { get; }

        /// <summary>
        ///     Determines if objects of this type are aggregated
        /// </summary>
        bool IsAggregated { get; }

        /// <summary>
        ///     Determines if object represents a collection
        /// </summary>
        /// <para>
        ///     In effect, means has got a <see cref="ICollectionFacet" />, and
        ///     therefore will return !<see cref="IsObject" />
        /// </para>
        /// <seealso cref="IsObject" />
        bool IsCollection { get; }

        /// <summary>
        ///     Determines if the object represents an object (value or otherwise).
        /// </summary>
        /// <para>
        ///     In effect, means that it doesn't have a <see cref="ICollectionFacet" />, and
        ///     therefore will return !<see cref="IsCollection" />
        /// </para>
        /// <seealso cref="IsCollection" />
        bool IsObject { get; }

        bool IsAbstract { get; }
        bool IsInterface { get; }
        bool HasNoIdentity { get; }
        bool IsQueryable { get; }
        bool IsVoid { get; }

        /// <summary>
        ///     Determines if objects of this specification can be persisted or not. If it can be persisted (i.e. it
        ///     returns something other than <see cref="PersistableType.Transient" /> then
        ///     <see cref="IDomainObjectContainer.IsPersistent" /> will indicate whether the object is persistent or not.
        /// </summary>
        PersistableType Persistable { get; }

        bool IsASet { get; }
        bool IsViewModel { get; }

        #region Default Provider

        /// <summary>
        ///     Default value to be provided for properties or parameters that are not declared as
        ///     <see cref="OptionallyAttribute" /> but where the UI has not (yet) provided a value.
        /// </summary>
        object DefaultValue { get; }

        #endregion

        /// <summary>
        ///     Returns the name of an icon to use for the specified object
        /// </summary>
        string GetIconName(INakedObjectAdapter forObjectAdapter);

        /// <summary>
        ///     Returns the title string for the specified object
        /// </summary>
        string GetTitle(INakedObjectAdapter nakedObjectAdapter);

        string GetInvariantString(INakedObjectAdapter nakedObjectAdapter);

        #region Name & Description

        /// <summary>
        ///     Returns the name of this specification. This will be the fully qualified name of the Class object that
        ///     this object represents (i.e. it includes the full namespace).
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///     Returns the plural name for objects of this specification
        /// </summary>
        string PluralName { get; }

        /// <summary>
        ///     Returns the class name without the namespace. Removes the text up to, and including the last
        ///     period (".")
        /// </summary>
        string ShortName { get; }

        /// <summary>
        ///     Returns the description, if any, of the specification
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Returns the singular name for objects of this specification
        /// </summary>
        string SingularName { get; }

        /// <summary>
        ///     Returns the singular name for objects of this specification
        /// </summary>
        string UntitledName { get; }

        #endregion

        #region Inheritance Hierarchy

        /// <summary>
        ///     Returns true if the <see cref="Subclasses" /> method will return an array of one or more elements (ie,
        ///     not an empty array).
        /// </summary>
        bool HasSubclasses { get; }

        /// <summary>
        ///     Get the list of specifications for all the interfaces that the class represented by this specification
        ///     implements.
        /// </summary>
        ITypeSpec[] Interfaces { get; }

        /// <summary>
        ///     Get the list of specifications for the subclasses of the class represented by this specification
        /// </summary>
        ITypeSpec[] Subclasses { get; }

        /// <summary>
        ///     Get the specification for this specification's class's superclass
        /// </summary>
        ITypeSpec Superclass { get; }

        /// <summary>
        ///     Determines if this specification represents the same specification, or a subclass, of the specified
        ///     specification.
        /// </summary>
        bool IsOfType(ITypeSpec spec);

        #endregion

        #region Actions

        IMenuImmutable Menu { get; }
        ITypeSpecImmutable InnerSpec { get; }

        /// <summary>
        ///     Returns an array of actions (i.e. native actions and object-contributedActions)
        /// </summary>
        IActionSpec[] GetActions();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}