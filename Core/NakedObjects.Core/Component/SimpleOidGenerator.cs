// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    /// <summary>
    ///     Generates OIDs based on the system clock
    /// </summary>
    public class SimpleOidGenerator : IOidGenerator {
        private readonly IMetamodelManager metamodel;
        private readonly long start;
        private long persistentSerialNumber;
        private long transientSerialNumber;

        public SimpleOidGenerator(IMetamodelManager metamodel)
            : this(metamodel, 0L) {}

        public SimpleOidGenerator(IMetamodelManager metamodel, long start) {
            Assert.AssertNotNull(metamodel);

            this.metamodel = metamodel;
            this.start = start;

            // TODO: REVIEW This is simple, but not reliable, fix to try to ensure that ids on
            // server and clients don't overlap.
            persistentSerialNumber = start;
            transientSerialNumber = -start;
        }

        public virtual string Name {
            get { return "Simple Serial OID Generator"; }
        }

        #region IOidGenerator Members

        public virtual void ConvertPersistentToTransientOid(IOid oid) {
            throw new UnexpectedCallException();
        }

        public virtual void ConvertTransientToPersistentOid(IOid oid) {
            Assert.AssertTrue(oid is SerialOid);
            var serialOid = (SerialOid) oid;
            serialOid.MakePersistent(persistentSerialNumber++);
        }

        public virtual IOid CreateTransientOid(object obj) {
            return SerialOid.CreateTransient(metamodel, transientSerialNumber++, obj.GetType().FullName);
        }

        public IOid RestoreOid(string[] encodedData) {
            return new SerialOid(metamodel, encodedData);
        }

        public IOid CreateOid(string typeName, object[] keys) {
            return SerialOid.CreateTransient(metamodel, transientSerialNumber++, typeName);
        }

        #endregion

        public void ResetTo(long resetValue) {
            persistentSerialNumber = resetValue;
            transientSerialNumber = -resetValue;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}