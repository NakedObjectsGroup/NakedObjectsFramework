using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesTerritoryMap : EntityTypeConfiguration<SalesTerritory>
    {
        public SalesTerritoryMap()
        {
            // Primary Key
            HasKey(t => t.TerritoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Group)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SalesTerritory", "Sales");
            Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.Group).HasColumnName("Group");
            Property(t => t.SalesYTD).HasColumnName("SalesYTD");
            Property(t => t.SalesLastYear).HasColumnName("SalesLastYear");
            Property(t => t.CostYTD).HasColumnName("CostYTD");
            Property(t => t.CostLastYear).HasColumnName("CostLastYear");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
