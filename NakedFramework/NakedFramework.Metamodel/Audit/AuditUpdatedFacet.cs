// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Audit;

[Serializable]
public sealed class AuditUpdatedFacet : CallbackFacetAbstract, IUpdatedCallbackFacet {
    private readonly IUpdatedCallbackFacet underlyingFacet;

    public AuditUpdatedFacet(IUpdatedCallbackFacet underlyingFacet) => this.underlyingFacet = underlyingFacet;

    public override void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (framework.ServiceProvider.GetService<IAuditManager>() is { } auditManager) {
            auditManager.Updated(nakedObjectAdapter, framework);
            underlyingFacet.Invoke(nakedObjectAdapter, framework);
        }
        else {
            throw new NakedObjectSystemException($"Attempting 'Updated' audit on {nakedObjectAdapter} but missing AuditManager");
        }
    }

    public override Type FacetType => typeof(IUpdatedCallbackFacet);
}