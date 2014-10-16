// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propcoll.NotCounted;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.NotCounted {

    /// <summary>
    ///     This is only used at by the custom 'SdmNotCountedAttribute' 
    /// </summary>
    public class NotCountedFacetAnnotation : NotCountedFacetAbstract {
        public NotCountedFacetAnnotation(ISpecification holder)
            : base(holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}