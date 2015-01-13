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
    // ReSharper disable once PartialTypeWithSinglePart

    public partial class ProductCategory {
        #region Primitive Properties

        #region ProductCategoryID (Int32)

        [MemberOrder(100)]
        public virtual int ProductCategoryID { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(120)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ProductSubcategories (Collection of ProductSubcategory)

        private ICollection<ProductSubcategory> _productSubcategories = new List<ProductSubcategory>();

        [MemberOrder(140), Disabled]
        public virtual ICollection<ProductSubcategory> ProductSubcategories {
            get { return _productSubcategories; }
            set { _productSubcategories = value; }
        }

        #endregion

        #endregion
    }
}