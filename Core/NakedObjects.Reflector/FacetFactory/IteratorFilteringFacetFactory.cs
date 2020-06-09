// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Designed to simply filter out <see cref="IEnumerable.GetEnumerator" /> method if it exists.
    /// </summary>
    /// <para>
    ///     Does not add any <see cref="IFacet" />s
    /// </para>
    public sealed class IteratorFilteringFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;

        static IteratorFilteringFacetFactory() => FixedPrefixes = new[] {RecognisedMethodsAndPrefixes.GetEnumeratorMethod};

        public IteratorFilteringFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public override string[] Prefixes => FixedPrefixes;

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (typeof(IEnumerable).IsAssignableFrom(type) && !TypeUtils.IsSystem(type)) {
                var method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.GetEnumeratorMethod, null, Type.EmptyTypes);
                if (method != null) {
                    methodRemover.RemoveMethod(method);
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}