// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.ParallelReflect;
using NakedObjects.ParallelReflector.Reflect;

namespace NakedObjects.Reflector.Reflect {
    public class ObjectIntrospector : Introspector, IIntrospector {
        public ObjectIntrospector(IReflector reflector, ILogger<ObjectIntrospector> logger) : base(reflector, logger) { }


        protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessType(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
            ((ObjectFacetFactorySet)FacetFactorySet).Process(Reflector, ClassStrategy, IntrospectedType, new IntrospectorMethodRemover(Methods), spec, metamodel);


        protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessCollection(PropertyInfo property, IOneToManyAssociationSpecImmutable collection, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
            ((ObjectFacetFactorySet)FacetFactorySet).Process(Reflector, ClassStrategy, property, new IntrospectorMethodRemover(Methods), collection, FeatureType.Collections, metamodel);


        protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessProperty(PropertyInfo property, IOneToOneAssociationSpecImmutable referenceProperty, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
            ((ObjectFacetFactorySet)FacetFactorySet).Process(Reflector, ClassStrategy, property, new IntrospectorMethodRemover(Methods), referenceProperty, FeatureType.Properties, metamodel);


        protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessAction(MethodInfo actionMethod, MethodInfo[] actions, IActionSpecImmutable action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => ((ObjectFacetFactorySet)FacetFactorySet).Process(Reflector, ClassStrategy, actionMethod, new IntrospectorMethodRemover(actions), action, FeatureType.Actions, metamodel);


        protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParameter(MethodInfo actionMethod, int i, IActionParameterSpecImmutable param, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => ((ObjectFacetFactorySet)FacetFactorySet).ProcessParams(Reflector, ClassStrategy, actionMethod, i, param, metamodel);


        // Copyright (c) Naked Objects Group Ltd.
    }
}