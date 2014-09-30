// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof {
    public class TypeOfFacetDefaultToObject : TypeOfFacetAbstract {
        public TypeOfFacetDefaultToObject(IFacetHolder holder, INakedObjectReflector reflector)
            : base(typeof (object), true, holder, reflector) {}
    }
}