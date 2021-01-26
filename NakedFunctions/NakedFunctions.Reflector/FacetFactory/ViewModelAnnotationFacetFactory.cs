// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class ViewModelAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IMethodFilteringFacetFactory
    {
        private readonly ILogger<ViewModelAnnotationFacetFactory> logger;
        private const string CreateFromKeys = "CreateFromKeys";
        private const string IsEditView = "IsEditView";
        private const string DeriveKeys = "DeriveKeys";

        public ViewModelAnnotationFacetFactory(IFacetFactoryOrder<ViewModelAnnotationFacetFactory> order, ILoggerFactory loggerFactory) :
            base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) => logger = loggerFactory.CreateLogger<ViewModelAnnotationFacetFactory>();

        private static MethodInfo GetMethod(Type type, string name) => type.GetMethods().SingleOrDefault(m => m.Name == name);

        private static MethodInfo GetDeriveMethod(Type type) => GetMethod(type, DeriveKeys);

        private static MethodInfo GetPopulateMethod(Type type) => GetMethod(type, CreateFromKeys);

        private static MethodInfo GetIsEditMethod(Type type) => GetMethod(type, IsEditView);

        [DoesNotReturn]
        private static void ThrowError(string msg) => throw new ReflectionException(msg);

        private static (Type, ViewModelAttribute) GetAndValidateContributedToType(MethodInfo deriveMethod, MethodInfo isEditMethod) {
            var deriveMethodVmType = deriveMethod.GetContributedToType();
            if (deriveMethodVmType is null) {
                ThrowError($"View model function {deriveMethod.Name} on {deriveMethod.DeclaringType} has missing 'this' parameter");
            }

            var vmAttribute = deriveMethodVmType?.GetCustomAttribute<ViewModelAttribute>();
            var attributeFunctionsType = vmAttribute?.TypeDefiningVMFunctions;
            if (attributeFunctionsType is null) {
                ThrowError($"Missing ViewModelAttribute on {deriveMethodVmType}");
            }

            if (attributeFunctionsType != deriveMethod.DeclaringType) {
                ThrowError($"View model function {deriveMethod.Name} and ViewModelAttribute on {deriveMethodVmType} have mismatched types");
            }

            var isEditMethodVmType = isEditMethod.GetContributedToType();

            if (isEditMethodVmType is not null && isEditMethodVmType != deriveMethodVmType) {
                ThrowError($"View model function {isEditMethod.Name} on {isEditMethod.DeclaringType} has mismatched types");
            }

            if (isEditMethodVmType is null && vmAttribute?.Editability == VMEditability.Switchable)
            {
                ThrowError($"Missing {IsEditView} function on {attributeFunctionsType} as ViewModel has Switchable flag");
            }

            return (deriveMethodVmType, vmAttribute);
        }

        private static Action<IMetamodelBuilder> GetAddAction(Type type) {
            var deriveMethod = GetDeriveMethod(type);
            var populateMethod = GetPopulateMethod(type);
            var isEditMethod = GetIsEditMethod(type);

            if (deriveMethod is not null && populateMethod is not null) {
                var (onType, attribute) = GetAndValidateContributedToType(deriveMethod, isEditMethod);

                IFacet FacetCreator(ISpecification spec) => attribute.Editability switch {
                    VMEditability.Switchable => new ViewModelSwitchableFacetViaFunctionsConvention(spec, deriveMethod, populateMethod, isEditMethod),
                    VMEditability.EditOnly => new ViewModelEditFacetViaFunctionsConvention(spec, deriveMethod, populateMethod),
                    _ => new ViewModelFacetViaFunctionsConvention(spec, deriveMethod, populateMethod)
                };

                return m => {
                    var spec = m.GetSpecification(onType);
                    var facet = FacetCreator(spec);
                    FacetUtils.AddFacet(facet);
                };
            }

            return null;
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  Type type,  ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var action = GetAddAction(type);
            if (action is not null) {
                var integrationFacet = specification.GetFacet<IIntegrationFacet>();

                if (integrationFacet is null) {
                    integrationFacet = new IntegrationFacet(specification, action);
                    FacetUtils.AddFacet(integrationFacet);
                }
                else {
                    integrationFacet.AddAction(action);
                }
            }
            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) {
            switch (method.Name) {
                case DeriveKeys when IsViewModelMatch(method):
                case IsEditView when IsViewModelMatch(method):
                    return true;
                case CreateFromKeys: {
                    var deriveMethod = GetMethod(method.DeclaringType, DeriveKeys);
                    return deriveMethod is not null && IsViewModelMatch(deriveMethod);
                }
                default:
                    return false;
            }
        }

        private static bool IsViewModelMatch(MethodInfo method) => method.GetContributedToType()?.GetCustomAttribute<ViewModelAttribute>()?.TypeDefiningVMFunctions == method.DeclaringType;
    }
}