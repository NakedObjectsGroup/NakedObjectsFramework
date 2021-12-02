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

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly;

[Table("Production.ProductInventory")]
public class ProductInventory {
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ProductID { get; set; }

    [Column(Order = 1)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public short LocationID { get; set; }

    [Required]
    [StringLength(10)]
    public string Shelf { get; set; }

    public byte Bin { get; set; }

    public short Quantity { get; set; }

    public Guid rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual Location Location { get; set; }

    public virtual Product Product { get; set; }
}