// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Validate;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Validate {
    public class PropertyValidateDefaultFacetFactory : FacetFactoryAbstract {
        public PropertyValidateDefaultFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.PropertiesOnly) {}

        public override bool Process(PropertyInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        private static IPropertyValidateFacet Create(ISpecification holder) {
            return new PropertyValidateFacetDefault(holder);
        }
    }
}