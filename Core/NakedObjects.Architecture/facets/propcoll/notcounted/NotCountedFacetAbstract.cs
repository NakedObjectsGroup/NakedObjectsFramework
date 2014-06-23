// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Propcoll.NotCounted {
    /// <summary>
    ///     This is only used by the custom 'SdmNotCountedAttribute'
    /// </summary>
    public abstract class NotCountedFacetAbstract : MarkerFacetAbstract, INotCountedFacet {
        protected NotCountedFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (INotCountedFacet); }
        }
    }
}