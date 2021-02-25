// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedObjects;

namespace NakedFramework.ParallelReflector.TypeFacetFactory {
    /// <summary>
    ///     Designed to simply filter out <see cref="IEnumerable.GetEnumerator" /> method if it exists.
    /// </summary>
    /// <para>
    ///     Does not add any <see cref="IFacet" />s
    /// </para>
    public sealed class IteratorFilteringFacetFactory : SystemTypeFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
        private static readonly string[] FixedPrefixes;

        static IteratorFilteringFacetFactory() => FixedPrefixes = new[] {RecognisedMethodsAndPrefixes.GetEnumeratorMethod};

        public IteratorFilteringFacetFactory(IFacetFactoryOrder<IteratorFilteringFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (typeof(IEnumerable).IsAssignableFrom(type) && !FasterTypeUtils.IsSystem(type)) {
                var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.GetEnumeratorMethod, null, Type.EmptyTypes);
                methodRemover.SafeRemoveMethod(method);
            }

            return metamodel;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}