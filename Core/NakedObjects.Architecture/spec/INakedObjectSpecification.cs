// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Spec {
    public interface INakedObjectSpecification : IActionContainer,
                                                 IPropertyContainer,
                                                 IFacetHolder,
                                                 IHierarchical,
                                                 IDefaultProvider {
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
        bool IsService { get; }
        bool HasNoIdentity { get; }

        bool IsQueryable { get; }

        bool IsVoid { get; }

        /// <summary>
        ///     Determines if objects of this specification can be persisted or not. If it can be persisted (i.e. it
        ///     returns something other than <see cref="NakedObjects.Architecture.Adapter.Persistable.TRANSIENT" /> then
        ///     <see cref="IDomainObjectContainer.IsPersistent" /> will indicate whether the object is persistent or not.
        /// </summary>
        Persistable Persistable { get; }

        bool IsASet { get; }
        bool IsViewModel { get; }

        /// <summary>
        ///     Returns the name of an icon to use for the specified object
        /// </summary>
        string GetIconName(INakedObject forObject);

        /// <summary>
        ///     Returns the title string for the specified object
        /// </summary>
        string GetTitle(INakedObject nakedObject, INakedObjectManager manager);

        /// <summary>
        ///     Determines whether the specified object can be persisted, that is, it is in a valid state to be saved
        /// </summary>
        IConsent ValidToPersist(INakedObject transientObject, ISession session);

        //object CreateObject(INakedObjectPersistor persistor);

        IEnumerable GetBoundedSet(INakedObjectPersistor persistor);

        void MarkAsService();

        string GetInvariantString(INakedObject nakedObject);
        string UniqueShortName(string sep);
    }

    // Copyright (c) Naked Objects Group Ltd.
}