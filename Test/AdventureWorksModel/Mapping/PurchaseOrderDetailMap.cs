using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class PurchaseOrderDetailMap : EntityTypeConfiguration<PurchaseOrderDetail>
    {
        public PurchaseOrderDetailMap()
        {
            // Primary Key
            HasKey(t => new { t.PurchaseOrderID, t.PurchaseOrderDetailID });

            // Properties
            Property(t => t.PurchaseOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.PurchaseOrderDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            ToTable("PurchaseOrderDetail", "Purchasing");
            Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID");
            Property(t => t.DueDate).HasColumnName("DueDate");
            Property(t => t.OrderQty).HasColumnName("OrderQty");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            Property(t => t.LineTotal).HasColumnName("LineTotal").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            Property(t => t.StockedQty).HasColumnName("StockedQty").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            HasRequired(t => t.PurchaseOrderHeader)
                .WithMany(t => t.Details)
                .HasForeignKey(d => d.PurchaseOrderID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PurchaseOrderDetail> builder)
        {
            builder.HasKey(t => new { t.PurchaseOrderID, t.PurchaseOrderDetailID });

            // Properties
            builder.Property(t => t.PurchaseOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.PurchaseOrderDetailID)
                   .ValueGeneratedOnAdd();

            // Table & Column Mappings
            builder.ToTable("PurchaseOrderDetail", "Purchasing");
            builder.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            builder.Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID");
            builder.Property(t => t.DueDate).HasColumnName("DueDate");
            builder.Property(t => t.OrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            builder.Property(t => t.LineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            builder.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            builder.Property(t => t.StockedQty).HasColumnName("StockedQty").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.PurchaseOrderHeader)
                   .WithMany(t => t.Details)
                   .HasForeignKey(d => d.PurchaseOrderID);
        }
    }
}
