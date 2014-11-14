// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using MemberInfo = System.Reflection.MemberInfo;

namespace NakedObjects.Reflect.FacetFactory {
    public class HiddenAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public HiddenAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.PropertiesCollectionsAndActions) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return Process(type.GetCustomAttributeByReflection<HiddenAttribute>,
                type.GetCustomAttributeByReflection<ScaffoldColumnAttribute>, specification);
        }

        private static bool Process(MemberInfo member, ISpecification holder) {
            return Process(member.GetCustomAttribute<HiddenAttribute>, member.GetCustomAttribute<ScaffoldColumnAttribute>, holder);
        }

        private static bool Process(Func<Attribute> getHidden, Func<Attribute> getScaffold, ISpecification specification) {
            Attribute attribute = getHidden();
            if (attribute != null) {
                return FacetUtils.AddFacet(Create((HiddenAttribute) attribute, specification));
            }
            attribute = getScaffold();
            return FacetUtils.AddFacet(Create((ScaffoldColumnAttribute) attribute, specification));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            return Process(property, specification);
        }

        private static IHiddenFacet Create(HiddenAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacet(attribute.Value, holder);
        }

        private static IHiddenFacet Create(ScaffoldColumnAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacet(attribute.Scaffold ? WhenTo.Never : WhenTo.Always, holder);
        }
    }
}