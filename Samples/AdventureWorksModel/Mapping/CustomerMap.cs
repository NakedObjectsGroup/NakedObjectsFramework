using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Properties
            Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed); 

            Property(t => t.CustomerType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            ToTable("Customer", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("CustomerID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.CustomerType).HasColumnName("CustomerType");
            Property(t => t.CustomerRowguid).HasColumnName("rowguid");
          Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate");

          // Relationships
          HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
