// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Modify {
    public class PropertySetterFacetViaSetterMethod : PropertySetterFacetAbstract, IImperativeFacet {
        private readonly PropertyInfo property;

        public PropertySetterFacetViaSetterMethod(PropertyInfo property, ISpecification holder)
            : base(holder) {
            this.property = property;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return property.GetSetMethod();
        }

        #endregion

        public override void SetProperty(INakedObject nakedObject, INakedObject value, ITransactionManager transactionManager) {
            try {
                property.SetValue(nakedObject.GetDomainObject(), value.GetDomainObject(), null);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + property, e);
            }
        }

        protected override string ToStringValues() {
            return "property=" + property;
        }
    }
}