using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerId);

            // Properties
            this.Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.CustomerType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Customer", "Sales");
            this.Property(t => t.CustomerId).HasColumnName("CustomerID");
            this.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            this.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            this.Property(t => t.CustomerType).HasColumnName("CustomerType");
            this.Property(t => t.CustomerRowguid).HasColumnName("rowguid");
          this.Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate");

          // Relationships
          this.HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
