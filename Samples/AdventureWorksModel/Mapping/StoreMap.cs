using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Store", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            Property(t => t.Demographics).HasColumnName("Demographics");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasOptional(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
        }
    }
}
