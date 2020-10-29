// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class ViewModelFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        public ViewModelFacetFactory(IFacetFactoryOrder<ViewModelFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            IFacet facet = null;

            if (!type.IsInterface && typeof(IViewModel).IsAssignableFrom(type)) {
                var deriveMethod = type.GetMethod("DeriveKeys", new Type[] { });
                var populateMethod = type.GetMethod("PopulateUsingKeys", new[] {typeof(string[])});

                var toRemove = new List<MethodInfo> {deriveMethod, populateMethod};

                if (typeof(IViewModelEdit).IsAssignableFrom(type)) {
                    facet = new ViewModelEditFacetConvention(specification);
                }
                else if (typeof(IViewModelSwitchable).IsAssignableFrom(type)) {
                    var isEditViewMethod = type.GetMethod("IsEditView");
                    toRemove.Add(isEditViewMethod);
                    facet = new ViewModelSwitchableFacetConvention(specification);
                }
                else {
                    facet = new ViewModelFacetConvention(specification);
                }

                methodRemover.RemoveMethods(toRemove.ToArray());
            }

            FacetUtils.AddFacet(facet);

            return metamodel;
        }
    }
}