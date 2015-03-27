using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class CountryRegionMap : EntityTypeConfiguration<CountryRegion>
    {
        public CountryRegionMap()
        {
            // Primary Key
            this.HasKey(t => t.CountryRegionCode);

            // Properties
            this.Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CountryRegion", "Person");
            this.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
