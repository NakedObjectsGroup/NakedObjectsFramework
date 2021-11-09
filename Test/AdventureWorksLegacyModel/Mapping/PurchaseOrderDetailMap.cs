using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PurchaseOrderDetail> builder)
        {
            builder.HasKey(t => new { t.PurchaseOrderID, t.PurchaseOrderDetailID });

            // Properties
            builder.Ignore(t => t.DueDate).Ignore(t => t.OrderQty).Ignore(t => t.ModifiedDate);

            // Table & Column Mappings
            builder.ToTable("PurchaseOrderDetail", "Purchasing");
            builder.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID").ValueGeneratedNever();
            builder.Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID").ValueGeneratedOnAdd();
            builder.Property(t => t.mappedDueDate).HasColumnName("DueDate");
            builder.Property(t => t.mappedOrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            builder.Property(t => t.LineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            builder.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            builder.Property(t => t.StockedQty).HasColumnName("StockedQty").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.PurchaseOrderHeader)
                   .WithMany(t => t.Details)
                   .HasForeignKey(d => d.PurchaseOrderID);
        }
    }
}

