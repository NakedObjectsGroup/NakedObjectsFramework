// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Transaction;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Transaction {
    public sealed class NestedTransaction : ITransaction {
        private readonly ILogger<NestedTransaction> logger;
        private readonly IObjectStore objectStore;
        private bool complete;

        public NestedTransaction(IObjectStore objectStore, ILogger<NestedTransaction> logger) {
            this.objectStore = objectStore;
            this.logger = logger;
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("complete", complete);
            return str.ToString();
        }

        #region ITransaction Members

        public void Abort() {
            if (complete) {
                throw new TransactionException(logger.LogAndReturn("Transaction already complete; cannot abort"));
            }

            complete = true;
        }

        public void Commit() {
            if (complete) {
                throw new TransactionException(logger.LogAndReturn("Transaction already complete; cannot commit"));
            }

            objectStore.EndTransaction();
            complete = true;
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}