using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CountryRegionCurrencyMap : EntityTypeConfiguration<CountryRegionCurrency>
    {
        public CountryRegionCurrencyMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CountryRegionCode, t.CurrencyCode });

            // Properties
            this.Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.CurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("CountryRegionCurrency", "Sales");
            this.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            this.Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);;
            this.HasRequired(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyCode); ;

        }
    }
}
