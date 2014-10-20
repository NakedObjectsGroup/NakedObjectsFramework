// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;
using NakedObjects.Metamodel.Utils;

namespace NakedObjects.Metamodel.Facet {
    public class ValueFacetAnnotation<T> : ValueFacetAbstract<T> {
        public ValueFacetAnnotation(Type annotatedClass, ISpecification holder)
            : this(SemanticsProviderName(annotatedClass), SemanticsProviderClass(annotatedClass), holder) {}

        private ValueFacetAnnotation(string candidateSemanticsProviderName,
                                     Type candidateSemanticsProviderClass,
                                     ISpecification holder)
            : base(ValueSemanticsProviderUtils.ValueSemanticsProviderOrNull<T>(candidateSemanticsProviderClass,
                candidateSemanticsProviderName),
                true,
                holder) {}

        /// <summary>
        ///     Always valid, even if the specified semanticsProviderName might have been wrong.
        /// </summary>
        public override bool IsValid {
            get { return true; }
        }

        private static string SemanticsProviderName(Type annotatedClass) {
            string semanticsProviderName = annotatedClass.GetCustomAttributeByReflection<ValueAttribute>().SemanticsProviderName;
            return string.IsNullOrEmpty(semanticsProviderName) ? null : semanticsProviderName;
        }

        private static Type SemanticsProviderClass(Type annotatedClass) {
            return annotatedClass.GetCustomAttributeByReflection<ValueAttribute>().SemanticsProviderClass;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}