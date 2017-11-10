using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductCostHistoryMap : EntityTypeConfiguration<ProductCostHistory>
    {
        public ProductCostHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.StartDate });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductCostHistory", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.StandardCost).HasColumnName("StandardCost");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }
}
