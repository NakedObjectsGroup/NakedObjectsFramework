// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Utils; 

public static class FacetUtils {
    public static string[] SplitOnComma(string toSplit) {
        if (string.IsNullOrEmpty(toSplit)) {
            return Array.Empty<string>();
        }

        return toSplit.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
    }

    public static bool IsAllowed(ISession session, string[] roles, string[] users) =>
        roles.Any(role => session.Principal.IsInRole(role)) ||
        users.Any(user => session.Principal.Identity?.Name == user);

    /// <summary>
    ///     Attaches the <see cref="IFacet" /> to its <see cref="IFacet.Specification" />
    /// </summary>
    public static void AddFacet(IFacet facet) => ((ISpecificationBuilder) facet?.Specification)?.AddFacet(facet);

    /// <summary>
    ///     Attaches each <see cref="IFacet" /> to its <see cref="IFacet.Specification" />
    /// </summary>
    public static void AddFacets(IEnumerable<IFacet> facetList) => facetList.ForEach(AddFacet);

    public static INakedObjectAdapter[] MatchParameters(string[] parameterNames, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
        INakedObjectAdapter GetValue(string name) =>
            parameterNameValues?.ContainsKey(name) == true
                ? parameterNameValues[name]
                : null;

        return parameterNames.Select(GetValue).ToArray();
    }

    public static bool IsNotANoopFacet(IFacet facet) => facet is {IsNoOp: false};

    public static bool IsTuple(Type type) => type.GetInterfaces().Any(i => i == typeof(ITuple));

    public static void ErrorOnDuplicates(this IEnumerable<ActionHolder> actions) {
        var names = actions.Select(s => s.Name).ToArray();
        var distinctNames = names.Distinct().ToArray();

        if (names.Length != distinctNames.Length) {
            var duplicates = names.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key);
            var errors = new List<string>();

            foreach (var name in duplicates) {
                var duplicateActions = actions.OrderBy(a => a.OwnerSpec.FullName).Where(s => s.Name == name);
                var error = duplicateActions.Aggregate("Name clash between user actions defined on", (s, a) => $"{s}{(s.EndsWith("defined on") ? " " : " and ")}{a.OwnerSpec.FullName}.{a.Name}");
                error += ": actions on and/or contributed to a menu or object must have unique names.";
                errors.Add(error);
            }

            throw new ReflectionException(string.Join(", ", errors));
        }
    }

    public record ActionHolder {
        private readonly object wrapped;

        public ActionHolder(IActionSpecImmutable actionSpecImmutable) => wrapped = actionSpecImmutable;

        public ActionHolder(IAssociationSpecImmutable associationSpecImmutable) => wrapped = associationSpecImmutable;

        public ActionHolder(IMenuActionImmutable menuActionImmutable) => wrapped = menuActionImmutable;

        public string Name => wrapped switch {
            IActionSpecImmutable action => action.Identifier.MemberName,
            IAssociationSpecImmutable association => association.Identifier.MemberName,
            IMenuActionImmutable menu => menu.Action.Identifier.MemberName,
            _ => ""
        };

        public ITypeSpecImmutable OwnerSpec => wrapped switch {
            IActionSpecImmutable action => action.OwnerSpec,
            IAssociationSpecImmutable association => association.OwnerSpec,
            IMenuActionImmutable menu => menu.Action.OwnerSpec,
            _ => null
        };
    }

    public static void AddIntegrationFacet(ISpecificationBuilder specification, Action<IMetamodelBuilder> action) {
        var integrationFacet = specification.GetFacet<IIntegrationFacet>();

        if (integrationFacet is null) {
            integrationFacet = new IntegrationFacet(specification, action);
            AddFacet(integrationFacet);
        }
        else {
            integrationFacet.AddAction(action);
        }
    }
}