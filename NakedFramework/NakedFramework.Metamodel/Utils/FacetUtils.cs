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
using NakedFramework.Architecture.Spec;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Utils {
    public static class FacetUtils {
        public static string[] SplitOnComma(string toSplit) {
            if (string.IsNullOrEmpty(toSplit)) {
                return new string[] { };
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

        public static bool IsNotANoopFacet(IFacet facet) => facet != null && !facet.IsNoOp;


        public static bool IsTuple(Type type) => type.GetInterfaces().Any(i => i == typeof(ITuple));

        public static bool IsAction(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                return genericTypeDefinition == typeof(Action<>);
            }

            return false;
        }



        public static int ValueTupleSize(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

            
                if (genericTypeDefinition == typeof(ValueTuple<>)) return 1;
                if (genericTypeDefinition == typeof(ValueTuple<,>)) return 2;
                if (genericTypeDefinition == typeof(ValueTuple<,,>)) return 3;

                if (genericTypeDefinition == typeof(ValueTuple<,,,>))
                {
                    throw new NotImplementedException("only support tuples up to size 3");
                }


            }

            return 0;
        }

        //public static bool IsTuple(Type type)
        //{
        //    if (type.IsGenericType)
        //    {
        //        var genericTypeDefinition = type.GetGenericTypeDefinition();
        //        return genericTypeDefinition == typeof(Tuple<>) ||
        //               genericTypeDefinition == typeof(Tuple<,>) ||
        //               genericTypeDefinition == typeof(Tuple<,,>);
        //    }

        //    return false;
        //}

        //public static bool IsEitherTuple(Type type) => IsValueTuple(type) || IsTuple(type);

    }
}