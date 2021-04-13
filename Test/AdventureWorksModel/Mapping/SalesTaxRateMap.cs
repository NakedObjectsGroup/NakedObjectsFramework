using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class SalesTaxRateMap : EntityTypeConfiguration<SalesTaxRate>
    {
        public SalesTaxRateMap()
        {
            // Primary Key
            HasKey(t => t.SalesTaxRateID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SalesTaxRate", "Sales");
            Property(t => t.SalesTaxRateID).HasColumnName("SalesTaxRateID");
            Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            Property(t => t.TaxType).HasColumnName("TaxType");
            Property(t => t.TaxRate).HasColumnName("TaxRate");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTaxRate> builder)
        {
            builder.HasKey(t => t.SalesTaxRateID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesTaxRate", "Sales");
            builder.Property(t => t.SalesTaxRateID).HasColumnName("SalesTaxRateID");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.TaxType).HasColumnName("TaxType");
            builder.Property(t => t.TaxRate).HasColumnName("TaxRate");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
        }
    }
}
