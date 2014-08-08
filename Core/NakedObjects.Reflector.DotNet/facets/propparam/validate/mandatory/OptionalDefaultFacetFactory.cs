// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Installs a <see cref="OptionalFacetDefault" /> onto all properties and parameters if they are non primitive
    ///     and readable.
    /// </summary>
    /// <para>
    ///     This is an alternative to <see cref="MandatoryDefaultFacetFactory" />. Which works the same way but
    ///     makes everything optional by default. Requiring the use of annotations to indicate mandatoryness.
    /// </para>
    public class OptionalDefaultFacetFactory : FacetFactoryAbstract {
        public OptionalDefaultFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.PropertiesAndParameters) {}

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacet(method.ReturnType.IsPrimitive ? CreateMandatory(holder) : CreateOptional(holder));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacet(property.PropertyType.IsPrimitive || property.GetGetMethod() == null ? CreateMandatory(holder) : CreateOptional(holder));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            return FacetUtils.AddFacet(parameter.ParameterType.IsPrimitive ? CreateMandatory(holder) : CreateOptional(holder));
        }

        private static IMandatoryFacet CreateOptional(IFacetHolder holder) {
            return new OptionalFacetDefault(holder);
        }

        private static IMandatoryFacet CreateMandatory(IFacetHolder holder) {
            return new MandatoryFacetDefault(holder);
        }
    }
}