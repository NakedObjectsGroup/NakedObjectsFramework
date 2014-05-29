// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Provides reflective access to a field on a domain object
    /// </summary>
    public interface INakedObjectAssociation : INakedObjectMember {
        /// <summary>
        ///     If true then can cast to a <see cref="IOneToOneAssociation" />
        /// </summary>
        /// <para>
        ///     Either this or <see cref="IsCollection" /> will be true
        /// </para>
        bool IsObject { get; }

        /// <summary>
        ///     If true then can cast to a <see cref="IOneToManyAssociation" />
        /// </summary>
        /// <para>
        ///     Either this or <see cref="IsObject" /> will be true
        /// </para>
        bool IsCollection { get; }

        /// <summary>
        ///     If true then can cast to a <see cref="IOneToManyAssociation" />
        /// </summary>
        /// and in addition the collection has set semantics
        /// <see cref="ISet{T}" />
        /// <para>
        ///     Either this or <see cref="IsObject" /> will be true
        /// </para>
        bool IsASet { get; }

        /// <summary>
        ///     Returns true if this field is persisted, and not calculated from other data in the object or
        ///     used transiently
        /// </summary>
        bool IsPersisted { get; }

        /// <summary>
        ///     Determines if this field must be complete before the object is in a valid state
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        ///     Returns true is this property cannot be externally set
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Returns <c>true</c> if this field on the specified object is inlined
        /// </summary>
        bool IsInline { get; }

        /// <summary>
        ///     Returns the <see cref="INakedObject" /> adapting this field's object. This invokes the specified
        ///     instances accessor method and creates an adapter for the returned value
        /// </summary>
        INakedObject GetNakedObject(INakedObject target);

        /// <summary>
        ///     Return the default for this property
        /// </summary>
        INakedObject GetDefault(INakedObject nakedObject);

        /// <summary>
        ///     Return the default for this property
        /// </summary>
        TypeOfDefaultValue GetDefaultType(INakedObject nakedObject);

        /// <summary>
        ///     Set the property to its default references or values
        /// </summary>
        void ToDefault(INakedObject target);

        /// <summary>
        ///     Returns <c>true</c> if this field on the specified object is deemed to be empty, or has no content
        /// </summary>
        bool IsEmpty(INakedObject target);
    }

    // Copyright (c) Naked Objects Group Ltd.
}