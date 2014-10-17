// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Exception;

namespace NakedObjects.Metamodel.Facet {
    public abstract class MaskFacetAbstract : SingleStringValueFacetAbstract, IMaskFacet {
        protected MaskFacetAbstract(string stringValue, ISpecification holder)
            : base(Type, holder, stringValue) {}

        public static Type Type {
            get { return typeof (IMaskFacet); }
        }

        #region IMaskFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (DoesNotMatch(proposedArgument)) {
                return string.Format(Resources.NakedObjects.MaskMismatch, proposedArgument.TitleString(), Value);
            }
            return null;
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidMaskException(ic, Invalidates(ic));
        }

        public abstract bool DoesNotMatch(INakedObject nakedObject);

        #endregion
    }
}