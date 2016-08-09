using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class StateProvinceMap : EntityTypeConfiguration<StateProvince>
    {
        public StateProvinceMap()
        {
            // Primary Key
            HasKey(t => t.StateProvinceID);

            // Properties
            Property(t => t.StateProvinceCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("StateProvince", "Person");
            Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            Property(t => t.StateProvinceCode).HasColumnName("StateProvinceCode");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            HasRequired(t => t.SalesTerritory)
                .WithMany(t => t.StateProvince)
                .HasForeignKey(d => d.TerritoryID);

        }
    }
}
