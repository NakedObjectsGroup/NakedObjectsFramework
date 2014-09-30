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
    public class PropertyClearFacetViaClearMethod : PropertyClearFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public PropertyClearFacetViaClearMethod(MethodInfo method, IFacetHolder holder)
            : base(holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override void ClearProperty(INakedObject nakedObject, ILifecycleManager persistor) {
            InvokeUtils.Invoke(method, nakedObject);
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }
}