using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class EmployeeAddressMap : EntityTypeConfiguration<EmployeeAddress>
    {
        public EmployeeAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.EmployeeID, t.AddressID });

            // Properties
            Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("EmployeeAddress", "HumanResources");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Employee);
                //.WithMany(t => t.Addresses)
                //.HasForeignKey(d => d.EmployeeID);
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmployeeAddress> builder)
        {
            builder.HasKey(t => new { t.EmployeeID, t.AddressID });

            // Table & Column Mappings
            builder.ToTable("EmployeeAddress", "HumanResources");
            // builder.Property(t => t.Address).HasColumnName("Address");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
