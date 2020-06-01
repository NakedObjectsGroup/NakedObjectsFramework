// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Transaction;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Transaction {
    public sealed class NestedTransaction : ITransaction {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NestedTransaction));
        private readonly IObjectStore objectStore;
        private bool complete;

        public NestedTransaction(IObjectStore objectStore) => this.objectStore = objectStore;

        #region ITransaction Members

        public void Abort() {
            if (complete) {
                throw new TransactionException(Log.LogAndReturn("Transaction already complete; cannot abort"));
            }

            complete = true;
        }

        public void Commit() {
            if (complete) {
                throw new TransactionException(Log.LogAndReturn("Transaction already complete; cannot commit"));
            }

            objectStore.EndTransaction();
            complete = true;
        }

        #endregion

        public override string ToString() {
            var str = new AsString(this);
            str.Append("complete", complete);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}