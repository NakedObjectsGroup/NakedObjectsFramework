// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    public class ViewModelFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ViewModelFacetFactory(INakedObjectReflector reflector) :base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            IFacet facet = null;

            if (!type.IsInterface && typeof (IViewModel).IsAssignableFrom(type)) {
                MethodInfo deriveMethod = type.GetMethod("DeriveKeys", new Type[] {});
                MethodInfo populateMethod = type.GetMethod("PopulateUsingKeys", new[] {typeof (string[])});

                var toRemove = new List<MethodInfo> {deriveMethod, populateMethod};

                if (typeof (IViewModelEdit).IsAssignableFrom(type)) {
                    facet = new ViewModelEditFacetConvention(specification);
                }
                else if (typeof (IViewModelSwitchable).IsAssignableFrom(type)) {
                    MethodInfo isEditViewMethod = type.GetMethod("IsEditView");
                    toRemove.Add(isEditViewMethod);
                    facet = new ViewModelSwitchableFacetConvention(specification);
                }
                else {
                    facet = new ViewModelFacetConvention(specification);
                }
                methodRemover.RemoveMethods(toRemove.ToArray());
            }

            return FacetUtils.AddFacet(facet);
        }
    }
}