// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.Access {
    public class PropertyAccessorFacetViaAccessor : PropertyAccessorFacetAbstract, IImperativeFacet {
        private readonly PropertyInfo propertyMethod;

        public PropertyAccessorFacetViaAccessor(PropertyInfo property, IFacetHolder holder)
            : base(holder) {
            propertyMethod = property;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return propertyMethod.GetGetMethod();
        }

        #endregion

        public override object GetProperty(INakedObject nakedObject) {
            try {
                return propertyMethod.GetValue(nakedObject.GetDomainObject(), null);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + propertyMethod, e);
                return null;
            }
        }

        protected override string ToStringValues() {
            return "propertyMethod=" + propertyMethod;
        }
    }
}