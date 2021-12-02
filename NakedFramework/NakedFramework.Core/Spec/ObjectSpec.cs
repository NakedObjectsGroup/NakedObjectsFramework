// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Spec;

public sealed class ObjectSpec : TypeSpec, IObjectSpec {
    private readonly IDictionary<string, IActionSpec[]> locallyContributedActions = new Dictionary<string, IActionSpec[]>();
    private readonly ILogger<ObjectSpec> logger;
    private IActionSpec[] collectionContributedActions;
    private IActionSpec[] combinedActions;
    private IActionSpec[] contributedActions;
    private IActionSpec[] finderActions;
    private IAssociationSpec[] objectFields;

    public ObjectSpec(SpecFactory memberFactory,
                      IObjectSpecImmutable innerSpec,
                      INakedFramework framework,
                      ILogger<ObjectSpec> logger) :
        base(memberFactory, innerSpec, framework) => this.logger = logger;

    private IActionSpec[] ContributedActions => contributedActions ??= MemberFactory.CreateActionSpecs(InnerSpec.OrderedContributedActions);

    protected override PersistableType GetPersistable() =>
        InnerSpec.ContainsFacet<INotPersistedFacet>()
            ? PersistableType.Transient
            : PersistableType.UserPersistable;

    #region IObjectSpec Members

    private IAssociationSpec[] ObjectFields => objectFields ??= InnerSpec.OrderedFields.Select(element => MemberFactory.CreateAssociationSpec(element)).ToArray();

    public IAssociationSpec[] Properties => ObjectFields;

    public IAssociationSpec GetProperty(string id) {
        try {
            return Properties.First(f => f.Id.Equals(id));
        }
        catch (InvalidOperationException) {
            throw new ReflectionException(logger.LogAndReturn($"No field called '{id}' in '{SingularName}'"));
        }
    }

    public override IActionSpec[] GetActions() {
        if (combinedActions is null) {
            var ca = new List<IActionSpec>();
            ca.AddRange(ObjectActions);
            ca.AddRange(ContributedActions);
            combinedActions = ca.ToArray();
        }

        return combinedActions;
    }

    public IActionSpec[] GetCollectionContributedActions() => collectionContributedActions ??= MemberFactory.CreateActionSpecs(InnerSpec.OrderedCollectionContributedActions);

    public IActionSpec[] GetFinderActions() => finderActions ??= MemberFactory.CreateActionSpecs(InnerSpec.OrderedFinderActions);

    public IActionSpec[] GetLocallyContributedActions(ITypeSpec typeSpec, string id) {
        if (!locallyContributedActions.ContainsKey(id)) {
            locallyContributedActions[id] = GetActions().Where(oa => oa.IsLocallyContributedTo(typeSpec, id)).ToArray();
        }

        return locallyContributedActions[id];
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.