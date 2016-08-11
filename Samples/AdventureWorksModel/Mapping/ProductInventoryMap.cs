using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductInventoryMap : EntityTypeConfiguration<ProductInventory>
    {
        public ProductInventoryMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.LocationID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LocationID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Shelf)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("ProductInventory", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.LocationID).HasColumnName("LocationID");
            Property(t => t.Shelf).HasColumnName("Shelf");
            Property(t => t.Bin).HasColumnName("Bin");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            HasRequired(t => t.Product)
                .WithMany(t => t.ProductInventory)
                .HasForeignKey(d => d.ProductID);

        }
    }
}
