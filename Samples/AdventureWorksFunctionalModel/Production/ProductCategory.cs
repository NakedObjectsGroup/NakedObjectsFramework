// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AdventureWorksModel {
    [Bounded]
        public record ProductCategory: IHasRowGuid, IHasModifiedDate  {

        public ProductCategory(
            int productCategoryID,
            string name,
            ICollection<ProductSubcategory> productSubcategory,
             Guid rowguid,
             DateTime modifiedDate
            )
        {
            ProductCategoryID = productCategoryID;
            Name = name;
            ProductSubcategory = productSubcategory;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public ProductCategory() {

        }

        [Hidden]
        public virtual int ProductCategoryID { get; set; }

        public virtual string Name { get; set; }

        [Named("Subcategories")]
        [TableView(true)] //TableView == ListView
        public virtual ICollection<ProductSubcategory> ProductSubcategory { get; set; } = new List<ProductSubcategory>();

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }

    public static class ProductCategoryFunctions
    {

        public static ProductCategory Updating(ProductCategory a, [Injected] DateTime now)
        {
            return a with {ModifiedDate =  now};
        }

    }
}