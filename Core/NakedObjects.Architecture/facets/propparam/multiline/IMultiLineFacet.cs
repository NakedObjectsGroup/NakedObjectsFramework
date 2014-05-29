// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propparam.MultiLine {
    /// <summary>
    ///     Whether the (string) property or parameter should be rendered over multiple lines
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to the <see cref="MultiLineAttribute" /> annotation
    /// </para>
    public interface IMultiLineFacet : IMultipleValueFacet {
        /// <summary>
        ///     How many lines to use
        /// </summary>
        int NumberOfLines { get; }

        /// <summary>
        ///     Width of each line before wrapping
        /// </summary>
        int Width { get; }
    }
}