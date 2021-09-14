using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class SalesOrderDetailMap : EntityTypeConfiguration<SalesOrderDetail> {
        public SalesOrderDetailMap() {
            // Primary Key
            HasKey(t => new { t.SalesOrderID, t.SalesOrderDetailID });

            //Ignores
            Ignore(t => t.Product);

            // Properties
            Property(t => t.SalesOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesOrderDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CarrierTrackingNumber)
                .HasMaxLength(25);

            // Table & Column Mappings
            ToTable("SalesOrderDetail", "Sales");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID");
            Property(t => t.CarrierTrackingNumber).HasColumnName("CarrierTrackingNumber");
            Property(t => t.OrderQty).HasColumnName("OrderQty");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            Property(t => t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount");
            Property(t => t.LineTotal).HasColumnName("LineTotal").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.SalesOrderHeader)
                .WithMany(t => t.Details)
                .HasForeignKey(d => d.SalesOrderID);
            HasRequired(t => t.SpecialOfferProduct).WithMany().HasForeignKey(t => new { t.SpecialOfferID, t.ProductID });
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<SalesOrderDetail> builder) {
            builder.HasKey(t => new { t.SalesOrderID, t.SalesOrderDetailID });

            //Ignores
            builder.Ignore(t => t.Product);

            // Properties
            builder.Property(t => t.SalesOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesOrderDetailID)
                   .ValueGeneratedOnAdd();

            builder.Property(t => t.CarrierTrackingNumber)
                   .HasMaxLength(25);

            // Table & Column Mappings
            builder.ToTable("SalesOrderDetail", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID");
            builder.Property(t => t.CarrierTrackingNumber).HasColumnName("CarrierTrackingNumber");
            builder.Property(t => t.OrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            builder.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            builder.Property(t => t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount");
            builder.Property(t => t.LineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.SalesOrderHeader)
                   .WithMany(t => t.Details)
                   .HasForeignKey(d => d.SalesOrderID);
            builder.HasOne(t => t.SpecialOfferProduct).WithMany().HasForeignKey(t => new { t.SpecialOfferID, t.ProductID });
        }
    }
}