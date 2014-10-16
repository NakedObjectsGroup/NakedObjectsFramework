// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.NotPersistable {
    public class ProgramPersistableOnlyFacetAnnotation : ProgramPersistableOnlyFacetAbstract {
        public ProgramPersistableOnlyFacetAnnotation(ISpecification holder)
            : base(holder) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}