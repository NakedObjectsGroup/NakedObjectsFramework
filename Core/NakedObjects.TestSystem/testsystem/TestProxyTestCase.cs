// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.TestSystem {
    public abstract class TestProxyTestCase {
        protected TestProxySystem system;

        public virtual void SetUp() {
            if (system == null) {
                CreateSystem();
                InitSystem();
            }
        }

        public virtual void TearDown() {
            system = null;
        }

        protected void InitSystem() {
            system.Init();
        }

        protected void CreateSystem() {
            system = new TestProxySystem();
        }

        protected INakedObject CreateValueAdapter(object domainObject) {
            return system.CreateValueAdapter(domainObject);
        }
    }
}