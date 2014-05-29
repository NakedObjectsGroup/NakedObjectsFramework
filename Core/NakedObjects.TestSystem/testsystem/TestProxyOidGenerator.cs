// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.TestSystem {
    public class TestProxyOidGenerator : IOidGenerator {
        private int persistentId = 90000;
        private int transientId = 1;

        #region IOidGenerator Members

        public void Init() {}

        public void Shutdown() {}

        public IOid CreateTransientOid(object obj) {
            return CreateOid();
        }

        public void ConvertTransientToPersistentOid(IOid oid) {
            ((TestProxyOid) oid).MakePersistent(persistentId++);
            ((TestProxyOid) oid).previous = CreateOid();
        }

        public void ConvertPersistentToTransientOid(IOid oid) {}

        #endregion

        public IOid RestoreOid(string[] encodedData) {
            return null;
        }

        private IOid CreateOid() {
            return new TestProxyOid(transientId++);
        }

        public string Name() {
            return "";
        }
    }
}