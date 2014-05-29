// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets {
    public interface ISingleClassValueFacet : IFacet {
        Type Value { get; }

        /// <summary>
        ///     Convenience to return the <see cref="INakedObjectSpecification" /> corresponding to this facet's
        ///     <see cref="Value" />
        /// </summary>
        INakedObjectSpecification ValueSpec { get; }
    }
}