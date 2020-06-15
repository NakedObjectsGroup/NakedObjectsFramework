// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
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
    /// <summary>
    ///     Creates an <see cref="IFindMenuFacet" /> based on the presence of an
    ///     <see cref="FindMenuAttribute" /> annotation
    /// </summary>
    public sealed class FindMenuFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private ILogger<FindMenuFacetFactory> logger;

        public FindMenuFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesAndActionParameters) {
            logger = loggerFactory.CreateLogger<FindMenuFacetFactory>();
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<FindMenuAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) => Process(method, specification);

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var pType = property.PropertyType;
            if ((pType.IsPrimitive || pType == typeof(string) || TypeUtils.IsEnum(pType)) && property.GetCustomAttribute<FindMenuAttribute>() != null) {
                logger.LogWarning($"Ignoring FindMenu annotation on primitive or un-readable parameter on {property.ReflectedType}.{property.Name}");
                return;
            }

            if (property.GetGetMethod() != null && !property.PropertyType.IsPrimitive) {
                Process(property, specification);
            }
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var parameter = method.GetParameters()[paramNum];
            var pType = parameter.ParameterType;
            if (pType.IsPrimitive || pType == typeof(string) || TypeUtils.IsEnum(pType)) {
                if (method.GetCustomAttribute<FindMenuAttribute>() != null) {
                    logger.LogWarning($"Ignoring FindMenu annotation on primitive parameter {paramNum} on {method.ReflectedType}.{method.Name}");
                }

                return;
            }

            var attribute = parameter.GetCustomAttribute<FindMenuAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IFacet Create(FindMenuAttribute attribute, ISpecification holder) => attribute == null ? null : new FindMenuFacet(holder);
    }
}