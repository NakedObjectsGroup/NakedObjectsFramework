// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Objects.NotPersistable {
    public abstract class ProgramPersistableOnlyFacetAbstract : MarkerFacetAbstract, IProgramPersistableOnlyFacet {
        protected ProgramPersistableOnlyFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IProgramPersistableOnlyFacet); }
        }
    }
}