// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Threading;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Exception;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public abstract class MaxLengthFacetAbstract : SingleIntValueFacetAbstract, IMaxLengthFacet {
        protected MaxLengthFacetAbstract(int intValue, ISpecification holder)
            : base(Type, holder, intValue) { }

        public static Type Type => typeof(IMaxLengthFacet);

        #region IMaxLengthFacet Members

        /// <summary>
        ///     Whether the provided argument exceeds the <see cref="SingleIntValueFacetAbstract.Value" /> maximum length}.
        /// </summary>
        public virtual bool Exceeds(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.GetDomainObject() is string str) {
                var maxLength = Value;
                return maxLength != 0 && str.Length > maxLength;
            }

            return false;
        }

        public virtual string Invalidates(IInteractionContext ic) {
            var proposedArgument = ic.ProposedArgument;
            return !Exceeds(proposedArgument) ? null : string.Format(NakedObjects.Resources.NakedObjects.MaximumLengthMismatch, Value);
        }

        public virtual System.Exception CreateExceptionFor(IInteractionContext ic) => new InvalidMaxLengthException(ic, Value, Invalidates(ic));

        #endregion

        protected override string ToStringValues() => Value == 0 ? "unlimited" : Value.ToString(Thread.CurrentThread.CurrentCulture);
    }
}