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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedObjects.Reflector.Utils;

namespace NakedObjects.Reflector.Facet {
    [Serializable]
    public sealed class PropertyAccessorFacetViaContributedAction : FacetAbstract, IPropertyAccessorFacet, IImperativeFacet {
        private readonly MethodInfo propertyMethod;
        
        public PropertyAccessorFacetViaContributedAction(MethodInfo propertyMethod, ISpecification holder, ILogger<PropertyAccessorFacetViaContributedAction> logger)
            : base(typeof(IPropertyAccessorFacet), holder) {
            this.propertyMethod = propertyMethod;
           
            PropertyDelegate = LogNull(DelegateUtils.CreateDelegate(propertyMethod), logger);
        }

        private Func<object, object[], object> PropertyDelegate { get; set; }
        public MethodInfo GetMethod() => propertyMethod;

        public Func<object, object[], object> GetMethodDelegate() => PropertyDelegate;

        #region IPropertyAccessorFacet Members

        public object GetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework nakedObjectsFramework) {
            try {
                var spec = nakedObjectsFramework.MetamodelManager.GetSpecification(propertyMethod.DeclaringType);
                var service = nakedObjectsFramework.ServicesManager.GetService(spec as IServiceSpec);
                return PropertyDelegate.Invoke<object>(propertyMethod, service.GetDomainObject(),  new[] { nakedObjectAdapter.GetDomainObject()});
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException($"Exception executing {propertyMethod}", e);
                return null;
            }
        }

        #endregion

        protected override string ToStringValues() => $"method={propertyMethod}";
    }
}