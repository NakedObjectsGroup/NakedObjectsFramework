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
using NakedObjects.Value;

namespace AdventureWorksModel {
    public class ProductPhoto  {

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        public string Title()
        {
            return "Product Photo";
        }

        private byte[] _LargePhoto = new byte[0];
        private ICollection<ProductProductPhoto> _ProductProductPhoto = new List<ProductProductPhoto>();
        private byte[] _ThumbNailPhoto = new byte[0];

        [NakedObjectsIgnore]
        public virtual int ProductPhotoID { get; set; }

        public virtual byte[] ThumbNailPhoto {
            get { return _ThumbNailPhoto; }
            set { _ThumbNailPhoto = value; }
        }

        public virtual string ThumbnailPhotoFileName { get; set; }
        public virtual byte[] LargePhoto {
            get { return _LargePhoto; }
            set { _LargePhoto = value; }
        }

        public virtual string LargePhotoFileName { get; set; }
        public virtual FileAttachment LargePhotoAsAttachment
        {
            get
            {
                // fake minetype
                return new FileAttachment(LargePhoto, LargePhotoFileName, "text/plain") { DispositionType = "inline" };
            }
        }

        [NakedObjectsIgnore]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto {
            get { return _ProductProductPhoto; }
            set { _ProductProductPhoto = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}