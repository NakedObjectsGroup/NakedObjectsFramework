// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Utils;

using NakedObjects.Util;

namespace NakedObjects.Reflector.FacetFactory {
    public class OptionalAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (OptionalAnnotationFacetFactory));

        public OptionalAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.PropertiesAndParameters) {}

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = AttributeUtils.GetCustomAttribute<OptionallyAttribute>(member);
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            if ((method.ReturnType.IsPrimitive || TypeUtils.IsEnum(method.ReturnType)) && AttributeUtils.GetCustomAttribute<OptionallyAttribute>(method) != null) {
                Log.Warn("Ignoring Optionally annotation on primitive parameter on " + method.ReflectedType + "." + method.Name);
                return false;
            }
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if ((property.PropertyType.IsPrimitive || TypeUtils.IsEnum(property.PropertyType)) && AttributeUtils.GetCustomAttribute<OptionallyAttribute>(property) != null) {
                Log.Warn("Ignoring Optionally annotation on primitive or un-readable parameter on " + property.ReflectedType + "." + property.Name);
                return false;
            }
            if (property.GetGetMethod() != null && !property.PropertyType.IsPrimitive) {
                return Process(property, specification);
            }
            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if ((parameter.ParameterType.IsPrimitive || TypeUtils.IsEnum(parameter.ParameterType))) {
                if (AttributeUtils.GetCustomAttribute<OptionallyAttribute>(method) != null) {
                    Log.Warn("Ignoring Optionally annotation on primitive parameter " + paramNum + " on " + method.ReflectedType + "." +
                             method.Name);
                }
                return false;
            }
            var attribute = parameter.GetCustomAttributeByReflection<OptionallyAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMandatoryFacet Create(OptionallyAttribute attribute, ISpecification holder) {
            return attribute != null ? new OptionalFacet(holder) : null;
        }
    }
}