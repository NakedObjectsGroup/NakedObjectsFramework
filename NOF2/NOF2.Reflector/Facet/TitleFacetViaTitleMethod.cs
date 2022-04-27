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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NOF2.Title;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class TitleFacetViaTitleMethod : TitleFacetAbstract, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public TitleFacetViaTitleMethod(MethodInfo method, ILogger<TitleFacetViaTitleMethod> logger) => methodWrapper = new MethodSerializationWrapper(method, logger);

    public override string GetTitle(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => methodWrapper.Invoke<ITitle>(nakedObjectAdapter.GetDomainObject()).TitleString();

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.