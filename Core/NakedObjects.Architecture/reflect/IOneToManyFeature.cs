// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Base interface for <see cref="IOneToManyAssociation" /> only.
    /// </summary>
    /// <para>
    ///     Introduced for symmetry with <see cref="IOneToOneFeature" />; if we ever
    ///     support collections as parameters then would also be the base
    ///     interface for a <c>IOneToManyActionParameter</c>.
    /// </para>
    /// <para>
    ///     Is also the route upto the <see cref="INakedObjectFeature" /> superinterface.
    /// </para>
    public interface IOneToManyFeature : INakedObjectFeature {}
}