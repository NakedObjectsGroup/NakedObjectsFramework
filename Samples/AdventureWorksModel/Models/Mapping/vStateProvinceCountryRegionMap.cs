using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class vStateProvinceCountryRegionMap : EntityTypeConfiguration<vStateProvinceCountryRegion>
    {
        public vStateProvinceCountryRegionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.StateProvinceID, t.StateProvinceCode, t.IsOnlyStateProvinceFlag, t.StateProvinceName, t.TerritoryID, t.CountryRegionCode, t.CountryRegionName });

            // Properties
            this.Property(t => t.StateProvinceID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.StateProvinceCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.StateProvinceName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TerritoryID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.CountryRegionName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("vStateProvinceCountryRegion", "Person");
            this.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            this.Property(t => t.StateProvinceCode).HasColumnName("StateProvinceCode");
            this.Property(t => t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            this.Property(t => t.StateProvinceName).HasColumnName("StateProvinceName");
            this.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            this.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            this.Property(t => t.CountryRegionName).HasColumnName("CountryRegionName");
        }
    }
}
