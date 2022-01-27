// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class TitleFacetViaProperty : TitleFacetAbstract, IImperativeFacet {
    private readonly ILogger<TitleFacetViaProperty> logger;
    private readonly MethodInfo method;

    [field: NonSerialized] private Func<object, object[], object> methodDelegate;

    public TitleFacetViaProperty(MethodInfo method, ISpecification holder, ILogger<TitleFacetViaProperty> logger)
        : base(holder) {
        this.method = method;
        this.logger = logger;
        methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    public override string GetTitle(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        var obj = methodDelegate.Invoke<object>(method, nakedObjectAdapter.GetDomainObject(), Array.Empty<object>());
        return obj == null ? null : framework.NakedObjectManager.CreateAdapter(obj, null, null).TitleString();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => method;

    public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.