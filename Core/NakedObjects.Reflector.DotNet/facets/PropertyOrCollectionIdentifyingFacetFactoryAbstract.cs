// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class PropertyOrCollectionIdentifyingFacetFactoryAbstract : MethodPrefixBasedFacetFactoryAbstract, IPropertyOrCollectionIdentifyingFacetFactory {
        protected PropertyOrCollectionIdentifyingFacetFactoryAbstract(NakedObjectFeatureType[] featureTypes)
            : base(featureTypes) {}

        protected static bool IsCollectionOrArray(Type type) {
            return CollectionUtils.IsCollection(type);
        }
    }
}