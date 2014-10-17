// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class DefaultedFacetFactory : AnnotationBasedFacetFactoryAbstract, INakedObjectConfigurationAware {
        public DefaultedFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsPropertiesAndParameters) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(type, specification));
        }

        private static IDefaultedFacet Create(Type type, ISpecification holder) {
            var annotation = type.GetCustomAttributeByReflection<DefaultedAttribute>();

            // create from annotation, if present
            if (annotation != null) {
                var facet = TypeUtils.CreateGenericInstance<IDefaultedFacet>(typeof (DefaultedFacetAnnotation<>),
                    new[] {type},
                    new object[] {type, holder});


                if (facet.IsValid) {
                    return facet;
                }
            }


            return null;
        }

        /// <summary>
        ///     If there is a <see cref="IDefaultedFacet" />on the properties return Type, then installs a
        ///     <see cref="IPropertyDefaultFacet" /> for the property with the same default.
        /// </summary>
        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            // don't overwrite any defaults already picked up
            if (specification.ContainsFacet(typeof (IPropertyDefaultFacet))) {
                return false;
            }

            // try to infer defaults from the underlying return Type
            Type returnType = method.ReturnType;
            IDefaultedFacet returnTypeDefaultedFacet = GetDefaultedFacet(returnType);
            if (returnTypeDefaultedFacet != null) {
                var propertyFacet = new PropertyDefaultFacetDerivedFromDefaultedFacet(returnTypeDefaultedFacet, specification);
                return FacetUtils.AddFacet(propertyFacet);
            }
            return false;
        }

        /// <summary>
        ///     If there is a <see cref="IDefaultedFacet" /> on any of the action's parameter types, then installs a
        ///     <see cref="IActionDefaultsFacet" /> for the action.
        /// </summary>
        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            // don't overwrite any defaults already picked up
            if (holder.ContainsFacet(typeof (IActionDefaultsFacet))) {
                return false;
            }

            // try to infer defaults from any of the parameter's underlying types

            Type paramType = method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray()[paramNum];
            IDefaultedFacet defaultedFacet = GetDefaultedFacet(paramType);
            if (defaultedFacet != null) {
                return FacetUtils.AddFacet(new ActionDefaultsFacetDerivedFromDefaultedFacets(defaultedFacet, holder));
            }
            return false;
        }

        private IDefaultedFacet GetDefaultedFacet(Type paramType) {
            var paramTypeSpec = Reflector.LoadSpecification(paramType);
            return paramTypeSpec.GetFacet<IDefaultedFacet>();
        }
    }
}