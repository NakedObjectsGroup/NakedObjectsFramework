// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.TestSystem {
    public class TestProxyContainerInjector : IContainerInjector {
        #region IContainerInjector Members

        public object[] ServicesToInject {
            set { }
        }

        public void InitDomainObject(object obj) {}
        public void InitInlineObject(object root, object inlineObject) {}

        #endregion
    }
}