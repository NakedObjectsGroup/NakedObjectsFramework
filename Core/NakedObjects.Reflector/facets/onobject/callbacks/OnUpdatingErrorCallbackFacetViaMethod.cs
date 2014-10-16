// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    public class OnUpdatingErrorCallbackFacetViaMethod : OnUpdatingErrorCallbackFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public OnUpdatingErrorCallbackFacetViaMethod(MethodInfo method, ISpecification holder)
            : base(holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override string Invoke(INakedObject nakedObject, Exception exception) {
            return (string) InvokeUtils.Invoke(method, nakedObject.Object, new object[] {exception});
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }
}