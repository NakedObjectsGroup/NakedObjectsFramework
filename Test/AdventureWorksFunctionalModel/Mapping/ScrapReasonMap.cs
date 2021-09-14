using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class ScrapReasonMap : EntityTypeConfiguration<ScrapReason> {
        public ScrapReasonMap() {
            // Primary Key
            HasKey(t => t.ScrapReasonID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ScrapReason", "Production");
            Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<ScrapReason> builder) {
            builder.HasKey(t => t.ScrapReasonID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ScrapReason", "Production");
            builder.Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }
}