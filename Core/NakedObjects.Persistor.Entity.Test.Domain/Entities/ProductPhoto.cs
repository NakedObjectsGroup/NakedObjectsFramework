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

    public partial class ProductPhoto {
        #region Primitive Properties

        #region ProductPhotoID (Int32)

        [MemberOrder(100)]
        public virtual int ProductPhotoID { get; set; }

        #endregion

        #region ThumbNailPhoto (Binary)

        [MemberOrder(110), Optionally]
        public virtual byte[] ThumbNailPhoto { get; set; }

        #endregion

        #region ThumbnailPhotoFileName (String)

        [MemberOrder(120), Optionally, StringLength(50)]
        public virtual string ThumbnailPhotoFileName { get; set; }

        #endregion

        #region LargePhoto (Binary)

        [MemberOrder(130), Optionally]
        public virtual byte[] LargePhoto { get; set; }

        #endregion

        #region LargePhotoFileName (String)

        [MemberOrder(140), Optionally, StringLength(50)]
        public virtual string LargePhotoFileName { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ProductProductPhotoes (Collection of ProductProductPhoto)

        private ICollection<ProductProductPhoto> _productProductPhotoes = new List<ProductProductPhoto>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes {
            get { return _productProductPhotoes; }
            set { _productProductPhotoes = value; }
        }

        #endregion

        #endregion
    }
}