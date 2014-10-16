// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class DotNetArrayFacet : DotNetCollectionFacet {
        public DotNetArrayFacet(ISpecification holder)
            : base(holder) {}

        public DotNetArrayFacet(ISpecification holder, Type elementType)
            : base(holder, elementType) {}


        public override void Init(INakedObject collection, INakedObject[] initData) {
            Array newCollection = Array.CreateInstance(collection.GetDomainObject().GetType().GetElementType(), initData.Length);
            collection.ReplacePoco(newCollection);

            int i = 0;
            foreach (INakedObject nakedObject in initData) {
                AsCollection(collection)[i++] = nakedObject.Object;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}