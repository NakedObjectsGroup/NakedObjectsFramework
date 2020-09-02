// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;
using NakedObjects.Menu;

namespace AdventureWorksModel {
    [IconName("cartons.png")]
    public class ProductInventory : IHasRowGuid, IHasModifiedDate {

        public ProductInventory(
            int productID,
            short locationID,
            string shelf,
            byte bin,
            short quantity,
            Location location,
            Product product,
            Guid rowguid,
            DateTime modifiedDate
            )
        {
            ProductID = productID;
            LocationID = locationID;
            Shelf = shelf;
            Bin = bin;
            Quantity = quantity;
            Location = location;
            Product = product;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public ProductInventory() { }

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [NakedObjectsIgnore]
        public virtual short LocationID { get; set; }

        [MemberOrder(40)]
        public virtual string Shelf { get; set; }

        [MemberOrder(50)]
        public virtual byte Bin { get; set; }

        [MemberOrder(10)]
        public virtual short Quantity { get; set; }

        [MemberOrder(30)]
        public virtual Location Location { get; set; }

        [MemberOrder(20)]
        public virtual Product Product { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

    }
    public static class ProductInventoryFunctions
    {
        public static string Title(this ProductInventory pi)
        {
            return pi.CreateTitle($"{pi.Quantity} in {pi.Location} - {pi.Shelf}");
        }
        public static ProductInventory Updating(ProductInventory a, [Injected] DateTime now)
        {
            return a.With(x => x.ModifiedDate, now);
        }
    }
}