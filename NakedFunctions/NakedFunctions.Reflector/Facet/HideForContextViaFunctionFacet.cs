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
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class HideForContextViaFunctionFacet : FacetAbstract, IHideForContextFacet, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public HideForContextViaFunctionFacet(MethodInfo method, ILogger<HideForContextViaFunctionFacet> logger) => methodWrapper = SerializationFactory.Wrap(method, logger);

    public override Type FacetType => typeof(IHideForContextFacet);

    #region IHideForContextFacet Members

    public string Hides(IInteractionContext ic) => HiddenReason(ic.Target, ic.Framework);

    public Exception CreateExceptionFor(IInteractionContext ic) => new HiddenException(ic, Hides(ic));

    public string HiddenReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        var isHidden = methodWrapper.Invoke<bool>(GetMethod().GetParameterValues(nakedObjectAdapter, framework));
        return isHidden ? NakedObjects.Resources.NakedObjects.Hidden : null;
    }

    #endregion

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.