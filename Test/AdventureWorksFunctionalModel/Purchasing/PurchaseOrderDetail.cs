// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using static AW.Helpers;

namespace AW.Types {
        public record PurchaseOrderDetail {

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion
        [Hidden]
        public virtual int PurchaseOrderID { get; set; }

        [Hidden]
        public virtual int PurchaseOrderDetailID { get; set; }

        [MemberOrder(26)]
        [Mask("d")]
        public virtual DateTime DueDate { get; set; }

        [MemberOrder(20)]
        public virtual short OrderQty { get; set; }

        [MemberOrder(22)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; set; }

        [MemberOrder(24)]
        [Mask("C")]
        
        public virtual decimal LineTotal { get; set; }

        [Mask("#")]
        [MemberOrder(30)]
        public virtual decimal ReceivedQty { get; set; }

        [Mask("#")]
        [MemberOrder(32)]
        public virtual decimal RejectedQty { get; set; }

        [Mask("#")]
        [MemberOrder(34)]
        
        public virtual decimal StockedQty { get; set; }

        [MemberOrder(99),ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [Hidden]
        public virtual int ProductID { get; set; }

        
        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        [Hidden]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }

        public override string ToString() => $"{OrderQty} x {Product}";
    }
}