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
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Configuration;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class PropertyAccessorFacetViaContributedAction : FacetAbstract, IPropertyAccessorFacet, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public PropertyAccessorFacetViaContributedAction(MethodInfo method, ILogger<PropertyAccessorFacetViaContributedAction> logger) => methodWrapper = MethodSerializationWrapper.Wrap(method, logger);

    public override Type FacetType => typeof(IPropertyAccessorFacet);

    #region IPropertyAccessorFacet Members

    public object GetProperty(INakedObjectAdapter nakedObjectAdapter, INakedFramework nakedFramework) {
        try {
            var spec = nakedFramework.MetamodelManager.GetSpecification(GetMethod().DeclaringType);
            var service = nakedFramework.ServicesManager.GetService(spec as IServiceSpec);
            return methodWrapper.Invoke<object>(service, new[] { nakedObjectAdapter });
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