using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class PurchaseOrderDetailMap : EntityTypeConfiguration<PurchaseOrderDetail>
    {
        public PurchaseOrderDetailMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PurchaseOrderID, t.PurchaseOrderDetailID });

            // Properties
            this.Property(t => t.PurchaseOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PurchaseOrderDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("PurchaseOrderDetail", "Purchasing");
            this.Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            this.Property(t => t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID");
            this.Property(t => t.DueDate).HasColumnName("DueDate");
            this.Property(t => t.OrderQty).HasColumnName("OrderQty");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.LineTotal).HasColumnName("LineTotal");
            this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            this.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            this.Property(t => t.StockedQty).HasColumnName("StockedQty");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.PurchaseOrderDetails)
                .HasForeignKey(d => d.ProductID);
            this.HasRequired(t => t.PurchaseOrderHeader)
                .WithMany(t => t.PurchaseOrderDetails)
                .HasForeignKey(d => d.PurchaseOrderID);

        }
    }
}
