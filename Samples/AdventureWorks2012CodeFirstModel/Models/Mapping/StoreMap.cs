using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerID);

            // Properties
            this.Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Store", "Sales");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            this.Property(t => t.Demographics).HasColumnName("Demographics");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Customer)
                .WithOptional(t => t.Store);
            this.HasOptional(t => t.SalesPerson)
                .WithMany(t => t.Stores)
                .HasForeignKey(d => d.SalesPersonID);

        }
    }
}
