using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductDescription> builder)
        {
            builder.HasKey(t => t.ProductDescriptionID);

            // Properties
            builder.Property(t => t.mappedDescription)
                   .IsRequired()
                   .HasMaxLength(400);

            // Table & Column Mappings
            builder.ToTable("ProductDescription", "Production");
            builder.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            builder.Property(t => t.mappedDescription).HasColumnName("Description");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
