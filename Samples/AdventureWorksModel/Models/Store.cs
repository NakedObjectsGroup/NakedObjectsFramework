using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class Store
    {
        public Store()
        {
            this.StoreContacts = new List<StoreContact>();
        }

        public int CustomerID { get; set; }
        public string Name { get; set; }
        public Nullable<int> SalesPersonID { get; set; }
        public string Demographics { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual SalesPerson SalesPerson { get; set; }
        public virtual ICollection<StoreContact> StoreContacts { get; set; }
    }
}
