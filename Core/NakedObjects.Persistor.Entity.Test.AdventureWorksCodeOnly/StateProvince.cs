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
    [Table("Person.StateProvince")]
    public class StateProvince {
        public int StateProvinceID { get; set; }

        [Required]
        [StringLength(3)]
        public string StateProvinceCode { get; set; }

        [Required]
        [StringLength(3)]
        public string CountryRegionCode { get; set; }

        public bool IsOnlyStateProvinceFlag { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int TerritoryID { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();

        public virtual CountryRegion CountryRegion { get; set; }

        public virtual ICollection<SalesTaxRate> SalesTaxRates { get; set; } = new HashSet<SalesTaxRate>();

        public virtual SalesTerritory SalesTerritory { get; set; }
    }
}