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
            : base(reflector, NakedObjectFeatureType.PropertiesOnly) {}

        public override bool Process(PropertyInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        private static IPropertyValidateFacet Create(IFacetHolder holder) {
            return new PropertyValidateFacetDefault(holder);
        }
    }
}