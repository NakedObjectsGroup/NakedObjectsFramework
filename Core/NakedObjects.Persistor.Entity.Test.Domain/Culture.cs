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
    // ReSharper disable InconsistentNaming

    public partial class Culture {
        #region Primitive Properties

        #region CultureID (String)

        [MemberOrder(100), StringLength(6)]
        public virtual string CultureID { get; set; }

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

        #region ProductModelProductDescriptionCultures (Collection of ProductModelProductDescriptionCulture)

        private ICollection<ProductModelProductDescriptionCulture> _productModelProductDescriptionCultures = new List<ProductModelProductDescriptionCulture>();

        [MemberOrder(130), Disabled]
        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures {
            get { return _productModelProductDescriptionCultures; }
            set { _productModelProductDescriptionCultures = value; }
        }

        #endregion

        #endregion
    }
}