// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFunctions.Reflector.Utils;
using NakedObjects;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;

namespace NakedFunctions.Reflector.Facet {
    [Serializable]
    public sealed class PropertyAccessorFacetViaFunction : FacetAbstract, IPropertyAccessorFacet {
        private readonly MethodInfo method;

        public PropertyAccessorFacetViaFunction(MethodInfo method, ISpecification holder)
            : base(typeof(IPropertyAccessorFacet), holder) =>
            this.method = method;

        #region IPropertyAccessorFacet Members

        public object GetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework nakedObjectsFramework) {
            try {
                return method.Invoke(null, method.GetParameterValues(nakedObjectAdapter, nakedObjectsFramework));
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException($"Exception executing {method}", e);
                return null;
            }
        }

        #endregion

        protected override string ToStringValues() => $"method={method}";
    }
}