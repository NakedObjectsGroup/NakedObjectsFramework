// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Removes any methods on a type that handle events.
    /// </summary>
    public sealed class RemoveEventHandlerMethodsFacetFactory : ObjectFacetFactoryProcessor {
        public RemoveEventHandlerMethodsFacetFactory(IFacetFactoryOrder<RemoveEventHandlerMethodsFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            FindAndRemoveEventHandlerMethods(type, methodRemover);
            return metamodel;
        }

        private static void RemoveIfNotNull(IMethodRemover methodRemover, MethodInfo mi) {
            if (mi is not null) {
                methodRemover.SafeRemoveMethod(mi);
            }
        }

        private static void FindAndRemoveEventHandlerMethods(Type type, IMethodRemover methodRemover) {
            foreach (var eInfo in type.GetEvents()) {
                RemoveIfNotNull(methodRemover, eInfo.GetAddMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRaiseMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRemoveMethod());

                eInfo.GetOtherMethods().ForEach(mi => RemoveIfNotNull(methodRemover, mi));
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}