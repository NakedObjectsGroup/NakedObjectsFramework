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
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NOF2.About;
using static NOF2.Reflector.Facet.AboutHelpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class MemberNamedViaAboutMethodFacet : AbstractViaAboutMethodFacet, IMemberNamedFacet {
    private readonly AboutTypeCodes aboutCode;
    private readonly int index;
    private readonly string inferredName;
    private readonly ILogger<MemberNamedViaAboutMethodFacet> logger;

    public MemberNamedViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutType aboutType, string inferredName, ILogger<MemberNamedViaAboutMethodFacet> logger)
        : base(typeof(IMemberNamedFacet), holder, method, aboutType, logger) {
        this.inferredName = NameUtils.NaturalName(TrimActionPrefix(inferredName, aboutType, aboutCode));
        aboutCode = AboutTypeCodes.Name;
        this.logger = logger;
    }

    public MemberNamedViaAboutMethodFacet(MethodInfo method, ISpecification holder, AboutType aboutType, string[] inferredNames, int index, ILogger<MemberNamedViaAboutMethodFacet> logger)
        : base(typeof(IMemberNamedFacet), holder, method, aboutType, logger) {
        inferredName = NameUtils.NaturalName(inferredNames[index]);
        aboutCode = AboutTypeCodes.Parameters;
        this.index = index;
        this.logger = logger;
    }

    public string FriendlyName(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        switch (aboutCode) {
            case AboutTypeCodes.Name: {
                var aboutName = GetAbout(nakedObjectAdapter, framework, false)?.Name ?? inferredName;
                return string.IsNullOrEmpty(aboutName) ? inferredName : aboutName;
            }
            case AboutTypeCodes.Parameters when GetAbout(nakedObjectAdapter, framework, true) is ActionAbout actionAbout: {
                var parameterNames = actionAbout.ParamLabels ?? Array.Empty<string>();
                var aboutName = parameterNames.Length > index ? parameterNames[index] : inferredName;
                return string.IsNullOrEmpty(aboutName) ? inferredName : aboutName;
            }
            default:
                return inferredName;
        }
    }

    public string FriendlyName() => inferredName;

    private static string TrimActionPrefix(string name, AboutType aboutType, AboutTypeCodes aboutCode) =>
        aboutType is AboutType.Action &&
        aboutCode is AboutTypeCodes.Name &&
        name.ToLower().StartsWith("action")
            ? name.Remove(0, 6)
            : name;

    public IAbout GetAbout(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework, bool flagNull) => InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), aboutCode, false, flagNull);
}

// Copyright (c) Naked Objects Group Ltd.