// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.TypicalLength {
    /// <summary>
    ///     The typical length of a property or a parameter
    /// </summary>
    /// <para>
    ///     Intended to be used by the viewer as a rendering hint to size the
    ///     UI field to an appropriate size
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <see cref="TypicalLengthAttribute" /> annotation
    /// </para>
    public interface ITypicalLengthFacet : ISingleIntValueFacet {}
}