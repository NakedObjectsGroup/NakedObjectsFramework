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
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class NamedViaAboutMethodFacet : AbstractViaAboutMethodFacet, INamedFacet {
    private readonly ILogger<NamedViaAboutMethodFacet> logger;

    public NamedViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutHelpers.AboutType aboutType, ILogger<NamedViaAboutMethodFacet> logger)
        : base(typeof(IDisableForContextFacet), holder, method, aboutType) =>
        this.logger = logger;

    public string Value => GetAbout().Name;
    public string ShortName => GetAbout().Name;
    public string SimpleName => GetAbout().Name;
    public string NaturalName => GetAbout().Name;
    public string CapitalizedName => GetAbout().Name;

    public IAbout GetAbout() => InvokeAboutMethod(null, AboutTypeCodes.Name);
}

// Copyright (c) Naked Objects Group Ltd.