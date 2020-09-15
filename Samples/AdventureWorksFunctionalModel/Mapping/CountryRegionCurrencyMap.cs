using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CountryRegionCurrencyMap : EntityTypeConfiguration<CountryRegionCurrency>
    {
        public CountryRegionCurrencyMap()
        {
            // Primary Key
            HasKey(t => new { t.CountryRegionCode, t.CurrencyCode });

            // Properties
            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.CurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            ToTable("CountryRegionCurrency", "Sales");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);;
            HasRequired(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyCode); ;

        }
    }
}
