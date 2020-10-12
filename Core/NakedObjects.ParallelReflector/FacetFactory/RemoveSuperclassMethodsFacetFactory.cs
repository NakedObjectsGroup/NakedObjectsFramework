// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.ParallelReflect.Component;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Removes all methods inherited from <see cref="object" />
    /// </summary>
    /// <para>
    ///     Implementation - .Net fails to find methods properly for root class, so we used the saved set.
    /// </para>
    public sealed class RemoveSuperclassMethodsFacetFactory : FacetFactoryAbstract {
        public RemoveSuperclassMethodsFacetFactory(IFacetFactoryOrder<RemoveSuperclassMethodsFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Objects) { }

        private static void InitForType(Type type, IDictionary<Type, MethodInfo[]> typeToMethods) {
            if (!typeToMethods.ContainsKey(type)) {
                typeToMethods.Add(type, type.GetMethods());
            }
        }

        public static void ProcessSystemType(Type type, IMethodRemover methodRemover, ISpecification holder) {
            var typeToMethods = new Dictionary<Type, MethodInfo[]>();
            InitForType(type, typeToMethods);
            foreach (var method in typeToMethods[type]) {
                if (methodRemover != null && method != null) {
                    methodRemover.RemoveMethod(method);
                }
            }
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var currentType = type;
            while (currentType != null) {
                if (FasterTypeUtils.IsSystem(currentType)) {
                    ProcessSystemType(currentType, methodRemover, specification);
                }

                currentType = currentType.BaseType;
            }

            return metamodel;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}