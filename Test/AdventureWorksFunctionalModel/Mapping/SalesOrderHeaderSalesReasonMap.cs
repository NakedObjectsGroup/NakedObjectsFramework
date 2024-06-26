using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class SalesOrderHeaderSalesReasonMap : EntityTypeConfiguration<SalesOrderHeaderSalesReason> {
        public SalesOrderHeaderSalesReasonMap() {
            // Primary Key
            HasKey(t => new { t.SalesOrderID, t.SalesReasonID });

            // Properties
            Property(t => t.SalesOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesReasonID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesOrderHeaderSalesReason", "Sales");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.SalesOrderHeader)
                .WithMany(t => t.SalesOrderHeaderSalesReason)
                .HasForeignKey(d => d.SalesOrderID);
            HasRequired(t => t.SalesReason).WithMany().HasForeignKey(t => t.SalesReasonID);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<SalesOrderHeaderSalesReason> builder) {
            builder.HasKey(t => new { t.SalesOrderID, t.SalesReasonID });

            // Properties
            builder.Property(t => t.SalesOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesReasonID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesOrderHeaderSalesReason", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.SalesOrderHeader)
                   .WithMany(t => t.SalesOrderHeaderSalesReason)
                   .HasForeignKey(d => d.SalesOrderID);
            builder.HasOne(t => t.SalesReason).WithMany().HasForeignKey(t => t.SalesReasonID);
        }
    }
}