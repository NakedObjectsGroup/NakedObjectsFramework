using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductCostHistory> builder)
        {
            builder.HasKey(t => new { t.ProductID, t.StartDate });

            // Properties
            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("ProductCostHistory", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.StandardCost).HasColumnName("StandardCost");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
