






using System;
using System.Collections.Generic;
using NakedFramework.Value;
using NakedFunctions;

namespace AW.Types {
    public class ProductPhoto {

        public ProductPhoto() { }

        public ProductPhoto(ProductPhoto cloneFrom)
        {
            ProductPhotoID = cloneFrom.ProductPhotoID;
            ThumbNailPhoto = cloneFrom.ThumbNailPhoto;
            ThumbnailPhotoFileName = cloneFrom.ThumbnailPhotoFileName;
            LargePhoto = cloneFrom.LargePhoto;
            LargePhotoFileName = cloneFrom.LargePhotoFileName;
            ProductProductPhoto = cloneFrom.ProductProductPhoto;
            ModifiedDate = cloneFrom.ModifiedDate;
        }

        [Hidden]
        public int ProductPhotoID { get; init; }

        public byte[] ThumbNailPhoto { get; set; } = new byte[0];

        public string? ThumbnailPhotoFileName { get; init; }

        public byte[] LargePhoto { get; set; } = new byte[0];

        public string? LargePhotoFileName { get; init; }

        public virtual FileAttachment LargePhotoAsAttachment =>
            new(LargePhoto, LargePhotoFileName, "text/plain") { DispositionType = "inline" };// fake mimetype

        [Hidden]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; init; } = new List<ProductProductPhoto>();

        [MemberOrder(99), Versioned]
        public DateTime ModifiedDate { get; init; }
      
        public override string ToString() => $"Product Photo: {ProductPhotoID}";
    }
}