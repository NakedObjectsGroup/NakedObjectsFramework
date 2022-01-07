using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTaxRate> builder)
        {
            builder.HasKey(t => t.SalesTaxRateID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesTaxRate", "Sales");
            builder.Property(t => t.SalesTaxRateID).HasColumnName("SalesTaxRateID");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.mappedTaxType).HasColumnName("TaxType");
            builder.Property(t => t.mappedTaxRate).HasColumnName("TaxRate");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
        }
    }
}
