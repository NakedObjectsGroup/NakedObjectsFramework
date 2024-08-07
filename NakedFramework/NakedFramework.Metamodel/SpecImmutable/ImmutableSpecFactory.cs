﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Concurrent;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.SpecImmutable;

public static class ImmutableSpecFactory {
    private static readonly ConcurrentDictionary<Type, ITypeSpecBuilder> SpecCache = new();

    public static IActionParameterSpecImmutable CreateActionParameterSpecImmutable(IObjectSpecImmutable spec, IIdentifier identifier) => new ActionParameterSpecImmutable(spec.Type, identifier);

    public static IActionSpecImmutable CreateActionSpecImmutable(IIdentifier identifier, ITypeSpecImmutable ownerSpec, IActionParameterSpecImmutable[] parameters) => new ActionSpecImmutable(identifier, ownerSpec?.Type, parameters);

    public static IOneToManyAssociationSpecImmutable CreateOneToManyAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec, IObjectSpecImmutable defaultElementSpec) => new OneToManyAssociationSpecImmutable(identifier, ownerSpec.Type, returnSpec.Type, defaultElementSpec.Type);

    public static IOneToOneAssociationSpecImmutable CreateOneToOneAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec) => new OneToOneAssociationSpecImmutable(identifier, ownerSpec.Type, returnSpec.Type);

    private static IObjectSpecBuilder CreateObjectSpecImmutable(Type type, bool isRecognized) {
        return (IObjectSpecBuilder)SpecCache.GetOrAdd(type, t => new ObjectSpecImmutable(t, isRecognized));
    }

    private static IServiceSpecBuilder CreateServiceSpecImmutable(Type type, bool isRecognized) {
        return (IServiceSpecBuilder)SpecCache.GetOrAdd(type, t => new ServiceSpecImmutable(t, isRecognized));
    }

    public static ITypeSpecBuilder CreateTypeSpecImmutable(Type type, bool isService, bool isRecognized) =>
        isService
            ? CreateServiceSpecImmutable(type, isRecognized)
            : CreateObjectSpecImmutable(type, isRecognized);

    public static IAssociationSpecImmutable CreateSpecAdapter(IActionSpecImmutable actionSpecImmutable, IMetamodel metamodel) =>
        actionSpecImmutable.GetReturnSpec(metamodel).IsCollection
            ? new ActionToCollectionSpecAdapter(actionSpecImmutable)
            : new ActionToAssociationSpecAdapter(actionSpecImmutable);

    public static void ClearCache() {
        SpecCache.Clear();
    }
}