using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class StateProvinceMap : EntityTypeConfiguration<StateProvince>
    {
        public StateProvinceMap()
        {
            // Primary Key
            this.HasKey(t => t.StateProvinceID);

            // Properties
            this.Property(t => t.StateProvinceCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("StateProvince", "Person");
            this.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            this.Property(t => t.StateProvinceCode).HasColumnName("StateProvinceCode");
            this.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            this.Property(t => t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.CountryRegion)
                .WithMany(t => t.StateProvinces)
                .HasForeignKey(d => d.CountryRegionCode);
            this.HasRequired(t => t.SalesTerritory)
                .WithMany(t => t.StateProvinces)
                .HasForeignKey(d => d.TerritoryID);

        }
    }
}
