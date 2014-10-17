// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    /// <summary>
    ///     Designed to simply filter out <see cref="IEnumerable.GetEnumerator" /> method if it exists.
    /// </summary>
    /// <para>
    ///     Does not add any <see cref="IFacet" />s
    /// </para>
    public class IteratorFilteringFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;

        static IteratorFilteringFacetFactory() {
            FixedPrefixes = new[] {PrefixesAndRecognisedMethods.GetEnumeratorMethod};
        }

        public IteratorFilteringFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsOnly) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            if (typeof (IEnumerable).IsAssignableFrom(type) && !TypeUtils.IsSystem(type)) {
                MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.GetEnumeratorMethod, null, Type.EmptyTypes);
                if (method != null) {
                    methodRemover.RemoveMethod(method);
                }
            }
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}