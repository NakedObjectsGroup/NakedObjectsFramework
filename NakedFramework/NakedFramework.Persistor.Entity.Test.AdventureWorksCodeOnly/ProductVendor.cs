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
    [Table("Purchasing.ProductVendor")]
    public class ProductVendor {
      
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }

      
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VendorID { get; set; }

        public int AverageLeadTime { get; set; }

        [Column(TypeName = "money")]
        public decimal StandardPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal? LastReceiptCost { get; set; }

        public DateTime? LastReceiptDate { get; set; }

        public int MinOrderQty { get; set; }

        public int MaxOrderQty { get; set; }

        public int? OnOrderQty { get; set; }

        [Required]
        [StringLength(3)]
        public string UnitMeasureCode { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }

        public virtual Vendor Vendor { get; set; }
    }
}