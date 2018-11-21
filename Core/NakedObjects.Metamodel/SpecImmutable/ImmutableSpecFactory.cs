// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.SpecImmutable {
    public static class ImmutableSpecFactory {
        private static readonly Dictionary<Type, ITypeSpecBuilder> specCache = new Dictionary<Type, ITypeSpecBuilder>();

        public static IActionParameterSpecImmutable CreateActionParameterSpecImmutable(IObjectSpecImmutable spec, IIdentifier identifier) {
            return new ActionParameterSpecImmutable(spec, identifier);
        }

        public static IActionSpecImmutable CreateActionSpecImmutable(IIdentifier identifier, ITypeSpecImmutable ownerSpec, IActionParameterSpecImmutable[] parameters) {
            return new ActionSpecImmutable(identifier, ownerSpec, parameters);
        }

        public static IObjectSpecBuilder CreateObjectSpecImmutable(Type type, IMetamodel metamodel) {
            return new ObjectSpecImmutable(type);
        }

        public static IServiceSpecBuilder CreateServiceSpecImmutable(Type type, IMetamodel metamodel) {
            return new ServiceSpecImmutable(type);
        }

        public static IOneToManyAssociationSpecImmutable CreateOneToManyAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec, IObjectSpecImmutable defaultElementSpec) {
            return new OneToManyAssociationSpecImmutable(identifier, ownerSpec, returnSpec, defaultElementSpec);
        }

        public static IOneToOneAssociationSpecImmutable CreateOneToOneAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec) {
            return new OneToOneAssociationSpecImmutable(identifier, ownerSpec, returnSpec);
        }

        public static IObjectSpecBuilder CreateObjectSpecImmutable(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            lock (specCache) {
                if (!specCache.ContainsKey(type)) {
                    specCache.Add(type, new ObjectSpecImmutable(type));
                }

                return specCache[type] as IObjectSpecBuilder;
            }
        }

        public static IServiceSpecBuilder CreateServiceSpecImmutable(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            lock (specCache) {
                if (!specCache.ContainsKey(type)) {
                    specCache.Add(type, new ServiceSpecImmutable(type));
                }

                return specCache[type] as IServiceSpecBuilder;
            }
        }
    }
}