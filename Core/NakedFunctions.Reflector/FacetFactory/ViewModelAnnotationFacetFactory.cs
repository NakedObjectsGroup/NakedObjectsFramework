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
using NakedObjects.Core;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ViewModelAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory
    {
        private readonly ILogger<ViewModelAnnotationFacetFactory> logger;

        public ViewModelAnnotationFacetFactory(IFacetFactoryOrder<ViewModelAnnotationFacetFactory> order, ILoggerFactory loggerFactory) : base(order.Order, loggerFactory,
                                                                                                                                               FeatureType.ObjectsAndInterfaces) =>
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

        private static MethodInfo GetMethod(Type type, string name) => type.GetMethods().SingleOrDefault(m => m.Name == name);

        private static MethodInfo GetDeriveMethod(Type type) => GetMethod(type, "DeriveKeys");

        private static MethodInfo GetPopulateMethod(Type type) => GetMethod(type, "PopulateUsingKeys");

        private static MethodInfo GetIsEditMethod(Type type) => GetMethod(type, "IsEditView");

        private static Type GetAndValidateContributedToType(MethodInfo deriveMethod, MethodInfo populateMethod, MethodInfo isEditMethod) {
            var t1 = FunctionalFacetFactoryHelpers.GetContributedToType(deriveMethod);
            var t2 = FunctionalFacetFactoryHelpers.GetContributedToType(populateMethod);
            var t3 = isEditMethod is null ? null : FunctionalFacetFactoryHelpers.GetContributedToType(isEditMethod);

            if (t1 != t2) {
                throw new ReflectionException($"View model functions {deriveMethod.Name} and {populateMethod.Name} on {deriveMethod.DeclaringType} have mismatched types");
            }

            if (t3 is not null && t3 != t1) {
                throw new ReflectionException($"View model function {isEditMethod.Name} on {deriveMethod.DeclaringType} has mismatched types");
            }

            return t1;
        }


        private static Action<IMetamodelBuilder> GetAddAction(Type type) {
            var deriveMethod = GetDeriveMethod(type);
            var populateMethod = GetPopulateMethod(type);
            var isEditMethod = GetIsEditMethod(type);

            if (deriveMethod is not null && populateMethod is not null) {
                var onType = GetAndValidateContributedToType(deriveMethod, populateMethod, isEditMethod);
                if (onType is not null) {
                    if (onType.GetCustomAttribute<ViewModelAttribute>() != null ||onType.GetCustomAttribute<ViewModelEditAttribute>() != null) {
                        IFacet FacetCreator(ISpecification spec) {
                            if (type.GetCustomAttribute<ViewModelEditAttribute>() is not null) {
                                return new ViewModelEditFacetViaFunctionsConvention(spec, deriveMethod, populateMethod);
                            }

                            if (isEditMethod is not null) {
                                return new ViewModelSwitchableFacetViaFunctionsConvention(spec, deriveMethod, populateMethod, isEditMethod);
                            }

                            return new ViewModelFacetViaFunctionsConvention(spec, deriveMethod, populateMethod);
                        }

                        return m => {
                            var spec = m.GetSpecification(onType);
                            var facet = FacetCreator(spec);
                            FacetUtils.AddFacet(facet);
                        };
                    }
                }
            }

            return null;
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var action = GetAddAction(type);
            if (action is not null) {
                var facet = new LifeCycleMethodsIntegrationFacet(specification, action);
                FacetUtils.AddFacet(facet);
            }
            return metamodel;
        }
    }
}