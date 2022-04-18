// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Audit;

[Serializable]
public sealed class AuditActionInvocationFacet : ActionInvocationFacetAbstract {
    private readonly IIdentifier identifier;
    private readonly IActionInvocationFacet underlyingFacet;

    public AuditActionInvocationFacet(IActionInvocationFacet underlyingFacet, IIdentifier identifier) {
        this.underlyingFacet = underlyingFacet;
        this.identifier = identifier;
    }

    public override bool IsQueryOnly => underlyingFacet.IsQueryOnly;

    public override MethodInfo ActionMethod => underlyingFacet.ActionMethod;

    public override Type ReturnType => underlyingFacet.ReturnType;

    public override Type ElementType => underlyingFacet.ElementType;

    public override Type OnType => underlyingFacet.OnType;

    public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, INakedFramework framework) {
        if (framework.ServiceProvider.GetService<IAuditManager>() is { } auditManager) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, framework);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, framework);
        }

        throw new NakedObjectSystemException($"Attempting 'Invoke' audit on {identifier} but missing AuditManager");
    }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, INakedFramework framework) {
        if (framework.ServiceProvider.GetService<IAuditManager>() is { } auditManager) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, framework);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, resultPage, framework);
        }

        throw new NakedObjectSystemException($"Attempting 'Invoke' audit on {identifier} but missing AuditManager");
    }
}