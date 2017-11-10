using System.Data.Entity.ModelConfiguration;

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
}
