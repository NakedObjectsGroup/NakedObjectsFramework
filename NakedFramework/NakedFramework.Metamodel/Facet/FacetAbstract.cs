// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class FacetAbstract : IFacet, IDeserializationCallback {
    protected FacetAbstract(Type facetType) {

    }

    #region IDeserializationCallback Members

    public virtual void OnDeserialization(object sender) {
        // hook if we need a log
    }

    #endregion

    protected static Func<object, object[], object> LogNull((Func<object, object[], object>, string) pair, ILogger logger) {
        var (delFunc, warning) = pair;
        if (delFunc is null && !string.IsNullOrWhiteSpace(warning)) {
            logger.LogInformation(warning);
        }

        return delFunc;
    }

    public override string ToString() {
        var details = "";
        if (this is IValidatingInteractionAdvisor) {
            details += "Validating";
        }

        if (this is IDisablingInteractionAdvisor) {
            details += $"{(details.Length > 0 ? ";" : "")}Disabling";
        }

        if (this is IHidingInteractionAdvisor) {
            details += $"{(details.Length > 0 ? ";" : "")}Hiding";
        }

        if (!string.IsNullOrEmpty(details)) {
            details = $"interaction={details},";
        }

        if (GetType() != FacetType) {
            var sFacetType = FacetType.FullName;
            details += $"type={sFacetType[(sFacetType.LastIndexOf('.') + 1)..]}";
        }

        var stringValues = ToStringValues();
        if (!string.IsNullOrEmpty(stringValues)) {
            details += ",";
        }

        var typeName = GetType().FullName;
        var last = typeName.IndexOf('`');
        if (last == -1) {
            last = typeName.Length - 1;
        }

        return $"{typeName[(typeName.LastIndexOf('.', last) + 1)..]}[{details}{stringValues}]";
    }

    protected virtual string ToStringValues() => "";

    #region IFacet Members

    /// <summary>
    ///     Assume implementation is <i>not</i> a no-op.
    /// </summary>
    /// <para>
    ///     No-op implementations should override and return <c>true</c>.
    /// </para>
    public virtual bool IsNoOp => false;

    public abstract Type FacetType { get; }

    /// <summary>
    ///     Default implementation of this method that returns <c>true</c>, ie
    ///     should replace non-<see cref="IsNoOp" /> implementations.
    /// </summary>
    /// <para>
    ///     Implementations that don't wish to replace non-<see cref="IsNoOp" /> implementations
    ///     should override and return <c>false</c>.
    /// </para>
    public virtual bool CanAlwaysReplace => true;

    public virtual bool CanNeverBeReplaced => false;

    #endregion
}