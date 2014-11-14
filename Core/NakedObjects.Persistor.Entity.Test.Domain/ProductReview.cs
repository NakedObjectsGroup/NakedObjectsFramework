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
    public partial class ProductReview {
        #region Primitive Properties

        #region ProductReviewID (Int32)

        [MemberOrder(100)]
        public virtual int ProductReviewID { get; set; }

        #endregion

        #region ReviewerName (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string ReviewerName { get; set; }

        #endregion

        #region ReviewDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ReviewDate { get; set; }

        #endregion

        #region EmailAddress (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string EmailAddress { get; set; }

        #endregion

        #region Rating (Int32)

        [MemberOrder(140)]
        public virtual int Rating { get; set; }

        #endregion

        #region Comments (String)

        [MemberOrder(150), Optionally, StringLength(3850)]
        public virtual string Comments { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(160), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(170)]
        public virtual Product Product { get; set; }

        #endregion

        #endregion
    }
}