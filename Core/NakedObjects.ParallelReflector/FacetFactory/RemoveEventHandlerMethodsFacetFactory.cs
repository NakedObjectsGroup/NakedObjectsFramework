// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Removes any calls to <c>Init</c>
    /// </summary>
    public sealed class RemoveEventHandlerMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        public RemoveEventHandlerMethodsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Objects) {}

        public override string[] Prefixes {
            get { return new string[] {}; }
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            FindAndRemoveEventHandlerMethods(type, methodRemover);
        }

        private void RemoveIfNotNull(IMethodRemover methodRemover, MethodInfo mi) {
            if (mi != null) {
                RemoveMethod(methodRemover, mi);
            }
        }

        private void FindAndRemoveEventHandlerMethods(Type type, IMethodRemover methodRemover) {
            foreach (EventInfo eInfo in type.GetEvents()) {
                RemoveIfNotNull(methodRemover, eInfo.GetAddMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRaiseMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRemoveMethod());

                eInfo.GetOtherMethods().ForEach(mi => RemoveIfNotNull(methodRemover, mi));
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}