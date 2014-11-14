// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class ProductModelProductDescriptionCulture {
        #region Primitive Properties

        #region ProductModelID (Int32)

        [MemberOrder(100)]
        public virtual int ProductModelID { get; set; }

        #endregion

        #region ProductDescriptionID (Int32)

        [MemberOrder(110)]
        public virtual int ProductDescriptionID { get; set; }

        #endregion

        #region CultureID (String)

        [MemberOrder(120), StringLength(6)]
        public virtual string CultureID { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Culture (Culture)

        [MemberOrder(140)]
        public virtual Culture Culture { get; set; }

        #endregion

        #region ProductDescription (ProductDescription)

        [MemberOrder(150)]
        public virtual ProductDescription ProductDescription { get; set; }

        #endregion

        #region ProductModel (ProductModel)

        [MemberOrder(160)]
        public virtual ProductModel ProductModel { get; set; }

        #endregion

        #endregion
    }
}