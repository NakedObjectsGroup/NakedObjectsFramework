// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Sales.SalesOrderHeader")]
    public class SalesOrderHeader {
        [Key]
        public int SalesOrderID { get; set; }

        public byte RevisionNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ShipDate { get; set; }

        public byte Status { get; set; }

        public bool OnlineOrderFlag { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Required]
        [StringLength(25)]
        public string SalesOrderNumber { get; set; }

        [StringLength(25)]
        public string PurchaseOrderNumber { get; set; }

        [StringLength(15)]
        public string AccountNumber { get; set; }

        public int CustomerID { get; set; }

        public int ContactID { get; set; }

        public int? SalesPersonID { get; set; }

        public int BillToAddressID { get; set; }

        public int ShipToAddressID { get; set; }

        public int ShipMethodID { get; set; }

        public int? CreditCardID { get; set; }

        [StringLength(15)]
        public string CreditCardApprovalCode { get; set; }

        public int? CurrencyRateID { get; set; }

        [Column(TypeName = "money")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "money")]
        public decimal TaxAmt { get; set; }

        [Column(TypeName = "money")]
        public decimal Freight { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalDue { get; set; }

        [StringLength(128)]
        public string Comment { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Address Address { get; set; }

        public virtual Address Address1 { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ShipMethod ShipMethod { get; set; }

        public virtual CreditCard CreditCard { get; set; }

        public virtual CurrencyRate CurrencyRate { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new HashSet<SalesOrderDetail>();

        public virtual SalesPerson SalesPerson { get; set; }

        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons { get; set; } = new HashSet<SalesOrderHeaderSalesReason>();
    }
}