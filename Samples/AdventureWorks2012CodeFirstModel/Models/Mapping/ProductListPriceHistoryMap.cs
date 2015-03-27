using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class ProductListPriceHistoryMap : EntityTypeConfiguration<ProductListPriceHistory>
    {
        public ProductListPriceHistoryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.StartDate });

            // Properties
            this.Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ProductListPriceHistory", "Production");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.ListPrice).HasColumnName("ListPrice");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductListPriceHistories)
                .HasForeignKey(d => d.ProductID);

        }
    }
}
