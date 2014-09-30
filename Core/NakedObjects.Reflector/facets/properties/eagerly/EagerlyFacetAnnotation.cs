// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Eagerly;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Eagerly {
    public class EagerlyFacetAnnotation : FacetAbstract, IEagerlyFacet {
        public EagerlyFacetAnnotation(EagerlyAttribute.Do what, IFacetHolder holder)
            : base(Type, holder) {
            What = what;
        }

        public static Type Type {
            get { return typeof (IEagerlyFacet); }
        }

        public EagerlyAttribute.Do What { get; private set; }
    }
}