// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Metamodel.Error;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class MandatoryFacetAbstract : MarkerFacetAbstract, IMandatoryFacet {
    public override Type FacetType => typeof(IMandatoryFacet);

    #region IMandatoryFacet Members

    public virtual string Invalidates(IInteractionContext ic) => IsRequiredButNull(ic.ProposedArgument) ? NakedObjects.Resources.NakedObjects.Mandatory : null;

    public virtual Exception CreateExceptionFor(IInteractionContext ic) => new InvalidMandatoryException(ic, Invalidates(ic));

    public virtual bool IsOptional => !IsMandatory;

    public abstract bool IsMandatory { get; }

    public virtual bool IsRequiredButNull(INakedObjectAdapter nakedObjectAdapter) => IsMandatory && nakedObjectAdapter == null;

    #endregion
}