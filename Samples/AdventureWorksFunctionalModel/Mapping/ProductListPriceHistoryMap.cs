using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductListPriceHistoryMap : EntityTypeConfiguration<ProductListPriceHistory>
    {
        public ProductListPriceHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.StartDate });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductListPriceHistory", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.ListPrice).HasColumnName("ListPrice");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }
}
