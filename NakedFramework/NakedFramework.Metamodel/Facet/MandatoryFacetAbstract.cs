// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class MandatoryFacetAbstract : MarkerFacetAbstract, IMandatoryFacet {
        protected MandatoryFacetAbstract(ISpecification holder)
            : base(Type, holder) { }

        public static Type Type => typeof(IMandatoryFacet);

        #region IMandatoryFacet Members

        public virtual string Invalidates(IInteractionContext ic) => IsRequiredButNull(ic.ProposedArgument) ? Resources.NakedObjects.Mandatory : null;

        public virtual Exception CreateExceptionFor(IInteractionContext ic) => new InvalidMandatoryException(ic, Invalidates(ic));

        public virtual bool IsOptional => !IsMandatory;

        public abstract bool IsMandatory { get; }

        public virtual bool IsRequiredButNull(INakedObjectAdapter nakedObjectAdapter) => IsMandatory && nakedObjectAdapter == null;

        #endregion
    }
}