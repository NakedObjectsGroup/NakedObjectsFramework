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
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.ParallelReflect.TypeFacetFactory {
    public sealed class EnumValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory {
        public EnumValueTypeFacetFactory(int numericOrder) : base(numericOrder) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            if (typeof (Enum).IsAssignableFrom(type)) {
                Type semanticsProviderType = typeof (EnumValueSemanticsProvider<>).MakeGenericType(type);
                var spec = reflector.LoadSpecification<IObjectSpecImmutable>(type, metamodel);
                object semanticsProvider = Activator.CreateInstance(semanticsProviderType, spec, specification);

                MethodInfo method = typeof (ValueUsingValueSemanticsProviderFacetFactory).GetMethod("AddValueFacets", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
                method.Invoke(null, new[] {semanticsProvider, specification});
            }
        }
    }
}