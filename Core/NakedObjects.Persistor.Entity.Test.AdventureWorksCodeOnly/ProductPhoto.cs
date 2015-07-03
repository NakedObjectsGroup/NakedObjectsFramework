namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.ProductPhoto")]
    public partial class ProductPhoto
    {
        public ProductPhoto()
        {
            ProductProductPhotoes = new HashSet<ProductProductPhoto>();
        }

        public int ProductPhotoID { get; set; }

        public byte[] ThumbNailPhoto { get; set; }

        [StringLength(50)]
        public string ThumbnailPhotoFileName { get; set; }

        public byte[] LargePhoto { get; set; }

        [StringLength(50)]
        public string LargePhotoFileName { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes { get; set; }
    }
}
