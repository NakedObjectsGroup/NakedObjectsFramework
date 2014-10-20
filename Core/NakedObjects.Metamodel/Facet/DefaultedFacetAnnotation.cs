// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.Facet {
    public class DefaultedFacetAnnotation<T> : DefaultedFacetAbstract<T> {
        public DefaultedFacetAnnotation(Type annotatedClass, ISpecification holder)
            : this(ProviderName(annotatedClass), ProviderClass(annotatedClass), holder) {}

        private DefaultedFacetAnnotation(string candidateProviderName, Type candidateProviderClass, ISpecification holder)
            : base(candidateProviderName, candidateProviderClass, holder) {}

        private static string ProviderName(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<DefaultedAttribute>();
            string providerName = annotation.DefaultsProviderName;
            return !string.IsNullOrEmpty(providerName) ? providerName : null;
        }

        private static Type ProviderClass(Type annotatedClass) {
            var annotation = annotatedClass.GetCustomAttributeByReflection<DefaultedAttribute>();
            return annotation.DefaultsProviderClass;
        }
    }
}