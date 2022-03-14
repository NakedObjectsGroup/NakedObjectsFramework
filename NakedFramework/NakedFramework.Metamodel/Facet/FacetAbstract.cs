// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Facet;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class FacetAbstract : IFacet, IDeserializationCallback {
    #region IDeserializationCallback Members

    public virtual void OnDeserialization(object sender) {
        // hook if we need a log
    }

    #endregion

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