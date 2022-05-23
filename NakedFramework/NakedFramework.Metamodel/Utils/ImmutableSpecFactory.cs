// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.SpecImmutable;

namespace NakedFramework.Metamodel.Utils;

public static class ImmutableSpecFactory
{
    private static readonly Dictionary<Type, ITypeSpecBuilder> SpecCache = new();

    public static IActionParameterSpecImmutable CreateActionParameterSpecImmutable(IObjectSpecImmutable spec, IIdentifier identifier) => new ActionParameterSpecImmutable(spec, identifier);

    public static IActionSpecImmutable CreateActionSpecImmutable(IIdentifier identifier, ITypeSpecImmutable ownerSpec, IActionParameterSpecImmutable[] parameters) => new ActionSpecImmutable(identifier, ownerSpec, parameters);

    public static IOneToManyAssociationSpecImmutable CreateOneToManyAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec, IObjectSpecImmutable defaultElementSpec) => new OneToManyAssociationSpecImmutable(identifier, ownerSpec, returnSpec, defaultElementSpec);

    public static IOneToOneAssociationSpecImmutable CreateOneToOneAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec) => new OneToOneAssociationSpecImmutable(identifier, ownerSpec, returnSpec);

    private static IObjectSpecBuilder CreateObjectSpecImmutable(Type type, bool isRecognized)
    {
        lock (SpecCache)
        {
            if (!SpecCache.ContainsKey(type))
            {
                SpecCache.Add(type, new ObjectSpecImmutable(type, isRecognized));
            }

            return SpecCache[type] as IObjectSpecBuilder;
        }
    }

    private static IServiceSpecBuilder CreateServiceSpecImmutable(Type type, bool isRecognized)
    {
        lock (SpecCache)
        {
            if (!SpecCache.ContainsKey(type))
            {
                SpecCache.Add(type, new ServiceSpecImmutable(type, isRecognized));
            }

            return SpecCache[type] as IServiceSpecBuilder;
        }
    }

    public static ITypeSpecBuilder CreateTypeSpecImmutable(Type type, bool isService, bool isRecognized) =>
        isService
            ? CreateServiceSpecImmutable(type, isRecognized)
            : CreateObjectSpecImmutable(type, isRecognized);

    public static IAssociationSpecImmutable CreateSpecAdapter(IActionSpecImmutable actionSpecImmutable, IMetamodel metamodel) =>
        actionSpecImmutable.GetReturnSpec(metamodel).IsCollection
            ? new ActionToCollectionSpecAdapter(actionSpecImmutable)
            : new ActionToAssociationSpecAdapter(actionSpecImmutable);

    public static void ClearCache()
    {
        lock (SpecCache)
        {
            SpecCache.Clear();
        }
    }
}