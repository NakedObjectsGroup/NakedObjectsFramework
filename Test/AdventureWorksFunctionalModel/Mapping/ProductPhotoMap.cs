using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class ProductPhotoMap : EntityTypeConfiguration<ProductPhoto>
    {
        public ProductPhotoMap()
        {
            // Primary Key
            HasKey(t => t.ProductPhotoID);

            // Properties
            Property(t => t.ThumbnailPhotoFileName)
                .HasMaxLength(50);

            //Property(t => t.LargePhotoFileName)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductPhoto", "Production");
            Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            Property(t => t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto");
            Property(t => t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName");
            Property(t => t.LargePhoto).HasColumnName("LargePhoto");
            //Property(t => t.LargePhotoFileName).HasColumnName("LargePhotoFileName");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductPhoto> builder)
        {
            builder.HasKey(t => t.ProductPhotoID);

            // Properties
            builder.Property(t => t.ThumbnailPhotoFileName)
                .HasMaxLength(50);

            //builder.Property(t => t.LargePhotoFileName)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductPhoto", "Production");
            builder.Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            builder.Property(t => t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto");
            builder.Property(t => t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName");
            builder.Property(t => t.LargePhoto).HasColumnName("LargePhoto");
            //builder.Property(t => t.LargePhotoFileName).HasColumnName("LargePhotoFileName");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
