using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CountryRegionMap : EntityTypeConfiguration<CountryRegion>
    {
        public CountryRegionMap()
        {
            // Primary Key
            HasKey(t => t.CountryRegionCode);

            // Properties
            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("CountryRegion", "Person");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
