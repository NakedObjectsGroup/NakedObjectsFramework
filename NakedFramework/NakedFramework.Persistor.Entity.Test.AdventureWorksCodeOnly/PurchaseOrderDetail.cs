// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Purchasing.PurchaseOrderDetail")]
    public class PurchaseOrderDetail {
     
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PurchaseOrderID { get; set; }

     
        [Column(Order = 1)]
        public int PurchaseOrderDetailID { get; set; }

        public DateTime DueDate { get; set; }

        public short OrderQty { get; set; }

        public int ProductID { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal LineTotal { get; set; }

        public decimal ReceivedQty { get; set; }

        public decimal RejectedQty { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal StockedQty { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }
    }
}