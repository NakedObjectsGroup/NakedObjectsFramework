// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;

namespace AW.Types {
        public record PurchaseOrderDetail {

        [Hidden]
        public virtual int PurchaseOrderID { get; init; }

        [Hidden]
        public virtual int PurchaseOrderDetailID { get; init; }

        [MemberOrder(26)]
        [Mask("d")]
        public virtual DateTime DueDate { get; init; }

        [MemberOrder(20)]
        public virtual short OrderQty { get; init; }

        [MemberOrder(22)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; init; }

        [MemberOrder(24)]
        [Mask("C")]
        
        public virtual decimal LineTotal { get; init; }

        [Mask("#")]
        [MemberOrder(30)]
        public virtual decimal ReceivedQty { get; init; }

        [Mask("#")]
        [MemberOrder(32)]
        public virtual decimal RejectedQty { get; init; }

        [Mask("#")]
        [MemberOrder(34)]
        
        public virtual decimal StockedQty { get; init; }

        [MemberOrder(99)]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual int ProductID { get; init; }

        
        [MemberOrder(10)]
        public virtual Product Product { get; init; }

        [Hidden]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; init; }

        public override string ToString() => $"{OrderQty} x {Product}";
    }
}