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
            :base(reflector, NakedObjectFeatureType.PropertiesAndParameters) { }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return AddFacetDerivedFromTypeIfPresent(specification, property.PropertyType);
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            Type type = method.ReturnType;
            return AddFacetDerivedFromTypeIfPresent(specification, type);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            return AddFacetDerivedFromTypeIfPresent(holder, parameter.ParameterType);
        }

        private bool AddFacetDerivedFromTypeIfPresent(ISpecification holder, Type type) {
            ITypicalLengthFacet facet = GetTypicalLengthFacet(type);
            if (facet != null) {
                return FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }
            return false;
        }

        private ITypicalLengthFacet GetTypicalLengthFacet(Type type) {
            var paramTypeSpec = Reflector.LoadSpecification(type);
            return paramTypeSpec.GetFacet<ITypicalLengthFacet>();
        }
    }
}