// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    public class ViewModelFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ViewModelFacetFactory(INakedObjectReflector reflector) : base(reflector, FeatureType.ObjectsOnly) {}

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