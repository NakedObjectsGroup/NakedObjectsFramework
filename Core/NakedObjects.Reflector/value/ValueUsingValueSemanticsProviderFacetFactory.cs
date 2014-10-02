// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public abstract class ValueUsingValueSemanticsProviderFacetFactory<T> : FacetFactoryAbstract {
        protected ValueUsingValueSemanticsProviderFacetFactory(IMetadata metadata, Type adapterFacetType)
            : base(metadata, NakedObjectFeatureType.ObjectsOnly) {}

        protected void AddFacets(ValueSemanticsProviderAbstract<T> adapter) {
            FacetUtils.AddFacet(new ValueFacetUsingSemanticsProvider<T>(adapter, adapter));
        }
    }
}