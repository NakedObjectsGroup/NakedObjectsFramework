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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;
using NOF2.About;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class SaveViaActionSaveWithAboutFacet : AbstractViaAboutMethodFacet, ISaveFacet, IMultipleImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public SaveViaActionSaveWithAboutFacet(MethodInfo saveMethod, MethodInfo aboutMethod, ILogger<SaveViaActionSaveWithAboutFacet> logger)
        : base(aboutMethod, AboutHelpers.AboutType.Action, logger) =>
        methodWrapper = SerializationFactory.Wrap(saveMethod, logger);

    public int Count => 2;

    public MethodInfo GetMethod(int index) => index switch {
        0 => methodWrapper.GetMethod(),
        1 => GetMethod(),
        _ => null
    };

    public Func<object, object[], object> GetMethodDelegate(int index) => index switch {
        0 => methodWrapper.GetMethodDelegate(),
        1 => GetMethodDelegate(),
        _ => null
    };

    public override Type FacetType => typeof(ISaveFacet);

    public string Save(INakedFramework framework, INakedObjectAdapter nakedObject, ILogger logger) {
        var msg = Validate(nakedObject, framework);
        if (msg is not null) {
            return msg;
        }

        methodWrapper.Invoke(nakedObject.GetDomainObject());
        return null;
    }

    public string Validate(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (InvokeAboutMethod(framework, nakedObjectAdapter.GetDomainObject(), AboutTypeCodes.Valid, false, true) is ActionAbout actionAbout) {
            return actionAbout.Usable ? null : string.IsNullOrWhiteSpace(actionAbout.UnusableReason) ? "Invalid Save" : actionAbout.UnusableReason;
        }

        return null;
    }
}