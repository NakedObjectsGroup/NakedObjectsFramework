// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Modify {
    public class PropertySetterFacetViaSetterMethod : PropertySetterFacetAbstract, IImperativeFacet {
        private readonly PropertyInfo property;

        public PropertySetterFacetViaSetterMethod(PropertyInfo property, IFacetHolder holder)
            : base(holder) {
            this.property = property;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return property.GetSetMethod();
        }

        #endregion

        public override void SetProperty(INakedObject nakedObject, INakedObject value) {
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