// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NakedObjects;
using NakedObjects.Value;

namespace AdventureWorksModel {
    public class ProductPhoto : AWDomainObject {
        private byte[] _LargePhoto = new byte[0];

        private ICollection<ProductProductPhoto> _ProductProductPhoto = new List<ProductProductPhoto>();

        private byte[] _ThumbNailPhoto = new byte[0];

        [Hidden]
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

        [Hidden]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto {
            get { return _ProductProductPhoto; }
            set { _ProductProductPhoto = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}