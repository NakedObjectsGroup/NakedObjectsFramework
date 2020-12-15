// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class FacetAbstract : IFacet, IDeserializationCallback {
        private ISpecification holder;

        protected FacetAbstract(Type facetType, ISpecification holder) {
            FacetType = facetType;
            this.holder = holder;
        }

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender) {
            // hook if we need a log
        }

        #endregion

        #region IFacet Members

        public virtual ISpecification Specification {
            get => holder;
            set => holder = value;
        }

        /// <summary>
        ///     Assume implementation is <i>not</i> a no-op.
        /// </summary>
        /// <para>
        ///     No-op implementations should override and return <c>true</c>.
        /// </para>
        public virtual bool IsNoOp => false;

        public Type FacetType { get; }

        /// <summary>
        ///     Default implementation of this method that returns <c>true</c>, ie
        ///     should replace non-<see cref="IsNoOp" /> implementations.
        /// </summary>
        /// <para>
        ///     Implementations that don't wish to replace non-<see cref="IsNoOp" /> implementations
        ///     should override and return <c>false</c>.
        /// </para>
        public virtual bool CanAlwaysReplace => true;

        #endregion

        //protected static Func<object, object[], object> LogNull((Func<object, object[], object>, string) pair) {
        //    var (delFunc, warning) = pair;
        //    if (delFunc == null && !string.IsNullOrWhiteSpace(warning)) {
        //        Log.Warn(warning);
        //    }

        //    return delFunc;
        //}

        protected static Func<object, object[], object> LogNull((Func<object, object[], object>, string) pair, ILogger logger) {
            var (delFunc, warning) = pair;
            if (delFunc == null && !string.IsNullOrWhiteSpace(warning)) {
                logger.LogWarning(warning);
            }

            return delFunc;
        }

        public override string ToString() {
            var details = "";
            if (typeof(IValidatingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += "Validating";
            }

            if (typeof(IDisablingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += $"{(details.Length > 0 ? ";" : "")}Disabling";
            }

            if (typeof(IHidingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += $"{(details.Length > 0 ? ";" : "")}Hiding";
            }

            if (!"".Equals(details)) {
                details = $"interaction={details},";
            }

            if (GetType() != FacetType) {
                var sFacetType = FacetType.FullName;
                details += $"type={sFacetType.Substring(sFacetType.LastIndexOf('.') + 1)}";
            }

            var stringValues = ToStringValues();
            if (!"".Equals(stringValues)) {
                details += ",";
            }

            var typeName = GetType().FullName;
            var last = typeName.IndexOf('`');
            if (last == -1) {
                last = typeName.Length - 1;
            }

            return $"{typeName.Substring(typeName.LastIndexOf('.', last) + 1)}[{details}{stringValues}]";
        }

        protected virtual string ToStringValues() => "";
    }
}