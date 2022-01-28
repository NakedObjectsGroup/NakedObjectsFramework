using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesReason> builder)
        {
            builder.HasKey(t => t.SalesReasonID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedReasonType)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesReason", "Sales");
            builder.Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedReasonType).HasColumnName("ReasonType");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
