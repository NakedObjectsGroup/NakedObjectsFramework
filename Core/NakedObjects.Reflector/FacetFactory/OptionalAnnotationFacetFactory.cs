// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class OptionalAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private ILogger<OptionalAnnotationFacetFactory> logger;

        public OptionalAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesAndActionParameters) {
            logger = loggerFactory.CreateLogger<OptionalAnnotationFacetFactory>();
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<OptionallyAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if ((method.ReturnType.IsPrimitive || TypeUtils.IsEnum(method.ReturnType)) && method.GetCustomAttribute<OptionallyAttribute>() != null) {
                logger.LogWarning($"Ignoring Optionally annotation on primitive parameter on {method.ReflectedType}.{method.Name}");
                return;
            }

            Process(method, specification);
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if ((property.PropertyType.IsPrimitive || TypeUtils.IsEnum(property.PropertyType)) && property.GetCustomAttribute<OptionallyAttribute>() != null) {
                logger.LogWarning($"Ignoring Optionally annotation on primitive or un-readable parameter on {property.ReflectedType}.{property.Name}");
                return;
            }

            if (property.GetGetMethod() != null && !property.PropertyType.IsPrimitive) {
                Process(property, specification);
            }
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var parameter = method.GetParameters()[paramNum];
            if (parameter.ParameterType.IsPrimitive || TypeUtils.IsEnum(parameter.ParameterType)) {
                if (method.GetCustomAttribute<OptionallyAttribute>() != null) {
                    logger.LogWarning($"Ignoring Optionally annotation on primitive parameter {paramNum} on {method.ReflectedType}.{method.Name}");
                }

                return;
            }

            var attribute = parameter.GetCustomAttribute<OptionallyAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMandatoryFacet Create(OptionallyAttribute attribute, ISpecification holder) => attribute != null ? new OptionalFacet(holder) : null;
    }
}