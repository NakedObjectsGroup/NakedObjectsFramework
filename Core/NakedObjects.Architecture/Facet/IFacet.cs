// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facet {
    public interface IFacet {
        /// <summary>
        ///     The <see cref="ISpecification" /> of this facet. Set allows reparenting of Facet.
        /// </summary>
        /// <para>
        ///     Used by Facet decorators.
        /// </para>
        ISpecification Specification { get; set; }

        /// <summary>
        ///     Whether this facet implementation is a no-op
        /// </summary>
        bool IsNoOp { get; }

        /// <summary>
        ///     Determines the type of this facet to be stored under.
        /// </summary>
        /// <para>
        ///     The framework looks for <see cref="IFacet" />s of certain well-known facet types.
        ///     Each facet implementation must specify which type of facet it corresponds to.
        ///     This therefore allows the (rules of the) programming model to be varied
        ///     without impacting the rest of the framework.
        /// </para>
        /// <para>
        ///     For example, the <see cref="IActionInvocationFacet" /> specifies the facet to
        ///     invoke an action.  The typical implementation of this wraps a <c>public</c>
        ///     method.  However, a different facet factory could be installed that creates
        ///     facet also of type <see cref="IActionInvocationFacet" /> but that have some other
        ///     rule, such as requiring an <i>action</i> prefix, or that decorate the
        ///     interaction by logging it, for example.
        /// </para>
        Type FacetType { get; }

        /// <summary>
        ///     Whether this facet implementation should replace existing(none-noop) implementations.
        /// </summary>
        bool CanAlwaysReplace { get; }
    }
}