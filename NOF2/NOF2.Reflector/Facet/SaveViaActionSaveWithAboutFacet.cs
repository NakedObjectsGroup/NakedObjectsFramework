﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
using NakedFramework.Core.Util;
using NakedFramework.ParallelReflector.Utils;
using NOF2.About;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class SaveViaActionSaveWithAboutFacet : AbstractViaAboutMethodFacet, ISaveFacet {
    private readonly MethodInfo saveMethod;

    public SaveViaActionSaveWithAboutFacet(MethodInfo saveMethod, MethodInfo aboutMethod, ISpecification holder, ILogger<SaveViaActionSaveWithAboutFacet> logger)
        : base(Type, holder, aboutMethod, AboutHelpers.AboutType.Action, logger) {
        this.saveMethod = saveMethod;
        SaveDelegate = LogNull(DelegateUtils.CreateDelegate(this.saveMethod), logger);
    }

    public Func<object, object[], object> SaveDelegate { get; set; }

    public static Type Type => typeof(ISaveFacet);

    public string Save(INakedFramework framework, INakedObjectAdapter nakedObject) {
        var msg = Validate(nakedObject, framework);
        if (msg is not null) {
            return msg;
        }

        SaveDelegate.Invoke(saveMethod, nakedObject.GetDomainObject(), Array.Empty<object>());
        return null;
    }

    public string Validate(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Valid, false, true) is ActionAbout actionAbout) {
            return actionAbout.Usable ? null : string.IsNullOrWhiteSpace(actionAbout.UnusableReason) ? "Invalid Save" : actionAbout.UnusableReason;
        }

        return null;
    }
}