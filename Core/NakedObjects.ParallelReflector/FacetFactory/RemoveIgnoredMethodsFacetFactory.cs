// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    /// Does not add any facets, but removes members that should be ignored - before they are introspected upon
    /// by other factories.  This factory thus needs to be registered earlier than most other factories.
    /// </summary>
    public class RemoveIgnoredMethodsFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public RemoveIgnoredMethodsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsAndInterfaces) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder spec, IMetamodelBuilder metamodel) {
            RemoveExplicitlyIgnoredMembers(type, methodRemover);
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder spec, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            RemoveExplicitlyIgnoredMembers(type, methodRemover);
            return metamodel;
        }

        private static void RemoveExplicitlyIgnoredMembers(Type type, IMethodRemover methodRemover) {
            foreach (MethodInfo method in type.GetMethods().Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null)) {
                methodRemover.RemoveMethod(method);
            }
        }
    }
}