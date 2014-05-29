// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Ordering.MemberOrder {
    /// <summary>
    ///     (One of the) mechanism(s) for determining the order in which the actions of the object should be rendered
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to the
    ///     <c>ActionOrder</c> method which returns a comma-separated list of action names.  An
    ///     alternative (and preferred, because it is refactoring-safe) mechanism is to annotate each of the methods using
    ///     <see cref="MemberOrderAttribute" />
    /// </para>
    /// <seealso cref="IMemberOrderFacet" />
    /// <seealso cref="IFieldOrderFacet" />
    public interface IActionOrderFacet : ISingleStringValueFacet {}
}