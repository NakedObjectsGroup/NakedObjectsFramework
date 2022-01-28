using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductCostHistory> builder)
        {
            builder.HasKey(t => new { t.ProductID, t.mappedStartDate });

            // Properties
            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("ProductCostHistory", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.mappedStandardCost).HasColumnName("StandardCost");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
