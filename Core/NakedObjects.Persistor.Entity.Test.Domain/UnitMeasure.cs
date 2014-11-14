// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class UnitMeasure {
        #region Primitive Properties

        #region UnitMeasureCode (String)

        [MemberOrder(100), StringLength(3)]
        public virtual string UnitMeasureCode { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region BillOfMaterials (Collection of BillOfMaterial)

        private ICollection<BillOfMaterial> _billOfMaterials = new List<BillOfMaterial>();

        [MemberOrder(130), Disabled]
        public virtual ICollection<BillOfMaterial> BillOfMaterials {
            get { return _billOfMaterials; }
            set { _billOfMaterials = value; }
        }

        #endregion

        #region Products (Collection of Product)

        private ICollection<Product> _products = new List<Product>();

        [MemberOrder(140), Disabled]
        public virtual ICollection<Product> Products {
            get { return _products; }
            set { _products = value; }
        }

        #endregion

        #region Products1 (Collection of Product)

        private ICollection<Product> _products1 = new List<Product>();

        [MemberOrder(150), Disabled]
        public virtual ICollection<Product> Products1 {
            get { return _products1; }
            set { _products1 = value; }
        }

        #endregion

        #region ProductVendors (Collection of ProductVendor)

        private ICollection<ProductVendor> _productVendors = new List<ProductVendor>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<ProductVendor> ProductVendors {
            get { return _productVendors; }
            set { _productVendors = value; }
        }

        #endregion

        #endregion
    }
}