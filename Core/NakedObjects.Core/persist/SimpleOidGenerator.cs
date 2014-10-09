// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Persist {
    /// <summary>
    ///     Generates OIDs based on the system clock
    /// </summary>
    public class SimpleOidGenerator : IOidGenerator {
        private readonly IMetamodel metamodel;
        private readonly long start;
        private long persistentSerialNumber;
        private long transientSerialNumber;

        public SimpleOidGenerator(IMetamodel metamodel)
            : this(metamodel, 0L) {}

        public SimpleOidGenerator(IMetamodel metamodel, long start) {
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

        public void ResetTo(long resetValue) {
            persistentSerialNumber = resetValue;
            transientSerialNumber = -resetValue;
        }

        #region IOidGenerator Members

       
        public virtual void ConvertPersistentToTransientOid(IOid oid) {
            throw new UnexpectedCallException();
        }

        public virtual void ConvertTransientToPersistentOid(IOid oid) {
            lock (this) {
                Assert.AssertTrue(oid is SerialOid);
                var serialOid = (SerialOid) oid;
                serialOid.MakePersistent(persistentSerialNumber++);
            }
        }

        public virtual IOid CreateTransientOid(object obj) {
            lock (this) {
                return SerialOid.CreateTransient(metamodel, transientSerialNumber++, obj.GetType().FullName);
            }
        }

        public IOid RestoreOid(ILifecycleManager persistor, string[] encodedData) {
            return persistor.RestoreGenericOid(encodedData) ?? new SerialOid(metamodel, encodedData);
        }

        public IOid CreateOid(string typeName, object[] keys) {
            lock (this) {
                return SerialOid.CreateTransient(metamodel, transientSerialNumber++, typeName);
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}