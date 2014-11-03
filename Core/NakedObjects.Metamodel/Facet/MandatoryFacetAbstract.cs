// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Except;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class MandatoryFacetAbstract : MarkerFacetAbstract, IMandatoryFacet {
        protected MandatoryFacetAbstract(ISpecification holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IMandatoryFacet); }
        }

        #region IMandatoryFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return IsRequiredButNull(ic.ProposedArgument) ? Resources.NakedObjects.Mandatory : null;
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidMandatoryException(ic, Invalidates(ic));
        }

        public virtual bool IsOptional {
            get { return !IsMandatory; }
        }

        public abstract bool IsMandatory { get; }

        public virtual bool IsRequiredButNull(INakedObject nakedObject) {
            return IsMandatory && nakedObject == null;
        }

        #endregion
    }
}