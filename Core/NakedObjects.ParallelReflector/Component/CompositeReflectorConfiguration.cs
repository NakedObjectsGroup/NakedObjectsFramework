// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core;
using NakedObjects.Menu;

namespace NakedObjects.ParallelReflect.Component {
    internal class NullObjectReflectorConfiguration : IObjectReflectorConfiguration {
        public Type[] TypesToIntrospect => Array.Empty<Type>();
        public Type[] Services => Array.Empty<Type>();
        public string[] ModelNamespaces => Array.Empty<string>();
        public List<Type> SupportedSystemTypes => new List<Type>();
        public (Type rootType, string name, bool allActions, Action<IMenu> customConstruction)[] MainMenus => null;
        public bool IgnoreCase => false;
        public bool ConcurrencyChecking => true;
        public bool HasConfig() => false;
    }


    internal class NullFunctionalReflectorConfiguration : IFunctionalReflectorConfiguration {
        public Type[] Types => Array.Empty<Type>();
        public Type[] Functions => Array.Empty<Type>();
        public Type[] Services => Array.Empty<Type>();
        public bool ConcurrencyChecking => true;
        public bool IgnoreCase => false;
        public (Type rootType, string name, bool allActions, Action<IMenu> customConstruction)[] MainMenus => null;
        public List<Type> SupportedSystemTypes => new List<Type>();
        public string[] ModelNamespaces => Array.Empty<string>();
        public bool HasConfig() => false;
    }


    public class CompositeReflectorConfiguration {
        private readonly IFunctionalReflectorConfiguration functionalReflectorConfiguration;
        private readonly IObjectReflectorConfiguration objectReflectorConfiguration;


        public CompositeReflectorConfiguration(IObjectReflectorConfiguration userObjectReflectorConfiguration,
                                               IFunctionalReflectorConfiguration userFunctionalReflectorConfiguration) {
            if (userObjectReflectorConfiguration is null && userFunctionalReflectorConfiguration is null) {
                throw new ReflectionException("Must have object or functional reflector configuration");
            }

            objectReflectorConfiguration = userObjectReflectorConfiguration ?? new NullObjectReflectorConfiguration();
            functionalReflectorConfiguration = userFunctionalReflectorConfiguration ?? new NullFunctionalReflectorConfiguration();

            if (objectReflectorConfiguration.HasConfig() && functionalReflectorConfiguration.HasConfig()) {
                if (objectReflectorConfiguration.ConcurrencyChecking != functionalReflectorConfiguration.ConcurrencyChecking) {
                    throw new ReflectionException("ConcurrencyChecking must be consistent between object and functional reflector configuration");
                }

                if (objectReflectorConfiguration.IgnoreCase != functionalReflectorConfiguration.IgnoreCase) {
                    throw new ReflectionException("Ignore must be consistent between object and functional reflector configuration");
                }
            }

            ConcurrencyChecking = objectReflectorConfiguration.HasConfig()
                ? objectReflectorConfiguration.ConcurrencyChecking
                : functionalReflectorConfiguration.ConcurrencyChecking;

            IgnoreCase = objectReflectorConfiguration.HasConfig()
                ? objectReflectorConfiguration.IgnoreCase
                : functionalReflectorConfiguration.IgnoreCase;
        }

        public bool ConcurrencyChecking { get; }

        public bool IgnoreCase { get; }

        public Type[] Services => objectReflectorConfiguration.Services.Union(functionalReflectorConfiguration.Services).ToArray();

        public Type[] ObjectTypes => Services.Union(GetObjectTypesToIntrospect()).ToArray();

        public Type[] RecordTypes => functionalReflectorConfiguration.Types;

        public Type[] FunctionTypes => functionalReflectorConfiguration.Functions;

        public ISet<Type> ServiceTypeSet => new HashSet<Type>(Services);

        public Func<IMenuFactory, IMenu[]> MainMenus() =>
            mf => {
                var omm = objectReflectorConfiguration.MainMenus;
                var fmm = functionalReflectorConfiguration.MainMenus;
                IMenu[] menus = null;

                if (omm is not null && omm.Any()) {
                    menus = omm.Select(tuple => {
                        var (type, name, addAll, action) = tuple;
                        var menu = mf.NewMenu(type, addAll, name);
                        action?.Invoke(menu);
                        return menu;
                    }).ToArray();
                }

                if (fmm is not null && fmm.Any()) {
                    menus ??= new IMenu[] { };
                    menus = menus.Union(fmm.Select(tuple => {
                        var (type, name, addAll, action) = tuple;
                        var menu = mf.NewMenu(type, addAll, name);
                        action?.Invoke(menu);
                        return menu;
                    })).ToArray();
                }


                return menus;
            };

        private static Type EnsureGenericTypeIsComplete(Type type) {
            if (type.IsGenericType && !type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }

            return type;
        }

        private Type[] GetObjectTypesToIntrospect() {
            var types = objectReflectorConfiguration.TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            var oSystemTypes = objectReflectorConfiguration.SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
            var fSystemTypes = functionalReflectorConfiguration.SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
            var systemTypes = oSystemTypes.Union(fSystemTypes).Distinct();
            return types.Union(systemTypes).ToArray();
        }
    }
}