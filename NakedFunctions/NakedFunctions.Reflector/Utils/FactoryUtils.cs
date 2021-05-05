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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Utils;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.Utils {
    public static class FactoryUtils {
        public static bool Matches(this MethodInfo methodInfo, string name, Type declaringType, Type returnType, Type targetType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == declaringType &&
            methodInfo.ReturnType == returnType &&
            methodInfo.ContributedToType() == targetType;

        public static MethodInfo FindComplementaryMethod(Type declaringType, string name, Func<MethodInfo, bool> matcher, ILogger logger) {
            var complementaryMethods = declaringType.GetMethods().Where(matcher).ToArray();

            if (complementaryMethods.Length > 1) {
                logger.LogWarning($"Multiple methods found: {name} with matching signature - ignoring");
                return null;
            }

            var complementaryMethod = complementaryMethods.SingleOrDefault();
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != complementaryMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"Method found: {methodInfo.Name} not matching expected signature");
            }

            return complementaryMethod;
        }

        public static T Invoke<T>(this Func<object, object[], object> methodDelegate, MethodInfo method, object[] parms) {
            try {
                return methodDelegate is not null ? (T) methodDelegate(null, parms) : (T) method.Invoke(null, parms);
            }
            catch (InvalidCastException) {
                throw new NakedObjectDomainException($"Must return {typeof(T)} from  method: {method.Name}");
            }
        }

        public static void AddIntegrationFacet(ISpecificationBuilder specification, Action<IMetamodelBuilder> action) {
            var integrationFacet = specification.GetFacet<IIntegrationFacet>();

            if (integrationFacet is null) {
                integrationFacet = new IntegrationFacet(specification, action);
                FacetUtils.AddFacet(integrationFacet);
            }
            else {
                integrationFacet.AddAction(action);
            }
        }

        public static bool IsVoid(Type type) => type == typeof(void);

        public static bool IsSealed(Type type) => type.IsSealed;

        public static bool IsInterface(Type type) => type.IsInterface;

        public static bool IsAbstract(Type type) => type.IsAbstract;

        public static bool IsStatic(Type type) => IsAbstract(type) && IsSealed(type);

        public record ActionHolder
        {
            private readonly object wrapped;

            public ActionHolder(IActionSpecImmutable actionSpecImmutable) => wrapped = actionSpecImmutable;

            public ActionHolder(IAssociationSpecImmutable associationSpecImmutable) => wrapped = associationSpecImmutable;

            public ActionHolder(IMenuActionImmutable menuActionImmutable) => wrapped = menuActionImmutable;

            public string Name => wrapped switch
            {
                IActionSpecImmutable action => action.Identifier.MemberName,
                IAssociationSpecImmutable association => association.Identifier.MemberName,
                IMenuActionImmutable menu => menu.Action.Identifier.MemberName,
                _ => ""
            };

            public ITypeSpecImmutable OwnerSpec => wrapped switch
            {
                IActionSpecImmutable action => action.OwnerSpec,
                IAssociationSpecImmutable association => association.OwnerSpec,
                IMenuActionImmutable menu => menu.Action.OwnerSpec,
                _ => null
            };
        }


        public static void ErrorOnDuplicates(IList<ActionHolder> actions)
        {
            var names = actions.Select(s => s.Name).ToArray();
            var distinctNames = names.Distinct().ToArray();

            if (names.Length != distinctNames.Length)
            {
                var duplicates = names.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key);
                var errors = new List<string>();

                foreach (var name in duplicates)
                {
                    var duplicateActions = actions.OrderBy(a => a.OwnerSpec.FullName).Where(s => s.Name == name);
                    var error = duplicateActions.Aggregate("Name clash between user actions defined on", (s, a) => $"{s}{(s.EndsWith("defined on") ? " " : " and ")}{a.OwnerSpec.FullName}.{a.Name}");
                    errors.Add(error);
                }

                throw new ReflectionException(string.Join(", ", errors));
            }
        }
    }
}