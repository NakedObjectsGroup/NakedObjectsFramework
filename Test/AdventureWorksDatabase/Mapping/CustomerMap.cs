using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(t => t.CustomerID);

            //builder.Ignore(t => t.CustomerType);

            // Properties
            builder.Property(t => t.mappedAccountNumber)
                   .IsRequired()
                   .HasMaxLength(10)
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.StoreID);//.IsOptional();
            builder.Property(t => t.PersonID);//.IsOptional();
            builder.Property(t => t.SalesTerritoryID);//.IsOptional();

            // Table & Column Mappings
            builder.ToTable("Customer", "Sales");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.StoreID).HasColumnName("StoreID");
            builder.Property(t => t.PersonID).HasColumnName("PersonID");
            builder.Property(t => t.mappedAccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.CustomerRowguid).HasColumnName("rowguid");
            builder.Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
            builder.HasOne(t => t.Store).WithMany().HasForeignKey(t => t.StoreID);
            builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
        }
    }
}
