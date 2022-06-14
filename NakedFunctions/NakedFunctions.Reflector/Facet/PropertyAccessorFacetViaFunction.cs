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
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class PropertyAccessorFacetViaFunction : FacetAbstract, IPropertyAccessorFacet, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public PropertyAccessorFacetViaFunction(MethodInfo method, ILogger<PropertyAccessorFacetViaFunction> logger) =>
        methodWrapper = SerializationFactory.Wrap(method, logger);

    public override Type FacetType => typeof(IPropertyAccessorFacet);

    #region IPropertyAccessorFacet Members

    public object GetProperty(INakedObjectAdapter nakedObjectAdapter, INakedFramework nakedFramework) {
        try {
            return methodWrapper.Invoke<object>(GetMethod().GetParameterValues(nakedObjectAdapter, nakedFramework));
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {GetMethod()}", e);
            return null;
        }
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