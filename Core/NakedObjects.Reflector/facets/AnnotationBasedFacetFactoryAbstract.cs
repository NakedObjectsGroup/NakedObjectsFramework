// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class AnnotationBasedFacetFactoryAbstract : FacetFactoryAbstract, IAnnotationBasedFacetFactory {
        protected AnnotationBasedFacetFactoryAbstract(INakedObjectReflector reflector, FeatureType[] featureTypes)
            : base(reflector, featureTypes) {}

        /// <summary>
        ///     Always returns <c>false</c> as <see cref="IFacetFactory" />s that look for annotations
        ///     won't recognize methods with prefixes.
        /// </summary>
        public bool Recognizes(MethodInfo method) {
            return false;
        }
    }
}