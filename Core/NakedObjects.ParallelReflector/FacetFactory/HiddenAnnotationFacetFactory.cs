// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class HiddenAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public HiddenAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.PropertiesCollectionsAndActions) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(type.GetCustomAttribute<HiddenAttribute>,
                type.GetCustomAttribute<ScaffoldColumnAttribute>, specification);
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            Process(member.GetCustomAttribute<HiddenAttribute>, member.GetCustomAttribute<ScaffoldColumnAttribute>, holder);
        }

        private static void Process(Func<Attribute> getHidden, Func<Attribute> getScaffold, ISpecification specification) {
            Attribute attribute = getHidden();
            FacetUtils.AddFacet(attribute != null ? Create((HiddenAttribute) attribute, specification) : Create((ScaffoldColumnAttribute) getScaffold(), specification));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(method, specification);
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(property, specification);
        }

        private static IHiddenFacet Create(HiddenAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacet(attribute.Value, holder);
        }

        private static IHiddenFacet Create(ScaffoldColumnAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new HiddenFacet(attribute.Scaffold ? WhenTo.Never : WhenTo.Always, holder);
        }
    }
}