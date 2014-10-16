// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Simply installs a <see cref="MandatoryFacetDefault" /> onto all properties and parameters.
    /// </summary>
    /// <para>
    ///     The idea is that this <see cref="IFacetFactory" /> is included early on in the
    ///     <see cref="FacetFactorySetImpl" />, but other <see cref="IMandatoryFacet" /> implementations
    ///     which don't require mandatory semantics will potentially replace these where the
    ///     property or parameter is annotated or otherwise indicated as being optional.
    /// </para>
    public class MandatoryDefaultFacetFactory : FacetFactoryAbstract {
        public MandatoryDefaultFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.PropertiesAndParameters) { }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(specification));
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            return FacetUtils.AddFacet(Create(holder));
        }

        private static IMandatoryFacet Create(ISpecification holder) {
            return new MandatoryFacetDefault(holder);
        }
    }
}