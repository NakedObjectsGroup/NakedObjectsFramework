// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Reflect {
    /// <summary>
    ///     Pulls together all the various strategy and configuration parameters
    ///     that influence what is built
    /// </summary>
    /// <para>
    ///     Assembled by <see cref="DotNetReflector" />, passed onto <see cref="NakedObjectSpecification" /> and
    ///     (in turn) <see cref="DotNetIntrospector" />.
    /// </para>
    public class IntrospectionControlParameters {
        public IntrospectionControlParameters(IFacetFactorySet facetFactorySet,
                                              IClassStrategy classStrategy) {
            FacetFactorySet = facetFactorySet;
            ClassStrategy = classStrategy;
        }

        public IFacetFactorySet FacetFactorySet { get; private set; }
        public IClassStrategy ClassStrategy { get; private set; }
    }
}