// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class MemberNamedViaAboutMethodFacet : AbstractViaAboutMethodFacet, IMemberNamedFacet {
    private readonly string inferredName;
    private readonly ILogger<MemberNamedViaAboutMethodFacet> logger;

    public MemberNamedViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutHelpers.AboutType aboutType, string inferredName, ILogger<MemberNamedViaAboutMethodFacet> logger)
        : base(typeof(IMemberNamedFacet), holder, method, aboutType) {
        this.inferredName = NameUtils.NaturalName(inferredName);
        this.logger = logger;
    }

    string IMemberNamedFacet.FriendlyName(INakedObjectAdapter nakedObjectAdapter) {
        var aboutName = GetAbout(nakedObjectAdapter).Name;
        return string.IsNullOrEmpty(aboutName) ? inferredName : aboutName;
    }

    public IAbout GetAbout(INakedObjectAdapter nakedObjectAdapter) => InvokeAboutMethod(nakedObjectAdapter.Object, AboutTypeCodes.Name);
}

// Copyright (c) Naked Objects Group Ltd.