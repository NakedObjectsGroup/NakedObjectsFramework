using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            HasKey(t => t.CustomerID);

            //Ignores
            Ignore(t => t.CustomerType);

            // Properties
            Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(t => t.StoreID).IsOptional();
            Property(t => t.PersonID).IsOptional();
            Property(t => t.SalesTerritoryID).IsOptional();

            // Table & Column Mappings
            ToTable("Customer", "Sales");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.StoreID).HasColumnName("StoreID");
            Property(t => t.PersonID).HasColumnName("PersonID");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.CustomerRowguid).HasColumnName("rowguid");
          Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate");

          // Relationships
          HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
          HasOptional(t => t.Store).WithMany().HasForeignKey(t => t.StoreID);
          HasOptional(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
        }
    }
}
