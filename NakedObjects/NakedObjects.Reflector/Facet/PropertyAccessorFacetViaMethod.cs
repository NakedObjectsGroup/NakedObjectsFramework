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
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class PropertyAccessorFacetViaMethod : FacetAbstract, IPropertyAccessorFacet, IImperativeFacet {
    private readonly MethodInfo propertyMethod;

    public PropertyAccessorFacetViaMethod(MethodInfo propertyMethod, ILogger<PropertyAccessorFacetViaMethod> logger) {
        this.propertyMethod = propertyMethod;
        PropertyDelegate = LogNull(DelegateUtils.CreateDelegate(propertyMethod), logger);
    }

    private Func<object, object[], object> PropertyDelegate { get; set; }
    public MethodInfo GetMethod() => propertyMethod;

    public Func<object, object[], object> GetMethodDelegate() => PropertyDelegate;

    public override Type FacetType => typeof(IPropertyAccessorFacet);

    #region IPropertyAccessorFacet Members

    public object GetProperty(INakedObjectAdapter nakedObjectAdapter, INakedFramework nakedFramework) {
        try {
            return PropertyDelegate.Invoke<object>(propertyMethod, nakedObjectAdapter.GetDomainObject(), Array.Empty<object>());
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {propertyMethod}", e);
            return null;
        }
    }

    #endregion

    protected override string ToStringValues() => $"method={propertyMethod}";
}