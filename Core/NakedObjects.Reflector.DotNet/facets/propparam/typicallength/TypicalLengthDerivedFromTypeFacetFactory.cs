// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets;

namespace NakedObjects.Architecture.Facets.Propparam.TypicalLength {
    public class TypicalLengthDerivedFromTypeFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypicalLengthDerivedFromTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.PropertiesAndParameters) { }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return AddFacetDerivedFromTypeIfPresent(holder, property.PropertyType);
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            Type type = method.ReturnType;
            return AddFacetDerivedFromTypeIfPresent(holder, type);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            return AddFacetDerivedFromTypeIfPresent(holder, parameter.ParameterType);
        }

        private bool AddFacetDerivedFromTypeIfPresent(IFacetHolder holder, Type type) {
            ITypicalLengthFacet facet = GetTypicalLengthFacet(type);
            if (facet != null) {
                return FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }
            return false;
        }

        private ITypicalLengthFacet GetTypicalLengthFacet(Type type) {
            INakedObjectSpecification paramTypeSpec = Reflector.LoadSpecification(type);
            return paramTypeSpec.GetFacet<ITypicalLengthFacet>();
        }
    }
}