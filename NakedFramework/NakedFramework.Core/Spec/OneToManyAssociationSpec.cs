// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using static NakedFramework.Core.Util.ToStringHelpers;

namespace NakedFramework.Core.Spec;

public sealed class OneToManyAssociationSpec : AssociationSpecAbstract, IOneToManyAssociationSpec {
    public OneToManyAssociationSpec(IOneToManyAssociationSpecImmutable association, INakedFramework framework)
        : base(association, framework) {
        IsASet = association.ContainsFacet<IIsASetFacet>();

        ElementSpec = framework.MetamodelManager.GetSpecification(association.GetElementSpec(framework.MetamodelManager.Metamodel));
    }

    public override bool IsAutoCompleteEnabled => false;

    public override INakedObjectAdapter[] GetCompletions(INakedObjectAdapter nakedObjectAdapter, string autoCompleteParm) => Array.Empty<INakedObjectAdapter>();

    private INakedObjectAdapter GetCollection(INakedObjectAdapter inObjectAdapter) {
        var collection = GetFacet<IPropertyAccessorFacet>().GetProperty(inObjectAdapter, Framework);
        if (collection is null) {
            return null;
        }

        var adapterFor = Framework.NakedObjectManager.CreateAggregatedAdapter(inObjectAdapter, ((IAssociationSpec)this).Id, collection);
        SetResolveStateForDerivedCollections(adapterFor);
        return adapterFor;
    }

    private void SetResolveStateForDerivedCollections(INakedObjectAdapter adapterFor) {
        var isDerived = !IsPersisted;
        if (isDerived && !adapterFor.ResolveState.IsResolved()) {
            if (adapterFor.GetAsEnumerable(Framework.NakedObjectManager).Any()) {
                adapterFor.ResolveState.Handle(Events.StartResolvingEvent);
                adapterFor.ResolveState.Handle(Events.EndResolvingEvent);
            }
        }
    }

    public override string ToString() =>
        $"{NameAndHashCode(this)} [{base.ToString()},persisted={IsPersisted},type={(ReturnSpec is null ? "unknown" : ReturnSpec.ShortName)}]";

    #region IOneToManyAssociationSpec Members

    public override INakedObjectAdapter GetNakedObject(INakedObjectAdapter inObjectAdapter) => GetCollection(inObjectAdapter);

    public override IObjectSpec ElementSpec { get; }

    public bool IsASet { get; }

    public override bool IsEmpty(INakedObjectAdapter inObjectAdapter) => Count(inObjectAdapter) == 0;

    public int Count(INakedObjectAdapter inObjectAdapter) => Framework.Persistor.CountField(inObjectAdapter, Id);

    public override bool IsInline => false;

    public override bool IsMandatory => false;

    public override INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter) => null;

    public override TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter) => TypeOfDefaultValue.Implicit;

    public override void ToDefault(INakedObjectAdapter target) { }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.