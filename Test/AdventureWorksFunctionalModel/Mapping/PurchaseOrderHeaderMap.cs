using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class PurchaseOrderHeaderMap : EntityTypeConfiguration<PurchaseOrderHeader>
    {
        public PurchaseOrderHeaderMap()
        {
            // Primary Key
            HasKey(t => t.PurchaseOrderID);

            // Properties
            // Table & Column Mappings
            ToTable("PurchaseOrderHeader", "Purchasing");
            Property(t => t.PurchaseOrderID).HasColumnName("PurchaseOrderID");
            Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.OrderPlacedByID).HasColumnName("EmployeeID");
            Property(t => t.VendorID).HasColumnName("VendorID");
            Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.ShipDate).HasColumnName("ShipDate");
            Property(t => t.SubTotal).HasColumnName("SubTotal");
            Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            Property(t => t.Freight).HasColumnName("Freight");
            Property(t => t.TotalDue).HasColumnName("TotalDue").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed);
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.OrderPlacedBy).WithMany().HasForeignKey(t => t.OrderPlacedByID);
            HasRequired(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            HasRequired(t => t.Vendor).WithMany().HasForeignKey(t => t.VendorID);

        }
    }
}
