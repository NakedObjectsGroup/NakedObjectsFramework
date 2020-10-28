// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedFunctions.Reflector.Reflect;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.Reflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ViewModelAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<ViewModelAnnotationFacetFactory> logger;

        public ViewModelAnnotationFacetFactory(IFacetFactoryOrder<ViewModelAnnotationFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory,
                                                                                                                                               FeatureType.ObjectsAndInterfaces, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<ViewModelAnnotationFacetFactory>();

        private static bool IsSameType(ParameterInfo pi, Type toMatch) =>
            pi != null &&
            pi.ParameterType == toMatch;

        private static bool IsSameTypeAndReturnType(MethodInfo mi, Type toMatch) {
            var pi = mi.GetParameters().FirstOrDefault();

            return pi != null &&
                   pi.ParameterType == toMatch &&
                   mi.ReturnType == toMatch;
        }

        private MethodInfo GetDeriveMethod(Type type) => FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => m.Name == "DeriveKeys").SingleOrDefault(m => IsSameType(m.GetParameters().FirstOrDefault(), type));

        private MethodInfo GetPopulateMethod(Type type) => FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => m.Name == "PopulateUsingKeys").SingleOrDefault(m => IsSameTypeAndReturnType(m, type));

        private MethodInfo GetIsEditMethod(Type type) => FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => m.Name == "IsEditView").SingleOrDefault(m => IsSameType(m.GetParameters().FirstOrDefault(), type));

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            IFacet facet = null;

            if (type.GetCustomAttribute<ViewModelAttribute>() != null ||
                type.GetCustomAttribute<ViewModelEditAttribute>() != null) {
                var deriveMethod = GetDeriveMethod(type);
                var populateMethod = GetPopulateMethod(type);
                var isEditMethod = GetIsEditMethod(type);

                if (deriveMethod != null && populateMethod != null) {
                    if (type.GetCustomAttribute<ViewModelEditAttribute>() != null) {
                        facet = new ViewModelEditFacetViaFunctionsConvention(specification, deriveMethod, populateMethod);
                    }
                    else if (isEditMethod != null) {
                        facet = new ViewModelSwitchableFacetViaFunctionsConvention(specification, deriveMethod, populateMethod, isEditMethod);
                    }
                    else {
                        facet = new ViewModelFacetViaFunctionsConvention(specification, deriveMethod, populateMethod);
                    }
                }
            }

            FacetUtils.AddFacet(facet);

            return metamodel;
        }
    }
}