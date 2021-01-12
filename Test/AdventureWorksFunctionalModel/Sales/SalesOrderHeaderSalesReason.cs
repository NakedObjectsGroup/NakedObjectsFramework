// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
    [Named("Reason")]
        public record SalesOrderHeaderSalesReason {

        [Hidden]
        public virtual int SalesOrderID { get; init; }

        public virtual int SalesReasonID { get; init; }
        public virtual SalesOrderHeader SalesOrderHeader { get; init; }
        public virtual SalesReason SalesReason { get; init; }

        #region ModifiedDate

        [MemberOrder(99)]
        
        
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        #endregion

        public override string ToString() => $"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}";

		public override int GetHashCode() => HashCode(this, SalesOrderID, SalesReasonID); 
    }
}