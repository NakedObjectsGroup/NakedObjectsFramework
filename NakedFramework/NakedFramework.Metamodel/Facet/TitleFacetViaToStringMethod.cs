// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class TitleFacetViaToStringMethod : TitleFacetAbstract, IImperativeFacet {
    private readonly MethodSerializationWrapper method;

    public TitleFacetViaToStringMethod(MethodInfo method, ILogger<TitleFacetViaToStringMethod> logger) {
        if (method is not null) {
            this.method = new MethodSerializationWrapper(method, logger, method.GetParameters().Select(p => p.ParameterType).ToArray());
        }
    }

    public MethodInfo GetMethod() => method?.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => method?.GetMethodDelegate();

    public override string GetTitle(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => nakedObjectAdapter.Object.ToString();

    public override string GetTitleWithMask(string mask, INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => method is not null ? method.Invoke<string>(nakedObjectAdapter.GetDomainObject(), new object[] { mask }) : GetTitle(nakedObjectAdapter, framework);
}

// Copyright (c) Naked Objects Group Ltd.