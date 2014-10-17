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

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Removes any calls to <c>Init</c>
    /// </summary>
    public class RemoveInitMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        public RemoveInitMethodFacetFactory(INakedObjectReflector reflector)
            : base(reflector, FeatureType.ObjectsOnly) {}

        public override string[] Prefixes {
            get { return new string[] {}; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            FindAndRemoveInitMethod(type, methodRemover);
            return false;
        }

        private void FindAndRemoveInitMethod(Type type, IMethodRemover methodRemover) {
            MethodInfo method = FindMethod(type, MethodType.Object, "Init", typeof (void), Type.EmptyTypes);
            if (method != null) {
                RemoveMethod(methodRemover, method);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}