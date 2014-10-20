// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;


namespace NakedObjects.Reflector.FacetFactory {
    public class RemoveDynamicProxyMethodsFacetFactory : FacetFactoryAbstract {
        private static readonly IList<string> methodsToRemove;

        static RemoveDynamicProxyMethodsFacetFactory() {
            methodsToRemove = new List<string> {"GetBasePropertyValue", "SetBasePropertyValue", "SetChangeTracker"};
        }

        public RemoveDynamicProxyMethodsFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsAndProperties) {}

        private static bool IsDynamicProxyType(Type type) {
            return type.FullName.StartsWith("System.Data.Entity.DynamicProxies");
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            if (IsDynamicProxyType(type)) {
                foreach (MethodInfo method in type.GetMethods().Join(methodsToRemove, mi => mi.Name, s => s, (mi, s) => mi)) {
                    if (methodRemover != null && method != null) {
                        methodRemover.RemoveMethod(method);
                    }
                }
            }

            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if (IsDynamicProxyType(property.DeclaringType) && property.Name == "RelationshipManager") {
                return FacetUtils.AddFacet(new HiddenFacet(WhenTo.Always, specification));
            }
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}