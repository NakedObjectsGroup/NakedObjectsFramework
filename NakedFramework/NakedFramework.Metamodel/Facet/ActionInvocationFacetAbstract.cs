// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class ActionInvocationFacetAbstract : FacetAbstract, IActionInvocationFacet {
    public override Type FacetType => typeof(IActionInvocationFacet);

    public abstract MethodInfo GetMethod();
    public abstract Func<object, object[], object> GetMethodDelegate();

    #region IActionInvocationFacet Members

    public abstract Type OnType { get; }
    public abstract Type ReturnType { get; }
    public abstract Type ElementType { get; }

    //Note: Some passed-in components are not used within NOF code, but are provided for third-party customization. DO NOT REMOVE.
    public abstract INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, INakedFramework framework);
    public abstract INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, INakedFramework framework);
    public abstract bool IsQueryOnly { get; }

    #endregion
}