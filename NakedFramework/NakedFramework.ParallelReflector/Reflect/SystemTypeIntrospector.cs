// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.ParallelReflector.Reflect;

public class SystemTypeIntrospector : Introspector {
    public SystemTypeIntrospector(IReflector reflector, ILogger<SystemTypeIntrospector> logger) : base(reflector, logger) { }

    protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessType(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
        ((SystemTypeFacetFactorySet)FacetFactorySet).Process(Reflector, IntrospectedType, new IntrospectorMethodRemover(Methods), spec, metamodel);

    protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessCollection(PropertyInfo property, IOneToManyAssociationSpecImmutable collection, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

    protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessProperty(PropertyInfo property, IOneToOneAssociationSpecImmutable referenceProperty, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

    protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessAction(MethodInfo actionMethod, MethodInfo[] actions, IActionSpecImmutable action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

    protected override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParameter(MethodInfo actionMethod, int i, IActionParameterSpecImmutable param, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

    // Copyright (c) Naked Objects Group Ltd.
}