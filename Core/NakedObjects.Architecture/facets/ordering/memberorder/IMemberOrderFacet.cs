// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Ordering.MemberOrder {
    /// <summary>
    ///     The preferred mechanism for determining the order in which the members of the object should
    ///     be rendered
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to annotating each of the
    ///     member methods with the <see cref="MemberOrderAttribute" />.  An alternative approach is to use the
    ///     action <see cref="IActionOrderFacet" /> or field <see cref="IFieldOrderFacet" /> order facets
    /// </para>
    /// <seealso cref="IMemberOrderFacet" />
    /// <seealso cref="IFieldOrderFacet" />
    public interface IMemberOrderFacet : IMultipleValueFacet {
        /// <summary>
        ///     To group members
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The sequence, in dewey-decimal notation
        /// </summary>
        string Sequence { get; }
    }
}