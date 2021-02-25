// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NakedFramework;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ExecutedControlMethodFacet : FacetAbstract, IExecutedControlMethodFacet {
        private readonly IDictionary<MethodInfo, Where> methodToWhere = new Dictionary<MethodInfo, Where>();

        public ExecutedControlMethodFacet(MethodInfo method, Where where, ISpecification holder)
            : base(typeof(IExecutedControlMethodFacet), holder) =>
            methodToWhere[method] = where;

        #region IExecutedControlMethodFacet Members

        public Where ExecutedWhere(MethodInfo method) => methodToWhere.ContainsKey(method) ? methodToWhere[method] : Where.Default;

        public void AddMethodExecutedWhere(MethodInfo method, Where where) => methodToWhere[method] = where;

        #endregion

        protected override string ToStringValues() {
            var sb = new StringBuilder();
            foreach (var (key, value) in methodToWhere) {
                sb.Append($"{key} Executed = {value}").Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}