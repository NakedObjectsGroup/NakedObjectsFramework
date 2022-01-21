// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class DescribedAsViaAboutMethodFacet : AbstractViaAboutMethodFacet, IDescribedAsFacet {
    private readonly ILogger<DescribedAsViaAboutMethodFacet> logger;

    public DescribedAsViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutHelpers.AboutType aboutType, ILogger<DescribedAsViaAboutMethodFacet> logger)
        : base(typeof(IDescribedAsFacet), holder, method, aboutType) =>
        this.logger = logger;

    public string Description(INakedObjectAdapter adapter) => GetAbout(adapter.GetDomainObject()).Description;

    public IAbout GetAbout(object target) => InvokeAboutMethod(target, AboutTypeCodes.Name, false, true);
}

// Copyright (c) Naked Objects Group Ltd.