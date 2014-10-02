// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof {
    public class TypeOfFacetInferredFromArray : TypeOfFacetAbstract {
        public TypeOfFacetInferredFromArray(Type type, IFacetHolder holder, IMetadata metadata)
            : base(type, true, holder, metadata) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}