// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Persist;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Modify {
    public class PropertySetterFacetViaModifyMethod : PropertySetterFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public PropertySetterFacetViaModifyMethod(MethodInfo method, ISpecification holder)
            : base(holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override void SetProperty(INakedObject inObject, INakedObject value, INakedObjectTransactionManager transactionManager) {
            InvokeUtils.Invoke(method, inObject, new[] {value});
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}